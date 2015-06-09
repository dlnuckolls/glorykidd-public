using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Xml;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Collections.Generic;


namespace Kindred.Knect.ITAT.Web
{
	public partial class UpdateSystem : BasePage
	{

		internal override Control ResizablePanel()
		{
			return pnlGridBody;
		}


		internal override HtmlGenericControl HTMLBody()
		{
			return this.htmlBody;
		}


		protected void Page_Load(object sender, EventArgs e)
		{
            if (!AllowAccess(Common.Names._ASR_SystemAdmin))
			{
                UnauthorizedPageAccess();
			}

			if (!IsPostBack)
				LoadGrid();
		}


    	protected void btnUpload_OnCommand(object sender, CommandEventArgs e)
		{
			if (!string.IsNullOrEmpty(txtSystemName.Text))
				if (!string.IsNullOrEmpty(filUpload.FileName))
					UploadSystemXML(hidSystemID.Value, txtSystemName.Text, filUpload.FileContent);
				else
					RegisterAlert("A file containing the System XML must be specified.");
			else
				RegisterAlert("A system name must be specified.");
		}


		protected void btnReset_OnCommand(object sender, CommandEventArgs e)
		{
			grd.SelectedIndex = -1;
			btnUpload.Text = "Add";
			hidSystemID.Value = string.Empty;
			txtSystemName.Text = string.Empty;
		}

		
		protected void grd_RowCreated(object sender, GridViewRowEventArgs e)
		{
			if (e.Row.RowType == DataControlRowType.DataRow)
			{
				int rowIndex = e.Row.RowIndex;
				//Assign each row an ID containing the RowIndex
				e.Row.ID = "R" + rowIndex.ToString();
				//Set the command argument for the Download buttons 
				((LinkButton)e.Row.Cells[2].Controls[0]).CommandArgument = rowIndex.ToString();
			}
		}


		protected void grd_RowCommand(object sender, GridViewCommandEventArgs e)
		{
			int rowIndex = Convert.ToInt32(e.CommandArgument);
			string systemID = grd.Rows[rowIndex].Cells[0].Text;
			string systemName = grd.Rows[rowIndex].Cells[1].Text;
			switch (e.CommandName)
			{
				case Common.Names._GRID_COMMAND_Download:
					DownloadSystemXML(systemID, systemName);
					break;

				case Common.Names._GRID_COMMAND_SingleClick:
					txtSystemName.Text = systemName;
					hidSystemID.Value = systemID;
					btnUpload.Text = "Update";
					break;

				default:
					break;
			}
		}


		//ASSUMPTION: if systemID.Length == 0, then this is a new system, otherwise it is an existing system
		private void UploadSystemXML(string systemID, string systemName, System.IO.Stream inputStream)
		{
			if (string.IsNullOrEmpty(systemID))
			{
				ITAT.Business.ITATSystem.CreateNew(Guid.NewGuid(), systemName, inputStream);
				RegisterAlert(String.Format("System \"{0}\" has been created and stored.", systemName));
				LoadGrid();
			}
			else
			{
				List<string> validation = new List<string>();
				ITAT.Business.ITATSystem itatSystem = ITAT.Business.ITATSystem.Get(new Guid(systemID), validation);
				if (itatSystem == null)
				{
					itatSystem = ITAT.Business.ITATSystem.CreateNew(new Guid(systemID), systemName, inputStream);
					RegisterAlert(String.Format("System \"{0}\" has been created and stored.", systemName));
				}
				else
				{
					itatSystem.UpdateSystemDef(inputStream);
					RegisterAlert(String.Format("System \"{0}\" has been updated.", systemName));
				}
			}
		}


		private void DownloadSystemXML(string systemID, string systemName)
		{
			string xml = string.Empty;
			using (DataSet ds = Data.ITATSystem.GetSystem(new Guid(systemID)))
			{
				xml = (string)ds.Tables[0].Rows[0]["ITATSystemDef"];
			}

			if (string.IsNullOrEmpty(xml))
				RegisterAlert("SystemDef not found");
			else
			{
				Response.Clear();
				Response.Buffer = true;
				Response.AddHeader("Content-Disposition", string.Format("attachment; filename={0}-SystemDef.xml", systemName));
				Response.ContentType = "Text/XML";
				Response.Write(xml);
				Response.End();
			}
		}



		private void LoadGrid()
		{
			using (DataSet ds = Business.ITATSystem.GetSystemList())
			{
				grd.DataSource = ds;
				grd.DataBind();
			}
		}



	}
}
