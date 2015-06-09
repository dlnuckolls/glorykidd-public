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
	public partial class TermEditMSO : BaseTermEditPage
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

		protected override void InitializeForm()
		{
            header.PageTitle = PageTitle;
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

		protected override void UpdateValues()
		{
            Business.MSOTerm MSOTerm = Term as Business.MSOTerm;
			if (MSOTerm == null)
                throw new Exception(string.Format("Unable to cast _term \"{0}\" to a Business.MSOTerm object.", TermName));

			MSOTerm.KeywordSearchable = chkboxKeywordSearchable.Checked;
			MSOTerm.Required = chkRequired.Checked;

			TermName = txtTermName.Text;
			//MSOTerm.ValidateOnSave = chkValidateOnSave.Checked;
            if (!IsComplexListField)
            {
                if (_template.SecurityModel == Business.SecurityModel.Advanced)
                    MSOTerm.TermGroupID = new Guid(ddlTermGroup.SelectedValue);
                else
                    MSOTerm.TermGroupID = _template.BasicSecurityTermGroupID;
            }
			MSOTerm.IsHeader = chkHeaderTerm.Checked;
			MSOTerm.ValidateOnSave = this.vos.Validate;
            MSOTerm.ValidationStatuses = this.vos.SelectedStatuses;

			MSOTerm.MSONameSpecified = chkMSONameDisplayed.Checked;
			MSOTerm.MSOName = txtMSOName.Text;

			MSOTerm.Address1NameSpecified = chkAddress1Displayed.Checked;
			MSOTerm.Address1Name = txtAddress1.Text;

			MSOTerm.Address2NameSpecified = chkAddress2Displayed.Checked;
			MSOTerm.Address2Name = txtAddress2.Text;

			MSOTerm.CityNameSpecified = chkCityDisplayed.Checked;
			MSOTerm.CityName = txtCity.Text;

			MSOTerm.StateNameSpecified = chkStateDisplayed.Checked;
			MSOTerm.StateName = txtState.Text;

			MSOTerm.ZipNameSpecified = chkZipDisplayed.Checked;
			MSOTerm.ZipName = txtZip.Text;

			MSOTerm.PhoneNameSpecified = chkPhoneDisplayed.Checked;
			MSOTerm.PhoneName = txtPhone.Text;
		}


		protected override List<string> ValidateForm()
		{
			List<string> rtn = new List<string>();

			if (chkMSONameDisplayed.Checked && string.IsNullOrEmpty(txtMSOName.Text))
				rtn.Add(string.Format("Since '{0}' is Checked the '{0}' Text is Required ...","Name"));

			if (chkAddress1Displayed.Checked && string.IsNullOrEmpty(txtAddress1.Text))
				rtn.Add(string.Format("Since '{0}' is Checked the '{0}' Text is Required ...", "Address1"));

			if (chkAddress2Displayed.Checked && string.IsNullOrEmpty(txtAddress2.Text))
				rtn.Add(string.Format("Since '{0}' is Checked the '{0}' Text is Required ...", "Address2"));

			if (chkCityDisplayed.Checked && string.IsNullOrEmpty(txtCity.Text))
				rtn.Add(string.Format("Since '{0}' is Checked the '{0}' Text is Required ...", "City"));

			if (chkStateDisplayed.Checked && string.IsNullOrEmpty(txtState.Text))
				rtn.Add(string.Format("Since '{0}' is Checked the '{0}' Text is Required ...", "State"));

			if (chkZipDisplayed.Checked && string.IsNullOrEmpty(txtZip.Text))
				rtn.Add(string.Format("Since '{0}' is Checked the '{0}' Text is Required ...", "Zip"));

			if (chkPhoneDisplayed.Checked && string.IsNullOrEmpty(txtPhone.Text))
				rtn.Add(string.Format("Since '{0}' is Checked the '{0}' Text is Required ...", "Phone"));


            //Make sure a Term Group is selected.

            if (trTermGroup.Visible == true && ddlTermGroup.SelectedValue == Guid.Empty.ToString())
                rtn.Add("A Tab must be selected.");

			return rtn;
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

		protected override void LoadValues()
		{
            Business.MSOTerm MSOTerm = Term as Business.MSOTerm;
			if (MSOTerm == null)
                throw new Exception(string.Format("Unable to cast _term \"{0}\" to a Business.MSOTerm object.", TermName));

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
            txtTermName.Text = TermName;
			chkHeaderTerm.Checked = MSOTerm.IsHeader;
			chkboxKeywordSearchable.Checked = (MSOTerm.KeywordSearchable ?? false);
			chkRequired.Checked = (MSOTerm.Required ?? false);
			//chkValidateOnSave.Checked = MSOTerm.ValidateOnSave;

			chkMSONameDisplayed.Checked = MSOTerm.MSONameSpecified ?? false;
			txtMSOName.Text = MSOTerm.MSOName;

			chkAddress1Displayed.Checked = MSOTerm.Address1NameSpecified ?? false;
			txtAddress1.Text = MSOTerm.Address1Name;

			chkAddress2Displayed.Checked = MSOTerm.Address2NameSpecified ?? false;
			txtAddress2.Text = MSOTerm.Address2Name;

			chkCityDisplayed.Checked = MSOTerm.CityNameSpecified ?? false;
			txtCity.Text = MSOTerm.CityName;

			chkStateDisplayed.Checked = MSOTerm.StateNameSpecified ?? false;
			txtState.Text = MSOTerm.StateName;

			chkZipDisplayed.Checked = MSOTerm.ZipNameSpecified ?? false;
			txtZip.Text = MSOTerm.ZipName;

			chkPhoneDisplayed.Checked = MSOTerm.PhoneNameSpecified ?? false;
			txtPhone.Text = MSOTerm.PhoneName;

			ShowHideFields();
		}

		protected override void ShowHideFields()
		{
			EnableDisableTextFields();
		}

		protected void EnableDisableTextFields()
		{
			string[] fields = { "MSO", "Address1", "Address2", "City", "State", "Zip", "Phone" };
			CheckBox checkbox = null;
			TextBox textbox = null;

			foreach (string field in fields)
			{
				switch (field)
				{
					case "MSO":
						checkbox = chkMSONameDisplayed;
						textbox = txtMSOName;
						break;

					case "Address1":
						checkbox = chkAddress1Displayed;
						textbox = txtAddress1;
						break;

					case "Address2":
						checkbox = chkAddress2Displayed;
						textbox = txtAddress2;
						break;

					case "City":
						checkbox = chkCityDisplayed;
						textbox = txtCity;
						break;

					case "State":
						checkbox = chkStateDisplayed;
						textbox = txtState;
						break;

					case "Zip":
						checkbox = chkZipDisplayed;
						textbox = txtZip;
						break;

					case "Phone":
						checkbox = chkPhoneDisplayed;
						textbox = txtPhone;
						break;
				}

				textbox.Enabled = checkbox.Checked;

			}
			
		}

		protected void chkMSONameDisplayed_CheckedChanged(object sender, EventArgs e)
		{
			ShowHideFields();
		}

		protected void chkAddress1Displayed_CheckedChanged(object sender, EventArgs e)
		{
			ShowHideFields();
		}

		protected void chkAddress2Displayed_CheckedChanged(object sender, EventArgs e)
		{
			ShowHideFields();
		}

		protected void chkCityDisplayed_CheckedChanged(object sender, EventArgs e)
		{
			ShowHideFields();
		}

		protected void chkStateDisplayed_CheckedChanged(object sender, EventArgs e)
		{
			ShowHideFields();
		}

		protected void chkZipDisplayed_CheckedChanged(object sender, EventArgs e)
		{
			ShowHideFields();
		}

		protected void chkPhoneDisplayed_CheckedChanged(object sender, EventArgs e)
		{
			ShowHideFields();
		}

	}
}
