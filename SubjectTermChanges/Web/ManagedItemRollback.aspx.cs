using System;
using System.Data;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

namespace Kindred.Knect.ITAT.Web
{
    public partial class ManagedItemRollback : BaseManagedItemPage
    {
        private const string _VS_MANGEDITEM_AUDIT_ID = "_VS_MANGEDITEM_AUDIT_ID";
        private Guid? _managedItemAuditID;

        protected override void OnPreRenderComplete(EventArgs e)
        {
            base.OnPreRenderComplete(e);
        }

		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);
            if (!IsPostBack)
            {
                DataSet ds = Data.ManagedItem.GetManagedItemDetailedHistory(_managedItem.ManagedItemID, false);
                grdResults.DataSource = ds.Tables[0];
                grdResults.DataBind();
            }
            else
            {
                string rollbackOptionValue = hfRollbackOption.Value;
                if (!string.IsNullOrEmpty(rollbackOptionValue))
                {
                    bool? orphan = null;
                    ManagedItemRollbackConfirmation.RollbackOption rollbackOption = (ManagedItemRollbackConfirmation.RollbackOption)(Enum.Parse(typeof(ManagedItemRollbackConfirmation.RollbackOption), rollbackOptionValue));
                    switch (rollbackOption)
                    {
                        case ManagedItemRollbackConfirmation.RollbackOption.CreateOrphan:
                            orphan = true;
                            break;

                        case ManagedItemRollbackConfirmation.RollbackOption.DoNotCreateOrphan:
                            orphan = false;
                            break;

                        default:
                            throw new Exception(string.Format("Rollback option '{0}' not handled", rollbackOption.ToString()));
                    }
                    string sEnvironment = System.Configuration.ConfigurationManager.AppSettings[Utility.EnvironmentHelper.GetEnvironment(Kindred.Common.WebServices.EnvironmentDetectionMode.Machine) + Data.DataNames._AC_ApplicationWebServer];
                    _managedItem.Rollback(_managedItemAuditID.Value, sEnvironment, orphan);
                    RegisterAlert(string.Format("{0} Rollback Successful.", _itatSystem.ManagedItemName));
                    RegisterRedirect(string.Format("ManagedItemProfile.aspx{0}", Utility.TextHelper.QueryString(true, Common.Names._QS_ITAT_SYSTEM_ID, _itatSystem.ID.ToString(), Common.Names._QS_MANAGED_ITEM_ID, _managedItem.ManagedItemID.ToString())));
                }
                hfRollbackOption.Value = null;
            }
            HeaderControl().Roles = _securityHelper.UserRoles.ToArray();
		}


		protected override void OnPreRender(EventArgs e)
		{
			ManagedItem.ApplyTermDependencies(true, null);
			SetComplexListsVisibility();
            RegisterPopupScript();
			base.OnPreRender(e);
		}


		internal override HtmlGenericControl HTMLBody()
		{
			return body;
		}

		internal override Control ResizablePanel()
		{
			return pnlGridBody;
		}

		protected override ManagedItemHeader HeaderControl()
		{
			return header;
		}

        private void RegisterPopupScript()
        {
            BaseSystemPage systemPage = this.Page as BaseSystemPage;
            if (systemPage == null)
                throw new NullReferenceException("page is null because ExternalTermControl.Page does not derive from BaseSystemPage.");
            Type t = this.Page.GetType();

            string scriptName = "_kh_ExternalTermLookup";
            string jsFunctionName = "btnRollbackSubmit";
            System.IO.StringWriter sw1 = new System.IO.StringWriter();
            string dlgProps = "center=yes; help=no; resizable=no; dialogHeight=40px; dialogWidth=480px; status=no";
            sw1.WriteLine("function {0}() ", jsFunctionName);
            sw1.WriteLine("{ ");
            sw1.WriteLine("  var retVal = window.showModalDialog('ManagedItemRollbackConfirmation.aspx?{0}={1}&{2}={3}&{4}={5}', '{6}', '{7}');", Common.Names._QS_ITAT_SYSTEM_ID, systemPage.ITATSystem.ID, Common.Names._QS_MANAGED_ITEM_ID, _managedItem.ManagedItemID, Common.Names._QS_MANAGED_ITEM_AUDIT_ID, _managedItemAuditID, string.Empty, dlgProps);
            sw1.WriteLine("  if (retVal && retVal.length > 0) ");
            sw1.WriteLine("  { ");
            sw1.WriteLine("	    document.getElementById('{0}').value = retVal; ", hfRollbackOption.ClientID);
            sw1.WriteLine("	    document.forms(0).submit(); ");
            sw1.WriteLine("  } ");
            sw1.WriteLine("} ");
            if (!this.Page.ClientScript.IsClientScriptBlockRegistered(t, scriptName))
                this.Page.ClientScript.RegisterClientScriptBlock(t, scriptName, sw1.ToString(), true);

            btnRollback.OnClientClick = string.Format("{0}(); return false;", jsFunctionName);
        }

		protected void grdResultsRowCommand(object sender, GridViewCommandEventArgs e)
		{
            _managedItemAuditID = (Guid)grdResults.DataKeys[grdResults.SelectedIndex].Value;
            btnRollback.Enabled = (int.Parse((string)e.CommandArgument) >= 0);				
		}

		protected void grdResultsRowDataBound(object sender, GridViewRowEventArgs e)
		{
			Literal litState = e.Row.FindControl("litState") as Literal;
			if (litState != null)
			{
				Guid stateId = (Guid)((DataRowView)e.Row.DataItem)["StateID"];
				Business.State state = _managedItem.Workflow.FindState(stateId);
				if (state != null)
					litState.Text = state.Name;
				else
					litState.Text = string.Empty;
			}
		}

        protected override void LoadViewState(object savedState)
        {
            base.LoadViewState(savedState);
            if (ViewState[_VS_MANGEDITEM_AUDIT_ID] != null)
                _managedItemAuditID = (Guid)ViewState[_VS_MANGEDITEM_AUDIT_ID];
        }

        protected override object SaveViewState()
        {
            ViewState[_VS_MANGEDITEM_AUDIT_ID] = _managedItemAuditID;
            return base.SaveViewState();
        }

	}
}
