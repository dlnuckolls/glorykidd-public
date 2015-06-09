using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Web.UI.HtmlControls;
using System.Collections;
using System.Xml;
using Kindred.Knect.ITAT.Business;
using System.Text.RegularExpressions;
using System.Reflection;
using Kindred.Knect.ITAT.Web.Controls;
using System.Text;
using System.IO;

namespace Kindred.Knect.ITAT.Web.Admin
{
    public partial class AddDataStoreDefinition : DataStoreDefPage
    {


        #region Private Members

        private List<DataStoreField> _selectedTerms;
        private const string VSKEY_REPORT = "_vskey_Report";
        private const string VSKEY_EXTERNALTERMS = "_vskey_ExternalTerms";
        private const string VSKEY_SYSTEMTERM = "_vskey_SystemTerms";
        private const string LIST_TERMS = "_list_Terms";
        private const string FIELD_NAME = "Name";
        private const string SELECTED_TERMS = "_selectedTerms";
        private const string LIST_VALUE = "Value";
        private const string LIST_KEY = "Key";
        private const char EMAIL_DELIMITER = ',';
        private const string VSKEY_SYSTEMTERMCONTROL = "_vskey_SystemTermControl";
        private const string PRIMARY_FACILITY_DELIMITER = ",";
        protected Control searchControl;
        public Dictionary<string, Control> controlDictionary;
        #endregion


        #region Protected Members
        protected ITAT.Business.Template _template;
        private List<DataStoreField> _listTerms;

        protected List<DataStoreField> selectedTerms
        {
            get
            {
                if (_selectedTerms == null)
                    _selectedTerms = new List<DataStoreField>();
                return _selectedTerms;
            }

            set
            {
                _selectedTerms = value;
            }
        }
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



        #endregion

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

        #region Events

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            //RenderSystemTermControls();
        }

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



        protected void Page_Load(object sender, EventArgs e)
        {
            if (!AllowAccess(Common.Names._ASR_SystemAdmin))
            {
                UnauthorizedPageAccess();
            }

            if (!IsPostBack)
            {
                //RenderSystemTermControls();
                DisplayTerm1 = true;
                DisplayTerm2 = true;
                DisplayTerm3 = false;
                DisplayTerm4 = true;
                DisplayTerm5 = true;
                DisplayTerm6 = false;
                DisplayTerm7 = false;

                searchControl = LoadControl("~\\Controls\\SearchControl.ascx", base._itatSystem, DisplayTerm1, DisplayTerm2, DisplayTerm3, DisplayTerm4, DisplayTerm5, DisplayTerm6, DisplayTerm7);
                searchControl.ID = "MySearchControl";
                controlDictionary = new Dictionary<string, Control>();
                controlDictionary.Add(searchControl.ID, searchControl);
                apnlCriteria.Controls.Add(searchControl);


                lblSystemName.Text = _itatSystem.Name;
                LoadTemplateList();
                LoadStatusList();
                ddlDefaultDateFormat.DataSource = DataStoreDefinition.GetDateFormatList();
                ddlDefaultDateFormat.DataBind();

                btnRemoveTerm.OnClientClick = string.Format("return confirm('All the selected Terms will be removed from the list.');");

                btnRemoveTemplate.OnClientClick = string.Format("return confirm('All the selected Terms from the selected Templates also removed by removing Templates.');");

            }
            else
            {

                selectedTerms = SelectedTerms;
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

            SaveViewState();
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

            SaveViewState();
        }

        protected void btnAddTerm_Click(object sender, EventArgs e)
        {
            UpdateTermSize();
            selectedTerms = SelectedTerms;
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
            selectedTerms = SelectedTerms;
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

        protected void lvSelectedTerms_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        protected void btnUp_Click(object sender, EventArgs e)
        {
            UpdateTermSize();
            selectedTerms = (List<DataStoreField>)ViewState[SELECTED_TERMS];

            List<DataStoreField> checkedTerms = new List<DataStoreField>();

            foreach (ListViewItem lvi in lvSelectedTerms.Items)
            {
                TextBox txt = (TextBox)lvi.FindControl("txtAlias");


                CheckBox chk = (CheckBox)lvi.FindControl("chkTerm");
                Predicate<DataStoreField> p2 = delegate(DataStoreField t) { return ((t.Name.Equals(chk.Text))); };
                selectedTerms.Find(p2).Alias = txt.Text;

                if (chk.Checked)
                {
                    Predicate<DataStoreField> p1 = delegate(DataStoreField t) { return ((t.Name.Equals(chk.Text))); };
                    DataStoreField selectedTerm = selectedTerms.Find(p1);
                    int selectedTermIndex = selectedTerms.IndexOf(selectedTerm);
                    checkedTerms.Add(selectedTerm);
                    if (selectedTermIndex > 0)
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
            SelectedTerms = selectedTerms;



            BindListViewSelectedTerms();


        }


        protected void btnFirst_Click(object sender, EventArgs e)
        {
            UpdateTermSize();
            selectedTerms = (List<DataStoreField>)ViewState[SELECTED_TERMS];
            int mIndex = 0;
            List<DataStoreField> checkedTerms = new List<DataStoreField>();

            foreach (ListViewItem lvi in lvSelectedTerms.Items)
            {
                TextBox txt = (TextBox)lvi.FindControl("txtAlias");


                CheckBox chk = (CheckBox)lvi.FindControl("chkTerm");
                Predicate<DataStoreField> p2 = delegate(DataStoreField t) { return ((t.Name.Equals(chk.Text))); };
                selectedTerms.Find(p2).Alias = txt.Text;

                if (chk.Checked)
                {
                    Predicate<DataStoreField> p1 = delegate(DataStoreField t) { return ((t.Name.Equals(chk.Text))); };
                    DataStoreField selectedTerm = selectedTerms.Find(p1);
                    int selectedTermIndex = selectedTerms.IndexOf(selectedTerm);
                    checkedTerms.Add(selectedTerm);
                    if (selectedTermIndex > 0)
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
            SelectedTerms = selectedTerms;



            BindListViewSelectedTerms();


        }

        protected void btnDown_Click(object sender, EventArgs e)
        {
            UpdateTermSize();
            selectedTerms = SelectedTerms;
            selectedTerms.Reverse();

            List<DataStoreField> checkedTerms = new List<DataStoreField>();

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
            selectedTerms = SelectedTerms;
            selectedTerms.Reverse();

            List<DataStoreField> checkedTerms = new List<DataStoreField>();

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


        protected void chklstStatus_DataBound(object sender, EventArgs e)
        {
            foreach (ListItem li in ((CheckBoxList)sender).Items)
            {
                li.Selected = true;
            }
        }
        protected void btnCancel_Click(object sender, EventArgs e)
        {
            Response.Redirect(string.Format("~/Admin/DataStoreDefinitionList.aspx?System={0}", _itatSystem.ID));
        }


        protected void btnPromote_Click(object sender, EventArgs e)
        {
            UpdateTermSize();

            Page.Validate("RequiredValidation");

            string emailvalidationerror;
            bool aliasvalidation = CheckSelectedTermsHasAlias();

            bool emailvalidation = CheckErrorLogEmailList(out emailvalidationerror);
            DataStoreDefinition dataStoreDef = new DataStoreDefinition(_itatSystem);

            string dupAliasError;
            bool duplicateAliasValidation = CheckAlias(out dupAliasError);

            Dictionary<string /*TemplateName*/, Guid /*TemplateID*/> templateIDLookup = DataStoreConfig.GetActiveSearchOnlyTemplateListByName(_itatSystem.ID);
            Dictionary<Guid /*TemplateID*/, Dictionary<string /*TermName*/, Guid /*TermID*/>> termIDLookup = null;
            Dictionary<Guid /*TemplateID*/, Dictionary<Guid /*ComplexListID*/, Dictionary<string /*FieldName*/, Guid /*FieldID*/>>> fieldIDLookup = null;
            SystemStore.GetTermAndFieldIDLookup(_itatSystem.ID, ref termIDLookup, ref fieldIDLookup);

            bool pathValidation = CheckPath(txtPath.Text);
            if (Page.IsValid && aliasvalidation && emailvalidation && duplicateAliasValidation && pathValidation)
            {
                dataStoreDef.SystemID = _itatSystem.ID;
                dataStoreDef.Active = radActiveYes.Checked ? true : false;
                dataStoreDef.Name = txtDefinitionName.Text;
                dataStoreDef.Description = txtDescription.Text;

                dataStoreDef.DataStoreConfig.LoadType = radTypeDelta.Checked ? LoadType.Delta : LoadType.Full;
                dataStoreDef.DataStoreConfig.DefaultDateFormat = ddlDefaultDateFormat.SelectedValue.ToString();

                int days;
                if (dataStoreDef.DataStoreConfig.LoadType == LoadType.Delta)
                {
                    if (!txtDeltaDays.Text.Equals(string.Empty))
                        if (int.TryParse(txtDeltaDays.Text, out days))
                            dataStoreDef.DataStoreConfig.DeltaDays = days;
                        else
                        {
                            RegisterAlert("Enter numeric value for Delta days.");
                        }
                    else
                    {
                        RegisterAlert("Enter Delta days.");
                    }
                }

                dataStoreDef.DataStoreConfig.SearchCriteria = this.GetSearchCriteria("MySearchControl");
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

                foreach (DataStoreField dsf in selectedTerms)
                {
                    dataStoreDef.DataStoreConfig.Terms.Add(dsf);
                }

                dataStoreDef.DataStoreConfig.ErrorLogRecepientEmail = txtEmail.Text;
                try
                {
                    if (!DataStoreDefinition.CheckDataStoreDefinitionExists(dataStoreDef.Name))
                    {
                        try
                        {
                            dataStoreDef.DataStoreDefinitionID = DataStoreDefinition.AddDataStoreDefinition(dataStoreDef, templateIDLookup, termIDLookup, fieldIDLookup);
                            Response.Redirect(string.Format("~/Admin/DataStoreDefinitionList.aspx?System={0}", dataStoreDef.SystemID));
                        }
                        catch (Exception ex)
                        {
                            RegisterAlert(string.Format("Error when creating DataStoreDefinition: {0}", ex.Message));
                        }
                    }
                    RegisterAlert(string.Format("DataStore Definition with Name : {0}, already exists in the System.", dataStoreDef.Name));
                }
                catch (Exception ex)
                {
                    RegisterAlert(string.Format("Unhandled exception: {0}", ex.Message));
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

                if (!aliasvalidation)
                {
                    errorMsg = errorMsg + "All Selected Terms must have Alias." + "\\n";
                }
                if (!pathValidation)
                {
                    errorMsg = errorMsg + "Path you have specified is not a valid Path." + "\\n";
                }
                if (!emailvalidation)
                {
                    errorMsg = errorMsg + emailvalidationerror + "\\n";
                }
                if (!duplicateAliasValidation)
                {
                    errorMsg = errorMsg + dupAliasError;
                }
                if (!errorMsg.Equals(string.Empty))
                    RegisterAlert(errorMsg);
            }
        }

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


        protected void lvSelectedTerms_ItemDataBound(object sender, ListViewItemEventArgs e)
        {
            if (e.Item.ItemType == ListViewItemType.DataItem)
            {
                //Not Required as 2011 Enhancement
                //DropDownList ddlTermType = e.Item.FindControl("ddlTermType") as DropDownList;
                //DropDownList ddlDateFormat = e.Item.FindControl("ddlDateFormat") as DropDownList;
                Label lblType = e.Item.FindControl("lblType") as Label;
                TextBox txtAlias = e.Item.FindControl("txtAlias") as TextBox;
                TextBox txtSize = e.Item.FindControl("txtSize") as TextBox;
                //if (selectedTerms[((ListViewDataItem)(e.Item)).DataItemIndex].IsComplexList)
                //{
                //    ddlTermType.DataSource = Enum.GetNames(typeof(DataStoreField.AssignedTermType));
                //    ddlTermType.DataBind();

                //    ddlDateFormat.DataSource = DataStoreDefinition.GetDateFormatList();
                //    ddlDateFormat.DataBind();
                //}
                ////else
                ////{
                    lblType.Visible = true;
                    lblType.Text = selectedTerms[((ListViewDataItem)(e.Item)).DataItemIndex].TermType.ToString();
                    //ddlTermType.Visible = false;
                    //ddlDateFormat.Visible = false;
                //}

                txtAlias.Text = selectedTerms[((ListViewDataItem)(e.Item)).DataItemIndex].Alias;
                if (selectedTerms[((ListViewDataItem)(e.Item)).DataItemIndex].RequiresTextLength)
                {
                    txtSize.Visible = true;
                    txtSize.Text = selectedTerms[((ListViewDataItem)(e.Item)).DataItemIndex].Length.ToString();
                }
                else
                {
                    txtSize.Visible = false;
                }


            }
        }
        #endregion

        #region Private Methods




        private void LoadStatusList()
        {
            chklstStatus.DataSource = _itatSystem.Statuses;
            chklstStatus.DataTextField = FIELD_NAME;
            chklstStatus.DataBind();
        }

        private void LoadTemplateList()
        {

            const string TEMPLATE_NAME = "TemplateName";
            const string TEMPLATE_ID = "TemplateID";

            lstSelectedTemplates.Items.Clear();

            lstTerms.Items.Clear();

            lvSelectedTerms.Items.Clear();

            lstTemplates.Items.Clear();

            lstTemplates.Visible = true;
            lstTemplates.Enabled = true;

            using (DataSet ds = SystemStore.GetActiveSearchOnlySystemTemplateList(_itatSystem.ID))
            {
                var finalTemplateList = from t in ds.Tables[0].AsEnumerable()
                                        where Business.Template.FinalTemplateDefValid(t.Field<Guid>(TEMPLATE_ID))
                                        select new { TemplateId = t.Field<Guid>(TEMPLATE_ID), TemplateName = t.Field<string>(TEMPLATE_NAME) };
                lstTemplates.DataSource = finalTemplateList;
                lstTemplates.DataTextField = TEMPLATE_NAME;
                lstTemplates.DataValueField = TEMPLATE_ID;
                lstTemplates.DataBind();
            }
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
                //listTerms = (List<DataStoreField>)ViewState[LIST_TERMS];
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
                                if (complextListTerm.TermType == TermType.ComplexList)
                                {

                                    //DataStoreField foundTerm = listTerms.Find(p);
                                    foreach (ComplexListField field in ((ComplexList)complextListTerm).Fields)
                                    {
                                        if(field.FilterTerm.TermType == TermType.Text)
                                            listTerms.Add(new DataStoreField(TermStore.GetTermConcatenatedName(template.Name, complextListTerm.Name, field.Name), field.Name, complextListTerm.RequiredSize, field.FilterTerm.TermType,((TextTerm)field.FilterTerm).Format, false));
                                        else
                                        listTerms.Add(new DataStoreField(TermStore.GetTermConcatenatedName(template.Name, complextListTerm.Name, field.Name), field.Name, complextListTerm.RequiredSize, field.FilterTerm.TermType, false));
                                    }
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
                selectedTerms = SelectedTerms;

                selectedTerms.RemoveAll(t => !listTerms.Exists(x => x.Name.Equals(t.Name)));

                listTerms.RemoveAll(t => selectedTerms.Exists(x => x.Name.Equals(t.Name)));

                lstTerms.DataSource = listTerms;
                lstTerms.DataValueField = FIELD_NAME;
                lstTerms.DataTextField = FIELD_NAME;
                lstTerms.DataBind();


                lvSelectedTerms.DataSource = selectedTerms;
                lvSelectedTerms.DataBind();

                SelectedTerms = selectedTerms;
                ViewState[LIST_TERMS] = listTerms;


                SortlistItems(lstTerms);

            }
            else
            {
                selectedTerms = null;
                listTerms = null;
                lstTerms.DataSource = listTerms;
                lstTerms.DataValueField = FIELD_NAME;
                lstTerms.DataTextField = FIELD_NAME;
                lstTerms.DataBind();
                lvSelectedTerms.DataSource = selectedTerms;
                lvSelectedTerms.DataBind();
                SelectedTerms = selectedTerms;
                ViewState[LIST_TERMS] = listTerms;
            }


        }










        private SortedList SortlistItems(ListBox list)
        {

            SortedList sl = new SortedList();

            foreach (ListItem li in list.Items)
                if (!sl.ContainsKey(li.Text))
                    sl.Add(li.Text, li.Value);

            list.DataSource = sl;
            list.DataValueField = LIST_VALUE;
            list.DataTextField = LIST_KEY;
            list.DataBind();

            return sl;
        }

        private void BindListViewSelectedTerms()
        {
            lvSelectedTerms.DataSource = SelectedTerms;
            lvSelectedTerms.DataBind();
        }

        protected void UpdateTermSize()
        {



            foreach (ListViewItem lvi in lvSelectedTerms.Items)
            {
                TextBox txt = lvi.FindControl("txtSize") as TextBox;
                CheckBox chk = lvi.FindControl("chkTerm") as CheckBox;
                TextBox txtAlias = lvi.FindControl("txtAlias") as TextBox;
                //Not Required as 2011 Enhancement
                //DropDownList ddlTermType = lvi.FindControl("ddlTermType") as DropDownList;
                //DropDownList ddlDateFormat = lvi.FindControl("ddlDateFormat") as DropDownList;
                Predicate<DataStoreField> p1 = delegate(DataStoreField t) { return ((t.Name.Equals(chk.Text))); };

                DataStoreField selterm = SelectedTerms.Find(p1);
                int length;
                if (selterm.RequiresTextLength)
                    if (int.TryParse(txt.Text, out length))
                        selterm.Length = length;
                    else
                        selterm.Length = null;
                //Not Required as 2011 Enhancement
                //if (selterm.IsComplexList)
                //{
                //    selterm.TermTypeAssigned = (Kindred.Knect.ITAT.Business.DataStoreField.AssignedTermType)Enum.Parse(typeof(Kindred.Knect.ITAT.Business.DataStoreField.AssignedTermType), ddlTermType.SelectedValue);
                //    if (ddlDateFormat.Enabled)
                //        selterm.DateFormat = ddlDateFormat.SelectedValue;
                //}
                selterm.Alias = txtAlias.Text;

            }




        }

        public List<DataStoreField> SelectedTerms { get { return (List<DataStoreField>)ViewState[SELECTED_TERMS]; } set { ViewState[SELECTED_TERMS] = value; } }
        protected bool CheckPath(string path)
        {
            return Directory.Exists(path);
        }
        protected void CheckAtLeastOneTermIsSelected(object sender, ServerValidateEventArgs args)
        {
            if (lvSelectedTerms.Items.Count > 0)
                args.IsValid = true;
            else
                args.IsValid = false;
        }

        private bool CheckAlias(out string errorMsg)
        {
            bool result = true;
            errorMsg = "";
            if (SelectedTerms != null)
            {
                List<DataStoreField> multiAliasList = SelectedTerms.GroupBy(s => s.Alias + s.TemplateName).Where(g => g.Count() > 1).SelectMany(g => g).ToList();
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
        protected void ChecNumericPeriodValue(object sender, ServerValidateEventArgs args)
        {
            //int period;
            //if (txtNoOfDays.Text.Length > 0)
            //{
            //    if (int.TryParse(txtNoOfDays.Text, out period))
            //        args.IsValid = true;
            //    else
            //        args.IsValid = false;

            //}

        }

        protected void CheckValidFileName(object sender, ServerValidateEventArgs args)
        {


            if (txtDefinitionName.Text.IndexOfAny(System.IO.Path.GetInvalidFileNameChars()) != -1)
            {
                args.IsValid = false;
            }
            else
                args.IsValid = true;



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


        protected override object SaveViewState()
        {
            //ViewState[VSKEY_REPORT] = _report;
            //ViewState[VSKEY_EXTERNALTERMS] = _externalTerms;
            // ViewState[VSKEY_SYSTEMTERM] = _systemTermControls;
            return base.SaveViewState();
        }

        protected override void LoadViewState(object savedState)
        {
            base.LoadViewState(savedState);
            //_systemTermControls = (Dictionary<string, WebControl>)ViewState[VSKEY_EXTERNALTERMS];
        }

        #endregion


    }
    public class DuplicateAliasComparer : IEqualityComparer<DataStoreField>
    {


        public bool Equals(DataStoreField x, DataStoreField y)
        {
            return x.TemplateName == y.TemplateName && x.Alias == y.Alias;
        }


        public int GetHashCode(DataStoreField obj)
        {
            return obj.Alias.GetHashCode();
        }
    }

    public static class Extension
    {
        public static string Concatenate<T>(this IEnumerable<T> source, string delimiter)
        {
            var s = new StringBuilder();
            bool first = true;
            foreach (T t in source)
            {
                if (first)
                {
                    first = false;
                }
                else
                {
                    s.Append(delimiter);
                }
                s.Append(t);
            }
            return s.ToString();
        }
    }
}