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

namespace Kindred.Knect.ITAT.Web
{
	public partial class ReportAdd : BaseSystemPage
	{
		private ITAT.Business.Report _report;
		private Guid _reportID;
		private const string VSKEY_REPORT_ID = "_vskey_ReportID";

		internal override Control ResizablePanel()
		{
			return null;  // this.pnlMain;
		}
		
		internal override HtmlGenericControl HTMLBody()
		{
			return this.htmlBody;
		}

		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);
			((StandardHeader)itatHeader).PageTitle = GetPageName();
			if (IsPostBack)
			{
			}
			else
			{
				GetReport();
				LoadFields();
			}
		}

		private void LoadFields()
		{
			if (_report != null)
			{
				txtName.Text = Business.Report.NewReportName(_itatSystem.ID, _report.Name);
				txtDescription.Text = _report.Description;
			}
		}

		private void GetReport()
		{
			//If this is a "Copy", then it is based on an existing Report.
			//If this is an "Add", then there is no existing Report.
			string qsValue = "Not Defned";
			try
			{
				qsValue = Request.QueryString[Common.Names._QS_REPORT_ID];
				_reportID = new Guid(qsValue);
				_report = Business.Report.Create(_reportID, _itatSystem);
			}
			catch
			{
			}
		}

		protected override string GetPageName()
		{
			return _itatSystem.ManagedItemName + " - Report Add/Copy";
		}

		protected void btnSave_Command(object sender, CommandEventArgs e)
		{
			if (_report != null)
			{
				//Perform a "Copy" of existing
				Guid newReportID = Guid.NewGuid();
				_report.ReportId = newReportID;
				_report.Name = txtName.Text;
				_report.Description = txtDescription.Text;

				Business.Report.CopyReport(_reportID, newReportID, txtName.Text, txtDescription.Text, _report.GetXml());
				Response.Redirect(String.Format("ReportSearch.aspx{0}", Utility.TextHelper.QueryString(true, Common.Names._QS_ITAT_SYSTEM_ID, _itatSystem.ID.ToString())));
			}
			else
			{
				//Perform an "Add" new
				//Continue defining the new Report...
				Guid newReportID = Guid.NewGuid();
				Business.Report report = new Business.Report(_itatSystem.ID, newReportID, txtName.Text, txtDescription.Text);
				Business.Report.UpdateReport(newReportID, _itatSystem.ID, txtName.Text, txtDescription.Text, report.GetXml());
				Response.Redirect(String.Format("ManagedItemSearch.aspx{0}", Utility.TextHelper.QueryString(true, Common.Names._QS_ITAT_SYSTEM_ID, _itatSystem.ID.ToString(), Common.Names._QS_REPORT_ID, newReportID.ToString())));
			}
		}

		protected void btnCancel_Command(object sender, CommandEventArgs e)
		{
			Response.Redirect(String.Format("ReportSearch.aspx{0}", Utility.TextHelper.QueryString(true, Common.Names._QS_ITAT_SYSTEM_ID, _itatSystem.ID.ToString())));
		}

		protected override object SaveViewState()
		{
			ViewState[VSKEY_REPORT_ID] = _reportID;
			return base.SaveViewState();
		}

		protected override void LoadViewState(object savedState)
		{
			base.LoadViewState(savedState);
			try
			{
				_reportID = (Guid)ViewState[VSKEY_REPORT_ID];
				_report = Business.Report.Create(_reportID, _itatSystem);
			}
			catch { }
		}

	}
}
