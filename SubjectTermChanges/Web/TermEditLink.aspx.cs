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
	public partial class TermEditLink : BaseTermEditPage
	{

		private const string _KH_CLOSEDIALOG = "_kh_closedialog";


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
			return editBody;
		}

		protected void btnOK_Click(object sender, EventArgs e)
		{
            if (!IsComplexListField)
                SetTermGroupInContext(ddlTermGroup.SelectedIndex);
            SetContextDataAndReturn(txtTermName.Text, true, chkHeaderTerm.Checked);
		}

		protected void btnCancel_Click(object sender, EventArgs e)
		{
			if (ShowTermGroups)
                SetTermGroupInContext((int)ViewState[Common.Names._CNTXT_SelectedTermGroupIndex]);
            SetContextDataAndReturn(txtTermName.Text, false, chkHeaderTerm.Checked);
		}



		protected override void InitializeForm()
		{
            header.PageTitle = PageTitle;
            txtTermName.Text = TermName;
            LoadLinkSources();
            LoadComplexList();
            if (ShowTermGroups)
			{
				trTermGroup.Visible = true;
				trHeaderTerm.Visible = true;
                Helper.LoadListControl(ddlTermGroup, _template.GetTermGroups(Business.TermGroup.TermGroupType.AdvancedBasicTerm), "Name", "ID", Term.TermGroupID.ToString(), true, "(Select a Tab)", Guid.Empty.ToString());
			}
			else
			{
				trTermGroup.Visible = false;
				trHeaderTerm.Visible = false;
			}
		}

		protected override void UpdateValues()
		{
            Business.LinkTerm linkTerm = Term as Business.LinkTerm;
			if (linkTerm == null)
                throw new Exception(string.Format("Unable to cast _term \"{0}\" to a Business.LinkTerm object.", TermName));

			TermName = txtTermName.Text;
            if (!IsComplexListField)
            {
                if (_template.SecurityModel == Business.SecurityModel.Advanced)
                    linkTerm.TermGroupID = new Guid(ddlTermGroup.SelectedValue);
                else
                    linkTerm.TermGroupID = _template.BasicSecurityTermGroupID;
            }
			linkTerm.IsHeader = chkHeaderTerm.Checked;
			linkTerm.Caption = txtCaption.Text;
			linkTerm.KeywordSearchable = chkboxKeywordSearchable.Checked;
            linkTerm.LinkSource = rblLinkSource.SelectedValue;
			//if (!string.IsNullOrEmpty(lstComplexList.SelectedValue))

            switch (rblLinkSource.SelectedValue)
            {
                case XMLNames._AV_ReferenceManagedItem:
                    linkTerm.IsManagedItemReference = true;
                    break;
                case XMLNames._AV_URL:
                    linkTerm.URL = txtURL.Text;
                    linkTerm.IsManagedItemReference = false;
                    break;
                case XMLNames._AV_ComplexList:
						 linkTerm.ComplexListID = _template.FindTerm(lstComplexList.SelectedValue).ID;
						 linkTerm.URL = null;
                    linkTerm.IsManagedItemReference = false;
                    break;
                default:
                    break;
            }

            
		}

		protected override List<string> ValidateForm()
		{
			List<string> rtn = new List<string>();
			if (string.IsNullOrEmpty(txtCaption.Text))
				rtn.Add("The Caption is Required.");

            switch (rblLinkSource.SelectedValue)
            {
                case XMLNames._AV_URL:
                    if (string.IsNullOrEmpty(txtURL.Text))
                        rtn.Add("URL Address is Required.");
                     break;
                case XMLNames._AV_ComplexList:
                    if (lstComplexList.SelectedIndex <= 0)
                        rtn.Add("A Complex List must be selected.");
                    break;
            }

            //Make sure a Term Group is selected.

            if (trTermGroup.Visible == true && ddlTermGroup.SelectedValue == Guid.Empty.ToString())
                rtn.Add("A Tab must be selected.");

			return rtn;
		}

		protected override void LoadValues()
		{
            Business.LinkTerm linkTerm = Term as Business.LinkTerm;
			if (linkTerm == null)
                throw new Exception(string.Format("Unable to cast _term \"{0}\" to a Business.LinkTerm object.", TermName));


			txtCaption.Text = linkTerm.Caption;
			chkHeaderTerm.Checked = linkTerm.IsHeader;
			chkboxKeywordSearchable.Checked = (linkTerm.KeywordSearchable ?? false);
			//chkboxReferenceManagedItem.Checked = linkTerm.IsManagedItemReference ?? false;
            if (!string.IsNullOrEmpty(linkTerm.LinkSource))
                rblLinkSource.SelectedValue = linkTerm.LinkSource;
            else
            {
                rblLinkSource.SelectedValue = XMLNames._AV_URL;
                txtURL.Visible = true;
            }

            switch(linkTerm.LinkSource)
            {
                case XMLNames._AV_ReferenceManagedItem:
                    txtURL.Visible = false;
                    lstComplexList.Visible = false;
                    break;
                case XMLNames._AV_URL:
                    txtURL.Visible = true;
                    txtURL.Text = linkTerm.URL;
                    lstComplexList.Visible = false;
                    break;
                case XMLNames._AV_ComplexList:
                    txtURL.Visible = false;
                    lstComplexList.Visible = true;
                    lstComplexList.SelectedValue = _template.FindTerm(linkTerm.ComplexListID).Name;
                    break;
                default:
                    break;
            }
        }


		protected override void ShowHideFields()
		{
		}

        private void LoadLinkSources()
        {
            rblLinkSource.Items.Add(new ListItem("URL", XMLNames._AV_URL));
            rblLinkSource.Items.Add(new ListItem("Complex List", XMLNames._AV_ComplexList));
            rblLinkSource.Items.Add(new ListItem("Reference Managed Item", XMLNames._AV_ReferenceManagedItem));

        }
        private void LoadComplexList()
        {

            Business.Template template = _template as Business.Template;
            lstComplexList.Items.Add(new ListItem("Select a Complex List ", "Default"));
            foreach (ComplexList complexList in template.ComplexLists)
            {
                lstComplexList.Items.Add(new ListItem(complexList.Name,complexList.Name));
            }

        }

        protected void rblLinkSource_SelectedIndexChanged(object sender, EventArgs e)
        {
            Business.LinkTerm linkTerm = Term as Business.LinkTerm;

            switch (rblLinkSource.SelectedValue)
            {
                case XMLNames._AV_ReferenceManagedItem:

                    txtURL.Visible = false;
                    lstComplexList.Visible = false;   
                    break;
                case XMLNames._AV_URL:
                    txtURL.Visible = true;
                    lstComplexList.Visible = false;   
                        break;
                case XMLNames._AV_ComplexList:
                    txtURL.Visible = false;
                    lstComplexList.Visible = true;   
                    break;
                default:
                    break;
            }
        }
	}
}
