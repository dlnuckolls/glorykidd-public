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
using Kindred.Common.Logging;
using System.Diagnostics;
using System.Collections;

namespace Kindred.Knect.ITAT.Web
{
	public abstract class BaseTemplatePage : BaseSystemPage
	{

		#region private members and constants

		private const string _KH_K_HF_IS_CHANGED = "_KH_K_HF_IS_CHANGED";
		//IsChanged related change
		private const string _KH_K_HF_IS_CHANGED_INITIAL = "_KH_K_HF_IS_CHANGED_INITIAL";
		private const string _KH_K_HF_SHOWCHANGECONFIRMATION = "_KH_K_HF_SHOWCHANGECONFIRMATION";
		private const string VSKEY_TEMPLATE = "_vskey_Template";

		private bool _isChanged;
		//IsChanged related change
		private bool _isChangedInitial = false;

        private string _bannerOverride;
        private string _subTitleOverride;

		#endregion


		#region Properties

		public ITAT.Business.Template Template
		{
			get { return _template; }
		}

		public bool IsChanged
		{
			get { return _isChanged; }
			set { _isChanged = value; }
		}

		protected bool IsChangedInitial
		{
			get { return _isChangedInitial; }
		}

        public string BannerOverride
        {
            get { return _bannerOverride; }
            set { _bannerOverride = value; }
        }

        public string SubTitleOverride
        {
            get { return _subTitleOverride; }
            set { _subTitleOverride = value; }
        }

		#endregion


		#region protected members

		protected ITAT.Business.Template _template;

		protected abstract TemplateHeader HeaderControl();

        private string GetSourceFileName(string rawName)
        {
            int nLastSlash = rawName.LastIndexOf('\\');
            return rawName.Substring(nLastSlash + 1, rawName.Length - nLastSlash - 1);
        }

		#endregion 


		#region overrides

		protected override string GetApplicationFunction()
		{
			return Business.XMLNames._AF_EditTemplate;
		}

		protected override void OnInit(EventArgs e)
		{
			base.OnInit(e);
			_isChanged = false;
			_pageType = Business.PageType.Template;
			if (!IsPostBack || Template == null || string.IsNullOrEmpty(Template.Name))
			{
				GetTemplate();
				//IsChanged related change
				_isChangedInitial = _isChanged;
            }
		}

		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);
		}


		protected override void OnPreRender(EventArgs e)
		{
			base.OnPreRender(e);
			ClientScript.RegisterClientScriptInclude(this.GetType(), "_kh_jsITAT", "Scripts/itat.js");
			RegisterConfirmationScriptBlocks();
		}



		#endregion
		

        #region  Template-specific methods

		protected void HandleHeaderEvent(object sender, HeaderEventArgs e)
		{
			string props = "center=yes; help=no; resizable=yes;";
			switch (e.CommandName)
			{
				case Common.Names._HEADER_EVENT_Preview:
					string managedItemId = Request.QueryString[Common.Names._QS_MANAGED_ITEM_ID];
					string queryString;

                    if (!string.IsNullOrEmpty(managedItemId))
                        queryString = Utility.TextHelper.QueryString(true, Common.Names._QS_DOC_DLG_ACTION, Common.Names._QS_DOC_DLG_ACTION_PREVIEW, Common.Names._QS_MANAGED_ITEM_ID, managedItemId, Common.Names._QS_ITAT_SYSTEM_ID, _itatSystem.ID.ToString(), Common.Names._QS_SHOW_DEFAULT_DOCUMENT, "true");
                    else
                        queryString = Utility.TextHelper.QueryString(true, Common.Names._QS_DOC_DLG_ACTION, Common.Names._QS_DOC_DLG_ACTION_PREVIEW, Common.Names._QS_TEMPLATE_ID, _template.ID.ToString("D"), Common.Names._QS_ITAT_SYSTEM_ID, _itatSystem.ID.ToString(), Common.Names._QS_SHOW_DEFAULT_DOCUMENT, "true");
					ShowDialog("DocumentDialog.aspx" + queryString, props, false);
					break;
				default:
					throw new Exception(string.Format("UnHandled HeaderEvent:  CommandName='{0}', CommandArgument='{1}'.", e.CommandName, e.CommandArgument));
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
			sw.WriteLine(	"	{	");
			sw.WriteLine(	"		if (document.forms['{0}']['{1}'].value.toLowerCase() == 'true') ", Form.Name, _KH_K_HF_IS_CHANGED	);
			sw.WriteLine(	"		{");
			sw.WriteLine("			    event.returnValue = '{0}';", warningMsg);
			sw.WriteLine("		    }");
			sw.WriteLine("	}	");
			sw.WriteLine(	"	function _kh_onChange()  ");
			sw.WriteLine(	"	{");
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
			sw.WriteLine("			document.forms['{0}']['{1}'].value = 'true';", Form.Name, _KH_K_HF_IS_CHANGED);
			sw.WriteLine("		}");
			sw.WriteLine(	"	function _kh_onSubmit() 		");
			sw.WriteLine(	"	{	");
			sw.WriteLine("		if(unhookOnBeforeUnload) 		");
			sw.WriteLine("		{	");
			sw.WriteLine("			document.body.onbeforeunload = null;		");
			sw.WriteLine("		}		");
			sw.WriteLine("		else 		");
			sw.WriteLine("		{	");
			sw.WriteLine("			unhookOnBeforeUnload = true;		");
			sw.WriteLine("		}		");
			sw.WriteLine(	"		return true;		");
			sw.WriteLine(	"	}		");
			if (!ClientScript.IsClientScriptBlockRegistered(t, scriptName))
				ClientScript.RegisterClientScriptBlock(t, scriptName, sw.ToString(), true);

			HTMLBody().Attributes.Add("onbeforeunload", "javascript: return _kh_canUnload();");

			scriptName = "_kh_onStartup";
			System.IO.StringWriter swStartup = new System.IO.StringWriter();
			swStartup.WriteLine("_kh_setDefaultValues(document.forms['{0}']);", Form.Name);
			if (!ClientScript.IsStartupScriptRegistered(t, scriptName))
				ClientScript.RegisterStartupScript(t, scriptName, swStartup.ToString(), true);

			scriptName = "_kh_onSubmit";
			if (!ClientScript.IsOnSubmitStatementRegistered(t, scriptName))
				ClientScript.RegisterOnSubmitStatement(t, scriptName, "if (!_kh_onSubmit()) return false;");
		}



		/// <summary>
		/// This function suppresses change notification for the supplied control.
		/// </summary>
		/// <param name="control">Indicates the control for which to suppress notification.</param>
		protected void SuppressChangeNotification(WebControl control)
		{
			if (control != null)
			{
				control.Attributes["suppressnotification"] = "true";
			}
		} 
		 
		protected void GetTemplate()
		{
			GetTemplate(false);
		}

		protected void GetTemplate(bool overrideContext)
		{
			if (overrideContext || (Context.Items[Common.Names._CNTXT_Template] == null))
			{
				string qsMIID = Request.QueryString[Common.Names._QS_MANAGED_ITEM_ID];
				if (!string.IsNullOrEmpty(qsMIID))
				{
					try
					{
						Guid managedItemID = new Guid(qsMIID);
						ManagedItem managedItem = ManagedItem.Get(managedItemID,true);
                        managedItem.IsEditLanguage = true;
						_template = managedItem;
					}
					catch
					{
						throw new Exception(String.Format("The \"{0}\" query string value \"{1}\" is not a valid GUID.", Common.Names._QS_MANAGED_ITEM_ID, qsMIID));
					}
				}
				else
				{
                    string qsTID = Request.QueryString[Common.Names._QS_TEMPLATE_ID];
                    
					try
					{
						Guid templateID = new Guid(qsTID);
						_template = new Business.Template(templateID, DefType.Draft);
					}
					catch
					{
						throw new Exception(String.Format("The \"{0}\" query string value \"{1}\" is not a valid GUID.", Common.Names._QS_TEMPLATE_ID, qsTID));
					}
				}
			}
			else
			{
				_template = (Business.Template)Context.Items[Common.Names._CNTXT_Template];

				//IsChanged related change
                if (Context.Items[Common.Names._CNTXT_IsChanged] != null)
                {
                    IsChanged = (bool)Context.Items[Common.Names._CNTXT_IsChanged];
                }
			}
		}

		#endregion


		#region ViewState events

        //Note: Temporary code used to examine the ViewState
        private string DecodeViewState(object obj)
        {
            LosFormatter format = new LosFormatter();
            System.IO.StringWriter writer = new System.IO.StringWriter();
            format.Serialize(writer, obj);
            byte[] bytes = Convert.FromBase64String(writer.ToString());

            for (int index = 0; index < bytes.Length; index++)
            {
                if (bytes[index] < 0x20 || bytes[index] > 0x7E)
                    bytes[index] = 0x20;
            }

            return Server.HtmlEncode(System.Text.Encoding.ASCII.GetString(bytes));
        }

		protected override void LoadViewState(object savedState)
		{
			base.LoadViewState(savedState);
			//retrieve system name from ViewState, and get the corresponding template object
			_template = (Business.Template)ViewState[VSKEY_TEMPLATE];
			_isChanged = bool.Parse(Request.Form[_KH_K_HF_IS_CHANGED]);
			_isChangedInitial = bool.Parse(Request.Form[_KH_K_HF_IS_CHANGED_INITIAL]);
		}


		protected override object SaveViewState()
		{
			//Save system name to ViewState
			ClientScript.RegisterHiddenField(_KH_K_HF_IS_CHANGED, _isChanged.ToString().ToLower());
			ClientScript.RegisterHiddenField(_KH_K_HF_IS_CHANGED_INITIAL, _isChangedInitial.ToString().ToLower());
			//ClientScript.RegisterHiddenField(_KH_K_HF_SHOWCHANGECONFIRMATION, "false");
			ViewState[VSKEY_TEMPLATE] = _template;
			return base.SaveViewState();
		}

		#endregion
	}
}
