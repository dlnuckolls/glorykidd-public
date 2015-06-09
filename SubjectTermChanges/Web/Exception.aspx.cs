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
	public partial class ExceptionPage : BasePage
	{
		internal override HtmlGenericControl HTMLBody()
		{
			return this.htmlBody;
		}

		internal override Control ResizablePanel()
		{
			return null;
		}

		protected void Page_Load(object sender, EventArgs e)
		{
			SetTitle();
			SetMessage();
		}

		private void SetMessage()
		{
			string value = Request.QueryString["message"];
			if (!string.IsNullOrEmpty(value))
				ExceptionMessageLiteral.Text = value.Trim();
		}

		private void SetTitle()
		{
			string title = Request.QueryString[Common.Names._QS_ITAT_SYSTEM_ID];
			if (!string.IsNullOrEmpty(title))
				litSystemName.Text = title;
			else
                litSystemName.Text = APPLICATION_NAME;
		}

	}
}
