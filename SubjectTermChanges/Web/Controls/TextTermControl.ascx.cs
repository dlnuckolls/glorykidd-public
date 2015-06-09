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
using System.Collections.Generic;

namespace Kindred.Knect.ITAT.Web
{
	public partial class TextTermControl : BaseProfileControl 
	{

		#region private members
		private Business.TextTerm _textTerm;
		#endregion


		#region base class overrides

		public override Business.Term Term
		{
			get
			{
				return _textTerm;
			}
			set
			{
				_textTerm = value as Business.TextTerm;
				if (_textTerm == null)
					throw new NullReferenceException("Unable to cast Term as a TextTerm");
			}
		}

		#endregion


		#region control lifecycle events

		protected override void OnInit(EventArgs e)
		{
			base.OnInit(e);
		}

		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);
			_textTerm = (Business.TextTerm)this.Term;
			if (CanEdit)
				SetControlProperties();
		}


		protected override void OnPreRender(EventArgs e)
		{
			SetChildControlsValues();
			base.OnPreRender(e);
			RegisterChildControlEventHandlers();
		}

		#endregion


		#region private methods


		private void SetChildControlsValues()
		{
			if (CanEdit)
			{
				txt.Visible = true;
				lbl.Visible = false;
				txt.Text = _textTerm.DisplayValue(string.Empty);
			}
			else
			{
				lbl.Visible = true;
				txt.Visible = false;
                lbl.Text = _textTerm.DisplayValue(string.Empty).Replace("\n", "<br/>");     //Note - Added for 1.5.1
            }
		}


		private void RegisterChildControlEventHandlers()
		{
			txt.Attributes["onfocus"] = "javascript:this.select();";
			if (txt.Page is ManagedItemProfile)
				txt.Attributes["onfocus"] += "ControlOnFocus();";
			if (_textTerm.Format == Business.TextTermFormat.Plain)
			{
				//add client-side event handler to prevent keystrokes beyond the field's max length
				txt.Attributes["onchange"] = string.Format("javascript:_kh_txtOnChange(this, {0});", _textTerm.Max);

				Type t = this.Page.GetType();
				string scriptName = "_kh_TextTermControlOnChange";
				System.IO.StringWriter sw = new System.IO.StringWriter();
				sw.WriteLine("function _kh_txtOnChange(o, n) ");
				sw.WriteLine("{");
				sw.WriteLine("	if (o.value.length > n)");
				sw.WriteLine("		o.value = o.value.substring(0, n);");
				sw.WriteLine("}");
				if (!this.Page.ClientScript.IsStartupScriptRegistered(t, scriptName))
					this.Page.ClientScript.RegisterStartupScript(t, scriptName, sw.ToString(), true);
			}
		}



		private void SetControlProperties()
		{
			switch (_textTerm.Format)
			{
				case Business.TextTermFormat.Plain:
					const int characterPerLine = 80;
					int max = _textTerm.Max ?? 1;
					if (max > characterPerLine)
					{
						txt.TextMode = TextBoxMode.MultiLine;
						txt.Width = Unit.Percentage(100.0);
						txt.Rows = 1 + max / ( 1 + characterPerLine);
						txt.MaxLength = max;
					}
					else
					{
						txt.TextMode = TextBoxMode.SingleLine;
						txt.Width = Unit.Percentage(100.0);
						txt.MaxLength = max;
					}
					break;
				case Business.TextTermFormat.Number:
					if (_textTerm.UseTextNumberFormat ?? false)
					{
						txt.MaxLength = 0;
						txt.Width = Unit.Percentage(100);
					}
					else
					{
						txt.MaxLength = 10;
						txt.Width = Unit.Pixel(120);
					}
					break;
				case Business.TextTermFormat.Currency:
					txt.MaxLength = 20;
					txt.Width = Unit.Pixel(120);
					break;
				case Business.TextTermFormat.SSN:
					txt.MaxLength = 11;
					txt.Width = Unit.Pixel(120);
					break;
				case Business.TextTermFormat.Phone:
					txt.MaxLength = 15;
					txt.Width = Unit.Pixel(120);
					break;
				case Business.TextTermFormat.PhonePlusExtension:
					txt.MaxLength = 22;
					txt.Width = Unit.Pixel(200);
					break;
				default:
					break;
			}
		}


		#endregion


		public override void UpdateTermValue(string termGroupContainerName)
		{
			string prefix = (string.IsNullOrEmpty(termGroupContainerName) ? "" : termGroupContainerName + "$");
			//20070809_DEG Added this code for Bug #131.  Only look at the portion of the name before the first quote.
			//Note - This means that term names with embedded quotes must be unique before the first quote.
			string sUniqueID = Business.Term.NameAtFirstChar(prefix + txt.UniqueID, '\"');
			string value = this.Page.Request.Form[sUniqueID];
			// If value is null, then the term is not in Request.Form.  Therefore, it is not an editable control, and we do not want to update the term's value.
			// Note that a value of string.Empty IS a valid value that can be retrieved from Request.Form, and we SHOULD update the term's value in that case.
			if (value != null)
				_textTerm.Value = value;
		}

        protected void Page_Load(object sender, EventArgs e)
        {

        }
	}
}