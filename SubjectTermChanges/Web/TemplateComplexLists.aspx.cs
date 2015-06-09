using System;
using System.Collections.Generic;
using System.Web.UI;
using System.Web.UI.WebControls;
using Kindred.Knect.ITAT.Business;
using System.Web.UI.HtmlControls;

namespace Kindred.Knect.ITAT.Web
{
    public partial class TemplateComplexLists : BaseTemplatePage
    {
        #region private members


        private const string _KH_K_HF_IS_VERIFIED = "_KH_K_HF_IS_VERIFIED";
        private DeleteStatus _deleteStatus = DeleteStatus.None;

        #endregion

        #region base class overrides

        internal override Control ResizablePanel()
        {
            return this.pnlListGridBody;
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

        private void ApplyTemplateUpdates()
        {
            if (Context.Items[Common.Names._CNTXT_TermEdit] != null)
            {
                IsChanged = (bool)Context.Items[Common.Names._CNTXT_IsChanged];

                BaseTermEditPage.TermEdit termEdit = (BaseTermEditPage.TermEdit)Context.Items[Common.Names._CNTXT_TermEdit];
                switch (termEdit.TermHandler)
                {
                    case BaseTermEditPage.TermHandler.ComplexList:
                        break;

                    case BaseTermEditPage.TermHandler.BasicTerm:
                    case BaseTermEditPage.TermHandler.ComplexListField:
                    case BaseTermEditPage.TermHandler.ComplexListItem:
                        throw new Exception(string.Format("TermHandler '{0}' is not handled", termEdit.TermHandler.ToString()));
                }
                if (termEdit.TermIndex <= _template.ComplexLists.Count - 1)
                {
                    _template.ComplexLists[termEdit.TermIndex] = (ComplexList)termEdit.Term;
                }
                _template.Refresh();
            }
        }

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

        private void AddDeleteVerificationClientScripts(string listName, List<string> references)
        {
            Type t = this.GetType();
            string scriptName = "_kh_swVerifyScript";
            System.IO.StringWriter swVerifyScript = new System.IO.StringWriter();
            if (!ClientScript.IsStartupScriptRegistered(t, scriptName))
            {

                if (references.Count > 1)
                    swVerifyScript.Write("	var msg = 'You have selected to delete the \"{0}\" complexlist, but this complexlist is referenced in the following locations: \\n\\n", listName);
                else
                    swVerifyScript.Write("	var msg = 'You have selected to delete the \"{0}\" complexlist, but this complexlist is referenced in the following location: \\n\\n", listName);
                foreach (string reference in references)
                    swVerifyScript.Write("- {0}\\n", reference);
                if (references.Count > 1)
                    swVerifyScript.WriteLine("\\nYou must delete these references before you can delete the \"{0}\" complexlist.';", listName);
                else
                    swVerifyScript.WriteLine("\\nYou must delete this reference before you can delete the \"{0}\" complexlist.';", listName);
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
                    Common.GridHelper.SyncToGridClientState(ucGridMoveRows, _template.ComplexLists, (Term term, string id) => { return term.ID.Equals(new Guid(id)); });
                    SaveTemplateTerms();
                    PopulateForm();
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
            Business.Term newTerm = Helper.CreateTerm(Business.TermType.ComplexList, false, _template.IsManagedItem, _template, false);
            newTerm.Name = Business.ComplexList.NewTermName(_template);
            _template.ComplexLists.Add(newTerm);
            _template.Refresh();

            Context.Items[Common.Names._CNTXT_Template] = _template;
            BaseTermEditPage.TermEdit termEdit = new BaseTermEditPage.TermEdit();
            termEdit.EditMode = EditMode.Add;
            termEdit.TermHandler = BaseTermEditPage.TermHandler.ComplexList;
            termEdit.TermIndex = _template.ComplexLists.Count - 1;  //item just added
            Context.Items[Common.Names._CNTXT_TermEdit] = termEdit;

            Server.Transfer(Helper.TemplateTermEditPage(TermType.ComplexList), true);
            return;
        }

        protected void grdList_RowCreated(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                //Assign each row an ID containing the RowIndex
                string rowIndex = e.Row.RowIndex.ToString();
                e.Row.ID = String.Concat("R_", grdList.DataKeys[e.Row.RowIndex].Value);
                //Set the command arguments for the Edit and Delete buttons
                ((LinkButton)e.Row.Cells[2].Controls[0]).CommandArgument = rowIndex;   //Edit button CommandArgument
                ((LinkButton)e.Row.Cells[3].Controls[0]).CommandArgument = rowIndex;   //Delete button CommandArgument

                //Disable delete for System level terms
                ((LinkButton)e.Row.Cells[3].Controls[0]).Visible = !_template.ComplexLists[e.Row.RowIndex].SystemTerm;

                //Set up client-side script to prompt the user if they click the Delete button
                ((LinkButton)e.Row.Cells[3].Controls[0]).OnClientClick = "return confirm('Are you sure you want to delete this ComplexList?');";
            }
        }

        protected void grd_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            Kindred.Common.Controls.KindredGridView grd = (Kindred.Common.Controls.KindredGridView)sender;
            int rowIndex = Convert.ToInt32(e.CommandArgument);

            switch (e.CommandName)
            {
                case Common.Names._GRID_COMMAND_EditRow:
                    BaseTermEditPage.TermEdit termEdit = new BaseTermEditPage.TermEdit();
                    Term t = _template.ComplexLists[rowIndex];
                    //IsChanged related change
                    if (IsChanged)
                        Context.Items[Common.Names._CNTXT_IsChanged] = true;
                    Context.Items[Common.Names._CNTXT_Template] = _template;

                    termEdit.EditMode = EditMode.Edit;
                    termEdit.TermHandler = BaseTermEditPage.TermHandler.ComplexList;
                    termEdit.TermIndex = rowIndex;
                    Context.Items[Common.Names._CNTXT_TermEdit] = termEdit;
                    Server.Transfer(Helper.TemplateTermEditPage(t.TermType));
                    break;

                case Common.Names._GRID_COMMAND_DeleteRow:
                    //20071019_DEG	Commented out these two lines - the 'conflicts' dialog was appearing on every other attempt.
                    //if (_deleteStatus == DeleteStatus.Cancelled)
                    //    _deleteStatus = DeleteStatus.None;
                    //else
                    IsChanged = TryDeleteTerm(rowIndex);
                    break;

                case Common.Names._GRID_COMMAND_SingleClick:
                case Common.Names._GRID_COMMAND_DoubleClick:
                    break;

                default:
                    break;
            }
        }

        private bool TryDeleteTerm(int index)
        {
            bool rtn = false;
            if (_deleteStatus == DeleteStatus.Verified)
            {
                DeleteTerm(index);
                _deleteStatus = DeleteStatus.None;
                rtn = true;
            }
            else
            {
                string termName =  _template.ComplexLists[index].Name;
                List<string> references = _template.TermReferences(_template, termName);
                //Now looking at the use of the term within the DataStoreConfigurations.
                references.AddRange(Business.SystemStore.GetTermReferences(_itatSystem.ID, _template.ID, _template.ComplexLists[index].ID));
                if (references.Count > 0)
                {
                    AddDeleteVerificationClientScripts(termName, references);
                    _deleteStatus = DeleteStatus.Prompted;
                    rtn = false;
                }
                else
                {
                    DeleteTerm(index);
                    _deleteStatus = DeleteStatus.None;
                    rtn = true;
                }
            }
            return rtn;
        }

        private void DeleteTerm(int index)
        {
            if (_template.SecurityModel == SecurityModel.Advanced)
                _template.DeleteTermGroup(_template.ComplexLists[index].TermGroupID);
            _template.ComplexLists.RemoveAt(index);
            _template.Refresh();
            grdList.SelectedIndex = -1;
            BindGrid(grdList, _template.ComplexLists);

        }

        #endregion

        #region private methods

        private void PopulateForm()
        {
            ApplyTemplateUpdates();
            BindGrid(grdList, _template.ComplexLists);
        }

        private void BindGrid(Kindred.Common.Controls.KindredGridView grd, List<Term> list)
        {
            grd.DataSource = list;
            grd.DataBind();
        }

        private void SaveTemplateTerms()
        {
            _template.SaveLoaded(true, true, "Complex Lists");

            if (_template.IsManagedItem)
                RegisterAlert(string.Format("{0} Complex Lists have been saved.", this._itatSystem.ManagedItemName));
            else
                RegisterAlert("Complex Lists have been saved.");
        }

        #endregion
    }
}
