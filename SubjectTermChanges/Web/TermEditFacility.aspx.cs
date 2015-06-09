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
	public partial class TermEditFacility : BaseTermEditPage
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

		protected override void OnPreRender(EventArgs e)
		{
			base.OnPreRender(e);
			RegisterScriptBlocks();
			chkEnforceSecurity.Attributes.Add("onclick", "javascript:return _kh_doSecurity();");
			HTMLBody().Attributes.Add("onload", "javascript: return _kh_doSecurity();");
            if (_ddlMapIndex.HasValue)
            {
                ddlMappedTerm.Items[_ddlMapIndex.Value].Value = GetMappedTermValue(_ddlMapIndex.Value, string.Empty);
                ddlMappedTerm.SelectedIndex = 0;
            }
            _ddlMapIndex = null;
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
            txtTermName.Text = TermName;
			LoadFacilityStatuses();
			LoadFacilityTypes();
			LoadProfileScreenViews();
			LoadSortOptions();

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
            Business.FacilityTerm facilityTerm = Term as Business.FacilityTerm;
			if (facilityTerm == null)
                throw new Exception(string.Format("Unable to cast _term \"{0}\" to a Business.FacilityTerm object.", TermName));

			TermName = txtTermName.Text;
            if (!IsComplexListField)
            {
                if (_template.SecurityModel == Business.SecurityModel.Advanced)
                    facilityTerm.TermGroupID = new Guid(ddlTermGroup.SelectedValue);
                else
                    facilityTerm.TermGroupID = _template.BasicSecurityTermGroupID;
            }
			facilityTerm.IsHeader = chkHeaderTerm.Checked;
			facilityTerm.Required = chkRequired.Checked;
			//facilityTerm.DefaultValueRequired = chkDefault.Checked;
			//facilityTerm.DefaultValue = txtDefaultValue.Text;
			//facilityTerm.Editable = chkEditable.Checked;
			//facilityTerm.ValidateOnSave = chkValidateOnSave.Checked;
            facilityTerm.ValidateOnSave = this.vos.Validate;
            facilityTerm.ValidationStatuses = this.vos.SelectedStatuses;

			facilityTerm.UseUserSecurity = chkEnforceSecurity.Checked;
			facilityTerm.MultiSelect = (rblProfileView.SelectedValue == "Multiple");
			facilityTerm.IncludeChildren = chkIncludeHierarchy.Checked;
			facilityTerm.FacilityStatus = (Business.FacilityStatusType)Enum.Parse(typeof(Business.FacilityStatusType), ddlFacilityStatus.SelectedValue);
			facilityTerm.SortField = (Business.FacilitySortField)Enum.Parse(typeof(Business.FacilitySortField), ddlSortBy.SelectedValue);

			facilityTerm.FacilityTypes.Clear();
			foreach (ListItem item in lstFacilityType.Items)
				if (item.Selected)
					facilityTerm.FacilityTypes.Add(int.Parse(item.Value));
			facilityTerm.KeywordSearchable = chkboxKeywordSearchable.Checked;
		}

		protected override List<string> ValidateForm()
		{
			List<string> rtn = new List<string>();
            //Make sure a Term Group is selected.

            if (trTermGroup.Visible == true && ddlTermGroup.SelectedValue == Guid.Empty.ToString())
                rtn.Add("A Tab must be selected.");
			return rtn;
		}

		protected override void LoadValues()
		{
            Business.FacilityTerm facilityTerm = Term as Business.FacilityTerm;
			if (facilityTerm == null)
                throw new Exception(string.Format("Unable to cast _term \"{0}\" to a Business.FacilityTerm object.", TermName));

			chkHeaderTerm.Checked = facilityTerm.IsHeader;
			chkRequired.Checked = (facilityTerm.Required ?? false);
			//chkDefault.Checked = facilityTerm.DefaultValueRequired;
			//txtDefaultValue.Text = facilityTerm.DefaultValue;
			//chkEditable.Checked = facilityTerm.Editable;
			//chkValidateOnSave.Checked = facilityTerm.ValidateOnSave;
			chkEnforceSecurity.Checked = (facilityTerm.UseUserSecurity ?? false);
			if (facilityTerm.MultiSelect.HasValue)
				rblProfileView.SelectedValue = ((bool)facilityTerm.MultiSelect ? "Multiple" : "Single");
			else
				rblProfileView.SelectedValue = "Single";
			chkIncludeHierarchy.Checked = (facilityTerm.IncludeChildren ?? false);

			ddlFacilityStatus.SelectedValue = facilityTerm.FacilityStatus.ToString("D");
			foreach (ListItem item in lstFacilityType.Items)
				item.Selected = facilityTerm.FacilityTypes.Contains(int.Parse(item.Value));
			chkboxKeywordSearchable.Checked = (facilityTerm.KeywordSearchable ?? false);

			ddlSortBy.SelectedValue = facilityTerm.SortField.ToString();
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
		protected override void ShowHideFields()
		{
			//ShowHideDefaultValue();
		}


		private void LoadFacilityStatuses()
		{
			ddlFacilityStatus.Items.Add(new ListItem("Open", "1"));
			ddlFacilityStatus.Items.Add(new ListItem("Closed", "2"));
			ddlFacilityStatus.Items.Add(new ListItem("All", "0"));
		}


		private void LoadFacilityTypes()
		{
			Kindred.Common.Facility.FacilityTypeSelector fts = new Kindred.Common.Facility.FacilityTypeSelector();
			lstFacilityType.DataSource = fts.SelectAllByOrganizationId(1);    //1 = Kindred
			lstFacilityType.DataTextField = "FacilityTypeName";
			lstFacilityType.DataValueField = "FacilityTypeID";
			lstFacilityType.DataBind();
		}

		private void LoadProfileScreenViews()
		{
			rblProfileView.Items.Add(new ListItem("Single", "Single"));
			rblProfileView.Items.Add(new ListItem("Multiple", "Multiple"));
		}


		private void LoadSortOptions()
		{
			Business.FacilitySortField[] sortFields = (Business.FacilitySortField[])Enum.GetValues(typeof(Business.FacilitySortField));
			foreach (Business.FacilitySortField sortField in sortFields)
			{
				ddlSortBy.Items.Add(new ListItem(Business.FacilitySort.DisplayValue(sortField), sortField.ToString()));
			}	
		}

		protected void RegisterScriptBlocks()
		{
			Type t = this.GetType();
			string scriptName = string.Empty;

			scriptName = "_kh_ScriptBlock";
			System.IO.StringWriter sw = new System.IO.StringWriter();
			sw.WriteLine("	function _kh_doSecurity()  ");
			sw.WriteLine("	{	");
			sw.WriteLine("		var chkIncludeHierarchy = document.getElementById('{0}');", chkIncludeHierarchy.ClientID);
			sw.WriteLine("		var chkEnforceSecurity = document.getElementById('{0}');", chkEnforceSecurity.ClientID);
			sw.WriteLine("		if (chkIncludeHierarchy && chkEnforceSecurity)");
			sw.WriteLine("		{");
			sw.WriteLine("			if (chkEnforceSecurity.checked == false)");
			sw.WriteLine("			{");
			sw.WriteLine("				chkIncludeHierarchy.disabled = true;");
			sw.WriteLine("				chkIncludeHierarchy.checked = true;");
			sw.WriteLine("			}");
			sw.WriteLine("			else");
			sw.WriteLine("			{");
			sw.WriteLine("				chkIncludeHierarchy.disabled = false;");
			sw.WriteLine("			}");
			sw.WriteLine("		}");
			sw.WriteLine("	}	");
			if (!ClientScript.IsClientScriptBlockRegistered(t, scriptName))
				ClientScript.RegisterClientScriptBlock(t, scriptName, sw.ToString(), true);

		}


	}
}
