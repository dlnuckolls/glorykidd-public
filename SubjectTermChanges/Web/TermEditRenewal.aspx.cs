using System;
using System.Data;
using System.Configuration;
using System.Collections.Generic;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using Kindred.Knect.ITAT.Business;

namespace Kindred.Knect.ITAT.Web
{
	public partial class TermEditRenewal : BaseTermEditPage
	{

		private const string _KH_CLOSEDIALOG = "_kh_closedialog";


		internal override HtmlGenericControl HTMLBody()
		{
			return this.body;
		}

		protected override TextBox TermNameControl()
		{
			return txtTermName;
		}

		internal override Control ResizablePanel()
		{
			return editBody;
		}

		protected void btnOK_Click(object sender, EventArgs e)
		{
            if (!IsComplexListField)
                SetTermGroupInContext(ddlTermGroup.SelectedIndex);
			SetContextDataAndReturn(txtTermName.Text, true, chkHeaderTerm.Checked);
		}

		protected void btnCancel_Click(object sender, EventArgs e)
		{
			if (ShowTermGroups)
                SetTermGroupInContext((int)ViewState[Common.Names._CNTXT_SelectedTermGroupIndex]);
            SetContextDataAndReturn(txtTermName.Text, false, chkHeaderTerm.Checked);
		}

        protected void btnMap_Click(object sender, EventArgs e)
        {
            if (!ddlMappedTerm.Enabled)
            {
                MapTerm(null, null, ddlMappedTerm.SelectedIndex);
                btnMap.Text = _MAPPING_BUTTON_MAP;
                ddlMappedTerm.Enabled = true;
            }
            else
            {
                if (ddlMappedTerm.SelectedIndex > 0)
                {
                    MapTerm(ddlMappedTerm.Items[ddlMappedTerm.SelectedIndex].Text, GetMappedTermName(ddlMappedTerm.Items[ddlMappedTerm.SelectedIndex].Value), null);
                    btnMap.Text = _MAPPING_BUTTON_UNMAP;
                    ddlMappedTerm.Enabled = false;
                }
            }
        }

		protected override void OnPreRender(EventArgs e)
		{
			base.OnPreRender(e);
			AddClientScripts();
			if (edt.Visible)
			{
				Helper.RegisterParagraphWrapperScript(this);
				Telerik.WebControls.RadEditorUtils.ToolbarDropDown tdd = (Telerik.WebControls.RadEditorUtils.ToolbarDropDown)edt.Toolbars["DynamicToolbar"].Tools["Insert Term"];
				Helper.InitializeToolBarItems(this, tdd);
				Helper.AddSpecialToolBarItems(this, tdd);
				Helper.AddTermsToolBarItems(this, tdd, _template.BasicTerms);
			}
			chkbxEditable.Checked = true;
			RowEditable.Visible = false;
            if (_ddlMapIndex.HasValue)
            {
                ddlMappedTerm.Items[_ddlMapIndex.Value].Value = GetMappedTermValue(_ddlMapIndex.Value, string.Empty);
                ddlMappedTerm.SelectedIndex = 0;
            }
            _ddlMapIndex = null;
        }

		private void AddClientScripts()
		{
			Type t = this.GetType();
			string scriptName = "_kh_ShowHideRenewalType";
			if (!ClientScript.IsClientScriptBlockRegistered(t, scriptName))
			{
				System.IO.StringWriter swClientScript = new System.IO.StringWriter();
				swClientScript.WriteLine("function {0}()", scriptName);
				swClientScript.WriteLine("{");
				swClientScript.WriteLine("	var ddlRenewalType = document.getElementById('{0}');", ddlRenewalType.ClientID);
				swClientScript.WriteLine("	if (ddlRenewalType.options[ddlRenewalType.selectedIndex].text == '{0}')", Business.RenewalTermType.Manual.ToString());
				swClientScript.WriteLine("	{");
				swClientScript.WriteLine("		document.getElementById('fldsetRenewal').style.display='';");
				swClientScript.WriteLine("		document.getElementById('tblRenewal').rows['trPopupTextWhenRenewSelected'].style.display='';");
				swClientScript.WriteLine("	}");
				swClientScript.WriteLine("	else");
				swClientScript.WriteLine("	{");
				swClientScript.WriteLine("		document.getElementById('fldsetRenewal').style.display='none';");
				swClientScript.WriteLine("		document.getElementById('tblRenewal').rows['trPopupTextWhenRenewSelected'].style.display='none';");
				swClientScript.WriteLine("	}");
				swClientScript.WriteLine("}");
				ClientScript.RegisterClientScriptBlock(t, scriptName, swClientScript.ToString(), true);
			}

			scriptName = "_kh_UpdateDropdown";
			if (!ClientScript.IsClientScriptBlockRegistered(t, scriptName))
			{
				System.IO.StringWriter swClientScript = new System.IO.StringWriter();
				swClientScript.WriteLine("function {0}(listBox, dropDown)", scriptName);
				swClientScript.WriteLine("{");
				//Wipeout the contents of the dropdown
				swClientScript.WriteLine("	for (var i = dropDown.options.length - 1;i >= 0;i--)");
				swClientScript.WriteLine("	{");
				swClientScript.WriteLine("		dropDown.options[i] = null;");
				swClientScript.WriteLine("	}");
				//Add selected items from the listbox to the dropdown
				swClientScript.WriteLine("	var dropDownItem = 0");
				swClientScript.WriteLine("	for (var listBoxItem = 0;listBoxItem < listBox.options.length;listBoxItem++)");
				swClientScript.WriteLine("	{");
				swClientScript.WriteLine("		if (listBox.options[listBoxItem].selected)");
				swClientScript.WriteLine("		{");
				swClientScript.WriteLine("			dropDown.options[dropDownItem] = new Option(listBox.options[listBoxItem].text,listBox.options[listBoxItem].value,0,0);");
				swClientScript.WriteLine("			dropDownItem += 1");
				swClientScript.WriteLine("		}");
				swClientScript.WriteLine("	}");
				swClientScript.WriteLine("}");
				ClientScript.RegisterClientScriptBlock(t, scriptName, swClientScript.ToString(), true);
			}

			scriptName = "_kh_ClickSendNotification";
			if (!ClientScript.IsClientScriptBlockRegistered(t, scriptName))
			{
				System.IO.StringWriter swClientScript = new System.IO.StringWriter();
				swClientScript.WriteLine("function {0}()", scriptName);
				swClientScript.WriteLine("{");
				swClientScript.WriteLine("	var checked = document.getElementById('{0}').checked;", chkbxSendNotification.ClientID);
				//swClientScript.WriteLine("	document.getElementById('{0}').disabled = !checked;", ddlSendNotificationStatus.ClientID);
				swClientScript.WriteLine("	document.getElementById('{0}').disabled = !checked;", ddlOffsetTerm.ClientID);
				swClientScript.WriteLine("	document.getElementById('{0}').disabled = !checked;", txtOffsetDefault.ClientID);
				swClientScript.WriteLine("	document.getElementById('{0}').disabled = !checked;", textbxOffsetDays.ClientID);
				swClientScript.WriteLine("	document.getElementById('{0}').disabled = !checked;", textbxSubject.ClientID);
				swClientScript.WriteLine("	document.getElementById('{0}').disabled = !checked;", listbxRecipients.ClientID);
				swClientScript.WriteLine("	document.getElementById('{0}').enabled = checked;", divEditor.ClientID);
				swClientScript.WriteLine("}");
				ClientScript.RegisterClientScriptBlock(t, scriptName, swClientScript.ToString(), true);
			}
		
		}


		private void LoadRenewalTypeListBox(DropDownList dropDown)
		{
			foreach (string renewalTermType in Enum.GetNames(typeof(Business.RenewalTermType)))
			{
				dropDown.Items.Add(new ListItem(renewalTermType, renewalTermType));
			}
		}

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            if (!IsPostBack)
            {
                this.vos.CanEdit = true;
                this.vos.Term = this.Term;
                this.vos.ITATSystem = ITATSystem;
            }
        }
		private void LoadFormatDropDowns()
		{
			DateTime dt = new DateTime(2006, 3, 1);
			foreach (string dateFormat in Utility.DateHelper.DateFormats)
			{
				ddlEffectiveDateFormat.Items.Add(new ListItem(Utility.DateHelper.FormatDate(dt, dateFormat), dateFormat));
				ddlExpirationDateFormat.Items.Add(new ListItem(Utility.DateHelper.FormatDate(dt, dateFormat), dateFormat));
			}
		}



		protected override void InitializeForm()
		{
            header.PageTitle = PageTitle;
            ddlRenewalType.Attributes.Add("onchange", "javascript:return _kh_ShowHideRenewalType();");
			listbxInitialDurationUnit.Attributes.Add("onchange", string.Format("javascript:return _kh_UpdateDropdown(document.getElementById('{0}'),document.getElementById('{1}'));", listbxInitialDurationUnit.ClientID, ddlDefaultInitialDurationUnit.ClientID));
			listbxRenewalDurationUnit.Attributes.Add("onchange", string.Format("javascript:return _kh_UpdateDropdown(document.getElementById('{0}'),document.getElementById('{1}'));", listbxRenewalDurationUnit.ClientID, ddlDefaultRenewalDurationUnit.ClientID));
			this.body.Attributes.Add("onload", "javascript:_kh_ClickSendNotification(); return _kh_ShowHideRenewalType();");
			chkbxSendNotification.Attributes.Add("onclick", "javascript:return _kh_ClickSendNotification();");

            Business.RenewalTerm renewalTerm = Term as Business.RenewalTerm;

			LoadRenewalTypeListBox(ddlRenewalType);
			LoadFormatDropDowns();
			txtTermName.Text = renewalTerm.Name;
			chkbxSendNotification.Checked = renewalTerm.SendNotification ?? true;

            if (ShowTermGroups)
			{
				trTermGroup.Visible = true;
				trHeaderTerm.Visible = true;
                Helper.LoadListControl(ddlTermGroup, _template.GetTermGroups(Business.TermGroup.TermGroupType.AdvancedBasicTerm), "Name", "ID", Term.TermGroupID.ToString(), true, "(Select a Tab)", Guid.Empty.ToString());
			}
			else
			{
				trTermGroup.Visible = false;
				trHeaderTerm.Visible = false;
			}

            if (DisplayTermMapping())
            {
                Helper.LoadListControl(ddlMappedTerm, _systemDBFieldTerms, _DBFIELD_TERM_TEXT, _DBFIELD_TERM_VALUE, _mappedTemplateTermValue, true, "(Select a System Term)", string.Empty);
                btnMap.OnClientClick = string.Format("javascript:return _khb_onMap('{0}','{1}');", ddlMappedTerm.ClientID, TermName);
                trTermMapping.Visible = true;
                if (string.IsNullOrEmpty(_mappedTemplateTermValue))
                {
                    btnMap.Text = _MAPPING_BUTTON_MAP;
                    ddlMappedTerm.Enabled = true;
                }
                else
                {
                    btnMap.Text = _MAPPING_BUTTON_UNMAP;
                    ddlMappedTerm.Enabled = false;
                }
            }
            else
                trTermMapping.Visible = false;
		}


		protected override void UpdateValues()
		{
            Business.RenewalTerm renewalTerm = Term as Business.RenewalTerm;
			if (renewalTerm == null)
                throw new Exception(string.Format("Unable to cast _term \"{0}\" to a Business.RenewalTerm object.", TermName));

			TermName = txtTermName.Text;

            if (!IsComplexListField)
            {
                if (_template.SecurityModel == Business.SecurityModel.Advanced)
                    renewalTerm.TermGroupID = new Guid(ddlTermGroup.SelectedValue);
                else
                    renewalTerm.TermGroupID = _template.BasicSecurityTermGroupID;
            }
			renewalTerm.IsHeader = chkHeaderTerm.Checked;
			renewalTerm.ValidateOnSave = this.vos.Validate;
            renewalTerm.ValidationStatuses = this.vos.SelectedStatuses;

			renewalTerm.Required = true;
			renewalTerm.AllowBackDating = chkbxAllowBackdating.Checked;
			//DEG_20071026  Made the switch - no longer looking at chkbxEditable, but instead looking at chkbxEditableRenewalDurationUnit
			//renewalTerm.Editable = chkbxEditable.Checked;
			renewalTerm.Editable = chkbxEditableRenewalDurationUnit.Checked;
			renewalTerm.KeywordSearchable = chkbxKeywordSearchable.Checked;
			renewalTerm.EffectiveDateFormat = ddlEffectiveDateFormat.SelectedValue;
			renewalTerm.ExpirationDateFormat = ddlExpirationDateFormat.SelectedValue;
			renewalTerm.SendNotification = chkbxSendNotification.Checked;

			renewalTerm.Name = txtTermName.Text;
			renewalTerm.AllowBackDating = chkbxAllowBackdating.Checked;
			renewalTerm.PopUpText = textbxRenewalPopup.Text;
			
			if (renewalTerm.InitialDurationUnits != null)
			{
				renewalTerm.InitialDurationUnits.Clear();
				foreach (ListItem li in listbxInitialDurationUnit.Items)
				{
					if (li.Selected)
					{
						Business.DurationUnit durationUnit = new Kindred.Knect.ITAT.Business.DurationUnit();
						durationUnit.Display = li.Text;
						durationUnit.Value = li.Value;
						durationUnit.Selected = (ddlDefaultInitialDurationUnit.SelectedValue == li.Value);
						durationUnit.Default = (ddlDefaultInitialDurationUnit.SelectedValue == li.Value);
						renewalTerm.InitialDurationUnits.Add(durationUnit);
					}
				}
			}

			renewalTerm.InitialDurationUnitCountDefault = int.Parse(textbxDefaultInitialDuration.Text);
			renewalTerm.InitialDurationUnitPopUpIfNot = chkbxInitialPopUpIfNot.Checked;
			renewalTerm.InitialDurationUnitPopUpText = textbxInitialDurationPopup.Text;

			Business.Event renewalEvent = _template.RenewalEvent;
			Business.Message message = _template.RenewalTermMessage;
			if (renewalEvent != null && message != null)
			{
				message.NotificationStatuses.Clear();
				message.NotificationStatuses.Add(ddlSendNotificationStatus.SelectedValue);
				message.Subject = textbxSubject.Text;
				if (string.IsNullOrEmpty(ddlOffsetTerm.SelectedValue))
					renewalEvent.OffsetTermID = Guid.Empty;
				else
					renewalEvent.OffsetTermID = _template.FindTerm(ddlOffsetTerm.SelectedValue).ID;
				renewalEvent.OffsetDefaultValue = int.Parse(txtOffsetDefault.Text);
				renewalEvent.BaseDateOffset = textbxOffsetDays.Text;
				message.Text = Business.Term.SubstituteTermNames(_template, edt.Html);

				message.Recipients.Clear();
				foreach (ListItem li in listbxRecipients.Items)
					if (li.Selected)
						message.Recipients.Add(li.Text);

                if (ddlFilterFacility.SelectedIndex > 0)
                {
                    string storedFilterFacility = string.Empty;
                    if (message.FilterFacilityTermID.HasValue)
                    {
                        storedFilterFacility = _template.FindTerm(message.FilterFacilityTermID.Value).Name;
                    }
                    if (storedFilterFacility != ddlFilterFacility.SelectedValue)
                    {
                        message.FilterFacilityTermID = _template.FindTerm(ddlFilterFacility.SelectedValue).ID;
                    }
                }
                else
                {
                    message.FilterFacilityTermID = null;
                }
			}

			renewalTerm.RenewalTermType = (Business.RenewalTermType)Enum.Parse(typeof(Business.RenewalTermType), ddlRenewalType.SelectedValue);

			if (renewalTerm.RenewerRoles != null)
			{
				renewalTerm.RenewerRoles.Clear();
				if (renewalTerm.RenewalTermType != Business.RenewalTermType.None)
					foreach (ListItem li in listbxRenewers.Items)
						if (li.Selected)
							renewalTerm.RenewerRoles.Add(new Business.Role(li.Text));
			}

			renewalTerm.RenewalDurationEditable = chkbxEditableRenewalDurationUnit.Checked;

			if (renewalTerm.RenewalDurationUnits != null)
			{
				renewalTerm.RenewalDurationUnits.Clear();
				foreach (ListItem li in listbxRenewalDurationUnit.Items)
				{
					if (li.Selected)
					{
						Business.DurationUnit durationUnit = new Kindred.Knect.ITAT.Business.DurationUnit();
						durationUnit.Display = li.Text;
						durationUnit.Value = li.Value;
						durationUnit.Selected =  (ddlDefaultRenewalDurationUnit.SelectedValue == li.Value);
						durationUnit.Default = (ddlDefaultRenewalDurationUnit.SelectedValue == li.Value);
						renewalTerm.RenewalDurationUnits.Add(durationUnit);
					}
				}
			}

			renewalTerm.RenewalDurationUnitCountDefault = int.Parse(textbxDefaultRenewalDuration.Text);
			renewalTerm.RenewalDurationUnitPopUpIfNot = chkbxRenewalPopUpIfNot.Checked;
			renewalTerm.RenewalDurationUnitPopUpText = textbxRenewalDurationPopup.Text;
		}


		protected override List<string> ValidateForm()
		{
			List<string> rtn = new List<string>();

			Business.RenewalTermType newRenewalTermType = (Business.RenewalTermType)Enum.Parse(typeof(Business.RenewalTermType), ddlRenewalType.SelectedValue);

			//Look at textbxOffsetDays - if not empty, make sure that the comma delimited string of values are all int's
			//20070813_DEG Added check due to Bug #125
			if (string.IsNullOrEmpty(textbxOffsetDays.Text))
				textbxOffsetDays.Text = "0";

			if (string.IsNullOrEmpty(txtOffsetDefault.Text))
				txtOffsetDefault.Text = "0";

			//Validate textbxDefaultInitialDuration
			if (!Utility.TextHelper.IsPositiveInteger(textbxDefaultInitialDuration.Text))
				rtn.Add("The Initial Default Duration entry must be a positive integer.");

			//Validate textbxDefaultRenewalDuration
			if (newRenewalTermType != Business.RenewalTermType.None)
				if (!Utility.TextHelper.IsPositiveInteger(textbxDefaultRenewalDuration.Text))
					rtn.Add("The Renewal Default Duration entry must be a positive integer.");

			//20070817_DEG Related to Bug 125, added more validation concerning the Notification info
			if (chkbxSendNotification.Checked)
			{
				string[] offsetDays = textbxOffsetDays.Text.Split(',');
				foreach (string offsetDay in offsetDays)
					if (!Utility.TextHelper.IsZeroOrPositiveInteger(offsetDay))
					{
						rtn.Add("The Notification OffsetDays entry must be a comma-delimited array of positive integers.");
						break;
					}
				if (string.IsNullOrEmpty(textbxSubject.Text))
					rtn.Add("The Notification Subject must not be blank.");

				int nSelected = 0;
				foreach (ListItem li in listbxRecipients.Items)
					if (li.Selected)
						nSelected++;

				if (nSelected == 0)
					rtn.Add("You must select at least one Notification recipient.");
			}
			
			return rtn;
		}

		protected override void LoadValues()
		{
            Business.RenewalTerm renewalTerm = Term as Business.RenewalTerm;
			if (renewalTerm == null)
                throw new Exception(string.Format("Unable to cast _term \"{0}\" to a Business.RenewalTerm object.", TermName));

			txtTermName.Text = renewalTerm.Name;
			//chkValidateOnSave.Checked = renewalTerm.ValidateOnSave;
			chkHeaderTerm.Checked = renewalTerm.IsHeader;
			chkbxAllowBackdating.Checked = renewalTerm.AllowBackDating ?? false;
			chkbxEditable.Checked = renewalTerm.Editable ?? false;
			textbxRenewalPopup.Text = renewalTerm.PopUpText;

			Helper.LoadListControl(listbxInitialDurationUnit, Business.DurationUnit.All(),   "Display", "Value", null);
			for (int i = 0; i < listbxInitialDurationUnit.Items.Count; i++)
				foreach (Business.DurationUnit du in renewalTerm.InitialDurationUnits)
					if (listbxInitialDurationUnit.Items[i].Value == du.Value)
					{
						listbxInitialDurationUnit.Items[i].Selected = true;
						break;
					}

			Helper.LoadListControl(listbxRenewalDurationUnit, Business.DurationUnit.All(), "Display", "Value", null);
			for (int i = 0; i < listbxRenewalDurationUnit.Items.Count; i++)
			{
				foreach (Business.DurationUnit du in renewalTerm.RenewalDurationUnits)
					if (listbxRenewalDurationUnit.Items[i].Value == du.Value)
					{
						listbxRenewalDurationUnit.Items[i].Selected = true;
						break;
					}
			}

			ddlEffectiveDateFormat.SelectedValue = renewalTerm.EffectiveDateFormat;
			ddlExpirationDateFormat.SelectedValue = renewalTerm.ExpirationDateFormat;

			Helper.LoadListControl(ddlSendNotificationStatus, _itatSystem.Statuses, "Name", "Name", string.Empty, true, "(All Statuses)", Business.XMLNames._M_AllStatuses);
			Helper.LoadListControl(ddlOffsetTerm, BasicTerms.FindTermsOfTypeExcluding(_template.BasicTerms, TermType.PlaceHolderAttachments | TermType.PlaceHolderComments), "Name", "Name", _template.FindTermName(renewalTerm.RenewalEvent.OffsetTermID, renewalTerm.RenewalEvent.OffsetTermName), true, "(None)", string.Empty);
			txtOffsetDefault.Text = renewalTerm.RenewalEvent.OffsetDefaultValue.ToString();

			Helper.LoadListControl(ddlDefaultInitialDurationUnit, renewalTerm.InitialDurationUnits, "Display", "Value", renewalTerm.InitialDurationUnitSelected);
			Helper.LoadListControl(ddlDefaultRenewalDurationUnit, renewalTerm.RenewalDurationUnits, "Display", "Value", renewalTerm.RenewalDurationUnitSelected);
			if (_template.RenewalTermMessage != null)
				Helper.LoadRoles(listbxRecipients, _itatSystem, Business.RoleType.Distribution, _template.RenewalTermMessage.Recipients);
			Helper.LoadRoles(listbxRenewers, _itatSystem, Business.RoleType.Security, renewalTerm.RenewerRoles);

			chkbxEditableRenewalDurationUnit.Checked = renewalTerm.RenewalDurationEditable ?? false;
			chkbxKeywordSearchable.Checked = renewalTerm.KeywordSearchable ?? false;

			textbxDefaultInitialDuration.Text = (renewalTerm.InitialDurationUnitCountDefault ?? 0).ToString();
			chkbxInitialPopUpIfNot.Checked = renewalTerm.InitialDurationUnitPopUpIfNot ?? false;
			textbxInitialDurationPopup.Text = renewalTerm.InitialDurationUnitPopUpText;

			Business.Message message = _template.RenewalTermMessage;
            if (message != null)
            {
                if (message.NotificationStatuses.Count > 0)
                    ddlSendNotificationStatus.SelectedValue = message.NotificationStatuses[0];
                else
                    ddlSendNotificationStatus.SelectedValue = Business.XMLNames._M_AllStatuses;
                textbxOffsetDays.Text = renewalTerm.RenewalEvent.BaseDateOffset;
                textbxSubject.Text = message.Subject;
                edt.Html = (string.IsNullOrEmpty(message.Text) ? Helper.DefaultEditorHtml(edt) : Business.Term.SubstituteTermIDs(_template, message.Text));
                Helper.MarkListBoxMultiSelection(listbxRecipients, message.Recipients);

                if (!(_itatSystem.HasOwningFacility ?? false) && _itatSystem.AllowNotificationFilterFacility)
                {
                    List<Business.Term> facilityTerms = _template.BasicTerms.FindAll(t => t.TermType == Business.TermType.Facility || (t.TransformTermType.HasValue && t.TransformTermType.Value == Business.TermType.Facility));
                    string facilityTermName = string.Empty;
                    if (message.FilterFacilityTermID.HasValue)
                        facilityTermName = _template.FindTermName(message.FilterFacilityTermID.Value, null);
                    Helper.LoadListControl(ddlFilterFacility, facilityTerms, "Name", "Name", facilityTermName, true, "(Select a Facility Term)", "");
                    row_ddlFilterFacility.Visible = true;
                }
                else
                {
                    ddlFilterFacility.SelectedIndex = -1;
                    row_ddlFilterFacility.Visible = false;
                }
            }

			ddlRenewalType.SelectedValue = renewalTerm.RenewalTermType.ToString();
			//chkbxEditableRenewalDurationUnit.Checked = RenewalTerm.RenewalDurationUnitEditable ?? false;

			textbxDefaultRenewalDuration.Text = (renewalTerm.RenewalDurationUnitCountDefault ?? 0).ToString();;
			chkbxRenewalPopUpIfNot.Checked = renewalTerm.RenewalDurationUnitPopUpIfNot ?? false;
			textbxRenewalDurationPopup.Text = renewalTerm.RenewalDurationUnitPopUpText;
		}


		protected override void ShowHideFields()
		{
		}


	}
}
