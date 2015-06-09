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
using System.Collections.Generic;
using Kindred.Knect.ITAT.Business;

namespace Kindred.Knect.ITAT.Web
{
	public partial class TemplateNotifications : BaseTemplatePage
	{

		#region private members

		private const string _KH_VSKEY_PREVSELECTEDINDEX= "_kh_PreviousSelectedIndex";
		private int _prevSelectedIndex;

		#endregion

		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);
			if (IsPostBack)
			{
				if (_prevSelectedIndex > -1)
					UpdateNotification();
			}
			else
			{
				LoadDateTermsAndNotifications();
				tblProperties.Visible = false;
				_prevSelectedIndex = -1;
				IsChanged = false;
			}
		}


		protected override void OnPreRender(EventArgs e)
		{
			RegisterDeleteConfirmation();
			if (edt.Visible)
			{
				Helper.RegisterParagraphWrapperScript(this);
				Telerik.WebControls.RadEditorUtils.ToolbarDropDown tdd = (Telerik.WebControls.RadEditorUtils.ToolbarDropDown)edt.Toolbars["DynamicToolbar"].Tools["Insert Term"];
				Helper.InitializeToolBarItems(this, tdd);
				Helper.AddSpecialToolBarItems(this, tdd);
				Helper.AddTermsToolBarItems(this, tdd, _template.BasicTerms);
			}
			SuppressChangeNotification(lstNotifications);
			_prevSelectedIndex = lstNotifications.SelectedIndex;
			//If the focus is set to txtName, it always ends up at the rad editor.  
			//Therefore, set it to the listbox and at least the rad editor does not end up with it.
			//Note - The rad editor "FocusOnload" call does not seem to make any difference.
			lstNotifications.Focus();
			base.OnPreRender(e);
		}

		protected override object SaveViewState()
		{
			ViewState[_KH_VSKEY_PREVSELECTEDINDEX] = _prevSelectedIndex;
			return base.SaveViewState();
		}


		protected override void LoadViewState(object savedState)
		{
			base.LoadViewState(savedState);
			_prevSelectedIndex = (int)ViewState[_KH_VSKEY_PREVSELECTEDINDEX];
		}


		internal override Control ResizablePanel()
		{
			return divEditor;
		}


		internal override HtmlGenericControl HTMLBody()
		{
			return this.htmlBody;
		}


		protected override TemplateHeader HeaderControl()
		{
			return (TemplateHeader)header;
		}



		private void RegisterDeleteConfirmation()
		{
			//Set up client-side script to prompt the user if they click the Delete button
			btnDeleteNotification.OnClientClick = "return confirm('Are you sure you want to delete this notification?');";
		}


		protected void OnHeaderEvent(object sender, HeaderEventArgs e)
		{
			switch (e.CommandName)
			{
				case Common.Names._HEADER_EVENT_Save:
			        _template.SaveLoaded(true, true, "Notifications");
					if (_template.IsManagedItem)
						RegisterAlert(string.Format("{0} Notifications have been saved.", this._itatSystem.ManagedItemName));
					else
						RegisterAlert("Template Notifications have been saved.");
					IsChanged = false;
					break;
				case Common.Names._HEADER_EVENT_Reset:
					GetTemplate(true);
				    LoadDateTermsAndNotifications();
                    edt.Html = "";
				    tblProperties.Visible = false;
				    _prevSelectedIndex = -1;
					IsChanged = false;
					break;
				default:
					base.HandleHeaderEvent(sender, e);
					break;
			}
		}

		private void UpdateNotification()
		{
			bool dataChanged = false;
			string selectedValue = lstNotifications.Items[_prevSelectedIndex].Value;
			string[] segments = selectedValue.Split('|');
			Business.EventType notificationType = Business.EventTypeHelper.GetNotificationType(segments[0]);
			if (notificationType == Kindred.Knect.ITAT.Business.EventType.Custom)
			{
				int index = int.Parse(segments[2]);
				Business.Event notification = _template.Events[index];

				if (notification.Name != txtName.Text)
				{
					dataChanged = true;
					notification.Name = txtName.Text;
					lstNotifications.Items[_prevSelectedIndex].Text = txtName.Text;
					lstNotifications.Items[_prevSelectedIndex].Value = string.Format("{0}|{1}|{2}", segments[0], txtName.Text, segments[2]);
				}

				string baseDateTermSelectedValue = ddlBaseDate.SelectedValue;
				if (!string.IsNullOrEmpty(baseDateTermSelectedValue))
				{
					string[] dateTermParts = baseDateTermSelectedValue.Split(new string[] { Business.XMLNames._M_NotificationBaseDateTermSeparator }, StringSplitOptions.RemoveEmptyEntries);
					string baseDateTermName = dateTermParts[0];
					string baseDateTermPart = string.Empty;
					if (dateTermParts.Length > 1)
						baseDateTermPart = dateTermParts[1];

					if (!_template.MatchTerm(notification.BaseDateTermID, notification.BaseDateTermName, baseDateTermName))
					{
						dataChanged = true;
						if (!string.IsNullOrEmpty(baseDateTermName))
							notification.BaseDateTermID = _template.FindTerm(baseDateTermName).ID;
					}

					if (notification.BaseDateTermPart != baseDateTermPart)
					{
						dataChanged = true;
						notification.BaseDateTermPart = baseDateTermPart;
					}
				}

				if (notification.BaseDateOffset != null || (notification.BaseDateOffset != txtDateOffset.Text))
				{
					dataChanged = true;
					notification.BaseDateOffset = txtDateOffset.Text;
				}

				Business.Term offsetTerm = _template.FindTerm(notification.OffsetTermID, notification.OffsetTermName);
				if ((offsetTerm == null) || ((offsetTerm != null && (offsetTerm.Name != ddlOffsetTerm.SelectedValue))))
				{
					dataChanged = true;
					Business.Term newOffsetTerm = _template.FindTerm(ddlOffsetTerm.SelectedValue);
					if (newOffsetTerm == null)
						notification.OffsetTermID = Guid.Empty;
					else
						notification.OffsetTermID = newOffsetTerm.ID;
				}

				if (notification.OffsetDefaultValue.ToString() != txtOffsetDefault.Text)
				{
					dataChanged = true;
					int value = 0;
					int.TryParse(txtOffsetDefault.Text, out value);
					notification.OffsetDefaultValue = value;
				}

				Business.Message message = notification.Messages[0];
				if (message != null)
				{
					if ((message.NotificationStatuses.Count == 0) || (message.NotificationStatuses[0] != ddlSendNotificationStatus.SelectedValue))
					{
						dataChanged = true;
						message.NotificationStatuses.Clear();
						message.NotificationStatuses.Add(ddlSendNotificationStatus.SelectedValue);
					}

					if (message.Subject != txtSubject.Text)
					{
						dataChanged = true;
						message.Subject = txtSubject.Text;
					}

					if (message.Text != edt.Xhtml)
					{
						dataChanged = true;
						message.Text = Business.Term.SubstituteTermNames(_template, edt.Xhtml);
					}

                    if (ddlFilterFacility.SelectedIndex > 0)
                    {
                        string storedFilterFacility = string.Empty;
                        if (message.FilterFacilityTermID.HasValue)
                        {
                            storedFilterFacility = _template.FindTerm(message.FilterFacilityTermID.Value).Name;
                        }
                        if (storedFilterFacility != ddlFilterFacility.SelectedValue)
                        {
                            dataChanged = true;
                            message.FilterFacilityTermID = _template.FindTerm(ddlFilterFacility.SelectedValue).ID;
                        }
                    }
                    else
                    {
                        if (message.FilterFacilityTermID.HasValue)
                        {
                            dataChanged = true;
                            message.FilterFacilityTermID = null;
                        }
                    }
				}
				else
				{
					dataChanged = true;
					message.NotificationStatuses.Clear();
					message.NotificationStatuses.Add(ddlSendNotificationStatus.SelectedValue);
					message.Subject = txtSubject.Text;
					message.Text = Business.Term.SubstituteTermNames(_template, edt.Xhtml);
				}

				dataChanged = Helper.UpdateList(lstRecipients, message.Recipients) || dataChanged;

				if (dataChanged)
					IsChanged = true;
			}
		}


		private void LoadDateTermsAndNotifications()
		{
			lstNotifications.Items.Clear();

			List<Business.Term> dateTerms = _template.FindAllBasicTerms(Business.TermType.Date);
			Helper.LoadListControl(ddlBaseDate, dateTerms, "Name", "Name", "", true, "(Select a Date Term)", "");

			//add custom notifications
			for (int index = 0, max = _template.Events.Count; index < max; index++)
			{
				if (_template.Events[index].EventType == Kindred.Knect.ITAT.Business.EventType.Custom)
				{
					string notificationName = _template.Events[index].Name;
					lstNotifications.Items.Add(new ListItem(notificationName, string.Format("{0}|{1}|{2}", Business.EventTypeHelper.Value(Business.EventType.Custom), notificationName, index)));
				}
			}

			//On 12/19/2006, discussed this with Roland.  Decided for now that we will
			//go with the 'standard' renewal message, and not add a date term for the renewal term to this dropdown.

			//Add renewal notification (if a renewal term exists)
			//TODO - Is it always true that a renewal message will exist if the renewal term is defined?
			if (_template.RenewalDefined)
			{
				//if renewal term exists, add renewal notification to list box and add term to "date" dropdown
				Business.RenewalTerm renewalTerm = _template.FindBasicTerm(Business.TermType.Renewal) as Business.RenewalTerm;
				if (renewalTerm != null)
				{
					//20070820_DEG  In support of Bug 125, we should 'hide' the renewal term message if SendNotification == false.
					//if (renewalTerm.SendNotification ?? true)
					//{
					//    ddlBaseDate.Items.Add(new ListItem(renewalTerm.Name, renewalTerm.Name));
					//    string itemText = string.Format("({0} - {1})", renewalTerm.Name, "Renewal Term");
					//    string itemValue = string.Format("{2}|{0}|{1}", renewalTerm.Name, "Renewal Term", Business.EventTypeHelper.Value(Business.EventType.Renewal));
					//    lstNotifications.Items.Add(new ListItem(itemText, itemValue));
					//}

					//20071026_DEG  In support of Bug 196 - add the renewal term to the dropdown, don't display the notification if renewalTerm.SendNotification == false.
					ddlBaseDate.Items.Add(new ListItem(string.Format("{0} (Effective)", renewalTerm.Name), string.Format("{0}{1}{2}", renewalTerm.Name, Business.XMLNames._M_NotificationBaseDateTermSeparator, Business.XMLNames._TPS_EffectiveDate)));
					ddlBaseDate.Items.Add(new ListItem(string.Format("{0} (Expiration)", renewalTerm.Name), string.Format("{0}{1}{2}", renewalTerm.Name, Business.XMLNames._M_NotificationBaseDateTermSeparator, Business.XMLNames._TPS_ExpirationDate)));
					if (renewalTerm.SendNotification ?? true)
					{
						string itemText = string.Format("({0} - {1})", renewalTerm.Name, "Renewal Term");
						string itemValue = string.Format("{2}|{0}|{1}", renewalTerm.Name, "Renewal Term", Business.EventTypeHelper.Value(Business.EventType.Renewal));
						lstNotifications.Items.Add(new ListItem(itemText, itemValue));
					}
				}
			}

			//add notifications due to workflow
			foreach (Business.State state in _template.Workflow.States)
			{
				foreach (Business.Action action in state.Actions)
				{
					string itemText = string.Format("({0} - {1})", state.Name, action.ButtonText);
					string itemValue = string.Format("{2}|{0}|{1}", state.Name, action.TargetState, Business.EventTypeHelper.Value(Business.EventType.Workflow));
					lstNotifications.Items.Add(new ListItem(itemText, itemValue));
				}
			}

			//add workflow "scheduled event notifications"
			if (_template.Workflow.UseFunction ?? false)
			{
				string itemText = string.Format("({0}-day approval window)", (_template.Workflow.DaysAfterWorkflowEntry ?? 0));
				string itemValue = string.Format("{0}||", Business.EventTypeHelper.Value(Business.EventType.WorkflowRevertToDraft));
				lstNotifications.Items.Add(new ListItem(itemText, itemValue));
			}
        }


		protected void btnAddNotification_OnCommand(object sender, CommandEventArgs e)
		{
			Business.Event notification = new Kindred.Knect.ITAT.Business.Event(Kindred.Knect.ITAT.Business.EventType.Custom, _template.IsManagedItem);
			notification.Name = "New Notification";
			notification.BaseDateTermID = Guid.Empty;
			notification.BaseDateOffset = "";

			Business.Message message = new Kindred.Knect.ITAT.Business.Message();
			message.NotificationStatuses.Clear();
			message.NotificationStatuses.Add(Business.XMLNames._M_AllStatuses);
			message.Subject = string.Empty;
			message.Text = Helper.DefaultEditorHtml(this.edt);
			notification.Messages.Add(message);

			_template.Events.Add(notification);
			LoadDateTermsAndNotifications();

			int nIndex = 0;
			foreach (ListItem item in lstNotifications.Items)
			{
				if (item.Text == "New Notification")
				{
					lstNotifications.SelectedIndex = nIndex;
					btnDeleteNotification.Enabled = true;
					string[] segments = lstNotifications.SelectedValue.Split('|');
					//segments[0] indicates NotificationType
					//on workflow notifications, segments[1] = state name, segments[2] = action TargetState
					Business.EventType notificationType = Business.EventTypeHelper.GetNotificationType(segments[0]);
					SetPropertyValues(notificationType, lstNotifications.SelectedItem.Text, segments);
					break;
				}
				nIndex++;
			}
			IsChanged = true;
		}


		protected void btnDeleteNotification_OnCommand(object sender, CommandEventArgs e)
		{
			if (lstNotifications.SelectedIndex > -1)
			{
				string[] segments = lstNotifications.SelectedValue.Split('|');
				Business.EventType notificationType = Business.EventTypeHelper.GetNotificationType(segments[0]);
				if (notificationType != Kindred.Knect.ITAT.Business.EventType.Custom)
					throw new Exception("The selected notification is NOT a Custom Notification.  This should not happen.");

				Business.Event ev = _template.FindEvent(segments[1]);
				if (ev != null)
				{
					_template.Events.Remove(ev);
					LoadDateTermsAndNotifications();
					SetPropertyValues(Business.EventType.None, null, null);
					IsChanged = true;
				}
				else
				{
					throw new Exception(string.Format("Unable to find Custom Notification called '{0}'.", segments[1]));
				}
			}
		}

		protected void lstNotifications_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (lstNotifications.SelectedIndex > -1)
			{
				//segments[0] indicates NotificationType on workflow notifications, 
				//segments[1] = state name, 
				//segments[2] = action TargetState
				string[] segments = lstNotifications.SelectedValue.Split('|');
				Business.EventType notificationType = Business.EventTypeHelper.GetNotificationType(segments[0]);
				SetPropertyValues(notificationType, lstNotifications.SelectedItem.Text, segments);
				btnDeleteNotification.Enabled = (notificationType == Business.EventType.Custom);
			}
			else
			{
				SetPropertyValues(Business.EventType.None, null, null);
				btnDeleteNotification.Enabled = false;
			}
		}

		private void SetPropertyValues(Business.EventType notificationType, string selectedText,  string[] segments)
		{
			switch (notificationType)
			{
				case Kindred.Knect.ITAT.Business.EventType.Custom:
					{
						tblProperties.Visible = true;
						edt.Visible = true;
						int notificationIndex = int.Parse(segments[2]);
						Business.Event notification = _template.Events[notificationIndex];
						txtName.Text = notification.Name;
						string baseDateTermName = _template.FindTermName(notification.BaseDateTermID, notification.BaseDateTermName);
						SelectBaseDateListItem(baseDateTermName, notification.BaseDateTermPart);		
					
						txtDateOffset.Text = notification.BaseDateOffset;
						Business.Message message = notification.Messages[0];

						ddlSendNotificationStatus.Style["visibility"] = "visible";
						Helper.LoadListControl(ddlSendNotificationStatus, _itatSystem.Statuses, "Name", "Name", string.Empty, true, "(All Statuses)", Business.XMLNames._M_AllStatuses);
						if (message.NotificationStatuses.Count > 0)
							ddlSendNotificationStatus.SelectedValue = message.NotificationStatuses[0];

                        SetUpFilterFaciltyDropdown(true, message.FilterFacilityTermID);

						Helper.LoadRoles(lstRecipients, _itatSystem, Business.RoleType.Distribution, message.Recipients);
						txtSubject.Text = message.Subject;
						edt.Html = Business.Term.SubstituteTermIDs(_template, message.Text);
						EnableProperties(true);
						txtDateOffset.Style["visibility"] = "visible";
						ddlBaseDate.Style["visibility"] = "visible";
						ddlOffsetTerm.Style["visibility"] = "visible";
						txtOffsetDefault.Style["visibility"] = "visible";
						Helper.LoadListControl(ddlOffsetTerm, BasicTerms.FindTermsOfTypeExcluding(_template.BasicTerms, TermType.PlaceHolderAttachments | TermType.PlaceHolderComments), "Name", "Name", _template.FindTermName(notification.OffsetTermID, notification.OffsetTermName), true, "(None)", string.Empty);
						txtOffsetDefault.Text = notification.OffsetDefaultValue.ToString();
						break;
					}

				case Kindred.Knect.ITAT.Business.EventType.Workflow:
					{
						tblProperties.Visible = true;
						edt.Visible = true;
						string stateName = segments[1];
						string targetState = segments[2];
                        Business.Message message = _template.Workflow.FindState(stateName).FindAction(targetState).Messages[0];
						ddlSendNotificationStatus.Style["visibility"] = "hidden";
						txtName.Text = selectedText;
						Helper.LoadRoles(lstRecipients, _itatSystem, Business.RoleType.Distribution, message.Recipients);
						txtSubject.Text = message.Subject;
						txtDateOffset.Text = "";
                        SetUpFilterFaciltyDropdown(false, null);
						ddlBaseDate.SelectedIndex = -1;
						edt.Html = Business.Term.SubstituteTermIDs(_template, message.Text);
						EnableProperties(false);
						txtDateOffset.Style["visibility"] = "hidden";
                        ddlBaseDate.Style["visibility"] = "hidden";
						ddlOffsetTerm.Style["visibility"] = "hidden";
						txtOffsetDefault.Style["visibility"] = "hidden";
						break;
					}

				case Kindred.Knect.ITAT.Business.EventType.WorkflowRevertToDraft:
					{
						tblProperties.Visible = true;
						edt.Visible = true;
						Business.Message message = _template.WorkflowRevertMessage;
						if (message != null)
						{
							Helper.LoadRoles(lstRecipients, _itatSystem, Business.RoleType.Distribution, message.Recipients);
							txtSubject.Text = message.Subject;
							edt.Html = Business.Term.SubstituteTermIDs(_template, message.Text);
						}
						txtName.Text = selectedText;
						ddlSendNotificationStatus.Style["visibility"] = "hidden";
						txtDateOffset.Text = "";
                        SetUpFilterFaciltyDropdown(false, null);
						ddlBaseDate.SelectedIndex = -1;
						EnableProperties(false);
						txtDateOffset.Style["visibility"] = "hidden";
                        ddlBaseDate.Style["visibility"] = "hidden";
						ddlOffsetTerm.Style["visibility"] = "hidden";
						txtOffsetDefault.Style["visibility"] = "hidden";
						break;
					}

				case Kindred.Knect.ITAT.Business.EventType.Renewal:
					{
						tblProperties.Visible = true;
						edt.Visible = true;
						Business.RenewalTerm renewalTerm = (Business.RenewalTerm)_template.FindAllBasicTerms(Kindred.Knect.ITAT.Business.TermType.Renewal)[0];
						Business.Message message = _template.RenewalTermMessage;
						txtName.Text = selectedText;

						ddlSendNotificationStatus.Style["visibility"] = "visible";
						Helper.LoadListControl(ddlSendNotificationStatus, _itatSystem.Statuses, "Name", "Name", string.Empty, true, "(All Statuses)", Business.XMLNames._M_AllStatuses);
						if (message.NotificationStatuses.Count > 0)
							ddlSendNotificationStatus.SelectedValue = message.NotificationStatuses[0];

                        SetUpFilterFaciltyDropdown(true, message.FilterFacilityTermID);

						Helper.LoadRoles(lstRecipients, _itatSystem, Business.RoleType.Distribution, message.Recipients);
						txtSubject.Text = message.Subject;
						edt.Html = Business.Term.SubstituteTermIDs(_template, message.Text);
						SelectBaseDateListItem(renewalTerm.Name, Business.XMLNames._TPS_ExpirationDate);
						Helper.LoadListControl(ddlOffsetTerm, BasicTerms.FindTermsOfTypeExcluding(_template.BasicTerms, TermType.PlaceHolderAttachments | TermType.PlaceHolderComments), "Name", "Name", _template.FindTermName(renewalTerm.RenewalEvent.OffsetTermID, renewalTerm.RenewalEvent.OffsetTermName), true, "(All)", string.Empty);
						txtOffsetDefault.Text = renewalTerm.RenewalEvent.OffsetDefaultValue.ToString();
						txtDateOffset.Text = renewalTerm.RenewalEvent.BaseDateOffset;

						txtDateOffset.Style["visibility"] = "visible";
                        ddlBaseDate.Style["visibility"] = "visible";
						ddlOffsetTerm.Style["visibility"] = "visible";
						txtOffsetDefault.Style["visibility"] = "visible";
						EnableProperties(false);
						break;
					}

				default:
					tblProperties.Visible = false;
					edt.Visible = false;
					break;
			}
		}

        private void SetUpFilterFaciltyDropdown(bool visible, Guid? filterFacilityTermID)
        {
            if (!(_itatSystem.HasOwningFacility ?? false) && visible && _itatSystem.AllowNotificationFilterFacility)
            {
                row_lstNotifications.Height = "280px";
                List<Business.Term> facilityTerms = _template.BasicTerms.FindAll(t => t.TermType == TermType.Facility || (t.TransformTermType.HasValue && t.TransformTermType.Value == TermType.Facility));
                string facilityTermName = string.Empty;
                if (filterFacilityTermID.HasValue)
                    facilityTermName = _template.FindTermName(filterFacilityTermID.Value, null);
                Helper.LoadListControl(ddlFilterFacility, facilityTerms, "Name", "Name", facilityTermName, true, "(Select a Facility Term)", "");
                row_ddlFilterFacility.Visible = true;
            }
            else
            {
                row_lstNotifications.Height = "202px";
                ddlFilterFacility.SelectedIndex = -1;
                row_ddlFilterFacility.Visible = false;
            }
        }

		private void SelectBaseDateListItem(string termName, string termPart)
		{
			//Find and select the item in ddlBaseDate whose Value matches BaseDateTermName and BaseDateTermPart ("Expiration" is the default BaseDateTermPart)
			ddlBaseDate.ClearSelection();
			foreach (ListItem item in ddlBaseDate.Items)
			{
				if (string.IsNullOrEmpty(item.Value))
					continue;
				string[] valueParts = item.Value.Split(new string[] { Business.XMLNames._M_NotificationBaseDateTermSeparator }, StringSplitOptions.RemoveEmptyEntries);
				if (valueParts[0] == termName)
				{
					if (string.IsNullOrEmpty(termPart))
					{
						if ((valueParts.Length == 1) || ((valueParts.Length == 2) && valueParts[1] == Business.XMLNames._TPS_ExpirationDate))
						{
							item.Selected = true;
							break;
						}
					}
					else
						if ((valueParts.Length == 2) && (valueParts[1] == termPart))
						{
							item.Selected = true;
							break;
						}
				}
			}  //foreach
		}

		private void EnableProperties(bool enabled)
		{
			txtName.Enabled = enabled;
			ddlSendNotificationStatus.Enabled = enabled;
			lstRecipients.Enabled = enabled;
			txtSubject.Enabled = enabled;
            ddlFilterFacility.Enabled = enabled;
			ddlBaseDate.Enabled = enabled;
			txtDateOffset.Enabled = enabled;
			edt.Enabled = enabled;
			ddlOffsetTerm.Enabled = enabled;
			txtOffsetDefault.Enabled = enabled;
		}
	}
}
