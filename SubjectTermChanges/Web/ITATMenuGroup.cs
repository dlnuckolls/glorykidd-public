using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

namespace Kindred.Knect.ITAT.Web
{
	public class ITATMenuGroup : WebControl, INamingContainer
	{
		#region private members
		private string _url;
		private string _caption;
		private int _menuWidth;
		private int _submenuWidth;
		#endregion

		#region Properties
		public string Url
		{
			get { return _url; }
			set { _url = value; }
		}

		public string Caption
		{
			get { return _caption; }
			set { _caption = value; }
		}

		public int MenuWidth
		{
			get { return _menuWidth; }
			set { _menuWidth = value; }
		}

		public int SubmenuWidth
		{
			get { return _submenuWidth; }
			set { _submenuWidth = value; }
		}
		#endregion

		#region static methods
		public static string GetID(string caption)
		{
			return "_itatmenugroup_" + caption.Replace(" ", string.Empty);
		}
		#endregion

		#region constructors
		public ITATMenuGroup(HtmlGenericControl parent, string caption, string url, int width, int submenuWidth, bool showArrow) : base(HtmlTextWriterTag.Div)
		{
			_url = url;
			_caption = caption;
			_menuWidth = width;
			_submenuWidth = submenuWidth;

			this.ID = GetID(caption);
			this.CssClass = "menuGroup";
			this.Width = _menuWidth;

			Label menuHeader = new Label();
			menuHeader.CssClass = "menuHeader";
			menuHeader.Text = caption;
			this.Controls.Add(menuHeader);

			if (showArrow)
			{
				System.Web.UI.WebControls.Label menuArrow = new Label();
				menuArrow.CssClass = "menuArrow";
				menuArrow.Text = "6";
				this.Controls.Add(menuArrow);
			}

			parent.Controls.Add(this);

			this.Attributes.Add("onresize", "_kh_ShowWindowedControls(this);");
		}
		#endregion


	}
}
