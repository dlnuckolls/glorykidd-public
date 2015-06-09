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
	public partial class DateTermControl : BaseProfileControl
	{

		#region private members
		private Business.DateTerm _dateTerm;
		private string _dateFormat;
		#endregion

		#region properties

		public string DateFormat
		{
			get { return _dateFormat; }
			set { _dateFormat = value; }
		} 

		#endregion


		#region base class overrides

		public override Business.Term Term
		{
			get
			{
				return _dateTerm;
			}
			set
			{
				_dateTerm = value as Business.DateTerm;
				if (_dateTerm == null)
					throw new NullReferenceException("Unable to cast Term as a DateTerm");
			}
		}

		#endregion


		#region control lifecycle events

		protected override void OnInit(EventArgs e)
		{
			base.OnInit(e);
			RegisterChildControlEventHandlers();
		}

		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);
			_dateTerm = (Business.DateTerm)this.Term;
		}



		protected override void OnPreRender(EventArgs e)
		{
			base.OnPreRender(e);
			SetChildControlsValues();
		}

		#endregion


		#region private methods


		private void SetChildControlsValues()
		{
			lbl.Visible = !CanEdit;
			txt.Visible = CanEdit;
			img.Visible = CanEdit;
			if (_dateTerm.Value.HasValue)
				txt.Text = Utility.DateHelper.FormatDate(_dateTerm.Value.Value, _dateFormat);
			else
				txt.Text = string.Empty;
			//if (ControlName(1).Length > 0)
			//   txt.Focus();
			lbl.Text = txt.Text;
		}


		private void RegisterChildControlEventHandlers()
		{
			txt.Attributes["onfocus"] = "javascript:this.select();";
			img.Attributes["onclick"] = string.Format("window.event.cancelBubble=true; ShowCalendar(document.all.CalFrame, window.frames.CalFrame, null, document.getElementById('{0}'), null, -600, 3600); ", txt.ClientID);
		}



		#endregion


		public override void UpdateTermValue(string termGroupContainerName)
		{
			string prefix = (string.IsNullOrEmpty(termGroupContainerName) ? "" : termGroupContainerName + "$");
			string value = this.Page.Request.Form[prefix + txt.UniqueID];
			DateTime dt;
			if (DateTime.TryParse(value, out dt))
				_dateTerm.Value = dt;
			else
				_dateTerm.Value = null;
		}


		//public override bool TryUpdateTermValue(ref System.Collections.Generic.List<string> validationErrors)
		//{
		//   DateTime? currentValue = _dateTerm.Value;
		//   string newValue = this.Page.Request.Form[txt.UniqueID];

		//   List<string> newValidationErrors = new List<string>();
		//   DateTime dt;
		//   if (DateTime.TryParse(newValue, out dt))
		//   {
		//      _dateTerm.Value = dt;
		//      newValidationErrors.AddRange(_dateTerm.Validate());
		//   }
		//   else
		//   {
		//      newValidationErrors.Add(string.Format("The value of {0}, '{1}', is not a valid date.", _dateTerm.Name, newValue));
		//   }

		//   if (newValidationErrors.Count == 0)
		//   {
		//      return true;
		//   }
		//   else
		//   {
		//      validationErrors.AddRange(newValidationErrors);
		//      _dateTerm.Value = currentValue;
		//      return false;
		//   }
		//}


	}
}