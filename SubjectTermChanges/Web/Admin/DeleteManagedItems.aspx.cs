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


namespace Kindred.Knect.ITAT.Web
{
	public partial class DeleteManagedItems : BaseSystemPage
	{

		internal override Control ResizablePanel()
		{
			return null;
		}


		internal override HtmlGenericControl HTMLBody()
		{
			return this.htmlBody;
		}

		
		protected override string GetApplicationFunction()
		{
			return "DeleteManagedItem";
		}


		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);
			if (!IsPostBack)
			{
				lblSystem.Text = _itatSystem.Name;
				ddlDeleteBy.Items.Add(new ListItem("(Select One)", ""));
				//ddlDeleteBy.Items.Add(new ListItem("System", "System"));
				ddlDeleteBy.Items.Add(new ListItem("Template", "Template"));
				ddlDeleteBy.Items.Add(new ListItem("Managed Item", "ManagedItem"));

				ShowTemplateOrItem();
			}
		}


		protected override void OnPreRender(EventArgs e)
		{
			base.OnPreRender(e);
			RegisterDeleteConfirmation();
		}


		protected void ddlDeleteByOnSelectedIndexChanged(object sender, EventArgs e)
		{
			ShowTemplateOrItem();
		}


		protected void btnResetOnCommand(object sender, CommandEventArgs e)
		{
			ddlDeleteBy.SelectedIndex = 0;
			ShowTemplateOrItem();
		}

		protected void btnDeleteOnCommand(object sender, CommandEventArgs e)
		{
			switch (ddlDeleteBy.SelectedValue)
			{
				case "System":
					DeleteBySystem(_itatSystem.ID);
					break;
				case "Template":
					DeleteByTemplate(ddlTemplate.SelectedValue, false);
					break;
				case "ManagedItem":
					DeleteByManagedItem(txtManagedItemNumber.Text);
					break;
				default:
					break;
			}
		}

		private void DeleteByManagedItem(string managedItemNumber)
		{
			RegisterAlert(string.Format("TODO: Delete by managed item (#={0}.", managedItemNumber));
		}
		 
		private void DeleteByTemplate(string templateId, bool bIncludeContainer)
		{
			int nDocuments = 0;
			int nDocumentsDeleted = 0;
			int nErrorCount = 0;
			DataSet dsDocument = Data.ManagedItem.GetTemplateDocumentList(new Guid(templateId));
			if (dsDocument.Tables.Count > 0)
			{
				nDocuments = dsDocument.Tables[0].Rows.Count;
				nDocumentsDeleted = 0;
				foreach (DataRow row in dsDocument.Tables[0].Rows)
				{
                    string documentStoreId = row[Data.DataNames._C_DocumentStoreID].ToString();
					Utility.DocumentStorage documentStorageObject = Utility.DocumentStorage.GetDocumentStorageObject(_itatSystem.DocumentStorageType);
					documentStorageObject.RootPath = _itatSystem.DocumentStorageRootPath;
					if (documentStorageObject.DeleteDocument(documentStoreId))
					{
						//if record exists in CachedDocuments table, delete it
                        Data.Document.RemoveCachedDocument(documentStoreId);
						nDocumentsDeleted++;
					}
					else
						nErrorCount++;
				}
			}
			Data.Template.DeleteTemplate(new Guid(templateId),false);
			if (nErrorCount == 0)
			{
				RegisterAlert(string.Format("Template (id={0}) and {1:D} documents deleted!", templateId, nDocumentsDeleted));
			}
			else
			{
				RegisterAlert(string.Format("For Template (id={0}), {1:D} documents out of {2:D} deleted!  ****  Please check the application log for {3:D} error(s)!  ****", templateId, nDocumentsDeleted, nDocuments, nErrorCount));
			}
		}

		private void DeleteBySystem(Guid systemId)
		{
			RegisterAlert(string.Format("TODO: Delete by System (id={0}.", systemId));
		}


		private void RegisterDeleteConfirmation()
		{
			switch (ddlDeleteBy.SelectedValue)
			{
				case "System":
					btnDelete.OnClientClick = string.Format("return confirm(\"This will delete every {0} from this system.  Are you sure?\");", _itatSystem.ManagedItemName);
					break;
				case "Template":
					btnDelete.OnClientClick = string.Format("return confirm(\"This will delete every {0} for this template.  Are you sure?\");", _itatSystem.ManagedItemName);
					break;
				case "ManagedItem":
					btnDelete.OnClientClick = string.Format("return confirm(\"This will delete the specified {0}.  Are you sure?\");", _itatSystem.ManagedItemName);
					break;
				default:
					break;
			}
		}


		private void ShowTemplateOrItem()
		{
			if (ddlDeleteBy.SelectedIndex > 0)
			{
				switch (ddlDeleteBy.SelectedValue)
				{
					case "System":
						trCriteria.Visible = false;
						btnDelete.Enabled = true;
						break;
					case "Template":
						trCriteria.Visible = true;
						ddlTemplate.Visible = true;
						txtManagedItemNumber.Visible = false;
						lblItem.Text = "Template:";
						lblItem.Visible = true;
						LoadTemplateList();
						btnDelete.Enabled = true;
						break;
					case "ManagedItem":
						trCriteria.Visible = true;
						ddlTemplate.Visible = false;
						txtManagedItemNumber.Visible = true;
						lblItem.Text = "Managed Item:";
						lblItem.Visible = true;
						btnDelete.Enabled = true;
						break;
					default:
						break;
				}
			}
			else
			{
				trCriteria.Visible = false;
				btnDelete.Enabled = false;
			}
		}


		private void LoadTemplateList()
		{
            using (DataSet ds = Business.Template.GetTemplateList(_itatSystem.ID,null))
			{
				ddlTemplate.DataSource = ds;
				ddlTemplate.DataTextField = "TemplateName";
				ddlTemplate.DataValueField = "TemplateID";
				ddlTemplate.DataBind();
			}
			ddlTemplate.Items.Insert(0, new ListItem("(Select a template)", ""));
		}

	}
}
