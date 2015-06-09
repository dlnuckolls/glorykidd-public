using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Data;
using Kindred.Knect.ITAT.Business;

namespace Kindred.Knect.ITAT.Web
{
    public partial class TemplateRollback : BaseTemplatePage
    {

        internal override HtmlGenericControl HTMLBody()
        {
            return body;
        }

        internal override Control ResizablePanel()
        {
            return pnlGridBody;
        }


        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!IsPostBack)
            {
                BindGrid();

            }
            btnRollback.OnClientClick += "return confirm('Are you sure you want to rollback?');";
        }

        private void BindGrid()
        {            
            DataTable dt = GetAuditTypes();
            grdResults.DataSource = Data.Template.GetTemplateHistory(_template.ID, dt).Tables[0];
            grdResults.DataBind();
        }

        /// <summary>
        /// This method will return the audit types that this page should show
        /// </summary>
        /// <returns>DataTable</returns>
        private static DataTable GetAuditTypes()
        {
            DataTable dtAuditTypes = new DataTable();
            dtAuditTypes.Columns.Add(Data.DataNames._C_AuditTypeID);

            DataRow drPromoted = dtAuditTypes.NewRow();
            drPromoted[Data.DataNames._C_AuditTypeID] = (int)Retro.AuditType.Promoted;
            dtAuditTypes.Rows.Add(drPromoted);
            DataRow drRetroWithEditLanguage = dtAuditTypes.NewRow();
            drRetroWithEditLanguage[Data.DataNames._C_AuditTypeID] = (int)Retro.AuditType.RetroWithEditLanguage;
            dtAuditTypes.Rows.Add(drRetroWithEditLanguage);
            DataRow drRetroWithoutEditLanguage = dtAuditTypes.NewRow();
            drRetroWithoutEditLanguage[Data.DataNames._C_AuditTypeID] = (int)Retro.AuditType.RetroWithoutEditLanguage;
            dtAuditTypes.Rows.Add(drRetroWithoutEditLanguage);
            DataRow drRollBack = dtAuditTypes.NewRow();
            drRollBack[Data.DataNames._C_AuditTypeID] = (int)Retro.AuditType.RollBack;
            dtAuditTypes.Rows.Add(drRollBack);
            return dtAuditTypes;
        }

        protected override TemplateHeader HeaderControl()
        {
            return (TemplateHeader)header;
        }



        protected void grdResults_RowCreated(object sender, GridViewRowEventArgs e)
        {

        }

        protected void btnRollback_Command(object sender, EventArgs e)
        {
            DateTime? keyDate = null;
            
            DataTable dtRetroDetails = _template.GetTemplateRetroDetails();

            foreach (DataRow dr in dtRetroDetails.Rows)
            {
                if (keyDate.HasValue)
                {
                    if (DateTime.Compare(keyDate.Value, (DateTime)dr[Data.DataNames._C_DateOfChange]) < 0)
                    {
                        keyDate = (DateTime)dr[Data.DataNames._C_DateOfChange];
                    }             
                }
                else
                {
                    keyDate = (DateTime)dr[Data.DataNames._C_DateOfChange];
                }

            }


            if (keyDate.HasValue)
            {
                DataTable dtTemplateAudit = _template.GetTemplateAudit((Guid)(grdResults.DataKeys[grdResults.SelectedIndex].Value));

                if (DateTime.Compare((DateTime)dtTemplateAudit.Rows[0][Data.DataNames._C_DateOfChange], keyDate.Value) < 0)
                {
                    //don't do a rollback
                    RegisterAlert("You cannot rollback to this point as retro has already been applied");
                    return;
                }
            }



            PerformRollback();
           

        }

        private void PerformRollback()
        {
            Template.Rollback(_template.ID, (Guid)(grdResults.DataKeys[grdResults.SelectedIndex].Value));
            RegisterAlert("The template has been successfully rolled back to the selected point");
            RegisterRedirect(string.Format("TemplateMain.aspx{0}", Utility.TextHelper.QueryString(true, Common.Names._QS_ITAT_SYSTEM_ID, _itatSystem.ID.ToString(), Common.Names._CNTXT_Template, _template.ID.ToString())));
        }

        protected void grdResults_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            btnRollback.Enabled = Convert.ToInt32(e.CommandArgument) >= 0;
        }


    }
}
