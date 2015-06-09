using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Xml.XPath;
using Kindred.Knect.ITAT.Business;
using System.Web.UI.HtmlControls;

namespace Kindred.Knect.ITAT.Web
{
    public partial class TermEditComplexListItem : BaseTermEditPage
    {
        #region private members
        Dictionary<string, WebControl> _fieldControls = new Dictionary<string, WebControl>();
        private const int BIG_TEXT_ROW_SIZE = 4;
        #endregion

        #region Properties
        #endregion

        #region base override
        internal override System.Web.UI.HtmlControls.HtmlGenericControl HTMLBody()
        {
            return this.body;
        }

        internal override Control ResizablePanel()
        {
            return null;
        }

        protected override TextBox TermNameControl()
        {
            return null;
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


        #endregion

        #region events

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            Business.ComplexList complexList = (Business.ComplexList)_template.ComplexLists[TermIndex];
            header.PageTitle = string.Format("Template - {0} - Edit", complexList.Name);
            ComplexListName.InnerText = "";

            RenderFieldTermControls();
            if (EditMode == EditMode.Edit)
            {
                SetComplexListItemValues(_fieldControls, termEdit.ComplexListItemIndex);
            }
            else
            {
                SetComplexListItemValues(_fieldControls, null);
            }

            if (!IsPostBack)
            {
                if (termEdit.ComplexListItemIndex >= 0 && complexList.Items != null && complexList.Items.Count >= termEdit.ComplexListItemIndex + 1)
                {
                    chkbxDeletable.Checked = complexList.Items[termEdit.ComplexListItemIndex].Deletable ?? false; ;
                    chkbxSelected.Checked = complexList.Items[termEdit.ComplexListItemIndex].Selected ?? false;
                    chkbxSelectable.Checked = complexList.Items[termEdit.ComplexListItemIndex].Selectable ?? false;
                    chkbxEditable.Checked = complexList.Items[termEdit.ComplexListItemIndex].Editable ?? false;
                }
                else
                {
                    chkbxDeletable.Checked = false;
                    chkbxSelected.Checked = false;
                    chkbxSelectable.Checked = false;
                    chkbxEditable.Checked = false;
                }
            }
        }

        private void SetComplexListItemValues(Dictionary<string, WebControl> _fieldControls, int? _complexListItemIndex)
        {
            Business.ComplexList complexList = (Business.ComplexList)_template.ComplexLists[TermIndex];
            foreach (ComplexListField field in complexList.Fields)
            {
                if (_complexListItemIndex.HasValue)
                {
                    switch (field.FilterTerm.TermType)
                    {
                        case TermType.Text:
                            SetTextBoxText(_fieldControls[field.Name], complexList.Items[_complexListItemIndex.Value].FindItemValue(field.ID).FieldValue, complexList.Items[_complexListItemIndex.Value].FindItemValue(field.ID).BigText);
                            break;
                        case TermType.PickList:
                            SetDropDownList(_fieldControls[field.Name], complexList.Items[_complexListItemIndex.Value].FindItemValue(field.ID) != null ? complexList.Items[_complexListItemIndex.Value].FindItemValue(field.ID).FieldValue : null);
                            break;
                        case TermType.Date:
                            SetDateValue(_fieldControls[field.Name], complexList.Items[_complexListItemIndex.Value].FindItemValue(field.ID) != null ? complexList.Items[_complexListItemIndex.Value].FindItemValue(field.ID).FieldValue : null);
                            break;

                        default:
                            break;
                    }
                }
                else
                {
                    string displayValue = null;
                    if (field.DefaultValueDefined)
                        displayValue = field.DefaultValue;
                    switch (field.FilterTerm.TermType)
                    {
                        case TermType.Text:
                            SetTextBoxText(_fieldControls[field.Name], displayValue, field.BigText ?? false);
                            break;
                        case TermType.PickList:
                            SetDropDownList(_fieldControls[field.Name], displayValue);
                            break;
                        case TermType.Date:
                            SetDateValue(_fieldControls[field.Name], displayValue);
                            break;

                        default:
                            break;
                    }
                }
            }
        }

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);
        }

        #endregion

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

        private void SetTextBoxText(WebControl webControl, string text,bool bigtext)
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

        private bool GetXmlAttributeBool(RepeaterItemEventArgs e, string attributeName)
        {
            try { return bool.Parse(((IXPathNavigable)e.Item.DataItem).CreateNavigator().GetAttribute(attributeName, "")); }
            catch { return false; }
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            BaseTransfer(null, TermHandler.ComplexListItem, "TermEditComplexListItems.aspx", false);
        }

        protected void btnOK_Click(object sender, EventArgs e)
        {
            Business.ComplexList complexList = (Business.ComplexList)_template.ComplexLists[TermIndex];
            List<ComplexListItemValue> complexListItemValues = new List<ComplexListItemValue>();

            foreach (Business.ComplexListField field in complexList.Fields)
            {
                ComplexListItemValue cliv = new ComplexListItemValue(complexList, field.FilterTerm);
                cliv.FieldID = field.ID;
                cliv.FieldValue = GetFieldValue(field);
                cliv.BigText = cliv.FieldValue.Length > 150;
                complexListItemValues.Add(cliv);
            }

            ComplexListItem complexListItem = new ComplexListItem();
            complexListItem.Default = true;
            complexListItem.ItemValues = complexListItemValues;

            complexListItem.Selectable = chkbxSelectable.Checked;
            complexListItem.Selected = chkbxSelected.Checked;
            complexListItem.Editable = chkbxEditable.Checked;
            complexListItem.Deletable = chkbxDeletable.Checked;

            if (EditMode == EditMode.Add)
                complexList.Items.Add(complexListItem);
            else
                complexList.Items[termEdit.ComplexListItemIndex] = complexListItem;

            BaseTransfer(null, TermHandler.ComplexListItem, "TermEditComplexListItems.aspx", true);
        }

        protected string GetFieldValue(ComplexListField field)
        {
            string result = "";
            switch (field.FilterTerm.TermType)
            { 
                case TermType.Text:
                    result = Request.Form[Helper.ControlID(field.Name, field.FilterTerm.TermType)];
                    break;
                case TermType.PickList:
                    result = Request.Form[Helper.ControlID(field.Name, field.FilterTerm.TermType)];
                    break;
                case TermType.Date:
                    result = Request.Form[Helper.ControlID(field.Name, field.FilterTerm.TermType)];
                    break;
            }
            return result;
        }
        
        private void RenderFieldTermControls()
        {
            Business.ComplexList complexList = (Business.ComplexList)_template.ComplexLists[TermIndex];
            WebControl ctl;
            foreach (Business.ComplexListField field in complexList.Fields)
            {
                switch (field.FilterTerm.TermType)
                {
                    case Business.TermType.Text:
                        ctl = Helper.CreateTextComplexListFieldControl(field);
                        break;
                    case Business.TermType.PickList:
                        ctl = Helper.CreatePickListComplexListFieldControl(field);
                        break;
                    case Business.TermType.Date:
                        ctl = Helper.CreateDateComplexListFieldControl(field);
                        break;
                    default:
                        ctl = new Label();
                        ((Label)ctl).Text = "Dummy Code";
                        break;
                }
                ctl.EnableViewState = true;
                _fieldControls.Add(field.Name, ctl);
            }

            HtmlTable dtblCriteria1 = new HtmlTable();
            dtblCriteria1.ID = Guid.NewGuid().ToString();
            dtblCriteria1.EnableViewState = false;
            dtblCriteria1.CellPadding = 1;
            dtblCriteria1.CellSpacing = 0;
            dtblCriteria1.Border = 0;
            apnlCriteria.Controls.Add(dtblCriteria1);

            foreach (Business.ComplexListField field in complexList.Fields)
            {
                System.Web.UI.HtmlControls.HtmlTableRow row = new System.Web.UI.HtmlControls.HtmlTableRow();

                System.Web.UI.HtmlControls.HtmlTableCell cellLabel = new System.Web.UI.HtmlControls.HtmlTableCell();
                cellLabel.ID = "tblField_cell_lable_" + field.Name;
                cellLabel.InnerText = field.Name;
                Helper.FormatCaptionCell(cellLabel);
                row.Cells.Add(cellLabel);

                System.Web.UI.HtmlControls.HtmlTableCell cellValue = new System.Web.UI.HtmlControls.HtmlTableCell();
                cellValue.ID = "tblField_cell_value_" + field.Name;
                cellValue.Controls.Add(_fieldControls[field.Name]);
                cellValue.Width = "90%";
                cellValue.Attributes.Add("class", "ProfileEdit");
                row.Cells.Add(cellValue);

                if (dtblCriteria1.Rows.Count == 0)
                    dtblCriteria1.Rows.Add(row);
                else
                    dtblCriteria1.Rows.Insert(dtblCriteria1.Rows.Count, row);
            }
        }
    }
}