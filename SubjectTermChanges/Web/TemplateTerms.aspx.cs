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
using Kindred.Knect.ITAT.Web.Controls;

namespace Kindred.Knect.ITAT.Web
{

	public partial class TemplateTerms : BaseTemplatePage
	{
		#region private members

		//hidden field to indicate status of a "Delete Term" transaction:
		private const string _KH_K_HF_IS_VERIFIED = "_KH_K_HF_IS_VERIFIED";
        //hidden field to indicate status of a "Delete Term Group" transaction:
        private const string _KH_K_HF_IS_VERIFIED_TERM_GROUP = "_KH_K_HF_IS_VERIFIED_TERM_GROUP";
		private DeleteStatus _deleteStatus = DeleteStatus.None;
        private DeleteStatus _deleteStatusTermGroup = DeleteStatus.None;

		#endregion


		#region base class overrides

		internal override Control ResizablePanel()
		{
			return  this.pnlTermGridBody;  
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
            if (Context.Items[Common.Names._CNTXT_TermEdit] != null)
            {
                BaseTermEditPage.TermEdit termEdit = (BaseTermEditPage.TermEdit)Context.Items[Common.Names._CNTXT_TermEdit];
                IsChanged = (bool)Context.Items[Common.Names._CNTXT_IsChanged];
                switch (termEdit.TermHandler)
                {
                    case BaseTermEditPage.TermHandler.BasicTerm:
                        break;

                    case BaseTermEditPage.TermHandler.ComplexList:
                    case BaseTermEditPage.TermHandler.ComplexListField:
                    case BaseTermEditPage.TermHandler.ComplexListItem:
                        throw new Exception(string.Format("TermHandler '{0}' is not handled", termEdit.TermHandler.ToString()));
                }
                if (termEdit.TermIndex >= 0 && _template.BasicTerms.Count >= termEdit.TermIndex + 1)
                    _template.BasicTerms[termEdit.TermIndex] = termEdit.Term;
                _template.Refresh();
            }

            if (Context.Items[Common.Names._CNTXT_TermGroup] != null)
            {
                TermGroup newTermGroup = (TermGroup)Context.Items[Common.Names._CNTXT_TermGroup];
                int termGroupIndex = (int)Context.Items[Common.Names._CNTXT_TermGroupIndex];
                _template.TermGroups[termGroupIndex] = newTermGroup;

                _template.Refresh();
                //IsChanged related change.  Used to always set IsChanged = true
                IsChanged = (bool)Context.Items[Common.Names._CNTXT_IsChanged];
            }
		}


		#endregion


		#region event handlers

		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);

            if (_template.SecurityModel == SecurityModel.Advanced)
            {
                SetTermGroupSelection();
            }

			if (IsPostBack)
			{
				_deleteStatus = (DeleteStatus)Enum.Parse(typeof(DeleteStatus), Request.Form[_KH_K_HF_IS_VERIFIED]);
                _deleteStatusTermGroup = (DeleteStatus)Enum.Parse(typeof(DeleteStatus), Request.Form[_KH_K_HF_IS_VERIFIED_TERM_GROUP]);
 			}
			else
			{
				_deleteStatus = DeleteStatus.None;
                _deleteStatusTermGroup = DeleteStatus.None;
				PopulateForm();
			}
		}

		protected override void OnPreRender(EventArgs e)
		{
			base.OnPreRender(e);
			ClientScript.RegisterHiddenField(_KH_K_HF_IS_VERIFIED, _deleteStatus.ToString("D"));
            ClientScript.RegisterHiddenField(_KH_K_HF_IS_VERIFIED_TERM_GROUP, _deleteStatus.ToString("D"));
		}

		private void AddDeleteVerificationClientScripts(string termName, List<string> references)
		{
			Type t = this.GetType();
			string scriptName = "_kh_swVerifyScript";
			System.IO.StringWriter swVerifyScript = new System.IO.StringWriter();
			if (!ClientScript.IsStartupScriptRegistered(t, scriptName))
			{
				if (references.Count > 1)
					swVerifyScript.Write("	var msg = 'You have selected to delete the \"{0}\" term, but this term is referenced in the following locations: \\n\\n", termName);
				else
					swVerifyScript.Write("	var msg = 'You have selected to delete the \"{0}\" term, but this term is referenced in the following location: \\n\\n", termName);
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

        private void AddDeleteVerificationClientScripts(string termGroupName, bool termGroupInUse)
        {
            Type t = this.GetType();
            string scriptName = "_kh_swVerifyScript_TermGroup";
            System.IO.StringWriter swVerifyScript = new System.IO.StringWriter();
            if (!ClientScript.IsStartupScriptRegistered(t, scriptName))
            {

                if (termGroupInUse)
                {
                    swVerifyScript.Write("	var msg = 'You have selected to delete the \"{0}\" term group, but this term group contains terms.'; ", termGroupName);
                    swVerifyScript.WriteLine("	alert(msg); ");
                    swVerifyScript.WriteLine("	document.forms['{0}']['{1}'].value = '{2}';", Form.Name, _KH_K_HF_IS_VERIFIED_TERM_GROUP, DeleteStatus.Cancelled);
                    ClientScript.RegisterStartupScript(t, scriptName, swVerifyScript.ToString(), true);
                }
            }
        }

		protected void OnHeaderEvent(object sender, HeaderEventArgs e)
		{
			switch (e.CommandName)
			{
				case Common.Names._HEADER_EVENT_Save:
                    List<Term> initialTermsList = GetTerms();
                    Common.GridHelper.SyncToGridClientState(ucGridMoveRows, initialTermsList, (Term term, string id) => { return term.ID.Equals(new Guid(id)); });
                    
                    //if the list being operated on is not same as the basic terms list obtained from the template, it needs synching up
                    if (!initialTermsList.Equals(_template.BasicTerms))
                    {
                        SyncTermsWithTemplate(initialTermsList);
                    }

                    SaveTemplateTerms();
                    GetTemplate(true);
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


        private void SyncTermsWithTemplate(List<Term> initialTermsList)
        {
            //Sync to the terms in the template.
            for (int i = 1; i < initialTermsList.Count; i++)
            {
                Guid termID = (Guid)initialTermsList[i].ID;
                Term term = _template.FindTerm(termID);
                //int termIndex = _template.FindTermIndex(termID, _template.BasicTerms);
                _template.BasicTerms.Remove(term);
                _template.BasicTerms.Insert(_template.BasicTerms.Count, term);
            }
        }

		protected void btnAdd_Command(object sender, CommandEventArgs e)
		{
            if ((string)e.CommandArgument == "Term")
            {
                BaseTermEditPage.TermEdit termEdit = new BaseTermEditPage.TermEdit();
                termEdit.EditMode = EditMode.Add;
                Context.Items[Common.Names._CNTXT_Template] = _template;

                if (_template.SecurityModel == SecurityModel.Advanced)
                    Context.Items[Common.Names._CNTXT_SelectedTermGroupIndex] = grdTermGroup.SelectedIndex;

                termEdit.TermHandler = BaseTermEditPage.TermHandler.BasicTerm;
                Context.Items[Common.Names._CNTXT_TermEdit] = termEdit;
                Server.Transfer("TermAdd.aspx", true);
                return;
            }

            if ((string)e.CommandArgument == "TermGroup")
            {
                Context.Items[Common.Names._CNTXT_EditMode] = EditMode.Add;
                Context.Items[Common.Names._CNTXT_Template] = _template;
                Server.Transfer("TemplateTermGroupAddEdit.aspx", true);
                return;
            }

            throw new Exception(string.Format("ERROR: the button's CommandArgument was {0}.   Valid values are Term, TermGroup.", (string)e.CommandArgument));
		}

		protected void grd_RowCreated(object sender, GridViewRowEventArgs e)
		{
			if (e.Row.RowType == DataControlRowType.DataRow)
			{
				//Assign each row an ID containing the RowIndex
				string rowIndex = e.Row.RowIndex.ToString();
                e.Row.ID = String.Concat("R_",grdTerm.DataKeys[e.Row.RowIndex].Value);

				//Set the command arguments for the Edit and Delete buttons
				((LinkButton)e.Row.Cells[4].Controls[0]).CommandArgument = rowIndex;   //Edit button CommandArgument
				((LinkButton)e.Row.Cells[5].Controls[0]).CommandArgument = rowIndex;   //Delete button CommandArgument

				//Disable delete for System level terms
				if (_template.BasicTerms[_template.FindTermIndex((Guid)grdTerm.DataKeys[e.Row.RowIndex].Value, _template.BasicTerms)].SystemTerm)
				{
					((LinkButton)e.Row.Cells[5].Controls[0]).Visible = false;
				}

                if (_template.BasicTerms[_template.FindTermIndex((Guid)grdTerm.DataKeys[e.Row.RowIndex].Value, _template.BasicTerms)].TermType == (TermType.PlaceHolderAttachments) ||
                    _template.BasicTerms[_template.FindTermIndex((Guid)grdTerm.DataKeys[e.Row.RowIndex].Value, _template.BasicTerms)].TermType == (TermType.PlaceHolderComments))
                {
                    //((LinkButton)e.Row.Cells[3].Controls[0]).Visible = false;
                    ((LinkButton)e.Row.Cells[5].Controls[0]).Visible = false;
                }

				//Set up client-side script to prompt the user if they click the Delete button
				((LinkButton)e.Row.Cells[5].Controls[0]).OnClientClick = "return confirm('Are you sure you want to delete this term?');";
			}
		}


		protected void grdTermGroup_RowCreated(object sender, GridViewRowEventArgs e)
		{
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                //Assign each row an ID containing the RowIndex
                string rowIndex = e.Row.RowIndex.ToString();
                e.Row.ID = "R" + rowIndex;

                //Set the command arguments for the Edit and Delete buttons
                ((LinkButton)e.Row.Cells[2].Controls[0]).CommandArgument = rowIndex;   //Edit button CommandArgument
                ((LinkButton)e.Row.Cells[3].Controls[0]).CommandArgument = rowIndex;   //Delete button CommandArgument

                // disable edit and delete buttons for the first option (ALL)
                if (e.Row.RowIndex == 0)
                {
                    ((LinkButton)e.Row.Cells[2].Controls[0]).Visible = false;
                    ((LinkButton)e.Row.Cells[3].Controls[0]).Visible = false;
                }

                 //Set up client-side script to prompt the user if they click the Delete button
                ((LinkButton)e.Row.Cells[3].Controls[0]).OnClientClick = "return confirm('Are you sure you want to delete this term group?');";
            }
		}

		protected void grd_RowCommand(object sender, GridViewCommandEventArgs e)
		{
			int rowIndex = Convert.ToInt32(e.CommandArgument);
            int termIndex = _template.FindTermIndex((Guid)grdTerm.DataKeys[rowIndex].Value, _template.BasicTerms);

			switch (e.CommandName)
			{
                case Common.Names._GRID_COMMAND_EditRow:
                    Term t = _template.FindTerm((Guid)grdTerm.DataKeys[rowIndex].Value);
                    BaseTermEditPage.TermEdit termEdit = new BaseTermEditPage.TermEdit();
                    Context.Items[Common.Names._CNTXT_Template] = _template;
                    termEdit.EditMode = EditMode.Edit;
                    termEdit.TermIndex = termIndex;
                    
                    //Term Group selection
                    if (_template.SecurityModel == SecurityModel.Advanced)
                        Context.Items[Common.Names._CNTXT_SelectedTermGroupIndex] = grdTermGroup.SelectedIndex;

                    termEdit.TermHandler = BaseTermEditPage.TermHandler.BasicTerm;
                    Context.Items[Common.Names._CNTXT_TermEdit] = termEdit;
                    Server.Transfer(Helper.TemplateTermEditPage(t.TermType));
					break;

				case Common.Names._GRID_COMMAND_DeleteRow:
                    IsChanged = TryDeleteTerm(termIndex);
					break;
				
				case Common.Names._GRID_COMMAND_SingleClick:
				case Common.Names._GRID_COMMAND_DoubleClick:
					break;
				
				default:
					break;
			}
		}

        protected void grd_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            Literal litHeaderTerm = e.Row.FindControl("litHeaderTerm") as Literal;
            if (litHeaderTerm != null)
            {
                Guid termID = (Guid)((Term)e.Row.DataItem).ID;
                Term term = _template.FindTerm(termID);
                if (term != null)
                    litHeaderTerm.Text = term.IsHeader? "Header Term" : String.Empty;
                else
                    litHeaderTerm.Text = String.Empty;
            }
        }

        protected void grdTermGroup_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            int rowIndex = Convert.ToInt32(e.CommandArgument);
            int termIndex = _template.FindTermGroupIndex((Guid)grdTermGroup.DataKeys[rowIndex].Value, _template.TermGroups);

            switch (e.CommandName)
            {
                case Common.Names._GRID_COMMAND_EditRow:
                    Context.Items[Common.Names._CNTXT_Template] = _template;
                    Context.Items[Common.Names._CNTXT_EditMode] = EditMode.Edit;
                    //TermDependencyID change
                    Context.Items[Common.Names._CNTXT_TermGroupIndex] = termIndex;
                    Context.Items[Common.Names._CNTXT_TermGroupId] = _template.TermGroups[termIndex].ID;

                    //IsChanged related change
                    if (IsChanged)
                        Context.Items[Common.Names._CNTXT_IsChanged] = true;
                    Server.Transfer("TemplateTermGroupAddEdit.aspx");
                    break;

                case Common.Names._GRID_COMMAND_DeleteRow:
                    TryDeleteTermGroup(termIndex);
                    IsChanged = true;
                    break;

                case Common.Names._GRID_COMMAND_SingleClick:
                case Common.Names._GRID_COMMAND_DoubleClick:
                    //HideTermGridMoveRows(rowIndex);
                    ViewState[Common.Names._CNTXT_SelectedTermGroupIndex] = rowIndex;
                    SetTermGroupSelection(); 
                    BindGrid(grdTerm, GetTerms());
                    SetMoveUpDownButtonEvents(grdTermGroup, btnTermGroupMoveUp, btnTermGroupMoveDown);
                    break;

                default:
                    break;
            }
        }

		//See if additional delete confirmation is needed.  For example, is this term referenced elsewhere.
		//Is so, pop up a confirm dialog, and get the user's reply on the next postback
		//If not, go ahead and delete the term
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
				string termName = _template.BasicTerms[index].Name;
				List<string> references = _template.TermReferences(_template, termName);
                //Now looking at the use of the term within the DataStoreConfigurations.
                references.AddRange(Business.SystemStore.GetTermReferences(_itatSystem.ID, _template.ID, _template.BasicTerms[index].ID));
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

        private bool TryDeleteTermGroup(int index)
        {
            bool rtn = false;
            if (_deleteStatus == DeleteStatus.Verified)
            {
                DeleteTermGroup(index);
                _deleteStatus = DeleteStatus.None;
                rtn = true;
            }
            else
            {
                
                Guid termGroupID = _template.TermGroups[index].ID;
                string termGroupName = _template.TermGroups[index].Name;

                bool termGroupInUse = _template.TermGroupInUse(termGroupID);


                if (termGroupInUse)
                {
                    AddDeleteVerificationClientScripts(termGroupName, termGroupInUse);
                    _deleteStatus = DeleteStatus.Prompted;
                    rtn = true;
                }
                else
                {
                    DeleteTermGroup(index);
                    _deleteStatus = DeleteStatus.None;
                    rtn = false;
                }
            }
            return rtn;
        }

		private void DeleteTerm(int index)
		{
				_template.BasicTerms.RemoveAt(index);
				_template.Refresh();
				grdTerm.SelectedIndex = -1;
				BindGrid(grdTerm, GetTerms());
		}

        private void DeleteTermGroup(int index)
        {
            _template.TermGroups.RemoveAt(index);
            _template.Refresh();
            grdTermGroup.SelectedIndex = -1;
            BindGrid(grdTermGroup, GetTermGroups());
        }

        protected void btnSwitchTermGroupRows_Command(object sender, CommandEventArgs e)
        {
            //Identify the 2 rows to be swapped
            int selectedRow = grdTermGroup.SelectedIndex;

            //If selected row index greater than 1 enable moving up

            //If selected row greater than 0 but less than the total number of rows enable moving down.

            if ((string)e.CommandArgument == "up" && selectedRow <= 1)
                return;

            if ((string)e.CommandArgument == "down" && selectedRow <= 0)
                return;

            //Identify the grid and List<Term> objects involved (this method is used for both grids on the page)
            List<TermGroup> list;
            ImageButton upButton;
            ImageButton downButton;

            list = GetTermGroups();            
            upButton = btnTermGroupMoveUp;
            downButton = btnTermGroupMoveDown;            
            
            int otherRow = ((string)e.CommandArgument == "up" ? selectedRow - 1 : selectedRow + 1);

            //Swap the 2 rows
            list.Reverse(Math.Min(selectedRow, otherRow), 2);
            //Re-bind the grid to the list (to reflect the new order of the Terms)
            grdTermGroup.SelectedIndex = otherRow;
            BindGrid(grdTermGroup, list);
            SetMoveUpDownButtonEvents(grdTermGroup, upButton, downButton);
            SyncTermGroupsWithTemplate(list);

            IsChanged = true;
            _template.Refresh();
        }

        private void SyncTermGroupsWithTemplate(List<TermGroup> list)
        {
            //Sync to the termgroups in the template.
            for (int i = 1; i < list.Count; i++)
            {
                Guid termGroupID = (Guid)list[i].ID;
                TermGroup termGroup = _template.FindTermGroup(termGroupID);
                _template.TermGroups.Remove(termGroup);
                _template.TermGroups.Insert(i - 1, termGroup);
            }
        }

		#endregion

		#region private methods
		private void PopulateForm()
		{
			ApplyTemplateUpdates();
            if (_template.SecurityModel == SecurityModel.Basic)
            {
                divTermGroupContainer.Visible = false;
            }
            else
            {               
                BindGrid(grdTermGroup, GetTermGroups());
                //SetTermGroupSelection();
                SetMoveUpDownButtonEvents(grdTermGroup, btnTermGroupMoveUp, btnTermGroupMoveDown);
            }

            BindGrid(grdTerm, GetTerms());            
		}

        private void SetTermGroupSelection()
        {
            int selectedGroupIndex = default(int);
            if (Context.Items[Common.Names._CNTXT_SelectedTermGroupIndex] != null && int.TryParse(Context.Items[Common.Names._CNTXT_SelectedTermGroupIndex].ToString(), out selectedGroupIndex))
            {
                grdTermGroup.SelectedIndex = selectedGroupIndex;
                //Store it in the viewstate so that it can be used if posting back.
                ViewState[Common.Names._CNTXT_SelectedTermGroupIndex] = selectedGroupIndex;
            }
            else if (ViewState[Common.Names._CNTXT_SelectedTermGroupIndex] != null)
            {
                grdTermGroup.SelectedIndex = (int)ViewState[Common.Names._CNTXT_SelectedTermGroupIndex];
            }
            else
            {
                grdTermGroup.SelectedIndex = 0;
            }

            if (grdTermGroup.SelectedIndex == 0)
                ucGridMoveRows.Visible = false;
            else
                ucGridMoveRows.Visible = true;
        }

		private void SetMoveUpDownButtonEvents(Kindred.Common.Controls.KindredGridView grd, ImageButton upButton, ImageButton downButton)
		{
			int selectedRow = grd.SelectedIndex;
			
			if (selectedRow > 1) //enable move up button
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

			if (selectedRow > 0 && selectedRow < grd.Rows.Count - 1)
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

        //TODO:KKH Combine both BindGrid Methods.
		private void BindGrid(Kindred.Common.Controls.KindredGridView grd, List<Term> list)
		{
			grd.DataSource = list;
			grd.DataBind();
		}

		private void BindGrid(Kindred.Common.Controls.KindredGridView grd, List<TermGroup> list)
		{
			grd.DataSource = list;
			grd.DataBind();
		}

        private List<TermGroup> GetTermGroups()
        {
            List<TermGroup> lstTermGroups = _template.GetTermGroups(TermGroup.TermGroupType.AdvancedBasicTerm);
            lstTermGroups.Insert(0, new TermGroup("All Tabs", "All Tabs", SecurityModel.Advanced, TermGroup.TermGroupType.AdvancedBasicTerm));

            return lstTermGroups;
        }

        private List<Term> GetTerms()
        {
            List<Term> lstTerms = _template.BasicTerms;
            
            if (grdTermGroup.SelectedIndex > 0)
            {
                string termGroupID = grdTermGroup.DataKeys[grdTermGroup.SelectedIndex].Value.ToString();
                if (!String.IsNullOrEmpty(termGroupID) && Utility.TextHelper.IsGuid(termGroupID))
                {
                    lstTerms =  _template.GetTermsByTermGroupID(new Guid(termGroupID));
                }
                else
                {
                    throw new ApplicationException("Invalid Term Group");
                }
            }
            else if (grdTermGroup.SelectedIndex == 0)
            {
                lstTerms.Sort(Term.TermGroupOrderTermOrderComparison);
            }

            //Filter Attachments and comments based on the template settings

            if (!_template.AllowAttachments)
                lstTerms = BasicTerms.FindTermsOfTypeExcluding(lstTerms, TermType.PlaceHolderAttachments);

            if (!_template.AllowComments)
                lstTerms = BasicTerms.FindTermsOfTypeExcluding(lstTerms, TermType.PlaceHolderComments);

            return lstTerms;
        }

		private void SaveTemplateTerms()
		{
			_template.SaveLoaded(true, true,"Basic Terms and Tabs");

			if (_template.IsManagedItem)
				RegisterAlert(string.Format("{0} Terms have been saved.", this._itatSystem.ManagedItemName));
			else
				RegisterAlert("Template Terms have been saved.");
		}

		#endregion
	}
}
