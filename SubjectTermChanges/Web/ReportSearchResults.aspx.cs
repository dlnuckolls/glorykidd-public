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
using System.Collections.Specialized;


namespace Kindred.Knect.ITAT.Web
{
	public partial class ReportSearchResults : BaseReportPage
	{

		private DataSet _dsDropDownSortTerms;
		private Business.SearchResults _searchResults;
		private Business.ReportManagedItemSort _reportManagedItemSort;

		private const string VIEWSTATE_REPORT_MANAGEDITEM_SORT = "_kh_vs_ReportManagedItemSort";
		private const string VIEWSTATE_SORT_TERMS = "_kh_vs_SortTerms";
		private const string VIEWSTATE_SEARCH_RESULTS = "_kh_vs_SearchResults";

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
			return string.Format("Report - {0} - Step 2: Select Terms", _report.Name);
		}

		protected override string GetApplicationFunction()
		{
			return null;		
		}

		#endregion


		#region event handlers

		private void InitializeGridControl()
		{
			//TODO - Need to modify MultiSelectGrid control to work with column headers.
			//Headings would be 'Term Name', 'Term Type', 'Is Used'
			grdTermList.CssClass = "MSG";
			//grdTermList.CssClass = "NetTable";
			grdTermList.Width = Unit.Percentage(100.0);
			grdTermList.CheckboxColumn = 0;
			grdTermList.BoundColumns = "TermName TermType";
			grdTermList.ColumnWidths = "60% 40%";
			grdTermList.Container = pnlGridBody.ID;
			grdTermList.HeaderContainer = "";
			grdTermList.AutoGenerateColumns = false;
			grdTermList.EnableClickEvent = true;
			grdTermList.RowHighlighting = false;
			grdTermList.EnableDoubleClickEvent = false;
			grdTermList.EnableHeaderClick = false;
			grdTermList.ShowHeader = false;
			grdTermList.Enabled = true;
		}

		protected override void OnInit(EventArgs e)
		{
			base.OnInit(e);
			InitializeGridControl();
		}

		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);
			((StandardHeader)itatHeader).PageTitle = GetPageName();
			int nSelectedRow = -1;
			if (!IsPostBack)
			{
				_reportManagedItemSort = null;
				try
				{
					//_CNTXT_ReportManagedItemSort is defined if from Report.aspx or could be defined if from ReportSearchResults.aspx 
					_reportManagedItemSort = (Business.ReportManagedItemSort)Context.Items[Common.Names._CNTXT_ReportManagedItemSort];
				}
				catch
				{
				}

				string selectedRow = Request.QueryString[Common.Names._QS_SELECTED_ROW];
				int nTryParse;
				if (int.TryParse(selectedRow, out nTryParse))
				{
					//From Report.aspx or from ReportSearchResults.aspx (in order to correctly display the checkboxes)
					nSelectedRow = nTryParse;
					_searchResults = (Business.SearchResults)Context.Items[Common.Names._CNTXT_SearchResults];
				}
				else
				{
					if (_reportManagedItemSort == null)
					{
						//From ManagedItemSearch.aspx
						_report.SearchCriteria = (Business.SearchCriteria)Context.Items[Common.Names._CNTXT_SearchCriteria];
						Business.ManagedItemSearch miSearch = new Kindred.Knect.ITAT.Business.ManagedItemSearch();
                        //If this is an 'admin viewer', then do not limit the search results based on security.
                        _searchResults = miSearch.Search(_itatSystem.ID, _report.SearchCriteria, UserRolesOrAdminViewer());
						_report.DefineTerms(_searchResults);
					}
					else
					{
						//From Report.aspx
					}
				}
			}
			else
			{
				//Update the selection criteria
				SetVisibility(true);
				//Note - the order is important here. If the same term is used for both, then
				//the second call will 'null out' the first.  So if we call SetSecondary first, then
				//SetPrimary call will null out the SecondaryTerm.
				if (ddlSort1.Items.Count > 0 && ddlSort2.Items.Count > 0)
				{
					string[] sValues1 = ddlSort1.Items[ddlSort1.SelectedIndex].Value.Split(','); ;
					string[] sValues2 = ddlSort2.Items[ddlSort2.SelectedIndex].Value.Split(','); ;

					_report.SetSecondary(ddlSort2.Items[ddlSort2.SelectedIndex].Text, sValues2[1]);
					_report.SetPrimary(ddlSort1.Items[ddlSort1.SelectedIndex].Text, sValues1[1]);
				}
			}
			PopulateForm(nSelectedRow);
		}

		private void PopulateForm(int nSelectedRow)
		{
			DataSet dsSortTerms = _report.GetTerms();
			if (_dsDropDownSortTerms == null)
				_dsDropDownSortTerms = dsSortTerms;
			BindDataView(dsSortTerms);
			if (nSelectedRow >= 0)
				grdTermList.SelectedIndex = nSelectedRow;

			SetVisibility(false);
			
			if (_report.PrimaryTerm != null)
				LoadShowDropDown(ddlSort1, _dsDropDownSortTerms, _report.PrimaryTerm.Name, _report.PrimaryTerm.ReportTermType.ToString());
			else
				LoadShowDropDown(ddlSort1, _dsDropDownSortTerms, "","");
			if (_report.SecondaryTerm != null)
				LoadShowDropDown(ddlSort2, _dsDropDownSortTerms, _report.SecondaryTerm.Name, _report.SecondaryTerm.ReportTermType.ToString());
			else
				LoadShowDropDown(ddlSort2, _dsDropDownSortTerms, "","");
			SetMoveUpDownButtonEvents(grdTermList, btnTermMoveUp, btnTermMoveDown);
			btnContinue.Enabled = _report.ReportTerms.Count > 0;
		}

		private void BindDataView(DataSet dsSortTerms)
		{
			grdTermList.TermName = "TermName";
			grdTermList.DataSource = dsSortTerms;
			grdTermList.DataBind();
		}

		private void SetVisibility(bool updateReport)
		{
			if (updateReport)
				_report.Visibility = grdTermList.SelectedRows;
			else
				grdTermList.SelectedRows = _report.Visibility;
		}

		protected override void OnPreRender(EventArgs e)
		{
			base.OnPreRender(e);
		}

		protected void grd_RowCommand(object sender, GridViewCommandEventArgs e)
		{
			switch (e.CommandName)
			{
				case Common.Names._GRID_COMMAND_SingleClick:
				case Common.Names._GRID_COMMAND_DoubleClick:
					SetMoveUpDownButtonEvents(grdTermList, btnTermMoveUp, btnTermMoveDown);
					break;
				case Common.Names._GRID_COMMAND_HeaderClick:
					int columnNumber = int.Parse((string)e.CommandArgument);
					break;
				default:
					break;
			}
		}

		private void SetMoveUpDownButtonEvents(Kindred.Common.Controls.KindredGridView grd, ImageButton upButton, ImageButton downButton)
		{
			int selectedRow = grd.SelectedIndex;

			if (selectedRow > 0) //enable move up button
			{
				upButton.ImageUrl = "~/Images/MoveUp.gif";
				upButton.Enabled = true;
				upButton.Style["cursor"] = "pointer";
				upButton.CommandName = "Term";
				upButton.CommandArgument = "up";
			}
			else
			{
				upButton.ImageUrl = "~/Images/MoveUpDisabled.gif";
				upButton.Enabled = false;
				upButton.Style["cursor"] = "default";
			}

			if (selectedRow > -1 && selectedRow < grd.Rows.Count - 1)
			{
				downButton.ImageUrl = "~/Images/MoveDown.gif";
				downButton.Enabled = true;
				downButton.Style["cursor"] = "pointer";
				downButton.CommandName = "Term";
				downButton.CommandArgument = "down";
			}
			else
			{
				downButton.ImageUrl = "~/Images/MoveDownDisabled.gif";
				downButton.Enabled = false;
				downButton.Style["cursor"] = "default";
			}
		}

		protected void btnSwitchRows_Command(object sender, CommandEventArgs e)
		{
			//Identify the grid and List<Term> objects involved (this method is used for both grids on the page)
			List<Business.ReportTerm> list = _report.ReportTerms;

			//Identify the 2 rows to be swapped
			int selectedRow = grdTermList.SelectedIndex;
			int otherRow = ((string)e.CommandArgument == "up" ? selectedRow - 1 : selectedRow + 1);

			//Swap the 2 rows
			list.Reverse(Math.Min(selectedRow, otherRow), 2);
			//Re-bind the grid to the list (to reflect the new order of the Terms)
			BindDataView(_report.GetTerms());
			grdTermList.SelectedIndex = otherRow;
			SetContextData();
			Context.Items[Common.Names._CNTXT_SearchCriteria] = _report.SearchCriteria;
			Context.Items[Common.Names._CNTXT_SearchResults] = _searchResults;
			Context.Items[Common.Names._CNTXT_ReportManagedItemSort] = _reportManagedItemSort;

			NameValueCollection qsColl = new NameValueCollection(Request.QueryString);
			qsColl[Common.Names._QS_SELECTED_ROW] = grdTermList.SelectedIndex.ToString();
			string sQS = "?";
			for (int i = 0; i < qsColl.Count; i++ )
			{
				sQS = sQS + string.Format("{0}={1}&", qsColl.GetKey(i), qsColl.Get(i));
			}
			sQS = sQS.TrimEnd('&');

			Server.Transfer(string.Format("ReportSearchResults.aspx{0}", sQS));
		}

		#endregion


		#region Private methods

		private void LoadShowDropDown(DropDownList ddl, DataSet dsTerms, string termName, string termType)
		{
			ddl.Items.Clear();
			foreach (DataRow row in dsTerms.Tables[0].Rows)
			{
				//Create a unique value for ddl indexing
				string sValue = string.Format("{0},{1}", row[Data.DataNames._C_TermName].ToString(), row["TermType"].ToString());
				ListItem itm = new ListItem(row[Data.DataNames._C_TermName].ToString(), sValue);
				if (termName == row[Data.DataNames._C_TermName].ToString())
					if (termType == row[Data.DataNames._C_TermType].ToString())
						itm.Selected = true;
				ddl.Items.Add(itm);
			}
		}

		protected override void LoadViewState(object savedState)
		{
			base.LoadViewState(savedState);
			_dsDropDownSortTerms = (DataSet)ViewState[VIEWSTATE_SORT_TERMS];
			_searchResults = (Business.SearchResults)ViewState[VIEWSTATE_SEARCH_RESULTS];
			_reportManagedItemSort = (Business.ReportManagedItemSort)ViewState[VIEWSTATE_REPORT_MANAGEDITEM_SORT];
		}

		protected override object SaveViewState()
		{
			ViewState[VIEWSTATE_SORT_TERMS] = _dsDropDownSortTerms;
			ViewState[VIEWSTATE_SEARCH_RESULTS] = _searchResults;
			ViewState[VIEWSTATE_REPORT_MANAGEDITEM_SORT] = _reportManagedItemSort;
			return base.SaveViewState();
		}


		protected void btnContinue_Click(object sender, CommandEventArgs e)
		{
			if (grdTermList.SelectedRows.Count > 0)
			{
				if (_reportManagedItemSort == null)
				{
					//We got to this page from ManagedItemSearch.aspx 
                    _reportManagedItemSort = _report.GetReportManagedItemSort(_searchResults, UserRolesOrAdminViewer());
				}
				else
				{
					//We got to this page from Report.aspx 
				}

				_reportManagedItemSort.Update(_report.ReportTerms, _report.PrimaryTerm, _report.SecondaryTerm, true);

				Context.Items[Common.Names._CNTXT_ReportManagedItemSort] = _reportManagedItemSort;
				SetContextData();
				Server.Transfer("Report.aspx");
			}
			else
			{
				RegisterAlert("You must select at least one term.");
			}
		}


		protected void btnCancel_Click(object sender, CommandEventArgs e)
		{
			Server.Transfer("ReportSearch.aspx");
		}

		#endregion
	}
}
