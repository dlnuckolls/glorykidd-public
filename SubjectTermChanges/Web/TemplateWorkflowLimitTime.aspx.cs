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
	public partial class TemplateWorkflowLimitTime : BaseTemplatePage
	{
        private const string _KH_VS_WORKFLOWID = "_kh_vs_WorkflowId";
		private const string _KH_VS_WORKFLOWEDITMODE = "_kh_vs_WorkflowEditMode";
        private Business.Workflow _workflow;
        private Guid _workflowId;
		private EditMode _workflowEditMode;

		protected override TemplateHeader HeaderControl()
		{
			return null;
		}

		internal override HtmlGenericControl HTMLBody()
		{
			return htmlBody;
		}

		internal override Control ResizablePanel()
		{
			return divEditor;
		}

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            ClientScript.RegisterClientScriptInclude(this.GetType(), "_kh_jsITAT", "Scripts/itat.js");
        }

		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);
			if (!IsPostBack)
			{
				GetContextData();
			}
			_workflow = _template.FindWorkflow(_workflowId);
			if (!IsPostBack)
			{
				chkLimitTime.Checked = (_workflow.UseFunction ?? false);
				LoadValues();
				chkLimitTime.Focus();
			}
		}


        protected override void LoadViewState(object savedState)
        {
            base.LoadViewState(savedState);
            _workflowId = (Guid)ViewState[_KH_VS_WORKFLOWID];
				_workflowEditMode = (EditMode)ViewState[_KH_VS_WORKFLOWEDITMODE];
        }


        protected override object SaveViewState()
        {
            ViewState[_KH_VS_WORKFLOWID] = _workflowId;
				ViewState[_KH_VS_WORKFLOWEDITMODE] = _workflowEditMode;
            return base.SaveViewState();
        }

        private void GetContextData()
        {
				_workflowId = (Guid)Context.Items[Common.Names._CNTXT_WorkflowId];
				_workflowEditMode = (EditMode)Context.Items[Common.Names._CNTXT_WorkflowEditMode];
        }

		private void LoadValues()
		{
			txtDays.Text = (_workflow.DaysAfterWorkflowEntry ?? 0).ToString();
			Business.Message message = _workflow.RevertMessage;  //assume only one message
			txtSubject.Text = message.Subject;
			edt.Html = (string.IsNullOrEmpty(message.Text) ? Helper.DefaultEditorHtml(edt) : Business.Term.SubstituteTermIDs(_template, message.Text));
			Helper.LoadRoles(lstRecipients, _itatSystem, Business.RoleType.Distribution, message.Recipients);

            if (!(_itatSystem.HasOwningFacility ?? false) && _itatSystem.AllowNotificationFilterFacility)
            {
                List<Business.Term> facilityTerms = _template.BasicTerms.FindAll(t => t.TermType == Business.TermType.Facility || (t.TransformTermType.HasValue && t.TransformTermType.Value == Business.TermType.Facility));
                string facilityTermName = string.Empty;
                if (message.FilterFacilityTermID.HasValue)
                    facilityTermName = _template.FindTermName(message.FilterFacilityTermID.Value, null);
                Helper.LoadListControl(ddlFilterFacility, facilityTerms, "Name", "Name", facilityTermName, true, "(Select a Facility Term)", "");
                row_ddlFilterFacility.Visible = true;
            }
            else
            {
                ddlFilterFacility.SelectedIndex = -1;
                row_ddlFilterFacility.Visible = false;
            }

			SetControlStatus(chkLimitTime.Checked);
		}


		private void SetControlStatus(bool enabled)
		{
			fldNotification.Disabled = !enabled;
			btnClearEditors.Disabled = !enabled;
			lstRecipients.Enabled = enabled;
			txtDays.Enabled = enabled;
			txtSubject.Enabled = enabled;
			edt.Editable = enabled;
			edt.Enabled = enabled;
			edt.EnableContextMenus = enabled;
            ddlFilterFacility.Enabled = enabled;
		}


		protected override void OnPreRender(EventArgs e)
		{
			if (edt.Visible)
			{
				Helper.RegisterParagraphWrapperScript(this);
				Telerik.WebControls.RadEditorUtils.ToolbarDropDown tdd = (Telerik.WebControls.RadEditorUtils.ToolbarDropDown)edt.Toolbars["DynamicToolbar"].Tools["Insert Term"];
				Helper.InitializeToolBarItems(this, tdd);
				Helper.AddSpecialToolBarItems(this, tdd);
				Helper.AddTermsToolBarItems(this, tdd, _template.BasicTerms);
			}
			base.OnPreRender(e);
		}


		protected void btnOK_Command(object sender, CommandEventArgs e)
		{
			ReturnToCaller(true);
		}

		protected void btnCancel_Command(object sender, CommandEventArgs e)
		{
			ReturnToCaller(false);
		}


		protected void chkLimitTime_CheckedChanged(object sender, EventArgs e)
		{
			LoadValues();
		}



		private void ReturnToCaller(bool updateValues)
		{
			if (updateValues)
			{
				if (ValidateForm())
				{
					bool dataChanged;
					SetValues(out dataChanged);
					Context.Items[Common.Names._CNTXT_IsChanged] = dataChanged;
				}
				else
					return;
			}
			Context.Items[Common.Names._CNTXT_ScheduledEventUpdated] = true;
			Context.Items[Common.Names._CNTXT_Template] = _template;
			Context.Items[Common.Names._CNTXT_WorkflowId] = _workflowId;
			Context.Items[Common.Names._CNTXT_WorkflowEditMode] = _workflowEditMode;
			Server.Transfer("TemplateWorkflowMain.aspx");
		}

		private bool ValidateForm()
		{
			System.Text.StringBuilder sb = new System.Text.StringBuilder();
			int days;

			if (!int.TryParse(txtDays.Text, out days))
				sb.Append("The # of Days from Workflow Entry must be numeric.\\n");
			
			bool isValid = (sb.Length == 0);
			if (!isValid)
				RegisterAlert(sb.ToString());
			return isValid;
		}


		private void SetValues(out bool dataChanged)
		{
			dataChanged = false;

			if (!_workflow.UseFunction.HasValue || ((bool)_workflow.UseFunction != chkLimitTime.Checked))
			{
				dataChanged = true;
				_workflow.UseFunction = chkLimitTime.Checked;
			}

			if (!_workflow.DaysAfterWorkflowEntry.HasValue || (int)_workflow.DaysAfterWorkflowEntry != int.Parse(txtDays.Text))
			{
				dataChanged = true;
				_workflow.DaysAfterWorkflowEntry = int.Parse(txtDays.Text);
			}

			if (_workflow.UseFunction ?? false)
			{
				Business.Message message = _workflow.RevertMessage;
				if (message.Subject != txtSubject.Text)
				{
					dataChanged = true;
					message.Subject = txtSubject.Text;
				}

				if (message.Text != edt.Xhtml)
				{
					dataChanged = true;
					message.Text = Business.Term.SubstituteTermNames(_template, edt.Xhtml);
				}
				dataChanged = Helper.UpdateList(lstRecipients, message.Recipients) || dataChanged;

                if (ddlFilterFacility.SelectedIndex > 0)
                {
                    string storedFilterFacility = string.Empty;
                    if (message.FilterFacilityTermID.HasValue)
                    {
                        storedFilterFacility = _template.FindTerm(message.FilterFacilityTermID.Value).Name;
                    }
                    if (storedFilterFacility != ddlFilterFacility.SelectedValue)
                    {
                        dataChanged = true;
                        message.FilterFacilityTermID = _template.FindTerm(ddlFilterFacility.SelectedValue).ID;
                    }
                }
                else
                {
                    if (message.FilterFacilityTermID.HasValue)
                    {
                        dataChanged = true;
                        message.FilterFacilityTermID = null;
                    }
                }
			}
			else
				_workflow.RevertMessage = null;
		}



	}
}
