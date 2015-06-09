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
using System.Linq;

namespace Kindred.Knect.ITAT.Web
{
	public partial class ManagedItemSearchResults : BaseSystemPage
	{
		private Business.SearchCriteria _searchCriteria;
		private const string VIEWSTATE_SEARCHCRITERIA = "_kh_vs_SearchCriteria";
		private const string VIEWSTATE_SEARCH_RESULT_COLUMNS = "_kh_vs_SearchResultColumns";
		private Business.SearchResultSortField[] _searchResultColumns;

		private const int MANAGEDITEMNUMBER_COLUMNPIXELWIDTH = 100;
		private const int TEMPLATE_COLUMN_PERCENT_WIDTH = 20;
		private const int STATUS_COLUMN_PERCENT_WIDTH = 9;
		private const int STATE_COLUMN_PERCENT_WIDTH = 9;
		private const int FACILITY_COLUMN_PERCENT_WIDTH = 20;
		private const int DB_TERM_COLUMN_PERCENT_WIDTH = 14;

		internal override HtmlGenericControl HTMLBody()
		{
			return body;
		}

		internal override Control ResizablePanel()
		{
			return pnlGridBody;
		}

		/// <summary>
		/// Raises the <see cref="E:Init"/> event.
		/// </summary>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		/// Created by Larry Richardson LRR 3/12/2008
		protected override void OnInit(EventArgs e)
		{
			base.OnInit(e);
			ClientScript.RegisterClientScriptInclude(this.GetType(), "_kh_jsITAT", "Scripts/itat.js");
		}

		protected void Page_Load(object sender, EventArgs e)
		{
			if (!IsPostBack)
			{
				_searchCriteria = (Business.SearchCriteria)Context.Items[Common.Names._CNTXT_SearchCriteria];
				_searchResultColumns = (Business.SearchResultSortField[])Context.Items[Common.Names._CNTXT_SearchResultColumns];

				((StandardHeader)header).PageTitle = "Search Results";
				SetColumns();
				grdResults.SortColumn = 0;
				grdResults.SortAscending = true;
				grdResults.HeaderRowSize = _itatSystem.SearchResultsHeaderRowSize;
				grdResults.HeaderStyle.Wrap = true;
				DisplayResults();
			}
		}

		private void DisplayResults()
		{
			Business.ManagedItemSearch miSearch = new Kindred.Knect.ITAT.Business.ManagedItemSearch();
			//If this is an 'admin viewer', then do not limit the search results based on security.
			Business.SearchResults results = miSearch.Search(_itatSystem.ID, _searchCriteria, UserRolesOrAdminViewer());

			if (results.Count >= 500)
			{
				pnlError.Visible = true;
				litError.Text = "500 or more results were found.  Only the first 500 results are shown.";
			}
			else
				if (results.Count == 0)
				{
					pnlError.Visible = true;
					litError.Text = "You are receiving no results from your Search as you may not have access to the data or there is no data that meets your criteria";
				}
				else
					pnlError.Visible = false;

			results.SortBy(_searchResultColumns[grdResults.SortColumn], grdResults.SortAscending);
			if (results.Count > 500)
				results.RemoveRange(500, results.Count - 500);
			grdResults.DataSource = results;
			grdResults.DataBind();
		}

		private bool DisplayTerm(Business.Term term)
		{
			if (term == null)
				return false;
			if (!term.SystemTerm)
				return false;
			if (term.TermType == Business.TermType.Facility)
				return true;
			if (!term.UseDBField ?? false)
				return false;
			switch (term.DBFieldName)
			{
				case Data.DataNames._C_Term1:
				case Data.DataNames._C_Term2:
				case Data.DataNames._C_Term3:
					//Note:  These are commented out for now, so as to ensure that the results display same as before.
					//case Data.DataNames._C_Term4:
					//case Data.DataNames._C_Term5:
					//case Data.DataNames._C_Term6:
					//case Data.DataNames._C_Term7:
					return true;
				default:
					return false;
			}
		}

		private void SetColumns()
		{
			List<string> monitoredFields = new List<string>();

			//Note:  This is commented out for now, so as to ensure that the results display same as before.
			//Survey what is displayed to determine column widths
			//int fixedWidthColumnCount = 0;
			//int variableWidthColumnCount = 0;

			//foreach (Business.SearchResultSortField field in _searchResultColumns)
			//{
			//    switch (field)
			//    {
			//        case Business.SearchResultSortField.ManagedItemNumber:
			//        case Business.SearchResultSortField.TemplateName:
			//        case Business.SearchResultSortField.Status:
			//        case Business.SearchResultSortField.Facility:
			//        case Business.SearchResultSortField.DateTerm3:
			//        case Business.SearchResultSortField.DateTerm6:
			//        case Business.SearchResultSortField.DateTerm7:
			//            fixedWidthColumnCount++;
			//            break;

			//        case Business.SearchResultSortField.TextTerm1:
			//        case Business.SearchResultSortField.TextTerm2:
			//        case Business.SearchResultSortField.TextTerm4:
			//        case Business.SearchResultSortField.TextTerm5:
			//            variableWidthColumnCount++;
			//            break;
			//    }
			//}

			foreach (Business.SearchResultSortField field in _searchResultColumns)
			{
				switch (field)
				{
					case Business.SearchResultSortField.ManagedItemNumber:
						AddBoundField("Number", field.ToString(), string.Empty, true, Unit.Pixel(MANAGEDITEMNUMBER_COLUMNPIXELWIDTH));     //width="100px"
						break;

					case Business.SearchResultSortField.TemplateName:
						AddBoundField("Template", field.ToString(), string.Empty, true, Unit.Percentage(TEMPLATE_COLUMN_PERCENT_WIDTH));       // width="20%"
						break;

					case Business.SearchResultSortField.Status:
						AddBoundField("Status", field.ToString(), string.Empty, true, Unit.Percentage(STATUS_COLUMN_PERCENT_WIDTH));         //width="8%"
						break;

					case Business.SearchResultSortField.State:
						AddBoundField("State", field.ToString(), string.Empty, true, Unit.Percentage(STATE_COLUMN_PERCENT_WIDTH));         //width="8%"
						break;

					case Business.SearchResultSortField.Facility:
						AddBoundField("Facility", Business.SearchCriteria._SC_Facility, string.Empty, true, Unit.Percentage(FACILITY_COLUMN_PERCENT_WIDTH));       //20%
						break;

					default:
						string dbFieldName = "";
						string sDatafield = null;
						//Note:  This is commented out for now, so as to ensure that the results display same as before.
						//Unit columnwidth = Unit.Percentage(100 / (variableWidthColumnCount == 0 ? 1 : variableWidthColumnCount));
						Unit columnwidth = Unit.Percentage(DB_TERM_COLUMN_PERCENT_WIDTH);
						switch (field)
						{
							case Business.SearchResultSortField.TextTerm1:
								sDatafield = Business.SearchCriteria._SC_TextTerm1;
								dbFieldName = Data.DataNames._C_Term1;
								break;

							case Business.SearchResultSortField.TextTerm2:
								sDatafield = Business.SearchCriteria._SC_TextTerm2;
								dbFieldName = Data.DataNames._C_Term2;
								break;

							case Business.SearchResultSortField.DateTerm3:
								sDatafield = Business.SearchCriteria._SC_DateTerm3;
								dbFieldName = Data.DataNames._C_Term3;
								//Note:  This is commented out for now, so as to ensure that the results display same as before.
								//columnwidth = Unit.Pixel(DATE_COLUMNPIXELWIDTH);
								break;

							case Business.SearchResultSortField.TextTerm4:
								sDatafield = Business.SearchCriteria._SC_TextTerm4;
								dbFieldName = Data.DataNames._C_Term4;
								break;

							case Business.SearchResultSortField.TextTerm5:
								sDatafield = Business.SearchCriteria._SC_TextTerm5;
								dbFieldName = Data.DataNames._C_Term5;
								break;

							case Business.SearchResultSortField.DateTerm6:
								sDatafield = Business.SearchCriteria._SC_DateTerm6;
								dbFieldName = Data.DataNames._C_Term6;
								//Note:  This is commented out for now, so as to ensure that the results display same as before.
								//columnwidth = Unit.Pixel(DATE_COLUMNPIXELWIDTH);
								break;

							case Business.SearchResultSortField.DateTerm7:
								sDatafield = Business.SearchCriteria._SC_DateTerm7;
								dbFieldName = Data.DataNames._C_Term7;
								//Note:  This is commented out for now, so as to ensure that the results display same as before.
								//columnwidth = Unit.Pixel(DATE_COLUMNPIXELWIDTH);
								break;

							default:
								throw new Exception(string.Format("System term not found for {0}", field.ToString()));
						}
						if (monitoredFields.Contains(dbFieldName))
							throw new Exception(string.Format("{0} is represented by more than one system term", dbFieldName));
						monitoredFields.Add(dbFieldName);

						Business.Term term = _itatSystem.Terms.Find(t => t.DBFieldName == dbFieldName);
						if (term == null)
							throw new Exception(string.Format("System term not found for {0}", dbFieldName));
						if (DisplayTerm(term))
						{
							string dataFormatString = string.Empty;
							bool htmlEncode = true;

							switch (term.TermType)
							{
								case Business.TermType.Date:
								case Business.TermType.Renewal:
									dataFormatString = "{0:MMM dd, yyyy}";
									htmlEncode = false;
									break;
							}
							AddBoundField(term.Name, sDatafield, dataFormatString, htmlEncode, columnwidth);
						}
						break;
				}
			}
		}

		private void AddBoundField(string termName, string datafield, string dataFormatString, bool htmlEncode, Unit width)
		{
			BoundField bnf = new BoundField();
			bnf.HeaderText = termName;
			bnf.ReadOnly = true;
			bnf.DataField = datafield;
			bnf.HeaderStyle.Width = width;
			bnf.ItemStyle.HorizontalAlign = HorizontalAlign.Left;
			bnf.DataFormatString = dataFormatString;
			bnf.HtmlEncode = htmlEncode;
			grdResults.Columns.Add(bnf);
		}

		protected void grd_RowCommand(object sender, GridViewCommandEventArgs e)
		{
			switch (e.CommandName)
			{
				case Common.Names._GRID_COMMAND_SingleClick:
					int rowNumber = int.Parse((string)e.CommandArgument);
					Guid managedItemId = (Guid)(grdResults.DataKeys[rowNumber].Value);
					Server.Transfer(string.Format("ManagedItemProfile.aspx{0}", Utility.TextHelper.QueryString(true, Common.Names._QS_ITAT_SYSTEM_ID, _itatSystem.ID.ToString(), Common.Names._QS_MANAGED_ITEM_ID, managedItemId.ToString())));
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
					DisplayResults();
					break;
				default:
					break;
			}
		}

		protected override void LoadViewState(object savedState)
		{
			base.LoadViewState(savedState);
			_searchCriteria = (Business.SearchCriteria)ViewState[VIEWSTATE_SEARCHCRITERIA];
			_searchResultColumns = (Business.SearchResultSortField[])ViewState[VIEWSTATE_SEARCH_RESULT_COLUMNS];
		}

		protected override object SaveViewState()
		{
			ViewState[VIEWSTATE_SEARCHCRITERIA] = _searchCriteria;
			ViewState[VIEWSTATE_SEARCH_RESULT_COLUMNS] = _searchResultColumns;
			return base.SaveViewState();
		}
	}
}
