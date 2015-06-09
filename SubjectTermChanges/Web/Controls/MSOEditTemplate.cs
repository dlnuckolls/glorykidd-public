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
	public class MSOEditTemplate : ITemplate, INamingContainer
	{

		#region private members
		private bool _canEdit;
		#endregion



		#region constructor
		public MSOEditTemplate(bool canEdit)
		{
			_canEdit = canEdit;
		}
		#endregion


		#region ITemplate Members

		void ITemplate.InstantiateIn(Control container)
		{
			WebControl c = container as WebControl;
			if (c != null)
			{
				c.Width = Unit.Percentage(100.0);
			}

			Table tbl = new Table();
			tbl.Style["table-layout"] = "fixed";
			tbl.Style["margin"] = "0";
			tbl.Style["padding"] = "0";
			tbl.CellPadding = 0;
			tbl.CellSpacing = 0;
			tbl.Width = Unit.Percentage(100.0);
			TableRow row = new TableRow();
			tbl.Rows.Add(row);
			TableCell cell1 = new TableCell();
			cell1.Width = Unit.Pixel(150);
			row.Cells.Add(cell1);
			TableCell cell2 = new TableCell();
			cell2.Width = Unit.Percentage(100.0);
			row.Cells.Add(cell2);
			c.Controls.Add(tbl);

			//Creates a new label control and add it to the container.
			Label lbl = new Label();
			lbl.DataBinding += new EventHandler(lbl_DataBinding);
			cell1.Controls.Add(lbl);

			if (_canEdit)
			{
				//Creates a new text box control and add it to the container.
				TextBox txtbox = new TextBox();
				txtbox.Width = Unit.Percentage(100.0);
				txtbox.DataBinding += new EventHandler(txtbox_DataBinding);
				cell2.Controls.Add(txtbox);
			}
			else
			{
				//Creates a new text box control and add it to the container.
				Label lblValue = new Label();
				lblValue.Width = Unit.Percentage(100.0);
				lblValue.DataBinding += new EventHandler(lblValue_DataBinding);
				cell2.Controls.Add(lblValue);
			}
		}


		#endregion


		void lbl_DataBinding(object sender, EventArgs e)
		{
			Label lbl = (Label)sender;
			DataListItem item = (DataListItem)lbl.NamingContainer;
			Business.MSOKeyValueItem msoKeyValueItem = (Business.MSOKeyValueItem)item.DataItem;
			lbl.Text = msoKeyValueItem.FieldName;
		}

		void lblValue_DataBinding(object sender, EventArgs e)
		{
			Label lblValue = (Label)sender;
			DataListItem item = (DataListItem)lblValue.NamingContainer;
			Business.MSOKeyValueItem msoKeyValueitem = (Business.MSOKeyValueItem)item.DataItem;
			lblValue.ID = msoKeyValueitem.Key;
			lblValue.Text = msoKeyValueitem.FieldValue;
		}



		void txtbox_DataBinding(object sender, EventArgs e)
		{
			TextBox txt = (TextBox)sender;
			DataListItem item = (DataListItem)txt.NamingContainer;
			Business.MSOKeyValueItem msoKeyValueitem = (Business.MSOKeyValueItem)item.DataItem;
			txt.ID =  msoKeyValueitem.Key;
			txt.Text = msoKeyValueitem.FieldValue;
			txt.Attributes["onfocus"] = "javascript:this.select();";
		}

	}
}
