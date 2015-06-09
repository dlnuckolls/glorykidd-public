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
using Kindred.Knect.ITAT.Business;

namespace Kindred.Knect.ITAT.Web
{
	public partial class TermEditDate : BaseTermEditPage
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
			LoadFormatDropDown();

            trBigText.Visible = IsComplexListField;
            trShowSummary.Visible = IsComplexListField;
            trKeywordSearchable.Visible = !IsComplexListField;

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

            if (DisplayTermMapping())
            {
                Helper.LoadListControl(ddlMappedTerm, _systemDBFieldTerms, _DBFIELD_TERM_TEXT, _DBFIELD_TERM_VALUE, _mappedTemplateTermValue, true, "(Select a System Term)", string.Empty);
                btnMap.OnClientClick = string.Format("javascript:return _khb_onMap('{0}','{1}');", ddlMappedTerm.ClientID, TermName);
                trTermMapping.Visible = true;
                if (string.IsNullOrEmpty(_mappedTemplateTermValue))
                {
                    btnMap.Text = _MAPPING_BUTTON_MAP;
                    ddlMappedTerm.Enabled = true;
                }
                else
                {
                    btnMap.Text = _MAPPING_BUTTON_UNMAP;
                    ddlMappedTerm.Enabled = false;
                }
            }
            else
                trTermMapping.Visible = false;
		}

		protected override void UpdateValues()
		{
            Business.DateTerm dateTerm = Term as Business.DateTerm;
			if (dateTerm == null)
                throw new Exception(string.Format("Unable to cast _term \"{0}\" to a Business.DateTerm object.", TermName));

			TermName = txtTermName.Text;
            if (!IsComplexListField)
            {
                if (_template.SecurityModel == Business.SecurityModel.Advanced)
                    dateTerm.TermGroupID = new Guid(ddlTermGroup.SelectedValue);
                else
                    dateTerm.TermGroupID = _template.BasicSecurityTermGroupID;
            }
			dateTerm.IsHeader = chkHeaderTerm.Checked;
			dateTerm.Editable = true;
			//dateTerm.ValidateOnSave = chkValidateOnSave.Checked;
            dateTerm.ValidateOnSave = this.vos.Validate;
            dateTerm.ValidationStatuses = this.vos.SelectedStatuses;

			dateTerm.Required = chkRequired.Checked;
			dateTerm.Format = ddlFormat.SelectedValue;
            if (trKeywordSearchable.Visible)
                dateTerm.KeywordSearchable = chkbxKeywordSearchable.Checked;
            dateTerm.BigText = chkBigText.Checked;
            dateTerm.Summary = chkShowSummary.Checked;
        }

       protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            if (!IsPostBack)
            {
                this.vos.CanEdit = true;
                this.vos.Term = this.Term;
                this.vos.ITATSystem = ITATSystem;
            }


        }

       protected override void OnPreRender(EventArgs e)
       {
           base.OnPreRender(e);
           if (_ddlMapIndex.HasValue)
           {
               ddlMappedTerm.Items[_ddlMapIndex.Value].Value = GetMappedTermValue(_ddlMapIndex.Value, string.Empty);
               ddlMappedTerm.SelectedIndex = 0;
           }
           _ddlMapIndex = null;
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
            Business.DateTerm dateTerm = Term as Business.DateTerm;
			if (dateTerm == null)
                throw new Exception(string.Format("Unable to cast _term \"{0}\" to a Business.DateTerm object.", TermName));
			//chkValidateOnSave.Checked = dateTerm.ValidateOnSave;
			chkHeaderTerm.Checked = dateTerm.IsHeader;
			chkRequired.Checked = (dateTerm.Required ?? false);
			ddlFormat.SelectedValue = dateTerm.Format;
            if (trKeywordSearchable.Visible)
                chkbxKeywordSearchable.Checked = (dateTerm.KeywordSearchable ?? false);
            chkBigText.Checked = dateTerm.BigText;
            chkShowSummary.Checked = dateTerm.Summary;
        }

		#endregion


		#region event handlers

		protected void btnOK_Click(object sender, EventArgs e)
		{
            //Pass the selected TermGroup Back so that that termgroup selection can be persisted.
            if (!IsComplexListField)
                SetTermGroupInContext(ddlTermGroup.SelectedIndex);
            SetContextDataAndReturn(txtTermName.Text, true, chkHeaderTerm.Checked);
		}

		protected void btnCancel_Click(object sender, EventArgs e)
		{
            //Pass the selected TermGroup Back so that that termgroup selection can be persisted.
            if (ShowTermGroups)
                SetTermGroupInContext((int)ViewState[Common.Names._CNTXT_SelectedTermGroupIndex]);
            SetContextDataAndReturn(txtTermName.Text, false, chkHeaderTerm.Checked);
		}

        protected void btnMap_Click(object sender, EventArgs e)
        {
            if (!ddlMappedTerm.Enabled)
            {
                MapTerm(null, null, ddlMappedTerm.SelectedIndex);
                btnMap.Text = _MAPPING_BUTTON_MAP;
                ddlMappedTerm.Enabled = true;
            }
            else
            {
                if (ddlMappedTerm.SelectedIndex > 0)
                {
                    MapTerm(ddlMappedTerm.Items[ddlMappedTerm.SelectedIndex].Text, GetMappedTermName(ddlMappedTerm.Items[ddlMappedTerm.SelectedIndex].Value), null);
                    btnMap.Text = _MAPPING_BUTTON_UNMAP;
                    ddlMappedTerm.Enabled = false;
                }
            }
        }


		#endregion


		#region private methods


		private void LoadFormatDropDown()
		{
			DateTime dt = new DateTime(2006, 3, 1);
			foreach (string dateFormat in Utility.DateHelper.DateFormats)
				ddlFormat.Items.Add(new ListItem(Utility.DateHelper.FormatDate(dt, dateFormat), dateFormat));
		}

		#endregion
		
	}
}
