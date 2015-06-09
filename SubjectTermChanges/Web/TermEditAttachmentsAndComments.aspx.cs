using System;
using System.Collections.Generic;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

namespace Kindred.Knect.ITAT.Web
{
    public partial class TermEditAttachmentsAndComments : BaseTermEditPage
    {

        #region base class overrides

        internal override HtmlGenericControl HTMLBody()
        {
            return this.body;
        }

        internal override Control ResizablePanel()
        {
            return editBody;
        }

        protected override TextBox TermNameControl()
        {
            return txtTermName;
        }

        protected override void InitializeForm()
        {
            header.PageTitle = PageTitle;
            txtTermName.Text = TermName;
            if (ShowTermGroups)
            {
                trTermGroup.Visible = true;
                Helper.LoadListControl(ddlTermGroup, _template.GetTermGroups(Business.TermGroup.TermGroupType.AdvancedBasicTerm), "Name", "ID", Term.TermGroupID.ToString(), true, "(Select a Tab)", Guid.Empty.ToString());
            }
            else
            {
                trTermGroup.Visible = false;
            }

        }

        protected override void UpdateValues()
        {
            switch (Term.TermType)
            {
                case Kindred.Knect.ITAT.Business.TermType.PlaceHolderAttachments:
                    Business.PlaceHolderAttachments placeHolderAttachments = Term as Business.PlaceHolderAttachments;
                    if (!IsComplexListField)
                    {
                        if (_template.SecurityModel == Business.SecurityModel.Advanced)
                        {
                            placeHolderAttachments.TermGroupID = new Guid(ddlTermGroup.SelectedValue);
                        }
                        else
                        {
                            placeHolderAttachments.TermGroupID = _template.BasicSecurityTermGroupID;
                        }
                    }
                    break;
                case Kindred.Knect.ITAT.Business.TermType.PlaceHolderComments:
                    Business.PlaceHolderComments placeHolderComments = Term as Business.PlaceHolderComments;
                    if (!IsComplexListField)
                    {
                        if (_template.SecurityModel == Business.SecurityModel.Advanced)
                        {
                            placeHolderComments.TermGroupID = new Guid(ddlTermGroup.SelectedValue);
                        }
                        else
                        {
                            placeHolderComments.TermGroupID = _template.BasicSecurityTermGroupID;
                        }
                    }
                    break;
            }

        }


        protected override void ShowHideFields()
        {
        }


        protected override System.Collections.Generic.List<string> ValidateForm()
        {
            List<string> rtn = new List<string>();

            //Make sure a Term Group is selected.

            if (trTermGroup.Visible == true && ddlTermGroup.SelectedValue == Guid.Empty.ToString())
                rtn.Add("A Tab must be selected.");


            return rtn;
        }

        protected override void LoadValues()
        {

            lblTermType.Text = Term.TermType.ToString();
            //switch (_term.TermType)
            //{
            //    case Kindred.Knect.ITAT.Business.TermType.PlaceHolderAttachments:
            //        Business.PlaceHolderAttachments placeHolderAttachments = _term as Business.PlaceHolderAttachments;
                    

            //        break;
            //    case Kindred.Knect.ITAT.Business.TermType.PlaceHolderComments:
            //        Business.PlaceHolderComments placeHolderComments = _term as Business.PlaceHolderComments;

            //        break;
            //}
        }

        #endregion

        #region event handlers

        protected void btnOK_Click(object sender, EventArgs e)
        {
            //Pass the selected TermGroup Back so that that termgroup selection can be persisted.
            if (!IsComplexListField)
                SetTermGroupInContext(ddlTermGroup.SelectedIndex);
            SetContextDataAndReturn(txtTermName.Text, true, false);
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            if (ShowTermGroups)
                SetTermGroupInContext((int)ViewState[Common.Names._CNTXT_SelectedTermGroupIndex]);
            SetContextDataAndReturn(txtTermName.Text, false, false);
        }

        #endregion

    }
}
