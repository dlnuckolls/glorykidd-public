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
using System.Collections.Generic;


namespace Kindred.Knect.ITAT.Web
{

	/// <summary>
	/// This page is designed to be run once per system (as a data cleanup task).  It replaces references to TextImage.aspx with a reference to TextImage.ashx.
	/// The TextImage.ashx file performs the same function as TextImage.aspx, but is much more efficient because it does not use the ASPX page lifecycle.
	/// The TextImage.aspx page is being kept (for now) for backward compatibility.
	/// </summary>
	public partial class UpgradeTextImages : BasePage
	{

		protected void Page_Load(object sender, EventArgs e)
		{
			if (!IsPostBack)
			{
				DataSet ds = Business.ITATSystem.GetSystemList();
				Helper.LoadListControl(ddlSystem, ds, "ITATSystemName", "ITATSystemID", "", true, "(Select a System)", "");
			}
			else
			{
				if (ddlSystem.SelectedIndex > 0)
				{
					string systemName = ddlSystem.SelectedItem.Text;
					Guid systemId = new Guid(ddlSystem.SelectedItem.Value);
					//UpgradeTemplateXMLTextImages(systemId);
					//UpgradeManagedItemXMLTextImages(systemId);
					Server.Transfer(string.Format("~/Message.aspx?message=The Text Image References for System \"{0}\" Have Been Upgraded.", systemName));
				}
				else
					RegisterAlert("Please select a system from the list");
			}
		}

		internal override HtmlGenericControl HTMLBody()
		{
			return htmlBody;
		}

		internal override Control ResizablePanel()
		{
			return null;
		}
	}
}
