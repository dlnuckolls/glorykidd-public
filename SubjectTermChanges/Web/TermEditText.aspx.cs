using System;
using System.Collections.Generic;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

namespace Kindred.Knect.ITAT.Web
{
	public partial class TermEditText : BaseTermEditPage
	{
		private const string _KH_CLOSEDIALOG = "_kh_closedialog";

        #region base overrides

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
            //Note - TermMapping Change
            if (_ddlMapIndex.HasValue)
            {
                ddlMappedTerm.Items[_ddlMapIndex.Value].Value = GetMappedTermValue(_ddlMapIndex.Value, string.Empty);
                ddlMappedTerm.SelectedIndex = 0;
            }
            _ddlMapIndex = null;
        }

        protected override void UpdateValues()
        {
            Business.TextTerm textTerm = Term as Business.TextTerm;
            if (textTerm == null)
                throw new Exception(string.Format("Unable to cast _term \"{0}\" to a Business.TextTerm object.", TermName));

            TermName = txtTermName.Text;
            textTerm.ValidateOnSave = this.vos.Validate;
            textTerm.ValidationStatuses = this.vos.SelectedStatuses;

            if (!IsComplexListField)
            {
                if (_template.SecurityModel == Business.SecurityModel.Advanced)
                    textTerm.TermGroupID = new Guid(ddlTermGroup.SelectedValue);
                else
                    textTerm.TermGroupID = _template.BasicSecurityTermGroupID;
            }

            textTerm.IsHeader = chkHeaderTerm.Checked;
            textTerm.Required = chkRequired.Checked;
            textTerm.Default = chkDefault.Checked;
            textTerm.DefaultValueRequired = chkDefault.Checked;
            textTerm.KeywordSearchable = chkKeywordSearchable.Checked;
            textTerm.DefaultValue = txtDefaultValue.Text;
            textTerm.Editable = chkEditable.Checked;
            textTerm.PreserveWhiteSpaceDocument = trPreserveDocumentLineBreaks.Visible ? chkPreserveDocumentWhiteSpace.Checked : false;
            textTerm.PreserveWhiteSpaceSummary = trPreserveSummaryLineBreaks.Visible ? chkPreserveSummaryWhiteSpace.Checked : false;
            textTerm.Format = (Business.TextTermFormat)Enum.Parse(typeof(Business.TextTermFormat), ddlFormat.SelectedValue);
            if (txtLowerLimit.Visible)
                textTerm.Min = int.Parse(txtLowerLimit.Text);
            if (txtUpperLimit.Visible)
                textTerm.Max = int.Parse(txtUpperLimit.Text);
            if (chkShowCents.Visible)
                textTerm.ShowCents = chkShowCents.Checked;
            if (trTextNumberFormat.Visible)
                textTerm.UseTextNumberFormat = chkTextNumberFormat.Checked;
            if (trKeywordSearchable.Visible)
                textTerm.KeywordSearchable = chkKeywordSearchable.Checked;
            textTerm.BigText = chkBigText.Checked;
            textTerm.Summary = chkShowSummary.Checked;
        }

        protected override List<string> ValidateForm()
        {
            List<string> rtn = new List<string>();

            if (txtLowerLimit.Visible)
            {
                long tempMin = 0;
                if (long.TryParse(txtLowerLimit.Text, out tempMin))
                {
                    if ((ddlFormat.SelectedValue == Business.TextTermFormat.Plain.ToString()) && (chkDefault.Checked))
                        if (txtDefaultValue.Text.Length < tempMin)
                            rtn.Add(string.Format("The length of the default text ({0}) is shorter than the minimum length ({1}).", txtDefaultValue.Text.Length, tempMin));
                }
                else
                    rtn.Add(string.Format("'{0}' is not a valid {1}.   ", txtLowerLimit.Text, lblLowerLimit.Text));
            }

            if (txtUpperLimit.Visible)
            {
                long tempMax = 0;
                if (long.TryParse(txtUpperLimit.Text, out tempMax))
                {
                    if ((ddlFormat.SelectedValue == Business.TextTermFormat.Plain.ToString()) && (chkDefault.Checked))
                        if (txtDefaultValue.Text.Length > tempMax)
                            rtn.Add(string.Format("The length of the default text ({0}) is longer than the maximum length ({1}).", txtDefaultValue.Text.Length, tempMax));
                }
                else
                    rtn.Add(string.Format("'{0}' is not a valid {1}.", txtUpperLimit.Text, lblUpperLimit.Text));
            }

            if (txtLowerLimit.Visible && txtUpperLimit.Visible)
            {
                long tempMin = 0;
                if (long.TryParse(txtLowerLimit.Text, out tempMin))
                {
                    long tempMax = 0;
                    if (long.TryParse(txtUpperLimit.Text, out tempMax))
                    {
                        if (tempMin > tempMax)
                            rtn.Add(string.Format("{0} value '{1}' cannot be greater than {2} value '{3}'.", lblUpperLimit.Text, txtUpperLimit.Text, lblLowerLimit.Text, txtLowerLimit.Text));
                    }
                }
            }

            if (chkDefault.Checked)
            {
                if (string.IsNullOrEmpty(txtDefaultValue.Text))
                {
                    rtn.Add("Since the Default text is required, it must be supplied.");
                }
            }

            //Make sure a Term Group is selected.

            if (trTermGroup.Visible == true && ddlTermGroup.SelectedValue == Guid.Empty.ToString())
                rtn.Add("A Tab must be selected.");

            return rtn;
        }

        protected override void LoadValues()
        {
            Business.TextTerm textTerm = Term as Business.TextTerm;
            if (textTerm == null)
                throw new Exception(string.Format("Unable to cast _term \"{0}\" to a Business.TextTerm object.", TermName));

            chkHeaderTerm.Checked = textTerm.IsHeader;
            chkRequired.Checked = (textTerm.Required ?? false);

            chkDefault.Checked = (textTerm.Default ?? false);
            txtDefaultValue.Text = textTerm.DefaultValue;
            chkEditable.Checked = (textTerm.Editable ?? false);
            chkPreserveDocumentWhiteSpace.Checked = textTerm.PreserveWhiteSpaceDocument;
            chkPreserveSummaryWhiteSpace.Checked = textTerm.PreserveWhiteSpaceSummary;
            chkKeywordSearchable.Checked = (textTerm.KeywordSearchable ?? false);

            ddlFormat.SelectedValue = textTerm.Format.ToString();
            if (ddlFormat.SelectedIndex == -1)
                ddlFormat.SelectedIndex = 0;

            if (txtLowerLimit.Visible)
                txtLowerLimit.Text = textTerm.Min.ToString();
            if (txtUpperLimit.Visible)
                txtUpperLimit.Text = textTerm.Max.ToString();
            if (chkShowCents.Visible)
                chkShowCents.Checked = (textTerm.ShowCents ?? false);
            if (trTextNumberFormat.Visible)
                chkTextNumberFormat.Checked = (textTerm.UseTextNumberFormat ?? false);
            if (trKeywordSearchable.Visible)
                chkKeywordSearchable.Checked = (textTerm.KeywordSearchable ?? false);

            chkBigText.Checked = textTerm.BigText;
            chkShowSummary.Checked = textTerm.Summary;
            ShowHideFields();
        }

        protected override void ShowHideFields()
        {
            ShowHideDefaultValue();
            ShowHideFormatSpecifiers();
        }

        protected override void InitializeForm()
        {
            header.PageTitle = PageTitle;
            txtTermName.Text = TermName;
            LoadFormatDropDown();

            trBigText.Visible = IsComplexListField;
            trShowSummary.Visible = IsComplexListField;
            trKeywordSearchable.Visible = !IsComplexListField;
            trPreserveSummaryLineBreaks.Visible = !IsComplexListField;
            trPreserveDocumentLineBreaks.Visible = !IsComplexListField;
            trEditable.Visible = !IsComplexListField;

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

            //Note - TermMapping Change
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

        //Note - TermMapping Change
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

		protected void ddlFormat_SelectedIndexChanged(object sender, EventArgs e)
		{
			ShowHideFormatSpecifiers();
		}

        protected void chkDefault_OnCheckedChanged(object sender, EventArgs e)
        {
            ShowHideDefaultValue();
        }

        #endregion

        #region private methods

        private void LoadFormatDropDown()
        {
            Business.TextTermFormat[] textTermFormats = (Business.TextTermFormat[])Enum.GetValues(typeof(Business.TextTermFormat));
            foreach (Business.TextTermFormat textTermFormat in textTermFormats)
                ddlFormat.Items.Add(new ListItem(textTermFormat.ToString("G")));
        }

        private void ShowHideFormatSpecifiers()
		{
			if (ddlFormat.SelectedValue == Business.TextTermFormat.Plain.ToString())
			{
				lblLowerLimit.Visible = true;
				lblUpperLimit.Visible = true;
				txtLowerLimit.Visible = true;
				txtUpperLimit.Visible = true;
				lblLowerLimit.Text = "Min Length:";
				lblUpperLimit.Text = "Max Length:";
				lblShowCents.Visible = false;
				chkShowCents.Visible = false;
				trTextNumberFormat.Visible = false;
			}
			else if (ddlFormat.SelectedValue == Business.TextTermFormat.Number.ToString())
			{
				lblLowerLimit.Visible = true;
				lblUpperLimit.Visible = true;
				txtLowerLimit.Visible = true;
				txtUpperLimit.Visible = true;
				lblLowerLimit.Text = "Lower Limit:";
				lblUpperLimit.Text = "Upper Limit:";
				lblShowCents.Visible = false;
				chkShowCents.Visible = false;
				trTextNumberFormat.Visible = true;
			}
			else if (ddlFormat.SelectedValue == Business.TextTermFormat.Currency.ToString())
			{
				lblLowerLimit.Visible = true;
				lblUpperLimit.Visible = true;
				txtLowerLimit.Visible = true;
				txtUpperLimit.Visible = true;
				lblLowerLimit.Text = "Lower Limit:";
				lblUpperLimit.Text = "Upper Limit:";
				lblShowCents.Visible = true;
				lblShowCents.Text = "Show Cents:";
				chkShowCents.Visible = true;
				trTextNumberFormat.Visible = false;
			}
			else if (ddlFormat.SelectedValue == Business.TextTermFormat.SSN.ToString() ||
			 ddlFormat.SelectedValue == Business.TextTermFormat.Phone.ToString() ||
			 ddlFormat.SelectedValue == Business.TextTermFormat.PhonePlusExtension.ToString())
			{
				lblLowerLimit.Visible = false;
				lblUpperLimit.Visible = false;
				txtLowerLimit.Visible = false;
				txtUpperLimit.Visible = false;
				lblShowCents.Visible = false;
				chkShowCents.Visible = false;
				trTextNumberFormat.Visible = false;
			}
			else
			{
				//default case
			}
		}

		private void ShowHideDefaultValue()
		{
			if (chkDefault.Checked)
				txtDefaultValue.Style["visibility"] = "visible";
			else
				txtDefaultValue.Style["visibility"] = "hidden";
        }
    
        #endregion
    }
}
