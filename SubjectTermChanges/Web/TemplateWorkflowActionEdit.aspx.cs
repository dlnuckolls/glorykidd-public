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
	public partial class TemplateWorkflowActionEdit : BaseTemplatePage
	{
		private Business.Action _action;
		private int _stateIndex;
		private int _actionIndex;
		private EditMode _editMode;
		private EditMode _workflowEditMode;
        private Guid _workflowId;
        private Business.Workflow _workflow;
		private Guid _selectedTermGroupId;
        private List<Business.State> _orgStates;

		//private const string _KH_VS_ACTION = "_kh_vs_Action";
		private const string _KH_VS_ACTIONINDEX = "_kh_vs_ActionIndex";
		private const string _KH_VS_EDITMODE = "_kh_vs_EditMode";
		private const string _KH_VS_WORKFLOWEDITMODE = "_kh_vs_WorkflowEditMode";
		private const string _KH_VS_STATEINDEX = "kh_vs_StateIndex";
        private const string _KH_VS_WORKFLOWID = "_kh_vs_WorkflowId";
		private const string _KH_VS_SELECTEDTERMGROUPID = "_kh_vs_SelectedTermGroupId";
        private const string _KH_VS_ORGSTATES = "_kh_vs_OrgStates";


		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);
			if (!IsPostBack)
			{
				GetContextData();
				((StandardHeader)header).PageTitle = string.Format("Edit Action:  {0} - {1}", _workflow.States[_stateIndex].Name,  _action.ButtonText);
				LoadValues();
			}
            else
                rowConfirmationText.Style["display"] = (chkRequiresConfirmation.Checked ? "block" : "none");
        }


		protected override void OnPreRender(EventArgs e)
		{
            base.OnPreRender(e);
			RegisterClearListBoxClientScript(lstPerformers);
			RegisterClearListBoxClientScript(lstRecipients);
			if (edt.Visible)
			{
				Helper.RegisterParagraphWrapperScript(this);
				Telerik.WebControls.RadEditorUtils.ToolbarDropDown tdd = (Telerik.WebControls.RadEditorUtils.ToolbarDropDown)edt.Toolbars["DynamicToolbar"].Tools["Insert Term"];
				Helper.InitializeToolBarItems(this, tdd);
				Helper.AddSpecialToolBarItems(this, tdd);
				Helper.AddTermsToolBarItems(this, tdd, _template.BasicTerms);
			}
            RegisterChildControlEventHandlers();
		}


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

		protected override void LoadViewState(object savedState)
		{
			base.LoadViewState(savedState);		
			_actionIndex = (int)ViewState[_KH_VS_ACTIONINDEX];
			_stateIndex = (int)ViewState[_KH_VS_STATEINDEX];
			_editMode = (EditMode)ViewState[_KH_VS_EDITMODE];
			_workflowEditMode = (EditMode)ViewState[_KH_VS_WORKFLOWEDITMODE];
            _workflowId = (Guid)ViewState[_KH_VS_WORKFLOWID];
            _workflow = _template.FindWorkflow(_workflowId);
            _action = _workflow.States[_stateIndex].Actions[_actionIndex];   // (Business.Action)ViewState[_KH_VS_ACTION];
			if (_template.SecurityModel == Business.SecurityModel.Advanced)
			{
				_selectedTermGroupId = (Guid)ViewState[_KH_VS_SELECTEDTERMGROUPID];
			}
            _orgStates = (List<Business.State>)ViewState[_KH_VS_ORGSTATES];
        }


		protected override object SaveViewState()
		{
			ViewState[_KH_VS_STATEINDEX] = _stateIndex;
			//ViewState[_KH_VS_ACTION] = _action;
			ViewState[_KH_VS_ACTIONINDEX] = _actionIndex;
			ViewState[_KH_VS_EDITMODE] = _editMode;
			ViewState[_KH_VS_WORKFLOWEDITMODE] = _workflowEditMode;
			ViewState[_KH_VS_WORKFLOWID] = _workflowId;
			if (_template.SecurityModel == Business.SecurityModel.Advanced)
			{
				ViewState[_KH_VS_SELECTEDTERMGROUPID] = _selectedTermGroupId;
			}
            ViewState[_KH_VS_ORGSTATES] = _orgStates;
            return base.SaveViewState();
		}


		private void LoadValues()
		{
			txtButtonText.Text = _action.ButtonText;
			Helper.LoadListControl(ddlTargetState, _workflow.States, "Name", "Name", _action.TargetState);

			Helper.LoadRoles(lstPerformers, _itatSystem, Business.RoleType.Security, _action.Performers);

			Business.Message message = _action.Messages[0];
			txtSubject.Text = message.Subject;
			Helper.LoadRoles(lstRecipients, _itatSystem, Business.RoleType.Distribution, message.Recipients);
			edt.Html = Business.Term.SubstituteTermIDs(_template, message.Text);

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

            if (_itatSystem.AllowActionConfirmation)
            {
                rowRequiresConfirmation.Visible = true;
                rowConfirmationText.Visible = true;
                chkRequiresConfirmation.Checked = _action.RequiresConfirmation;
                rowConfirmationText.Style["display"] = (chkRequiresConfirmation.Checked ? "block" : "none");
                txtConfirmationText.Text = _action.ConfirmationText;
            }
		}

        private void RegisterChildControlEventHandlers()
        {
            string function = @"function toggleVisible(chk, obj) {
					var ck = document.getElementById(chk);
					if (ck)
					{
						var el = document.getElementById(obj);
						if (ck.checked) {
							el.style.display = 'block';
						}
						else {
							el.style.display = 'none';
						}
					}
				}";

            if (!this.Page.ClientScript.IsClientScriptBlockRegistered(typeof(TemplateWorkflowActionEdit), "toggleVisible"))
                this.Page.ClientScript.RegisterClientScriptBlock(typeof(TemplateWorkflowActionEdit), "toggleVisible", function, true);

            chkRequiresConfirmation.Attributes["onclick"] = string.Format("javascript:toggleVisible('{0}', '{1}'); javascript:_kh_doResize();", chkRequiresConfirmation.ClientID, rowConfirmationText.ClientID);
        }

		protected void btnOK_Command(object sender, CommandEventArgs e)
		{
            string validName = Business.State.ValidName("Button Text", txtButtonText.Text);
            if (validName.Length > 0)
                RegisterAlert(validName);
            else
                if (string.IsNullOrEmpty(txtButtonText.Text))
                    RegisterAlert("Please enter the Button Text.");
                else
                    if (_itatSystem.AllowActionConfirmation && chkRequiresConfirmation.Checked && string.IsNullOrEmpty(txtConfirmationText.Text))
                        RegisterAlert("Please enter the Confirmation Text.");
                    else
    			        ReturnToCaller(true);
		}

		protected void btnCancel_Command(object sender, CommandEventArgs e)
		{
			if (_editMode == EditMode.Add)
				_workflow.States[_stateIndex].Actions.RemoveAt(_actionIndex);
			ReturnToCaller(false);
		}


		private void ReturnToCaller(bool updateValues)
		{
			if (updateValues)
			{
				bool dataChanged;
				SetValues(out dataChanged);
                _workflow.States[_stateIndex].Actions[_actionIndex] = _action;
				Context.Items[Common.Names._CNTXT_IsChanged] = dataChanged;
			}
			Context.Items[Common.Names._CNTXT_FromActionEdit] = true;
			Context.Items[Common.Names._CNTXT_StateIndex] = _stateIndex;
			Context.Items[Common.Names._CNTXT_Template] = _template;
			Context.Items[Common.Names._CNTXT_WorkflowId] = _workflowId;
			Context.Items[Common.Names._CNTXT_WorkflowEditMode] = _workflowEditMode;
			if (_template.SecurityModel == Business.SecurityModel.Advanced)
			{
				Context.Items[Common.Names._CNTXT_TermGroupId] = _selectedTermGroupId;
			}
            Context.Items[Common.Names._CNTXT_OrgStates] = _orgStates;
            Server.Transfer("TemplateWorkflowStateEdit.aspx");
		}


		private void SetValues(out bool dataChanged)
		{
			dataChanged = false;

			if (_action.ButtonText != txtButtonText.Text)
			{
			   dataChanged = true;
			   _action.ButtonText = txtButtonText.Text;
			}

			if (_action.TargetState != ddlTargetState.SelectedValue)
			{
                dataChanged = true;
				Business.State state = _workflow.FindState(ddlTargetState.SelectedValue);
				if (state != null)
					_action.TargetStateID = state.ID;
			}

			dataChanged = Helper.UpdateList(lstPerformers, _action.Performers) || dataChanged;

			Business.Message message = _action.Messages[0];

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

            if (_itatSystem.AllowActionConfirmation)
            {
                if (_action.RequiresConfirmation != chkRequiresConfirmation.Checked)
                {
                    dataChanged = true;
                    _action.RequiresConfirmation = chkRequiresConfirmation.Checked;
                }

                if (_action.RequiresConfirmation)
                {
                    if (_action.ConfirmationText != txtConfirmationText.Text)
                    {
                        dataChanged = true;
                        _action.ConfirmationText = txtConfirmationText.Text;
                    }
                }
            }

			dataChanged = Helper.UpdateList(lstRecipients, message.Recipients) || dataChanged;
		}
	
		private void GetContextData()
		{
			_editMode = (EditMode)Context.Items[Common.Names._CNTXT_EditMode];
			_workflowEditMode = (EditMode)Context.Items[Common.Names._CNTXT_WorkflowEditMode];
			_actionIndex = (int)Context.Items[Common.Names._CNTXT_ActionIndex];
			_stateIndex = (int)Context.Items[Common.Names._CNTXT_StateIndex];
			_workflowId = (Guid)Context.Items[Common.Names._CNTXT_WorkflowId];
            _workflow = _template.FindWorkflow(_workflowId);
            if (_template.SecurityModel == Business.SecurityModel.Advanced)
            {
                _selectedTermGroupId = (Guid)Context.Items[Common.Names._CNTXT_TermGroupId];
            }
			_action = _workflow.States[_stateIndex].Actions[_actionIndex];
            _orgStates = (List<Business.State>)Context.Items[Common.Names._CNTXT_OrgStates];
            if (_action == null)
				throw new Exception("Action not found.");
		}

		private void RegisterClearListBoxClientScript(ListBox lst)
		{
			Type t = this.GetType();
			string clientId = lst.ClientID;
			string scriptName = string.Format("_kh_ClearSelected_{0}_Performers", clientId);
			if (!ClientScript.IsClientScriptBlockRegistered(t, scriptName))
			{
				System.IO.StringWriter swClearSelectedItems = new System.IO.StringWriter();
				swClearSelectedItems.WriteLine("function ClearSelected_{0}_Items()", clientId);
				swClearSelectedItems.WriteLine("{");
				swClearSelectedItems.WriteLine("	var o = document.getElementById('{0}'); ", clientId);
				swClearSelectedItems.WriteLine("	for (i = 0; i < o.options.length; i++)");
				swClearSelectedItems.WriteLine("	{");
				swClearSelectedItems.WriteLine("			o.options[i].selected = false;");
				swClearSelectedItems.WriteLine("	}");
				swClearSelectedItems.WriteLine("}");
				ClientScript.RegisterClientScriptBlock(t, scriptName, swClearSelectedItems.ToString(), true);
			}
		}


	}
}
