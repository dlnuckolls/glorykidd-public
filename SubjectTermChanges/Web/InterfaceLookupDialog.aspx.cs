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
	public partial class InterfaceLookupDialog : BasePage 
	{

		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);
			if (!IsPostBack)
				LoadData();
		}

		protected override void OnInit(EventArgs e)
		{
			base.OnInit(e);
		}


		private void LoadData()
		{
			switch (Request.QueryString["interface"].ToUpper())
			{
				case "MSO":
					((StandardHeader)itatHeader).PageTitle = "MSO Provider Lookup";
					//grd.ShowHeader = false;
					Interfaces.MSO ifcMSO = new Kindred.Knect.ITAT.Interfaces.MSO();
					ifcMSO.Criteria = Request.QueryString["param"];
					DataSet ds = ifcMSO.GetData();
					CreateColumns(ds);
					grd.DataSource = ds;
					grd.DataBind();
					break;

				default:
					break;
			}
		}


		private void CreateColumns(DataSet ds)
		{
			//Add columns to GridView
			if (ds != null)
			{
				grd.Columns.Clear();
				foreach (DataColumn column in ds.Tables[0].Columns)
				{
					BoundField fld = new BoundField();
					fld.DataField = column.ColumnName;
					fld.HeaderText = column.ColumnName;
					grd.Columns.Add(fld);
				}
			}
		}

		protected void grd_RowCommand(object sender, GridViewCommandEventArgs e)
		{
			switch (e.CommandName)
			{
				case "singleClick":
					System.Text.StringBuilder sb = new System.Text.StringBuilder();
					int rowIndex = int.Parse(e.CommandArgument.ToString());
					if (grd.Columns.Count > 1)
						for (int colNumber = 0; colNumber < grd.Columns.Count - 1; colNumber++)
						{
							sb.Append(grd.Rows[rowIndex].Cells[colNumber].Text);
							sb.Append("|");
						}
					if (grd.Columns.Count > 0)
						sb.Append(grd.Rows[rowIndex].Cells[grd.Columns.Count - 1].Text);
					CloseDialog(System.Web.HttpUtility.HtmlDecode(sb.ToString()));
					break;

				default:
					break;
			}
		}


		internal override HtmlGenericControl HTMLBody()
		{
			return body;
		}

		internal override Control ResizablePanel()
		{
			return pnlGridBody;
		}
 
	}
}
