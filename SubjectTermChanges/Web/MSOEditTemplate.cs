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
	public class GridViewTemplateTextBox : ITemplate
	{
		private string _keyColumnName;
		private string _valueColumnName;
		private string _gridID;

		public GridViewTemplateTextBox(string gridID, string keyColumnName, string valueColumnName)
		{
			_keyColumnName = keyColumnName;
			_valueColumnName = valueColumnName;
			_gridID = gridID;
		}

		#region ITemplate Members

		void ITemplate.InstantiateIn(Control container)
		{

			//Creates a new text box control and add it to the container.
			TextBox txtbox = new TextBox();                        
			txtbox.Width = Unit.Percentage(100.0);
			txtbox.DataBinding += new EventHandler(txtbox_DataBinding);
			container.Controls.Add(txtbox);                            
		}

		#endregion


		void txtbox_DataBinding(object sender, EventArgs e)
		{
			TextBox txt = (TextBox)sender;
			GridViewRow row = (GridViewRow)txt.NamingContainer;
			GridView grd = (GridView)row.Parent.Parent;
			Business.MSOKeyValueItem msoKeyValueitem = (Business.MSOKeyValueItem)row.DataItem;
			txt.ID =  msoKeyValueitem.Key;
			txt.Text = msoKeyValueitem.FieldValue;
			//object dataValue = DataBinder.Eval(row.DataItem, _valueColumnName);
			//if (dataValue != null)
			//   txt.Text = dataValue.ToString();
			//else
			//   txt.Text = string.Empty;
		}

	}
}
