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

namespace Kindred.Knect.ITAT.Web
{
    public partial class WorkFlowAdd : BaseTemplatePage
    {
        private Guid _workflowId;
        private Business.Workflow _workflow; 

        #region private members
        private EditMode _editMode = EditMode.None;
        private const string _KH_CLOSEDIALOG = "_kh_closedialog";
        private const string _KH_VS_EDITMODE = "_kh_vs_EditMode";
        private const string _KH_VS_WORKFLOWID = "_kh_vs_WorkflowId";
        #endregion

        #region eventhandlers

		 protected override void OnLoad(EventArgs e)
		 {
			 base.OnLoad(e);
			 if (!IsPostBack)
			 {
				 if (Context.Items[Common.Names._CNTXT_WorkflowEditMode] != null)
					 _editMode = (EditMode)Context.Items[Common.Names._CNTXT_WorkflowEditMode];
				 else
					 _editMode = EditMode.None;
				 txtWorkflowName.Text = Helper.GetDefaultWorkflowName(_template);
			 }
		 }

        protected void btnContinue_Click(object sender, EventArgs e)
        {
            txtWorkflowName.Text = txtWorkflowName.Text;
            if (_template.WorkflowExists(txtWorkflowName.Text))
                RegisterAlert(string.Format("The workflow name '{0}' is already being used.   You must select a different workflow name.", txtWorkflowName.Text));
            if (this.AlertCount > 0)
                return;

            if (_editMode == EditMode.Add)
            {
                //Note - similar code checks are made at the 'BaseTermEditPage' SetContextDataAndReturn call

                _workflow = new Business.Workflow(txtWorkflowName.Text, _template.IsManagedItem, _template);
                _template.Workflows.Add(_workflow);
            }
            
            Context.Items[Common.Names._CNTXT_Template] = _template;
            Context.Items[Common.Names._CNTXT_WorkflowEditMode] =  _editMode;
            Context.Items[Common.Names._CNTXT_WorkflowId] = _workflow.ID;  
            Server.Transfer("TemplateWorkflowMain.aspx");
        }


        protected void btnCancel_Click(object sender, EventArgs e)
        {
            Context.Items[Common.Names._CNTXT_Template] = _template;
            Server.Transfer("TemplateWorkflows.aspx");
        }

        #endregion



        #region base class overrides

        internal override HtmlGenericControl HTMLBody()
        {
            return this.body;
        }

        internal override Control ResizablePanel()
        {
            return editBody;
        }

        protected override TemplateHeader HeaderControl()
        {
            return null;
        }

        protected override void LoadViewState(object savedState)
        {
            base.LoadViewState(savedState);
            _editMode = (EditMode)ViewState[_KH_VS_EDITMODE];
            _workflowId = (Guid)ViewState[_KH_VS_WORKFLOWID];
            _workflow = _template.FindWorkflow(_workflowId);
        }

        protected override object SaveViewState()
        {
            ViewState[_KH_VS_EDITMODE] = _editMode;
            ViewState[_KH_VS_WORKFLOWID] = _workflowId;
            return base.SaveViewState();
        }
        
        #endregion
    }
}
