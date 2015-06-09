using System;
using System.Collections.Generic;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Kindred.Knect.ITAT.Business;

namespace Kindred.Knect.ITAT.Web
{

	/// <summary>
	/// Event Args class to define the args passed when an event is raised from the control
	/// </summary>
	public class HeaderEventArgs : EventArgs
	{
		private readonly string _commandName;
		private readonly string _commandArgument;

		public HeaderEventArgs(string commandName, string commandArgument)
		{
			_commandName = commandName;
			_commandArgument = commandArgument;
		}

		public string CommandName { get { return _commandName; } }
		public string CommandArgument { get { return _commandArgument; } }
	}


	public abstract class BaseHeader : System.Web.UI.UserControl
	{

		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);
		}

		protected override void OnPreRender(EventArgs e)
		{
			base.OnPreRender(e);
		}

		protected string GetPageBaseFileName()
		{
			string[] segments = this.Request.Path.Split(new char[] { '\\', '/' });
			return segments[segments.Length - 1];
		}



		#region events / delegates

		public event EventHandler<HeaderEventArgs> HeaderEvent;

		protected virtual void OnHeaderEvent(HeaderEventArgs e)
		{
			EventHandler<HeaderEventArgs> temp = HeaderEvent;
			if (temp != null)
				HeaderEvent(this, e);
		}

		#endregion



	}
}
