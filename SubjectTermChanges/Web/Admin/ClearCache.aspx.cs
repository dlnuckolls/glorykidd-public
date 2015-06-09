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
	public partial class AdminClearCache : BasePage
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
					string systemId = ddlSystem.SelectedItem.Value;
					Business.ITATSystem.ClearSystemCache(new Guid(systemId));
					Server.Transfer(string.Format("~/Message.aspx?message=The Cache for System \"{0}\" Has Been Cleared", systemName));
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
