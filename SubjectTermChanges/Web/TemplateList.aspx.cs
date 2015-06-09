using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using Kindred.Common.Controls;
using System.Collections.Generic;

namespace Kindred.Knect.ITAT.Web
{
	public partial class TemplateList : BaseSystemPage
	{

		#region BasePage overrides


		internal override HtmlGenericControl HTMLBody()
		{
			return htmlBody;
		}

		internal override Control ResizablePanel()
		{
			return pnlGridBody;
		}

		protected override string GetPageName()
		{
			if (_itatSystem == null)
				return "Template List";
			else
				return _itatSystem.Name + " - Template List";
		}

		protected override string GetApplicationFunction()
		{
			return Business.XMLNames._AF_EditTemplate;
		}

		#endregion


		#region event handlers

		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);
			((StandardHeader)itatHeader).PageTitle = GetPageName();
			if (!IsPostBack)
				LoadShowDropDown();
		}

		protected override void OnPreRender(EventArgs e)
		{
			base.OnPreRender(e);
			LoadGrid();
		}

		protected void grdTemplateList_RowDataBound(object sender, GridViewRowEventArgs e)
		{
			if (e.Row.RowType == DataControlRowType.DataRow)
			{
				const int statusColumn = 1;
				System.Data.DataRowView drv = (System.Data.DataRowView)e.Row.DataItem;
                string statusValue = drv[Data.DataNames._C_Status].ToString();
				Business.TemplateStatusType status = (Business.TemplateStatusType)Enum.Parse(typeof(Business.TemplateStatusType), statusValue);
				e.Row.Cells[statusColumn].Text = status.ToString();
			}
		}


		protected void grdTemplateList_RowCreated(object sender, GridViewRowEventArgs e)
		{
			if (e.Row.RowType == DataControlRowType.DataRow)
			{
			   string rowIndex = e.Row.RowIndex.ToString();
			   ((LinkButton)e.Row.Cells[2].Controls[0]).CommandArgument = rowIndex;   //Edit button CommandArgument
			   ((LinkButton)e.Row.Cells[2].Controls[0]).CommandArgument = rowIndex;   //Copy button CommandArgument
			}
		}


		protected void grdTemplateList_RowCommand(object sender, GridViewCommandEventArgs e)
		{
			int index = Convert.ToInt32(e.CommandArgument);
			Guid id = (Guid)grdTemplateList.DataKeys[index].Value;
			switch (e.CommandName)
			{
				case Common.Names._GRID_COMMAND_Copy:
					string oldTemplateName = grdTemplateList.Rows[index].Cells[0].Text;
					Guid newID = CopyTemplate(id, oldTemplateName);
					Response.Redirect(String.Format("TemplateMain.aspx{0}", Utility.TextHelper.QueryString(true, Common.Names._QS_ITAT_SYSTEM_ID, _itatSystem.ID.ToString(), Common.Names._QS_TEMPLATE_ID, newID.ToString())));
					break;
				case Common.Names._GRID_COMMAND_Edit:
					Response.Redirect(String.Format("TemplateMain.aspx{0}", Utility.TextHelper.QueryString(true, Common.Names._QS_ITAT_SYSTEM_ID, _itatSystem.ID.ToString(), Common.Names._QS_TEMPLATE_ID, id.ToString())));
					break;
				default:
					throw new Exception(String.Format("Unknown CommandName in grdTemplateList: {0}", e.CommandName)); 
			}
		}


		protected void btnAdd_Click(object sender, EventArgs e)
		{
			List<string> validation = new List<string>();
			Guid newTemplateID = AddTemplate(validation);
			if (!RegisterValidationAlerts(validation,"Unable to create a new template due to the following system xml errors:"))
			{
				Response.Redirect(String.Format("TemplateMain.aspx{0}", Utility.TextHelper.QueryString(true, Common.Names._QS_ITAT_SYSTEM_ID, _itatSystem.ID.ToString(), Common.Names._QS_TEMPLATE_ID, newTemplateID.ToString())));
			}
		}

		#endregion


		#region Private methods

		private Guid AddTemplate(List<string> validation)
		{
			Guid rtn = Business.Template.AddTemplate(_itatSystem.ID, "New Template", "New Template", false, false, (short)Business.TemplateStatusType.Inactive, true, true, false, validation);
			return rtn;
		}

		private Guid CopyTemplate(Guid copyFromID, string oldName)
		{
			string newName = Business.Template.NewTemplateName(_itatSystem.ID, oldName);
			return Business.Template.CopyTemplate(_itatSystem, copyFromID, newName, newName);
		}

		private void LoadGrid()
		{
			if (ddlShowFilter.SelectedItem.Text.ToUpper() == "ALL")
			{
				List<short> templateStatuses = new List<short>();
				Business.TemplateStatusType[] templateStatusValues = (Business.TemplateStatusType[])Enum.GetValues(typeof(Business.TemplateStatusType));
				foreach (Business.TemplateStatusType templateStatusValue in templateStatusValues)
					templateStatuses.Add((short)templateStatusValue);

				using (DataSet ds = Business.Template.GetTemplateListWithStatus(_itatSystem.ID, templateStatuses,null))
				{
					grdTemplateList.DataSource = ds;
					grdTemplateList.DataBind();
				}
			}
			else
			{
                using (DataSet ds = Business.Template.GetTemplateListWithStatus(_itatSystem.ID, new short[] { short.Parse(ddlShowFilter.SelectedValue) }, null))
				{
					grdTemplateList.DataSource = ds;
					grdTemplateList.DataBind();
				}
			}
		}

		private void LoadShowDropDown()
		{
			Helper.LoadTemplateStatusDropDown(ddlShowFilter, true, Kindred.Knect.ITAT.Business.TemplateStatusType.Active);
		}




		#endregion
	}
}
