using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Collections.Generic;
using System.Xml.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace Kindred.Knect.ITAT.Web
{
	public partial class ManagedItemSearch : BaseSystemPage
	{
		private const string VSKEY_REPORT = "_vskey_Report";
		private const string VSKEY_EXTERNALTERMS = "_vskey_ExternalTerms";

		private const string COOKIE_SEARCH_CRITERIA = "ITATManagedSearchResultsCriteria2";
		private const string COOKIE_SEARCH_CRITERIA_VALUE = "criteria2";

		private const string COOKIE_SEARCH_COLUMNS = "ITATManagedSearchResultsColumns2";
		private const string COOKIE_SEARCH_COLUMNS_VALUE = "columns2";

		private Dictionary<string, WebControl> _systemTermControls;
		private Dictionary<string, Business.ExternalTerm> _externalTerms;
		private ITAT.Business.Report _report;

		protected override void OnInit(EventArgs e)
		{
			base.OnInit(e);
			if (!IsPostBack)
			{
				_externalTerms = new Dictionary<string, Business.ExternalTerm>();
			}
			ClientScript.RegisterClientScriptInclude(this.GetType(), "_kh_jsITAT", "Scripts/itat.js");

			if (!String.IsNullOrEmpty(Request.QueryString[Common.Names._QS_SAVED_SEARCH]))
			{
				List<Business.SearchResultSortField> searchResultColumns = null;

				try
				{
					Guid guidSearchColumns = new Guid(GetCookie(COOKIE_SEARCH_COLUMNS, COOKIE_SEARCH_COLUMNS_VALUE));
					searchResultColumns = (List<Business.SearchResultSortField>)Deserialize(Business.Template.GetSessionParameter(guidSearchColumns));
				}
				catch
				{
				}

				if (searchResultColumns == null)
					searchResultColumns = GetSearchResultColumns(true);
				Context.Items[Common.Names._CNTXT_SearchResultColumns] = searchResultColumns.ToArray();


				Business.SearchCriteria criteria = null;

				try
				{
					Guid guidCriteria = new Guid(GetCookie(COOKIE_SEARCH_CRITERIA, COOKIE_SEARCH_CRITERIA_VALUE));
					criteria = (Business.SearchCriteria)Deserialize(Business.Template.GetSessionParameter(guidCriteria));
				}
				catch
				{
				}

				if (criteria != null)
				{
					Context.Items[Common.Names._CNTXT_SearchCriteria] = criteria;
					Server.Transfer("ManagedItemSearchResults.aspx");
				}
			}
		}

		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);
			if (!IsPostBack)
			{
				GetReport();
			}
			if (_itatSystem != null)
			{
				lblManagedItemNumber.Text = _itatSystem.ManagedItemName + " Number";
				if (_report != null)
				{
					((StandardHeader)header).PageTitle = string.Format("Report - {0} - Step 1: Criteria", _report.Name);
					divReportButtons.Visible = true;
					divSearchButtons.Visible = false;
				}
				else
				{
					((StandardHeader)header).PageTitle = _itatSystem.ManagedItemName + " Search";
					divReportButtons.Visible = false;
					divSearchButtons.Visible = true;
				}
				if (!IsPostBack)
				{
					Helper.LoadListControl(ddlTemplate, Business.Template.GetTemplateListWithStatus(_itatSystem.ID, searchableStatuses(), null), "TemplateName", "TemplateID", Guid.Empty.ToString(), true, "(Select a Template)", Guid.Empty.ToString());
					Helper.LoadListControl(ddlStatus, _itatSystem.ViewableStatuses(_securityHelper.UserRoles), "Name", "Name", string.Empty, true, "(Select a Status)", string.Empty);
				}
				RenderSystemTermControls();
				RenderExternalTermControls();
				if (!IsPostBack)
					if (_report != null)
						SetControlPreSelects();
			}
		}


		private void GetReport()
		{
			string qsValue = Request.QueryString[Common.Names._QS_REPORT_ID];
			if (string.IsNullOrEmpty(qsValue))
				_report = null;
			else
				_report = Business.Report.Create(new Guid(qsValue), _itatSystem);
			//			if (!string.IsNullOrEmpty(qsValue))
			//					throw new Exception("Unable to create report from ID: " + qsValue + "      " +  ex.Message);
		}


		private List<short> searchableStatuses()
		{
			List<short> rtn = new List<short>();
			rtn.Add((short)Business.TemplateStatusType.Active);
			rtn.Add((short)Business.TemplateStatusType.SearchOnly);
			return rtn;
		}


		private ExternalTermControl BuildExternalControl(Business.ExternalInterfaceConfig eic)
		{
			Business.ExternalTerm extTerm = null;
			if (IsPostBack)
				extTerm = _externalTerms[eic.Name];
			else
			{
				extTerm = new Kindred.Knect.ITAT.Business.ExternalTerm(false, eic, null, null, false);
				_externalTerms.Add(eic.Name, extTerm);
			}

			ExternalTermControl c = (ExternalTermControl)LoadControl(Common.Names._UC_ExternalTermControl);
			if (c != null)
			{
				c.CanEdit = true;
				c.Term = extTerm;
				c.ID = Helper.ControlID(extTerm.Name);
				if (IsPostBack)
					c.UpdateTermValue(null);
			}
			return c;
		}


		private void RenderExternalTermControls()
		{
			foreach (Business.ExternalInterfaceConfig eic in _itatSystem.ExternalInterfaces)
			{
				ExternalTermControl c = BuildExternalControl(eic);
				if (c != null)
				{
					System.Web.UI.HtmlControls.HtmlTableRow row = new System.Web.UI.HtmlControls.HtmlTableRow();

					System.Web.UI.HtmlControls.HtmlTableCell cellLabel = new System.Web.UI.HtmlControls.HtmlTableCell();
					Helper.FormatCaptionCell(cellLabel);
					cellLabel.InnerHtml = eic.Name;
					row.Cells.Add(cellLabel);

					System.Web.UI.HtmlControls.HtmlTableCell cellValue = new System.Web.UI.HtmlControls.HtmlTableCell();
					Helper.FormatDataCell(cellValue, true);
					cellValue.Controls.Add(c);
					row.Cells.Add(cellValue);

					if (tblCriteria.Rows.Count == 0)
						this.tblCriteria.Rows.Add(row);
					else
						this.tblCriteria.Rows.Insert(tblCriteria.Rows.Count - 1, row);
				}
			}
		}


		private void RenderSystemTermControls()
		{
			_itatSystem.SetTermsVisible(false);
			foreach (Business.Term term in _itatSystem.Terms)
			{
				//For a facility term, force the control to be single-select
				//If this is a Facility Term (system term), then we assume that the system has an owning facility (and this is it)
				if (term.TermType == Kindred.Knect.ITAT.Business.TermType.Facility)
				{
					((Business.FacilityTerm)term).MultiSelect = false;
					term.Runtime.Visible = true;
				}
			}

			//The system xml could contain more terms than we need to perform searches, so filter the list of terms to be displayed here.  
			//The terms will display in the order that they are defined in the system xml.
			foreach (Business.Term term in _itatSystem.Terms)
			{
				if (term.SystemTerm)
				{
					if (term.UseDBField ?? false)
					{
						switch (term.DBFieldName)
						{
							case Data.DataNames._C_Term1:
							case Data.DataNames._C_Term2:
							case Data.DataNames._C_Term3:
							case Data.DataNames._C_Term4:
							case Data.DataNames._C_Term5:
							case Data.DataNames._C_Term6:
							case Data.DataNames._C_Term7:
								term.Runtime.Visible = true;
								break;
							default:
								break;
						}
					}
				}
			}

			_systemTermControls = Helper.CreateSearchControls(_itatSystem.Terms, true, _securityHelper);
			foreach (Business.Term term in _itatSystem.Terms)
			{
				if (term.Runtime.Visible)
				{
					System.Web.UI.HtmlControls.HtmlTableRow row = new System.Web.UI.HtmlControls.HtmlTableRow();

					System.Web.UI.HtmlControls.HtmlTableCell cellLabel = new System.Web.UI.HtmlControls.HtmlTableCell();
					Helper.FormatCaptionCell(cellLabel);
					switch (term.TermType)
					{
						case Kindred.Knect.ITAT.Business.TermType.Renewal:
							if (((Business.RenewalTerm)term).DisplayedDate == Business.DisplayedDate.EffectiveDate)
								cellLabel.InnerHtml = term.Name + "<br />(Effective Date)";
							else
								cellLabel.InnerHtml = term.Name + "<br />(Renewal/Expiration)";
							break;
						default:
							cellLabel.InnerHtml = term.Name;
							break;
					}
					row.Cells.Add(cellLabel);

					System.Web.UI.HtmlControls.HtmlTableCell cellValue = new System.Web.UI.HtmlControls.HtmlTableCell();
					Helper.FormatDataCell(cellValue, true);
					cellValue.Controls.Add(_systemTermControls[term.Name]);
					row.Cells.Add(cellValue);

					//Insert these term rows just above the KeyWords row...
					if (tblCriteria.Rows.Count == 0)
						this.tblCriteria.Rows.Add(row);
					else
						this.tblCriteria.Rows.Insert(tblCriteria.Rows.Count - 1, row);

					//Note:  This is commented out for now, so as to ensure that the results display same as before.
					//System.Web.UI.HtmlControls.HtmlTableCell cellCheckbox = new System.Web.UI.HtmlControls.HtmlTableCell();
					//CheckBox checkBox = new CheckBox();
					//checkBox.ID = GetCheckBoxName(term);
					//checkBox.Checked = true;
					//checkBox.Width = Unit.Pixel(25);
					//cellCheckbox.Controls.Add(checkBox);
					//row.Cells.Add(cellCheckbox);
				}
			}
		}

		private string GetCheckBoxName(Business.Term term)
		{
			return string.Format("chk{0}", term.Name.Replace(" ", "_"));
		}

		private void SetControlPreSelects()
		{
			if (_report == null)
				return;

			if (_report.SearchCriteria == null)
				return;

			if (_report.SearchCriteria.ManagedItemNumber != null)
				txtManagedItemNumber.Text = _report.SearchCriteria.ManagedItemNumber;

			if (_report.SearchCriteria.TemplateId.HasValue)
				ddlTemplate.SelectedValue = _report.SearchCriteria.TemplateId.ToString();

			if (_report.SearchCriteria.Statuses != null)
				if (_report.SearchCriteria.Statuses.Count == 1)
					if (ddlStatus.Items.Count > 0)
						ddlStatus.SelectedValue = _report.SearchCriteria.Statuses[0];

			if (_report.SearchCriteria.KeyWords != null)
				txtKeywords.Text = _report.SearchCriteria.KeyWords;

			string field1 = null;
			string field2 = null;
			foreach (Business.Term term in _itatSystem.Terms)
			{
				if (term.Runtime.Visible)
				{
					switch (term.TermType)
					{
						case Business.TermType.Text:
						case Business.TermType.MSO:
							GetSearchDBInfo(term.DBFieldName, ref field1, ref field2);
							SetTextBoxText(_systemTermControls[term.Name], field1);
							break;
						case Business.TermType.Date:
						case Business.TermType.Renewal:
							GetSearchDBInfo(term.DBFieldName, ref field1, ref field2);
							SetDateRangeText(_systemTermControls[term.Name], field1, field2);
							break;
						case Business.TermType.Facility:
							if (_report.SearchCriteria.FacilityIds != null)
								if (_report.SearchCriteria.FacilityIds.Count == 1)
									SetDropDownList(_systemTermControls[term.Name], _report.SearchCriteria.FacilityIds[0].ToString());
							break;
						case Business.TermType.PickList:
							GetSearchDBInfo(term.DBFieldName, ref field1, ref field2);
							SetDropDownList(_systemTermControls[term.Name], field1);
							break;
					}
				}
			}

			if (_report.SearchCriteria.ExternalTermsCriteria != null)
			{
				foreach (string interfaceName in _report.SearchCriteria.ExternalTermsCriteria.Keys)
				{
					Business.ExternalInterfaceSearchCriteria eisc = _report.SearchCriteria.ExternalTermsCriteria[interfaceName];
					Business.ExternalInterfaceConfig eic = _itatSystem.FindExternalInterfaceConfig(interfaceName);
					Business.ExternalTerm extTerm = _externalTerms[interfaceName];

					extTerm.InitializeSelectedItems();
					foreach (Business.ExternalInterfaceSearchCriteriaValue eiscv in eisc.Values)
					{
						Business.ExternalInterfaceListItem eili = new Business.ExternalInterfaceListItem(eic);
						eili.Key = eiscv.KeyValue;
						eili.FieldValues = new Dictionary<string, string>();
						foreach (Business.ExternalInterfaceSearchCriteriaDetail detail in eiscv.Details)
							eili.FieldValues.Add(detail.FieldName, detail.FieldValue);
						extTerm.SelectedItems.Add(eili);
					}
				}
			}
		}

		private void GetSearchDBInfo(string DBFieldName, ref string field1, ref string field2)
		{
			switch (DBFieldName)
			{
				case Data.DataNames._C_Term1:
					field1 = _report.SearchCriteria.TextTerm1;
					break;

				case Data.DataNames._C_Term2:
					field1 = _report.SearchCriteria.TextTerm2;
					break;

				case Data.DataNames._C_Term3:
					if (_report.SearchCriteria.DateTerm3Range.Start.HasValue)
						field1 = Utility.DateHelper.FormatDate(_report.SearchCriteria.DateTerm3Range.Start.Value, _itatSystem.DefaultDateFormat);
					if (_report.SearchCriteria.DateTerm3Range.End.HasValue)
						field2 = Utility.DateHelper.FormatDate(_report.SearchCriteria.DateTerm3Range.End.Value, _itatSystem.DefaultDateFormat);
					break;

				case Data.DataNames._C_Term4:
					field1 = _report.SearchCriteria.TextTerm4;
					break;

				case Data.DataNames._C_Term5:
					field1 = _report.SearchCriteria.TextTerm5;
					break;

				case Data.DataNames._C_Term6:
					if (_report.SearchCriteria.DateTerm6Range.Start.HasValue)
						field1 = Utility.DateHelper.FormatDate(_report.SearchCriteria.DateTerm6Range.Start.Value, _itatSystem.DefaultDateFormat);
					if (_report.SearchCriteria.DateTerm6Range.End.HasValue)
						field2 = Utility.DateHelper.FormatDate(_report.SearchCriteria.DateTerm6Range.End.Value, _itatSystem.DefaultDateFormat);
					break;

				case Data.DataNames._C_Term7:
					if (_report.SearchCriteria.DateTerm7Range.Start.HasValue)
						field1 = Utility.DateHelper.FormatDate(_report.SearchCriteria.DateTerm7Range.Start.Value, _itatSystem.DefaultDateFormat);
					if (_report.SearchCriteria.DateTerm7Range.End.HasValue)
						field2 = Utility.DateHelper.FormatDate(_report.SearchCriteria.DateTerm7Range.End.Value, _itatSystem.DefaultDateFormat);
					break;

				default:
					break;
			}
		}

		private void SetTextBoxText(WebControl webControl, string text)
		{
			TextBox textBox = webControl as TextBox;
			if (textBox != null)
			{
				textBox.Text = text;
				return;
			}

			Panel panel = webControl as Panel;
			if (panel != null)
			{
				foreach (Control control in panel.Controls)
				{
					TextBox textBox2 = control as TextBox;
					if (textBox2 != null)
					{
						textBox2.Text = text;
						return;
					}
				}
			}
		}


		private void SetDateRangeText(WebControl webControl, string sDateStart, string sDateEnd)
		{
			Panel panel = webControl as Panel;
			if (panel != null)
			{
				foreach (Control control in panel.Controls)
				{
					HtmlContainerControl htmlContainerControl = control as HtmlContainerControl;
					if (htmlContainerControl != null)
					{
						foreach (Control innerControl in htmlContainerControl.Controls)
						{
							TextBox textBox = innerControl as TextBox;
							if (textBox != null)
							{
								string sID = textBox.ID;
								DateTime dt;
								if (sID.IndexOf(Common.Names._IDENTIFIER_StartDate) >= 0)
									if (DateTime.TryParse(sDateStart, out dt))
										textBox.Text = sDateStart;
									else
										textBox.Text = string.Empty;
								else
									if (sID.IndexOf(Common.Names._IDENTIFIER_EndDate) >= 0)
										if (DateTime.TryParse(sDateEnd, out dt))
											textBox.Text = sDateEnd;
										else
											textBox.Text = string.Empty;
									else
										textBox.Text = string.Empty;
							}
						}
					}
				}
			}
		}


		private void SetDropDownList(WebControl webControl, string text)
		{
			Panel panel = webControl as Panel;
			if (panel != null)
			{
				foreach (Control control in panel.Controls)
				{
					DropDownList dropDownList = control as DropDownList;
					if (dropDownList != null)
					{
						dropDownList.SelectedValue = text;
						return;
					}
				}
			}
		}


		internal override HtmlGenericControl HTMLBody()
		{
			return body;
		}


		internal override Control ResizablePanel()
		{
			return pnlCriteria;
		}



		/// <summary>
		/// Handles the Command event of the btnSearch control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.Web.UI.WebControls.CommandEventArgs"/> instance containing the event data.</param>
		/// Modified by Larry Richardson LRR 3/13/2008 to save cookie
		protected void btnSearch_Command(object sender, CommandEventArgs e)
		{
			const int purgeKeepDays = 10;

			Business.SearchCriteria criteria = GetSearchCriteria();
			Guid guidCriteria = Business.Template.InsertSessionParameter(Serialize(criteria), purgeKeepDays);
			CreateCookie(COOKIE_SEARCH_CRITERIA, guidCriteria.ToString(), COOKIE_SEARCH_CRITERIA_VALUE);
			Context.Items[Common.Names._CNTXT_SearchCriteria] = criteria;

			List<Business.SearchResultSortField> searchResultColumns = GetSearchResultColumns(false);
			Guid guidSearchResultColumns = Business.Template.InsertSessionParameter(Serialize(searchResultColumns), purgeKeepDays);
			CreateCookie(COOKIE_SEARCH_COLUMNS, guidSearchResultColumns.ToString(), COOKIE_SEARCH_COLUMNS_VALUE);
			Context.Items[Common.Names._CNTXT_SearchResultColumns] = searchResultColumns.ToArray();

			Server.Transfer("ManagedItemSearchResults.aspx", true);
		}

		private List<Business.SearchResultSortField> GetSearchResultColumns(bool includeAll)
		{
			//Note:  This code is included so as to ensure that the results display same as before.
			includeAll = true;

			List<Business.SearchResultSortField> searchResultColumns = new List<Business.SearchResultSortField>();
			searchResultColumns.Add(Business.SearchResultSortField.ManagedItemNumber);
			if (includeAll || Request.Form["chkTemplate"] != null)
				searchResultColumns.Add(Business.SearchResultSortField.TemplateName);
			if (includeAll || Request.Form["chkStatus"] != null)
				searchResultColumns.Add(Business.SearchResultSortField.Status);
			if (includeAll)
				searchResultColumns.Add(Business.SearchResultSortField.State);

			foreach (Business.Term term in _itatSystem.Terms)
			{
				if (includeAll || Request.Form[GetCheckBoxName(term)] != null)
				{
					if (term.TermType == Business.TermType.Facility)
						searchResultColumns.Add(Business.SearchResultSortField.Facility);
					else
					{
						switch (term.DBFieldName)
						{
							case Data.DataNames._C_Term1:
								searchResultColumns.Add(Business.SearchResultSortField.TextTerm1);
								break;

							case Data.DataNames._C_Term2:
								searchResultColumns.Add(Business.SearchResultSortField.TextTerm2);
								break;

							case Data.DataNames._C_Term3:
								searchResultColumns.Add(Business.SearchResultSortField.DateTerm3);
								break;

							//Note:  These are commented out for now, so as to ensure that the results display same as before.
							//case Data.DataNames._C_Term4:
							//    searchResultColumns.Add(Business.SearchResultSortField.TextTerm4);
							//    break;

							//case Data.DataNames._C_Term5:
							//    searchResultColumns.Add(Business.SearchResultSortField.TextTerm5);
							//    break;

							//case Data.DataNames._C_Term6:
							//    searchResultColumns.Add(Business.SearchResultSortField.DateTerm6);
							//    break;

							//case Data.DataNames._C_Term7:
							//    searchResultColumns.Add(Business.SearchResultSortField.DateTerm7);
							//    break;
						}
					}
				}
			}
			return searchResultColumns;
		}

		private string Serialize(Object objValue)
		{
			System.IO.MemoryStream ms = new System.IO.MemoryStream();
			new BinaryFormatter().Serialize(ms, objValue);
			return Convert.ToBase64String(ms.ToArray());
		}

		private Object Deserialize(string strValue)
		{
			System.IO.MemoryStream ms = new System.IO.MemoryStream(Convert.FromBase64String(strValue));
			return new BinaryFormatter().Deserialize(ms);
		}


		/// <summary>
		/// Creates the search result cookie.
		/// </summary>
		/// <param name="cookieValue">The criteria.</param>
		/// Created by Larry Richardson LRR 4/9/2008
		private void CreateCookie(string cookieName, string cookieValue, string cookieValueName)
		{
			HttpCookie cookie = new HttpCookie(cookieName);
			cookie.Values.Add(cookieValueName, cookieValue);
			cookie.Path = @"/";
			Response.Cookies.Add(cookie);
		}

		/// <summary>
		/// Gets the criteria cookie.
		/// </summary>
		/// <param name="cookie">The cookie.</param>
		/// <returns></returns>
		/// Created by Larry Richardson LRR 4/9/2008
		private string GetCookie(string cookieName, string valueName)
		{
			HttpCookie cookie = Request.Cookies[cookieName];
			if (cookie != null)
			{
				return cookie[valueName];
			}
			else
				return null;
		}


		protected void btnReportCancel_Command(object sender, CommandEventArgs e)
		{
			Server.Transfer("ReportSearch.aspx");
		}


		protected void btnReportContinue_Command(object sender, CommandEventArgs e)
		{
			Context.Items.Add(Common.Names._CNTXT_SearchCriteria, GetSearchCriteria());
			Server.Transfer("ReportSearchResults.aspx");
		}


		private Business.SearchCriteria GetSearchCriteria()
		{
			Business.SearchCriteria criteria = new Business.SearchCriteria();
			criteria.ManagedItemNumber = txtManagedItemNumber.Text;

			if (ddlTemplate.SelectedIndex > 0)
				criteria.TemplateId = new Guid(ddlTemplate.SelectedValue);
			else
				criteria.TemplateId = null;

			if (ddlStatus.SelectedIndex > 0)
			{
				criteria.Statuses.Add(ddlStatus.SelectedValue);
			}
			else
			{
				//build list of statuses that were in the Status dropdown
				if (_securityHelper == null)
					_securityHelper = new Business.SecurityHelper(_itatSystem);
				List<Business.Status> statuses = _itatSystem.ViewableStatuses(_securityHelper.UserRoles);
				foreach (Business.Status status in statuses)
					criteria.Statuses.Add(status.Name);
			}

			if (!string.IsNullOrEmpty(txtKeywords.Text))
				criteria.KeyWords = txtKeywords.Text;
			else
				criteria.KeyWords = null;

			//Read system term controls
			foreach (Business.Term term in _itatSystem.Terms)
			{
				switch (term.TermType)
				{
					case Kindred.Knect.ITAT.Business.TermType.Date:
					case Kindred.Knect.ITAT.Business.TermType.Renewal:
						switch (term.DBFieldName)
						{
							case Data.DataNames._C_Term3:
							case Data.DataNames._C_Term6:
							case Data.DataNames._C_Term7:
								DateTime dt;
								Business.SearchCriteria.DateRange dateRange = new Business.SearchCriteria.DateRange();
								if (DateTime.TryParse(Request.Form[Helper.ControlID(term.Name, Common.Names._IDENTIFIER_StartDate)], out dt))
									dateRange.Start = dt;
								if (DateTime.TryParse(Request.Form[Helper.ControlID(term.Name, Common.Names._IDENTIFIER_EndDate)], out dt))
									dateRange.End = dt;
								switch (term.DBFieldName)
								{
									case Data.DataNames._C_Term3:
										criteria.DateTerm3Range = dateRange;
										break;

									case Data.DataNames._C_Term6:
										criteria.DateTerm6Range = dateRange;
										break;

									case Data.DataNames._C_Term7:
										criteria.DateTerm7Range = dateRange;
										break;
								}
								break;
						}
						break;

					case Kindred.Knect.ITAT.Business.TermType.Facility:
						Business.FacilityTerm facilityTerm = term as Business.FacilityTerm;
						string strFacID = Request.Form[Helper.ControlID(term.Name, Kindred.Knect.ITAT.Business.TermType.Facility)];
						if (!string.IsNullOrEmpty(strFacID))
						{
							int facID = int.Parse(strFacID);
							if (facID > 0)
							{
								criteria.FacilityIds.Add(int.Parse(strFacID));
							}
							else
							{
								Business.FacilityCollection facilities;
								if (facilityTerm.UseUserSecurity ?? false)
									if (facilityTerm.IncludeChildren ?? false)
										facilities = Business.FacilityCollection.FilteredFacilityList(_securityHelper.AllUserFacilities, facilityTerm);
									else
										facilities = Business.FacilityCollection.FilteredFacilityList(_securityHelper.UserFacilities, facilityTerm);
								else
									facilities = Business.FacilityCollection.FilteredFacilityList(Business.FacilityCollection.FacilityList(Data.Facility.CorporateFacilityId, facilityTerm.IncludeChildren ?? false), facilityTerm);
								criteria.FacilityIds.AddRange(facilities.Keys);
							}
						}
						break;

					default:
						string value = Request.Form[Helper.ControlID(term.Name, term.TermType)];
						if (!string.IsNullOrEmpty(value))
						{
							value = criteria.UpdateTermValue(term.DBFieldName, value, _itatSystem);
							switch (term.DBFieldName)
							{
								case Data.DataNames._C_Term1:
									criteria.TextTerm1 = value;
									break;

								case Data.DataNames._C_Term2:
									criteria.TextTerm2 = value;
									break;

								case Data.DataNames._C_Term4:
									criteria.TextTerm4 = value;
									break;

								case Data.DataNames._C_Term5:
									criteria.TextTerm5 = value;
									break;

								default:
									break;
							}
						}
						break;
				}
			}

			foreach (Business.ExternalInterfaceConfig eic in _itatSystem.ExternalInterfaces)
			{
				Business.ExternalTerm extTerm = _externalTerms[eic.Name];
				if (extTerm.SelectedItems != null)
				{
					ExternalTermControl c = BuildExternalControl(eic);
					Business.ExternalInterfaceSearchCriteria eisc = new Kindred.Knect.ITAT.Business.ExternalInterfaceSearchCriteria();
					eisc.InterfaceConfigName = eic.Name;
					eisc.Values = new List<Business.ExternalInterfaceSearchCriteriaValue>();
					foreach (Business.ExternalInterfaceListItem eili in extTerm.SelectedItems)
					{
						Business.ExternalInterfaceSearchCriteriaValue eiscv = new Kindred.Knect.ITAT.Business.ExternalInterfaceSearchCriteriaValue();
						eiscv.KeyValue = eili.Key;
						foreach (string fieldName in eili.FieldValues.Keys)
							eiscv.Details.Add(new Kindred.Knect.ITAT.Business.ExternalInterfaceSearchCriteriaDetail(fieldName, eili.FieldValues[fieldName]));
						eisc.Values.Add(eiscv);
					}
					criteria.ExternalTermsCriteria.Add(eisc.InterfaceConfigName, eisc);
				}
			}
			return criteria;
		}


		protected override object SaveViewState()
		{
			ViewState[VSKEY_REPORT] = _report;
			ViewState[VSKEY_EXTERNALTERMS] = _externalTerms;
			return base.SaveViewState();
		}

		protected override void LoadViewState(object savedState)
		{
			base.LoadViewState(savedState);
			_externalTerms = (Dictionary<string, Business.ExternalTerm>)ViewState[VSKEY_EXTERNALTERMS];
			try
			{
				_report = (Business.Report)ViewState[VSKEY_REPORT];
			}
			catch { }
		}



	}
}
