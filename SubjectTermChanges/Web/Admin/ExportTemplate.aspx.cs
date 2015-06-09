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
using System.IO;
using System.Xml.Serialization;


namespace Kindred.Knect.ITAT.Web
{
	public partial class ExportTemplate : BasePage
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
				
				LoadTemplateList();
			}
		}

		private void DownloadXml(string xml)
		{
			try { xml = Utility.XMLHelper.FormatXML(xml); }
			catch { }

			Response.Clear();
			Response.Buffer = true;
			Response.AddHeader("Content-Disposition", string.Format("attachment; filename={0}-{1} TemplateDef.xml", ddlSystem.SelectedItem.Text, ddlTemplate.SelectedItem.Text));
			Response.ContentType = "Text/XML";
			Response.Write(xml);
			Response.End();
		}


		protected void ddlSystemOnSelectedIndexChanged(object sender, EventArgs e)
		{
			LoadTemplateList();
		}



		protected void btnResetOnCommand(object sender, CommandEventArgs e)
		{
			ddlSystem.SelectedIndex = 0;
			LoadTemplateList();
		}


		protected void btnOpenOnCommand(object sender, CommandEventArgs e)
		{
			DownloadTemplateDef();
		}


		private void DownloadTemplateDef()
		{
			Business.Template template = new Kindred.Knect.ITAT.Business.Template(new Guid(ddlTemplate.SelectedValue), Business.DefType.Final);
			System.Text.StringBuilder sb = new System.Text.StringBuilder();
			sb.Append("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
			sb.Append("<Template>");
			sb.AppendFormat("<Name>{0}</Name>", template.Name);
			sb.AppendFormat("<Description>{0}</Description>", template.Description);
			sb.AppendFormat("<Status>{0}</Status>", template.Status);
			sb.AppendFormat("<CanGenerateDocument>{0}</CanGenerateDocument>", template.CanGenerateDocument);
            sb.AppendFormat("<CanGenerateUserDocuments>{0}</CanGenerateUserDocuments>", template.CanGenerateUserDocuments);
			sb.AppendFormat("<AllowAttachments>{0}</AllowAttachments>", template.AllowAttachments);
			sb.AppendFormat("<Version>{0}</Version>", template.Version);
			sb.AppendFormat("<XMLVersion>{0}</XMLVersion>", template.XMLVersion);
			sb.AppendFormat("<TemplateDef>{0}</TemplateDef>", template.TemplateDef);
			sb.Append("</Template>");

			Response.Clear();
			Response.Buffer = true;
			Response.AddHeader("Content-Disposition", string.Format("attachment; filename={0}-{1}-template.xml", ddlSystem.SelectedItem.Text, ddlTemplate.SelectedItem.Text));
			Response.ContentType = "text/xml";
			Response.Write(sb.ToString());
			Response.End();						
		}


		private void LoadTemplateList()
		{
			if (ddlSystem.SelectedIndex > 0)
			{
				ddlTemplate.Visible = true;
				btnOpen.Enabled = true;

                using (DataSet ds = Business.Template.GetTemplateList(new Guid(ddlSystem.SelectedValue),null))
				{
					ddlTemplate.DataSource = ds;
					ddlTemplate.DataTextField = "TemplateName";
					ddlTemplate.DataValueField = "TemplateID";
					ddlTemplate.DataBind();
				}
				ddlTemplate.Items.Insert(0, new ListItem("(Select a template)", ""));
			}
			else
			{
				ddlTemplate.Visible = false;
				btnOpen.Enabled = false;
			}
		}

	}
}
