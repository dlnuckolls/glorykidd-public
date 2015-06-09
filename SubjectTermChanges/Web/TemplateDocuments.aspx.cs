using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using Kindred.Knect.ITAT.Business;

namespace Kindred.Knect.ITAT.Web
{
    public partial class TemplateDocuments : BaseTemplatePage
    {
        #region private members

        //hidden field to indicate status of a "Delete Term" transaction:
        private const string _KH_K_HF_IS_VERIFIED = "_KH_K_HF_IS_VERIFIED";
        private DeleteStatus _deleteStatus = DeleteStatus.None;

        #endregion



        #region base class overrides

        internal override Control ResizablePanel()
        {
            return this.pnlDocumentGridBody;
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

            Context.Items[Common.Names._CNTXT_EditMode] = EditMode.Add;
            Context.Items[Common.Names._CNTXT_Template] = _template;
            Server.Transfer("TemplateClauses.aspx");
            return;
        }

        protected void grdDocument_RowCreated(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                //Assign each row an ID containing the RowIndex
                string rowIndex = e.Row.RowIndex.ToString();
                e.Row.ID = "R" + rowIndex;
                //Set the command arguments for the Edit and Delete buttons
                ((LinkButton)e.Row.Cells[3].Controls[0]).CommandArgument = rowIndex;   //Edit button CommandArgument
                ((LinkButton)e.Row.Cells[4].Controls[0]).CommandArgument = rowIndex;   //Delete button CommandArgument

                //Set up client-side script to prompt the user if they click the Delete button
                ((LinkButton)e.Row.Cells[4].Controls[0]).OnClientClick = "return confirm('Are you sure you want to delete this document?');";
            }
        }

        protected void grdDocument_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            int rowIndex = Convert.ToInt32(e.CommandArgument);

            switch (e.CommandName)
            {
                case Common.Names._GRID_COMMAND_EditRow:
                    Context.Items[Common.Names._CNTXT_Template] = _template;
                    Context.Items[Common.Names._CNTXT_EditMode] = EditMode.Edit;
                    Context.Items[Common.Names._CNTXT_ITATDocumentID] = grdDocument.DataKeys[rowIndex].Value.ToString();

                    //IsChanged related change
                    if (IsChanged)
                        Context.Items[Common.Names._CNTXT_IsChanged] = true;
                    Server.Transfer("TemplateClauses.aspx");
                    break;

                case Common.Names._GRID_COMMAND_DeleteRow:
                    DeleteITATDocument(rowIndex);
                    IsChanged = true;
                    break;

                case Common.Names._GRID_COMMAND_SingleClick:
                case Common.Names._GRID_COMMAND_DoubleClick:
                    SetMoveUpDownButtonEvents(grdDocument, btnDocumentMoveUp, btnDocumentMoveDown);
                    break;
                case "Preview":
                    if (IsChanged)
                    {
                        RegisterAlert("Please click the Save or Reset button before previewing");
                    }
                    else
                    {
                        string _ITATDocumentID = grdDocument.DataKeys[rowIndex].Value.ToString();
                        string props = "center=yes; help=no; resizable=yes;";

                        string managedItemId = Request.QueryString[Common.Names._QS_MANAGED_ITEM_ID];
                        string queryString;

                        if (!string.IsNullOrEmpty(managedItemId))
                            queryString = Utility.TextHelper.QueryString(true, Common.Names._QS_DOC_DLG_ACTION, Common.Names._QS_DOC_DLG_ACTION_PREVIEW, Common.Names._QS_MANAGED_ITEM_ID, managedItemId, Common.Names._QS_ITAT_SYSTEM_ID, _itatSystem.ID.ToString(), Common.Names._QS_ITAT_DOCUMENT_ID, _ITATDocumentID, Common.Names._QS_SHOW_DEFAULT_DOCUMENT, "false");
                        else
                            queryString = Utility.TextHelper.QueryString(true, Common.Names._QS_DOC_DLG_ACTION, Common.Names._QS_DOC_DLG_ACTION_PREVIEW, Common.Names._QS_TEMPLATE_ID, _template.ID.ToString("D"), Common.Names._QS_ITAT_SYSTEM_ID, _itatSystem.ID.ToString(), Common.Names._QS_ITAT_DOCUMENT_ID, _ITATDocumentID, Common.Names._QS_SHOW_DEFAULT_DOCUMENT, "false");
                        ShowDialog("DocumentDialog.aspx" + queryString, props, false);

                    }
                    break;
                default:
                    break;
            }
        }


        private void DeleteITATDocument(int index)
        {
            _template.Documents.RemoveAt(index);
            _template.Refresh();
            grdDocument.SelectedIndex = -1;
            BindGrid(grdDocument, _template.Documents);
        }


        protected void btnSwitchDocumentRows_Command(object sender, CommandEventArgs e)
        {
            //Identify the grid and List<Term> objects involved (this method is used for both grids on the page)
            List<ITATDocument> list = _template.Documents;
            Kindred.Common.Controls.KindredGridView grd = grdDocument;
            ImageButton upButton = btnDocumentMoveUp;
            ImageButton downButton = btnDocumentMoveDown;

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
            //ApplyTemplateUpdates();
            //BindGrid(grdDocument, _template.FilterDependency(DependencyTarget.Term));
            //SetMoveUpDownButtonEvents(grdDocument, btnDocumentMoveUp, btnDocumentMoveDown);

            BindGrid(grdDocument, _template.Documents);
            SetMoveUpDownButtonEvents(grdDocument, btnDocumentMoveUp, btnDocumentMoveDown);
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



        private void BindGrid(Kindred.Common.Controls.KindredGridView grd, List<ITATDocument> list)
        {
            grd.DataSource = list;
            grd.DataBind();
        }

        private void SaveTemplateTerms()
        {
            _template.SaveLoaded(true, true, "TemplateDocuments");

            if (_template.IsManagedItem)
                RegisterAlert(string.Format("{0} Documents have been saved.", this._itatSystem.ManagedItemName));
            else
                RegisterAlert("Template Documents have been saved.");
        }

        #endregion
    }
}