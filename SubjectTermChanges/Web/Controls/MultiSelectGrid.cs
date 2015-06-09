using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Kindred.Knect.ITAT.Web
{
	[ToolboxData("<{0}:multiselectgrid runat=server></{0}:multiselectgrid>")]
	public class MultiSelectGrid : Kindred.Common.Controls.KindredGridView
	{
		

			#region private members

		private List<int> _selectedRows;
		private List<int> _disabledRows;
		private int _checkboxColumn;
		private string[] _boundColumns;
		private string[] _columnWidths;
		private List<string> _columnHeaders;
		private bool _columnsCreated;
		private bool _hiddenFieldsParsed;
		private bool _doPostBack;
		private string _termName;
		//private string _controlHasFocus;

		private const string HF_SUFFIX_SELECTEDROWS = "_SelectedRows";
		private const string HF_SUFFIX_DISABLEDROWS = "_DisabledRows";
		private const string VSKEY_CHECKBOXCOLUMN = "_kh_vs_CheckboxColumn_";

			#endregion


			#region properties

		public List<int> SelectedRows
		{
			get { return _selectedRows; }
			set { _selectedRows = value; }
		}

		public List<int> DisabledRows
		{
			get { return _disabledRows; }
			set { _disabledRows = value; }
		}

		public bool ColumnsCreated
		{
			get { return _columnsCreated; }
		}

		public bool DoPostBack
		{
			get { return _doPostBack; }
			set { _doPostBack = value; }
		}

		public string TermName
		{
			get { return _termName; }
			set { _termName = value; }
		}

		//public string ControlHasFocus
		//{
		//   get { return _controlHasFocus; }
		//   set { _controlHasFocus = value; }
		//}

		//
		// Summary:
		//     This property gets/sets the column in which the selection checkbox will be created.  Default=0
		[Category("Kindred")]
		[DefaultValue("0")]
		[Description("An integer indicating the column in which the selection checkbox will be created.  The default value is 0.")]
		public int CheckboxColumn
		{
			get { return _checkboxColumn; }
			set { _checkboxColumn = value; }
		}

		//
		// Summary:
		//     This property sets the data columns or public properties to which the columns of the grid will be bound, delimited by spaces
		[Category("Kindred")]
		[DefaultValue("")]
		[Description("A string value indicating the data columns or public properties to which the columns of the grid will be bound, delimited by spaces.")]
		public string BoundColumns
		{
			set { _boundColumns = value.Split(new char[] { ' ' }); }
		}

		//
		// Summary:
		//     This property sets the column widths for each column, delimited by spaces
		[Category("Kindred")]
		[DefaultValue("")]
		[Description("A string value indicating the width for each column, delimited by spaces.")]
		public string ColumnWidths
		{
			set { _columnWidths = value.Split(new char[] { ' ' }); }
		}


		//
		// Summary:
		//     This property sets or retrieves the column headers for each column
		public List<string> ColumnHeaders
		{
			get { return _columnHeaders; }
			set { _columnHeaders = value; }
		}

		public string SelectedRowsHiddenFieldID
		{
			get { return this.UniqueID + HF_SUFFIX_SELECTEDROWS; }
		}

		public string DisabledRowsHiddenFieldID
		{
			get { return this.UniqueID + HF_SUFFIX_DISABLEDROWS; }
		}

		#endregion

		#region Constructor

		public MultiSelectGrid()
		{
			_selectedRows = new List<int>();
			_disabledRows = new List<int>();
			_columnHeaders = new List<string>();
			_columnsCreated = false;
			_hiddenFieldsParsed = false;
			this.EnableViewState = false;
		}

		#endregion

		#region event handlers

		protected override void OnInit(EventArgs e)
		{
		   base.OnInit(e);
			if (Page.IsPostBack)
			{
				if (!_hiddenFieldsParsed)
					ParseHiddenFields();
			}
		}

		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);
		}


		protected override void OnPreRender(EventArgs e)
		{
			base.OnPreRender(e);
			Style["table-layout"] = "fixed";
			this.CellPadding = 0;
			this.CellSpacing = 0;
			this.BorderWidth = Unit.Pixel(0);
			Attributes["border"] = "0";

			WebControl container = this.GetContainerControl() as WebControl;
			if (container != null)
			{
				container.Style["overflow-y"] = "auto";
				container.Style["overflow-x"] = "hidden";
				container.Style["border"] = "inset thin";
			}

			if (!Page.IsPostBack)
				PopulateCheckBoxes();
			RegisterHiddenFields();
			RegisterClientSideEvents();
		}


		private void PopulateCheckBoxes()
		{
			foreach (int row in _selectedRows)
			{
				CheckBox chk = (CheckBox)(Rows[row].Cells[_checkboxColumn].Controls[0]);
				chk.Checked = true;
				if (_disabledRows.Contains(row))
					chk.Enabled = false;
			}
		}


		private void ParseHiddenFields()
		{
			_selectedRows = Utility.ListHelper.FromDelimitedString(Page.Request.Form[SelectedRowsHiddenFieldID], '|', true);
			_disabledRows = Utility.ListHelper.FromDelimitedString(Page.Request.Form[DisabledRowsHiddenFieldID], '|', true);
			_hiddenFieldsParsed = true;
		}


		private void RegisterHiddenFields()
		{
			Type t = Page.GetType();
			Page.ClientScript.RegisterHiddenField(SelectedRowsHiddenFieldID, Utility.ListHelper.ToDelimitedString(_selectedRows, '|', true));
			Page.ClientScript.RegisterHiddenField(DisabledRowsHiddenFieldID, Utility.ListHelper.ToDelimitedString(_disabledRows, '|', true));
		}

		private void RegisterClientSideEvents()
		{
			Type t = Page.GetType();
			string scriptName = "_kh_SelectedRowsHiddenField";
			System.IO.StringWriter sw = new System.IO.StringWriter();
			sw.WriteLine(" function ToggleSelected(hfID, row)			");
			sw.WriteLine("	{		");
			sw.WriteLine("		if (row < 0)		");
			sw.WriteLine("			return;		");
			sw.WriteLine("		var newValue;		");
			sw.WriteLine("		var hf = document.getElementById(hfID);		");
			sw.WriteLine("		if (hf)	");
			sw.WriteLine("		{	");
			sw.WriteLine("			var s = hf.value;	");
			sw.WriteLine("			var searchText = '|' + row.toString() + '|';	");
			sw.WriteLine("			var index1 = s.indexOf(searchText);	");
			sw.WriteLine("			if (index1 > -1)");
			sw.WriteLine("			{	");
			sw.WriteLine("				newValue = s.replace(searchText, '|'); ");
			sw.WriteLine("			}	");
			sw.WriteLine("			else");
			sw.WriteLine("			{	");
			sw.WriteLine("				if (s == '||') ");
			sw.WriteLine("					newValue = '|' + row.toString() + '|';		");
			sw.WriteLine("				else	");
			sw.WriteLine("					newValue = s + row.toString() + '|'; ");
			sw.WriteLine("			}	");
			sw.WriteLine("			hf.value = newValue;	");
			sw.WriteLine("		}	");
			sw.WriteLine("		window.event.cancelBubble=true;	");
			sw.WriteLine("	}		");
			if (!Page.ClientScript.IsClientScriptBlockRegistered(t, scriptName))
				Page.ClientScript.RegisterClientScriptBlock(t, scriptName, sw.ToString(), true);
		}


		public void CreateColumns()
		{
			this.Columns.Clear();
			for (int i = 0; i < _boundColumns.Length; i++)
			{
				if (i == _checkboxColumn)
				{
					BoundField chkbxfld = new BoundField();
					chkbxfld.DataField = null;
					this.Columns.Add(chkbxfld);
				}

				BoundField fld = new BoundField();
				fld.DataField = _boundColumns[i];
				if (_columnHeaders != null && _columnHeaders.Count > i)
					if (!string.IsNullOrEmpty(_columnHeaders[i]))
						fld.HeaderText = _columnHeaders[i];
				this.Columns.Add(fld);
			}
			_columnsCreated = true;
		}

		protected override void OnRowDeleted(GridViewDeletedEventArgs e)
		{
			base.OnRowDeleted(e);
		}

		protected override void OnDataBinding(EventArgs e)
		{
			if (!_columnsCreated)
				CreateColumns();
			base.OnDataBinding(e);
		}

		protected override void OnRowCreated(GridViewRowEventArgs e)
		{
			if (!_columnsCreated)
				CreateColumns();
			base.OnRowCreated(e);
			//add checkbox control to cell
			MultiSelectGridCheckBox chk = new MultiSelectGridCheckBox();
			chk.AutoPostBack = DoPostBack;
			chk.EnableViewState = false;
			string sControlName = string.Format("{0}$grid${1}", Helper.ControlID(_termName), e.Row.RowIndex.ToString());
			chk.Attributes["onclick"] = string.Format("ToggleSelected('{0}', {1}); ", SelectedRowsHiddenFieldID, e.Row.RowIndex);
			if (chk.Page is ManagedItemProfile)
				chk.Attributes["onclick"] += "ControlOnFocus();";
			e.Row.Cells[_checkboxColumn].Controls.Add(chk);

			//set column widths
			if ((e.Row.RowType == DataControlRowType.Header) || (!ShowHeader && e.Row.RowIndex == 0))
			{
				if (_columnWidths == null)
					_columnWidths = AutoCalcColumnWidths(e.Row.Cells.Count, (_checkboxColumn >= 0));
				for (int i = 0; i < _columnWidths.Length; i++)
				{
					if (i < _checkboxColumn)
					{
						e.Row.Cells[i].Width = Unit.Parse(_columnWidths[i]);
						if (e.Row.RowType == DataControlRowType.Header)
							e.Row.Cells[i].CssClass = "NoBorder";
					}
					else
					{
						if (i == _checkboxColumn)
							e.Row.Cells[i].Width = Unit.Pixel(24);
						e.Row.Cells[i + 1].Width = Unit.Parse(_columnWidths[i]);
						if (e.Row.RowType == DataControlRowType.Header)
							e.Row.Cells[i + 1].CssClass = "NoBorder";
					}
				}
			}
		}


		private string[] AutoCalcColumnWidths(int columnCount, bool hasCheckboxColumn)
		{
			List<string> widths = new List<string>();
			if (hasCheckboxColumn)
				columnCount--;
			double width = 100.0 / (double)columnCount;
			for (int i = 0; i < columnCount; i++)
				widths.Add(string.Format("{0}%", width));
			return widths.ToArray();
		}


		#endregion



	}
}
