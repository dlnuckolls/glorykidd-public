using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using Kindred.Knect.ITAT.Business;
using System.Collections.Generic;

namespace Kindred.Knect.ITAT.Web
{
	public abstract class BaseManagedItemPage : BaseSystemPage
	{
		#region private members and constants

		private const string _KH_K_HF_IS_CHANGED = "_KH_K_HF_IS_CHANGED";
		private const string _KH_K_HF_SHOWCHANGECONFIRMATION = "_KH_K_HF_SHOWCHANGECONFIRMATION";
		private const string VSKEY_MANAGEDITEM = "_vskey_ManagedItem";
		private const string VSKEY_ACTIVETERMGROUPID = "_vskey_ActiveTermGroupId";

		private bool _isChanged;
		protected string _banner;



		#endregion

		#region Properties

		public ITAT.Business.ManagedItem ManagedItem
		{
			get { return _managedItem; }
		}

		public bool IsChanged
		{
			get { return _isChanged; }
			set { _isChanged = value; }
		}

		public string Banner
		{
			get { return _banner; }
		}

		public TermGroup ActiveTermGroup { get; private set; }

		#endregion

		#region protected members

		protected ITAT.Business.ManagedItem  _managedItem;

		protected abstract ManagedItemHeader HeaderControl();

		#endregion 

        #region base class overrides

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            _pageType = Business.PageType.ManagedItem;
            if (!IsPostBack || ManagedItem == null || string.IsNullOrEmpty(ManagedItem.ItemNumber))
            {
                GetManagedItem();
            }
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
        }

        protected override void OnPreRender(EventArgs e)
        {
            ClientScript.RegisterClientScriptInclude(this.GetType(), "_kh_jsITAT", "Scripts/itat.js");
            RegisterConfirmationScriptBlocks();
            base.OnPreRender(e);
        }

        protected override void LoadViewState(object savedState)
        {
            base.LoadViewState(savedState);
            _managedItem = (Business.ManagedItem)ViewState[VSKEY_MANAGEDITEM];
            if (ViewState[VSKEY_ACTIVETERMGROUPID] != null)
            {
                Guid activeTermGroupId = (Guid)ViewState[VSKEY_ACTIVETERMGROUPID];
                ActiveTermGroup = _managedItem.FindTermGroup(activeTermGroupId);
            }
            else
                ActiveTermGroup = null;
            _isChanged = bool.Parse(Request.Form[_KH_K_HF_IS_CHANGED]);
        }

        protected override object SaveViewState()
        {
            ViewState[VSKEY_MANAGEDITEM] = _managedItem;
            if (ActiveTermGroup != null)
                ViewState[VSKEY_ACTIVETERMGROUPID] = ActiveTermGroup.ID;
            ClientScript.RegisterHiddenField(_KH_K_HF_IS_CHANGED, _isChanged.ToString().ToLower());
            return base.SaveViewState();
        }

        protected override string GetApplicationFunction()
        {
            return null;
        }

        internal override HtmlGenericControl HTMLBody()
        {
            return null;
        }

        internal override Control ResizablePanel()
        {
            return null;
        }

        #endregion

        #region protected methods

        protected virtual void GetManagedItem()
        {
            string qsValue = Request.QueryString[Common.Names._QS_MANAGED_ITEM_ID];
            bool loadComplexLists = false;
            _managedItem = Business.ManagedItem.Get(new Guid(qsValue), loadComplexLists);
            string qsTermGroup = Request.QueryString[Common.Names._QS_MANAGED_ITEM_TERMGROUP];
            if (_securityHelper == null)
                _securityHelper = new SecurityHelper(_itatSystem);
            if (string.IsNullOrEmpty(qsTermGroup))
            {
                bool isAdmin = Utility.ListHelper.HaveAMatch(_itatSystem.AllowedRoles(Business.XMLNames._AF_AdminViewer), _securityHelper.UserRoles);
                ActiveTermGroup = _managedItem.GetDefaultTermGroup(_securityHelper.UserRoles, isAdmin);
                try
                {
                    Guid guid = ActiveTermGroup.ID;
                }
                catch (Exception)
                {
                    UnauthorizedPageAccess();
                }
            }
            else
            {
                ActiveTermGroup = _managedItem.FindTermGroup(new Guid(qsTermGroup));
                if (ActiveTermGroup == null)
                    throw new Exception(string.Format("Term Group ID {0} not found.", qsTermGroup));
                if (!_managedItem.CanAccessTermGroup(ActiveTermGroup, _securityHelper.UserRoles))
                {
                    if (!Utility.ListHelper.HaveAMatch(_itatSystem.AllowedRoles(Business.XMLNames._AF_AdminViewer), _securityHelper.UserRoles))
                        UnauthorizedPageAccess();
                }
            }
        }

        protected void SetComplexListsVisibility()
        {
            Dictionary<Guid /*TermGroupID*/, bool /*CanEdit*/> canEditTermGroup = BuildEditableTermGroups();
            Dictionary<Guid /*TermGroupID*/, bool /*CanView*/> canViewTermGroup = BuildViewableTermGroups();

            foreach (Business.ComplexList complexList in _managedItem.ComplexLists)
            {
                bool canSeeComplexList;
                if ((_managedItem.SecurityModel == SecurityModel.Basic) && _itatSystem.ViewersCannotSeeComplexLists)
                {
                    //This is the compatibility mode provided primarily for the HCMS system.
                    //In this secnario, if a TermDependency causes a ComplexList to be disabled, then the ComplexList should not be accessible.
                    canSeeComplexList = complexList.Runtime.Enabled && canEditTermGroup[complexList.TermGroupID];
                }
                else
                {
                    canSeeComplexList = canEditTermGroup[complexList.TermGroupID] || canViewTermGroup[complexList.TermGroupID];
                }
                HeaderControl().SetMenuItemVisibility(_itatSystem.ManagedItemName, complexList.Name, canSeeComplexList);
            }
        }

        protected void RegisterConfirmationScriptBlocks()
        {
            Type t = this.GetType();
            string scriptName = string.Empty;

            scriptName = "_kh_ConfirmationScriptBlock";
            string warningMsg = "You have made changes to the current data.   If you click OK, you will lose these changes.    If you wish to save your changes, click Cancel to remain on this page, and then click Save.";
            System.IO.StringWriter sw = new System.IO.StringWriter();
            sw.WriteLine("	var unhookOnBeforeUnload = true;		");
            sw.WriteLine("	function _kh_canUnload()  ");
            sw.WriteLine("	{	");
            //sw.WriteLine("			if (document.forms['{0}']['{1}'].value.toLowerCase() == 'true') ", Form.Name, _KH_K_HF_IS_CHANGED);
            sw.WriteLine("			if (document.getElementById('{0}').value.toLowerCase() == 'true') ", _KH_K_HF_IS_CHANGED);
            sw.WriteLine("			{");
            sw.WriteLine("				event.returnValue = '{0}';", warningMsg);
            sw.WriteLine("			}");
            sw.WriteLine("	}	");

            sw.WriteLine("	function _kh_onChange()  ");
            sw.WriteLine("	{");

            if (HeaderControl() != null)
            {
                sw.WriteLine("			var p = document.getElementById('{0}');", HeaderControl().SubTitle.ClientID);
                sw.WriteLine("			if (p)");
                sw.WriteLine("			{");
                sw.WriteLine("				var subTitle = p.innerHTML;");
                sw.WriteLine("				if (subTitle.indexOf(\"*\") == -1)");
                sw.WriteLine("					p.innerHTML = p.innerHTML + '*';");
                sw.WriteLine("			}");
            }
            //sw.WriteLine("		document.forms['{0}']['{1}'].value = 'true';", Form.Name, _KH_K_HF_IS_CHANGED);
            sw.WriteLine("		document.getElementById('{0}').value = 'true';", _KH_K_HF_IS_CHANGED);
            sw.WriteLine("	}");

            sw.WriteLine("	function _kh_onSubmit() 		");
            sw.WriteLine("	{	");
            sw.WriteLine("		if(unhookOnBeforeUnload) 		");
            sw.WriteLine("		{	");
            sw.WriteLine("			document.body.onbeforeunload = null;		");
            sw.WriteLine("		}		");
            sw.WriteLine("		else 		");
            sw.WriteLine("		{	");
            sw.WriteLine("			unhookOnBeforeUnload = true;		");
            sw.WriteLine("		}		");
            sw.WriteLine("		return true;		");
            sw.WriteLine("	}		");
            if (!ClientScript.IsClientScriptBlockRegistered(t, scriptName))
                ClientScript.RegisterClientScriptBlock(t, scriptName, sw.ToString(), true);

            RegisterLoadAction("document.body.onbeforeunload = _kh_canUnload;");
            //HTMLBody().Attributes["onbeforeunload"] = "_kh_canUnload();";

            scriptName = "_kh_onStartup";
            System.IO.StringWriter swStartup = new System.IO.StringWriter();
            swStartup.WriteLine("_kh_setDefaultValues(document.forms['{0}']);", Form.Name);
            if (!ClientScript.IsStartupScriptRegistered(t, scriptName))
                ClientScript.RegisterStartupScript(t, scriptName, swStartup.ToString(), true);

            scriptName = "_kh_onSubmit";
            if (!ClientScript.IsOnSubmitStatementRegistered(t, scriptName))
                ClientScript.RegisterOnSubmitStatement(t, scriptName, "if (!_kh_onSubmit()) return false;");
        }

        #endregion

        #region public methods

        public Dictionary<Guid /*TermGroupID*/, bool /*CanEdit*/> BuildEditableTermGroups()
        {
            Dictionary<Guid /*TermGroupID*/, bool /*CanEdit*/> dictCanEditTermGroup = new Dictionary<Guid, bool>();
            foreach (TermGroup tg in this._managedItem.TermGroups)
            {
                if (!dictCanEditTermGroup.ContainsKey(tg.ID))
                {
                    bool canEditTermGroup = false;
                    if (_itatSystem.HasOwningFacility ?? false)
                        canEditTermGroup = _securityHelper.CanPerformFunction(_managedItem.State.GetTermGroup(tg.ID).Editors, _managedItem.OwningFacilityIDs);
                    else
                        canEditTermGroup = _securityHelper.CanPerformFunction(_managedItem.State.GetTermGroup(tg.ID).Editors);
                    dictCanEditTermGroup.Add(tg.ID, canEditTermGroup);
                }
            }
            return dictCanEditTermGroup;
        }

        public Dictionary<Guid /*TermGroupID*/, bool /*CanEdit*/> BuildViewableTermGroups()
        {
            Dictionary<Guid /*TermGroupID*/, bool /*CanView*/> dictCanViewTermGroup = new Dictionary<Guid, bool>();
            foreach (TermGroup tg in this._managedItem.TermGroups)
            {
                if (!dictCanViewTermGroup.ContainsKey(tg.ID))
                {
                    bool canViewTermGroup = false;
                    if (_itatSystem.HasOwningFacility ?? false)
                        canViewTermGroup = _securityHelper.CanPerformFunction(_managedItem.State.GetTermGroup(tg.ID).Viewers, _managedItem.OwningFacilityIDs);
                    else
                        canViewTermGroup = _securityHelper.CanPerformFunction(_managedItem.State.GetTermGroup(tg.ID).Viewers);
                    dictCanViewTermGroup.Add(tg.ID, canViewTermGroup);
                }
            }
            return dictCanViewTermGroup;
        }

        /// <summary>
        /// Gets the validation errors.
        /// </summary>
        /// <param name="fullValidation">if set to <c>true</c> [full validation].
        /// True when changing state to a state that requires validation
        /// False otherwise (on a Save)
        /// </param>
        /// <returns>List of validation errors</returns>
        /// Created by Larry Richardson LRR 4/14/2008
        protected List<string> GetValidationErrors(ManagedItemValidationType validationType,
            Dictionary<string /* Term Name */, List<string> /* Error Messages */ > allTermTypeErrors,
            Dictionary<Guid /*TermGroupID*/, bool /*CanEdit*/> canEditTermGroup)
        {
            List<string> validationResultsAllTermAlert = new List<string>();

            bool includeTab = _managedItem.SecurityModel == SecurityModel.Advanced;

            foreach (Term term in _managedItem.BasicTerms)
            {
                if (!canEditTermGroup[term.TermGroupID])
                    continue;

                List<string> validationResultsTermAlert = new List<string>();
                List<string> validationResultsTermHover = new List<string>();

                switch (validationType)
                {
                    case ManagedItemValidationType.ValidateOnSave:
                        if (term.ValidationStatuses.Contains(_managedItem.State.Status))
                        {
                            if (term.Runtime.Enabled)
                            {
                                validationResultsTermAlert = term.Validate(includeTab, null);
                                validationResultsTermHover = includeTab ? term.Validate(false, null) : validationResultsTermAlert;
                            }
                        }
                        break;

                    case ManagedItemValidationType.FullValidation:
                        if (term.Runtime.Enabled)
                        {
                            validationResultsTermAlert = term.Validate(includeTab, null);
                            validationResultsTermHover = includeTab ? term.Validate(false, null) : validationResultsTermAlert;
                        }
                        break;

                    case ManagedItemValidationType.None:
                    default:
                        break;
                }

                List<string> termTypeErrors = null;
                if (allTermTypeErrors != null && allTermTypeErrors.ContainsKey(term.Name))
                    termTypeErrors = allTermTypeErrors[term.Name];
                if (termTypeErrors != null && termTypeErrors.Count > 0)
                {
                    if (validationResultsTermAlert == null)
                        validationResultsTermAlert = new List<string>();
                    foreach (string sMessage in termTypeErrors)
                    {
                        if (!validationResultsTermAlert.Contains(sMessage))
                            validationResultsTermAlert.Add(sMessage);
                    }
                }

                if (validationResultsTermAlert != null)
                {
                    validationResultsAllTermAlert.AddRange(validationResultsTermAlert);
                    term.Runtime.HasError = (validationResultsTermAlert.Count > 0);
                    System.Text.StringBuilder sbMsg = new System.Text.StringBuilder();
                    foreach (string msg in validationResultsTermHover)
                    {
                        sbMsg.Append(msg);
                        sbMsg.Append(@"\n");
                    }
                    term.Runtime.ErrorMessage = sbMsg.ToString();
                }
                else
                {
                    term.Runtime.HasError = false;
                    term.Runtime.ErrorMessage = string.Empty;
                }
            }

            return validationResultsAllTermAlert;
        }

        //Note - Added for 1.5.1
        protected Dictionary<string /* Term Name */, List<string> /* Error Messages */ > GetTermTypeErrors(Dictionary<Guid /*TermGroupID*/, bool /*CanEdit*/> canEditTermGroup)
        {
            //List<string> rtn = new List<string>();
            Dictionary<string /* Term Name */, List<string> /* Error Messages */ > errorMessages = new Dictionary<string, List<string>>();
            foreach (Term term in _managedItem.BasicTerms)
            {
                if (!canEditTermGroup[term.TermGroupID])
                    continue;
                List<string> sErrors = term.CheckType(false, null);
                if (sErrors != null)
                    errorMessages.Add(term.Name, sErrors);
            }

            return errorMessages;
        }

        #endregion
	}
}
