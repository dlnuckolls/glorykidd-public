using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using Kindred.Common.Controls;
using Kindred.Knect.ITAT.Business;
using System.Collections.Generic;

namespace Kindred.Knect.ITAT.Web
{
	public partial class ManagedItemComplexList : BaseManagedItemComplexListPage
	{

		#region private members and constants

        private const string EXPORT_HIDDENFIELD_NAME = "_kh_hf_GridContents";

        private Business.ComplexList _complexList;
        private Business.StateTermGroup _stateTermGroup;
        private bool _userCanEditComplexList;
        private List<string> _validationErrors;

		#endregion

        #region Properties 

        int CheckBoxColumn
        {
            get { return (int)ViewState["CheckBoxColumn"];  }
            set { ViewState["CheckBoxColumn"] = value; }
        }

        int EditColumn
        {
            get { return (int)ViewState["EditColumn"]; }
            set { ViewState["EditColumn"] = value; }
        }

        int DeleteColumn
        {
            get { return (int)ViewState["DeleteColumn"]; }
            set { ViewState["DeleteColumn"] = value; }
        }

        #endregion

        #region BasePage overrides

        internal override HtmlGenericControl HTMLBody()
		{
			return htmlBody;
		}

		protected override ManagedItemHeader HeaderControl()
		{
			return (ManagedItemHeader)header;
		}

		internal override Control ResizablePanel()
		{
			return pnlGridBody;
		}

		protected override string GetPageName()
		{
			return "Edit Complex List";
		}

		protected override string GetApplicationFunction()
		{
			return null;
		}

        protected override void OnInit(EventArgs e)
        {
            _validationErrors = new List<string>();
            base.OnInit(e);
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            HeaderControl().Roles = _securityHelper.UserRoles.ToArray();
            _banner = string.Concat(_itatSystem.ManagedItemName, " - ", Request.QueryString[Common.Names._QS_COMPLEXLIST_NAME]);

            int index = -1;
            _complexList = GetComplexList(ref index, false);
            _stateTermGroup = GetStateTermGroup(_complexList);
            _userCanEditComplexList = Utility.ListHelper.HaveAMatch(_stateTermGroup.Editors.ConvertAll<string>(Role.StringConverter), _securityHelper.UserRoles);

            if (!IsPostBack)
            {
                grdList.SortColumn = 1;
                grdList.SortAscending = true;
                SetMoveUpDownButtonEvents(grdList, btnItemMoveUp, btnItemMoveDown);
                grdList.HeaderRowSize = _itatSystem.HeaderRowSize;
            }

        }

        protected override void OnPreRender(EventArgs e)
        {
            if (_validationErrors.Count > 0)
            {
                RenderValidationErrors(_validationErrors);
                this.IsChanged = true;
            }
            base.OnPreRender(e);
            RegisterExportClientScript();
            LoadGrid();
        }

		#endregion

		#region event handlers

		protected void grdList_RowDataBound(object sender, GridViewRowEventArgs e)
		{
			if (e.Row.RowType == DataControlRowType.DataRow)
			{
				Business.ComplexListItem item = _complexList.Items[e.Row.RowIndex];

				foreach (TableCell cell in e.Row.Cells)
				{
					//Need to set the Cell.Wrap property to 'True' if this cell corresponds to a field with BigText == true
					if (_complexList.HasBigText)
					{
						System.Web.UI.WebControls.DataControlFieldCell dcfc = cell as DataControlFieldCell;
						if (dcfc != null)
						{
							System.Web.UI.WebControls.BoundField bf = dcfc.ContainingField as System.Web.UI.WebControls.BoundField;
							if (bf != null)
							{
                                Business.ComplexListField clf = _complexList.FindField(bf.HeaderText);
								if (clf != null)
									cell.Wrap = clf.BigText ?? false;
							}
						}
					}

					foreach (Control control in cell.Controls)
					{
						//TODO:  ADDED 7/6/07
						if (control  is CheckBox)
						{
							CheckBox cbf = control as CheckBox;
							if (_complexList != null)
							{
								cbf.Enabled = item.Selectable ?? true;
							}
						}

						if (control.GetType().BaseType == typeof(LinkButton))
						{
							LinkButton linkButton = control as LinkButton;
							linkButton.CommandArgument = e.Row.RowIndex.ToString();
							linkButton.CommandName = linkButton.Text;
							if (_complexList != null)
							{
								switch (linkButton.Text)
								{
									case "Edit":
										linkButton.Visible = item.Editable ?? true;
										break;
									case "Delete":
										linkButton.Visible = item.Deletable ?? true;
										linkButton.OnClientClick = "return confirm('Are you sure you want to delete this item?');";
										break;
									default:
										break;
								}
							}
						}
					}
				}
			}
		}

		protected void grdList_RowCommand(object sender, GridViewCommandEventArgs e)
		{
			Kindred.Common.Controls.KindredGridView grd = (Kindred.Common.Controls.KindredGridView)sender;
			int rowIndex = Convert.ToInt32(e.CommandArgument);

			switch (e.CommandName)
			{
				case Common.Names._GRID_COMMAND_EditRow:
					break;

				case Common.Names._GRID_COMMAND_DeleteRow:
					break;

                //TODO:  Need to re-implement this functionality at a later date. 
				//case Common.Names._GRID_COMMAND_SingleClick:
				case Common.Names._GRID_COMMAND_DoubleClick:
					SetMoveUpDownButtonEvents(grd, btnItemMoveUp, btnItemMoveDown);                    
					break;
                case Common.Names._GRID_COMMAND_HeaderClick:
                    string commandName = (string)e.CommandName;
                    int columnNumber = int.Parse((string)e.CommandArgument);
                    if (!SkipColumn(columnNumber))
                    {
                        if (columnNumber == grdList.SortColumn)
                        {
                            grdList.SortAscending = !grdList.SortAscending;
                        }
                        else
                        {
                            grdList.SortColumn = columnNumber;
                            grdList.SortAscending = true;
                        }

                        SortComplexList(columnNumber, grdList.SortAscending);
                    }
                    break;
				default:
					break;
			}
		}

		protected void grdList_RowEditing(object sender, GridViewEditEventArgs e)
		{
			int nRowIndex = e.NewEditIndex;
			string listName = Request.QueryString[Common.Names._QS_COMPLEXLIST_NAME];
			string sQueryString = Utility.TextHelper.QueryString(true, Common.Names._QS_ITAT_SYSTEM_ID, ITATSystem.ID.ToString(), Common.Names._QS_MANAGED_ITEM_ID, this.ManagedItem.ManagedItemID.ToString(), Common.Names._QS_COMPLEXLIST_NAME, listName, Common.Names._QS_COMPLEXLIST_INDEX, nRowIndex.ToString());
			Context.Items[Common.Names._CNTXT_ManagedItem] = _managedItem;
			string sURL = string.Concat("~/ManagedItemComplexListEdit.aspx", sQueryString);
            Server.Transfer(sURL);
		}
		
		protected void grdList_RowDeleting(object sender, GridViewDeleteEventArgs e)
		{
			int nIndex = -1;
			Business.ComplexList complexList = GetComplexList(ref nIndex, false);
			if (complexList == null)
				return;
			complexList.Items.RemoveAt(e.RowIndex);
			if (grdList.SelectedIndex == e.RowIndex)
				grdList.SelectedIndex = -1;
			IsChanged = true;
			LoadGrid();
		}

        protected void btnSwitchRows_Command(object sender, CommandEventArgs e)
        {
            if (_complexList != null)
            {
                ImageButton upButton;
                ImageButton downButton;
                upButton = btnItemMoveUp;
                downButton = btnItemMoveDown;

                //Identify the 2 rows to be swapped
                int selectedRow = grdList.SelectedIndex;
                int otherRow = ((string)e.CommandArgument == "up" ? selectedRow - 1 : selectedRow + 1);

                //Swap the 2 rows
                _complexList.Items.Reverse(Math.Min(selectedRow, otherRow), 2);
                //Re-bind the grid to the list (to reflect the new order of the Terms)
                grdList.SelectedIndex = otherRow;
                LoadGrid();
                SetMoveUpDownButtonEvents(grdList, upButton, downButton);
                IsChanged = true;
            }
        }

        private bool SkipColumn(int columnIndex)
        {
            if (CheckBoxColumn == columnIndex)
                return true;

            if (EditColumn == columnIndex)
                return true;

            if (DeleteColumn == columnIndex)
                return true;

            return false;
        }

        protected void OnHeaderEvent(object sender, HeaderEventArgs e)
        {
            switch (e.CommandName)
            {
                case "Save":
                    Dictionary<Guid /*TermGroupID*/, bool /*CanAccess*/> canAccessTermGroup = BuildEditableTermGroups();
                    Dictionary<string /* Term Name */, List<string> /* Error Messages */ > termTypeErrors = GetTermTypeErrors(canAccessTermGroup);
                    _validationErrors.AddRange(GetValidationErrors(ManagedItemValidationType.ValidateOnSave, termTypeErrors, canAccessTermGroup));
                    foreach (ComplexList complexList in _managedItem.ComplexLists)
                    {
                        Dictionary<string /* Term Name */, List<string> /* Error Messages */ > complexListTermTypeErrors = Helper.GetComplexListTermTypeErrors(canAccessTermGroup, complexList, null);
                        _validationErrors.AddRange(Helper.GetComplexListValidationErrors(canAccessTermGroup, ManagedItemValidationType.ValidateOnSave, termTypeErrors, complexList.Items, true, _managedItem.SecurityModel, _managedItem.State.Status, complexList.TermGroupID, complexList.Name));
                    }

                    if (_validationErrors.Count == 0)
                    {
                        UpdateComplexListItems();
                        //DEG_20100212 Change made due to inadvertent orphaning after Complex List changes.
                        _managedItem.Update(false, Business.Retro.AuditType.Saved);
                        IsChanged = false;
                    }
                    break;

                case "Reset":
                    Server.Transfer(Request.Url.PathAndQuery);
                    IsChanged = false;
                    break;

                default:
                    break;

            }
        }

        protected void btnAdd_Click(object sender, EventArgs e)
        {
            int nIndex = -1;
            Business.ComplexList complexList = GetComplexList(ref nIndex, false);
            if (complexList == null)
                return;

            Business.ComplexListItem item = new Business.ComplexListItem();

            item.Editable = true;
            item.Deletable = true;
            item.Selectable = true;
            item.Selected = true;
            item.Default = false;
            foreach (Business.ComplexListField field in complexList.Fields)
            {
                Business.ComplexListItemValue itemValue = new Business.ComplexListItemValue(complexList, field.FilterTerm);
                itemValue.BigText = field.BigText ?? false;
                itemValue.FieldID = field.ID;
                if (itemValue.DefaultValueDefined)
                    itemValue.FieldValue = itemValue.DefaultValue;
                else
                    itemValue.FieldValue = string.Empty;
                item.ItemValues.Add(itemValue);
            }
            complexList.Items.Add(item);

            int nRowIndex = complexList.Items.Count - 1;
            string listName = Request.QueryString[Common.Names._QS_COMPLEXLIST_NAME];
            string sQueryString = Utility.TextHelper.QueryString(true, Common.Names._QS_ITAT_SYSTEM_ID, ITATSystem.ID.ToString(), Common.Names._QS_MANAGED_ITEM_ID, this.ManagedItem.ManagedItemID.ToString(), Common.Names._QS_COMPLEXLIST_NAME, listName, Common.Names._QS_COMPLEXLIST_INDEX, nRowIndex.ToString());
            Context.Items[Common.Names._CNTXT_ManagedItem] = _managedItem;
            Context.Items[Common.Names._CNTXT_EditMode] = EditMode.Add;
            string sURL = string.Concat("~/ManagedItemComplexListEdit.aspx", sQueryString);
            IsChanged = true;
            Server.Transfer(sURL);
        }

        protected void btnExportToExcel_Click(object sender, EventArgs e)
        {
            int nIndex = -1;
            Business.ComplexList complexList = GetComplexList(ref nIndex, false);
            if (complexList == null)
                return;

            string gridContents = Request.Form[EXPORT_HIDDENFIELD_NAME];
            if (!string.IsNullOrEmpty(gridContents))
                ExportToExcel(string.Format("{0}-{1}", _managedItem.ItemNumber.Trim(), complexList.Name.Trim()), gridContents);
        }

        #endregion

		#region Private methods

        private void ExportToExcel(string reportName, string gridContents)
        {
            Response.Clear();
            Response.ContentType = Utility.WebServiceHelper.ExtensionToMimeType("xls");
            Response.AddHeader("Content-Disposition", string.Format("attachment;filename={0}.xls", reportName));
            Response.Write(gridContents);
            Response.End();
        }

        private void RegisterExportClientScript()
        {
            //this javascript will get executed CLIENT-SIDE when the Export button is clicked
            Page.ClientScript.RegisterHiddenField(EXPORT_HIDDENFIELD_NAME, string.Empty);

            string scriptBlockName = "_kh_ExportGridToExcel";
            System.IO.StringWriter scriptBlock = new System.IO.StringWriter();
            scriptBlock.WriteLine("function SaveGrid()");
            scriptBlock.WriteLine("{");
            scriptBlock.WriteLine("	var hf = document.getElementById('{0}');", EXPORT_HIDDENFIELD_NAME);
            scriptBlock.WriteLine("	var grd = document.getElementById('{0}');", grdList.ClientID);
            scriptBlock.WriteLine("	if (grd && hf)");
            scriptBlock.WriteLine("	{ ");
            scriptBlock.WriteLine("		var htmlarray = grd.outerHTML.split(\"\\n\");");
            scriptBlock.WriteLine("		var htmlOutput = '';");
            scriptBlock.WriteLine("		for (i = 0;i < htmlarray.length;i++)");
            scriptBlock.WriteLine("		{");
            scriptBlock.WriteLine("		    var hasCheckBoxHeader = htmlarray[i].indexOf('<TH style=\"WIDTH: 50px\" scope=col>Select</TH>') != -1");
            scriptBlock.WriteLine("		    var hasBtnHeader = htmlarray[i].indexOf('<TH style=\"WIDTH: 60px\" scope=col>&nbsp;</TH>') != -1");
            scriptBlock.WriteLine("		    var hasCheckBox = htmlarray[i].indexOf('type=checkbox') != -1");
            scriptBlock.WriteLine("		    var hasBtn = htmlarray[i].indexOf('href=\"javascript:__doPostBack') != -1");
            scriptBlock.WriteLine("		    if (!hasCheckBoxHeader && !hasBtnHeader && !hasCheckBox && !hasBtn)");
            scriptBlock.WriteLine("		    {");
            scriptBlock.WriteLine("		        htmlOutput = htmlOutput + htmlarray[i] + \"\\n\"");
            scriptBlock.WriteLine("		    }");
            scriptBlock.WriteLine("		}");
            scriptBlock.WriteLine("		hf.value = htmlOutput.replace('style=\"HEIGHT: 0px\"','').replace('border=1','border=0');");  //fixes minor formatting issues in Excel
            scriptBlock.WriteLine("		return true;");
            scriptBlock.WriteLine("	} ");
            scriptBlock.WriteLine("	else");
            scriptBlock.WriteLine("		return false;");
            scriptBlock.WriteLine("}");
            if (!ClientScript.IsClientScriptBlockRegistered(scriptBlockName))
                ClientScript.RegisterClientScriptBlock(this.GetType(), scriptBlockName, scriptBlock.ToString(), true);
            scriptBlock.Close();
        }
        
        private void SetMoveUpDownButtonEvents(Kindred.Common.Controls.KindredGridView grd, ImageButton upButton, ImageButton downButton)
        {
            StateTermGroup stateTermGroup = GetStateTermGroup(_complexList);
            bool userIsComplexListEditor = Utility.ListHelper.HaveAMatch(stateTermGroup.Editors.ConvertAll<string>(Role.StringConverter), _securityHelper.UserRoles);

            if (userIsComplexListEditor)
            {
                btnAdd.Visible = true;
                btnItemMoveUp.Visible = true;
                btnItemMoveDown.Visible = true;
                grd.RowHighlighting = true;
                grd.EnableClickEvent = true;
                grd.EnableHeaderClick = true;

                int selectedRow = grd.SelectedIndex;

                if (selectedRow > 0) //enable move up button
                {
                    upButton.ImageUrl = "~/Images/MoveUp.gif";
                    upButton.Enabled = true;
                    upButton.Style["cursor"] = "pointer";
                    upButton.CommandName = "List";
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
                    upButton.CommandName = "List";
                    downButton.CommandArgument = "down";
                }
                else
                {
                    downButton.ImageUrl = "~/Images/MoveDownDisabled.gif";
                    downButton.Enabled = false;
                    downButton.Style["cursor"] = "default";
                }
            }
            else
            {
                btnAdd.Visible = false;
                btnItemMoveUp.Visible = false;
                btnItemMoveDown.Visible = false;
                grd.RowHighlighting = false;
                grd.EnableClickEvent = false;
                grd.EnableHeaderClick = false;
            }
        }

        private void SortComplexList(int columnNumber, bool sortAscending)
        {
            if (columnNumber > 0) //Ignore the checkbox column
            {
                //Business.ComplexList complexList;
                DataSet dsComplexList = BuildComplexListData();

                //Sort Column

                string sortColumnName = grdList.Columns[columnNumber].ToString();

                //Sort Order
                string sortType = default(string);
                if (sortAscending)
                    sortType = "ASC";
                else
                    sortType = "DESC";

                string sortCriteria = String.Concat(sortColumnName, " ", sortType);

                DataView dvComplexList = new DataView(dsComplexList.Tables[0]);
                dvComplexList.Sort = sortCriteria;

                for (int i = 0; i < dvComplexList.Count; i++)
                {
                    //Get the index of the item
                    Guid itemID = new Guid(dvComplexList[i]["Index"].ToString());
                    //Find the item where it is in the list and delete it
                    ComplexListItem cli = _complexList.FindItem(itemID);
                    _complexList.Items.Remove(cli);
                    _complexList.Items.Insert(i, cli);
                }
                IsChanged = true;
            }
        }

		private void LoadGrid()
		{
            DataSet complexListData = BuildComplexListData();

			grdList.Columns.Clear();

			//TODO:   ADDED 7/6/07
			if (_userCanEditComplexList)
			{
				CheckBoxField cbf = new CheckBoxField();
				cbf.DataField = "Selected";
				cbf.HeaderText = "Select";
				cbf.ItemStyle.HorizontalAlign = HorizontalAlign.Center;
				cbf.Text = "";
				cbf.HeaderStyle.Width = Unit.Parse("50px");
				cbf.Visible = true;
				cbf.SortExpression = String.Empty;
				grdList.Columns.Add(cbf);
                CheckBoxColumn = grdList.Columns.Count - 1;
			}

			foreach (Business.ComplexListField field in _complexList.Fields)
			{
				if (field.Summary ?? false)
				{
					BoundField bnf = new BoundField();
					bnf.HeaderText = field.Name;
					bnf.DataField = field.Name.Trim();      //Note - extra spaces play havoc with the table generation.
					grdList.Columns.Add(bnf);
				}
			}

			if (_userCanEditComplexList)
			{
				ButtonField btf = new ButtonField();
				btf.HeaderText = "";
				btf.ItemStyle.HorizontalAlign = HorizontalAlign.Center;
				btf.Text = "Edit";
				btf.ButtonType = ButtonType.Link;
				btf.HeaderStyle.Width = Unit.Parse("60px");
				btf.Visible = true;
				btf.SortExpression = String.Empty;
				grdList.Columns.Add(btf);
                EditColumn = grdList.Columns.Count - 1;

				btf = new ButtonField();
				btf.HeaderText = "";
				btf.ItemStyle.HorizontalAlign = HorizontalAlign.Center;
				btf.Text = "Delete";
				btf.ButtonType = ButtonType.Link;
				btf.HeaderStyle.Width = Unit.Parse("60px");
				btf.Visible = true;
				btf.SortExpression = String.Empty;
				grdList.Columns.Add(btf);
                DeleteColumn = grdList.Columns.Count - 1;
            }

            grdList.DataSource = complexListData;
			grdList.DataBind();
		}

        private DataSet BuildComplexListData()
        {
            DataTable dt = new DataTable();

            dt.Columns.Add("Index", typeof(System.Guid));
            dt.Columns.Add("Selected", typeof(bool));
            foreach (Business.ComplexListField field in _complexList.Fields)
            {
                if (field.Summary ?? false)
                {
                    dt.Columns.Add(field.Name.Trim());      //Note - extra spaces play havoc with the table generation.
                }
            }

            //Add in the items
            foreach (Business.ComplexListItem listItem in _complexList.Items)
            {
                DataRow row = dt.NewRow();
                row["Index"] = listItem.ID;
                row["Selected"] = listItem.Selected ?? false;    //TODO:   ADDED 7/6/07
                foreach (Business.ComplexListField field in _complexList.Fields)
                {
                    if (field.Summary ?? false)
                    {
                        string sFieldName = field.Name;
                        foreach (Business.ComplexListItemValue itemValue in listItem.ItemValues)
                        {
                            if (itemValue.FieldName == sFieldName)
                            {
                                row[field.Name.Trim()] = itemValue.DisplayValue;        //Note - extra spaces play havoc with the table generation.
                                break;
                            }
                        }
                    }
                }
                dt.Rows.Add(row);
            }

            DataSet ds = new DataSet();
            ds.Tables.Add(dt);
            return ds;
        }

		private void UpdateComplexListItems()
		{
			foreach (GridViewRow row in grdList.Rows)
			{
				if (row.RowType == DataControlRowType.DataRow)
				{
					int nIndex = -1;
					Business.ComplexList complexList = GetComplexList(ref nIndex, false);
					Business.ComplexListItem item = complexList.Items[row.RowIndex];

					foreach (DataControlFieldCell fieldCell in row.Controls)
					{
						if (fieldCell.ContainingField.HeaderText == "Select")
						{
							Control control = fieldCell.Controls[0];
							if (control is CheckBox)
							{
								CheckBox cb = control as CheckBox;
								item.Selected = cb.Checked;
								break;
							}
						}
					}
				}
			}
		}

		#endregion
	}
}
