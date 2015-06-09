using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using Kindred.Knect.ITAT.Business;
using System.Web.UI.HtmlControls;
using System.Collections;
using System.Data;
using System.Text.RegularExpressions;
using System.Reflection;
using Kindred.Knect.ITAT.Web.Controls;
using System.IO;

namespace Kindred.Knect.ITAT.Web.Admin
{

    public class DataStoreDefPage : BaseSystemPage
    {
        public bool DisplayTerm1 { get { return (bool)ViewState["_displayTerm1"]; } set { ViewState["_displayTerm1"] = value; } }
        public bool DisplayTerm2 { get { return (bool)ViewState["_displayTerm2"]; } set { ViewState["_displayTerm2"] = value; } }
        public bool DisplayTerm3 { get { return (bool)ViewState["_displayTerm3"]; } set { ViewState["_displayTerm3"] = value; } }
        public bool DisplayTerm4 { get { return (bool)ViewState["_displayTerm4"]; } set { ViewState["_displayTerm4"] = value; } }
        public bool DisplayTerm5 { get { return (bool)ViewState["_displayTerm5"]; } set { ViewState["_displayTerm5"] = value; } }
        public bool DisplayTerm6 { get { return (bool)ViewState["_displayTerm6"]; } set { ViewState["_displayTerm6"] = value; } }
        public bool DisplayTerm7 { get { return (bool)ViewState["_displayTerm7"]; } set { ViewState["_displayTerm7"] = value; } }
        
        internal override HtmlGenericControl HTMLBody()
        {
            throw new NotImplementedException();
        }

        internal override Control ResizablePanel()
        {
            throw new NotImplementedException();
        }

        public Business.SearchCriteria GetSearchCriteria(string ControlId)
        {
            Business.SearchCriteria criteria = new Business.SearchCriteria();

            //Read system term controls
            foreach (Business.Term term in _itatSystem.Terms)
            {
                switch (term.TermType)
                {
                    case Kindred.Knect.ITAT.Business.TermType.Date:
                    case Kindred.Knect.ITAT.Business.TermType.Renewal:
                        if (term.DBFieldName == Data.DataNames._C_Term3)
                        {
                            DateTime dt;
                            Business.SearchCriteria.DateRange dr = new SearchCriteria.DateRange();
                            if (DateTime.TryParse(Request.Form[ControlId + "$" + Helper.ControlID(term.Name, Common.Names._IDENTIFIER_StartDate)], out dt))
                                dr.Start = dt;
                            if (DateTime.TryParse(Request.Form[ControlId + "$" + Helper.ControlID(term.Name, Common.Names._IDENTIFIER_EndDate)], out dt))
                                dr.End = dt;
                            criteria.DateTerm3Range = dr;
                        }

                        else if (term.DBFieldName == Data.DataNames._C_Term6)
                        {
                            DateTime dt;
                            Business.SearchCriteria.DateRange dr = new SearchCriteria.DateRange();
                            if (DateTime.TryParse(Request.Form[ControlId + "$" + Helper.ControlID(term.Name, Common.Names._IDENTIFIER_StartDate)], out dt))
                                dr.Start = dt;
                            if (DateTime.TryParse(Request.Form[ControlId + "$" + Helper.ControlID(term.Name, Common.Names._IDENTIFIER_EndDate)], out dt))
                                dr.End = dt;
                            criteria.DateTerm6Range = dr;
                        }

                        else if (term.DBFieldName == Data.DataNames._C_Term7)
                        {
                            DateTime dt;
                            Business.SearchCriteria.DateRange dr = new SearchCriteria.DateRange();
                            if (DateTime.TryParse(Request.Form[ControlId + "$" + Helper.ControlID(term.Name, Common.Names._IDENTIFIER_StartDate)], out dt))
                                dr.Start = dt;
                            if (DateTime.TryParse(Request.Form[ControlId + "$" + Helper.ControlID(term.Name, Common.Names._IDENTIFIER_EndDate)], out dt))
                                dr.End = dt;
                            criteria.DateTerm7Range = dr;
                        }
                        break;

                    case Kindred.Knect.ITAT.Business.TermType.Facility:
                        Business.FacilityTerm facilityTerm = term as Business.FacilityTerm;
                        string strFacID = Request.Form[ControlId + "$" + Helper.ControlID(term.Name, Kindred.Knect.ITAT.Business.TermType.Facility)];
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
                                        facilities = Business.FacilityCollection.FilteredFacilityList(new Business.SecurityHelper(_itatSystem).AllUserFacilities, facilityTerm);
                                    else
                                        facilities = Business.FacilityCollection.FilteredFacilityList(new Business.SecurityHelper(_itatSystem).UserFacilities, facilityTerm);
                                else
                                    facilities = Business.FacilityCollection.FilteredFacilityList(Business.FacilityCollection.FacilityList(Data.Facility.CorporateFacilityId, facilityTerm.IncludeChildren ?? false), facilityTerm);
                                criteria.FacilityIds.AddRange(facilities.Keys);
                            }
                        }
                        break;

                    default:
                        string value = Request.Form[ControlId + "$" + Helper.ControlID(term.Name, term.TermType)];
                        if (!string.IsNullOrEmpty(value))
                        {
                            value = criteria.UpdateTermValue(term.DBFieldName, value, null);
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

            return criteria;
        }
    }

    public partial class EditDataStoreDefinition : DataStoreDefPage
    {

        #region Private Members
        private DataStoreDefinition _dataStoreDefinition;
        private const string VSKEY_REPORT = "_vskey_Report";
        private const string VSKEY_EXTERNALTERMS = "_vskey_ExternalTerms";
        private const string VSKEY_SELECTEDTERMS = "_vskey_SelectedTerms";
        private const string LIST_TERMS = "_list_Terms";
        private const string FIELD_NAME = "Name";
        private const string SELECTED_TERMS = "_selectedTerms";
        private const string VALIDATION_GROUP = "RequiredValidation";
        private const string STATUS_NAME = "Name";
        private const string LIST_KEY = "Key";
        private const string LIST_VALUE = "Value";
        private const char EMAIL_DELIMITER = ',';
        private const string PRIMARY_FACILITY_DELIMITER = ",";

        protected Control searchControl;
        #endregion

        #region Protected Members
        protected ITAT.Business.Template _template;
        protected ITAT.Business.ITATSystem _itatsystem;
        private List<DataStoreField> _listTerms;
        protected List<DataStoreField> listTerms
        {
            get
            {
                if (_listTerms == null)
                    _listTerms = new List<DataStoreField>();
                return _listTerms;
            }

            set
            {
                _listTerms = value;
            }
        }
        internal List<DataStoreField> SelectedTerms
        {
            get
            {
                return (List<DataStoreField>)ViewState[VSKEY_SELECTEDTERMS];
            }
            set
            {
                ViewState[VSKEY_SELECTEDTERMS] = value;
            }
        }
        #endregion
        #region Private Methods
        private UserControl LoadControl(string UserControlPath, params object[] constructorParameters)
        {
            List<Type> constParamTypes = new List<Type>();
            foreach (object constParam in constructorParameters)
            {
                constParamTypes.Add(constParam.GetType());
            }

            UserControl ctl = Page.LoadControl(UserControlPath) as UserControl;

            // Find the relevant constructor
            ConstructorInfo constructor = ctl.GetType().BaseType.GetConstructor(constParamTypes.ToArray());

            //And then call the relevant constructor
            if (constructor == null)
            {
                throw new MemberAccessException("The requested constructor was not found on : " + ctl.GetType().BaseType.ToString());
            }
            else
            {
                constructor.Invoke(ctl, constructorParameters);
            }

            // Finally return the fully initialized UC
            return ctl;
        }

        #endregion
        #region Events
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                if (Request.QueryString[Common.Names._QS_DEFINITION_ID].ToString() != string.Empty)
                {
                    try
                    {
                        Dictionary<Guid, Dictionary<Guid, string>> termNameLookup = null;
                        Dictionary<Guid /*TemplateID*/, Dictionary<Guid /*ComplexListID*/, Dictionary<Guid /*FieldID*/, string /*FieldName*/>>> fieldNameLookup = null;
                        SystemStore.GetTermAndFieldNameLookup(_itatSystem.ID, ref termNameLookup, ref fieldNameLookup);
                        _dataStoreDefinition = Business.DataStoreDefinition.GetDataStoreDefinitionByID(new Guid(Request.QueryString[Common.Names._QS_DEFINITION_ID].ToString()), _itatSystem, termNameLookup, fieldNameLookup, false);
                    }
                    catch (Exception ex)
                    {
                        RegisterAlert(string.Format("Error: {0}",ex.Message));
                    }
                }

                if (_dataStoreDefinition == null)
                {
                    RegisterAlert("Data Store Definition does not exists");
                }
                else
                {
                    DisplayTerm1 = true;
                    DisplayTerm2 = true;
                    DisplayTerm3 = false;
                    DisplayTerm4 = true;
                    DisplayTerm5 = true;
                    DisplayTerm6 = false;
                    DisplayTerm7 = false;
                    searchControl = LoadControl("~\\Controls\\SearchControl.ascx", base._itatSystem, DisplayTerm1, DisplayTerm2, DisplayTerm3, DisplayTerm4, DisplayTerm5, DisplayTerm6,DisplayTerm7, _dataStoreDefinition.DataStoreConfig.SearchCriteria);
                    searchControl.ID = "MySearchControl";

                    apnlCriteria.Controls.Add(searchControl);
                    _itatsystem = Business.ITATSystem.Get(_dataStoreDefinition.SystemID);

                    LoadStatusList();

                    lblSystemName.Text = Business.ITATSystem.Get(_dataStoreDefinition.SystemID).Name;
                    lblDefinitionName.Text = _dataStoreDefinition.Name;
                    txtDescription.Text = _dataStoreDefinition.Description;

                    ddlDefaultDateFormat.DataSource = DataStoreDefinition.GetDateFormatList();
                    ddlDefaultDateFormat.DataBind();

                    ddlDefaultDateFormat.SelectedIndex = ddlDefaultDateFormat.Items.IndexOf(new ListItem(_dataStoreDefinition.DataStoreConfig.DefaultDateFormat));

                    LoadTemplateList(_dataStoreDefinition.SystemID);
                    string errSelectedTemplates = "";
                    foreach (KeyValuePair<Guid, string> template in _dataStoreDefinition.DataStoreConfig.Templates)
                    {

                        if (!lstTemplates.Items.Contains(new ListItem(template.Value, template.Key.ToString())))
                            errSelectedTemplates = errSelectedTemplates + string.Format("- {0} \\n", template.Value.ToString());
                        else
                        {
                            lstTemplates.Items.Remove(new ListItem(template.Value, template.Key.ToString()));
                            lstSelectedTemplates.Items.Add(new ListItem(template.Value, template.Key.ToString()));
                        }
                    }

                    if (errSelectedTemplates.Length > 0)
                        RegisterAlert("Following selected Template(s) are no longer Active \\n" + errSelectedTemplates);

                    SelectedTerms = _dataStoreDefinition.DataStoreConfig.Terms;

                    LoadTermList();
                    SortlistItems(lstTemplates);
                    MoveTerm(SelectedTerms);


                    if (_dataStoreDefinition.Active)
                        radActiveYes.Checked = true;
                    else
                        radActiveNo.Checked = true;

                    foreach (ListItem li in chklstStatus.Items)
                    {
                        if (_dataStoreDefinition.DataStoreConfig.SearchCriteria.Statuses.IndexOf(li.Text) > -1)
                            li.Selected = true;
                        else
                            li.Selected = false;
                    }

                    foreach (string status in _dataStoreDefinition.DataStoreConfig.SearchCriteria.Statuses)
                    {
                        chklstStatus.Items.FindByText(status).Selected = true;
                    }
                    radTypeDelta.Checked = _dataStoreDefinition.DataStoreConfig.LoadType == LoadType.Delta;
                    txtDeltaDays.Text = _dataStoreDefinition.DataStoreConfig.DeltaDays.ToString();
                    txtPath.Text = _dataStoreDefinition.DataStoreConfig.Path;
                    txtEmail.Text = _dataStoreDefinition.DataStoreConfig.ErrorLogRecepientEmail;
                }

                btnRemoveTerm.OnClientClick = string.Format("return confirm('All the selected Terms will be removed from the list.');");

                btnRemoveTemplate.OnClientClick = string.Format("return confirm('All the selected Terms from the selected Templates also removed by removing Templates.');");
            }
        }

        protected void btnAddTemplate_Click(object sender, EventArgs e)
        {
            if (lstTemplates.Items.Count > 0)
            {
                foreach (ListItem li in lstTemplates.Items)
                {
                    if (li.Selected)
                    {
                        lstSelectedTemplates.Items.Add(li);
                    }
                }

                while (lstTemplates.GetSelectedIndices().Length > 0)
                    lstTemplates.Items.Remove(lstTemplates.SelectedItem);

                LoadTermList();
            }
        }

        protected void btnRemoveTemplate_Click(object sender, EventArgs e)
        {
            if (lstSelectedTemplates.Items.Count > 0)
            {
                foreach (ListItem li in lstSelectedTemplates.Items)
                {
                    if (li.Selected)
                    {
                        lstTemplates.Items.Add(li);
                    }
                }

                while (lstSelectedTemplates.GetSelectedIndices().Length > 0)
                    lstSelectedTemplates.Items.Remove(lstSelectedTemplates.SelectedItem);

                SortlistItems(lstTemplates);
            }
            LoadTermList();
        }

        protected void btnAddTerm_Click(object sender, EventArgs e)
        {
            UpdateTermSize();
            List<DataStoreField> selectedTerms = SelectedTerms;
            listTerms = (List<DataStoreField>)ViewState[LIST_TERMS];

            if (lstTerms.Items.Count > 0)
            {
                foreach (ListItem lt in lstTerms.Items)
                {
                    if (lt.Selected)
                    {
                        DataStoreField foundTerm = listTerms.Find(t => t.Name.Equals(lt.Text));
                        selectedTerms.Add(foundTerm);
                    }

                }
            }

            listTerms.RemoveAll(t => selectedTerms.Exists(x => x.Name.Equals(t.Name)));
            listTerms = listTerms.Distinct().ToList();
            SelectedTerms = selectedTerms;
            ViewState[LIST_TERMS] = listTerms;
            BindListViewSelectedTerms();

            lstTerms.Items.Clear();
            lstTerms.DataSource = listTerms;
            lstTerms.DataTextField = FIELD_NAME;
            lstTerms.DataValueField = FIELD_NAME;
            lstTerms.DataBind();

            SortlistItems(lstTerms);
        }

        protected void btnRemoveTerm_Click(object sender, EventArgs e)
        {
            UpdateTermSize();
            List<DataStoreField> selectedTerms = SelectedTerms;
            listTerms = (List<DataStoreField>)ViewState[LIST_TERMS];

            if (lvSelectedTerms.Items.Count > 0)
            {
                foreach (ListViewItem lvi in lvSelectedTerms.Items)
                {
                    CheckBox chk = (CheckBox)lvi.FindControl("chkTerm");
                    if (chk.Checked)
                    {
                        Predicate<DataStoreField> p1 = delegate(DataStoreField t) { return ((t.Name.Equals(chk.Text))); };
                        DataStoreField checkedTerm = selectedTerms.Find(p1);
                        listTerms.Add(checkedTerm);
                        selectedTerms.Remove(checkedTerm);
                    }
                }

                SelectedTerms = selectedTerms;
                ViewState[LIST_TERMS] = listTerms;
                BindListViewSelectedTerms();


                lstTerms.Items.Clear();
                lstTerms.DataSource = listTerms;
                lstTerms.DataTextField = FIELD_NAME;
                lstTerms.DataValueField = FIELD_NAME;
                lstTerms.DataBind();

                SortlistItems(lstTerms);
            }
        }

        protected void btnUp_Click(object sender, EventArgs e)
        {
            UpdateTermSize();
            List<DataStoreField> selectedTerms = (List<DataStoreField>)ViewState[SELECTED_TERMS];

            List<DataStoreField> checkedTerms = new List<DataStoreField>();

            foreach (ListViewItem lvi in lvSelectedTerms.Items)
            {
                TextBox txt = (TextBox)lvi.FindControl("txtAlias");

                CheckBox chk = (CheckBox)lvi.FindControl("chkTerm");
                Predicate<DataStoreField> p2 = delegate(DataStoreField t) { return ((t.Name.Equals(chk.Text))); };
                SelectedTerms.Find(p2).Alias = txt.Text;

                if (chk.Checked)
                {
                    Predicate<DataStoreField> p1 = delegate(DataStoreField t) { return ((t.Name.Equals(chk.Text))); };
                    DataStoreField selectedTerm = SelectedTerms.Find(p1);
                    checkedTerms.Add(selectedTerm);
                    if (SelectedTerms.IndexOf(selectedTerm) > 0)
                        SelectedTerms.Reverse(SelectedTerms.IndexOf(selectedTerm) - 1, 2);
                }
            }

            selectedTerms = SelectedTerms.Select(c => { c.IsChecked = false; return c; }).ToList();

            foreach (DataStoreField checkedterm in checkedTerms)
            {
                selectedTerms[selectedTerms.IndexOf(checkedterm)].IsChecked = true;
            }

            SelectedTerms = selectedTerms;

            BindListViewSelectedTerms();
        }

        protected void btnFirst_Click(object sender, EventArgs e)
        {
            int mIndex = 0;
            UpdateTermSize();
            List<DataStoreField> selectedTerms = (List<DataStoreField>)ViewState[SELECTED_TERMS];

            List<DataStoreField> checkedTerms = new List<DataStoreField>();

            foreach (ListViewItem lvi in lvSelectedTerms.Items)
            {
                TextBox txt = (TextBox)lvi.FindControl("txtAlias");

                CheckBox chk = (CheckBox)lvi.FindControl("chkTerm");
                Predicate<DataStoreField> p2 = delegate(DataStoreField t) { return ((t.Name.Equals(chk.Text))); };
                SelectedTerms.Find(p2).Alias = txt.Text;

                if (chk.Checked)
                {
                    Predicate<DataStoreField> p1 = delegate(DataStoreField t) { return ((t.Name.Equals(chk.Text))); };
                    DataStoreField selectedTerm = SelectedTerms.Find(p1);
                    checkedTerms.Add(selectedTerm);
                    if (SelectedTerms.IndexOf(selectedTerm) > 0)
                    {
                        SelectedTerms.Remove(selectedTerm);
                        SelectedTerms.Insert(mIndex, selectedTerm);
                        mIndex++;
                    }
                }
            }

            selectedTerms = SelectedTerms.Select(c => { c.IsChecked = false; return c; }).ToList();

            foreach (DataStoreField checkedterm in checkedTerms)
            {
                selectedTerms[selectedTerms.IndexOf(checkedterm)].IsChecked = true;
            }

            SelectedTerms = selectedTerms;

            BindListViewSelectedTerms();
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            Response.Redirect(string.Format("~/Admin/DataStoreDefinitionList.aspx?System={0}", base._itatSystem.ID));
        }

        protected void btnPromote_Click(object sender, EventArgs e)
        {
            UpdateTermSize();

            Page.Validate(VALIDATION_GROUP);

            string emailvalidationerror;

            bool aliasvalidation = CheckSelectedTermsHasAlias();

            bool emailvalidation = CheckErrorLogEmailList(out emailvalidationerror);

            string dupAliasError;
            bool duplicateAliasValidation = CheckAlias(out dupAliasError);

            Guid dataStoreDefinitionID = new Guid(Request.QueryString[Common.Names._QS_DEFINITION_ID].ToString());
            Dictionary<string /*TemplateName*/, Guid /*TemplateID*/> templateIDLookup = DataStoreConfig.GetActiveSearchOnlyTemplateListByName(_itatSystem.ID);

            Dictionary<Guid /*TemplateID*/, Dictionary<string /*TermName*/, Guid /*TermID*/>> termIDLookup = null;
            Dictionary<Guid /*TemplateID*/, Dictionary<Guid /*ComplexListID*/, Dictionary<string /*FieldName*/, Guid /*FieldID*/>>> fieldIDLookup = null;
            SystemStore.GetTermAndFieldIDLookup(_itatSystem.ID, ref termIDLookup, ref fieldIDLookup);

            bool pathValidation = CheckPath(txtPath.Text);
            if (Page.IsValid && aliasvalidation && emailvalidation && duplicateAliasValidation && pathValidation)
            {
                DataStoreDefinition dataStoreDef = new DataStoreDefinition(_itatSystem);
                dataStoreDef.SystemID = _itatSystem.ID;
                dataStoreDef.DataStoreDefinitionID = dataStoreDefinitionID;
                dataStoreDef.Active = radActiveYes.Checked ? true : false;
                dataStoreDef.Description = txtDescription.Text;
                dataStoreDef.DataStoreConfig.DefaultDateFormat = ddlDefaultDateFormat.SelectedValue.ToString();
                dataStoreDef.DataStoreConfig.LoadType = radTypeDelta.Checked ? LoadType.Delta : LoadType.Full;

                int deltadays;
                int.TryParse(txtDeltaDays.Text, out deltadays);

                dataStoreDef.DataStoreConfig.DeltaDays = deltadays;

                dataStoreDef.DataStoreConfig.SearchCriteria = GetSearchCriteria("MySearchControl");
                dataStoreDef.DataStoreConfig.PrimaryFacility = Extension.Concatenate<int>(dataStoreDef.DataStoreConfig.SearchCriteria.FacilityIds, PRIMARY_FACILITY_DELIMITER);
                dataStoreDef.DataStoreConfig.Path = txtPath.Text;

                foreach (ListItem li in chklstStatus.Items)
                {
                    if (li.Selected)
                    {
                        dataStoreDef.DataStoreConfig.SearchCriteria.Statuses.Add(li.Text);
                    }
                }

                foreach (ListItem li in lstSelectedTemplates.Items)
                {
                    dataStoreDef.DataStoreConfig.Templates.Add(new Guid(li.Value), li.Text);
                }

                foreach (DataStoreField dsf in SelectedTerms)
                {
                    dataStoreDef.DataStoreConfig.Terms.Add(dsf);
                }

                dataStoreDef.DataStoreConfig.ErrorLogRecepientEmail = txtEmail.Text;
                try
                {
                    DataStoreDefinition.UpdatedDataStoreDefinition(dataStoreDef, templateIDLookup, termIDLookup, fieldIDLookup);
                    Response.Redirect(string.Format("~/Admin/DataStoreDefinitionList.aspx?System={0}", dataStoreDef.SystemID));
                }
                catch (Exception ex)
                {
                    RegisterAlert(string.Format("UpdatedDataStoreDefinition call failed: {0}", ex.Message));
                }
            }
            else
            {
                string errorMsg = "";
                foreach (IValidator iv in Page.Validators)
                {
                    if (!iv.IsValid)
                    {
                        errorMsg = errorMsg + iv.ErrorMessage + "\\n";
                    }
                }

                if (!emailvalidation)
                {
                    errorMsg = errorMsg + emailvalidationerror + "\\n";
                }
                if (!pathValidation)
                {
                    errorMsg = errorMsg + "Path you have specified is not a valid Path." + "\\n";
                }
                if (!duplicateAliasValidation)
                {
                    errorMsg = errorMsg + dupAliasError;
                }
                if (errorMsg.Length > 0)
                    RegisterAlert(errorMsg);
            }
        }

        private bool CheckPath(string path)
        {
            return Directory.Exists(path);
        }

        protected void chklstStatus_DataBound(object sender, EventArgs e)
        {
            foreach (ListItem li in ((CheckBoxList)sender).Items)
            {
                li.Selected = true;
            }
        }

        protected void btnDown_Click(object sender, EventArgs e)
        {
            UpdateTermSize();
            List<DataStoreField> selectedTerms = SelectedTerms;
            List<DataStoreField> checkedTerms = new List<DataStoreField>();
            selectedTerms.Reverse();
            foreach (ListViewItem lvi in lvSelectedTerms.Items.OrderByDescending(Item => Item.DisplayIndex))
            {
                TextBox txt = (TextBox)lvi.FindControl("txtAlias");
                CheckBox chk = (CheckBox)lvi.FindControl("chkTerm");
                Predicate<DataStoreField> p1 = delegate(DataStoreField t) { return ((t.Name.Equals(chk.Text))); };
                selectedTerms.Find(p1).Alias = txt.Text;
                if (chk.Checked)
                {
                    DataStoreField selectedTerm = selectedTerms.Find(p1);
                    checkedTerms.Add(selectedTerm);
                    if (selectedTerms.IndexOf(selectedTerm) > 0)
                    {
                        selectedTerms.Reverse(selectedTerms.IndexOf(selectedTerm) - 1, 2);
                    }
                }
            }
            selectedTerms = selectedTerms.Select(c => { c.IsChecked = false; return c; }).ToList();

            foreach (DataStoreField checkedterm in checkedTerms)
            {
                selectedTerms[selectedTerms.IndexOf(checkedterm)].IsChecked = true;
            }

            selectedTerms.Reverse();
            SelectedTerms = selectedTerms;

            BindListViewSelectedTerms();
        }

        protected void btnLast_Click(object sender, EventArgs e)
        {
            int mIndex = 0;
            UpdateTermSize();
            List<DataStoreField> selectedTerms = SelectedTerms;
            List<DataStoreField> checkedTerms = new List<DataStoreField>();
            selectedTerms.Reverse();
            foreach (ListViewItem lvi in lvSelectedTerms.Items.OrderByDescending(Item => Item.DisplayIndex))
            {
                TextBox txt = (TextBox)lvi.FindControl("txtAlias");
                CheckBox chk = (CheckBox)lvi.FindControl("chkTerm");
                Predicate<DataStoreField> p1 = delegate(DataStoreField t) { return ((t.Name.Equals(chk.Text))); };
                selectedTerms.Find(p1).Alias = txt.Text;
                if (chk.Checked)
                {
                    DataStoreField selectedTerm = selectedTerms.Find(p1);
                    checkedTerms.Add(selectedTerm);
                    if (selectedTerms.IndexOf(selectedTerm) > 0)
                    {
                            selectedTerms.Remove(selectedTerm);
                            selectedTerms.Insert(mIndex, selectedTerm);
                            mIndex++;                  
                    }
                }
            }
            selectedTerms = selectedTerms.Select(c => { c.IsChecked = false; return c; }).ToList();

            foreach (DataStoreField checkedterm in checkedTerms)
            {
                selectedTerms[selectedTerms.IndexOf(checkedterm)].IsChecked = true;
            }

            selectedTerms.Reverse();
            SelectedTerms = selectedTerms;

            BindListViewSelectedTerms();
        }

        protected void lvSelectedTerms_ItemDataBound(object sender, ListViewItemEventArgs e)
        {
        }
        #endregion //events

        # region Base Override
        internal override Control ResizablePanel()
        {
            return null;
        }


        internal override HtmlGenericControl HTMLBody()
        {
            return this.htmlBody;
        }
        #endregion

        #region Private Methods

        private void LoadRadioButtonList(RadioButtonList rdlTerm, RadioButtonList listcontrol)
        {
            foreach (ListItem li in listcontrol.Items)
            {
                ListItem rli = new ListItem(li.Text, li.Value);
                rdlTerm.Items.Add(rli);
            }
        }

        private void LoadDropDownList(DropDownList ddlTerm1, DropDownList listcontrol)
        {
            foreach (ListItem li in listcontrol.Items)
            {
                ListItem dli = new ListItem(li.Text, li.Value);
                ddlTerm1.Items.Add(dli);
            }
        }

        private void LoadStatusList()
        {
            chklstStatus.DataSource = _itatsystem.Statuses;
            chklstStatus.DataTextField = STATUS_NAME;
            chklstStatus.DataBind();
        }

        private void LoadTemplateList(Guid systemID)
        {
            lstSelectedTemplates.Items.Clear();
            lstTerms.Items.Clear();
            lvSelectedTerms.Items.Clear();
            lstTemplates.Items.Clear();
            lstTemplates.Visible = true;
            lstTemplates.Enabled = true;
            using (DataSet ds = SystemStore.GetActiveSearchOnlySystemTemplateList(_itatSystem.ID))
            {
                var finalTemplateList = from t in ds.Tables[0].AsEnumerable()
                                        where Business.Template.FinalTemplateDefValid(t.Field<Guid>(Common.Names._CNTXT_TemplateId))
                                        select new { TemplateId = t.Field<Guid>(Common.Names._CNTXT_TemplateId), TemplateName = t.Field<string>(Common.Names._CNTXT_TemplateName) };
                lstTemplates.DataSource = finalTemplateList;
                lstTemplates.DataTextField = Common.Names._CNTXT_TemplateName;
                lstTemplates.DataValueField = Common.Names._CNTXT_TemplateId;
                lstTemplates.DataBind();
            }

        }
        private SortedList SortlistItems(ListBox list)
        {
            SortedList sortedList = new SortedList();

            foreach (ListItem li in list.Items)
                if (!sortedList.ContainsKey(li.Text))
                    sortedList.Add(li.Text, li.Value);

            list.DataSource = sortedList;
            list.DataValueField = LIST_VALUE;
            list.DataTextField = LIST_KEY;
            list.DataBind();

            return sortedList;
        }

        private void LoadTermList()
        {
            lstTerms.Items.Clear();
            lvSelectedTerms.Items.Clear();
            if (lstSelectedTemplates.Items.Count > 0)
            {
                List<Template> templates = new List<Template>();
                foreach (ListItem li in lstSelectedTemplates.Items)
                {
                    Predicate<Template> p = delegate(Template t) { return t.ID == new Guid(li.Value); };
                    Template foundTemplate = templates.Find(p);
                    if (foundTemplate == null)
                    {
                        Template template = new Business.Template(new Guid(li.Value), DefType.Final);
                        templates.Add(template);
                    }
                }

                //Get a unique collection of Terms from the list of Templates
                // listTerms = (List<DataStoreField>)ViewState[LIST_TERMS];
                listTerms = null;
                foreach (Template template in templates)
                {
                    foreach (Term templateTerm in template.BasicTerms)
                    {
                        //exclude External Terms from this list
                        if (templateTerm.IsStored)
                        {
                            Predicate<DataStoreField> p = delegate(DataStoreField t) { return ((t.Name.Equals(templateTerm.Name))); };
                            DataStoreField foundTerm = listTerms.Find(p);
                            if (foundTerm == null)
                            {
                                switch (templateTerm.TermType)
                                {
                                    case TermType.ComplexList:
                                        foreach (string termname in templateTerm.StoreColumns)
                                        {
                                            listTerms.Add(new DataStoreField(TermStore.GetTermConcatenatedName(template.Name, templateTerm.Name, termname), termname, templateTerm.RequiredSize, templateTerm.TermType, false));
                                        }
                                        break;

                                    case TermType.MSO:
                                        foreach (string termname in templateTerm.StoreColumns)
                                        {
                                            listTerms.Add(new DataStoreField(TermStore.GetTermConcatenatedName(template.Name, templateTerm.Name, termname), termname, templateTerm.RequiredSize, templateTerm.TermType, false));
                                        }
                                        break;

                                    case TermType.External:
                                        foreach (string termname in templateTerm.StoreColumns)
                                        {
                                            listTerms.Add(new DataStoreField(TermStore.GetTermConcatenatedName(template.Name, templateTerm.Name, termname), termname, templateTerm.RequiredSize, templateTerm.TermType, false));
                                        }
                                        break;

                                    case TermType.Text:
                                        listTerms.Add(new DataStoreField(TermStore.GetTermConcatenatedName(template.Name, templateTerm.Name, ""), templateTerm.Name, templateTerm.RequiredSize, templateTerm.TermType, false));
                                        break;

                                    default:
                                        listTerms.Add(new DataStoreField(TermStore.GetTermConcatenatedName(template.Name, templateTerm.Name, ""), templateTerm.Name, null, templateTerm.TermType, false));
                                        break;
                                }
                            }
                        }
                    }

                    foreach (Term complextListTerm in template.ComplexLists)
                    {
                        if (complextListTerm.IsStored)
                        {
                            Predicate<DataStoreField> p = delegate(DataStoreField t) { return ((t.Name.Equals(complextListTerm.Name))); };
                            DataStoreField foundTerm = listTerms.Find(p);
                            if (foundTerm == null)
                            {
                                switch (complextListTerm.TermType)
                                {
                                    case TermType.ComplexList:
                                        //DataStoreField foundTerm = listTerms.Find(p);
                                        foreach (ComplexListField field in ((ComplexList)complextListTerm).Fields)
                                        {
                                            if(field.FilterTerm.TermType == TermType.Text)
                                                listTerms.Add(new DataStoreField(TermStore.GetTermConcatenatedName(template.Name, complextListTerm.Name, field.Name), field.Name, complextListTerm.RequiredSize, field.FilterTerm.TermType,((TextTerm)field.FilterTerm).Format, false));
                                            else
                                            listTerms.Add(new DataStoreField(TermStore.GetTermConcatenatedName(template.Name, complextListTerm.Name, field.Name), field.Name, complextListTerm.RequiredSize, field.FilterTerm.TermType, false));
                                        }
                                        break;
                                }
                            }
                        }
                    }
                }

                IList<string> lstStandardColumn = Enum.GetNames(typeof(SystemStore.StandardColumn)).ToList();

                foreach (string standardColum in lstStandardColumn)
                {
                    listTerms.Add(new DataStoreField(standardColum, standardColum, null, TermType.None, true));
                }

                List<DataStoreField> selectedTerms = SelectedTerms;

                //This logic removes the selected terms that are no longer found in the template.
                selectedTerms = listTerms.Intersect(selectedTerms, new DataStoreFieldComparer()).OrderBy(dsf => selectedTerms.FindIndex(st => st.Name == dsf.Name)).ToList();
                List<DataStoreField> updatedSelectedTerms = SelectedTerms;
                updatedSelectedTerms = updatedSelectedTerms.Except(selectedTerms, new DataStoreFieldComparer()).ToList();

                //This message is no longer needed - the selected terms must be removed by the user whenm deleting the term from the template.
                //string erroneousterms = "";
                //if (updatedSelectedTerms.Count > 0)
                //{
                //    foreach (DataStoreField udsf in updatedSelectedTerms)
                //    {
                //        erroneousterms = erroneousterms + string.Format("- {0} \\n", udsf.Name);
                //    }
                //    RegisterAlert("Following Selected Terms are changed/removed from selected templates \\n" + erroneousterms);
                //}

                selectedTerms = selectedTerms.Select(dsf =>
                {
                    dsf.Length = SelectedTerms.Find(x => x.Name == dsf.Name).Length;
                    dsf.Alias = SelectedTerms.Find(x => x.Name == dsf.Name).Alias;
                    dsf.SortSequence = SelectedTerms.Find(x => x.Name == dsf.Name).SortSequence;
                    return dsf;
                }).ToList();

                //listTerms.RemoveAll(t => selectedTerms.Exists(x => x.Name.Equals(t.Name)));

                lstTerms.DataSource = listTerms;
                lstTerms.DataValueField = FIELD_NAME;
                lstTerms.DataTextField = FIELD_NAME;
                lstTerms.DataBind();

                lvSelectedTerms.DataSource = selectedTerms.OrderBy(d => d.SortSequence);
                lvSelectedTerms.DataBind();

                SelectedTerms = selectedTerms;
                ViewState[LIST_TERMS] = listTerms;
                SortlistItems(lstTerms);
            }
            else
            {
                IList<string> lstStandardColumn = Enum.GetNames(typeof(SystemStore.StandardColumn)).ToList();

                foreach (string standardColum in lstStandardColumn)
                {
                    listTerms.Add(new DataStoreField(standardColum, standardColum, null, TermType.None, true));
                }
                //listTerms = null;
                lstTerms.DataSource = listTerms;
                lstTerms.DataValueField = FIELD_NAME;
                lstTerms.DataTextField = FIELD_NAME;
                lstTerms.DataBind();
                lvSelectedTerms.DataSource = SelectedTerms.Intersect(listTerms, new DataStoreFieldComparer()).ToList();
                lvSelectedTerms.DataBind();
                ViewState[LIST_TERMS] = listTerms;
            }
        }

        private void UpdateTermSize()
        {
            foreach (ListViewItem lvi in lvSelectedTerms.Items)
            {
                TextBox txt = lvi.FindControl("txtSize") as TextBox;
                CheckBox chk = lvi.FindControl("chkTerm") as CheckBox;
                TextBox txtAlias = lvi.FindControl("txtAlias") as TextBox;
                Predicate<DataStoreField> p1 = delegate(DataStoreField t) { return ((t.Name.Equals(chk.Text))); };

                DataStoreField selterm = SelectedTerms.Find(p1);
                int length;
                if (selterm.RequiresTextLength)
                    if (int.TryParse(txt.Text, out length))
                        selterm.Length = length;
                    else
                        selterm.Length = null;
                selterm.Alias = txtAlias.Text;

            }
        }

        private void MoveTerm(List<DataStoreField> sTerms)
        {
            List<DataStoreField> listTerms = (List<DataStoreField>)ViewState[LIST_TERMS];
            if (sTerms.Count > 0)
            {
                foreach (DataStoreField selTerm in sTerms)
                {
                    lstTerms.Items.Remove(new ListItem(selTerm.Name));
                    listTerms.Remove(listTerms.Find(t => t.Name.Equals(selTerm.Name)));
                }
            }
            SelectedTerms = sTerms;
            ViewState[LIST_TERMS] = listTerms;


        }

        private void BindListViewSelectedTerms()
        {
            lvSelectedTerms.DataSource = SelectedTerms;
            lvSelectedTerms.DataBind();
        }

        #endregion

        #region Validations

        private bool CheckSelectedTermsHasAlias()
        {
            if (SelectedTerms != null)
            {
                foreach (DataStoreField dsf in SelectedTerms)
                {
                    if (dsf.Alias == null || dsf.Alias.Equals(string.Empty))
                    {
                        return false;
                    }

                }
            }
            return true;
        }

        private bool CheckAlias(out string errorMsg)
        {
            bool result = true;
            errorMsg = "";
            List<DataStoreField> multiAliasList = SelectedTerms.GroupBy(s => s.Alias + s.TemplateName).Where(g => g.Count() > 1).SelectMany(g => g).ToList();
            if (SelectedTerms != null)
            {
                if (multiAliasList.Count > 0)
                {
                    result = false;
                    errorMsg = "Following Terms has same Alias assigned in same Template, which is not allowed : \\n";
                    foreach (DataStoreField dsf in multiAliasList)
                    {
                        errorMsg = errorMsg + " - " + dsf.Name + " : " + dsf.Alias + "\\n";
                    }
                }
                else
                {
                    result = true;
                }
            }
            return result;
        }

        private bool CheckErrorLogEmailList(out string errormsg)
        {
            if (txtEmail.Text.Equals(string.Empty))
            {
                errormsg = "Enter ErrorLog Recipient Email Address";
                return false;
            }
            else
            {
                string[] emails = txtEmail.Text.Split(EMAIL_DELIMITER);
                foreach (string email in emails)
                {
                    if (!IsValidEmail(email.ToLower()))
                    {
                        errormsg = "Enter only Kindred email addresses for Error Log Recipient(s)";
                        return false;
                    }
                }
            }
            errormsg = "";
            return true;
        }

        public static bool IsValidEmail(string inputEmail)
        {
            string strRegex = @"^([a-zA-Z0-9_\-\.]+)@kindredhealthcare.com";
            Regex re = new Regex(strRegex);
            if (re.IsMatch(inputEmail))
                return (true);
            else
                return (false);
        }

        protected void CheckAtLeastOneTermIsSelected(object sender, ServerValidateEventArgs args)
        {
            if (lvSelectedTerms.Items.Count > 0)
                args.IsValid = true;
            else
                args.IsValid = false;
        }

        protected void CheckAtLeastOneTemplateIsSelected(object sender, ServerValidateEventArgs args)
        {
            if (lstSelectedTemplates.Items.Count > 0)
                args.IsValid = true;
            else
                args.IsValid = false;
        }

        protected void CheckAtLeastOneStatus(object sender, ServerValidateEventArgs args)
        {
            foreach (ListItem li in chklstStatus.Items)
            {
                if (li.Selected)
                {
                    args.IsValid = true;
                    break;
                }
                else

                    args.IsValid = false;
            }
        }

        public bool IsTextTerm(string termType)
        {
            return TermType.Text == (TermType)Enum.Parse(typeof(TermType), termType, true);
        }

        protected void CheckValidFileName(object sender, ServerValidateEventArgs args)
        {


            if (lblDefinitionName.Text.IndexOfAny(System.IO.Path.GetInvalidFileNameChars()) != -1)
            {
                args.IsValid = false;
            }
            else
                args.IsValid = false;



        }

        // move to TermSTore Business->Store
        public string GetTermConcatenatedName(string termName, string fieldName)
        {
            //if (termName.IndexOf("|") > -1)
            //    throw new Exception(string.Format("The term name cannot contain the delimiter character '{0}'", _TERM_NAME_DELIMITER));
            //if (fieldName.IndexOf("|") > -1)
            //    throw new Exception(string.Format("The term fieldName name cannot contain the delimiter character '{0}'", _TERM_NAME_DELIMITER));
            return string.Format("{0}{1}{2}", termName, "|", fieldName);
        }

        public string GetTermConcatenatedName(string templateName, string termName, string fieldName)
        {
            //if (termName.IndexOf("|") > -1)
            //    throw new Exception(string.Format("The term name cannot contain the delimiter character '{0}'", _TERM_NAME_DELIMITER));
            //if (fieldName.IndexOf("|") > -1)
            //    throw new Exception(string.Format("The term fieldName name cannot contain the delimiter character '{0}'", _TERM_NAME_DELIMITER));
            return string.Format("{0}{1}{2}", termName, "|", fieldName);
        }
        #endregion
    }
}