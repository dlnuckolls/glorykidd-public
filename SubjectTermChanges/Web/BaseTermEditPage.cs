using System;
using System.Collections.Generic;
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
	public abstract class BaseTermEditPage : BaseTemplatePage
	{

        public enum TermHandler
        {
            BasicTerm,
            ComplexList,
            ComplexListField,
            ComplexListItem
        }

        [Serializable]
        public struct TermEdit
        {
            public Term Term;
            public EditMode EditMode;
            public int TermIndex;
            public ComplexListField ComplexListField;
            public int ComplexListFieldIndex;
            public int ComplexListItemIndex;
            public TermHandler TermHandler;
            public string OldFieldName;
            public string NewFieldName;
        }

		#region virtual/abstract methods

		protected abstract List<string> ValidateForm();
		protected abstract void InitializeForm();
		protected abstract void LoadValues();
		protected abstract void UpdateValues();
		protected abstract void ShowHideFields();
		protected abstract TextBox TermNameControl();
        #endregion

        #region constants
        private const int _MAX_HEADER_TERM_COUNT = 4;
        private const string _KH_VS_TERM_EDIT = "_kh_vs_TermEdit";
        private const string _MAPPING_SEPARATOR = "|";
        protected const string _DBFIELD_TERM_TEXT = "Text";
        protected const string _DBFIELD_TERM_VALUE = "Value";
        protected const string _MAPPING_BUTTON_MAP = "Map";
        protected const string _MAPPING_BUTTON_UNMAP = "UnMap";
        #endregion

        #region private members
        private TermEdit _termEdit;
        #endregion

        #region protected members
        protected DataSet _systemDBFieldTerms;
        protected string _mappedTemplateTermValue = string.Empty;
        protected int? _ddlMapIndex = null;
        #endregion

        #region Properties

        public int TermIndex
        {
            get 
            {
                if (IsComplexListField)
                    throw new Exception("Term Index is not supported for a Term Filter.");
                return _termEdit.TermIndex; 
            }
        }
        
        public bool IsComplexListField
        {
            get { return CheckComplexListField(_termEdit); }
        }

        public static bool CheckComplexListField(TermEdit termEdit)
        {
            return termEdit.TermHandler == TermHandler.ComplexListField;
        }

        protected Term Term
        {
            get
            {
                if (IsComplexListField)
                    return _termEdit.ComplexListField.FilterTerm;
                else
                    return _termEdit.Term;
            }
        }

        protected string TermName
        {
            get
            {
                if (IsComplexListField)
                    return _termEdit.ComplexListField.Name;
                else
                    return _termEdit.Term.Name;
            }
            set
            {
                if (IsComplexListField)
                    _termEdit.ComplexListField.Name = value;
                else
                    _termEdit.Term.Name = value;
            }
        }

        protected bool ShowTermGroups
        {
            get
            {
                return !IsComplexListField && _template.SecurityModel == Business.SecurityModel.Advanced;
            }
        }

        protected string PageTitle
        {
            get
            {
                return IsComplexListField ? "Edit Field" : "Edit Term";
            }
        }

        protected EditMode EditMode
        {
            get { return _termEdit.EditMode; }
        }
        
        protected TermEdit termEdit
        {
            get { return _termEdit; }
        }

        #endregion

		#region overrides

		protected override TemplateHeader HeaderControl()
		{
			return null;
		}

		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);
            if (!IsPostBack)
            {
                GetContextData();
                GetSystemTermsList();
                InitializeForm();
                LoadValues();
                SetTermGroupInViewState();
            }
        }

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);
            RegisterBTEPConfirmationScriptBlocks();
        }

		#endregion

		#region  Term-specific methods

        private void GetContextData()
        {
            _termEdit = (TermEdit)Context.Items[Common.Names._CNTXT_TermEdit];

            switch (_termEdit.TermHandler)
            {
                case TermHandler.BasicTerm:
                    _termEdit.Term = _template.BasicTerms[_termEdit.TermIndex];
                    break;

                case TermHandler.ComplexList:
                    _termEdit.Term = _template.ComplexLists[_termEdit.TermIndex];
                    break;

                case TermHandler.ComplexListField:
                    ComplexList complexList = _template.ComplexLists[_termEdit.TermIndex] as ComplexList;
                    _termEdit.ComplexListField = complexList.Fields[_termEdit.ComplexListFieldIndex];
                    _termEdit.OldFieldName = _termEdit.ComplexListField.Name;
                    _termEdit.Term = _termEdit.ComplexListField.FilterTerm;
                    _termEdit.Term.BigText = _termEdit.ComplexListField.BigText ?? false;
                    _termEdit.Term.Summary = _termEdit.ComplexListField.Summary ?? false;
                    break;

                case TermHandler.ComplexListItem:
                    break;
            }

            if (_termEdit.Term == null)
                throw new Exception("The term was not found in this template.");
        }

        protected string GetMappedTermValue(int index, string mappedTermName)
        {
            return string.Format("{0:D}{1}{2}", index, _MAPPING_SEPARATOR, mappedTermName);
        }

        protected string GetMappedTermName(string mappedTermValue)
        {
            try
            {
                return mappedTermValue.Split(new char[_MAPPING_SEPARATOR[0]])[1];
            }
            catch
            {
                return string.Empty;
            }
        }

        private void GetSystemTermsList()
        {
            DataTable dtSystemDBFieldTerms = new DataTable();
            dtSystemDBFieldTerms.Columns.Add(_DBFIELD_TERM_TEXT);
            dtSystemDBFieldTerms.Columns.Add(_DBFIELD_TERM_VALUE);
            List<Term> systemDBFieldTerms = _itatSystem.Terms.FindAll(t => (t.UseDBField ?? false) && t.TermType.Equals(Term.TermType));
            int index = 0;
            foreach (Term systemTerm in systemDBFieldTerms)
            {
                index++;    //This is used to uniquely identify each value in the ddl ... start at '1' since index '0' is meant for the 'Select One' text.
                string mappedTermName = string.Empty;
                Term mappedTerm = _template.BasicTerms.Find(t => (t.UseDBField ?? false) && t.DBFieldName.Equals(systemTerm.DBFieldName));
                if (mappedTerm != null)
                {
                    mappedTermName = mappedTerm.Name;
                    if ((Term.UseDBField ?? false) && Term.DBFieldName.Equals(systemTerm.DBFieldName))
                        _mappedTemplateTermValue = GetMappedTermValue(index, mappedTermName);
                }
                DataRow drSystemDBFieldTerm = dtSystemDBFieldTerms.NewRow();
                drSystemDBFieldTerm[_DBFIELD_TERM_TEXT] = systemTerm.Name;
                drSystemDBFieldTerm[_DBFIELD_TERM_VALUE] = GetMappedTermValue(index, mappedTermName); 
                dtSystemDBFieldTerms.Rows.Add(drSystemDBFieldTerm);
            }
            _systemDBFieldTerms = new DataSet();
            _systemDBFieldTerms.Tables.Add(dtSystemDBFieldTerms);
        }

        protected void RegisterBTEPConfirmationScriptBlocks()
        {
            Type t = this.GetType();
            string scriptName = string.Empty;

            scriptName = "_khb_ConfirmationScriptBlock";
            System.IO.StringWriter sw = new System.IO.StringWriter();
            sw.WriteLine("	function _khb_onMap(ddl, termName)  ");
            sw.WriteLine("	{");
            sw.WriteLine("		var p = document.getElementById(ddl);");
            sw.WriteLine("		if (p)");
            sw.WriteLine("		{");
            sw.WriteLine("		    if (p.disabled)");
            sw.WriteLine("		    {");
            sw.WriteLine("		        p.options[p.selectedIndex].value = '';");
            sw.WriteLine("		        return true;");
            sw.WriteLine("		    }");
            sw.WriteLine("		    if (p.selectedIndex == 0)");
            sw.WriteLine("		    {");
            sw.WriteLine("		        alert('Please select a term');");
            sw.WriteLine("		        return false;");
            sw.WriteLine("		    }");

            sw.WriteLine("		    var selectedTermNameArray = p.options[p.selectedIndex].value.split('{0}');", _MAPPING_SEPARATOR);
            sw.WriteLine("		    var selectedTermName = '';");
            sw.WriteLine("		    if (selectedTermNameArray.length > 1)");
            sw.WriteLine("		        selectedTermName = selectedTermNameArray[1];");

            sw.WriteLine("		    if (selectedTermName.length > 0)");
            sw.WriteLine("		    {");
            sw.WriteLine("		        if (selectedTermName == termName)");
            sw.WriteLine("		        {");
            sw.WriteLine("		            alert('Please select a different term');");
            sw.WriteLine("		            return false;");
            sw.WriteLine("		        }");
            sw.WriteLine("		        var selectedSystemTermName = p.options[p.selectedIndex].text;");
            sw.WriteLine("		        return confirm('Confirm that you want to unmap template term \"' + selectedTermName + '\" from system term \"' + selectedSystemTermName + '\" ?');");
            sw.WriteLine("		    }");
            sw.WriteLine("		    return true;");
            sw.WriteLine("		}");
            sw.WriteLine("		return false;");
            sw.WriteLine("	}");
            if (!ClientScript.IsClientScriptBlockRegistered(t, scriptName))
                ClientScript.RegisterClientScriptBlock(t, scriptName, sw.ToString(), true);
        }

        protected void BaseTransfer(EditMode? editMode, TermHandler termHandler, string destination, bool isChanged)
        {
            _termEdit.TermHandler = termHandler;
            Context.Items[Common.Names._CNTXT_Template] = _template;
            Context.Items[Common.Names._CNTXT_IsChanged] = isChanged;
            if (editMode.HasValue)
                _termEdit.EditMode = editMode.Value;
            Context.Items[Common.Names._CNTXT_TermEdit] = _termEdit;

            Server.Transfer(destination);
        }

        protected bool DisplayTermMapping()
        {
            bool isRetroAdmin = SecurityHelper.CanPerformFunction(_itatSystem.AllowedRoles(Business.XMLNames._AF_RetroAdmin));

            return (!IsComplexListField && 
                isRetroAdmin &&
                _template.RetroModel != Retro.RetroModel.Off &&
                _systemDBFieldTerms != null &&
                _systemDBFieldTerms.Tables.Count > 0 &&
                _systemDBFieldTerms.Tables[0].Rows.Count > 0);
        }

        protected void MapTerm(string systemTermName, string droppedTermName, int? ddlMapIndex)
        {
            if (string.IsNullOrEmpty(systemTermName))
            {
                if (!ddlMapIndex.HasValue)
                    throw new Exception(string.Format("Tasked with unmapping term '{0}' but no ddl index was provided", Term.Name));
                if (ddlMapIndex.Value <= 0)
                    throw new Exception(string.Format("Tasked with unmapping term '{0}' but an invalid ddl index ({1:D}) was provided", Term.Name, ddlMapIndex.Value));
                Term.SystemTerm = false;
                Term.UseDBField = null;
                Term.DBFieldName = null;
                _ddlMapIndex = ddlMapIndex;
            }
            else
            {
                Term systemTerm = _itatSystem.Terms.Find(t => t.Name == systemTermName);
                if (systemTerm == null)
                    throw new Exception(string.Format("Unable to load system term {0}", systemTermName));

                Term droppedTerm = null;
                if (!string.IsNullOrEmpty(droppedTermName))
                {
                    droppedTerm = _itatSystem.Terms.Find(t => t.Name == droppedTermName);
                    if (droppedTerm == null)
                        throw new Exception(string.Format("Unable to load template term {0}", droppedTermName));
                }

                Term.SystemTerm = true;
                Term.UseDBField = true;
                Term.DBFieldName = systemTerm.DBFieldName;

                if (droppedTerm != null)
                {
                    droppedTerm.SystemTerm = false;
                    droppedTerm.UseDBField = null;
                    droppedTerm.DBFieldName = null;
                }
            }
            IsChanged = true;
        }

        protected void SetContextDataAndReturn(string termName, bool updateValues, bool isHeader)
        {
            string newFieldName = termName.Trim();

            // Larry Richardson 4/25/2008 
            if (!Page.IsValid && updateValues)
            {
                foreach (BaseValidator validator in Page.Validators)
                {
                    if (!validator.IsValid)
                        RegisterAlert(validator.ErrorMessage);
                }
            }

            if (updateValues)
            {
                switch (_termEdit.TermHandler)
                {
                    case TermHandler.BasicTerm:
                        //If the term is renamed, check for references
                        if (Term != null && termName != TermName)
                            if (!Business.Term.ValidName(termName))
                                RegisterAlert(string.Format("You have attempted to rename the \"{0}\" term, but \"{1}\" is not a valid term name.", TermName, termName));

                        if (_template.TermNameInUse(termName, _termEdit.Term.ID))
                            RegisterAlert(string.Format("The term name {0} is already being used.  You must select a different term name.", termName));

                        if (isHeader && _template.SecurityModel == SecurityModel.Advanced)
                        {
                            int headerTermCount = 0;
                            foreach (Term term in _template.BasicTerms)
                            {
                                if (term.IsHeader)
                                    headerTermCount++;
                            }
                            if (headerTermCount > _MAX_HEADER_TERM_COUNT - 1)
                            {
                                RegisterAlert(string.Format("Maximum number of header fields ({0:D}) has been reached.  In order to add this term to the header, you will first need to de-select another term.", _MAX_HEADER_TERM_COUNT));
                            }
                        }

                        char cFirst = '\"';
                        if (_template.TermName_FirstChar_InUse(termName, cFirst, _termEdit.Term.ID))
                            RegisterAlert(string.Format("The term name (up to the first character - ' {0} ')  '{1}' is already being used.   You must select a different term name.", cFirst, Term.NameAtFirstChar(_termEdit.Term.Name, cFirst)));
                        break;

                    case TermHandler.ComplexList:
                        //If the term is renamed, check for references
                        if (Term != null && termName != TermName)
                            if (!Business.Term.ValidName(termName))
                                RegisterAlert(string.Format("You have attempted to rename the \"{0}\" term, but \"{1}\" is not a valid term name.", TermName, termName));

                        if (_template.ComplexListTermNameInUse(termName, Term.ID))
                            RegisterAlert(string.Format("The term name {0} is already being used in the Complex List.   You must select a different term name.", termName));
                        break;

                    case TermHandler.ComplexListField:
                        if ((_template.ComplexLists[_termEdit.TermIndex] as ComplexList).FieldNameInUse(newFieldName, _termEdit.ComplexListField.ID))
                            RegisterAlert(string.Format("The field name {0} is already being used.  You must select a different field name.", TermName));
                        else
                        {
                            _termEdit.NewFieldName = newFieldName;
                        }
                        break;

                    case TermHandler.ComplexListItem:
                        throw new Exception(string.Format("TermHandler '{0}' not handled", _termEdit.TermHandler.ToString()));
                }
            }
            else
            {
                switch (_termEdit.TermHandler)
                {
                    case TermHandler.BasicTerm:
                        //If the user clicks Cancel and the EditMode == Add, then remove the newly added term from the collection
                        if (_termEdit.EditMode == EditMode.Add)
                            _template.BasicTerms.RemoveAt(_termEdit.TermIndex);
                        break;

                    case TermHandler.ComplexList:
                        //If the user clicks Cancel and the EditMode == Add, then remove the newly added term from the collection
                        if (_termEdit.EditMode == EditMode.Add)
                            _template.ComplexLists.RemoveAt(_termEdit.TermIndex);
                        break;

                    case TermHandler.ComplexListField:
                        //If the user clicks Cancel and the EditMode == Add, then remove the newly added field from the collection
                        if (_termEdit.EditMode == EditMode.Add)
                        {
                            Business.ComplexList complexList = _template.ComplexLists[_termEdit.TermIndex] as ComplexList;
                            complexList.Fields.RemoveAt(complexList.Fields.Count - 1);
                        }
                        break;


                    case TermHandler.ComplexListItem:
                        break;
                }
            }

            if (this.AlertCount > 0)
                return;

            List<string> errors = ValidateForm();

            if (updateValues)
            {
                switch (_termEdit.TermHandler)
                {
                    case TermHandler.BasicTerm:
                        if (_termEdit.Term != null && termName != TermName)
                            errors.AddRange(_template.ReplaceEmbeddedAlteredTermNames());
                        break;

                    case TermHandler.ComplexList:
                        if (_termEdit.Term != null && termName != TermName)
                            errors.AddRange(_template.ReplaceEmbeddedAlteredTermNames());
                        break;

                    case TermHandler.ComplexListField:
                    case TermHandler.ComplexListItem:
                        break;
                }

                if ((errors != null) && (errors.Count > 0))
                {
                    System.IO.StringWriter sw = new System.IO.StringWriter();
                    sw.Write("Unable to save the term due to the following errors:\\n\\n");
                    foreach (string error in errors)
                    {
                        sw.Write(error);
                        sw.Write("\\n");
                    }
                    sw.Write("\\nPlease correct the error(s) and try again.");
                    RegisterAlert(sw.ToString());
                    return;
                }

                UpdateValues();
            }

            Context.Items[Common.Names._CNTXT_Template] = _template;

            //IsChanged related change
            if (IsChangedInitial)
                Context.Items[Common.Names._CNTXT_IsChanged] = true;
            else if (IsChanged)
                Context.Items[Common.Names._CNTXT_IsChanged] = updateValues;
            else
                Context.Items[Common.Names._CNTXT_IsChanged] = false;

            switch (_termEdit.TermHandler)
            {
                case TermHandler.BasicTerm:
                    _termEdit.TermHandler = TermHandler.BasicTerm;
                    Context.Items[Common.Names._CNTXT_TermEdit] = _termEdit;
                    Server.Transfer("TemplateTerms.aspx");
                    break;

                case TermHandler.ComplexList:
                    Context.Items[Common.Names._CNTXT_TermEdit] = _termEdit;
                    Server.Transfer("TemplateComplexLists.aspx");
                    break;

                case TermHandler.ComplexListField:
                    if (updateValues)
                    {
                        _termEdit.ComplexListField.Name = newFieldName;
                        _termEdit.ComplexListField.BigText = _termEdit.ComplexListField.FilterTerm.BigText;
                        _termEdit.ComplexListField.Summary = _termEdit.ComplexListField.FilterTerm.Summary;
                        (_template.ComplexLists[_termEdit.TermIndex] as ComplexList).Fields[_termEdit.ComplexListFieldIndex] = _termEdit.ComplexListField;
                    }

                    _termEdit.TermHandler = TermHandler.ComplexList;
                    Context.Items[Common.Names._CNTXT_TermEdit] = _termEdit;
                    Server.Transfer("TermEditComplexList.aspx");
                    break;

                case TermHandler.ComplexListItem:
                    throw new Exception(string.Format("TermHandler '{0}' not handled", _termEdit.TermHandler.ToString()));
            }

        }

        protected void SetTermGroupInContext(int selectedIndex)
        {
            if (_template.SecurityModel == SecurityModel.Advanced)
                Context.Items[Common.Names._CNTXT_SelectedTermGroupIndex] = selectedIndex;			
        }

        protected void SetTermGroupInViewState()
        {
            //Store the Term Group
            if (_template.SecurityModel == SecurityModel.Advanced && Context.Items[Common.Names._CNTXT_SelectedTermGroupIndex]!= null)
                ViewState[Common.Names._CNTXT_SelectedTermGroupIndex] = Context.Items[Common.Names._CNTXT_SelectedTermGroupIndex];
        }
		#endregion

		#region ViewState events

		protected override object SaveViewState()
		{
            ViewState[_KH_VS_TERM_EDIT] = _termEdit;
            return base.SaveViewState();
		}

		protected override void LoadViewState(object savedState)
		{
			base.LoadViewState(savedState);
            _termEdit = (TermEdit)ViewState[_KH_VS_TERM_EDIT];
        }

		#endregion

        public static void BindMultiSelectList(CheckBoxList CheckBoxList1, List<string> selectedIds)
        {
            foreach (ListItem item in CheckBoxList1.Items)
            {
                item.Selected = selectedIds.Contains(item.Value);
            }
        }
    }
}
