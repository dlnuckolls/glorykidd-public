using System;
using System.Data;
using System.Configuration;
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
	public partial class TermEditComplexList : BaseTermEditPage
	{
		private const string VSKEY_INITIALIZED = "VSKEY_INITIALIZED";
		private const string VSKEY_FIELDS_IN_USE = "VSKEY_FIELDS_IN_USE";
		private bool _initialized = false;
		private string _fieldsInUse = "";

		internal override HtmlGenericControl HTMLBody()
		{
			return this.body;
		}

		protected override TextBox TermNameControl()
		{
			return txtTermName;
		}

		internal override Control ResizablePanel()
		{
			return pnlEditorContainer;
		}

		protected override void OnPreRender(EventArgs e)
		{
			InitializeEditorControls();
			base.OnPreRender(e);

            txtTermName.Text = TermName;

			ShowHideFields();

			List<string> complexLists = Business.ComplexLists.TermReferences(_template,edtBody.Html);
			complexLists.AddRange(Business.ComplexLists.TermReferences(_template, edtStandardHeader.Html));
			complexLists.AddRange(Business.ComplexLists.TermReferences(_template,edtAlternateHeader.Html));
			if (complexLists.Count > 0)
			{
				_fieldsInUse = string.Join(",", complexLists.ToArray());
			}

			AddClientScripts();
		}

		private void InitializeEditorControls()
		{
			Helper.RegisterParagraphWrapperScript(this);
            Business.ComplexList complexList = Term as Business.ComplexList;
			BuildFieldsToolbarItem(edtBody, complexList);
			BuildTermsToolbarItem(edtStandardHeader);
			BuildTermsToolbarItem(edtAlternateHeader);
			RegisterEditorAction();
		}

		private void BuildFieldsToolbarItem(Telerik.WebControls.RadEditor edt, Business.ComplexList complexList)
		{
			Telerik.WebControls.RadEditorUtils.ToolbarDropDown tddTerms = (Telerik.WebControls.RadEditorUtils.ToolbarDropDown)edt.Toolbars["DynamicToolbar"].Tools["Insert Field"];
			tddTerms.DropDownHeight = Unit.Parse("240px");
			tddTerms.ShowText = false;
			tddTerms.Items.Clear();
			foreach (Business.ComplexListField field in complexList.Fields)
				tddTerms.Items.Add(new Telerik.WebControls.RadEditorUtils.ListItem(field.Name, field.Name));
		}

		private void BuildTermsToolbarItem(Telerik.WebControls.RadEditor edt)
		{
			Telerik.WebControls.RadEditorUtils.ToolbarDropDown tdd = (Telerik.WebControls.RadEditorUtils.ToolbarDropDown)edt.Toolbars["DynamicToolbar"].Tools["Insert Field"];
			Helper.InitializeToolBarItems(this, tdd);
			Helper.AddSpecialToolBarItems(this, tdd);
			Helper.AddTermsToolBarItems(this, tdd, _template.BasicTerms);
		}

		private void RegisterEditorAction()
		{
			Type t = this.GetType();
			string scriptName = "_kh_InsertFieldAction";
			System.IO.StringWriter sw = new System.IO.StringWriter();
			sw.WriteLine("RadEditorCommandList[\"Insert Field\"] = function(commandName, editor, oTool) ");
			sw.WriteLine("{");
			sw.WriteLine("	oValue = oTool.GetSelectedValue();");
			//20070817_DEG  Bug 131 - Special case - replace the quotes
			sw.WriteLine("	oValue = oValue.replace(/\"/g,\"&quot;\");");
			sw.WriteLine("	editor.PasteHtml('<img class=\"TermImg\" src=\"TextImage.ashx?text=' + oValue + '\"/>');");
			sw.WriteLine("}");
			if (!ClientScript.IsStartupScriptRegistered(t, scriptName))
				ClientScript.RegisterStartupScript(t, scriptName, sw.ToString(), true);
		}

		private void AddClientScripts()
		{
			Type t = this.GetType();
            string scriptName = "_kh_DeleteClicked";
            if (!ClientScript.IsClientScriptBlockRegistered(t, scriptName))
            {
                System.IO.StringWriter swClientScript = new System.IO.StringWriter();
                string sConfirmationText = "This field is currently being used by the editor.  Please delete all instances of it from the text before deleting the field. However, if this is the last use of this field, then click OK to continue with the delete.";
                swClientScript.WriteLine("function {0}(fieldName)", scriptName);
                swClientScript.WriteLine("{");
                //Check whether or not the selected field is being used in the text
                swClientScript.WriteLine("	var fieldsInUse = document.forms['{0}']['{1}'].value;", GetFormName(), VSKEY_FIELDS_IN_USE);
                swClientScript.WriteLine("	if (fieldsInUse)");
                swClientScript.WriteLine("	{");
                swClientScript.WriteLine("		var fieldsInUseArray = new Array();");
                swClientScript.WriteLine("		fieldsInUseArray = fieldsInUse.split(',');");
                swClientScript.WriteLine("		for (var i = 0;i < fieldsInUseArray.length;i++)");
                swClientScript.WriteLine("		{");
                swClientScript.WriteLine("			if (fieldsInUseArray[i] == fieldName)");
                swClientScript.WriteLine("			{");
                swClientScript.WriteLine("				var confirmValue = confirm('{0}');", sConfirmationText);
                swClientScript.WriteLine("				if (!confirmValue)");
                swClientScript.WriteLine("					return false;");
                swClientScript.WriteLine("			}");
                swClientScript.WriteLine("		}");
                swClientScript.WriteLine("	}");
                //Check whether or not default items are defined before deleting

                Business.ComplexList complexList = Term as Business.ComplexList;
                if (complexList.ItemsDefined)
                {
                    string confirmationText = "There are default items defined. Click OK to continue with deletion.";
                    swClientScript.WriteLine("	return confirm('{0}');", confirmationText);
                }
                else
                {
                    swClientScript.WriteLine("	return true;");
                }
                swClientScript.WriteLine("}");
                ClientScript.RegisterClientScriptBlock(t, scriptName, swClientScript.ToString(), true);
            }
		}

		protected override void OnInit(EventArgs e)
		{
            base.OnInit(e);
		}

		protected void Page_Load(object sender, EventArgs e)
		{
			if (_initialized)
				LoadPage();
		}

		protected void LoadPage()
		{
			if (IsPostBack)
			{
                TermName = txtTermName.Text;
			}
			else
			{
                header.PageTitle = "Edit Complex List";
                Business.ComplexList complexList = Term as Business.ComplexList;
				if (complexList != null)
				{
                    BindGrid(complexList.Fields);
                }
                SetMoveUpDownButtonEvents(grdFieldList, btnTermGroupMoveUp, btnTermGroupMoveDown);
            }
			_initialized = true;
		}

		protected override void InitializeForm()
		{
			LoadPage();
		}

		private string ValidatedXHTML(string html)
		{
			string rtn = html;
			if (!rtn.StartsWith("<p"))
			{
				rtn = string.Concat(@"<p align=""left"">", rtn, @"</p>");
			}
			return rtn;
		}

        protected void btnSwitchTermGroupRows_Command(object sender, CommandEventArgs e)
        {
            //Identify the 2 rows to be swapped
            int selectedRow = grdFieldList.SelectedIndex;

            //If selected row index greater than 1 enable moving up
            //If selected row greater than 0 but less than the total number of rows enable moving down.

            if ((string)e.CommandArgument == "up" && selectedRow <= 0)
                return;

            if ((string)e.CommandArgument == "down" && selectedRow >= grdFieldList.Rows.Count - 1)
                return;

            //Identify the grid and List<Term> objects involved (this method is used for both grids on the page)
            ImageButton upButton;
            ImageButton downButton;

            Business.ComplexList complexList = Term as Business.ComplexList;
            List<ComplexListField> list = complexList.Fields;

            upButton = btnTermGroupMoveUp;
            downButton = btnTermGroupMoveDown;

            int otherRow = ((string)e.CommandArgument == "up" ? selectedRow - 1 : selectedRow + 1);

            //Swap the 2 rows
            list.Reverse(Math.Min(selectedRow, otherRow), 2);
            //Re-bind the grid to the list (to reflect the new order of the Terms)
            grdFieldList.SelectedIndex = otherRow;
            BindGrid(list);
            SetMoveUpDownButtonEvents(grdFieldList, upButton, downButton);
            IsChanged = true;
        }

		protected override void UpdateValues()
		{
            Business.ComplexList complexList = Term as Business.ComplexList;
			if (complexList == null)
                throw new Exception(string.Format("Unable to cast _term \"{0}\" to a Business.ComplexList object.", TermName));

			TermName = txtTermName.Text;
			complexList.ColumnCount = 1; // int.Parse(ddlColumnCount.Items[ddlColumnCount.SelectedIndex].Text);
			complexList.KeywordSearchable = chkbxKeywordSearchable.Checked;
			complexList.ShowOnItemSummary = chkbxShowOnItemSummary.Checked;

			complexList.Rendering = ValidatedXHTML(edtBody.Html);
			complexList.StandardHeader = ValidatedXHTML(Business.Term.SubstituteTermNames(_template,edtStandardHeader.Html));
			complexList.AlternateHeader = ValidatedXHTML(Business.Term.SubstituteTermNames(_template,edtAlternateHeader.Html));
		}

		protected override List<string> ValidateForm()
		{
			List<string> rtn = new List<string>();
			return rtn;
		}

		protected override void LoadValues()
		{
            Business.ComplexList complexList = Term as Business.ComplexList;
            if (complexList == null)
                throw new Exception(string.Format("Unable to cast _term \"{0}\" to a Business.ComplexList object.", TermName));

            chkbxKeywordSearchable.Checked = complexList.KeywordSearchable ?? false;
            chkbxShowOnItemSummary.Checked = complexList.ShowOnItemSummary;

            if (complexList.Fields != null)
            {
                BindGrid(complexList.Fields);
            }

            edtBody.Html = complexList.Rendering;
            edtStandardHeader.Html = Business.Term.SubstituteTermIDs(_template, complexList.StandardHeader);
            edtAlternateHeader.Html = Business.Term.SubstituteTermIDs(_template, complexList.AlternateHeader);

            UpdateBodyHTML(termEdit.OldFieldName, termEdit.NewFieldName);
        }

		protected override void ShowHideFields()
		{
		}

		protected void btnDefaultItems_Click(object sender, EventArgs e)
		{
            BaseTransfer(EditMode.Edit, TermHandler.ComplexListItem, "TermEditComplexListItems.aspx", false);
        }

        private void UpdateBodyHTML(string oldFieldName, string newFieldName)
        {
            if (!string.IsNullOrEmpty(oldFieldName) && !string.IsNullOrEmpty(newFieldName) && !oldFieldName.Equals(newFieldName))
            {
                string sHTML = edtBody.Html;
                if (!string.IsNullOrEmpty(sHTML))
                {
                    sHTML = ValidatedXHTML(sHTML);
                    Business.ComplexLists.ReplaceComplexTermNames(oldFieldName, newFieldName, ref sHTML);
                    edtBody.Html = sHTML;
                }
            }
        }
            
		protected void btnTabOnCommand(object sender, CommandEventArgs e)
		{
			pnlEditorBody.Visible = false;
			pnlEditorStandardHeader.Visible = false;
			pnlEditorAlternateHeader.Visible = false;
			btnTabBody.CssClass = "TabButton";
			btnTabStandardHeader.CssClass = "TabButton";
			btnTabAlternateHeader.CssClass = "TabButton";

			switch (e.CommandName)
			{
				case "ContentBody":
					pnlEditorBody.Visible = true;
					btnTabBody.CssClass = "TabButtonSelected";
					break;
				case "ContentStandardHeader":
					pnlEditorStandardHeader.Visible = true;
					btnTabStandardHeader.CssClass = "TabButtonSelected";
					break;
				case "ContentAlternateHeader":
					pnlEditorAlternateHeader.Visible = true;
					btnTabAlternateHeader.CssClass = "TabButtonSelected";
					break;
				default: 
					break;
			}
		}

		protected void btnOK_Click(object sender, EventArgs e)
		{
            SetContextDataAndReturn(txtTermName.Text, true, false);
		}

		protected void btnCancel_Click(object sender, EventArgs e)
		{
            SetContextDataAndReturn(txtTermName.Text, false, false);
		}

		protected override object SaveViewState()
		{
			ClientScript.RegisterHiddenField(VSKEY_INITIALIZED, _initialized.ToString());
			ClientScript.RegisterHiddenField(VSKEY_FIELDS_IN_USE, _fieldsInUse);
			return base.SaveViewState();
		}

		protected override void LoadViewState(object savedState)
		{
			base.LoadViewState(savedState);
			_initialized = bool.Parse((string)Request.Form[VSKEY_INITIALIZED]);
			_fieldsInUse = (string)Request.Form[VSKEY_FIELDS_IN_USE];
		}

        #region New Code

        // grid for complex list fields
        protected void btnAdd_Command(object sender, CommandEventArgs e)
        {
            if ((string)e.CommandArgument == "Field")
            {
                Context.Items[Common.Names._CNTXT_Template] = _template;

                BaseTermEditPage.TermEdit termEdit = new TermEdit();
                termEdit.EditMode = EditMode.Add;
                termEdit.TermHandler = TermHandler.ComplexListField;
                termEdit.TermIndex = TermIndex;

                Context.Items[Common.Names._CNTXT_TermEdit] = termEdit;
                Server.Transfer("TermAdd.aspx", true);
                return;
            }
        }

        protected void grdFieldList_RowCreated(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                //Assign each row an ID containing the RowIndex
                string rowIndex = e.Row.RowIndex.ToString();
                e.Row.ID = "R" + rowIndex;

                Business.ComplexListField clf = (Business.ComplexListField)e.Row.DataItem;
                string fieldName = string.Empty;
                if (clf != null)
                    fieldName = ((Business.ComplexListField)e.Row.DataItem).Name;

                //Set the command arguments for the Edit and Delete buttons
                ((LinkButton)e.Row.Cells[2].Controls[0]).CommandArgument = rowIndex;   //Edit button CommandArgument
                SuppressChangeNotification((WebControl)e.Row.Cells[2].Controls[0]);
                ((LinkButton)e.Row.Cells[3].Controls[0]).CommandArgument = rowIndex;   //Delete button CommandArgument

                //Set up client-side script to prompt the user if they click the Delete button
                ((LinkButton)e.Row.Cells[3].Controls[0]).OnClientClick = string.Format("javascript:return _kh_DeleteClicked('{0}');", fieldName);

            }
        }

        protected void grdFieldList_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            int rowIndex = Convert.ToInt32(e.CommandArgument);
            Guid fieldID = (Guid)grdFieldList.DataKeys[rowIndex].Value;
            int fieldIndex = FindFieldIndex(fieldID, _template.ComplexLists, Term.ID);

            Predicate<Term> pcomp = delegate(Term c) { return (c.ID == Term.ID); };
            Business.ComplexList complexList = null;
            switch (e.CommandName)
            {
                case Common.Names._GRID_COMMAND_EditRow:
                    ComplexListField f = ((ComplexList)_template.ComplexLists.Find(pcomp)).FindField((Guid)grdFieldList.DataKeys[rowIndex].Value);

                    //IsChanged related change
                    if (IsChanged)
                        Context.Items[Common.Names._CNTXT_IsChanged] = true;
                    Context.Items[Common.Names._CNTXT_Template] = _template;

                    BaseTermEditPage.TermEdit termEdit = new TermEdit();
                    termEdit.EditMode = EditMode.Edit;
                    termEdit.TermHandler = TermHandler.ComplexListField;
                    termEdit.TermIndex = TermIndex;
                    termEdit.ComplexListFieldIndex = fieldIndex;

                    Context.Items[Common.Names._CNTXT_TermEdit] = termEdit;

                    Server.Transfer(Helper.TemplateTermEditPage(f.FilterTerm.TermType));
                    break;

                case Common.Names._GRID_COMMAND_DeleteRow:
                    complexList = Term as Business.ComplexList;
                    complexList.Fields.RemoveAt(rowIndex);
                    complexList.DeleteItemValue(fieldID);
                    IsChanged = false;  //Do this to suppress the 'navigate from page' warning.
                    BindGrid(complexList.Fields);
                    IsChanged = true;
                    break;

                case Common.Names._GRID_COMMAND_SingleClick:
                case Common.Names._GRID_COMMAND_DoubleClick:
                    complexList = Term as Business.ComplexList;
                    ViewState[Common.Names._CNTXT_SelectedTermGroupIndex] = rowIndex;
                    BindGrid(complexList.Fields);
                    SetMoveUpDownButtonEvents(grdFieldList, btnTermGroupMoveUp, btnTermGroupMoveDown);
                    break;

                default:
                    break;
            }
        }

        private void BindGrid(List<Business.ComplexListField> fields)
        {
            grdFieldList.DataSource = fields;
            grdFieldList.DataBind();
        }

        public int FindFieldIndex(Guid fieldID, List<Term> complexList,Guid complexListId)
        {
            Predicate<Term> pcomp = delegate(Term c) {return (c.ID == complexListId);};
            Predicate<ComplexListField> pfield = delegate(ComplexListField t) { return (t.ID == fieldID); };
            return ((ComplexList)complexList.Find(pcomp)).Fields.FindIndex(pfield);
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

            if (selectedRow >= 0 && selectedRow < grd.Rows.Count - 1)
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

        #endregion
    }
}
