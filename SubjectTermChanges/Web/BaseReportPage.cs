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

namespace Kindred.Knect.ITAT.Web
{
	public abstract class BaseReportPage : BaseSystemPage
	{


		#region private members and constants

		private const string _KH_K_HF_IS_CHANGED = "_KH_K_HF_IS_CHANGED";
		private const string _KH_K_HF_SHOWCHANGECONFIRMATION = "_KH_K_HF_SHOWCHANGECONFIRMATION";
		private const string VSKEY_REPORT = "_vskey_Report";

		protected string _banner;

		#endregion


		#region Properties


		public string Banner
		{
			get { return _banner; }
		}

		#endregion


		#region protected members

		protected ITAT.Business.Report _report;

		#endregion 


		protected override void OnInit(EventArgs e)
		{
			base.OnInit(e);
			_pageType = Business.PageType.ManagedItem;
			if (!IsPostBack)
			{
				GetReport();
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
			//RegisterConfirmationScriptBlocks();
		}

		protected override string GetApplicationFunction()
		{
			return null;		//null means that anyone can run a report (you don't have to be in a certain role)
		}

		private void GetReport()
		{
			//First try to retrieve the data from the Context collection.
			//If not there, try to retrieve from the QueryString.
			if (!GetContextData())
				if (!GetQueryString())
					throw new Exception("Report not found");
		}

		private bool GetQueryString()
		{
			string qsValue = Request.QueryString[Common.Names._QS_REPORT_ID];
			Guid reportId = Guid.Empty;
			try
			{
				reportId = new Guid(qsValue);
			}
			catch
			{
				return false;
			}
			try
			{
				_report = Business.Report.Create(reportId, _itatSystem);
			}
			catch
			{
				return false;
			}
			return _report != null;
		}

		protected override void LoadViewState(object savedState)
		{
			base.LoadViewState(savedState);
			_report = (Business.Report)ViewState[VSKEY_REPORT];
		}

		protected override object SaveViewState()
		{
			ViewState[VSKEY_REPORT] = _report;
			return base.SaveViewState();
		}

		private bool GetContextData()
		{
			try
			{
				_report = (Business.Report)Context.Items[Common.Names._CNTXT_Report];
			}
			catch
			{
				return false;
			}
			return _report != null;
		}

		protected void SetContextData()
		{
			Context.Items[Common.Names._CNTXT_Report] = _report;
		}

		//protected void RegisterConfirmationScriptBlocks()
		//{
		//    Type t = this.GetType();
		//    string scriptName = string.Empty;

		//    scriptName = "_kh_ConfirmationScriptBlock";
		//    string warningMsg = "You have made changes to the current data.   If you click OK, you will lose these changes.    If you wish to save your changes, click Cancel to remain on this page, and then click Save.";
		//    System.IO.StringWriter sw = new System.IO.StringWriter();
		//    sw.WriteLine("	var unhookOnBeforeUnload = true;		");
		//    sw.WriteLine("	function _kh_canUnload()  ");
		//    sw.WriteLine("	{	");
		//    sw.WriteLine("		if (document.forms['{0}']['{1}'].value.toLowerCase() == 'true') ", Form.Name, _KH_K_HF_IS_CHANGED);
		//    sw.WriteLine("		{");
		//    sw.WriteLine("			event.returnValue = '{0}';", warningMsg);
		//    sw.WriteLine("		}");
		//    sw.WriteLine("	}	");

		//    sw.WriteLine("	function _kh_onChange()  ");
		//    sw.WriteLine("	{");

		//    //if (HeaderControl() != null)
		//    //{
		//    //    sw.WriteLine("			var p = document.getElementById('{0}');", HeaderControl().SubTitle.ClientID);
		//    //    sw.WriteLine("			if (p)");
		//    //    sw.WriteLine("			{");
		//    //    sw.WriteLine("				var subTitle = p.innerHTML;");
		//    //    sw.WriteLine("				if (subTitle.indexOf(\"*\") == -1)");
		//    //    sw.WriteLine("					p.innerHTML = p.innerHTML + '*';");
		//    //    sw.WriteLine("			}");
		//    //}
		//    sw.WriteLine("		document.forms['{0}']['{1}'].value = 'true';", Form.Name, _KH_K_HF_IS_CHANGED);
		//    sw.WriteLine("	}");

		//    sw.WriteLine("	function _kh_onSubmit() 		");
		//    sw.WriteLine("	{	");
		//    sw.WriteLine("		if(unhookOnBeforeUnload) 		");
		//    sw.WriteLine("		{	");
		//    sw.WriteLine("			document.body.onbeforeunload = null;		");
		//    sw.WriteLine("		}		");
		//    sw.WriteLine("		else 		");
		//    sw.WriteLine("		{	");
		//    sw.WriteLine("			unhookOnBeforeUnload = true;		");
		//    sw.WriteLine("		}		");
		//    sw.WriteLine("		return true;		");
		//    sw.WriteLine("	}		");
		//    if (!ClientScript.IsClientScriptBlockRegistered(t, scriptName))
		//        ClientScript.RegisterClientScriptBlock(t, scriptName, sw.ToString(), true);

		//    HTMLBody().Attributes.Add("onbeforeunload", "javascript: return _kh_canUnload();");

		//    scriptName = "_kh_onStartup";
		//    System.IO.StringWriter swStartup = new System.IO.StringWriter();
		//    swStartup.WriteLine("_kh_setDefaultValues(document.forms['{0}']);", Form.Name);
		//    if (!ClientScript.IsStartupScriptRegistered(t, scriptName))
		//        ClientScript.RegisterStartupScript(t, scriptName, swStartup.ToString(), true);

		//    scriptName = "_kh_onSubmit";
		//    if (!ClientScript.IsOnSubmitStatementRegistered(t, scriptName))
		//        ClientScript.RegisterOnSubmitStatement(t, scriptName, "if (!_kh_onSubmit()) return false;");
		//}

		internal override HtmlGenericControl HTMLBody()
		{
			return null;
		}

		internal override Control ResizablePanel()
		{
			return null;
		}
	}
}
