using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Collections.Generic;

namespace Kindred.Knect.ITAT.Web
{
	[Serializable]
	public abstract class BaseProfileControl : System.Web.UI.UserControl
	{

		#region private members

		private bool _canEdit;

		private const string _SINGLE_QUOTE = "&#039;";
		private const string _BACK_SLASH = "&#092;";
		private const string _LEFT_PAREN = "&#040;";
		private const string _RIGHT_PAREN = "&#041;";

		#endregion


		#region properties

		public abstract Business.Term Term
		{
			get;
			set;
		}

		public bool CanEdit
		{
			get { return _canEdit; }
			set { _canEdit = value; }
		}

		public bool IsHeaderControl { get; set; }


		#endregion


		#region control lifecycle events

		protected override void OnInit(EventArgs e)
		{
			base.OnInit(e);
			Page.ClientScript.GetPostBackEventReference(this, string.Empty);
		}

		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);
			if (Term == null)
				throw new NullReferenceException("The Term property has not been set.");
		}

		protected override void OnPreRender(EventArgs e)
		{
			base.OnPreRender(e);
			if (this.Page is ManagedItemProfile)
				SetOnFocus(this);
		}

		public override void RenderControl(HtmlTextWriter writer)
		{
            if (!Term.IsFilter)
    			writer.AddAttribute("TermID", Term.ID.ToString());
			base.RenderControl(writer);
		}

		protected virtual void SetOnFocus(Control c)
		{
			string script = "ControlOnFocus();";
			WebControl wc = c as WebControl;
			if (wc != null)
			{
				Helper.AddAttribute(wc.Attributes, "onfocus", script);
			}
			else
			{
				HtmlControl hc = c as HtmlControl;
				if (hc != null)
					Helper.AddAttribute(hc.Attributes, "onfocus", script);
			}
			foreach (Control child in c.Controls)
				SetOnFocus(child);
		}




		#endregion

		#region public static methods

		public static string EncodeName(string sRawName)
		{
			//This call converts a human readable string into one suitable for embedding into an html doc.
			//The resulting string will also be suitable for passing within a javascript method call.
			string sEncodedName = HttpUtility.HtmlEncode(sRawName);
			sEncodedName = sEncodedName.Replace("'", _SINGLE_QUOTE).Replace("\\", _BACK_SLASH).Replace("(", _LEFT_PAREN).Replace(")", _RIGHT_PAREN);
			return sEncodedName;
		}

		public static string DecodeName(string sEncodedName)
		{
			//This call reverses the encoding from call 'EncodeName'.
			string sDecodedName = sEncodedName.Replace(_SINGLE_QUOTE, "'").Replace(_BACK_SLASH, "\\").Replace(_LEFT_PAREN, "(").Replace(_RIGHT_PAREN, ")");
			sDecodedName = HttpUtility.HtmlDecode(sDecodedName);
			return sDecodedName;
		}

		#endregion

		#region private/protected methods

		////Pass in a 1-based part (i.e. '1' == array index 0)
		//protected string ControlName(int nPart)
		//{
		//   return ControlName(_controlHasFocus, nPart);
		//}

		//public static string ControlName(string controlHasFocus, int nPart)
		//{
		//   string sControlName = "";
		//   if (!string.IsNullOrEmpty(controlHasFocus))
		//   {
		//      string[] sParts = controlHasFocus.Split('$');
		//      if (sParts.Length >= nPart)
		//         return sParts[nPart - 1];
		//   }
		//   return sControlName;
		//}

		#endregion


		public abstract void UpdateTermValue(string termGroupContainerName);



	}
}
