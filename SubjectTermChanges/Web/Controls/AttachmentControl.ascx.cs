using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Collections.Generic;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using Kindred.Common.Security;
using System.IO;

namespace Kindred.Knect.ITAT.Web
{
	public partial class AttachmentControl : System.Web.UI.UserControl
	{

        #region private members and constants

        private const string VSKEY_FILE_NEW_ATTACH_VIS = "VSKEY_FILE_NEW_ATTACH_VIS";
        private const string VSKEY_LINK_OPEN_DOC_VIS = "VSKEY_LINK_OPEN_DOC_VIS";
        private const string VSKEY_DROP_DOWN_ENABLED = "VSKEY_DROP_DOWN_ENABLED";
        private const string VSKEY_LINK_DELETE_DOC_VIS = "VSKEY_LINK_DELETE_DOC_VIS";
        private const string VSKEY_LINK_DELETE_SCANNED_DOC_VIS = "VSKEY_LINK_DELETE_SCANNED_DOC_VIS";

        private List<Business.DocumentType> _documentTypes;
		private Business.ManagedItem _managedItem;
		private Business.ITATSystem _itatSystem;
        private Guid _termGroupID;
        private bool _fileNewAttachmentVisible = false;
        private bool _linkOpenDocumentVisible = false;
        private bool _dropDownEnabled = false;
        private bool _linkDeleteDocumentVisible = false;
        private bool _linkDeleteScannedDocumentVisible = false;

		#endregion


		#region properties

		public List<Business.DocumentType> DocumentTypes
		{
			get { return _documentTypes; }
			set { _documentTypes = value; }
		}

		public Business.ManagedItem ManagedItem
		{
			get { return _managedItem; }
			set { _managedItem = value; }
		}

		public Business.ITATSystem ItatSystem
		{
			get { return _itatSystem; }
			set { _itatSystem = value; }
		}

		public Guid TermGroupID
		{
			get { return _termGroupID; }
			set { _termGroupID = value; }
		}

		#endregion


		#region  base class overrides

		protected override void OnInit(EventArgs e)
		{
			base.OnInit(e);
		}


		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);
			if (!IsPostBack)
			{
				if (_documentTypes == null)
					throw new NullReferenceException("The DocumentTypes property has not been initialized.");
				if (_managedItem == null)
					throw new NullReferenceException("The ManagedItem property has not been initialized.");
				if (_itatSystem == null)
					throw new NullReferenceException("The ItatSystem property has not been initialized.");


				Business.SecurityHelper securityHelper = ((BaseSystemPage)this.Page).SecurityHelper;
				bool isSystemAdmin = securityHelper.CanPerformFunction(_itatSystem.AllowedRoles(Business.XMLNames._AF_EditAttachment));

                Business.StateTermGroup stateTermGroup = _managedItem.State.GetTermGroup(TermGroupID);

                bool isViewer = Utility.ListHelper.HaveAMatch(securityHelper.UserRoles, stateTermGroup.Viewers.ConvertAll<string>(Business.Role.StringConverter));
                bool isEditor = Utility.ListHelper.HaveAMatch(securityHelper.UserRoles, stateTermGroup.Editors.ConvertAll<string>(Business.Role.StringConverter));
                bool isAttachmentRemover = Utility.ListHelper.HaveAMatch(securityHelper.UserRoles, stateTermGroup.AttachmentRemovers.ConvertAll<string>(Business.Role.StringConverter));
                bool isScannedAttachmentRemover = Utility.ListHelper.HaveAMatch(securityHelper.UserRoles, stateTermGroup.ScannedAttachmentRemovers.ConvertAll<string>(Business.Role.StringConverter));

                bool viewAttachment = isViewer; 
                bool editAttachment = isEditor || (isViewer && (_itatSystem.ViewersEditAttachments ?? false));
                bool deleteAttachment = isEditor && isAttachmentRemover;
                bool deleteScannedAttachment = isEditor && isScannedAttachmentRemover;

                _linkOpenDocumentVisible = viewAttachment || editAttachment || deleteAttachment || deleteScannedAttachment;
                _dropDownEnabled = editAttachment || deleteAttachment || deleteScannedAttachment;
                _fileNewAttachmentVisible = _dropDownEnabled;
				//TODO:RR - should the ScannedAttachmentDeleters automatically be "Regular" AttachmentDeleters
                _linkDeleteDocumentVisible = deleteAttachment /* || deleteScannedAttachment */ ;
				//TODO:RR - should the SystemAdmins automatically be ScannedAttachmentDeleters?  If so, what about "Regular" AttachmentDeleters
                _linkDeleteScannedDocumentVisible = deleteScannedAttachment /* || isSystemAdmin */ ;

                filNewAttachment.Visible = _fileNewAttachmentVisible;

                StringWriter scriptBlock = null;
                string scriptBlockName = String.Empty;
                scriptBlockName = "_kh_RefreshPage";
                scriptBlock = new StringWriter();
                scriptBlock.WriteLine("<script type=\"text/javascript\">");
                scriptBlock.WriteLine("<!--");
                scriptBlock.WriteLine("function _kh_DeleteAttachmentRow(clientID,attachmentIndex,documentStoreID)");
                scriptBlock.WriteLine("{");
                //First delete the row that was just clicked.
                scriptBlock.WriteLine(" var clientControl = document.getElementById(clientID);");
                scriptBlock.WriteLine(" clientControl.parentNode.deleteRow(clientControl.rowIndex);");
                //Indicate that the page has been modified.
                scriptBlock.WriteLine(" _kh_onChange();");
                //Store the value of the document being deleted for notification back to the server.
                scriptBlock.WriteLine(" var hiddenControl = document.getElementById('{0}');", (Page as ManagedItemProfile).DeletedAttachmentsControl().ClientID);
                scriptBlock.WriteLine(" hiddenControl.value = hiddenControl.value + attachmentIndex + '{0}' + documentStoreID + '{1}';", ManagedItemProfile._ATTACHMENT_PART_DELIMITER, ManagedItemProfile._ATTACHMENT_DELIMITER);
                scriptBlock.WriteLine(" return false;");
                scriptBlock.WriteLine("}");
                scriptBlock.WriteLine("//-->");
                scriptBlock.WriteLine("</script>");
                if (!Page.ClientScript.IsClientScriptBlockRegistered(scriptBlockName))
                    Page.ClientScript.RegisterClientScriptBlock(this.GetType(), scriptBlockName, scriptBlock.ToString());
                scriptBlock.Close();

            }
		}

		protected override void OnPreRender(EventArgs e)
		{
			base.OnPreRender(e);
			BindData();
			separator.Visible = (_managedItem.Attachments.Count > 0);
		}


		private string ShortAttachmentName(string attachmentName)
		{
			int lastSlashPosition = Math.Max(attachmentName.IndexOf('\\'), attachmentName.IndexOf('/'));
			if (lastSlashPosition > -1)
				return attachmentName.Substring(lastSlashPosition + 1);
			return attachmentName;
		}

		protected void lstAttachments_ItemDataBound(object sender, DataListItemEventArgs e)
		{
			switch (e.Item.ItemType)
			{
				case ListItemType.AlternatingItem:
				case ListItemType.Item:
					{
						if (e.Item.DataItem == null)
							throw new NullReferenceException("e.Item.DataItem is null");
						Business.Attachment att = e.Item.DataItem as Business.Attachment;
						if (att == null)
							throw new Exception("lstAttachments DataItem is not of type Business.Attachment. It's type is " + e.Item.DataItem.GetType().FullName);

                        if (_linkOpenDocumentVisible)
                        {
                            HyperLink linkOpenDocument = (HyperLink)e.Item.FindControl("linkOpenDocument");
                            linkOpenDocument.Target = "_blank";
                            linkOpenDocument.Text = ShortAttachmentName(att.Name);
                            string filename = System.IO.Path.GetFileName(att.Name);
                            linkOpenDocument.NavigateUrl = "~/ShowDoc.ashx" + Utility.TextHelper.QueryString(true, "type", "attachment", Common.Names._QS_DOCUMENT_ID, att.DocumentStoreID, Common.Names._QS_ITAT_SYSTEM_ID, _managedItem.SystemID.ToString(), Common.Names._QS_FILENAME, filename);
                            linkOpenDocument.ToolTip = (string.IsNullOrEmpty(att.Description) ? att.Name : att.Description);
                        }


                        DropDownList ddlDocumentType = (DropDownList)e.Item.FindControl("ddlDocumentType");
                        Helper.LoadListControl(ddlDocumentType, _documentTypes, "Name", "Name", string.Empty, true, "(Select One)", string.Empty);
                        //Find the selected value in the dropdown.  If it is not there, add it.
                        string docType = att.DocumentType.Name;
                        if (docType.Length > 0)
                        {
                            bool matchFound = false;
                            for (int itemIndex = 0; itemIndex < ddlDocumentType.Items.Count; itemIndex++)
                            {
                                if (ddlDocumentType.Items[itemIndex].Value == docType)
                                {
                                    ddlDocumentType.SelectedIndex = itemIndex;
                                    matchFound = true;
                                    break;
                                }
                            }
                            if (!matchFound)
                            {
                                ddlDocumentType.Items.Add(new ListItem(docType, docType));
                                ddlDocumentType.SelectedValue = docType;
                            }
                        }
                        ddlDocumentType.Enabled = _dropDownEnabled;

                        bool displayLink = false;
                        if (att.IsScanned)
                            displayLink = _linkDeleteScannedDocumentVisible;
                        else
                            displayLink = _linkDeleteDocumentVisible;


						LinkButton lnkRemoveAttachment = (LinkButton)e.Item.FindControl("lnkRemoveAttachment");
						if (lnkRemoveAttachment != null)
						{
							if (displayLink)
							{
                                lnkRemoveAttachment.Visible = true;
                                HtmlControl rowControl = (HtmlControl)e.Item.FindControl("rowLink");
                                lnkRemoveAttachment.OnClientClick = string.Format("return _kh_DeleteAttachmentRow('{0}','{1}','{2}');", rowControl.ClientID, e.Item.ItemIndex.ToString(), att.DocumentStoreID);
							}
							else
							{
								lnkRemoveAttachment.Visible = false;
							}
						}
						else
						{
							throw new NullReferenceException(string.Format("Unable to find the \"Remove\" link for attachment \"{0}\".", att.Name));
						}
						break;
					}
				default:
					break;
			}
		}

		protected void ddlDocumentType_SelectedIndexChanged(object sender, EventArgs e)
		{
            DropDownList ddl = (DropDownList)sender;
            int rowIndex = ((DataListItem)((Control)((HtmlTableCell)ddl.Parent).Parent).Parent).ItemIndex;
    		_managedItem.Attachments[rowIndex].UpdateDocumentType(_itatSystem, ddl.SelectedValue);
		}

		protected void filNewAttachment_OnUpload(object sender, FileUploadEventArgs e)
		{

			//get the uploaded file and its metadata
			HttpPostedFile postedFile = e.PostedFile;
			string filepath = postedFile.FileName;
			string filename = filepath.Substring(filepath.LastIndexOf('\\') + 1);

			if (ValidateFile(filename))
			{
				//separate the file extension from the filename
				string fileExt = System.IO.Path.GetExtension(filename);
				string fileNameWithoutExt = System.IO.Path.GetFileNameWithoutExtension(filename);

				int contentLength = postedFile.ContentLength;
				string contentType = postedFile.ContentType;

				System.IO.BinaryReader reader = new System.IO.BinaryReader(postedFile.InputStream);
				byte[] bytes = new byte[contentLength];
				reader.Read(bytes, 0, contentLength);

				//store the file into the data store
				string documentStoreId = string.Empty;
				try
				{
					Utility.DocumentStorage documentStorageObject = Utility.DocumentStorage.GetDocumentStorageObject(_itatSystem.DocumentStorageType);
					documentStorageObject.RootPath = _itatSystem.DocumentStorageRootPath;
					documentStoreId = documentStorageObject.SaveDocument(fileNameWithoutExt, fileExt, bytes);
				}
				catch (ArgumentException ex)
				{
					if (ex.Message.Contains(Utility.Names._EM_UnrecognizedExtension))
					{
						BasePage p = Page as BasePage;
						if (p == null)
							throw new Exception("The AttachmentControl object must be used on a page that derives from BasePage.");
						p.RegisterAlert("Unrecognized file extension on attachment.");
					}
					else
						throw;
				}

				//store a reference to the document in the DocumentCache table
				if (!string.IsNullOrEmpty(documentStoreId))
				{
					Data.Document.AddCachedDocument(Guid.NewGuid(), _managedItem.ManagedItemID, documentStoreId, filepath, "", "");

					//create attachment and add it to the managed item
					Business.Attachment newAttachment = new Kindred.Knect.ITAT.Business.Attachment(Guid.NewGuid(), filepath, "", documentStoreId, new Business.DocumentType(""), false);
					_managedItem.Attachments.Add(newAttachment);
				}
			}
		}



		private bool ValidateFile(string filename)
		{
			BasePage p = Page as BasePage;
			if (p == null)
				throw new Exception("The AttachmentControl object must be used on a page that derives from BasePage.");

			if (string.IsNullOrEmpty(filename))
			{
				p.RegisterAlert("A file must be selected.");
				return false;
			}

			return true;
		}

        protected override object SaveViewState()
        {
            ViewState[VSKEY_FILE_NEW_ATTACH_VIS] = _fileNewAttachmentVisible;
            ViewState[VSKEY_LINK_OPEN_DOC_VIS] = _linkOpenDocumentVisible;
            ViewState[VSKEY_DROP_DOWN_ENABLED] = _dropDownEnabled;
            ViewState[VSKEY_LINK_DELETE_DOC_VIS] = _linkDeleteDocumentVisible;
            ViewState[VSKEY_LINK_DELETE_SCANNED_DOC_VIS] = _linkDeleteScannedDocumentVisible;
            return base.SaveViewState();
        }

        protected override void LoadViewState(object savedState)
        {
            base.LoadViewState(savedState);
            _fileNewAttachmentVisible = (bool)ViewState[VSKEY_FILE_NEW_ATTACH_VIS];
            _linkOpenDocumentVisible = (bool)ViewState[VSKEY_LINK_OPEN_DOC_VIS];
            _dropDownEnabled = (bool)ViewState[VSKEY_DROP_DOWN_ENABLED];
            _linkDeleteDocumentVisible = (bool)ViewState[VSKEY_LINK_DELETE_DOC_VIS];
            _linkDeleteScannedDocumentVisible = (bool)ViewState[VSKEY_LINK_DELETE_SCANNED_DOC_VIS];
        }

		#endregion


		#region private methods


		private void BindData()
		{
			lstAttachments.DataSource = _managedItem.Attachments;
			lstAttachments.DataBind();
		}

		
		#endregion




	}
}