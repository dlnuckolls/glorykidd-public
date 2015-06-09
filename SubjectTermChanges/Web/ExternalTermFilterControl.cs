using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Collections.Generic;

namespace Kindred.Knect.ITAT.Web
{
	public class ExternalTermFilterControl : WebControl
	{
		#region fields
		private string _searchableFieldName;
		private Dictionary<string, string> _bindingDataSource;
		private string _hiddenFieldId;
		#endregion


		#region properties
		public string SearchableFieldName
		{
			get { return _searchableFieldName; }
			set { _searchableFieldName = value; }
		}

		public Dictionary<string, string> BindingDataSource
		{
			get { return _bindingDataSource; }
			set { _bindingDataSource = value; }
		}

		public string HiddenFieldId
		{
			get { return _hiddenFieldId; }
			set { _hiddenFieldId = value; }
		}
		#endregion


		#region constructor

		public ExternalTermFilterControl()
			: base()
		{
		}

		#endregion


	}
}
