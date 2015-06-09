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
	public partial class TemplateWorkflowStateEdit : BaseTemplatePage
	{
        private Guid _workflowId;
        private Business.Workflow _workflow; 
        private Business.State _state;
		private Guid _currentTermGroupId = Guid.Empty;
		private int _stateIndex;
		private EditMode _editMode;
		private EditMode _workflowEditMode;
        private List<Business.State> _orgStates;

		//private const string _KH_VS_STATE = "_kh_vs_State";
		private const string _KH_VS_STATEINDEX = "_kh_vs_StateIndex";
		private const string _KH_VS_EDITMODE = "_kh_vs_EditMode";
		private const string _KH_VS_WORKFLOWEDITMODE = "_kh_vs_WorkflowEditMode";
        private const string _KH_VS_WORKFLOWID = "_kh_vs_WorkflowId";
        private const string _KH_VS_ORGSTATES = "_kh_vs_OrgStates";
		private const string _KH_VS_CURRENTTERMGROUPID = "_kh_vs_CurrentTermGroupId";

		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);
			if (!IsPostBack)
			{
				GetContextData();
				((StandardHeader)header).PageTitle = "Edit State - " + _state.Name;
				LoadValues();
				SetMoveUpDownButtonEvents();
			}
		}


		protected override void OnPreRender(EventArgs e)
		{
			base.OnPreRender(e);
			SuppressChangeNotification(grdActions);
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
			return pnlGridBody;
		}

		protected override void LoadViewState(object savedState)
		{
			base.LoadViewState(savedState);
            _workflowId = (Guid)ViewState[_KH_VS_WORKFLOWID];
            _workflow = _template.FindWorkflow(_workflowId);
			_stateIndex = (int)ViewState[_KH_VS_STATEINDEX];
			_editMode = (EditMode)ViewState[_KH_VS_EDITMODE];
			_workflowEditMode = (EditMode)ViewState[_KH_VS_WORKFLOWEDITMODE];
			_state = _workflow.States[_stateIndex]; //(Business.State)ViewState[_KH_VS_STATE];
            _orgStates = (List<Business.State>)ViewState[_KH_VS_ORGSTATES];
			if (_template.SecurityModel == Kindred.Knect.ITAT.Business.SecurityModel.Advanced)
			{
				_currentTermGroupId = (Guid)ViewState[_KH_VS_CURRENTTERMGROUPID];
			}
		}


		protected override object SaveViewState()
		{
			//ViewState[_KH_VS_STATE] = _state;
			ViewState[_KH_VS_STATEINDEX] = _stateIndex;
			ViewState[_KH_VS_EDITMODE] = _editMode;
			ViewState[_KH_VS_WORKFLOWEDITMODE] = _workflowEditMode;
            ViewState[_KH_VS_WORKFLOWID] = _workflowId;
            ViewState[_KH_VS_ORGSTATES] = _orgStates;
			if (_template.SecurityModel == Kindred.Knect.ITAT.Business.SecurityModel.Advanced)
			{
				ViewState[_KH_VS_CURRENTTERMGROUPID] = _currentTermGroupId;
			}
			return base.SaveViewState();
		}


		protected void grd_RowCreated(object sender, GridViewRowEventArgs e)
		{
			if (e.Row.RowType == DataControlRowType.DataRow)
			{
				//Assign each row an ID containing the RowIndex
				string rowIndex = e.Row.RowIndex.ToString();
				e.Row.ID = "R" + rowIndex;
				//Set the command arguments for the Edit and Delete buttons
				((LinkButton)e.Row.Cells[2].Controls[0]).CommandArgument = rowIndex;   //Edit button CommandArgument
				((LinkButton)e.Row.Cells[3].Controls[0]).CommandArgument = rowIndex;   //Delete button CommandArgument

				//Set up client-side script to prompt the user if they click the Delete button
				((LinkButton)e.Row.Cells[3].Controls[0]).OnClientClick = "return confirm('Are you sure you want to delete this action?');";
			}
		}


		protected void grd_RowCommand(object sender, GridViewCommandEventArgs e)
		{
			int rowIndex = Convert.ToInt32(e.CommandArgument);
			switch (e.CommandName)
			{
				case Common.Names._GRID_COMMAND_EditRow:
					EditAction((Business.Action)(_state.Actions[rowIndex]), rowIndex, false);
					break;

				case Common.Names._GRID_COMMAND_DeleteRow:
					DeleteAction(rowIndex);
					IsChanged = true;
					break;

				case Common.Names._GRID_COMMAND_SingleClick:
				case Common.Names._GRID_COMMAND_DoubleClick:
					SetMoveUpDownButtonEvents();
					break;

				default:
					break;
			}
		}


		private void DeleteAction(int rowIndex)
		{
			_state.Actions.RemoveAt(rowIndex);
			if (grdActions.SelectedIndex == rowIndex)
				grdActions.SelectedIndex = -1;
			LoadGrid();
		}

		private void EditAction(Kindred.Knect.ITAT.Business.Action action, int rowIndex, bool newAction)
		{
			bool dataChanged;
			SetValues(out dataChanged);   //Update values before transferring to ActionEdit page
			Context.Items[Common.Names._CNTXT_Template] = _template;
			//Context.Items[Common.Names._CNTXT_Action] = action;
			Context.Items[Common.Names._CNTXT_ActionIndex] = rowIndex;
			Context.Items[Common.Names._CNTXT_StateIndex] = _stateIndex;	//pass to "child" page so that it can be passed back when control returns to this page
			if (_template.SecurityModel == Business.SecurityModel.Advanced)
			{
				Context.Items[Common.Names._CNTXT_TermGroupId] = _currentTermGroupId; //pass to "child" page so that it can be passed back when control returns to this page			
			}
			Context.Items[Common.Names._CNTXT_WorkflowId] = _workflowId;
			Context.Items[Common.Names._CNTXT_EditMode] = (newAction ? EditMode.Add : EditMode.Edit);
			Context.Items[Common.Names._CNTXT_WorkflowEditMode] = _workflowEditMode;
            Context.Items[Common.Names._CNTXT_OrgStates] = _orgStates;
            Server.Transfer("TemplateWorkflowActionEdit.aspx");
		}

		protected void btnAddAction_Command(object sender, CommandEventArgs e)
		{
			Business.Action action = new Kindred.Knect.ITAT.Business.Action(_workflow);
			action.ButtonText = "";
			_state.Actions.Add(action);
			EditAction(action, _state.Actions.Count - 1, true);
		}


		private void LoadGrid()
		{
			grdActions.DataSource = _state.Actions;
			grdActions.DataBind();
		}

		private void LoadValues()
		{
			txtName.Text = _state.Name;
			chkBase.Checked = (_state.IsBase ?? false);

            if (!(_itatSystem.HasContent ?? false) || (!_template.CanGenerateDocument && !Template.CanGenerateUserDocuments))
			{
				chkDraft.Visible = false;
				chkDraft.Checked = false;
			}
			else
				chkDraft.Checked = (_state.IsDraft ?? false);
			chkExit.Checked = (_state.IsExit ?? false);
			chkRequiresValidation.Checked = (_state.RequiresValidation ?? false);
			
			Helper.LoadListControl(ddlStatus, _itatSystem.Statuses, "Name", "Name", _state.Status, true, "(Select a Status)", string.Empty);

			if (_template.SecurityModel == Kindred.Knect.ITAT.Business.SecurityModel.Advanced)
			{
				trTermGroup.Visible = true;

                List<Business.TermGroup> termGroups = new List<Business.TermGroup>();
                foreach (Business.TermGroup tg in _template.GetTermGroups(Business.TermGroup.TermGroupType.AdvancedBasicTerm))
                {
                    termGroups.Add(tg);
                }

                foreach (Business.ComplexList complexList in _template.ComplexLists)
                {
                    termGroups.Add(_template.FindTermGroup(complexList.TermGroupID));
                }

				if (_currentTermGroupId != Guid.Empty)
				{
                    Helper.LoadListControl(ddlTermGroup, termGroups, "Name", "ID", _currentTermGroupId.ToString(), true, "(Select a Tab or Complex List)", Guid.Empty.ToString());
					InitializeRoleSelectors(_state.StateTermGroups.Find(stg => stg.TermGroupID == _currentTermGroupId));   //StateTermGroups[0]);
				}
				else
				{
                    Helper.LoadListControl(ddlTermGroup, termGroups, "Name", "ID", Guid.Empty.ToString(), true, "(Select a Tab or Complex List)", Guid.Empty.ToString());
				}
			}
			else
			{
				trTermGroup.Visible = false;
				InitializeRoleSelectors(_state.StateTermGroups[0]);
			}
			LoadGrid();
		}


		private void InitializeRoleSelectors(Business.StateTermGroup stateTermGroup)
		{
			rsEditors.Initialize(stateTermGroup.Editors);
			rsViewers.Initialize(stateTermGroup.Viewers);
			rsAttachmentRemovers.Initialize(stateTermGroup.AttachmentRemovers);
			rsScannedAttachmentRemovers.Initialize(stateTermGroup.ScannedAttachmentRemovers);
		}


		protected void ddlTermGroupSelectedIndexChanged(object sender, EventArgs e)
		{
			if (_currentTermGroupId != Guid.Empty)
			{
				bool dataChanged = false;
				SetValues(out dataChanged);
				IsChanged = IsChanged || dataChanged;
			}
			_currentTermGroupId = new Guid(ddlTermGroup.SelectedValue);
			InitializeRoleSelectors(_state.GetTermGroup(_currentTermGroupId));
		}

		protected void btnOK_Command(object sender, CommandEventArgs e)
		{
            string validStateName = Business.State.ValidName("State Name", txtName.Text);
            if (validStateName.Length > 0)
                RegisterAlert(validStateName);
            else
                if (ddlStatus.SelectedValue == "")
                    RegisterAlert("A status must be selected.");
                else
                    ReturnToCaller(true);
		}

		protected void btnCancel_Command(object sender, CommandEventArgs e)
		{
			if (_editMode == EditMode.Add)
				_workflow.States.RemoveAt(_stateIndex);
			ReturnToCaller(false);
		}


		private void ReturnToCaller(bool updateValues)
		{
			if (updateValues)
			{
				bool dataChanged;
				SetValues(out dataChanged);
				_workflow.States[_stateIndex] = _state;
                if (_editMode == EditMode.Add)
					Context.Items[Common.Names._CNTXT_IsChanged] = true;
				else
					Context.Items[Common.Names._CNTXT_IsChanged] = dataChanged;
			}
			else
			{
				if (IsChangedInitial)
					Context.Items[Common.Names._CNTXT_IsChanged] = true;
			}

            Context.Items[Common.Names._CNTXT_WorkflowEditMode] = _workflowEditMode;
			Context.Items[Common.Names._CNTXT_Template] = _template;
            Context.Items[Common.Names._CNTXT_WorkflowId] = _workflowId;
            Context.Items[Common.Names._CNTXT_FromWorkflowStateEdit] = "True";
            Context.Items[Common.Names._CNTXT_OrgStates] = _orgStates;
			Server.Transfer("TemplateWorkflowMain.aspx");
		}


		private void SetValues(out bool dataChanged)
		{
			dataChanged = false;

			if (_state.Name != txtName.Text)
			{
				dataChanged = true;
				_state.Name = txtName.Text;
			}

			if (_state.Status != ddlStatus.SelectedValue)
			{
				dataChanged = true;
				_state.Status = ddlStatus.SelectedValue;
			}

			if (_state.IsBase != chkBase.Checked)
			{
				dataChanged = true;
				_state.IsBase = chkBase.Checked;
			}

			if (_state.IsDraft != chkDraft.Checked)
			{
				dataChanged = true;
				_state.IsDraft = chkDraft.Checked;
			}

			if (_state.RequiresValidation != chkRequiresValidation.Checked)
			{
				dataChanged = true;
				_state.RequiresValidation = chkRequiresValidation.Checked;
			}

			if (_state.IsExit != chkExit.Checked)
			{
				dataChanged = true;
				_state.IsExit = chkExit.Checked;
			}

			Business.StateTermGroup stateTermGroup = null;
			if (_template.SecurityModel == Kindred.Knect.ITAT.Business.SecurityModel.Advanced)
			{
				if (_currentTermGroupId != null)
					stateTermGroup = _state.GetTermGroup(_currentTermGroupId);
			}
			else
			{
				stateTermGroup = _state.StateTermGroups[0];
			}
			//if (stateTermGroup == null)
			//	throw new Exception("stateTermGroup is null.  SecurityModel=" + _template.SecurityModel.ToString());
			if (stateTermGroup != null)
				dataChanged = UpdateRoles(stateTermGroup) || dataChanged;
		}


		private bool UpdateRoles(Business.StateTermGroup stateTermGroup)
		{
			bool dataHasChanged = false;

			if (rsEditors.Update())
			{
				stateTermGroup.Editors = rsEditors.SelectedRoles;
				dataHasChanged = true;
			}

			if (rsViewers.Update())
			{
				stateTermGroup.Viewers = rsViewers.SelectedRoles;
				dataHasChanged = true;
			}

			if (rsAttachmentRemovers.Update())
			{
				stateTermGroup.AttachmentRemovers = rsAttachmentRemovers.SelectedRoles;
				dataHasChanged = true;
			}

			if (rsScannedAttachmentRemovers.Update())
			{
				stateTermGroup.ScannedAttachmentRemovers = rsScannedAttachmentRemovers.SelectedRoles;
				dataHasChanged = true;
			}

			return dataHasChanged;
		}




		private void GetContextData()
		{
			if (Context.Items[Common.Names._CNTXT_FromActionEdit] != null)
			{
				//returning from TemplateWorkflowActionEdit.aspx
				_template = (Business.Template)Context.Items[Common.Names._CNTXT_Template];
				if (Context.Items[Common.Names._CNTXT_IsChanged] != null)
				{
					if ((bool)Context.Items[Common.Names._CNTXT_IsChanged])
					{
						IsChanged = true;
					}
				}
				if (_template.SecurityModel == Business.SecurityModel.Advanced)
				{
					_currentTermGroupId = (Guid)Context.Items[Common.Names._CNTXT_TermGroupId];  // get the TermGroup that was passed to TemplateWorkflowActionEdit.aspx 
				}
			}
			else
			{
				//coming from TemplateWorkflowMain.aspx
				_editMode = (EditMode)Context.Items[Common.Names._CNTXT_EditMode];
			}
            _orgStates = (List<Business.State>)Context.Items[Common.Names._CNTXT_OrgStates];
            _workflowId = (Guid)Context.Items[Common.Names._CNTXT_WorkflowId];
            _workflow = _template.FindWorkflow(_workflowId);
			_workflowEditMode = (EditMode)Context.Items[Common.Names._CNTXT_WorkflowEditMode];
			_stateIndex = (int)Context.Items[Common.Names._CNTXT_StateIndex];   // get the State object that was passed to TemplateWorkflowActionEdit.aspx
			_state = _workflow.States[_stateIndex];
			if (_state == null)
				throw new Exception("State not found.");
		}


		private void SwapRows(int selectedRow, int otherRow)
		{
			_state.Actions.Reverse(Math.Min(selectedRow, otherRow), 2);
			LoadGrid();
			grdActions.SelectedIndex = otherRow;
			SetMoveUpDownButtonEvents();
			IsChanged = true;
		}


		private void SetMoveUpDownButtonEvents()
		{
			int selectedRow = grdActions.SelectedIndex;
			if (selectedRow > 0) //enable move up button
			{
				imgMoveUp.ImageUrl = "~/Images/MoveUp.gif";
				imgMoveUp.Enabled = true;
				imgMoveUp.Style["cursor"] = "pointer";
			}
			else
			{
				imgMoveUp.ImageUrl = "~/Images/MoveUpDisabled.gif";
				imgMoveUp.Enabled = false;
				imgMoveUp.Style["cursor"] = "default";
			}

			if (selectedRow > -1 && selectedRow < grdActions.Rows.Count - 1)
			{
				imgMoveDown.ImageUrl = "~/Images/MoveDown.gif";
				imgMoveDown.Enabled = true;
				imgMoveDown.Style["cursor"] = "pointer";
			}
			else
			{
				imgMoveDown.ImageUrl = "~/Images/MoveDownDisabled.gif";
				imgMoveDown.Enabled = false;
				imgMoveDown.Style["cursor"] = "default";
			}
		}


		protected void imgMoveUp_OnClick(object sender, ImageClickEventArgs e)
		{
			SwapRows(grdActions.SelectedIndex, grdActions.SelectedIndex - 1);
		}

		protected void imgMoveDown_OnClick(object sender, ImageClickEventArgs e)
		{
			SwapRows(grdActions.SelectedIndex, grdActions.SelectedIndex + 1);
		}

	}
}
