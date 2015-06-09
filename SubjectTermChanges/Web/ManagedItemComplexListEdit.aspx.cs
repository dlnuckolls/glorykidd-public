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
using Kindred.Knect.ITAT.Business;
using System.Collections.Generic;

namespace Kindred.Knect.ITAT.Web
{

	public partial class ManagedItemComplexListEdit : BaseManagedItemComplexListPage
	{
		#region Private Members and Constants

        private const string CSSCLASS_EDIT = "ProfileEdit";
        private const string CSSCLASS_EDITREADONLY = "ProfileEditReadOnly";
        private const string CSSCLASS_CAPTION = "ProfileCaption";
        private const string CSSCLASS_CAPTION_ERROR = "ComplexListValidationError";
        private const string COMMAND_OK = "OK";
        private const string COMMAND_CANCEL = "CANCEL";

        private const string _KH_VS_EDITMODE = "_kh_vs_EditMode";
        private const string _KH_VS_ITEMS = "_kh_vs_Items";
        private const int BIG_TEXT_ROW_SIZE = 4;
        
        private EditMode _editMode;
        private List<string> _validationErrors;
        private List<ComplexListItemValue> _itemValues;

        Dictionary<string, WebControl> _fieldControls = new Dictionary<string, WebControl>();

		#endregion

        #region Base Class Overrides
        internal override Control ResizablePanel()
        {
            return pnlTerms;
        }

        internal override HtmlGenericControl HTMLBody()
        {
            return this.htmlBody;
        }

        protected override ManagedItemHeader HeaderControl()
        {
            return (ManagedItemHeader)header;
        }

        protected override string GetApplicationFunction()
        {
            return null;   // Anyone (who can access the application) can access this page
        }

        #endregion

        #region Base Page Overrides

        protected override void OnPreRender(EventArgs e)
        {
            if (IsPostBack)
            {
                if (_validationErrors.Count > 0)
                {
                    RenderValidationErrors(_validationErrors);
                    this.IsChanged = true;
                }
            }
            base.OnPreRender(e);
        }

        protected override void OnInit(EventArgs e)
        {
            _validationErrors = new List<string>();
            base.OnInit(e);
        }

        protected override void OnLoad(EventArgs e)
        {
            _banner = string.Concat(_itatSystem.ManagedItemName, " - ", Request.QueryString[Common.Names._QS_COMPLEXLIST_NAME], " - Edit");
            base.OnLoad(e);
            int nIndex = -1;
            Business.ComplexList complexList = GetComplexList(ref nIndex, true);
            if (IsPostBack)
            {
                if (complexList != null)
                {
                    UpdateValues(complexList, nIndex);
                    Dictionary<string /* Term Name */, List<string> /* Error Messages */ > termTypeErrors = Helper.GetComplexListTermTypeErrors(null, complexList, nIndex);
                    Helper.GetComplexListItemValidationErrors(ManagedItemValidationType.ValidateOnSave, termTypeErrors, complexList.Items[nIndex], _validationErrors, true, nIndex, true, _managedItem.State.Status, complexList.Name);
                }
            }
            else
            {
                if (complexList != null)
                {
                    _itemValues = complexList.Items[nIndex].ItemValues;
                    complexList.Items[nIndex].ClearRunTime();
                }
                GetContextData();
            }
            RenderTerms(complexList, nIndex);
            RenderButtons();
            HeaderControl().Roles = _securityHelper.UserRoles.ToArray();
        }

        protected override void LoadViewState(object savedState)
        {
            base.LoadViewState(savedState);
            _editMode = (EditMode)ViewState[_KH_VS_EDITMODE];
            _itemValues = (List<ComplexListItemValue>)ViewState[_KH_VS_ITEMS];
        }

        protected override object SaveViewState()
        {
            ViewState[_KH_VS_EDITMODE] = _editMode;
            ViewState[_KH_VS_ITEMS] = _itemValues;
            return base.SaveViewState();
        }

        #endregion

        #region Event Handlers
        void ActionButtonCommand(object sender, CommandEventArgs e)
        {
            int nIndex = -1;
            Business.ComplexList complexList = GetComplexList(ref nIndex, true);
            if (complexList != null)
            {
                switch (e.CommandName)
                {
                    case COMMAND_OK:
                        if (_validationErrors.Count == 0)
                            SetContextDataAndReturn(true, complexList, nIndex);
                        break;

                    case COMMAND_CANCEL:
                        if (_editMode == EditMode.Add)
                            complexList.Items.RemoveAt(complexList.Items.Count - 1);
                        SetContextDataAndReturn(false, complexList, nIndex);
                        break;
                }
            }
        }

        protected void OnHeaderEvent(object sender, HeaderEventArgs e)
        {
        }

        #endregion

        #region Private Methods
        private void GetContextData()
        {
            if (Context.Items[Common.Names._CNTXT_EditMode] != null)
                _editMode = (EditMode)Context.Items[Common.Names._CNTXT_EditMode];
        }

        private void FormatCaptionCell(HtmlTableCell cell, string termName, bool isRequired, string errorMessage)
        {
            cell.InnerHtml = isRequired ? string.Concat("* ", termName) : termName;
            cell.Attributes["width"] = "150px";
            cell.Attributes["class"] = string.IsNullOrEmpty(errorMessage) ? CSSCLASS_CAPTION : CSSCLASS_CAPTION_ERROR;
            if (!string.IsNullOrEmpty(errorMessage))
            {
                errorMessage = errorMessage.Replace("'", "\"");
                cell.Attributes.Add("onmouseover", "ShowItatToolTip(event, 'itatToolTip', '" + errorMessage + "')");
                cell.Attributes.Add("onmouseout", "HideItatToolTip('itatToolTip');");
                cell.Attributes.Add("onmousemove", "MoveItatToolTip(event, 'itatToolTip');");
            }
        }

        private void FormatDataCell(HtmlTableCell cell, bool canEdit)
        {
            cell.Attributes["width"] = "100%";
            if (canEdit)
                cell.Attributes["class"] = CSSCLASS_EDIT;
            else
                cell.Attributes["class"] = CSSCLASS_EDITREADONLY;
        }

        private void RenderTerms(Business.ComplexList complexList, int nIndex)
        {
            if (complexList == null)
                return;

            WebControl ctl;

            foreach (Business.ComplexListItemValue itemValue in complexList.Items[nIndex].ItemValues)
            {
                HtmlTableRow row = new HtmlTableRow();
                HtmlTableCell cellLabel = new HtmlTableCell();
                FormatCaptionCell(cellLabel, itemValue.FieldName, itemValue.FieldFilterTerm.Required ?? false, itemValue.FieldFilterTerm.Runtime.ErrorMessage);

                row.Cells.Add(cellLabel);
                HtmlTableCell cellValue = new HtmlTableCell();
                FormatDataCell(cellValue, true);

                switch (itemValue.TermType)
                {
                    case Business.TermType.Text:
                        ctl = Helper.CreateTextComplexListFieldControl(itemValue);
                        SetTextBoxText(ctl, itemValue.FieldValue, itemValue.BigText);
                        break;
                    case Business.TermType.PickList:
                        ctl = Helper.CreatePickListComplexListFieldControl(itemValue);
                        SetDropDownList(ctl, itemValue.FieldValue);
                        break;
                    case Business.TermType.Date:
                        ctl = Helper.CreateDateComplexListFieldControl(itemValue);
                        SetDateValue(ctl, itemValue.FieldValue);
                        break;
                    default:
                        ctl = new Label();
                        ((Label)ctl).Text = "Dummy Code";
                        break;
                }
                ctl.EnableViewState = true;
                cellValue.Controls.Add(ctl);
                row.Cells.Add(cellValue);
                tblTerms.Rows.Add(row);
            }
        }

        private void RenderButtons()
        {
            Button btn1 = new Button();
            btn1.Text = "OK";
            btn1.Command += new CommandEventHandler(ActionButtonCommand);
            btn1.CommandName = COMMAND_OK;
            btn1.CssClass = "KnectButton ActionButton";
            pnlButtons.Controls.Add(btn1);

            Button btn2 = new Button();
            btn2.Text = "Cancel";
            btn2.Command += new CommandEventHandler(ActionButtonCommand);
            btn2.CommandName = COMMAND_CANCEL;
            btn2.CssClass = "KnectButton ActionButton";
            pnlButtons.Controls.Add(btn2);
        }

        private string BuildControlName(string controlName)
        {
            return string.Format("txtText_{0}", controlName);
        }

        private string GetControlValue(string controlName)
        {
            return Request.Form[BuildControlName(controlName)];
        }

        private void UpdateValues(Business.ComplexList complexList, int nIndex)
        {
            if (complexList == null)
                return;

            foreach (Business.ComplexListItemValue itemValue in complexList.Items[nIndex].ItemValues)
            {
                itemValue.FieldFilterTerm.Runtime.Reset(true, itemValue.FieldFilterTerm.Required ?? false);
                itemValue.FieldValue = GetFieldValue(itemValue);
            }
        }

        private void SetContextDataAndReturn(bool updateValues, Business.ComplexList complexList, int nIndex)
        {
            if (!updateValues)
            {
                if (nIndex >= 0 && nIndex <= complexList.Items.Count - 1)
                    complexList.Items[nIndex].ItemValues = _itemValues;
            }
            Context.Items[Common.Names._CNTXT_ManagedItem] = _managedItem;
            if (IsChanged)
                Context.Items[Common.Names._CNTXT_IsChanged] = updateValues;
            Server.Transfer("ManagedItemComplexList.aspx");
        }

        private void SetDateValue(WebControl webControl, string text)
        {
            Panel panel = webControl as Panel;
            if (panel != null)
            {
                foreach (Control control in panel.Controls)
                {
                    TextBox textBox2 = control as TextBox;
                    if (textBox2 != null)
                    {
                        textBox2.Text = text;
                        return;
                    }
                }
            }
        }

        private void SetTextBoxText(WebControl webControl, string text, bool bigtext)
        {
            TextBox textBox = webControl as TextBox;
            if (textBox != null)
            {
                textBox.Text = text;

                if (bigtext)
                {
                    textBox.TextMode = TextBoxMode.MultiLine;
                    textBox.Rows = BIG_TEXT_ROW_SIZE;
                }
                return;
            }

            Panel panel = webControl as Panel;
            if (panel != null)
            {
                foreach (Control control in panel.Controls)
                {
                    TextBox textBox2 = control as TextBox;
                    if (textBox2 != null)
                    {
                        textBox2.Text = text;
                        if (bigtext)
                        {
                            textBox.TextMode = TextBoxMode.MultiLine;
                            textBox.Rows = BIG_TEXT_ROW_SIZE;
                        }
                        return;
                    }
                }
            }
        }

        private void SetDateRangeText(WebControl webControl, string sDateStart, string sDateEnd)
        {
            Panel panel = webControl as Panel;
            if (panel != null)
            {
                foreach (Control control in panel.Controls)
                {
                    HtmlContainerControl htmlContainerControl = control as HtmlContainerControl;
                    if (htmlContainerControl != null)
                    {
                        foreach (Control innerControl in htmlContainerControl.Controls)
                        {
                            TextBox textBox = innerControl as TextBox;
                            if (textBox != null)
                            {
                                string sID = textBox.ID;
                                DateTime dt;
                                if (sID.IndexOf(Common.Names._IDENTIFIER_StartDate) >= 0)
                                    if (DateTime.TryParse(sDateStart, out dt))
                                        textBox.Text = sDateStart;
                                    else
                                        textBox.Text = string.Empty;
                                else
                                    if (sID.IndexOf(Common.Names._IDENTIFIER_EndDate) >= 0)
                                        if (DateTime.TryParse(sDateEnd, out dt))
                                            textBox.Text = sDateEnd;
                                        else
                                            textBox.Text = string.Empty;
                                    else
                                        textBox.Text = string.Empty;
                            }
                        }
                    }
                }
            }
        }

        private void SetDropDownList(WebControl webControl, string text)
        {
            Panel panel = webControl as Panel;
            if (panel != null)
            {
                foreach (Control control in panel.Controls)
                {
                    DropDownList dropDownList = control as DropDownList;
                    if (dropDownList != null)
                    {
                        dropDownList.SelectedValue = text;
                        return;
                    }

                    RadioButtonList radioButtonList = control as RadioButtonList;
                    if (radioButtonList != null)
                    {
                        radioButtonList.SelectedValue = text;
                        return;
                    }
                }
            }
        }
        protected string GetFieldValue(ComplexListItemValue itemValue)
        {
            string result = "";
            switch (itemValue.TermType)
            {
                case TermType.Text:
                    result = Request.Form[Helper.ControlID(itemValue.FieldName, itemValue.TermType)];
                    break;
                case TermType.PickList:
                    result = Request.Form[Helper.ControlID(itemValue.FieldName, itemValue.TermType)];
                    break;
                case TermType.Date:
                    result = Request.Form[Helper.ControlID(itemValue.FieldName, itemValue.TermType)];
                    break;
            }
            return result;
        }
        #endregion

        
       
	}
}
