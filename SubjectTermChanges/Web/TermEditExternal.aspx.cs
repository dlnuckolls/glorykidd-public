using System;
using System.Data;
using System.Configuration;
using System.Collections.Generic;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

namespace Kindred.Knect.ITAT.Web
{
	public partial class TermEditExternal : BaseTermEditPage
	{

		#region base class overrides

		internal override HtmlGenericControl HTMLBody()
		{
			return this.body;
		}

		internal override Control ResizablePanel()
		{
			return editBody;
		}

		protected override TextBox TermNameControl()
		{
			return txtTermName;
		}

		protected override void InitializeForm()
		{
            header.PageTitle = PageTitle;
            txtTermName.Text = TermName;

            if (ShowTermGroups)
			{
				trTermGroup.Visible = true;
				trHeaderTerm.Visible = true;
                Helper.LoadListControl(ddlTermGroup, _template.GetTermGroups(Business.TermGroup.TermGroupType.AdvancedBasicTerm), "Name", "ID", Term.TermGroupID.ToString(), true, "(Select a Tab)", Guid.Empty.ToString());
			}
			else
			{
				trTermGroup.Visible = false;
				trHeaderTerm.Visible = false;
			}

            bool isRetroAdmin = SecurityHelper.CanPerformFunction(_itatSystem.AllowedRoles(Business.XMLNames._AF_RetroAdmin));
            trSyncWithSystem.Visible = isRetroAdmin && 
                _template.RetroModel != Business.Retro.RetroModel.Off &&
                Term.RequiresSystemSync(_itatSystem);
		}

		protected override void UpdateValues()
		{
            Business.ExternalTerm externalTerm = Term as Business.ExternalTerm;
			if (externalTerm == null)
                throw new Exception(string.Format("Unable to cast term \"{0}\" to a Business.ExternalTerm object.", TermName));
			TermName = txtTermName.Text;
			lblTermType.Text = string.Format("{0} (External Term)", externalTerm.InterfaceConfig.Name);
            if (!IsComplexListField)
            {
                if (_template.SecurityModel == Business.SecurityModel.Advanced)
                    externalTerm.TermGroupID = new Guid(ddlTermGroup.SelectedValue);
                else
                    externalTerm.TermGroupID = _template.BasicSecurityTermGroupID;
            }
			externalTerm.IsHeader = chkHeaderTerm.Checked;
			externalTerm.Editable = chkEditable.Checked;
			//externalTerm.ValidateOnSave = false;
			externalTerm.Required = chkRequired.Checked;
			externalTerm.KeywordSearchable = chkbxKeywordSearchable.Checked;
		}


		protected override void ShowHideFields()
		{
		}

		protected override System.Collections.Generic.List<string> ValidateForm()
		{
			List<string> rtn = new List<string>();

            //Make sure a Term Group is selected.

            if (trTermGroup.Visible == true && ddlTermGroup.SelectedValue == Guid.Empty.ToString())
                rtn.Add("A Tab must be selected.");


			return rtn;
		}

		protected override void LoadValues()
		{
            Business.ExternalTerm externalTerm = Term as Business.ExternalTerm;
			if (externalTerm == null)
                throw new Exception(string.Format("Unable to cast _term \"{0}\" to a Business.ExternalTerm object.", TermName));
			lblTermType.Text = externalTerm.InterfaceConfig.Name;
			chkHeaderTerm.Checked = externalTerm.IsHeader;
			chkEditable.Checked = externalTerm.Editable ?? true;
			//chkValidateOnSave.Checked = externalTerm.ValidateOnSave;
			chkRequired.Checked = (externalTerm.Required ?? false);
			chkbxKeywordSearchable.Checked = (externalTerm.KeywordSearchable ?? false);
		}

		#endregion


		#region event handlers

		protected void btnOK_Click(object sender, EventArgs e)
		{
            if (!IsComplexListField)
                SetTermGroupInContext(ddlTermGroup.SelectedIndex);
            SetContextDataAndReturn(txtTermName.Text, true, chkHeaderTerm.Checked);
		}

		protected void btnCancel_Click(object sender, EventArgs e)
		{
			if (ShowTermGroups)
                SetTermGroupInContext((int)ViewState[Common.Names._CNTXT_SelectedTermGroupIndex]);
            SetContextDataAndReturn(txtTermName.Text, false, chkHeaderTerm.Checked);
		}

        protected void btnSyncWithSystem_Click(object sender, EventArgs e)
        {
            Term.SyncWithSystem(_itatSystem);
            trSyncWithSystem.Visible = false;
            IsChanged = true;
        }

		#endregion


		#region private methods


		#endregion
		
	}
}
