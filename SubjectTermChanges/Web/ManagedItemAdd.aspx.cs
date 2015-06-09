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
using System.Collections.Generic;

namespace Kindred.Knect.ITAT.Web
{
	public partial class ManagedItemAdd : BaseSystemPage
	{
		List<string> _validationErrors;
		int _selectedFacilityId;
		Guid _selectedTemplate;


		protected override void OnInit(EventArgs e)
		{
			base.OnInit(e);
			_validationErrors = new List<string>();
		}

		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);

            panelDescription.Visible = false;
            litTemplateName.Text = string.Empty;
            litWhenToUse.Text = string.Empty;
            litWhenNotToUse.Text = string.Empty;
            litAdditionalInfo.Text = string.Empty;

            if (!IsPostBack)
            {
                if (_itatSystem != null)
                    ((StandardHeader)header).PageTitle = "Add " + _itatSystem.ManagedItemName;
                LoadTemplates();
                if (_itatSystem.HasOwningFacility ?? false)
                    LoadFacilities();
                else
                    trFacilities.Visible = false;
            }
            else
            {
                if (!string.IsNullOrEmpty(ddlTemplate.SelectedValue))
                {
                    Kindred.Knect.ITAT.Business.Template template = new Kindred.Knect.ITAT.Business.Template(new Guid(ddlTemplate.SelectedValue), Kindred.Knect.ITAT.Business.DefType.Final);
                    if (template != null)
                    {
                        litTemplateName.Text = template.Name;
                        if (template.UseDetailedDescription)
                        {
                            panelDescription.Visible = true;

                            litWhenToUse.Text = template.FindDetailedDescription(Business.DetailedDescription.DetailedDescriptionType.WhenToUse);
                            litWhenNotToUse.Text = template.FindDetailedDescription(Business.DetailedDescription.DetailedDescriptionType.WhenNotToUse);
                            litAdditionalInfo.Text = template.FindDetailedDescription(Business.DetailedDescription.DetailedDescriptionType.AdditionalInfo);

                            rowWhenToUse.Visible = !string.IsNullOrEmpty(litWhenToUse.Text);
                            rowWhenNotToUse.Visible = !string.IsNullOrEmpty(litWhenNotToUse.Text);
                            rowAdditionalInfo.Visible = !string.IsNullOrEmpty(litAdditionalInfo.Text);
                        }
                    }
                }
            }
		}

		private void ValidateForm()
		{
			try	{_selectedTemplate = new Guid(ddlTemplate.SelectedValue);}
			catch {_validationErrors.Add(string.Format("You must select a Template for this {0}.", _itatSystem.ManagedItemName));	}

			if (_itatSystem.HasOwningFacility ?? false)
				if (!int.TryParse(ddlFacility.SelectedValue, out _selectedFacilityId))
					_validationErrors.Add(string.Format("You must select an Owning Facility for this {0}.", _itatSystem.ManagedItemName));
		}


		private void RenderValidationErrors()
		{
			if (_validationErrors.Count > 0)
			{
				System.Text.StringBuilder sb = new System.Text.StringBuilder();
				sb.AppendFormat("Unable to save the {0} due to the following errors:\\n\\n", _itatSystem.ManagedItemName);
				foreach (string error in _validationErrors)
					sb.Append(error + "\\n");
				sb.Append("\\nPlease correct the errors and save again.");
				RegisterAlert(sb.ToString());
			}
		}



		private void LoadFacilities()
		{
			Business.FacilityCollection facilities = Business.FacilityCollection.FilteredFacilityList(_securityHelper.AllUserFacilities, _itatSystem.PrimaryFacility);
			List<Data.FacilityDataRow> sortedFacilities = facilities.SortedList(_itatSystem.PrimaryFacility.SortField);
			Helper.LoadListControl(ddlFacility, sortedFacilities, "SapIdPlusName", "FacilityId", string.Empty, true, "(Select a Facility)", string.Empty);
		}

		
		private void LoadTemplates()
		{
            if (_securityHelper == null)
                _securityHelper = new Business.SecurityHelper(_itatSystem);
            Helper.LoadListControl(ddlTemplate, Business.Template.GetTemplateListWithStatus(_itatSystem.ID, new short[] { (short)Business.TemplateStatusType.Active }, _securityHelper.UserRoles), "TemplateName", "TemplateID", string.Empty, true, "(Select a Template)", string.Empty);
		}


		protected void btnAdd_Command(object sender, CommandEventArgs e)
		{
				ValidateForm();
				if (_validationErrors.Count > 0)
				{
					RenderValidationErrors();
				}
				else
				{
					Business.ManagedItem managedItem;
					if (_itatSystem.HasOwningFacility ?? false)
						managedItem = Business.ManagedItem.Create(true, new Guid(ddlTemplate.SelectedValue), int.Parse(ddlFacility.SelectedValue));
					else
						managedItem = Business.ManagedItem.Create(true, new Guid(ddlTemplate.SelectedValue),null);

					bool bValidate = true;
					managedItem.FirstSave(true, bValidate);
					Response.Redirect(string.Format("ManagedItemProfile.aspx{0}",Utility.TextHelper.QueryString(true, Common.Names._QS_ITAT_SYSTEM_ID, _itatSystem.ID.ToString(), Common.Names._QS_MANAGED_ITEM_ID, managedItem.ManagedItemID.ToString())));
				}
		}



		internal override HtmlGenericControl HTMLBody()
		{
			return body;
		}
		
		internal override Control ResizablePanel()
		{
			return null;
		}

	}
}
