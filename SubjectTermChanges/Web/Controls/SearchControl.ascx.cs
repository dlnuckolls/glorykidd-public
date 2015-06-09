using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Kindred.Knect.ITAT.Business;
using System.Web.UI.HtmlControls;
using System.Reflection;

namespace Kindred.Knect.ITAT.Web.Controls
{
    [Serializable]
    public partial class SearchControl : System.Web.UI.UserControl
    {
        private ITATSystem _itatSystem;

        private bool _displayTerm1;
        private bool _displayTerm2;
        private bool _displayTerm3;
        private bool _displayTerm4;
        private bool _displayTerm5;
        private bool _displayTerm6;
        private bool _displayTerm7;
        private SearchCriteria _searchCriteria;

        public SearchControl()
        {
            string qsValue = ((Page)HttpContext.Current.Handler).Request.QueryString[Common.Names._QS_ITAT_SYSTEM_ID];
            //ASSUMPTION: 
            //   If qsValue is of the form xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx or {xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx}, 
            //           then it is a systemID.
            //   Otherwise, it is the system name.
            if (Utility.TextHelper.IsGuid(qsValue))
                _itatSystem = Business.ITATSystem.Get(new Guid(qsValue));
            else
                _itatSystem = Business.ITATSystem.Get(qsValue);


            _displayTerm1 = ((ITAT.Web.Admin.DataStoreDefPage)((Page)HttpContext.Current.Handler)).DisplayTerm1;
            _displayTerm2 = ((ITAT.Web.Admin.DataStoreDefPage)((Page)HttpContext.Current.Handler)).DisplayTerm2;
            _displayTerm3 = ((ITAT.Web.Admin.DataStoreDefPage)((Page)HttpContext.Current.Handler)).DisplayTerm3;
            _displayTerm4 = ((ITAT.Web.Admin.DataStoreDefPage)((Page)HttpContext.Current.Handler)).DisplayTerm4;
            _displayTerm5 = ((ITAT.Web.Admin.DataStoreDefPage)((Page)HttpContext.Current.Handler)).DisplayTerm5;
            _displayTerm6 = ((ITAT.Web.Admin.DataStoreDefPage)((Page)HttpContext.Current.Handler)).DisplayTerm6;
            _displayTerm7 = ((ITAT.Web.Admin.DataStoreDefPage)((Page)HttpContext.Current.Handler)).DisplayTerm7;
        }

        public SearchControl(ITATSystem itatSystem, bool showTerm1, bool showTerm2, bool showTerm3, bool showTerm4, bool showTerm5, bool showTerm6, bool showTerm7)
        {
            this._itatSystem = itatSystem;
            this._displayTerm1 = showTerm1;
            this._displayTerm2 = showTerm2;
            this._displayTerm3 = showTerm3;
            this._displayTerm4 = showTerm4;
            this._displayTerm5 = showTerm5;
            this._displayTerm6 = showTerm6;
            this._displayTerm7 = showTerm7;


        }


        public SearchControl(ITATSystem itatSystem, bool showTerm1, bool showTerm2, bool showTerm3, bool showTerm4, bool showTerm5, bool showTerm6, bool showTerm7, SearchCriteria searchCriteria)
        {
            this._itatSystem = itatSystem;
            this._displayTerm1 = showTerm1;
            this._displayTerm2 = showTerm2;
            this._displayTerm3 = showTerm3;
            this._displayTerm4 = showTerm4;
            this._displayTerm5 = showTerm5;
            this._displayTerm6 = showTerm6;
            this._displayTerm7 = showTerm7;

            this._searchCriteria=searchCriteria;
        }



        public void SetControlPreSelects(SearchCriteria searchCriteria)
        {

            string field1 = null;
            string field2 = null;
            _systemTermControls = Helper.CreateSearchControls(_itatSystem.Terms, true, new Business.SecurityHelper(_itatSystem));
            foreach (Business.Term term in _itatSystem.Terms)
            {
                if (term.Runtime.Visible)
                {
                    switch (term.TermType)
                    {
                        case Business.TermType.Text:
                        case Business.TermType.MSO:
                            GetSearchDBInfo(term.DBFieldName, ref field1, ref field2, searchCriteria);
                            SetTextBoxText(_systemTermControls[term.Name], field1);
                            break;
                        case Business.TermType.Date:
                        case Business.TermType.Renewal:
                            GetSearchDBInfo(term.DBFieldName, ref field1, ref field2, searchCriteria);
                            SetDateRangeText(_systemTermControls[term.Name], field1, field2);
                            break;
                        case Business.TermType.Facility:
                            if (searchCriteria.FacilityIds != null)
                                if (searchCriteria.FacilityIds.Count == 1)
                                    SetDropDownList(_systemTermControls[term.Name], searchCriteria.FacilityIds[0].ToString());
                            break;
                        case Business.TermType.PickList:
                            GetSearchDBInfo(term.DBFieldName, ref field1, ref field2, searchCriteria);
                            SetDropDownList(_systemTermControls[term.Name], field1);
                            break;
                    }
                }
            }


        }


        private void GetSearchDBInfo(string DBFieldName, ref string field1, ref string field2,SearchCriteria searchCriteria)
        {
            switch (DBFieldName)
            {
                case Data.DataNames._C_Term1:
                    field1 = searchCriteria.TextTerm1;
                    break;
                case Data.DataNames._C_Term2:
                    field1 = searchCriteria.TextTerm2;
                    break;
                case Data.DataNames._C_Term3:
                    if (searchCriteria.DateTerm3Range.Start.HasValue)
                        field1 = Utility.DateHelper.FormatDate(searchCriteria.DateTerm3Range.Start.Value, _itatSystem.DefaultDateFormat);
                    if (searchCriteria.DateTerm3Range.End.HasValue)
                        field2 = Utility.DateHelper.FormatDate(searchCriteria.DateTerm3Range.End.Value, _itatSystem.DefaultDateFormat);
                    break;
                case Data.DataNames._C_Term4:
                    field1 =searchCriteria.TextTerm4;
                    break;
                case Data.DataNames._C_Term5:
                    field1 = searchCriteria.TextTerm5;
                    break;
                case Data.DataNames._C_Term6:
                    if (searchCriteria.DateTerm6Range.Start.HasValue)
                        field1 = Utility.DateHelper.FormatDate(searchCriteria.DateTerm6Range.Start.Value, _itatSystem.DefaultDateFormat);
                    if (searchCriteria.DateTerm3Range.End.HasValue)
                        field2 = Utility.DateHelper.FormatDate(searchCriteria.DateTerm6Range.End.Value, _itatSystem.DefaultDateFormat);
                    break;
                case Data.DataNames._C_Term7:
                    if (searchCriteria.DateTerm6Range.Start.HasValue)
                        field1 = Utility.DateHelper.FormatDate(searchCriteria.DateTerm7Range.Start.Value, _itatSystem.DefaultDateFormat);
                    if (searchCriteria.DateTerm3Range.End.HasValue)
                        field2 = Utility.DateHelper.FormatDate(searchCriteria.DateTerm7Range.End.Value, _itatSystem.DefaultDateFormat);
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

                    RadioButtonList radioButtonList = control as RadioButtonList;
                    if (radioButtonList != null)
                    {
                        radioButtonList.SelectedValue = text;
                        return;
                    }
                }
            }
        }


        protected void Page_Load(object sender, EventArgs e)
        {

            _itatSystem.SetTermsVisible(false);
            foreach (Business.Term term in _itatSystem.Terms)
            {
                //For a facility term, force the control to be single-select
                if (term.TermType == Kindred.Knect.ITAT.Business.TermType.Facility)
                {
                    ((Business.FacilityTerm)term).MultiSelect = false;
                    term.Runtime.Visible = true;
                }
            }

            //The system xml could contain more terms than we need to perform searches, so filter the list of terms to be displayed here.  
            //The terms will display in the order that they are defined in the system xml.
            _systemTermControls = Helper.CreateSearchControls(_itatSystem.Terms, true, new Business.SecurityHelper(_itatSystem));
            foreach (Business.Term term in _itatSystem.Terms)
            {
                if (term.SystemTerm)
                {
                    if (term.UseDBField ?? false)
                    {
                        switch (term.DBFieldName)
                        {
                            case Data.DataNames._C_Term1:
                                term.Runtime.Visible = _displayTerm1;
                                break;
                            case Data.DataNames._C_Term2:
                                term.Runtime.Visible = _displayTerm2;
                                break;
                            case Data.DataNames._C_Term3:
                                term.Runtime.Visible = _displayTerm3;
                                break;
                            case Data.DataNames._C_Term4:
                                term.Runtime.Visible = _displayTerm4;
                                break;
                            case Data.DataNames._C_Term5:
                                term.Runtime.Visible = _displayTerm5;
                                   break;
                            case Data.DataNames._C_Term6:
                                term.Runtime.Visible = _displayTerm6;
                                break;
                            case Data.DataNames._C_Term7:
                                term.Runtime.Visible = _displayTerm7;
                                    break;
                            default:
                                break;
                        }
                    }
                }
            }

            if (this._searchCriteria != null)
                this.SetControlPreSelects(_searchCriteria);

            HtmlTable dtblCriteria1 = new HtmlTable();
            dtblCriteria1.ID = "dtblCriteria1";
            dtblCriteria1.EnableViewState = true;
            dtblCriteria1.CellPadding = 1;
            dtblCriteria1.CellSpacing = 0;
            dtblCriteria1.Border = 0;
            this.Controls.Add(dtblCriteria1);

            foreach (Business.Term term in _itatSystem.Terms)
            {
                if (term.Runtime.Visible)
                {
                    System.Web.UI.HtmlControls.HtmlTableRow row = new System.Web.UI.HtmlControls.HtmlTableRow();
                    row.ID = "tr_row_" + term.Name;
                    System.Web.UI.HtmlControls.HtmlTableCell cellLabel = new System.Web.UI.HtmlControls.HtmlTableCell();
                    cellLabel.ID = row.ID + "_Cell_" + row.Cells.Count.ToString();
                    cellLabel.NoWrap = true;
                    cellLabel.Width = "105";
                    cellLabel.Attributes.Add("class", "ProfileCaption");
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
                            cellLabel.Width = "120";
                            break;
                    }

                    row.Cells.Add(cellLabel);

                    System.Web.UI.HtmlControls.HtmlTableCell cellValue = new System.Web.UI.HtmlControls.HtmlTableCell();

                    Helper.FormatDataCell(cellValue, true);
                    cellValue.ID = row.ID + "_Cell_" + row.Cells.Count.ToString();
                    cellValue.Width = "100%";
                    cellValue.Attributes.Add("class", "ProfileEdit");
                    row.Cells.Add(cellValue);
                    cellValue.Controls.Add(_systemTermControls[term.Name]);


                    if (dtblCriteria1.Rows.Count == 0)
                        dtblCriteria1.Rows.Add(row);
                    else
                        dtblCriteria1.Rows.Insert(dtblCriteria1.Rows.Count - 1, row);
                }
            }

          
        }
        public Dictionary<string, WebControl> _systemTermControls;
    }
}
