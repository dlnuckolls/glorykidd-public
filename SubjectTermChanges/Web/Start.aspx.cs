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
	public partial class Start : BaseSystemPage
	{

		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);
			lblSystem.Text = _itatSystem.Name;
			divBody.InnerHtml = _itatSystem.IntroPage;
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
