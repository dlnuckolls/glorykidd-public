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
    public partial class TemplateMain : BaseTemplatePage
    {
        internal override Control ResizablePanel()
        {
            return pnlForm;
        }

        internal override HtmlGenericControl HTMLBody()
        {
            return this.htmlBody;
        }

        protected override TemplateHeader HeaderControl()
        {
            return (TemplateHeader)header;
        }

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            if (!IsPostBack)
            {
                LoadFields();
                //ShowHideUserGeneratedDocumentsOption();
                UpdateRetroControls();
                if (_template.RetroModel != Retro.RetroModel.Off)
                    btnPromote.OnClientClick = "return confirmPromote(true)";
                else
                    btnPromote.OnClientClick = "return confirmPromote(false)";
            }

            chkAdvancedSecurity.Enabled = !chkAdvancedSecurity.Checked;

            rowDetailedDescription1.Visible = chkDetailedDescriptions.Checked;
            rowDetailedDescription2.Visible = chkDetailedDescriptions.Checked;
            rowDetailedDescription3.Visible = chkDetailedDescriptions.Checked;

        }


        private void UpdateRetroControls()
        {

            if (_itatSystem.AllowRetro != true)
            {
                pnlRetro.Visible = false;
                return;
            }


            //Load Retro Drop Down
            Helper.LoadRetroOptionsDropDown(ddlRetroOptions, false, _template.RetroModel);



            switch (_template.RetroModel)
            {
                case Retro.RetroModel.Off:
                    ddlRetroOptions.Visible = false;
                    chkRetro.Checked = false;
                    break;
                case Retro.RetroModel.OnWithoutEditLanguage:
                    chkRetro.Checked = true;
                    chkRetro.Enabled = false;
                    break;
                case Retro.RetroModel.OnWithEditLanguage:
                    chkRetro.Checked = true;
                    chkRetro.Enabled = false;
                    ddlRetroOptions.Enabled = false;
                    break;
            }




        }

        protected void OnHeaderEvent(object sender, HeaderEventArgs e)
        {
            switch (e.CommandName)
            {
                case Common.Names._HEADER_EVENT_Save:
                    SaveTemplateSummary();
                    break;

                case Common.Names._HEADER_EVENT_Reset:
                    GetTemplate(true);
                    LoadFields();
                    break;

                default:
                    base.HandleHeaderEvent(sender, e);
                    break;
            }
        }

        private void LoadFields()
        {
            txtName.Text = _template.Name;
            txtDescription.Text = _template.Description;
            Helper.LoadTemplateStatusDropDown(ddlStatus, false, _template.Status);
            if (!(_itatSystem.HasContent ?? false))
            {
                chkGenerateDocuments.Visible = false;
                chkGenerateDocuments.Checked = false;
            }
            else
                chkGenerateDocuments.Checked = _template.CanGenerateDocument;

            if (!(_itatSystem.SupportMultipleDocuments ?? false))
            {
                chkGenerateUserDocuments.Visible = false;
                chkGenerateUserDocuments.Checked = false;
            }
            else
                chkGenerateUserDocuments.Checked = _template.CanGenerateUserDocuments;

            if (_template.CanGenerateDocument)
            {
                rsPrintRolesSystemDocs.Visible = true;
                rsPrintRolesSystemDocs.Initialize(_template.DocumentPrinters);
            }
            else
                rsPrintRolesSystemDocs.Visible = false;

            if (_template.CanGenerateUserDocuments)
            {
                rsPrintRolesUserDocs.Visible = true;
                rsPrintRolesUserDocs.Initialize(_template.UserDocumentPrinters);
            }
            else
                rsPrintRolesUserDocs.Visible = false;

            if (_itatSystem.AllowAttachments ?? true)
            {
                chkAllowAttachments.Visible = true;
                chkAllowAttachments.Checked = _template.AllowAttachments;
            }
            else
            {
                chkAllowAttachments.Visible = false;
                chkAllowAttachments.Checked = false;
            }

            chkAdvancedSecurity.Checked = (_template.SecurityModel == SecurityModel.Advanced);
            chkDetailedDescriptions.Checked = _template.UseDetailedDescription;
            try
            {
                txtDetailedDescription1.Text = _template.FindDetailedDescription(DetailedDescription.DetailedDescriptionType.WhenToUse);
            }
            catch
            {
            }

            try
            {
                txtDetailedDescription2.Text = _template.FindDetailedDescription(DetailedDescription.DetailedDescriptionType.WhenNotToUse);
            }
            catch
            {
            }

            try
            {
                txtDetailedDescription3.Text = _template.FindDetailedDescription(DetailedDescription.DetailedDescriptionType.AdditionalInfo);
            }
            catch
            {
            }

            IsChanged = false;
        }


        private void SaveTemplateSummary()
        {
            Business.TemplateStatusType newStatus = (Business.TemplateStatusType)Enum.Parse(typeof(Business.TemplateStatusType), ddlStatus.SelectedValue);
            if (newStatus == TemplateStatusType.Active)
            {
                if (!Business.Template.FinalVersionExists(_template.ID))
                {
                    RegisterAlert("You must first promote the template before setting it to 'Active'.");
                    return;
                }
            }

            _template.Name = txtName.Text;
            _template.Description = txtDescription.Text;

            //System Document
            if (chkGenerateDocuments.Checked && _template.GetDefaultITATDocument() == null)
            {
                if (_template.Documents != null && _template.Documents.Count > 0)
                {
                    _template.Documents[0].DefaultDocument = true;
                    RegisterAlert("The first document was set as the default.  Please ensure that this is correct.");
                }
            }

            _template.CanGenerateDocument = chkGenerateDocuments.Checked;
            if (_template.CanGenerateDocument)
            {
                rsPrintRolesSystemDocs.Update();
                _template.DocumentPrinters = rsPrintRolesSystemDocs.SelectedRoles;
            }

            //User Documents
            _template.CanGenerateUserDocuments = chkGenerateUserDocuments.Checked;
            if (_template.CanGenerateUserDocuments)
            {
                rsPrintRolesUserDocs.Update();
                _template.UserDocumentPrinters = rsPrintRolesUserDocs.SelectedRoles;
            }


            _template.AllowAttachments = chkAllowAttachments.Checked;
            //TODO Note! If the security model is changed from Advanced to Basic, need to ensure that the Workflow States are populated correctly!
            _template.SecurityModel = (chkAdvancedSecurity.Checked ? SecurityModel.Advanced : SecurityModel.Basic);
            _template.Status = newStatus;

            _template.UseDetailedDescription = chkDetailedDescriptions.Checked;
            if (chkDetailedDescriptions.Checked)
            {
                Dictionary<Business.DetailedDescription.DetailedDescriptionType, String> detailedDescriptions = new Dictionary<DetailedDescription.DetailedDescriptionType, string>(3);
                detailedDescriptions.Add(DetailedDescription.DetailedDescriptionType.WhenToUse, txtDetailedDescription1.Text);
                detailedDescriptions.Add(DetailedDescription.DetailedDescriptionType.WhenNotToUse, txtDetailedDescription2.Text);
                detailedDescriptions.Add(DetailedDescription.DetailedDescriptionType.AdditionalInfo, txtDetailedDescription3.Text);
                _template.SetDetailedDescriptions(detailedDescriptions);
            }
            else
                _template.SetDetailedDescriptions(null);

            //Save retro option
            if (_itatSystem.AllowRetro && chkRetro.Checked)
                _template.RetroModel = (Retro.RetroModel)Enum.Parse(typeof(Retro.RetroModel), ddlRetroOptions.SelectedValue);

            _template.SaveLoaded(true, false, "Template Main");	//Note - Added to save DetailedDescriptions
            _template.UpdateTemplateSummary(_template.TemplateDef);
            IsChanged = false;
            RegisterAlert("Template Summary Info has been saved");
        }

        protected void btnPromote_Command(object sender, CommandEventArgs e)
        {
            //If this is the first time the template is promoted after selecting 'RetroModel <> Off', then set AuditType = RetroWithEditLanguage or RetroWithoutEditLanguage
            Retro.AuditType auditType = Retro.AuditType.Promoted;
            switch (_template.RetroModel)
            {
                case Retro.RetroModel.Off:
                    break;

                case Retro.RetroModel.OnWithEditLanguage:
                    if (!_template.GetRetroAuditTypes().Contains(Retro.AuditType.RetroWithEditLanguage))
                        auditType = Retro.AuditType.RetroWithEditLanguage;
                    break;

                case Retro.RetroModel.OnWithoutEditLanguage:
                    if (!_template.GetRetroAuditTypes().Contains(Retro.AuditType.RetroWithoutEditLanguage))
                        auditType = Retro.AuditType.RetroWithoutEditLanguage;
                    break;
            }
            if (Template.PromoteTemplate(_template.ID, _template.Status, auditType))
            {
                Data.Template.UpdateTemplateBaseStateRole(_template.ID, Utility.ListHelper.EliminateDuplicates(_template.Workflow.FindBaseState().EditorRoleNames));
                RegisterAlert(string.Format("The template has been promoted.  Any {0} created from this point forward using this template will use this version of the template.", _itatSystem.ManagedItemName));
            }
            else
            {
                RegisterAlert("No changes were detected, therefore no promotion has occurred.");
            }
        }

        protected void btnDemote_Command(object sender, CommandEventArgs e)
        {
            Template.DemoteTemplate(_template.ID, _template.Status);
            RegisterAlert("The template has been demoted.  All changes to the template since the last time it was promoted have been discarded.");
        }

        protected void chkGenerateDocumentsOnCheckedChanged(object sender, EventArgs e)
        {
            ShowHidePrintRoles(chkGenerateDocuments, rsPrintRolesSystemDocs, _template.DocumentPrinters);
            _template.CanGenerateDocument = chkGenerateDocuments.Checked;
            //if no system generated documents we don't want any 
            if (!chkGenerateDocuments.Checked)
                DisableDefaultDocument();


        }

        private void DisableDefaultDocument()
        {
            //disable default
            foreach (ITATDocument doc in _template.Documents)
                doc.DefaultDocument = false;
            //disable security
            _template.DocumentPrinters = new List<Role>();

        }


        protected void chkGenerateUserDocumentsOnCheckedChanged(object sender, EventArgs e)
        {
            ShowHidePrintRoles(chkGenerateUserDocuments, rsPrintRolesUserDocs, _template.UserDocumentPrinters);
            _template.CanGenerateUserDocuments = chkGenerateUserDocuments.Checked;
        }

        private void ShowHidePrintRoles(CheckBox cb, RoleSelector rs, List<Role> roles)
        {
            if (cb.Checked)
            {
                rs.Visible = true;
                rs.Initialize(roles);
            }
            else
            {
                rs.Visible = false;
            }
        }

        protected void chkRetro_CheckedChanged(object sender, EventArgs e)
        {
            ddlRetroOptions.Visible = chkRetro.Checked;

            if (chkRetro.Checked)
                RegisterAlert("Enabling retro will require you to set a retro message for every workflow. You will have to go to the Workflows page in order to perform that task.");
        }

    }
}
