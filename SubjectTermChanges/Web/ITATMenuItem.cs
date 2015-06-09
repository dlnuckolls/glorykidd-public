using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

namespace Kindred.Knect.ITAT.Web 
{
	public class ITATMenuItem : HyperLink  // LinkButton
	{
		public static string GetID(string groupCaption, string text)
		{
			return string.Concat(ITATMenuGroup.GetID(groupCaption), "_", text.Replace(" ", string.Empty));
		}

		public ITATMenuItem(string groupCaption, string caption, string url, System.Web.UI.WebControls.CommandEventHandler eventHandler, bool disableLink)
			: this(groupCaption, caption, url, eventHandler, disableLink, false)
		{
		}


		public ITATMenuItem(string groupCaption, string caption, string url, System.Web.UI.WebControls.CommandEventHandler eventHandler, bool disableLink, bool showAsDialog)
		{
			this.ID = ITATMenuItem.GetID(groupCaption, caption);
			this.Text = caption;
			this.CssClass = "menuLink";
			if (disableLink)
			{
				this.Enabled = false;
			}
			else
			{
				this.Enabled = true;
				if (showAsDialog)
				{
					string props = "'center=yes; help=no; resizable=yes; dialogHeight:' + (0.95 * screen.availHeight).toString() + 'px; dialogWidth:' + (0.9 *  screen.availWidth).toString() + 'px;'";
					this.Attributes["onclick"] = "window.showModalDialog('" + ResolveUrl(url) + "',''," + props + ");";
					this.NavigateUrl = "";
				}
				else
				{
					this.NavigateUrl = url;
				}
			}
		}


		private string UrlWithoutQueryString(string url)
		{
			return url.Substring(0, url.IndexOf("?"));
		}
	}
}
