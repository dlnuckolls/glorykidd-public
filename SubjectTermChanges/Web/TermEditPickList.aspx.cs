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
	public partial class TermEditPickList : BaseTermEditPage
	{

		private const string _KH_CLOSEDIALOG = "_kh_closedialog";



		internal override HtmlGenericControl HTMLBody()
		{
			return this.body;
		}

		protected override TextBox TermNameControl()
		{
			return txtTermName;
		}

		internal override Control ResizablePanel()
		{
			return editBody;
		}

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

		protected override void InitializeForm()
		{
            header.PageTitle = PageTitle;
            txtTermName.Text = TermName;
			LoadProfileScreenViews();

            trBigText.Visible = IsComplexListField;
            trShowSummary.Visible = IsComplexListField;
            trKeywordSearchable.Visible = !IsComplexListField;
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
            Business.PickListTerm pickListTerm = Term as Business.PickListTerm;
			if (pickListTerm == null)
                throw new Exception(string.Format("Unable to cast _term \"{0}\" to a Business.PickListTerm object.", TermName));

            if (trKeywordSearchable.Visible)
                pickListTerm.KeywordSearchable = chkboxKeywordSearchable.Checked;

			TermName = txtTermName.Text;
            if (!IsComplexListField)
            {
                if (_template.SecurityModel == Business.SecurityModel.Advanced)
                    pickListTerm.TermGroupID = new Guid(ddlTermGroup.SelectedValue);
                else
                    pickListTerm.TermGroupID = _template.BasicSecurityTermGroupID;
            }
			pickListTerm.IsHeader = chkHeaderTerm.Checked;
			pickListTerm.ValidateOnSave = this.vos.Validate;
            pickListTerm.ValidationStatuses = this.vos.SelectedStatuses;

			pickListTerm.Required = chkRequired.Checked;
			pickListTerm.RequiredSelectedValue = txtRequiredValue.Text; 
			pickListTerm.DefaultValueRequired = chkDefault.Checked;
			pickListTerm.DefaultValue = txtDefaultValue.Text;
			pickListTerm.Default = chkDefault.Checked;
			pickListTerm.Editable = chkEditable.Checked;
			pickListTerm.UseTextNumberFormat = chkTextNumberFormat.Checked;
			pickListTerm.MultiSelect = (rblProfileView.SelectedValue == "Multiple");

			//translate each line in txtListItems.Text into a string in pickListTerm.PickListItems
			pickListTerm.PickListItems.Clear();
			pickListTerm.PickListItems = GetListItems();
            pickListTerm.BigText = chkBigText.Checked;
            pickListTerm.Summary = chkShowSummary.Checked;
        }

		private List<Business.PickListItem> GetListItems()
		{
			List<Business.PickListItem> pickList = new List<Kindred.Knect.ITAT.Business.PickListItem>();
			System.IO.StringReader sr = new System.IO.StringReader(txtListItems.Text);
			bool atEnd = false;
			while (!atEnd)
			{
				string s = sr.ReadLine();
				if (string.IsNullOrEmpty(s))
					atEnd = true;
				else
				{
					Business.PickListItem pickListItem = new Business.PickListItem();
					pickListItem.Value = s;
					pickList.Add(pickListItem);
				}
			}
			return pickList;
		}

		protected override List<string> ValidateForm()
		{
			List<string> rtn = new List<string>();
			if (txtListItems.Text.Trim().Length == 0)
			{
				rtn.Add("The PickList term must have at least one item.");
			}
			else
			{
                if (!IsComplexListField)
                {
                    List<Business.PickListItem> pickListItems = GetListItems();
                    Business.PickListTerm pickListTerm = Term as Business.PickListTerm;
                    foreach (Business.TermDependency termDependency in _template.TermDependencies)
                    {
                        if (termDependency.Target == Business.DependencyTarget.Term)
                        {
                            foreach (Guid dependentTermID in termDependency.DependentTermIDs)
                            {
                                if (pickListTerm.ID.Equals(dependentTermID))
                                    if ((termDependency.Action.SetValue != null) && (termDependency.Action.SetValue != Business.Term._SET_VALUE_DEFAULT) && (!Business.PickListTerm.Contains(GetListItems(), termDependency.Action.SetValue)))
                                        rtn.Add(string.Format("The PickList term must contain value '{0}' since it is referenced in term dependency '{1}'.", termDependency.Action.SetValue, termDependency.SourceTermText));
                            }
                        }

                        foreach (Business.TermDependencyCondition tdCondition in termDependency.Conditions)
                        {
                            if (pickListTerm.ID.Equals(tdCondition.SourceTermID))
                            {
                                bool bValue1Exists = false;
                                if (!string.IsNullOrEmpty(tdCondition.Value1))
                                {
                                    foreach (Business.PickListItem pickListItem in pickListItems)
                                    {
                                        if (pickListItem.Value == tdCondition.Value1)
                                        {
                                            bValue1Exists = true;
                                            break;
                                        }
                                    }
                                }
                                else
                                    bValue1Exists = true;

                                bool bValue2Exists = false;
                                if (!string.IsNullOrEmpty(tdCondition.Value2))
                                {
                                    foreach (Business.PickListItem pickListItem in pickListItems)
                                    {
                                        if (pickListItem.Value == tdCondition.Value2)
                                        {
                                            bValue2Exists = true;
                                            break;
                                        }
                                    }
                                }
                                else
                                    bValue2Exists = true;

                                if (!bValue1Exists)
                                    rtn.Add(string.Format("The PickList term must contain value '{0}' since it is referenced in term dependency '{1}'.", tdCondition.Value1, termDependency.SourceTermText));
                                if (!bValue2Exists)
                                    rtn.Add(string.Format("The PickList term must contain value '{0}' since it is referenced in term dependency '{1}'.", tdCondition.Value2, termDependency.SourceTermText));
                            }
                        }
                    }
                }
			}

			if (chkDefault.Checked)
			{
				if (string.IsNullOrEmpty(txtDefaultValue.Text))
				{
					rtn.Add("Since the Default choice is required, it must be supplied.");
				}
				else
				{
					//Make sure the default text matches one of the choices
					if (null == Business.PickListTerm.FindItem(GetListItems(), txtDefaultValue.Text))
					{
						rtn.Add("The Default choice must be a member of the list.");
					}
				}
			}

            //Make sure a Term Group is selected.

            if (!IsComplexListField)
            {
                if (trTermGroup.Visible == true && ddlTermGroup.SelectedValue == Guid.Empty.ToString())
                    rtn.Add("A Tab must be selected.");
            }

			return rtn;
		}

		protected override void LoadValues()
		{
            Business.PickListTerm pickListTerm = Term as Business.PickListTerm;
			if (pickListTerm == null)
                throw new Exception(string.Format("Unable to cast _term \"{0}\" to a Business.PickListTerm object.", TermName));

			chkHeaderTerm.Checked = pickListTerm.IsHeader;
            if (trKeywordSearchable.Visible)
                chkboxKeywordSearchable.Checked = (pickListTerm.KeywordSearchable ?? false);

			//chkValidateOnSave.Checked = pickListTerm.ValidateOnSave;
			chkRequired.Checked = (pickListTerm.Required ?? false);
			chkDefault.Checked = (pickListTerm.Default ?? false);
			txtDefaultValue.Text = pickListTerm.DefaultValue;
			txtRequiredValue.Text = pickListTerm.RequiredSelectedValue;
			chkEditable.Checked = (pickListTerm.Editable ?? false);
			chkTextNumberFormat.Checked = (pickListTerm.UseTextNumberFormat ?? false);
			if (pickListTerm.MultiSelect.HasValue)
				rblProfileView.SelectedValue = (bool)pickListTerm.MultiSelect ? "Multiple" : "Single";
			else
				rblProfileView.SelectedValue = "Single";

			System.IO.StringWriter sw = new System.IO.StringWriter();
			foreach (Business.PickListItem item in pickListTerm.PickListItems)
				sw.WriteLine(item.Value);
			txtListItems.Text = sw.ToString();
            chkBigText.Checked = pickListTerm.BigText;
            chkShowSummary.Checked = pickListTerm.Summary;
            ShowHideFields();
		}


		private void LoadProfileScreenViews()
		{
            if (!IsComplexListField)
            {
                rblProfileView.Items.Add(new ListItem("Single", "Single"));
                rblProfileView.Items.Add(new ListItem("Multiple", "Multiple"));
            }
            else
                rblProfileView.Visible = false;
		}


		protected override void ShowHideFields()
		{
			ShowHideRequiredValue();
			ShowHideDefaultValue();
		}

		private void ShowHideRequiredValue()
		{
			if (chkRequired.Checked)
				txtRequiredValue.Style["visibility"] = "visible";
			else
				txtRequiredValue.Style["visibility"] = "hidden";
		}

		private void ShowHideDefaultValue()
		{
			if (chkDefault.Checked)
				txtDefaultValue.Style["visibility"] = "visible";
			else
				txtDefaultValue.Style["visibility"] = "hidden";			
		}

		protected void chkDefault_OnCheckedChanged(object sender, EventArgs e)
		{
			ShowHideDefaultValue();
		}

		protected void chkRequired_OnCheckedChanged(object sender, EventArgs e)
		{
			ShowHideRequiredValue();
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


	}
}
