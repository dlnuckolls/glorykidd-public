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


namespace Kindred.Knect.ITAT.Web
{


	public partial class RenewalTermControl : BaseProfileControl
	{

		#region private members
		private Business.RenewalTerm _renewalTerm;
		private string _dateFormat;
		private const string CONTROL_EVENT_PREFIX = "_kh_RenewalPopup_";
		#endregion

		#region properties

		public string DateFormat
		{
			get { return _dateFormat; }
			set { _dateFormat = value; }
		}

		#endregion


		#region base class overrides


		public override Kindred.Knect.ITAT.Business.Term Term
		{
			get
			{
				return _renewalTerm;
			}
			set
			{
				_renewalTerm = value as Business.RenewalTerm;
				if (_renewalTerm == null)
					throw new NullReferenceException("Unable to cast Term as a RenewalTerm");
			}
		}

		#endregion



		#region control lifecycle events

		protected override void OnInit(EventArgs e)
		{
			base.OnInit(e);
		}

		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);
			_renewalTerm = (Business.RenewalTerm)this.Term;
			if (!IsPostBack)
				SetChildControlsInitialValues();
		}

		protected override void OnPreRender(EventArgs e)
		{
			base.OnPreRender(e);
            List<string> tryShowRenewButton = TryShowRenewButton();
            SetChildControlVisibility(tryShowRenewButton);
			SetCaptions();
			RegisterChildControlEventHandlers();
            RegisterClientSideScripts(tryShowRenewButton);
		}

		protected void btnRenewOnClick(object sender, EventArgs e)
		{
			BaseManagedItemPage page = this.Page as BaseManagedItemPage;
			if (page == null)
				throw new Exception("The Renew button should only appear on a BaseManagedItemPage (such as the Profile page)");

			_renewalTerm.Renew();
			//save the ManagedItem
            page.ManagedItem.Update(false, Business.Retro.AuditType.Saved);
			Server.Transfer(string.Format("ManagedItemProfile.aspx{0}", Utility.TextHelper.QueryString(true, Common.Names._QS_ITAT_SYSTEM_ID, page.ITATSystem.ID.ToString(), Common.Names._QS_MANAGED_ITEM_ID, page.ManagedItem.ManagedItemID.ToString())), false);
		}


		#endregion



		#region private methods

		private void SetChildControlsInitialValues()
		{
			LoadDropDowns();
			if (_renewalTerm.EffectiveDate.HasValue)
				txtEffectiveDate.Text = Utility.DateHelper.FormatDate(_renewalTerm.EffectiveDate.Value, _dateFormat);
			if (_renewalTerm.InitialDurationUnitCount.HasValue)
				txtInitialDurationCount.Text = _renewalTerm.InitialDurationUnitCount.Value.ToString();
			if (!string.IsNullOrEmpty(_renewalTerm.InitialDurationUnitSelected))
				ddlInitialDurationUnits.SelectedValue = _renewalTerm.InitialDurationUnitSelected;
			if (!_renewalTerm.IsTypeNone)
			{
				if (_renewalTerm.RenewalDurationUnitCount.HasValue)
					txtRenewalDurationCount.Text = _renewalTerm.RenewalDurationUnitCount.Value.ToString();
				if (!string.IsNullOrEmpty(_renewalTerm.InitialDurationUnitSelected))
					ddlRenewalDurationUnits.SelectedValue = _renewalTerm.RenewalDurationUnitSelected;
			}
		}


		private void RegisterChildControlEventHandlers()
		{
			imgEffectiveDate.Attributes["onclick"] = string.Format("window.event.cancelBubble=true; ShowCalendar(document.all.CalFrame, window.frames.CalFrame, null, document.getElementById('{0}'), null, -600, 3600);", txtEffectiveDate.ClientID);

			if (pnlRenewButton.Visible)
				RegisterRenewalConfirmation();
		}


        private void SetChildControlVisibility(List<string> tryShowRenewButton)
		{
			//If the managed item is not renewable hide the Renewal Duration row
			divRenewalDuration.Visible = !_renewalTerm.IsTypeNone;

            pnlRenewButton.Visible = tryShowRenewButton.Count == 0;

            if (tryShowRenewButton.Count > 0)
            {
                BaseManagedItemPage page = this.Page as BaseManagedItemPage;
                if (page != null)
                {
                    pnlRenewButtonHidden.Visible = page.ITATSystem.UserIsInRole(Business.XMLNames._AF_ITATAdmin);
                }
            }

			lblEffectiveDateValue.Visible = !CanEdit;
			lblInitialDurationValue.Visible = !CanEdit;

			txtEffectiveDate.Visible = CanEdit;
			txtInitialDurationCount.Visible = CanEdit;
			imgEffectiveDate.Visible = CanEdit;
			ddlInitialDurationUnits.Visible = CanEdit;

			lblRenewalDurationValue.Visible = !CanEdit;
			txtRenewalDurationCount.Visible = CanEdit;
			ddlRenewalDurationUnits.Visible = CanEdit;
			lblRenewalCount.Visible = ((_renewalTerm.RenewalCount ?? 0) > 0);
		}

        private List<string> TryShowRenewButton()
        {
            List<string> errors = new List<string>();
            //we must be on a MangedItem page (such as the Profile page) to show the Renew button
            BaseManagedItemPage page = this.Page as BaseManagedItemPage;
            if (page == null)
                errors.Add("The source page is not a BaseManagedItemPage");
            if (_renewalTerm.IsTypeNone)
                errors.Add("The renewal term has notification type *None* defined");
            Business.ManagedItem mi = page.ManagedItem;

            //verify that the managed item status is correct
            if (mi.RenewalTermMessage == null)
                errors.Add("The renewal term message is not defined");

            if (!mi.RenewalTermMessage.AllStatusesValid)
            {
                if (mi.RenewalTermMessage.NotificationStatuses == null || mi.RenewalTermMessage.NotificationStatuses.Count == 0)
                    errors.Add("There are no renewal term Notification Statuses defined");

                if (mi.State == null)
                    errors.Add("The item state is not defined");
                else
                {
                    if (mi.RenewalTermMessage.NotificationStatuses != null && mi.RenewalTermMessage.NotificationStatuses.Count > 0 && mi.State.Status != mi.RenewalTermMessage.NotificationStatuses[0])
                        errors.Add(string.Format("The item status of *{0}* does not match the Notification Status of *{1}*", mi.State.Status, mi.RenewalTermMessage.NotificationStatuses[0]));
                }
            }

            //verify that the user is in a proper role (and if IsOwningFacility == true, the correct facility)
            if (page.ITATSystem.HasOwningFacility ?? false)
            {
                if (!page.SecurityHelper.CanPerformFunction(_renewalTerm.RenewerRoles, page.ManagedItem.OwningFacilityIDs))
                {
                    if (_renewalTerm.RenewerRoles.Count > 0)
                    {
                        string listRenewerRoles = string.Join(",", _renewalTerm.RenewerRoles.ConvertAll<string>(Business.Role.StringConverter).ToArray());
                        errors.Add(string.Format("The user must belong to at least one role from *{0}*", listRenewerRoles));
                    }
                    else
                    {
                        errors.Add("There are no Renewer Roles defined");
                    }

                    if (page.ManagedItem.OwningFacilityIDs.Count > 0)
                    {
                        string owningFacilityIDs = string.Join(",", page.ManagedItem.OwningFacilityIDs.ConvertAll<string>(delegate(int i) { return i.ToString(); }).ToArray());
                        errors.Add(string.Format("The user must belong to at least one owning facility from *{0}*", owningFacilityIDs));
                    }
                    else
                    {
                        errors.Add("There are no Owning Facilities defined");
                    }
                }
            }
            else
            {
                if (!page.SecurityHelper.CanPerformFunction(_renewalTerm.RenewerRoles))
                {
                    if (_renewalTerm.RenewerRoles.Count > 0)
                    {
                        string listRenewerRoles = string.Join(",", _renewalTerm.RenewerRoles.ConvertAll<string>(Business.Role.StringConverter).ToArray());
                        errors.Add(string.Format("The user must belong to at least one role from *{0}*", listRenewerRoles));
                    }
                    else
                    {
                        errors.Add("There are no Renewer Roles defined");
                    }
                }
            }

            if (_renewalTerm.RenewalEvent == null)
                errors.Add("The RenewalEvent is not defined");

            if (_renewalTerm.RenewalEvent.ScheduledEvents == null)
                errors.Add("The ScheduledEvents are missing");

            if (!_renewalTerm.ExpirationDate.HasValue)
                errors.Add("The ExpirationDate is not defined");

            List<DateTime> eventDates = _renewalTerm.GetEventDates();
            if (eventDates == null || eventDates.Count == 0)
                errors.Add("The renewal term event dates were not defined (this could be due to an undefined RenewalEvent, an undefined Expiration Date, or missing ScheduledEvents)");

            int offsetTermValue = _renewalTerm.RenewalEvent.OffsetDefaultValue;
            try
            {
                offsetTermValue = int.Parse(mi.FindTerm(_renewalTerm.RenewalEvent.OffsetTermID, _renewalTerm.RenewalEvent.OffsetTermName).DisplayValue(""));
            }
            catch { }

            try
            {
                DateTime earliestDate = DateTime.MaxValue;
                foreach (DateTime eventDate in eventDates)
                {
                    DateTime dtTest = eventDate.AddDays(-1 * offsetTermValue);
                    if (dtTest < earliestDate)
                        earliestDate = dtTest;
                }
                if (earliestDate > DateTime.Today)
                    errors.Add(string.Format("The anticipated send date is *{0}* which is after todays date *{1}*", earliestDate.ToString("d"), DateTime.Today.ToString("d")));
            }
            catch { }

            return errors;
        }


		private void SetCaptions()
		{
			if (!CanEdit)
			{
				//show read-only values
				if (_renewalTerm.EffectiveDate.HasValue)
					lblEffectiveDateValue.Text = Utility.DateHelper.FormatDate(_renewalTerm.EffectiveDate.Value, _dateFormat);

				if (_renewalTerm.InitialDurationUnitCount.HasValue && (!string.IsNullOrEmpty(_renewalTerm.InitialDurationUnitSelected)))
				{
					string durationUnits = _renewalTerm.InitialDurationUnitSelected + (_renewalTerm.InitialDurationUnitCount.Value == 1? "" : "s");
					lblInitialDurationValue.Text = string.Format("{0} {1}", _renewalTerm.InitialDurationUnitCount.Value, durationUnits);
				}

				if (_renewalTerm.RenewalDurationUnitCount.HasValue && (!string.IsNullOrEmpty(_renewalTerm.RenewalDurationUnitSelected)))
				{
					string durationUnits = _renewalTerm.RenewalDurationUnitSelected + (_renewalTerm.RenewalDurationUnitCount.Value == 1 ? "" : "s");
					lblRenewalDurationValue.Text = string.Format("{0} {1}", _renewalTerm.RenewalDurationUnitCount.Value, durationUnits);
				}
			}
			lblRenewalDateCaption.Text = (_renewalTerm.IsTypeNone ?  Business.XMLNames._TPS_ExpirationDate : Business.XMLNames._TPS_RenewalDate ); // "Expiration Date" : "Renewal Date");
			if ((_renewalTerm.RenewalCount ?? 0) > 0)
				lblRenewalCount.Text = string.Format("  (Has Been Renewed {0} Time{1})", _renewalTerm.RenewalCount.Value, (_renewalTerm.RenewalCount.Value == 1 ? "" : "s"));

			//calculate and display the Expiration/Renewal date
			DateTime? expDate = _renewalTerm.ExpirationDate;
			if (expDate.HasValue)
				lblRenewalDateValue.Text = Utility.DateHelper.FormatDate(expDate.Value, _dateFormat);
			else
				lblRenewalDateValue.Text = string.Empty;


		}

		private void LoadDropDowns()
		{
			Helper.LoadListControl(ddlInitialDurationUnits, _renewalTerm.InitialDurationUnits, "Display", "Value", _renewalTerm.InitialDurationUnitSelected);
			Helper.LoadListControl(ddlRenewalDurationUnits, _renewalTerm.RenewalDurationUnits, "Display", "Value", _renewalTerm.InitialDurationUnitSelected);
		}


        private void RegisterClientSideScripts(List<string> tryShowRenewButton)
		{
            RegisterRenewalButtonHidden(tryShowRenewButton);

			//ddlInitialDurationUnits
			if (_renewalTerm.InitialDurationUnitPopUpIfNot ?? false)
			{
				RegisterRenewalPopup(ddlInitialDurationUnits.ClientID, _renewalTerm.InitialDurationUnitDefault, _renewalTerm.InitialDurationUnitPopUpText);
				Helper.AddAttribute(ddlInitialDurationUnits.Attributes, "onchange", string.Concat(CONTROL_EVENT_PREFIX, ddlInitialDurationUnits.ClientID, "_onchange();"));
			}

				if (!_renewalTerm.IsTypeNone)
				{
					//ddlRenewalDurationUnits
						if (_renewalTerm.RenewalDurationUnitPopUpIfNot ?? false)
						{
							RegisterRenewalPopup(ddlRenewalDurationUnits.ClientID, _renewalTerm.RenewalDurationUnitDefault, _renewalTerm.RenewalDurationUnitPopUpText);
							Helper.AddAttribute(ddlRenewalDurationUnits.Attributes, "onchange", string.Concat(CONTROL_EVENT_PREFIX, ddlRenewalDurationUnits.ClientID, "_onchange();"));
						}
				}
		}


		private void RegisterRenewalConfirmation()
		{
			//Set up client-side script to prompt the user if they click the Delete button
			btnRenew.OnClientClick = "javascript:return RenewalConfirmation();";

			Type t = this.GetType();
			string scriptName = "_kh_RenewalConfirmation";
			if (!Page.ClientScript.IsClientScriptBlockRegistered(t, scriptName))
			{
				string confirmationText = _renewalTerm.PopUpText;
				if (string.IsNullOrEmpty(confirmationText))
					confirmationText = "Are you sure you want to renew this item?";
				confirmationText = confirmationText.Replace("\r", "\\r").Replace("\n", "\\n").Replace(@"'", @"\'");

				System.IO.StringWriter swRenewalConfirmation = new System.IO.StringWriter();
				swRenewalConfirmation.WriteLine("function RenewalConfirmation()");
				swRenewalConfirmation.WriteLine("{");
				swRenewalConfirmation.WriteLine("	return confirm('{0}');", confirmationText);
				swRenewalConfirmation.WriteLine("}");
				Page.ClientScript.RegisterClientScriptBlock(t, scriptName, swRenewalConfirmation.ToString(), true);
			}
		}

        private void RegisterRenewalButtonHidden(List<string> tryShowRenewButton)
        {
            //Set up client-side script to prompt the user if they click the Delete button
            btnRenewHidden.OnClientClick = "javascript:return RenewalButtonHidden();";

            Type t = this.GetType();
            string scriptName = "_kh_RenewalButtonHidden";
            if (!Page.ClientScript.IsClientScriptBlockRegistered(t, scriptName))
            {
                System.IO.StringWriter swRenewalButtonHidden = new System.IO.StringWriter();
                swRenewalButtonHidden.WriteLine("function RenewalButtonHidden()");
                swRenewalButtonHidden.WriteLine("{");
                swRenewalButtonHidden.WriteLine("   alert('{0}');", string.Join("\\n", tryShowRenewButton.ToArray()));
                swRenewalButtonHidden.WriteLine("   return false;");
                swRenewalButtonHidden.WriteLine("}");
                Page.ClientScript.RegisterClientScriptBlock(t, scriptName, swRenewalButtonHidden.ToString(), true);
            }
        }

		private void RegisterRenewalPopup(string dropDownID, string defaultValue, string popupTextIfNot)
		{
			Type t = this.GetType();
			string scriptName = CONTROL_EVENT_PREFIX + dropDownID;
			if (!Page.ClientScript.IsClientScriptBlockRegistered(t, scriptName))
			{
				System.IO.StringWriter swClientScript = new System.IO.StringWriter();
				swClientScript.WriteLine("function {0}_onchange()", scriptName);
				swClientScript.WriteLine("{");
				swClientScript.WriteLine("	var ddl = document.getElementById('{0}');", dropDownID);
				swClientScript.WriteLine("	if (ddl)  ");
				swClientScript.WriteLine("	{");
				swClientScript.WriteLine("		var selectedValue = ddl.options[ddl.selectedIndex].value; ");
				swClientScript.WriteLine("		if (selectedValue.toString() != '{0}') ", defaultValue);
				swClientScript.WriteLine("		{");
				swClientScript.WriteLine("			if (!confirm('{0} \\n\\n\\nClick OK to proceed, or Cancel otherwise'))", popupTextIfNot.Replace(System.Environment.NewLine, @"\n").Replace(@"'", @"\'"));
				swClientScript.WriteLine("			{");
				swClientScript.WriteLine("				for (var i=0; i<ddl.options.length; i++)");
				swClientScript.WriteLine("				{");
				swClientScript.WriteLine("					if (ddl.options[i].value == '{0}')", defaultValue);
				swClientScript.WriteLine("					{");
				swClientScript.WriteLine("						ddl.selectedIndex = i;");
				swClientScript.WriteLine("						break;");
				swClientScript.WriteLine("					}");
				swClientScript.WriteLine("				}");
				swClientScript.WriteLine("			}");
				swClientScript.WriteLine("		}");
				swClientScript.WriteLine("	}");
				swClientScript.WriteLine("}");
				Page.ClientScript.RegisterClientScriptBlock(t, scriptName, swClientScript.ToString(), true);
			}
		}




		#endregion



		public override void UpdateTermValue(string termGroupContainerName)
		{
			string prefix = (string.IsNullOrEmpty(termGroupContainerName) ? "" : termGroupContainerName + "$");

			_renewalTerm.InitialDurationUnitSelected = Page.Request.Form[prefix + ddlInitialDurationUnits.UniqueID];
			_renewalTerm.RenewalDurationUnitSelected = Page.Request.Form[prefix + ddlRenewalDurationUnits.UniqueID];

			int value1;
			if (int.TryParse(Page.Request.Form[prefix + txtInitialDurationCount.UniqueID], out value1))
				_renewalTerm.InitialDurationUnitCount = value1;
			else
				_renewalTerm.InitialDurationUnitCount = null;

			int value2;
			if (int.TryParse(Page.Request.Form[prefix + txtRenewalDurationCount.UniqueID], out value2))
				_renewalTerm.RenewalDurationUnitCount = value2;
			else
				_renewalTerm.RenewalDurationUnitCount = null;

			DateTime value3;
			if (DateTime.TryParse(Page.Request.Form[prefix + txtEffectiveDate.UniqueID], out value3))
				_renewalTerm.EffectiveDate = value3;
			else
				_renewalTerm.EffectiveDate = null;
		}
	}
}