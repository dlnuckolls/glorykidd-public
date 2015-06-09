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
	public partial class TemplateWorkflowMain : BaseTemplatePage
	{

		#region private members

		//hidden field to indicate status of a "Delete Term" transaction:
		private const string _KH_K_HF_IS_VERIFIED = "_KH_K_HF_IS_VERIFIED";
		private const string _KH_VS_WORKFLOWID = "_kh_vs_WorkflowId";
		private const string _KH_VS_WORKFLOWEDITMODE = "_kh_vs_WorkflowEditMode";
		private const string _KH_VS_ORGSTATES = "_kh_vs_OrgStates";
		private DeleteStatus _deleteStatus = DeleteStatus.None;

		#endregion

		#region protected membders

		private Business.Workflow _workflow;
		private Guid _workflowId;
		private List<Business.State> _orgStates;
		private EditMode _workflowEditMode;

		#endregion



		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);
			if (!IsPostBack)
			{
                //enable the retro notification button only when retro is turned on at the template level.
                btnRetroNotification.Visible = true;//(_template.RetroModel != Kindred.Knect.ITAT.Business.Retro.RetroModel.Off);
                ApplyTemplateUpdates();
                ((StandardHeader)header).PageTitle = "Edit Workflow ";
				LoadGrid();
				SetMoveUpDownButtonEvents();
			}
		}



		protected override void OnPreRender(EventArgs e)
		{
			base.OnPreRender(e);
			SuppressChangeNotification(grd);
			ClientScript.RegisterHiddenField(_KH_K_HF_IS_VERIFIED, _deleteStatus.ToString("D"));

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
			_orgStates = (List<Business.State>)ViewState[_KH_VS_ORGSTATES];
			_workflowEditMode = (EditMode)ViewState[_KH_VS_WORKFLOWEDITMODE];
		}


		protected override object SaveViewState()
		{
			ViewState[_KH_VS_WORKFLOWID] = _workflowId;
			ViewState[_KH_VS_ORGSTATES] = _orgStates;
			ViewState[_KH_VS_WORKFLOWEDITMODE] = _workflowEditMode;
			return base.SaveViewState();
		}


		private void ApplyTemplateUpdates()
		{
			if (Context.Items[Common.Names._CNTXT_Template] != null)
				_template = (Business.Template)Context.Items[Common.Names._CNTXT_Template];

			if (Context.Items[Common.Names._CNTXT_IsChanged] != null)
				if ((bool)Context.Items[Common.Names._CNTXT_IsChanged])
					IsChanged = true;

			if (Context.Items[Common.Names._CNTXT_WorkflowId] != null)
			{
				_workflowEditMode = (EditMode)Context.Items[Common.Names._CNTXT_WorkflowEditMode];
				_workflowId = (Guid)Context.Items[Common.Names._CNTXT_WorkflowId];
				_workflow = _template.FindWorkflow(_workflowId);
				txtWorkflowName.Text = _workflow.Name;
				chkboxDefaultWorkflow.Checked = (_template.ActiveWorkflowID == _workflowId);

				if (_template.ActiveWorkflowID == _workflowId)
					chkboxDefaultWorkflow.Enabled = false;

				if (Context.Items[Common.Names._CNTXT_FromWorkflowStateEdit] != null)
					_orgStates = (List<Business.State>)Context.Items[Common.Names._CNTXT_OrgStates];
				else
					_orgStates = _workflow.States;

			}

		}


		private void LoadGrid()
		{
			grd.DataSource = _workflow.States;
			grd.DataBind();
		}



		private void SwapRows(int selectedRow, int otherRow)
		{
			_workflow.States.Reverse(Math.Min(selectedRow, otherRow), 2);
			LoadGrid();
			grd.SelectedIndex = otherRow;
			SetMoveUpDownButtonEvents();
			IsChanged = true;
		}


		private void SetMoveUpDownButtonEvents()
		{
			int selectedRow = grd.SelectedIndex;
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

			if (selectedRow > -1 && selectedRow < grd.Rows.Count - 1)
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
			SwapRows(grd.SelectedIndex, grd.SelectedIndex - 1);
		}

		protected void imgMoveDown_OnClick(object sender, ImageClickEventArgs e)
		{
			SwapRows(grd.SelectedIndex, grd.SelectedIndex + 1);
		}



        protected void btnRetroNotification_Command(object sender, CommandEventArgs e)
		{
            Context.Items[Common.Names._CNTXT_Template] = _template;
            Context.Items[Common.Names._CNTXT_WorkflowId] = _workflowId;
            Context.Items[Common.Names._CNTXT_WorkflowEditMode] = _workflowEditMode;
            Server.Transfer("RetroNotification.aspx");
        }

		protected void btnLimitTime_Command(object sender, CommandEventArgs e)
		{
			Context.Items[Common.Names._CNTXT_Template] = _template;
			Context.Items[Common.Names._CNTXT_WorkflowId] = _workflowId;
			Context.Items[Common.Names._CNTXT_WorkflowEditMode] = _workflowEditMode;
			Server.Transfer("TemplateWorkflowLimitTime.aspx");
		}


		protected void btnAdd_Command(object sender, CommandEventArgs e)
		{
			Business.State state = new Kindred.Knect.ITAT.Business.State(_template);
			state.Name = "New State";
			state.Status = "";
			state.IsBase = false;
			state.IsExit = false;
			state.IsDraft = false;
			state.RequiresValidation = false;
			_workflow.States.Add(state);
			EditState(state, _workflow.States.Count - 1, true);
		}


		protected void grd_RowCreated(object sender, GridViewRowEventArgs e)
		{
			if (e.Row.RowType == DataControlRowType.DataRow)
			{
				int rowIndex = e.Row.RowIndex;
				Business.State state = _workflow.States[rowIndex];
				//Assign each row an ID containing the RowIndex
				e.Row.ID = "R" + rowIndex.ToString();

				//Set the command arguments for the Edit and Delete buttons (except for the Base State)
				//Set up client-side script to prompt the user if they click the Delete button (except for the Base State)
				((LinkButton)e.Row.Cells[6].Controls[0]).CommandArgument = rowIndex.ToString();   //Edit button CommandArgument
				if (state.IsBase ?? false)
				{
					e.Row.Cells[7].Controls.Clear();
				}
				else
				{
					((LinkButton)e.Row.Cells[7].Controls[0]).CommandArgument = rowIndex.ToString();   //Delete button CommandArgument
					((LinkButton)e.Row.Cells[7].Controls[0]).OnClientClick = "return confirm('Are you sure you want to delete this state?');";
				}
			}
		}


		protected void grd_RowCommand(object sender, GridViewCommandEventArgs e)
		{
			int rowIndex = Convert.ToInt32(e.CommandArgument);
			switch (e.CommandName)
			{
				case Common.Names._GRID_COMMAND_EditRow:
					EditState((Business.State)(_workflow.States[rowIndex]), rowIndex, false);
					break;

				case Common.Names._GRID_COMMAND_DeleteRow:
					if (_deleteStatus == DeleteStatus.Cancelled)
						_deleteStatus = DeleteStatus.None;
					else
						IsChanged = TryDeleteState(rowIndex);
					break;

				case Common.Names._GRID_COMMAND_SingleClick:
				case Common.Names._GRID_COMMAND_DoubleClick:
					SetMoveUpDownButtonEvents();
					break;

				default:
					break;
			}
		}

		protected void btnOK_Command(object sender, CommandEventArgs e)
		{
			if (_workflow.Name != txtWorkflowName.Text)
			{
				if (_template.WorkflowExists(txtWorkflowName.Text))
					RegisterAlert(string.Format("The workflow name '{0}' is already being used.   You must select a different workflow name.", txtWorkflowName.Text));
				if (this.AlertCount > 0)
					return;
			}
			_workflow.Name = txtWorkflowName.Text;
			ReturnToCaller(true);
		}

		protected void btnCancel_Command(object sender, CommandEventArgs e)
		{
			ReturnToCaller(false);
		}


		private void ReturnToCaller(bool updateValues)
		{

			if (updateValues)
			{
				if (chkboxDefaultWorkflow.Checked && _template.ActiveWorkflowID != _workflowId)
				{
					_template.ActiveWorkflowID = _workflowId;
					if (_template.IsManagedItem)
					{
						Business.ManagedItem mi = (Business.ManagedItem)_template;
						Business.Workflow wf = mi.FindWorkflow(_workflowId);
						if (wf != null)
							mi.State = wf.FindBaseState();
					}
				}
				Context.Items[Common.Names._CNTXT_Template] = _template;
				Context.Items[Common.Names._CNTXT_IsChanged] = true;
			}
			else
			{
				//If the user clicks Cancel and the EditMode == Add, then remove the newly added term from the collection
				if (_workflowEditMode == EditMode.Add)
				{
					_template.Workflows.Remove(_workflow);
					_workflow = null;
				}
				else
				{
					_workflow.States = _orgStates;
				}
				Context.Items[Common.Names._CNTXT_Template] = _template;
				Context.Items[Common.Names._CNTXT_IsChanged] = false;
			}
			Context.Items[Common.Names._CNTXT_EditMode] = _workflowEditMode;
			Server.Transfer("TemplateWorkflows.aspx");
		}


		private bool TryDeleteState(int rowIndex)
		{
			//See if additional delete confirmation is needed.  For example, is this state referenced elsewhere.
			//Is so, pop up a confirm dialog, and get the user's reply on the next postback
			//If not, go ahead and delete the term
			bool rtn = false;
			if (_deleteStatus == DeleteStatus.Verified)
			{
				DeleteState(rowIndex);
				_deleteStatus = DeleteStatus.None;
				rtn = true;
			}
			else
			{
				Guid stateID = _workflow.States[rowIndex].ID;
                List<string> references = _template.Workflow.StateReferences(stateID);
				if (references.Count > 0)
				{
                    RegisterDeleteVerificationClientScripts(_workflow.States[rowIndex].Name, references);
					_deleteStatus = DeleteStatus.Prompted;
					rtn = false;
				}
				else
				{
					DeleteState(rowIndex);
					_deleteStatus = DeleteStatus.None;
					rtn = true;
				}
			}
			return rtn;
		}



		private void DeleteState(int rowIndex)
		{
			_workflow.States.RemoveAt(rowIndex);
			if (grd.SelectedIndex == rowIndex)
				grd.SelectedIndex = -1;
			LoadGrid();
			SetMoveUpDownButtonEvents();
		}



		private void EditState(Business.State state, int stateIndex, bool newState)
		{
			Context.Items[Common.Names._CNTXT_Template] = _template;
			Context.Items[Common.Names._CNTXT_StateIndex] = stateIndex;
			Context.Items[Common.Names._CNTXT_WorkflowId] = _workflowId;
			Context.Items[Common.Names._CNTXT_EditMode] = (newState ? EditMode.Add : EditMode.Edit);
			Context.Items[Common.Names._CNTXT_WorkflowEditMode] = _workflowEditMode;
			Context.Items[Common.Names._CNTXT_IsChanged] = IsChanged;
			Context.Items[Common.Names._CNTXT_OrgStates] = _orgStates;
			Server.Transfer("TemplateWorkflowStateEdit.aspx");
		}


		private void RegisterDeleteVerificationClientScripts(string stateName, List<string> references)
		{
			Type t = this.GetType();
			string scriptName = "_kh_swVerifyScript";
			System.IO.StringWriter swVerifyScript = new System.IO.StringWriter();
			if (!ClientScript.IsStartupScriptRegistered(t, scriptName))
			{
				if (references.Count > 1)
					swVerifyScript.Write("	var msg = 'You have selected to delete the \"{0}\" state, but this state is referenced in the following locations: \\n\\n", stateName);
				else
					swVerifyScript.Write("	var msg = 'You have selected to delete the \"{0}\" state, but this state is referenced in the following location: \\n\\n", stateName);
				foreach (string reference in references)
					swVerifyScript.Write("- {0}\\n", reference);
				if (references.Count > 1)
					swVerifyScript.WriteLine("\\nYou must delete these references before you can delete the \"{0}\" state.';", stateName);
				else
					swVerifyScript.WriteLine("\\nYou must delete this reference before you can delete the \"{0}\" state.';", stateName);
				swVerifyScript.WriteLine("	alert(msg); ");
				swVerifyScript.WriteLine("	document.forms['{0}']['{1}'].value = '{2}';", Form.Name, _KH_K_HF_IS_VERIFIED, DeleteStatus.Cancelled);
				//swVerifyScript.WriteLine("__doPostBack('{0}', '{1}');", Request.Form["__EVENTTARGET"], Request.Form["__EVENTARGUMENT"]);
				ClientScript.RegisterStartupScript(t, scriptName, swVerifyScript.ToString(), true);
			}
		}

	}
}
