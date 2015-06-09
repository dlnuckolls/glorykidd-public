using System;
using System.Collections.Generic;
using System.Text;
using System.Web.UI.WebControls;

namespace Kindred.Knect.ITAT.Web
{
	public class ITATButton : System.Web.UI.WebControls.Button
	{


		#region constructors

		public ITATButton(string text, System.Web.UI.WebControls.CommandEventHandler eventHandler)
			: this(text, eventHandler, 0)
		{
		}



		public ITATButton(string text, System.Web.UI.WebControls.CommandEventHandler eventHandler, int width)
		{
			//width<=0 means use the default width
			this.Text = text;
			this.ID = GetID(text);
			this.CssClass = "KnectButton";
			if (width > 0)
				this.Width = width;
			this.OnClientClick = "return ShowWait(this);";

			this.CommandName = text;
			this.CommandArgument = String.Empty;
			this.Command += eventHandler;
		}


		#endregion


		#region static methods
		public static string GetID(string text)
		{
			return "_itat_btn" + text;
		}
		#endregion


	}
}
