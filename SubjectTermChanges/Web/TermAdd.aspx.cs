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
	public partial class TermAdd : BaseTemplatePage
	{

		#region private members
		private const string _KH_CLOSEDIALOG = "_kh_closedialog";
        private const string _KH_VS_TERM_EDIT = "_kh_vs_TermEdit";      
        private BaseTermEditPage.TermEdit _termEdit;
        #endregion

		#region eventhandlers

		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);
			if (!IsPostBack)
			{
                _termEdit = (BaseTermEditPage.TermEdit)Context.Items[Common.Names._CNTXT_TermEdit];
                if (_termEdit.EditMode != EditMode.Add)
                    throw new Exception(string.Format("Received edit mode {0} when edit mode {1} was expected.", _termEdit.EditMode.ToString(), EditMode.Add.ToString()));

                (header as Kindred.Knect.ITAT.Web.StandardHeader).PageTitle = _termEdit.TermHandler == BaseTermEditPage.TermHandler.ComplexListField ? "Add Field" : "Add Term";
				txtTermName.Text = Helper.GetDefaultBasicTermName(_template);
				LoadTermTypes();
                //Store the Term Group
                SetTermGroupInViewState();
			}
		}

		protected void btnContinue_Click(object sender, EventArgs e)
		{
            Business.TermType termType = (Kindred.Knect.ITAT.Business.TermType)Enum.Parse(typeof(Kindred.Knect.ITAT.Business.TermType), ddlTermType.SelectedValue);

            switch (_termEdit.TermHandler)
            {
                case BaseTermEditPage.TermHandler.BasicTerm:
                    //Note - similar code checks are made at the 'BaseTermEditPage' SetContextDataAndReturn call
                    if (_template.BasicTermExists(txtTermName.Text))
                        RegisterAlert(string.Format("The term name '{0}' is already being used.   You must select a different term name.", txtTermName.Text));

                    //20070810_DEG Bug #131 Add code here to check for 'first quote'
                    char cFirst = '\"';
                    if (_template.BasicTerm_FirstChar_Exists(txtTermName.Text, cFirst))
                        RegisterAlert(string.Format("The term name (up to the first character - ' {0} ')  '{1}' is already being used.   You must select a different term name.", cFirst, Business.Term.NameAtFirstChar(txtTermName.Text, cFirst)));

                    if (_template.ComplexListTermNameInUse(txtTermName.Text, null))
                        RegisterAlert(string.Format("The term name '{0}' is already being used in the Complex List.   You must select a different term name.", txtTermName.Text));

                    if (!Business.Term.ValidName(txtTermName.Text))
                        RegisterAlert(string.Format("\"{0}\" is not a valid term name.", txtTermName.Text));

                    if (this.AlertCount > 0)
                        return;

                    //Check to see if the selected term type is an External Term.
                    if (termType == Business.TermType.External)
                    {
                        Business.ExternalTerm externalTerm = new Business.ExternalTerm(false, _itatSystem.FindExternalInterfaceConfig(ddlTermType.SelectedItem.Text), this.Template as Business.ManagedItem, this.Template, false);   //note: if "this.Template" is a Template and not a ManagedItem, "this.Template as ManagedItem" equals null.
                        externalTerm.Name = txtTermName.Text;
                        externalTerm.TermType = Business.TermType.External;
                        _template.BasicTerms.Add(externalTerm);
                    }
                    else
                    {
                        Business.Term newTerm = Helper.CreateTerm(termType, false, _template.IsManagedItem, _template, false);
                        newTerm.Name = txtTermName.Text;
                        _template.BasicTerms.Add(newTerm);
                    }
                    _template.Refresh();

                    _termEdit.TermIndex = _template.BasicTerms.Count - 1;  //item just added
                    break;

                case BaseTermEditPage.TermHandler.ComplexList:
                    throw new Exception(string.Format("TermHandler '{0}' is not handled", _termEdit.TermHandler.ToString()));

                case BaseTermEditPage.TermHandler.ComplexListField:
                    Business.ComplexList complexList = _template.ComplexLists[_termEdit.TermIndex] as ComplexList;
                    //Trim leading and trailing spaces from the name to assist in locating the name within the rendering text.
                    string fieldName = txtTermName.Text.Trim();
                    if (complexList.FindField(fieldName) != null)
                        RegisterAlert(string.Format("The field name '{0}' is already being used.   You must select a different field name.", txtTermName.Text));

                    if (this.AlertCount > 0)
                        return;

                    Business.ComplexListField complexListField = new ComplexListField(complexList);

                    complexListField.FilterTerm = Helper.CreateTerm(termType, false, _template.IsManagedItem, _template, true);
                    complexListField.Name = fieldName;

                    complexList.Fields.Add(complexListField);
                    complexList.AddFieldToCurrentItems(complexListField.ID, false, complexListField.FilterTerm);
                    _termEdit.ComplexListFieldIndex = complexList.Fields.Count - 1;  //item just added
                    break;

                case BaseTermEditPage.TermHandler.ComplexListItem:
                    throw new Exception(string.Format("TermHandler '{0}' is not handled", _termEdit.TermHandler.ToString()));

            }
            Context.Items[Common.Names._CNTXT_Template] = _template;
            Context.Items[Common.Names._CNTXT_TermEdit] = _termEdit;

            SetTermGroupInContext();
            Server.Transfer(Helper.TemplateTermEditPage(termType));
        }

		protected void btnCancel_Click(object sender, EventArgs e)
		{
			Context.Items[Common.Names._CNTXT_Template] = _template;

            SetTermGroupInContext();

            switch (_termEdit.TermHandler)
            {
                case BaseTermEditPage.TermHandler.BasicTerm:
                    Server.Transfer("TemplateTerms.aspx");
                    break;

                case BaseTermEditPage.TermHandler.ComplexList:
                    throw new Exception(string.Format("TermHandler '{0}' is not handled", _termEdit.TermHandler));

                case BaseTermEditPage.TermHandler.ComplexListField:
                    _termEdit.Term = _template.ComplexLists[_termEdit.TermIndex] as ComplexList;
                    _termEdit.TermHandler = BaseTermEditPage.TermHandler.ComplexList;
                    Context.Items[Common.Names._CNTXT_TermEdit] = _termEdit;
                    Server.Transfer("TermEditComplexList.aspx");
                    break;

                case BaseTermEditPage.TermHandler.ComplexListItem:
                    throw new Exception(string.Format("TermHandler '{0}' is not handled", _termEdit.TermHandler));
            }
		}

        private void SetTermGroupInContext()
        {
            //Store the Term Group
            if (_template.SecurityModel == SecurityModel.Advanced)
                Context.Items[Common.Names._CNTXT_SelectedTermGroupIndex] = ViewState[Common.Names._CNTXT_SelectedTermGroupIndex];
        }

		#endregion


		#region private methods

        private void SetTermGroupInViewState()
        {
            if (!BaseTermEditPage.CheckComplexListField(_termEdit) && _template.SecurityModel == SecurityModel.Advanced)
                ViewState[Common.Names._CNTXT_SelectedTermGroupIndex] = Context.Items[Common.Names._CNTXT_SelectedTermGroupIndex];
        }

		private void LoadTermTypes()
		{
			ddlTermType.Items.Clear();
            bool externalTermValid = true;
            Business.TermType[] termTypes = null;
            if (BaseTermEditPage.CheckComplexListField(_termEdit))
            {
                externalTermValid = Business.ComplexListField.ValidTermTypes().Contains(Business.TermType.External);
                termTypes = Business.ComplexListField.ValidTermTypes().ToArray();
            }
            else
            {
                termTypes = (Business.TermType[])Enum.GetValues(typeof(Business.TermType));
            }
			foreach (Business.TermType termType in termTypes)
			{
				switch (termType)
				{
					case Business.TermType.None:
					case Business.TermType.ComplexList:
					case Business.TermType.External:
                    case Business.TermType.PlaceHolderAttachments:
                    case Business.TermType.PlaceHolderComments:
                        break;
					default:
						if (termType == Business.TermType.MSO)
							if (_template.MSODefined)
								continue;
						if (termType == Business.TermType.Renewal)
							if (_template.RenewalDefined)
								continue;
						ddlTermType.Items.Add(termType.ToString());
						break;
				}
            }
            if (externalTermValid)
            {
                //add each External Term individually
                foreach (Business.ExternalInterfaceConfig extInterface in _itatSystem.ExternalInterfaces)
                    if (!_template.ExternalTermDefined(extInterface.Name))
                        ddlTermType.Items.Add(new ListItem(extInterface.Name, Business.TermType.External.ToString()));
            }
        }

		#endregion


		#region base class overrides

		internal override HtmlGenericControl HTMLBody()
		{
			return this.body;
		}

		internal override Control ResizablePanel()
		{
			return editBody;
		}

		protected override TemplateHeader HeaderControl()
		{
			return null;
		}

		protected override void LoadViewState(object savedState)
		{
			base.LoadViewState(savedState);
            _termEdit = (BaseTermEditPage.TermEdit)ViewState[_KH_VS_TERM_EDIT];
        }

		protected override object SaveViewState()
		{
            ViewState[_KH_VS_TERM_EDIT] = _termEdit;
            return base.SaveViewState();
		}


		#endregion

	}
}
