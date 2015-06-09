using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Collections;
using Kindred.Common.Security;
using System.Collections.Generic;

namespace Kindred.Knect.ITAT.Web
{
	public abstract class BaseSystemPage : BasePage
	{
		#region private members and constants

		private const string VSKEY_SYSTEMID = "_vskey_SystemName";
		private const string VSKEY_SECURITYHELPER = "_vskey_SecurityHelper";

		#endregion

		#region protected members
		
		protected ITAT.Business.ITATSystem _itatSystem;
		protected ITAT.Business.PageType _pageType;
        protected Business.SecurityHelper _securityHelper;

		#endregion 


		#region Properties

		public ITAT.Business.ITATSystem ITATSystem
		{
			get { return _itatSystem; }
		}

		public ITAT.Business.PageType PageType
		{
			get { return _pageType; }
		}

		public Business.SecurityHelper SecurityHelper
		{
            get
            {
                if (_securityHelper == null && _itatSystem != null)
                {
                    _securityHelper = new Business.SecurityHelper(_itatSystem);
                }
                return _securityHelper; 
            }
		}

		#endregion


		#region event handlers

		protected override void OnInit(EventArgs e)
		{
			base.OnInit(e);
			_pageType = Kindred.Knect.ITAT.Business.PageType.Regular;
			if (!IsPostBack || this.ITATSystem == null || string.IsNullOrEmpty(ITATSystem.Name))
			{
				GetITATSystem();
			}
		}

		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);
			_securityHelper = new Business.SecurityHelper(_itatSystem);
			if (!AllowAccess())
			{
                UnauthorizedPageAccess();
			}
		}

		#endregion


		#region ITAT-specific methods

		/// <summary>
		/// This function returns a string designating the application function being performed on this page.
		/// This value must be one the ApplicationFunction nodes in the ITATSystemDef for the current system.
		/// </summary>
		/// <returns>String array of security resources.</returns>
		protected virtual string GetApplicationFunction()
		{
			return null;
		}


		protected virtual bool AllowAccess()
		{
			bool rtn = false;
			// If no application function is defined, allow access.
			string applicationFunction = GetApplicationFunction();
			if (string.IsNullOrEmpty(applicationFunction))
				rtn = true;
			else
			{
				if (_itatSystem == null)
					throw new Exception("_itatSystem is null");
				if (_securityHelper == null)
					throw new Exception("_securityHelper is null");
				if (_securityHelper.UserRoles == null)
					throw new Exception("_securityHelper.UserRoles is null");
				rtn = Utility.ListHelper.HaveAMatch(_itatSystem.AllowedRoles(applicationFunction), _securityHelper.UserRoles);
			}
			return rtn;
		}

        //This returns a list of role names or null if this is an 'AdminViewer'
        protected List<string> UserRolesOrAdminViewer()
        {
            List<string> roles = null;
            if (_securityHelper == null)
                _securityHelper = new Business.SecurityHelper(_itatSystem);
            if (Utility.ListHelper.HaveAMatch(_itatSystem.AllowedRoles(Business.XMLNames._AF_AdminViewer), _securityHelper.UserRoles))
                roles = null;
            else
                roles = _securityHelper.UserRoles;
            return roles;
        }

		protected override void GetITATSystem()
		{
			string qsValue = Request.QueryString[Common.Names._QS_ITAT_SYSTEM_ID];
			//ASSUMPTION: 
			//   If qsValue is of the form xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx or {xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx}, 
			//           then it is a systemID.
			//   Otherwise, it is the system name.
			if (Utility.TextHelper.IsGuid(qsValue))
				_itatSystem = Business.ITATSystem.Get(new Guid(qsValue));
			else
				_itatSystem = Business.ITATSystem.Get(qsValue);
		}

        protected void RenderValidationErrors(List<string> validationErrors)
        {
            if (validationErrors.Count > 0)
            {
                System.Text.StringBuilder sb = new System.Text.StringBuilder();
                sb.Append("Unable to complete the action due to the following errors:\\n\\n");
                foreach (string error in validationErrors)
                    sb.Append(string.Concat("- ", error, "\\n"));
                sb.Append("\\nPlease correct the errors and try again.");
                RegisterAlert(sb.ToString());
            }
        }


		#endregion


		#region ViewState events

		protected override object SaveViewState()
		{
			//Save system name to ViewState
			ViewState[VSKEY_SYSTEMID] = _itatSystem.ID;
//			ViewState[VSKEY_SECURITYHELPER] = _securityHelper;
			return base.SaveViewState();
		}

		protected override void LoadViewState(object savedState)
		{
			base.LoadViewState(savedState);
			//retrieve system name from ViewState, and get the corresponding ITATSystem object (from Cache or database)
			Guid systemID = (Guid)ViewState[VSKEY_SYSTEMID];
			_itatSystem = Business.ITATSystem.Get(systemID);
//			_securityHelper= (Business.SecurityHelper)ViewState[VSKEY_SECURITYHELPER];
		}
		
		#endregion


	}
}
