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
	public partial class TemplateTermGroupAddEdit : BaseTemplatePage
	{

		#region private members
		private Guid _termGroupId;
		private TermGroup _termGroup;
		private EditMode _editMode = EditMode.None;
		private const string _KH_CLOSEDIALOG = "_kh_closedialog";
		private const string _KH_VS_EDITMODE = "_kh_vs_EditMode";
		private const string _KH_VS_TERMGROUP = "_kh_vs_TermGroup";
		#endregion

		#region eventhandlers

		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);
			if (!IsPostBack)
			{
				GetContextData();
				txtTermGroupName.Text = _termGroup.Name;
                txtTermGroupDescription.Text = _termGroup.Description;
				header.PageTitle = string.Format("{0} Tab - {1}", _editMode, _termGroup.Name);
			}
		}

		private void GetContextData()
		{
			if (Context.Items[Common.Names._CNTXT_EditMode] != null)
				_editMode = (EditMode)Context.Items[Common.Names._CNTXT_EditMode];
			else
				_editMode = EditMode.Add;

			if (_editMode == EditMode.Edit)
			{
				_termGroupId = (Guid)Context.Items[Common.Names._CNTXT_TermGroupId];
				_termGroup = _template.FindTermGroup(_termGroupId);
			}
			else
			{
				_termGroup = new TermGroup(Helper.GetDefaultTermGroupName(_template),String.Empty, _template.SecurityModel,Business.TermGroup.TermGroupType.AdvancedBasicTerm);
				_termGroupId = _termGroup.ID;
			}
		}


		protected void btnOk_Click(object sender, EventArgs e)
		{


			//Context.Items[Common.Names._CNTXT_Template] = _template;
			//Server.Transfer("TemplateTerms.aspx");
            ReturnToCaller(true);
		}


		protected void btnCancel_Click(object sender, EventArgs e)
		{
			//Context.Items[Common.Names._CNTXT_Template] = _template;
			//Server.Transfer("TemplateTerms.aspx");
            ReturnToCaller(false);
		}

		#endregion

        #region Private Methods
        private void ReturnToCaller(bool updateValues)
        {


            if (updateValues)
            {
                //Validation
                if (!ValidateForm())
                    return;

                //set ischanged flag in the context
                bool dataChanged;
                SetValues(out dataChanged);
                Context.Items[Common.Names._CNTXT_IsChanged] = dataChanged;


                //Update the template with the new values
                UpdateTemplate();
            }



            
            Server.Transfer("TemplateTerms.aspx");
        }

        private void UpdateTemplate()
        {
            _termGroup.Name = txtTermGroupName.Text;
            _termGroup.Description = txtTermGroupDescription.Text;
            if (_editMode == EditMode.Add)
            {
                _template.AddTermGroup(_termGroup.Name, _termGroup.Description, _template.SecurityModel, TermGroup.TermGroupType.AdvancedBasicTerm);
            }
            else
            {
                _template.FindTermGroup(_termGroup.ID).Name = _termGroup.Name;
                _template.FindTermGroup(_termGroup.ID).Description = _termGroup.Description;
            }

            Context.Items[Common.Names._CNTXT_Template] = _template;
        }

        private bool ValidateForm()
        {
            if (_editMode == EditMode.Add)
                if (_template.TermGroupExists(txtTermGroupName.Text))
                    RegisterAlert(string.Format("The tab name '{0}' is already being used.   You must select a different tab name.", txtTermGroupName.Text));

            if (_editMode == EditMode.Edit)
                if (txtTermGroupName.Text != _termGroup.Name)
                    if (_template.TermGroupExists(txtTermGroupName.Text))
                        RegisterAlert(string.Format("The tab name '{0}' is already being used.   You must select a different tab name.", txtTermGroupName.Text));

            if (this.AlertCount == 0)
                return true;

            return false;
        }


        private void SetValues(out bool dataChanged)
        {
            dataChanged = false;

            if (_editMode == EditMode.Edit)
            {
                if (_termGroup.Name != txtTermGroupName.Text)
                    dataChanged = true;

                if (_termGroup.Description != txtTermGroupDescription.Text)
                    dataChanged = true;
            }
            else if (_editMode == EditMode.Add)
            {
                dataChanged = true;
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
			_editMode = (EditMode)ViewState[_KH_VS_EDITMODE];
			_termGroup = (TermGroup)ViewState[_KH_VS_TERMGROUP];
		}

		protected override object SaveViewState()
		{
			ViewState[_KH_VS_EDITMODE] = _editMode;
			ViewState[_KH_VS_TERMGROUP] = _termGroup;
			return base.SaveViewState();
		}

		#endregion

	}
}
