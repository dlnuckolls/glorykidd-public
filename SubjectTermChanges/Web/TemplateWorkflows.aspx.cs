using System;
using System.Data;
using System.Configuration;
using System.Collections;
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

	public partial class TemplateWorkflows : BaseTemplatePage
	{
        #region private members

        //hidden field to indicate status of a "Delete Term" transaction:
        private const string _KH_K_HF_IS_VERIFIED = "_KH_K_HF_IS_VERIFIED";
        private DeleteStatus _deleteStatus = DeleteStatus.None;

        #endregion


        #region base class overrides

        internal override Control ResizablePanel()
        {
            return this.pnlWorkflowGridBody;
        }

        internal override HtmlGenericControl HTMLBody()
        {
            return this.htmlBody;
        }

        protected override TemplateHeader HeaderControl()
        {
            return (TemplateHeader)header;
        }

        protected override object SaveViewState()
        {
            return base.SaveViewState();
        }

        protected override void LoadViewState(object savedState)
        {
            base.LoadViewState(savedState);
        }

        private void ApplyWorkflowUpdates()
        {
            if (Context.Items[Common.Names._CNTXT_Template] != null)
            {
                //IsChanged related change.  Used to always set IsChanged = true
				if (Context.Items[Common.Names._CNTXT_IsChanged] != null)
	                IsChanged = (bool)Context.Items[Common.Names._CNTXT_IsChanged];
                _template.Refresh();
            }

            if (Context.Items[Common.Names._CNTXT_TermDependency] != null)
            {
                TermDependency newTermDependency = (TermDependency)Context.Items[Common.Names._CNTXT_TermDependency];
                _template.AssignTermDependency(newTermDependency.ID, newTermDependency);
                //IsChanged related change.  Used to always set IsChanged = true
                IsChanged = (bool)Context.Items[Common.Names._CNTXT_IsChanged];
                _template.Refresh();
            }

            
        }


        #endregion


        #region event handlers

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            if (IsPostBack)
            {
                _deleteStatus = (DeleteStatus)Enum.Parse(typeof(DeleteStatus), Request.Form[_KH_K_HF_IS_VERIFIED]);
            }
            else
            {
                _deleteStatus = DeleteStatus.None;
                PopulateForm();
            }
        }


        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);
            ClientScript.RegisterHiddenField(_KH_K_HF_IS_VERIFIED, _deleteStatus.ToString("D"));
        }


        private void AddDeleteVerificationClientScripts(string termName, List<string> references)
        {
            Type t = this.GetType();
            string scriptName = "_kh_swVerifyScript";
            System.IO.StringWriter swVerifyScript = new System.IO.StringWriter();
            if (!ClientScript.IsStartupScriptRegistered(t, scriptName))
            {

                if (references.Count > 1)
                    swVerifyScript.Write("	var msg = 'You have selected to delete the \"{0}\" workflow, but this workflow is referenced in the following locations: \\n\\n", termName);
                else
                    swVerifyScript.Write("	var msg = 'You have selected to delete the \"{0}\" workflow, but this workflow is referenced in the following location: \\n\\n", termName);
                foreach (string reference in references)
                    swVerifyScript.Write("- {0}\\n", reference);
                if (references.Count > 1)
                    swVerifyScript.WriteLine("\\nYou must delete these references before you can delete the \"{0}\" term.';", termName);
                else
                    swVerifyScript.WriteLine("\\nYou must delete this reference before you can delete the \"{0}\" term.';", termName);
                swVerifyScript.WriteLine("	alert(msg); ");
                swVerifyScript.WriteLine("	document.forms['{0}']['{1}'].value = '{2}';", Form.Name, _KH_K_HF_IS_VERIFIED, DeleteStatus.Cancelled);
                ClientScript.RegisterStartupScript(t, scriptName, swVerifyScript.ToString(), true);
            }
        }


        protected void OnHeaderEvent(object sender, HeaderEventArgs e)
        {
            switch (e.CommandName)
            {
                case Common.Names._HEADER_EVENT_Save:
                    SaveTemplateWorkflows();
                    IsChanged = false;
                    break;
                case Common.Names._HEADER_EVENT_Reset:
                    GetTemplate(true);
                    PopulateForm();
                    IsChanged = false;
                    break;
                default:
                    base.HandleHeaderEvent(sender, e);
                    break;
            }
        }


        protected void btnAdd_Command(object sender, CommandEventArgs e)
        {
            if ((string)e.CommandArgument == "Workflow")
            {
					Context.Items[Common.Names._CNTXT_WorkflowEditMode] = EditMode.Add;
                Context.Items[Common.Names._CNTXT_Template] = _template;
                Server.Transfer("WorkFlowAdd.aspx", true);
                return;
            }


            if ((string)e.CommandArgument == "Dependency")
            {
                Business.TermDependency newTermDependency = new TermDependency(_template, DependencyTarget.Workflow);
                _template.TermDependencies.Add(newTermDependency);
                _template.Refresh();
                Context.Items[Common.Names._CNTXT_EditMode] = EditMode.Add;
                Context.Items[Common.Names._CNTXT_Template] = _template;
                Context.Items[Common.Names._CNTXT_TermDependency] = newTermDependency;
                Server.Transfer("TermDependencyEdit.aspx", true);
                return;
            }

            throw new Exception(string.Format("ERROR: the button's CommandArgument was {0}.   Valid values are Term, ComplexList, and Dependency.", (string)e.CommandArgument));

        }

        protected void grd_RowCreated(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                //Assign each row an ID containing the RowIndex
                string rowIndex = e.Row.RowIndex.ToString();
                e.Row.ID = "R" + rowIndex;
                if (_template.ActiveWorkflowID == _template.Workflows[e.Row.RowIndex].ID)
                    e.Row.Cells[1].Text = "True";                       
                else
                   e.Row.Cells[1].Text = "False";  

                //Set the command arguments for the Edit and Delete buttons
                ((LinkButton)e.Row.Cells[2].Controls[0]).CommandArgument = rowIndex;   //Copy button CommandArgument
                ((LinkButton)e.Row.Cells[3].Controls[0]).CommandArgument = rowIndex;   //Edit button CommandArgument
                if (_template.ActiveWorkflowID != _template.Workflows[e.Row.RowIndex].ID)
                {
                    ((LinkButton)e.Row.Cells[4].Controls[0]).CommandArgument = rowIndex;   //Delete button CommandArgument
                    //Set up client-side script to prompt the user if they click the Delete button
                    ((LinkButton)e.Row.Cells[4].Controls[0]).OnClientClick = "return confirm('Are you sure you want to delete this workflow?');";
                }
                else
                    ((LinkButton)e.Row.Cells[4].Controls[0]).Text = "";
            }
        }

        protected void grdDependency_RowCreated(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                //Assign each row an ID containing the RowIndex
                string rowIndex = e.Row.RowIndex.ToString();
                Guid workflowId = new Guid(_template.FilterDependency(DependencyTarget.Workflow)[e.Row.RowIndex].Action.SetValue);
                Business.Workflow workflow = _template.FindWorkflow(workflowId);
                e.Row.ID = "R" + rowIndex;
                e.Row.Cells[0].Text = workflow.Name;
                //Set the command arguments for the Edit and Delete buttons
                ((LinkButton)e.Row.Cells[2].Controls[0]).CommandArgument = rowIndex;   //Edit button CommandArgument
                ((LinkButton)e.Row.Cells[3].Controls[0]).CommandArgument = rowIndex;   //Delete button CommandArgument

                //Set up client-side script to prompt the user if they click the Delete button
                ((LinkButton)e.Row.Cells[3].Controls[0]).OnClientClick = "return confirm('Are you sure you want to delete this term?');";
            }
        }

        protected void grdDependency_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            int rowIndex = Convert.ToInt32(e.CommandArgument);

            switch (e.CommandName)
            {
                case Common.Names._GRID_COMMAND_EditRow:
                    Context.Items[Common.Names._CNTXT_Template] = _template;
                    Context.Items[Common.Names._CNTXT_EditMode] = EditMode.Edit;
                    Context.Items[Common.Names._CNTXT_TermDependency] = _template.FilterDependency(DependencyTarget.Workflow)[rowIndex];

                    //IsChanged related change
                    if (IsChanged)
                        Context.Items[Common.Names._CNTXT_IsChanged] = true;
                    Server.Transfer("TermDependencyEdit.aspx");
                    break;

                case Common.Names._GRID_COMMAND_DeleteRow:
                    DeleteTermDependency(rowIndex);
                    IsChanged = true;
                    break;

                case Common.Names._GRID_COMMAND_SingleClick:
                case Common.Names._GRID_COMMAND_DoubleClick:
						 //TODO: The following line is TEMPORARILY commented out, until we resolve issue number R-118
						 //SetMoveUpDownButtonEvents(grdDependency, btnDependencyMoveUp, btnDependencyMoveDown);
                    break;

                default:
                    break;
            }
        }

        protected void grd_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            Kindred.Common.Controls.KindredGridView grd = (Kindred.Common.Controls.KindredGridView)sender;
            int rowIndex = Convert.ToInt32(e.CommandArgument);

            switch (e.CommandName)
            {
                case Common.Names._GRID_COMMAND_EditRow:
                    Workflow w;
                    
                    w = _template.Workflows[rowIndex];

                    Context.Items[Common.Names._CNTXT_Template] = _template;
                    Context.Items[Common.Names._CNTXT_WorkflowEditMode] = EditMode.Edit;
                    Context.Items[Common.Names._CNTXT_WorkflowId] = _template.Workflows[rowIndex].ID;
                    //IsChanged related change
                    if (IsChanged)
                        Context.Items[Common.Names._CNTXT_IsChanged] = true;

                    Server.Transfer("TemplateWorkflowMain.aspx");
                    break;

                case Common.Names._GRID_COMMAND_DeleteRow:
                    if (_deleteStatus == DeleteStatus.Cancelled)
                        _deleteStatus = DeleteStatus.None;
                    else
                        IsChanged = TryDeleteWorkflow(rowIndex);
                    break;

                case Common.Names._GRID_COMMAND_CopyRow:
                    
                    string newName = _template.Workflow.GetNewWorkflowName(_template, grdWorkflow.Rows[rowIndex].Cells[0].Text);
                    Workflow newWorkflow = _template.Workflows[rowIndex].Copy(newName, _template.IsManagedItem, _template);
					_template.Workflows.Add(newWorkflow);
                    Context.Items[Common.Names._CNTXT_Template] = _template;
						  Context.Items[Common.Names._CNTXT_WorkflowEditMode] = EditMode.Add;
                    Context.Items[Common.Names._CNTXT_WorkflowId] = newWorkflow.ID;
                    Server.Transfer("TemplateWorkflowMain.aspx");
                    break;

                case Common.Names._GRID_COMMAND_SingleClick:
                case Common.Names._GRID_COMMAND_DoubleClick:
                    if (grd == grdWorkflow)
                        SetMoveUpDownButtonEvents(grdWorkflow, btnWorkflowMoveUp, btnWorkflowMoveDown);
                    break;

                default:
                    break;
            }
        }

        //See if additional delete confirmation is needed.  For example, is this term referenced elsewhere.
        //Is so, pop up a confirm dialog, and get the user's reply on the next postback
        //If not, go ahead and delete the term
        private bool TryDeleteWorkflow(int index)
        {
            bool rtn = false;
                foreach (GridViewRow i in grdDependency.Rows)
                {
                    if (_template.FilterDependency(DependencyTarget.Workflow)[i.RowIndex].Action.SetValue == _template.Workflows[index].ID.ToString())
                    {
                        RegisterAlert(string.Format("{0} can't be deleted. Remove Workflow dependencies first.",_template.Workflows[index].Name ));
                        rtn = false;
                        return rtn;
                    }

                }
                DeleteWorkflow(index);
                _deleteStatus = DeleteStatus.None;
                rtn = true;
            return rtn;
        }


        private void DeleteWorkflow(int index)
        {
                _template.Workflows.RemoveAt(index);
                _template.Refresh();
                grdWorkflow.SelectedIndex = -1;
                BindGrid(grdWorkflow, _template.Workflows);
         
        }


        private void DeleteTermDependency(int index)
        {
            _template.RemoveTermDependency(_template.FilterDependency(DependencyTarget.Workflow)[index].ID);
            _template.Refresh();
            grdDependency.SelectedIndex = -1;
            BindGrid(grdDependency, _template.FilterDependency(DependencyTarget.Workflow));
        }

        protected void btnSwitchRows_Command(object sender, CommandEventArgs e)
        {
            //Identify the grid and List<Term> objects involved (this method is used for both grids on the page)
            List<Workflow> list;
            Kindred.Common.Controls.KindredGridView grd;
            ImageButton upButton;
            ImageButton downButton;
            switch (e.CommandName)
            {
                case "Workflow":
                    list = _template.Workflows;
                    grd = grdWorkflow;
                    upButton = btnWorkflowMoveUp;
                    downButton = btnWorkflowMoveDown;
                    break;
                 default:
                    throw new Exception("Invalid Command Name: " + e.CommandName);
            }

            //Identify the 2 rows to be swapped
            int selectedRow = grd.SelectedIndex;
            int otherRow = ((string)e.CommandArgument == "up" ? selectedRow - 1 : selectedRow + 1);

            //Swap the 2 rows
            list.Reverse(Math.Min(selectedRow, otherRow), 2);
            //Re-bind the grid to the list (to reflect the new order of the Terms)
            grd.SelectedIndex = otherRow;
            BindGrid(grd, list);
            SetMoveUpDownButtonEvents(grd, upButton, downButton);
            IsChanged = true;
            _template.Refresh();
        }


        protected void btnSwitchDependencyRows_Command(object sender, CommandEventArgs e)
        {
            //Identify the grid and List<Term> objects involved (this method is used for both grids on the page)
            List<TermDependency> list = _template.TermDependencies;
            Kindred.Common.Controls.KindredGridView grd = grdDependency;
            ImageButton upButton = btnDependencyMoveUp;
            ImageButton downButton = btnDependencyMoveDown;

            //Identify the 2 rows to be swapped
            int selectedRow = grd.SelectedIndex;
            int otherRow = ((string)e.CommandArgument == "up" ? selectedRow - 1 : selectedRow + 1);

            //Swap the 2 rows
            list.Reverse(Math.Min(selectedRow, otherRow), 2);
            //Re-bind the grid to the list (to reflect the new order of the Terms)
            grd.SelectedIndex = otherRow;
            BindGrid(grd, list);
            SetMoveUpDownButtonEvents(grd, upButton, downButton);
            IsChanged = true;
        }


        //#endregion


        //#region private methods

  
        private void PopulateForm()
        {
            ApplyWorkflowUpdates();
            BindGrid(grdWorkflow, _template.Workflows);
            BindGrid(grdDependency, _template.FilterDependency(DependencyTarget.Workflow));
            SetMoveUpDownButtonEvents(grdWorkflow, btnWorkflowMoveUp, btnWorkflowMoveDown);
            SetMoveUpDownButtonEvents(grdDependency, btnDependencyMoveUp, btnDependencyMoveDown);
        }
        
        private string UpDownButtonCommandName(Kindred.Common.Controls.KindredGridView grd)
        {
            if (grd == grdWorkflow)
                return "Workflow";
            if (grd == grdDependency)
                return "Dependency";
            return string.Empty;
        }


        private void SetMoveUpDownButtonEvents(Kindred.Common.Controls.KindredGridView grd, ImageButton upButton, ImageButton downButton)
        {
            int selectedRow = grd.SelectedIndex;
            string buttonCommandName = UpDownButtonCommandName(grd);

            if (selectedRow > 0) //enable move up button
            {
                upButton.ImageUrl = "~/Images/MoveUp.gif";
                upButton.Enabled = true;
                upButton.Style["cursor"] = "pointer";
                upButton.CommandName = buttonCommandName;
                upButton.CommandArgument = "up";
            }
            else
            {
                upButton.ImageUrl = "~/Images/MoveUpDisabled.gif";
                upButton.Enabled = false;
                upButton.Style["cursor"] = "default";
            }

            if (selectedRow > -1 && selectedRow < grd.Rows.Count - 1)
            {
                downButton.ImageUrl = "~/Images/MoveDown.gif";
                downButton.Enabled = true;
                downButton.Style["cursor"] = "pointer";
                downButton.CommandName = buttonCommandName;
                downButton.CommandArgument = "down";
            }
            else
            {
                downButton.ImageUrl = "~/Images/MoveDownDisabled.gif";
                downButton.Enabled = false;
                downButton.Style["cursor"] = "default";
            }
        }


        private void BindGrid(Kindred.Common.Controls.KindredGridView grd, List<Workflow> list)
        {
            grd.DataSource = list;
            grd.DataBind();
        }

        private void BindGrid(Kindred.Common.Controls.KindredGridView grd, List<TermDependency> list)
        {
            grd.DataSource = list;
            grd.DataBind();
        }



        private void SaveTemplateWorkflows()
        {
			_template.SaveLoaded(true, true, "Workflows");
			//_template.SaveWorkflow(true, false);
			//_template.SaveTermDependencies(true, true);
            if (_template.IsManagedItem)
                RegisterAlert(string.Format("{0} Workflows have been saved.", this._itatSystem.ManagedItemName));
            else
                RegisterAlert("Template Workflows have been saved.");
        }

        #endregion

	}
}

