using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using Kindred.Knect.ITAT.Business;
using System.Collections.Generic;
using System.Diagnostics;
using Kindred.Common.Logging;

namespace Kindred.Knect.ITAT.Web
{

	public partial class ManagedItemProfile : BaseManagedItemPage
	{
		 
		#region  private members and constants

		//The '_COMMA' symbol is used to assist in creating a comma delimited list of term names which may contain embedded commas.
		//The embedded commas within the term names are first converted before the list is created.
		private const string _COMMA = "&#044;";

        List<string> _validationErrors;
		string _previousManagedItemDef;
		const string VSKEY_DELETED_ATTACHMENTS = "_kh_vs_DeletedAttachments";

		#endregion

        public const char _ATTACHMENT_DELIMITER = ',';
        public const char _ATTACHMENT_PART_DELIMITER = '|';

		#region  base class overrides

		internal override Control ResizablePanel()
		{
			return pnlTerms;
		}


		internal override HtmlGenericControl HTMLBody()
		{
			return this.htmlBody;
		}


		protected override ManagedItemHeader HeaderControl()
		{
			return (ManagedItemHeader)header;
		}


		protected override string GetApplicationFunction()
		{
			return null;   // Anyone (who can access the application) can access this page
		}

		#endregion

        #region Properties
        Guid CurrentStateID
        {
            get { return (Guid)ViewState["CurrentState"]; }
            set { ViewState["CurrentState"] = value; }
        }

        string CurrentStatus
        {
            get { return (string)ViewState["CurrentStatus"]; }
            set { ViewState["CurrentStatus"] = value; }
        }
        #endregion
        
        #region event handlers

		protected override void OnInit(EventArgs e)
		{
			base.OnInit(e);
			_validationErrors = new List<string>();
		}

		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);
			_previousManagedItemDef = _managedItem.TemplateDef;

			//Verify that the user has access to this ManagedItem, unless this is an 'admin viewer'
			if (!IsPostBack)
			{
                //Preserve the current values for State and Status in the event of a Workflow switch, and validation errors result.
                CurrentStateID = _managedItem.State.ID;
                CurrentStatus = _managedItem.State.Status;
                if (!(Utility.ListHelper.HaveAMatch(_managedItem.State.AccessorRoleNames, _securityHelper.UserRoles)))
					if (!(Utility.ListHelper.HaveAMatch(_itatSystem.AllowedRoles(Business.XMLNames._AF_AdminViewer), _securityHelper.UserRoles)))
					{
						UnauthorizedPageAccess();
					}
			}

			Dictionary<Guid /*TermGroupID*/, bool /*CanAccess*/> canAccessTermGroup = BuildEditableTermGroups();		

			if (IsPostBack)
			{
				UpdateManagedItemValues();
				_managedItem.ApplyTermDependencies(null, canAccessTermGroup);
				//determine target State, ValidationType, and validation errors
				string targetStateName = Business.State.ParseStateName(hfTargetState.Value);
                string buttonText = Business.State.ParseButtonText(hfTargetState.Value);
				if (string.IsNullOrEmpty(targetStateName))
				{
					if (hfDoValidation.Value == "1")
					{
						Dictionary<string /* Term Name */, List<string> /* Error Messages */ > termTypeErrors = GetTermTypeErrors(canAccessTermGroup);
						_validationErrors.AddRange(GetValidationErrors(ManagedItemValidationType.ValidateOnSave, termTypeErrors, canAccessTermGroup));
                        foreach (ComplexList complexList in _managedItem.ComplexLists)
                        {
                            Dictionary<string /* Term Name */, List<string> /* Error Messages */ > complexListTermTypeErrors = Helper.GetComplexListTermTypeErrors(canAccessTermGroup, complexList, null);
                            _validationErrors.AddRange(Helper.GetComplexListValidationErrors(canAccessTermGroup, ManagedItemValidationType.ValidateOnSave, termTypeErrors, complexList.Items, true, _managedItem.SecurityModel, _managedItem.State.Status, complexList.TermGroupID, complexList.Name));
                        }
                    }
				}
				else
				{
					Business.State targetState = _managedItem.Workflow.FindState(targetStateName);
					Business.ManagedItemValidationType validationType = targetState.RequiresValidation ?? false ? ManagedItemValidationType.FullValidation : ManagedItemValidationType.None;
					Dictionary<string /* Term Name */, List<string> /* Error Messages */ > termTypeErrors = GetTermTypeErrors(canAccessTermGroup);
					_validationErrors.AddRange(GetValidationErrors(validationType, termTypeErrors, canAccessTermGroup));
                    foreach (ComplexList complexList in _managedItem.ComplexLists)
                    {
                        Dictionary<string /* Term Name */, List<string> /* Error Messages */ > complexListTermTypeErrors = Helper.GetComplexListTermTypeErrors(canAccessTermGroup, complexList, null);
                        _validationErrors.AddRange(Helper.GetComplexListValidationErrors(canAccessTermGroup, validationType, termTypeErrors, complexList.Items, true, _managedItem.SecurityModel, _managedItem.State.Status, complexList.TermGroupID, complexList.Name));
                    }

					if (_validationErrors.Count == 0)
						UpdateManagedItemState(targetState, buttonText);
				}
			}
			else
			{
				_managedItem.ApplyTermDependencies(null, canAccessTermGroup);
			}
			BuildPage();
		}

		protected override void OnPreRender(EventArgs e)
		{
			SetComplexListsVisibility();
			if (_validationErrors.Count > 0)
			{
				RenderValidationErrors(_validationErrors);
				UndoManagedItemUpdate();
				this.IsChanged = true;
			}
			RegisterClientSideEvents();
			RestoreFocusAndScrollPosition();
            RegisterChildControlEventHandlers();
			base.OnPreRender(e);
		}

		public void OnHeaderEvent(object sender, HeaderEventArgs e)
		{
			switch (e.CommandName)
			{
				case Common.Names._HEADER_EVENT_Save:
                    DeleteAttachments();
                    _managedItem.Update(false, Business.Retro.AuditType.Saved);
					this.IsChanged = false;
					break;

				case Common.Names._HEADER_EVENT_Reset:
					UndoManagedItemUpdate();
					Server.Transfer(string.Format("ManagedItemProfile.aspx{0}", Utility.TextHelper.QueryString(true, Common.Names._QS_ITAT_SYSTEM_ID, _itatSystem.ID.ToString(), Common.Names._QS_MANAGED_ITEM_ID, _managedItem.ManagedItemID.ToString())), false);
					break;

				default:
					break;
			}
		}

        protected override object SaveViewState()
        {
            ViewState[VSKEY_DELETED_ATTACHMENTS] = _kh_hf_DeletedAttachments.Value;
            return base.SaveViewState();
        }

        protected override void LoadViewState(object savedState)
        {
            base.LoadViewState(savedState);
            _kh_hf_DeletedAttachments.Value = (string)ViewState[VSKEY_DELETED_ATTACHMENTS];
        }

		#endregion


		#region Page-rendering methods


		private void InvalidTargetState(string targetStateName)
		{
			System.Text.StringBuilder sb = new System.Text.StringBuilder();
			sb.AppendFormat("Unable to find target state \"{0}\".  Possible states include: ", targetStateName);
			foreach (Business.State state in _managedItem.Workflow.States)
				sb.AppendFormat("{0}, ", state.Name);
			string msg = sb.ToString();
			throw new Exception(msg.Substring(0, msg.Length - 2) + ".");

		}

		private void BuildPage()
		{
			//Add terms to the Profile Table
            HeaderControl().Roles = _securityHelper.UserRoles.ToArray();
			RenderStaticInfo("Status / Workflow State", string.Format("{0} / {1}", _managedItem.State.Status, _managedItem.State.Name), 0, true);
			RenderStaticInfo("Template", _managedItem.Name, 1, true);
			RenderStaticInfo("Workflow Name", _managedItem.FindWorkflow(_managedItem.ActiveWorkflowID).Name, 2, false);
			if (_managedItem.SecurityModel == SecurityModel.Advanced)
			{
				RenderHeaderTerms();
			}

			List<TermGroup> termGroups = _managedItem.GetTermGroups(_managedItem.SecurityModel == SecurityModel.Advanced ? TermGroup.TermGroupType.AdvancedBasicTerm : TermGroup.TermGroupType.BasicSecurity);
			foreach (TermGroup tg in termGroups)
			{
				ManagedItemProfilePanel mipp = RenderControls(tg);
				mipp.IsActiveTermGroup = (tg == ActiveTermGroup);
				pnlTerms.Controls.Add(mipp);
			}
			RenderButtons();
		}


		private void RenderStaticInfo(string caption, string data, bool visible)
		{
			//Add a new table row at the end of the current table
			RenderStaticInfo(caption, data, -1, visible);
		}

		private void RenderStaticInfo(string caption, string data, int rowIndex, bool visible)
		{
			//Add a new table row at the specified row index
			TableRow row = new TableRow();
			if (!visible)
				row.Style.Add("display", "none");

			TableCell cellLabel = new TableCell();
			Helper.FormatCaptionCell(cellLabel);

			cellLabel.Text = caption;
			row.Cells.Add(cellLabel);

			TableCell cellValue = new TableCell();
			Helper.FormatDataCell(cellValue, false);
			cellValue.Text = data;
			row.Cells.Add(cellValue);

			if (rowIndex > -1)
			    tblHeaderTerms.Rows.AddAt(rowIndex, row);
			else
				tblHeaderTerms.Rows.Add(row);
		}

        private void RenderAttachments(Table table, Guid termGroupID)
		{
			TableRow row = new TableRow();

			TableCell cellLabel = new TableCell();
			Helper.FormatCaptionCell(cellLabel);
			cellLabel.Text = "Attachments";
			row.Cells.Add(cellLabel);

			TableCell cellValue = new TableCell();
			Helper.FormatDataCell(cellValue, false);
			cellValue.Style["white-space"] = "normal";
            cellValue.Controls.Add(CreateAttachmentControl(termGroupID));
			row.Cells.Add(cellValue);

            table.Rows.Add(row);
		}

        private void RenderComments(Table table, Guid TermGroupID)
		{
			TableRow row = new TableRow();

			TableCell cellLabel = new TableCell();
			Helper.FormatCaptionCell(cellLabel);
			cellLabel.Text = "Comments";
			if (_managedItem.CanGenerateDocument)
				cellLabel.Text += string.Format("<br /><br /><b>Comments do not<br />affect the {0}.</b>", _itatSystem.ManagedItemName);
			row.Cells.Add(cellLabel);

			TableCell cellValue = new TableCell();
			Helper.FormatDataCell(cellValue, false);
			cellValue.Style["white-space"] = "normal";
            cellValue.Controls.Add(CreateCommentControl(TermGroupID));
			row.Cells.Add(cellValue);

            table.Rows.Add(row);
		}


		private void RestoreFocusAndScrollPosition()
		{
			RegisterLoadAction(string.Format("_kh_RestoreFocus('{0}', '{1}');", _kh_hf_FocusTermGroup.ClientID, _kh_hf_FocusControl.ClientID));
			RegisterLoadAction(string.Format("_kh_RestoreScrollPosition('{0}','{1}');", _kh_hf_PageScrollTop.ClientID, pnlTerms.ClientID));
		}

		private void RegisterClientSideEvents()
		{
			Helper.AddAttribute(divBody.Attributes, "onclick", "ControlOnFocus();");
			Helper.AddAttribute(divBody.Attributes, "TermID", "");
			Helper.AddAttribute(pnlTerms.Attributes, "onscroll", string.Format("_kh_SaveScrollPosition('{0}','{1}');", _kh_hf_PageScrollTop.ClientID, pnlTerms.ClientID));
			hfDoValidation.Value = "0";

			Type t = Page.GetType();
			string scriptName = "_kh_ControlOnFocus";
			System.IO.StringWriter sw = new System.IO.StringWriter();

			List<Guid> sourceTermIds = new List<Guid>();

			foreach (TermDependency td in _managedItem.TermDependencies)
				foreach (TermDependencyCondition tdc in td.Conditions)
					if (!sourceTermIds.Contains(tdc.SourceTermID))
						sourceTermIds.Add(tdc.SourceTermID);

			sw.WriteLine("	var _TDSourceTermIDs = new Array(); ");
			for (int index = 0; index < sourceTermIds.Count; index++)
				sw.WriteLine(" _TDSourceTermIDs[{0}] = '{1}';", index, sourceTermIds[index]);

			if (!Page.ClientScript.IsClientScriptBlockRegistered(t, scriptName))
				Page.ClientScript.RegisterClientScriptBlock(t, scriptName, sw.ToString(), true);

			Page.ClientScript.RegisterStartupScript(t, "_kh_setActiveTermGroup", string.Format("_activeTermGroupId='{0}';_activeTermGroupName='{1}';",ActiveTermGroup.ID, ActiveTermGroup.Name), true);
		}

        private void RegisterChildControlEventHandlers()
        {
            string function = @"function doConfirmation(confirmationMessage, hfTargetState, hfTargetStateValue) {
                    if (confirm(confirmationMessage)) 
                    { 
                        document.getElementById(hfTargetState).value = hfTargetStateValue;
                        return ShowWait(this);
                    } 
                    else 
                    { 
                        return false;
                    }
				}";

            if (!this.Page.ClientScript.IsClientScriptBlockRegistered(typeof(ManagedItemProfile), "doConfirmation"))
                this.Page.ClientScript.RegisterClientScriptBlock(typeof(ManagedItemProfile), "doConfirmation", function, true);
        }

		#endregion


		#region Dynamic ("Term") Controls

        public HiddenField DeletedAttachmentsControl()
        {
            return _kh_hf_DeletedAttachments;
        }

		/// <summary>
		/// Undoes the managed item update.
		/// </summary>
		/// Changed by Larry Richardson LRR 3/31/2008 to allow saving of attachments &amp; comments
		private void UndoManagedItemUpdate()
		{
			List<Comment> comments = this._managedItem.Comments;
			List<Attachment> attachments = this._managedItem.Attachments;

            _managedItem = Business.ManagedItem.Get(_managedItem.ManagedItemID, true, null, CurrentStateID, CurrentStatus, _previousManagedItemDef, _managedItem.ItemNumber, _managedItem.TemplateID, _managedItem.IsOrphaned);
 
			this._managedItem.Attachments = attachments;
			this._managedItem.Comments = comments;

			_managedItem.Update(false, Business.Retro.AuditType.Saved);

            _managedItem.ApplyTermDependencies(null, BuildEditableTermGroups());
		}

        private void DeleteAttachments()
        {
            if (_kh_hf_DeletedAttachments.Value.Length > 0)
            {
                string[] deletedAttachments = _kh_hf_DeletedAttachments.Value.Split(_ATTACHMENT_DELIMITER);
                foreach (string deletedAttachment in deletedAttachments)
                {
                    if (!string.IsNullOrEmpty(deletedAttachment) && deletedAttachment.IndexOf(_ATTACHMENT_PART_DELIMITER) > -1)
                    {
                        string[] deletedAttachmentPart = deletedAttachment.Split(_ATTACHMENT_PART_DELIMITER);
                        DeleteAttachment(int.Parse(deletedAttachmentPart[0]), deletedAttachmentPart[1]);
                    }
                }
            }
            _kh_hf_DeletedAttachments.Value = null;
        }

        private void DeleteAttachment(int attachmentIndex, string documentStoreId)
        {
            //Try to delete document from Document Store
            Utility.DocumentStorage documentStorageObject = Utility.DocumentStorage.GetDocumentStorageObject(_itatSystem.DocumentStorageType);
            documentStorageObject.RootPath = _itatSystem.DocumentStorageRootPath;
            documentStorageObject.DeleteDocument(documentStoreId);
            //If record exists in CachedDocuments table, delete it
            Data.Document.RemoveCachedDocument(documentStoreId);
            //Remove the attachment from the managedItem's Attachments collection
            _managedItem.Attachments.RemoveAt(attachmentIndex);
        }

		private void UpdateManagedItemValues()
		{
			//NOTE:  
			//      Because the UpdateTermValue() method may depend on term.Runtime, it is important that this is executed before 
			//      _managedItem.ApplyTermDependencies() is called in the page lifecycle (which will reset the values of term.Runtime).
			// 
            Dictionary<Guid /*TermGroupID*/, bool /*CanEdit*/> canEditTermGroup = BuildEditableTermGroups();

			foreach (Business.Term term in _managedItem.BasicTerms)
			{
				TermGroup tg = _managedItem.FindTermGroup(term.TermGroupID);
				if (tg == null)
					throw new NullReferenceException(string.Format("TermGroup is null for term '{0}'.", term.Name));
				string termGroupContainerName = ManagedItemProfilePanel.TermGroupContainerId(tg);

                switch (term.TermType)
                {
                    case TermType.PlaceHolderAttachments:
                        if (_managedItem.AllowAttachments)
							UpdateAttachments(termGroupContainerName);
                        break;
                    case TermType.PlaceHolderComments:
						if (_managedItem.AllowComments)
                            UpdateComments(termGroupContainerName, term.TermGroupID);
                        break;
                    default:
                        if (canEditTermGroup[term.TermGroupID])
                        {
                            BaseProfileControl c = CreateControl(term, canEditTermGroup[term.TermGroupID]);
                            if (c != null)
                            {
                                c.UpdateTermValue(termGroupContainerName);
                            }
                        }
                        break;
				
                }
			}
		}


        private void UpdateComments(string termGroupContainerName, Guid TermGroupID)
		{
            CommentControl c = CreateCommentControl(TermGroupID);
			if (c != null)
				c.UpdateManagedItemComments(termGroupContainerName);
		}


		private void UpdateAttachments(string termGroupContainerName)
		{
		}


		private void RenderHeaderTerms()
		{
			List<Term> headerTerms = _managedItem.HeaderTerms;
			RegisterHeaderHeightSetting();
            Dictionary<Guid /*TermGroupID*/, bool /*CanEdit*/> canEditTermGroup = BuildEditableTermGroups();
			foreach (Business.Term term in headerTerms)
			{
                BaseProfileControl c = CreateControl(term, canEditTermGroup[term.TermGroupID]);
				if (c != null)
				{
					c.ID += "_Header";    //makes control ID unique from other instance on the page
					c.CanEdit = false;  
					TableRow row = new TableRow();

					TableCell cellLabel = new TableCell();
					Helper.FormatCaptionCell(cellLabel, term);
					row.Cells.Add(cellLabel);

					TableCell cellValue = new TableCell();
					Helper.FormatDataCell(cellValue, false);
					cellValue.Controls.Add(c);
					row.Cells.Add(cellValue);

					tblHeaderTerms.Rows.Add(row);
				}
			}
		}


		private void RegisterHeaderHeightSetting()
		{
			Type t = this.GetType();
			Page.ClientScript.RegisterStartupScript(t, "_kh_setHeaderHeight", string.Format("SetMaxHeaderHeight('{0}', {1});", pnlHeaderTerms.ClientID, 160), true);
		}

		private ManagedItemProfilePanel RenderControls(TermGroup tg)
		{
			ManagedItemProfilePanel mipp = (ManagedItemProfilePanel)LoadControl(Common.Names._UC_ManagedItemProfilePanel);
			mipp.TermGroup = tg;
            Dictionary<Guid /*TermGroupID*/, bool /*CanEdit*/> canEditTermGroup = BuildEditableTermGroups();
            foreach (Business.Term term in _managedItem.FindAllBasicTerms(tg))
			{
                switch (term.TermType)
                {
                    case TermType.PlaceHolderAttachments:
                        if (_managedItem.AllowAttachments)
                            RenderAttachments(mipp.TermsTable, term.TermGroupID);
                        break;
                    case TermType.PlaceHolderComments:
                        if (_managedItem.AllowComments)
                            RenderComments(mipp.TermsTable, term.TermGroupID);
                        break;
                    default:
                        BaseProfileControl c = CreateControl(term, canEditTermGroup[term.TermGroupID]);
                        if (c != null)
                        {
                            TableRow row = new TableRow();

                            TableCell cellLabel = new TableCell();
                            Helper.FormatCaptionCell(cellLabel, term);
                            row.Cells.Add(cellLabel);

                            TableCell cellValue = new TableCell();
                            Helper.FormatDataCell(cellValue, true);
                            cellValue.Controls.Add(c);
                            row.Cells.Add(cellValue);

                            mipp.TermsTable.Rows.Add(row);
                        }
                        break;
                }
			}
			return mipp;
		}

		private CommentControl CreateCommentControl(Guid TermGroupID)
		{
			CommentControl c = (CommentControl)LoadControl(Common.Names._UC_CommentControl);
			c.ID = Helper.ControlID("_kh_Comments");
			c.Comments = _managedItem.Comments;
            Business.StateTermGroup stateTermGroup = _managedItem.State.GetTermGroup(TermGroupID);
            bool isViewer = Utility.ListHelper.HaveAMatch(_securityHelper.UserRoles, stateTermGroup.Viewers.ConvertAll<string>(Business.Role.StringConverter));
            bool isEditor = Utility.ListHelper.HaveAMatch(_securityHelper.UserRoles, stateTermGroup.Editors.ConvertAll<string>(Business.Role.StringConverter));
            bool commentEditor = (isViewer && (_itatSystem.ViewersAddComments ?? false)) || isEditor;
            c.CanAddComments = commentEditor;
			return c;
		}

		private AttachmentControl CreateAttachmentControl(Guid termGroupID)
		{
			AttachmentControl c = (AttachmentControl)LoadControl(Common.Names._UC_AttachmentControl);
			c.ID = Helper.ControlID("_kh_Attachments");
			c.DocumentTypes = _itatSystem.DocumentTypes;
			c.ManagedItem = _managedItem;
			c.ItatSystem = _itatSystem;
            c.TermGroupID = termGroupID;
			return c;
		}


		private BaseProfileControl CreateControl(Business.Term term, bool canEdit)
		{
            if (canEdit)
				if (!term.Runtime.Enabled)
					term.SetDefaultValue();

			BaseProfileControl c = null;
			//if (term.Runtime.Visible)
			//{
			switch (term.TermType)
			{
				case Business.TermType.Date:
					{
						c = (DateTermControl)LoadControl(Common.Names._UC_DateTermControl);
						((DateTermControl)c).DateFormat = _itatSystem.DefaultDateFormat;
						c.CanEdit = term.Runtime.Enabled;
						break;
					}
				case Business.TermType.Renewal:
					{
						c = (RenewalTermControl)LoadControl(Common.Names._UC_RenewalTermControl);
						((RenewalTermControl)c).DateFormat = _itatSystem.DefaultDateFormat;
						c.CanEdit = term.Runtime.Enabled;
						break;
					}
				case Business.TermType.Text:
					{
						c = (TextTermControl)LoadControl(Common.Names._UC_TextTermControl);
						c.CanEdit = term.Runtime.Enabled;
						break;
					}
				case Business.TermType.MSO:
					{
						c = (MSOTermControl)LoadControl(Common.Names._UC_MSOTermControl);
						c.CanEdit = term.Runtime.Enabled;
						break;
					}
				case Business.TermType.Facility:
					{
						c = (FacilityTermControl)LoadControl(Common.Names._UC_FacilityTermControl);
						c.CanEdit = false;
                        if (canEdit)
                            if (!(((FacilityTerm)term).IsPrimary ?? false))
                                c.CanEdit = true;
						((FacilityTermControl)c).SecurityHelper = _securityHelper;
						break;
					}
				case Business.TermType.PickList:
					{
						c = (PickListTermControl)LoadControl(Common.Names._UC_PickListTermControl);
						c.CanEdit = term.Runtime.Enabled;
						break;
					}
				case Business.TermType.Link:
					{
						//only show the term on the Profile page if it is NOT a ManagedItemReference link
						if (!((Business.LinkTerm)term).IsManagedItemReference ?? false)
						{
							c = (LinkTermControl)LoadControl(Common.Names._UC_LinkTermControl);
							c.CanEdit = true;
							if (Business.Term.ValidID(((Business.LinkTerm)term).ComplexListID))
							{
								Business.ComplexList complexList = (Business.ComplexList)(this.Page as BaseManagedItemPage).ManagedItem.FindTerm(((Business.LinkTerm)term).ComplexListID);
								c.CanEdit = complexList.Runtime.Enabled;
							}
						}
						break;
					}
				case Business.TermType.External:
				   {
					  c = (ExternalTermControl)LoadControl(Common.Names._UC_ExternalTermControl);
					  c.CanEdit = term.Runtime.Enabled;
					  break;
				   }
				default:
					break;
			}
			//}     // if (term.Runtime.Visible)
			if (c != null)
			{
				c.Term = term;
				c.ID = Helper.ControlID(term.Name);
				//if (!string.IsNullOrEmpty(_termNameHasFocus))
				//{
				//   if (_termNameHasFocus == term.Name)
				//   {
				//      c.ControlHasFocus = _controlHasFocus;
				//   }
				//}
			}
			return c;
		}

		#endregion


		#region Action Buttons

		private void RenderButtons()
		{
			hfTargetState.Value = string.Empty;
			pnlButtons.Controls.Clear();
			foreach (Business.Action action in _managedItem.State.Actions)
			{
				bool canPerformFunction = false;
				if (_itatSystem.HasOwningFacility ?? false)
					canPerformFunction = _securityHelper.CanPerformFunction(action.Performers, _managedItem.OwningFacilityIDs);
				else
					canPerformFunction = _securityHelper.CanPerformFunction(action.Performers);

				if (canPerformFunction)
				{
					Button btn = new Button();
					btn.Text = action.ButtonText;
					btn.CssClass = "KnectButton ActionButton";
					Helper.AddAttribute(btn.Attributes, "onfocus", "ControlOnFocus();");
                    //Note - added confirmation change here.
                    if (action.RequiresConfirmation)
                        btn.OnClientClick = string.Format("javascript:return doConfirmation('{0}','{1}','{2}');", action.ConfirmationText, hfTargetState.ClientID, Business.State.CombineStateNameAndButtonText(action.TargetState, action.ButtonText));
                    else
                        btn.OnClientClick = string.Format("javascript:document.getElementById('{0}').value = '{1}';return ShowWait(this);", hfTargetState.ClientID, Business.State.CombineStateNameAndButtonText(action.TargetState, action.ButtonText));
                    pnlButtons.Controls.Add(btn);
				}
			}
		}

		private void UpdateManagedItemState(Business.State targetState, string buttonText)
		{
            Business.Action action = _managedItem.State.FindAction(targetState.ID, buttonText);
			string sEnvironment = System.Configuration.ConfigurationManager.AppSettings[Utility.EnvironmentHelper.GetEnvironment(Kindred.Common.WebServices.EnvironmentDetectionMode.Machine) + Data.DataNames._AC_ApplicationWebServer];

			//send notifications (if any are defined for this "action"
			List<string> errors = new List<string>();
			foreach (Business.Message message in action.Messages)
			{
				string error = message.Send(_managedItem, _itatSystem, sEnvironment, _managedItem.OwningFacilityIDs);
				if (!string.IsNullOrEmpty(error))
					errors.Add(error);
			}
			if (errors.Count > 0)
				throw new Exception("Error sending e-mail: " + string.Join(" | ", errors.ToArray()));

			//set the new target state and save the managed item to the database
			_managedItem.State = targetState;
            DeleteAttachments();
            _managedItem.Update(true, Business.Retro.AuditType.Saved);

			//if the "watermark" option is checked for the new State, then delete the Generated Document from the Document Store (e.g., Documentum)
			if (targetState.IsDraft ?? false)
			{
                //KH:Document:New
                foreach (ITATDocument doc in _managedItem.Documents)
                {
                    if (!string.IsNullOrEmpty(doc.GeneratedDocumentID))
                    {
                        Utility.DocumentStorage documentStorageObject = Utility.DocumentStorage.GetDocumentStorageObject(_itatSystem.DocumentStorageType);
                        documentStorageObject.DeleteDocument(doc.GeneratedDocumentID);
                        _managedItem.UpdateGeneratedDocument(string.Empty, doc.ITATDocumentID);
                    }		
                }
			}

			//reload the ManagedItemProfile page (requerying the database to get updated values)
			Server.Transfer(string.Format("ManagedItemProfile.aspx{0}", Utility.TextHelper.QueryString(true, Common.Names._QS_ITAT_SYSTEM_ID, _itatSystem.ID.ToString(), Common.Names._QS_MANAGED_ITEM_ID, _managedItem.ManagedItemID.ToString())), false);
		}

		#endregion
	}
}
