using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Collections.Generic;

namespace Kindred.Knect.ITAT.Web
{
    public partial class RetroNotification : BaseTemplatePage
    {
        private const string _KH_VS_WORKFLOWID = "_kh_vs_WorkflowId";
        private const string _KH_VS_WORKFLOWEDITMODE = "_kh_vs_WorkflowEditMode";
        private Business.Workflow _workflow;
        private Guid _workflowId;
        private EditMode _workflowEditMode;



        protected override TemplateHeader HeaderControl()
        {
            return null;
        }

        internal override HtmlGenericControl HTMLBody()
        {
            return htmlBody;
        }

        internal override Control ResizablePanel()
        {
            return divEditor;
        }


        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            if (!IsPostBack)
            {
                GetContextData();
            }

            _workflow = _template.FindWorkflow(_workflowId);

            if (!IsPostBack)
            {
                LoadValues();                
            }

        }


        protected override void LoadViewState(object savedState)
        {
            base.LoadViewState(savedState);
            _workflowId = (Guid)ViewState[_KH_VS_WORKFLOWID];
            _workflowEditMode = (EditMode)ViewState[_KH_VS_WORKFLOWEDITMODE];
        }


        protected override object SaveViewState()
        {
            ViewState[_KH_VS_WORKFLOWID] = _workflowId;
            ViewState[_KH_VS_WORKFLOWEDITMODE] = _workflowEditMode;
            return base.SaveViewState();
        }



        private void GetContextData()
        {
            _workflowId = (Guid)Context.Items[Common.Names._CNTXT_WorkflowId];
            _workflowEditMode = (EditMode)Context.Items[Common.Names._CNTXT_WorkflowEditMode];
        }

        private void LoadValues()
        {
            Business.Message message = _workflow.RetroRevertMessage;
            txtSubject.Text = message.Subject;
            edt.Html = (string.IsNullOrEmpty(message.Text) ? Helper.DefaultEditorHtml(edt) : Business.Term.SubstituteTermIDs(_template, message.Text));
            Helper.LoadRoles(lstRecipients, _itatSystem, Business.RoleType.Distribution, message.Recipients);
            if (!(_itatSystem.HasOwningFacility ?? false) && _itatSystem.AllowNotificationFilterFacility)
            {
                List<Business.Term> facilityTerms = _template.BasicTerms.FindAll(t => t.TermType == Business.TermType.Facility || (t.TransformTermType.HasValue && t.TransformTermType.Value == Business.TermType.Facility));
                string facilityTermName = string.Empty;
                if (message.FilterFacilityTermID.HasValue)
                    facilityTermName = _template.FindTermName(message.FilterFacilityTermID.Value, null);
                Helper.LoadListControl(ddlFilterFacility, facilityTerms, "Name", "Name", facilityTermName, true, "(Select a Facility Term)", "");
                row_ddlFilterFacility.Visible = true;
            }
            else
            {
                ddlFilterFacility.SelectedIndex = -1;
                row_ddlFilterFacility.Visible = false;
            }
        }


        protected override void OnPreRender(EventArgs e)
        {
            if (edt.Visible)
            {
                Helper.RegisterParagraphWrapperScript(this);
                Telerik.WebControls.RadEditorUtils.ToolbarDropDown tdd = (Telerik.WebControls.RadEditorUtils.ToolbarDropDown)edt.Toolbars["DynamicToolbar"].Tools["Insert Term"];
                tdd.IsEnabled = false;
            }
            base.OnPreRender(e);
        }

        protected void btnOK_Command(object sender, CommandEventArgs e)
        {
            ReturnToCaller(true);
        }

        private void ReturnToCaller(bool updateValues)
        {
            if (updateValues)
            {
                 bool dataChanged;
                 SetValues(out dataChanged);
                 Context.Items[Common.Names._CNTXT_IsChanged] = dataChanged;

            }
            Context.Items[Common.Names._CNTXT_ScheduledEventUpdated] = true;
            Context.Items[Common.Names._CNTXT_Template] = _template;
            Context.Items[Common.Names._CNTXT_WorkflowId] = _workflowId;
            Context.Items[Common.Names._CNTXT_WorkflowEditMode] = _workflowEditMode;
            Server.Transfer("TemplateWorkflowMain.aspx");

        }

        protected void btnCancel_Command(object sender, CommandEventArgs e)
        {
            ReturnToCaller(false);
        }


        private void SetValues(out bool dataChanged)
        {
            dataChanged = false;

            Business.Message message = _workflow.RetroRevertMessage;
            if (message.Subject != txtSubject.Text)
            {
                dataChanged = true;
                message.Subject = txtSubject.Text;
            }

            if (message.Text != edt.Xhtml)
            {
                dataChanged = true;
                message.Text = Business.Term.SubstituteTermNames(_template, edt.Xhtml);
            }
            dataChanged = Helper.UpdateList(lstRecipients, message.Recipients) || dataChanged;

            if (ddlFilterFacility.SelectedIndex > 0)
            {
                string storedFilterFacility = string.Empty;
                if (message.FilterFacilityTermID.HasValue)
                {
                    storedFilterFacility = _template.FindTerm(message.FilterFacilityTermID.Value).Name;
                }
                if (storedFilterFacility != ddlFilterFacility.SelectedValue)
                {
                    dataChanged = true;
                    message.FilterFacilityTermID = _template.FindTerm(ddlFilterFacility.SelectedValue).ID;
                }
            }
            else
            {
                if (message.FilterFacilityTermID.HasValue)
                {
                    dataChanged = true;
                    message.FilterFacilityTermID = null;
                }
            }
        }
    }
}
