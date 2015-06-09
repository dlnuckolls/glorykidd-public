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
	public partial class Report : BaseReportPage
	{
		private Business.ReportManagedItemSort _reportManagedItemSort;
		private const string VIEWSTATE_REPORT_MANAGEDITEM_SORT = "_kh_vs_ReportManagedItemSort";
		private const string EXPORT_HIDDENFIELD_NAME = "_kh_hf_GridContents";

		internal override HtmlGenericControl HTMLBody()
		{
			return body;
		}

		internal override Control ResizablePanel()
		{
			return pnlGridBody;
		}


		protected void Page_Load(object sender, EventArgs e)
		{
			if (!IsPostBack)
			{
				((StandardHeader)header).PageTitle = string.Format("Report - {0} - Results", _report.Name);
				grdResults.SortColumn = 0;
				grdResults.SortAscending = true;
				//This check is needed since a user could save a report with no definition.
				//If the user selects 'Run' for that report, need to avoid trying to display it.
				if (_report.IsValid)
					DisplayResults(-1, grdResults.SortAscending);
				else
					btnPrevious.Enabled = false;
			}
		}


		private void ExportToExcel(string reportName, string gridContents)
		{
			Response.Clear();
			Response.ContentType = Utility.WebServiceHelper.ExtensionToMimeType("xls");
			Response.AddHeader("Content-Disposition", string.Format("attachment;filename={0}.xls", reportName));
			Response.Write(gridContents);
			Response.End();
		}


		protected override void OnPreRender(EventArgs e)
		{
			base.OnPreRender(e);
			RegisterExportClientScript();
		}


		private void DisplayResults(int sortColumn, bool ascending)
		{
			grdResults.Columns.Clear();

            pnlError.Visible = false;

			if (_reportManagedItemSort == null)
			{
				try
				{
					_reportManagedItemSort = (Business.ReportManagedItemSort)Context.Items[Common.Names._CNTXT_ReportManagedItemSort];
				}
				catch
				{
				}
			}

			if (_reportManagedItemSort == null)
			{
				//Came directly from ReportSearch.aspx ("Run")
				Business.ManagedItemSearch miSearch = new Kindred.Knect.ITAT.Business.ManagedItemSearch();
                //If this is an 'admin viewer', then do not limit the search results based on security.
                List<string> roles = UserRolesOrAdminViewer();
                Business.SearchResults _searchResults = miSearch.Search(_itatSystem.ID, _report.SearchCriteria, roles);
				_report.DefineTerms(_searchResults);
                _reportManagedItemSort = _report.GetReportManagedItemSort(_searchResults, roles);
				_reportManagedItemSort.Update(_report.ReportTerms, _report.PrimaryTerm, _report.SecondaryTerm, true);
			}
			DataSet dsResults = _reportManagedItemSort.GetDisplay();
			//Add columns to grdResults
			if (_reportManagedItemSort != null)
			{
                if (_reportManagedItemSort.Count == 0)
                {
                    pnlError.Visible = true;
                    litError.Text = "You are receiving no results from your Report as you may not have access to the data or there is no data that meets your criteria";
                }
                else
				{
					Business.ReportManagedItem rmi = _reportManagedItemSort[0];
					foreach (Business.ReportTerm rptTerm in rmi.ReportTerms)
					{
						if (rptTerm.Visible ?? false)
						{
							BoundField fld = new BoundField();
							fld.HeaderText = rptTerm.ColumnHeaderText(_itatSystem.ManagedItemName);
							fld.DataField = rptTerm.Name;
							grdResults.Columns.Add(fld);
						}
					}
					if (sortColumn >= 0)
					{
						dsResults.Tables[0].DefaultView.Sort = string.Format("{0} {1}", dsResults.Tables[0].Columns[sortColumn].ColumnName, ascending ? "ASC" : "DESC");
					}
					grdResults.DataSource = dsResults.Tables[0].DefaultView;
					grdResults.DataBind();
				}
			}
		}

		protected void grd_RowCommand(object sender, GridViewCommandEventArgs e)
		{
			switch (e.CommandName)
			{
				case Common.Names._GRID_COMMAND_SingleClick:
					break;
				case Common.Names._GRID_COMMAND_HeaderClick:
					int columnNumber = int.Parse((string)e.CommandArgument);
					if (columnNumber == grdResults.SortColumn)
					{
						grdResults.SortAscending = !grdResults.SortAscending;
					}
					else
					{
						grdResults.SortColumn = columnNumber;
						grdResults.SortAscending = true;
					}
					DisplayResults(columnNumber, grdResults.SortAscending);
					break;
				default:
					break;
			}
		}

		protected void btnPrevious_Click(object sender, CommandEventArgs e)
		{
			Context.Items[Common.Names._CNTXT_ReportManagedItemSort] = _reportManagedItemSort;
			SetContextData();
			Server.Transfer("ReportSearchResults.aspx");
		}

		protected void btnSave_Click(object sender, CommandEventArgs e)
		{
			_report.Save();
			Response.Redirect(String.Format("ReportSearch.aspx{0}", Utility.TextHelper.QueryString(true, Common.Names._QS_ITAT_SYSTEM_ID, _itatSystem.ID.ToString())));
		}




		private void RegisterExportClientScript()
		{
			//this javascript will get executed CLIENT-SIDE when the Export button is clicked
			Page.ClientScript.RegisterHiddenField(EXPORT_HIDDENFIELD_NAME, string.Empty);

			string scriptBlockName = "_kh_ExportGridToExcel";
			System.IO.StringWriter scriptBlock = new System.IO.StringWriter();
			scriptBlock.WriteLine("function SaveGrid()");
			scriptBlock.WriteLine("{");
			scriptBlock.WriteLine("	var hf = document.getElementById('{0}');", EXPORT_HIDDENFIELD_NAME);
			scriptBlock.WriteLine("	var grd = document.getElementById('{0}');", grdResults.ClientID);
			scriptBlock.WriteLine("	if (grd && hf)");
			scriptBlock.WriteLine("	{ ");
			scriptBlock.WriteLine("		hf.value = grd.outerHTML.replace('style=\"HEIGHT: 0px\"','').replace('border=1','border=0');");  //fixes minor formatting issues in Excel
			scriptBlock.WriteLine("		return true;");
			scriptBlock.WriteLine("	} ");
			scriptBlock.WriteLine("	else");
			scriptBlock.WriteLine("		return false;");
			scriptBlock.WriteLine("}");
			if (!ClientScript.IsClientScriptBlockRegistered(scriptBlockName))
				ClientScript.RegisterClientScriptBlock(this.GetType(), scriptBlockName, scriptBlock.ToString(), true);
			scriptBlock.Close();


		}


		protected void btnCancel_Click(object sender, CommandEventArgs e)
		{
			Response.Redirect(String.Format("ReportSearch.aspx{0}", Utility.TextHelper.QueryString(true, Common.Names._QS_ITAT_SYSTEM_ID, _itatSystem.ID.ToString())));
		}

		protected void btnExport_Click(object sender, CommandEventArgs e)
		{
			string gridContents = Request.Form[EXPORT_HIDDENFIELD_NAME];
			if (!string.IsNullOrEmpty(gridContents))
				ExportToExcel(_report.Name, gridContents);
		}


		protected override void LoadViewState(object savedState)
		{
			base.LoadViewState(savedState);
			_reportManagedItemSort = (Business.ReportManagedItemSort)ViewState[VIEWSTATE_REPORT_MANAGEDITEM_SORT];
		}

		protected override object SaveViewState()
		{
			ViewState[VIEWSTATE_REPORT_MANAGEDITEM_SORT] = _reportManagedItemSort;
			return base.SaveViewState();
		}

	}
}
