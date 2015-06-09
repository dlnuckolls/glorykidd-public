using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Data;
using Kindred.Knect.ITAT.Business;

namespace Kindred.Knect.ITAT.Web
{
    public partial class TermEditComplexListItems : BaseTermEditPage
    {
        #region private members and constants
        Business.ComplexList _complexList;
        bool _userCanEditComplexList = true;

        private const string VSKEY_CURR_DEF_ITEMS_PARAMETERID = "VSKEY_CURR_DEF_ITEMS_PARAMETERID";
        private const string VSKEY_NEW_DEF_ITEMS_PARAMETERID = "VSKEY_NEW_DEF_ITEMS_PARAMETERID";
        private const string VSKEY_DEF_ITEMS_STRUCTURE_PARAMETERID = "VSKEY_DEF_ITEMS_STRUCTURE_PARAMETERID";
        #endregion

        #region baseOverride

        protected override TextBox TermNameControl()
        {
            return null;        // txtTermName;
        }

        protected override void ShowHideFields()
        {
        }

        protected override void UpdateValues()
        {
        }

        protected override void LoadValues()
        {
        }

        protected override void InitializeForm()
        {
        }

        protected override System.Collections.Generic.List<string> ValidateForm()
        {
            List<string> rtn = new List<string>();

            return rtn;
        }

        protected override string GetPageName()
        {
            return "Complex List Default Items";
        }

        protected override TemplateHeader HeaderControl()
        {
            return (TemplateHeader)header;
        }

        internal override HtmlGenericControl HTMLBody()
        {
            return this.htmlBody;
        }

        internal override Control ResizablePanel()
        {
			return pnlListGridBody;
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

        #region Events

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            _complexList = (ComplexList)_template.ComplexLists[TermIndex];
            _userCanEditComplexList = true;
            BannerOverride = "Template - " + _complexList.Name;
            SubTitleOverride = "";
            grdList.HeaderRowSize = _itatSystem.HeaderRowSize;

            LoadGrid();
        }

        protected void btnAdd_Click(object sender, EventArgs e)
        {
            BaseTransfer(EditMode.Add, TermHandler.ComplexListItem, "TermEditComplexListItem.aspx", false);
        }

        protected void grd_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            Kindred.Common.Controls.KindredGridView grd = (Kindred.Common.Controls.KindredGridView)sender;
            int rowIndex = Convert.ToInt32(e.CommandArgument);

            switch (e.CommandName)
            {
                case Common.Names._GRID_COMMAND_EditRow:
                    //IsChanged related change
                    if (IsChanged)
                        Context.Items[Common.Names._CNTXT_IsChanged] = true;
                    Context.Items[Common.Names._CNTXT_Template] = _template;

                    BaseTermEditPage.TermEdit localTermEdit = termEdit;
                    localTermEdit.EditMode = EditMode.Edit;
                    localTermEdit.ComplexListItemIndex = rowIndex;
                    Context.Items[Common.Names._CNTXT_TermEdit] = localTermEdit;
                    Server.Transfer("TermEditComplexListItem.aspx");
                    break;

                case Common.Names._GRID_COMMAND_DeleteRow:
                    Business.ComplexList complexList = _template.ComplexLists[termEdit.TermIndex] as ComplexList;
                    if (complexList == null)
                        return;
                    complexList.Items.RemoveAt(rowIndex);
                    if (grdList.SelectedIndex == rowIndex)
                        grdList.SelectedIndex = -1;
                    IsChanged = true;
                    LoadGrid();
                    break;

                case Common.Names._GRID_COMMAND_SingleClick:
                case Common.Names._GRID_COMMAND_DoubleClick:
                    break;

                default:
                    break;
            }
        }
        protected void grdList_RowCreated(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                //Assign each row an ID containing the RowIndex
                string rowIndex = e.Row.RowIndex.ToString();
                e.Row.ID = String.Concat("R_", grdList.DataKeys[e.Row.RowIndex].Value);
                //Set the command arguments for the Edit and Delete buttons
                ((LinkButton)e.Row.Cells[e.Row.Cells.Count - 2].Controls[0]).CommandArgument = rowIndex;// grdList.DataKeys[e.Row.RowIndex].Value.ToString();   //Edit button CommandArgument
                ((LinkButton)e.Row.Cells[e.Row.Cells.Count - 1].Controls[0]).CommandArgument = rowIndex;   //Delete button CommandArgument
                ((LinkButton)e.Row.Cells[e.Row.Cells.Count - 2].Controls[0]).CommandName = Common.Names._GRID_COMMAND_EditRow;
                ((LinkButton)e.Row.Cells[e.Row.Cells.Count - 1].Controls[0]).CommandName = Common.Names._GRID_COMMAND_DeleteRow;
                //Set up client-side script to prompt the user if they click the Delete button
                ((LinkButton)e.Row.Cells[e.Row.Cells.Count - 1].Controls[0]).OnClientClick = "return confirm('Are you sure you want to delete this row?');";
            }
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
                    dt.Columns.Add(field.Name);
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
                                row[field.Name] = itemValue.DisplayValue;
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

        private void LoadGrid()
        {
            DataSet complexListData = BuildComplexListData();

            grdList.Columns.Clear();
            if (_userCanEditComplexList)
            {
                CheckBoxField cbf = new CheckBoxField();
                cbf.DataField = "Selected";
                cbf.HeaderText = "Select";
                cbf.ItemStyle.HorizontalAlign = HorizontalAlign.Center;
                cbf.Text = "";
                cbf.HeaderStyle.Width = Unit.Parse("50px");
                cbf.Visible = true;
                cbf.ReadOnly = true;
                cbf.SortExpression = String.Empty;
                grdList.Columns.Add(cbf);
            }

            foreach (Business.ComplexListField field in _complexList.Fields)
            {
                if (field.Summary ?? false)
                {
                    BoundField bnf = new BoundField();
                    bnf.HeaderText = field.Name;
                    bnf.DataField = field.Name;
                    if (field.FilterTerm.TermType == TermType.Date)
                    {
                        bnf.ItemStyle.Width = Unit.Pixel(120);
                        bnf.HeaderStyle.Width = Unit.Pixel(120);
                        bnf.FooterStyle.Width = Unit.Pixel(120);
                    }
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

                btf = new ButtonField();
                btf.HeaderText = "";
                btf.ItemStyle.HorizontalAlign = HorizontalAlign.Center;
                btf.Text = "Delete";
                btf.ButtonType = ButtonType.Link;
                btf.HeaderStyle.Width = Unit.Parse("60px");
                btf.Visible = true;
                btf.SortExpression = String.Empty;
                grdList.Columns.Add(btf);
            }
            
            grdList.DataSource = complexListData;
            grdList.DataBind();
        }

        protected void btnOK_Click(object sender, EventArgs e)
        {
            BaseTransfer(null, TermHandler.ComplexList, "TermEditComplexList.aspx", true);
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            BaseTransfer(null, TermHandler.ComplexList, "TermEditComplexList.aspx", false);
        }

        #endregion
    }
}