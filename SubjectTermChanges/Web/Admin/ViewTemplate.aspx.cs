using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Xml;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Collections.Generic;
using System.Transactions;
using System.Linq;


namespace Kindred.Knect.ITAT.Web
{
	public partial class ViewTemplate : BasePage
	{

		internal override Control ResizablePanel()
		{
			return null;
		}


		internal override HtmlGenericControl HTMLBody()
		{
			return this.htmlBody;
		}


		protected void Page_Load(object sender, EventArgs e)
		{
            if (!AllowAccess(Common.Names._ASR_SystemAdmin))
			{
                UnauthorizedPageAccess();
			}

			if (!IsPostBack)
			{
				DataSet ds = Business.ITATSystem.GetSystemList();
				Helper.LoadListControl(ddlSystem, ds, "ITATSystemName", "ITATSystemID", "", true, "(Select a System)", "");
				
				ddlType.Items.Add(new ListItem("(Select One)", ""));
				ddlType.Items.Add(new ListItem("Draft Template", "Draft"));
				ddlType.Items.Add(new ListItem("Final Template", "Final"));
                ddlType.Items.Add(new ListItem("New Template", "New"));
                ddlType.Items.Add(new ListItem("Managed Item", "ManagedItem"));

				ShowTemplateOrItem(true);
			}
		}


		private void DownloadXml(string xml)
		{
			//xml = Utility.XMLHelper.FormatXML(xml);

			Response.Clear();
			Response.Buffer = true;
			if (ddlType.SelectedValue == "ManagedItem")
				Response.AddHeader("Content-Disposition", string.Format("attachment; filename={0}-{1} Managed Item Template Def.xml", ddlSystem.SelectedItem.Text, txtManagedItemNumber.Text));
			else
				Response.AddHeader("Content-Disposition", string.Format("attachment; filename={0}-{1} {2} Def.xml", ddlSystem.SelectedItem.Text, ddlTemplate.SelectedItem.Text, ddlType.SelectedItem.Text));
			Response.ContentType = "Text/XML";
			Response.Write(xml);
			Response.End();
		}


		protected void ddlSystemOnSelectedIndexChanged(object sender, EventArgs e)
		{
			ShowTemplateOrItem(true);
		}


		protected void ddlTypeOnSelectedIndexChanged(object sender, EventArgs e)
		{
            ShowTemplateOrItem(true);
		}

        protected void ddlTemplateOnSelectedIndexChanged(object sender, EventArgs e)
        {
            if (ddlType.SelectedValue != "Final")
                ShowTemplateOrItem(false);
        }

		protected void chkIncludeInactiveOnCheckedChanged(object sender, EventArgs e)
		{
            ShowTemplateOrItem(true);
		}

		protected void btnResetOnCommand(object sender, CommandEventArgs e)
		{
			ddlSystem.SelectedIndex = 0;
			ddlType.SelectedIndex = 0;
			ShowTemplateOrItem(true);
		}

		protected void btnOpenOnCommand(object sender, CommandEventArgs e)
		{
			if (ddlTemplate.Visible)
				DownloadTemplateDef();
			else
				DownloadManagedItemTemplateDef();
		}

        protected void btnUploadOnCommand(object sender, CommandEventArgs e)
        {
            string templateName = txtManagedItemNumber.Text;

            if (ddlType.SelectedValue == "New")
            {
                if (string.IsNullOrEmpty(templateName))
                {
                    RegisterAlert("You must supply a template name");
                    return;
                }

                //Check for template name collisions....
                using (DataSet ds = GetTemplateList(true))
                {
                    var templates = from t in ds.Tables[0].AsEnumerable()
                                    select new
                                    {
                                        TemplateName = t.Field<string>("TemplateName")
                                    };

                    if (templates.Select(x => x.TemplateName).ToList().Contains(templateName))
                    {
                        RegisterAlert("You must supply a unique template name");
                        return;
                    }
                }
            }

            if (string.IsNullOrEmpty(filUpload.FileName) || !filUpload.HasFile)
            {
                RegisterAlert("You must supply a file for upload");
                return;
            }

            string xml = null;

            try
            {
                XmlDocument rawXML = new XmlDocument();
                rawXML.Load(filUpload.FileContent);
                xml = rawXML.OuterXml;
            }
            catch
            {
                RegisterAlert("The supplied file is not a properly formatted xml document");
                return;
            }

            if (ddlType.SelectedValue == "Draft")
            {
                ModifyDraftTemplate(templateName, xml);
            }
            else
            {
                if (ddlType.SelectedValue == "New")
                {
                    Guid systemID = new Guid(ddlSystem.SelectedValue);
                    AddNewTemplate(systemID, templateName, xml);
                }
                else
                    RegisterAlert("Option not handled");
            }
        }

        private void ModifyDraftTemplate(string templateName, string xml)
        {
            using (TransactionScope transScope = new TransactionScope(Data.Common.TransactionScopeOption))
            {
                short templateStatus = ddlTemplate.SelectedItem.Text.IndexOf("(Inactive)") > -1 ? (short)Business.TemplateStatusType.Inactive : (short)Business.TemplateStatusType.Active;
                Guid templateGuid = new Guid(ddlTemplate.SelectedValue);
                Data.Template.UpdateDraftTemplateDef(templateGuid, xml);
                Data.Template.InsertTemplateAudit(templateGuid, Business.SecurityHelper.GetLoggedOnPersonId(), templateStatus, xml, (int)Business.Retro.AuditType.Saved);
                transScope.Complete();
            }
        }

        private void AddNewTemplate(Guid systemID, string templateName, string xml)
        {
            if (string.IsNullOrEmpty(templateName))
                RegisterAlert("You must provide a name for the new template");
            else
            {
                using (DataSet ds = Business.Template.GetTemplateList(systemID, null))
                {
                    foreach (DataRow row in ds.Tables[0].Rows)
                    {
                        if (row["TemplateName"].ToString() == templateName)
                        {
                            RegisterAlert("The template name provided is already in use");
                            return;
                        }
                    }
                }
            }

            using (TransactionScope transScope = new TransactionScope(Data.Common.TransactionScopeOption))
            {
                short templateStatus = (short)Business.TemplateStatusType.Inactive;
                Guid templateGuid = Data.Template.InsertTemplate(systemID, templateName);
                Data.Template.UpdateTemplateSummary(templateGuid, templateName, templateStatus, null);
                Data.Template.UpdateDraftTemplateDef(templateGuid, xml);
                Data.Template.InsertTemplateAudit(templateGuid, Business.SecurityHelper.GetLoggedOnPersonId(), templateStatus, xml, (int)Business.Retro.AuditType.Created);
                transScope.Complete();
            }
        }

        private void DownloadTemplateDef()
		{
				if (ddlTemplate.SelectedIndex == 0)
					RegisterAlert("You must select a template first");
				else
				{
					Business.DefType defType = (Business.DefType)Enum.Parse(typeof(Business.DefType), ddlType.SelectedValue);
					DownloadXml(Business.Template.GetTemplateDef(null, new Guid(ddlTemplate.SelectedValue), defType));
				}
		}

		private void DownloadManagedItemTemplateDef()
		{
			string miNumber = txtManagedItemNumber.Text.Trim();
			if (miNumber.Length == 0)
				RegisterAlert("You must enter a managed item number first");
			else
			{
				DataSet ds = Data.ManagedItem.FindByNumber(new Guid(ddlSystem.SelectedValue), miNumber, null);
				Guid managedItemId = new Guid(ds.Tables[0].Rows[0][Data.DataNames._C_ManagedItemID].ToString());
				DownloadXml(Data.ManagedItem.GetTemplateDef(managedItemId));
			}
		}


		private void ShowTemplateOrItem(bool reloadTemplateList)
		{
            divUpload.Visible = false;
            
            if (ddlSystem.SelectedIndex > 0 && ddlType.SelectedIndex > 0)
			{
                switch (ddlType.SelectedValue)
                {
                    case "ManagedItem":
					    ddlTemplate.Visible = false;
					    txtManagedItemNumber.Visible = true;
					    lblItem.Text = "Managed Item:";
					    trIncludeInactive.Visible = false;
					    lblItem.Visible = true;
					    btnOpen.Enabled = true;
                        break;

                    case "New":
                        ddlTemplate.Visible = false;
                        txtManagedItemNumber.Visible = true;
                        lblItem.Text = "Template:";
                        trIncludeInactive.Visible = false;
                        lblItem.Visible = true;
                        btnOpen.Enabled = false;
                        divUpload.Visible = true;
                        break;

                    case "Draft":
                        ddlTemplate.Visible = true;
                        txtManagedItemNumber.Visible = false;
                        lblItem.Text = "Template:";
                        trIncludeInactive.Visible = true;
                        lblItem.Visible = true;
                        if (reloadTemplateList)
                            LoadTemplateList();
                        btnOpen.Enabled = true;
                        if (ddlTemplate.SelectedIndex > 0)
                            divUpload.Visible = true;
                        break;

                    case "Final":
                    default:
                        ddlTemplate.Visible = true;
                        txtManagedItemNumber.Visible = false;
                        lblItem.Text = "Template:";
                        trIncludeInactive.Visible = true;
                        lblItem.Visible = true;
                        LoadTemplateList();
                        btnOpen.Enabled = true;
                        break;
                }
			}
			else
			{
				txtManagedItemNumber.Visible = false;
				trIncludeInactive.Visible = false;
				ddlTemplate.Visible = false;
				lblItem.Visible = false;
				btnOpen.Enabled = false;
			}
		}

		private void LoadTemplateList()
		{
            using (DataSet ds = GetTemplateList(false))
			{
				var templates = from t in ds.Tables[0].AsEnumerable()
								orderby t.Field<string>("TemplateName"), t.Field<short>("Status")
								select new
								{
									Desc = string.Format("{0} ({1})", t.Field<string>("TemplateName"), ((Business.TemplateStatusType)t.Field<short>("Status")).ToString()),
									ID = t.Field<Guid>("TemplateID")
								};
				ddlTemplate.DataSource = templates;
				ddlTemplate.DataTextField = "Desc";
				ddlTemplate.DataValueField = "ID";
				ddlTemplate.DataBind();
			}
			ddlTemplate.Items.Insert(0, new ListItem("(Select a template)", ""));
		}

        private DataSet GetTemplateList(bool forceAll)
        {
            List<short> statuses = new List<short>();
            statuses.Add((short)Business.TemplateStatusType.Active);
            statuses.Add((short)Business.TemplateStatusType.SearchOnly);
            if (chkIncludeInactive.Checked || forceAll)
                statuses.Add((short)Business.TemplateStatusType.Inactive);

            return Business.Template.GetTemplateListWithStatus(new Guid(ddlSystem.SelectedValue), statuses, null);
        }

	}
}
