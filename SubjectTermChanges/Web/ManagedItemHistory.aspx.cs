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
	public partial class ManagedItemHistory :  BaseManagedItemPage
	{
		internal override HtmlGenericControl HTMLBody()
		{
			return body;
		}

		internal override Control ResizablePanel()
		{
			return pnlGridBody;
		}


		protected void Page_Load(object sender, EventArgs e)
		{
			if (!IsPostBack)
			{
				DataSet ds = Data.ManagedItem.GetManagedItemHistory(_managedItem.ManagedItemID);
				grdResults.DataSource = ds.Tables[0];
				grdResults.DataBind();
			}
            HeaderControl().Roles = _securityHelper.UserRoles.ToArray();
        }

		protected override void OnPreRender(EventArgs e)
		{
			ManagedItem.ApplyTermDependencies(true,null);
			SetComplexListsVisibility();
			base.OnPreRender(e);
		}

		protected override ManagedItemHeader HeaderControl()
		{
			return (ManagedItemHeader)header;
		}
	}
}
