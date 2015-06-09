using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using Kindred.Common.Controls;
using System.Collections.Generic;

namespace Kindred.Knect.ITAT.Web
{
	public partial class ReportSearch : BaseSystemPage
	{

		private bool _canDeleteReports;

		#region BasePage overrides


		internal override HtmlGenericControl HTMLBody()
		{
			return htmlBody;
		}

		internal override Control ResizablePanel()
		{
			return pnlGridBody;
		}

		protected override string GetPageName()
		{
			return string.Format("{0} - Report List", _itatSystem.Name);
		}

		protected override string GetApplicationFunction()
		{
			return null;		
		}

		#endregion


		#region event handlers

		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);
			((StandardHeader)itatHeader).PageTitle = GetPageName();
			_canDeleteReports = SecurityHelper.CanPerformFunction(_itatSystem.AllowedRoles(Business.XMLNames._AF_DeleteReport));
		}

		protected override void OnPreRender(EventArgs e)
		{
			base.OnPreRender(e);
			//RegisterDeleteClientScript();
			LoadGrid();
		}


		protected void grdTemplateList_RowCreated(object sender, GridViewRowEventArgs e)
		{
			if (e.Row.RowType == DataControlRowType.DataRow)
			{
				if (_canDeleteReports)
					((LinkButton)e.Row.Cells[5].Controls[0]).OnClientClick = "return confirm('Are you sure you want to delete this report?');";
				else
					((LinkButton)e.Row.Cells[5].Controls[0]).Visible = false;
		   }
		}

		protected void grdTemplateList_RowCommand(object sender, GridViewCommandEventArgs e)
		{
			int index = Convert.ToInt32(e.CommandArgument);
			Guid reportID = (Guid)grdTemplateList.DataKeys[index].Value;
			switch (e.CommandName)
			{
				case "Copy":
					Response.Redirect(String.Format("ReportAdd.aspx{0}", Utility.TextHelper.QueryString(true, Common.Names._QS_ITAT_SYSTEM_ID, _itatSystem.ID.ToString(), Common.Names._QS_REPORT_ID, reportID.ToString())));
					break;
				case "Edit":
					Response.Redirect(String.Format("ManagedItemSearch.aspx{0}", Utility.TextHelper.QueryString(true, Common.Names._QS_ITAT_SYSTEM_ID, _itatSystem.ID.ToString(), Common.Names._QS_REPORT_ID, reportID.ToString())));
					break;
				case "Delete":
					Business.Report.DeleteReport(reportID);
					Response.Redirect(String.Format("ReportSearch.aspx{0}", Utility.TextHelper.QueryString(true, Common.Names._QS_ITAT_SYSTEM_ID, _itatSystem.ID.ToString())));
					break;
				case "Run":
					Server.Transfer(string.Format("Report.aspx{0}", Utility.TextHelper.QueryString(true, Common.Names._QS_REPORT_ID, reportID.ToString(), Common.Names._QS_ITAT_SYSTEM_ID, _itatSystem.ID.ToString())));
					break;
				default:
					throw new Exception(String.Format("Unknown CommandName in grdTemplateList: {0}", e.CommandName)); 
			}
		}


		protected void btnAdd_Click(object sender, EventArgs e)
		{
			Response.Redirect(String.Format("ReportAdd.aspx{0}", Utility.TextHelper.QueryString(true, Common.Names._QS_ITAT_SYSTEM_ID, _itatSystem.ID.ToString())));
		}

		#endregion


		#region Private methods

		private void LoadGrid()
		{
			using (DataSet ds = Business.Report.GetSystemReports(_itatSystem.ID))
			{
				grdTemplateList.DataSource = ds;
				grdTemplateList.DataBind();
			}
		}


		#endregion
	}
}
