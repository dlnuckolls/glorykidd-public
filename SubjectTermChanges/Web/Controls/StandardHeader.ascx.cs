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
	public partial class StandardHeader : System.Web.UI.UserControl
	{
		public string PageTitle
		{
			get { return litPageTitle.Text; }
			set { litPageTitle.Text = value; }
		}

		protected void Page_Load(object sender, EventArgs e)
		{

		}
	}
}