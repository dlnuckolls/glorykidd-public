using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Kindred.Knect.ITAT.Web
{
	[DefaultProperty("Text")]
	[ToolboxData("<{0}:MultiSelectGridCheckBox runat=server></{0}:MultiSelectGridCheckBox>")]
	public class MultiSelectGridCheckBox : CheckBox
	{
		private string _VS_ROWINDEX = "_kh_vs_RowIndex";

		[Bindable(true)]
		[Localizable(true)]
		public int RowIndex
		{
			get
			{
				if (ViewState[_VS_ROWINDEX] == null)
					return -1;
				else
					return (int)ViewState[_VS_ROWINDEX];
			}

			set
			{
				ViewState[_VS_ROWINDEX] = value;
			}
		}

	}
}
