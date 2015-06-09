using System;
using System.Collections.Generic;
using System.Web.UI;
using System.Web.UI.WebControls;
using Kindred.Knect.ITAT.Business;
using System.Web.UI.HtmlControls;

namespace Kindred.Knect.ITAT.Web
{
    public partial class TemplateTermDependencies : BaseTemplatePage
    {
        #region private members

        //hidden field to indicate status of a "Delete Term" transaction:
        private const string _KH_K_HF_IS_VERIFIED = "_KH_K_HF_IS_VERIFIED";
        private DeleteStatus _deleteStatus = DeleteStatus.None;

        #endregion



        #region base class overrides

        internal override Control ResizablePanel()
        {
            return this.pnlDependencyGridBody;
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

        private void ApplyTemplateUpdates()
        {

            if (Context.Items[Common.Names._CNTXT_TermDependency] != null)
            {
                TermDependency newTermDependency = (TermDependency)Context.Items[Common.Names._CNTXT_TermDependency];
                _template.AssignTermDependency(newTermDependency.ID, newTermDependency);
                _template.Refresh();
                IsChanged = (bool)Context.Items[Common.Names._CNTXT_IsChanged];
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
                    swVerifyScript.Write("	var msg = 'You have selected to delete the \"{0}\" term dependency, but this term dependency is referenced in the following locations: \\n\\n", termName);
                else
                    swVerifyScript.Write("	var msg = 'You have selected to delete the \"{0}\" term dependency, but this term dependency is referenced in the following location: \\n\\n", termName);
                foreach (string reference in references)
                    swVerifyScript.Write("- {0}\\n", reference);
                if (references.Count > 1)
                    swVerifyScript.WriteLine("\\nYou must delete these references before you can delete the \"{0}\" term dependency.';", termName);
                else
                    swVerifyScript.WriteLine("\\nYou must delete this reference before you can delete the \"{0}\" term dependency.';", termName);
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
                    SaveTemplateTerms();
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

                Business.TermDependency newTermDependency = new TermDependency(_template, DependencyTarget.Term);
                _template.TermDependencies.Add(newTermDependency);
                _template.Refresh();
                Context.Items[Common.Names._CNTXT_EditMode] = EditMode.Add;
                Context.Items[Common.Names._CNTXT_Template] = _template;
                Context.Items[Common.Names._CNTXT_TermDependency] = newTermDependency;
                Server.Transfer("TermDependencyEdit.aspx", true);
                return;
        }

        protected void grdDependency_RowCreated(object sender, GridViewRowEventArgs e)
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
                ((LinkButton)e.Row.Cells[3].Controls[0]).OnClientClick = "return confirm('Are you sure you want to delete this term dependency?');";
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
                    //TermDependencyID change
                    Context.Items[Common.Names._CNTXT_TermDependency] = _template.FilterDependency(DependencyTarget.Term)[rowIndex];

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
                    SetMoveUpDownButtonEvents(grdDependency, btnDependencyMoveUp, btnDependencyMoveDown);
                    break;

                default:
                    break;
            }
        }

        private void DeleteTermDependency(int index)
        {
            //TermDependencyID change
            _template.RemoveTermDependency(_template.FilterDependency(DependencyTarget.Term)[index].ID);
            _template.Refresh();
            grdDependency.SelectedIndex = -1;
            BindGrid(grdDependency, _template.FilterDependency(DependencyTarget.Term));
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

        #endregion


        #region private methods

        private void PopulateForm()
        {
            ApplyTemplateUpdates();
            BindGrid(grdDependency, _template.FilterDependency(DependencyTarget.Term));
            SetMoveUpDownButtonEvents(grdDependency, btnDependencyMoveUp, btnDependencyMoveDown);
        }

        private void SetMoveUpDownButtonEvents(Kindred.Common.Controls.KindredGridView grd, ImageButton upButton, ImageButton downButton)
        {
            int selectedRow = grd.SelectedIndex;

            if (selectedRow > 0) //enable move up button
            {
                upButton.ImageUrl = "~/Images/MoveUp.gif";
                upButton.Enabled = true;
                upButton.Style["cursor"] = "pointer";
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
                downButton.CommandArgument = "down";
            }
            else
            {
                downButton.ImageUrl = "~/Images/MoveDownDisabled.gif";
                downButton.Enabled = false;
                downButton.Style["cursor"] = "default";
            }
        }



        private void BindGrid(Kindred.Common.Controls.KindredGridView grd, List<TermDependency> list)
        {
            grd.DataSource = list;
            grd.DataBind();
        }

        private void SaveTemplateTerms()
        {
            _template.SaveLoaded(true, true, "Term Dependencies");

            if (_template.IsManagedItem)
                RegisterAlert(string.Format("{0} Terms have been saved.", this._itatSystem.ManagedItemName));
            else
                RegisterAlert("Template Terms have been saved.");
        }

        #endregion
    }
}
