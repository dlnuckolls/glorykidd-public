using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Data;

namespace Kindred.Knect.ITAT.Web.Admin
{
    public partial class DataStoreDefinitionList : BasePage
    {
        private ITAT.Business.ITATSystem _itatSystem;


        # region Base Override
        internal override Control ResizablePanel()
        {
            return null;
        }


        internal override HtmlGenericControl HTMLBody()
        {
            return this.htmlBody;
        }
        #endregion

        #region Events
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
        }
        protected void Page_Load(object sender, EventArgs e)
        {


            if (!AllowAccess(Common.Names._ASR_SystemAdmin))
            {
                UnauthorizedPageAccess();
            }

            
                string qsValue = Request.QueryString[Common.Names._QS_ITAT_SYSTEM_ID];
                //ASSUMPTION: 
                //   If qsValue is of the form xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx or {xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx}, 
                //           then it is a systemID.
                //   Otherwise, it is the system name.
                if (qsValue!=null)
                {
                    if (Utility.TextHelper.IsGuid(qsValue))
                        _itatSystem = Business.ITATSystem.Get(new Guid(qsValue));
                    else
                        _itatSystem = Business.ITATSystem.Get(qsValue);
                }
                else
                {
                    _itatSystem = null;
                }

            

            if (!IsPostBack)
            {
                DataSet ds = Business.ITATSystem.GetSystemList();

                if (_itatSystem != null)
                {
                    Helper.LoadListControl(ddlSystem, ds, "ITATSystemName", "ITATSystemID", _itatSystem.ID.ToString(), true, "(Select a System)", "");
                    ddlSystemOnSelectedIndexChanged(this, e);
                }
                else
                    Helper.LoadListControl(ddlSystem, ds, "ITATSystemName", "ITATSystemID", "", true, "(Select a System)", "");
                
            }

        }


        protected void ddlSystemOnSelectedIndexChanged(object sender, EventArgs e)
        {
            using (DataTable dt = Business.DataStoreDefinition.GetDataStoreDefinitionsBySystemID(new Guid(ddlSystem.SelectedValue)))
            {
                grdResults.DataSource = dt;
                grdResults.DataBind();
            }
            if (grdResults.Rows.Count > 0)
                grdResults.EnableHeaderClick = true;

        }

        protected void btnAdd_Click(object sender, EventArgs e)
        {

            if (ddlSystem.SelectedIndex > 0)
            {
                Response.Redirect("AddDataStoreDefinition.aspx?System=" + ddlSystem.SelectedValue);
            }
            else
                RegisterAlert("Please select a system from the list");
        }

        protected void grdResults_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            int index = Convert.ToInt32(e.CommandArgument);
            switch (e.CommandName)
            {
                case Common.Names._GRID_COMMAND_Edit:
                    Guid id = (Guid)grdResults.DataKeys[index].Value;
                    Response.Redirect(String.Format("EditDataStoreDefinition.aspx{0}", Utility.TextHelper.QueryString(true, Common.Names._QS_ITAT_SYSTEM_ID, ddlSystem.SelectedValue, Common.Names._QS_DEFINITION_ID, id.ToString())));
                    break;
                case Common.Names._GRID_COMMAND_HeaderClick:
                    SortDirection sortDirection = SortDirection.Ascending;
                    int columnNumber = int.Parse((string)e.CommandArgument);
                    if (columnNumber == grdResults.SortColumn)
                    {
                        grdResults.SortAscending = !grdResults.SortAscending;
                        sortDirection = SortDirection.Descending;
                    }
                    else
                    {
                        grdResults.SortColumn = columnNumber;
                        grdResults.SortAscending = true;
                         sortDirection = SortDirection.Ascending;
                    }
                    switch (grdResults.SortColumn)
                    {
                        case 0: grdResults.Sort("Name", sortDirection);
                            break;
                        case 1: grdResults.Sort("Active", sortDirection);
                            break;
                        case 3: grdResults.Sort("LastModifiedDate", sortDirection);
                            break;
                        default : break;
                    }
                    
                    break;
                default:
                    throw new Exception(String.Format("Unknown CommandName in grdDefinitonList: {0}", e.CommandName));
            }
        }

        protected void gridView_Sorting(object sender, GridViewSortEventArgs e)
        {
            using (DataTable dataTable = Business.DataStoreDefinition.GetDataStoreDefinitionsBySystemID(new Guid(ddlSystem.SelectedValue)))
            {

            if (dataTable != null)
            {
                DataView dataView = new DataView(dataTable);
                dataView.Sort = e.SortExpression + " " + ConvertSortDirectionToSql(e.SortDirection);

                grdResults.DataSource = dataView;
                grdResults.DataBind();
            }
            }
        }

        private string ConvertSortDirectionToSql(SortDirection sortDirection)
        {
            string newSortDirection = String.Empty;

            switch (sortDirection)
            {
                case SortDirection.Ascending:
                    newSortDirection = "ASC";
                    break;

                case SortDirection.Descending:
                    newSortDirection = "DESC";
                    break;
            }

            return newSortDirection;
        }


        protected void grdResults_RowCreated(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                string rowIndex = e.Row.RowIndex.ToString();
                ((LinkButton)e.Row.Cells[4].Controls[0]).CommandArgument = rowIndex;   //Edit button CommandArgument
            }
        }

        protected void grdResults_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                const int statusColumn = 1;
                System.Data.DataRowView drv = (System.Data.DataRowView)e.Row.DataItem;
                string statusValue = drv["Active"].ToString();
                e.Row.Cells[statusColumn].Text = Boolean.Parse(statusValue) ? Common.Names._STATUS_Active : Common.Names._STATUS_Inactive;
            }
        }

        #endregion

    }



}