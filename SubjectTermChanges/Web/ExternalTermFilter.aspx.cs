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
	public partial class ExternalTermFilter : BaseSystemPage
	{
		private const string _VSKEY_EXTERNALTERM = "_kh_vskey_ExternalTerm";
		private const string _VSKEY_INTERFACECONFIG = "_kh_vskey_InterfaceConfig";
		private const string _VSKEY_DATASOURCEPREFIX = "_kh_vskey_DataSourcePrefix_";
		private const string _VSKEY_HIDDENFIELDIDS = "_kh_vskey_HiddenFieldIds";
		private const string _VSKEY_FILTEREDDATA = "_kh_vskey_FilteredData";

		private Business.ExternalTerm _externalTerm;
		private Dictionary<string, string> _hiddenFieldIds;

		internal override HtmlGenericControl HTMLBody()
		{
			return this.body;
		}

		internal override Control ResizablePanel()
		{
			return pnlGrid;
		}


		protected override void OnInit(EventArgs e)
		{
			base.OnInit(e);
			ClientScript.RegisterClientScriptInclude(this.GetType(), "_kh_jsITAT", "Scripts/itat.js");
		}

		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);
			if (!IsPostBack)
			{
				_externalTerm = GetTerm();
				_hiddenFieldIds = new Dictionary<string, string>();
				LoadFilterCriteriaControls();
			}
		}

		private Business.ExternalTerm GetTerm()
		{
			Business.ExternalTerm rtn = null;
			if (string.IsNullOrEmpty(Request.QueryString["item"]))
			{
				string externalInterfaceConfigName = Request.QueryString["eic"];
				Business.ExternalInterfaceConfig eic = _itatSystem.FindExternalInterfaceConfig(externalInterfaceConfigName);
				rtn = new Kindred.Knect.ITAT.Business.ExternalTerm(false, eic,null,null,false);
				if (rtn == null)
					throw new Exception(string.Format("Error finding External Interface Config '{0}'.", externalInterfaceConfigName));
			}
			else
			{
				Guid termId = new Guid(Request.QueryString["term"]);
				Guid managedItemId = new Guid(Request.QueryString["item"]);
				Business.ManagedItem mi = Business.ManagedItem.Get(managedItemId, false);
				rtn = mi.FindTerm(termId) as Business.ExternalTerm;
				if (rtn == null)
					throw new Exception(string.Format("Error finding term '{0}'.", termId));
				//refresh the term's InterfaceConfig object (in case the current system interface config is different than the interface config stored with the term)
				rtn.RefreshInterfaceConfig(_itatSystem);
			}
			return rtn;
		}


		protected void btnGetDataOnCommand(object sender, CommandEventArgs e)
		{
			string[][] criteria = GetFilterCriteria();
			Kindred.Knect.ITAT.ExternalTerm.ExternalInterface externalInterface = new Kindred.Knect.ITAT.ExternalTerm.ExternalInterface(_externalTerm.InterfaceConfig.Name, _externalTerm.InterfaceConfig.WebServiceURL);
			DataTable filteredData = externalInterface.GetFilteredData(criteria).Tables[0];
			foreach (Business.ExternalInterfaceAvailableField availableField in _externalTerm.InterfaceConfig.AvailableFields)
				if (!filteredData.Columns.Contains(availableField.Name))
					filteredData.Columns.Remove(availableField.Name);
			filteredData.DefaultView.Sort = _externalTerm.InterfaceConfig.DisplayedFields(",");
			grd.DataSource = filteredData;

			grd.CheckboxColumn = 0;
			grd.TermName = "";
			grd.RowHighlighting = false;
			grd.EnableClickEvent = false;
			grd.EnableDoubleClickEvent = false;
			grd.EnableHeaderClick = false;
			grd.AutoGenerateColumns = false;
			grd.Container = pnlGrid.ID;
			grd.HeaderContainer = pnlGridHeader.ID;
			grd.ShowHeader = true;

			List<Business.ExternalInterfaceAvailableField> displayedFields = _externalTerm.InterfaceConfig.DisplayedFields();
			List<string> displayWidths = new List<string>();
			List<string> boundColumns = new List<string>();
			grd.ColumnHeaders.Clear();
			foreach (Business.ExternalInterfaceAvailableField displayedField in displayedFields)
			{
				grd.ColumnHeaders.Add(displayedField.DisplayName);
				displayWidths.Add(displayedField.DisplayWidth);
				boundColumns.Add(displayedField.Name);
			}
			grd.ColumnWidths = string.Join(" ", displayWidths.ToArray());
			grd.BoundColumns = string.Join(" ", boundColumns.ToArray());

			List<string> keyFieldNames = new List<string>();
			foreach (Business.ExternalInterfaceAvailableField availableField in _externalTerm.InterfaceConfig.AvailableFields)
			{
				if (availableField.IsKeyField)
					keyFieldNames.Add(availableField.Name);
			}
			grd.DataKeyNames = keyFieldNames.ToArray();	

			grd.DataBind();

			if (filteredData.Rows.Count == 0)
				RegisterAlert("No items meet your selection criteria.");
			pnlFilterButtons.Visible = false;
			btnOK.Enabled = (filteredData.Rows.Count > 0);
			btnSearchAgain.Visible = true;
			chkSelectAll.Visible = true;
			chkSelectAll.Attributes["onclick"] = string.Format("SelectAllCheckboxes(document.getElementById('{0}'), this.checked);", pnlGrid.ClientID);
			ViewState[_VSKEY_FILTEREDDATA] = filteredData;
		}


		protected void btnOKOnCommand(object sender, CommandEventArgs e)
		{
			DataTable filteredData = (DataTable)ViewState[_VSKEY_FILTEREDDATA];
			if (filteredData == null)
			    throw new Exception("OK was clicked but filtered data is null.");
			filteredData.DefaultView.Sort = _externalTerm.InterfaceConfig.DisplayedFields(",");
			System.Text.StringBuilder sb = new System.Text.StringBuilder();
			foreach (int rowNumber in grd.SelectedRows)
			{
				sb.Append(grd.DataKeys[rowNumber].Value);
				sb.Append("|");
				foreach (Business.ExternalInterfaceAvailableField availableField in _externalTerm.InterfaceConfig.AvailableFields)
				{
					sb.AppendFormat("{0}={1}|", Utility.EscapeHelper.Escape(availableField.Name, Kindred.Knect.ITAT.Utility.EscapeMode.JavaScript),  Utility.EscapeHelper.Escape(filteredData.DefaultView[rowNumber][availableField.Name].ToString(), Kindred.Knect.ITAT.Utility.EscapeMode.JavaScript));
				}
				sb[sb.Length - 1] = '~';
			}
			CloseDialog(sb.ToString());
		}


		protected void btnCancelOnCommand(object sender, CommandEventArgs e)
		{
			CloseDialog();
		}

		protected void btnSearchAgainOnCommand(object sender, CommandEventArgs e)
		{
			Response.Redirect(this.Request.Url.PathAndQuery);
		}

		private string[][] GetFilterCriteria()
		{
			string[][] rtn = new string[_externalTerm.InterfaceConfig.SearchableFields.Count][];
			int index = 0;
			foreach (Business.ExternalInterfaceSearchableField searchableField in _externalTerm.InterfaceConfig.SearchableFields)
			{
				if (!searchableField.Visible)
				{
					//This is a system pre-defined filter that should always be applied (in addition to any user-selected criteria)
					rtn[index] = new string[] { searchableField.Name, searchableField.Filter };
				}
				else
				{
					switch (searchableField.FieldType)
					{
						case Business.SearchableFieldType.MultiValue:
							Dictionary<string, string> dataSource = (Dictionary<string, string>)ViewState[_VSKEY_DATASOURCEPREFIX + searchableField.Name];
							string[] keys = new string[dataSource.Keys.Count];
							dataSource.Keys.CopyTo(keys, 0);
							string[] selectedRows = Request.Form[_hiddenFieldIds[searchableField.Name]].Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
							string[] selectedKeys = new string[selectedRows.Length];
							for (int i = 0; i < selectedRows.Length; i++)
								selectedKeys[i] = keys[int.Parse(selectedRows[i])];
							rtn[index] = new string[] { searchableField.Name, string.Join("|", selectedKeys) };
							break;
						case Business.SearchableFieldType.Entry:
							//get value in text box from Request.Form
							rtn[index] = new string[] { searchableField.Name, Request.Form[Helper.ControlID(searchableField.Name)] };
							break;
						case Business.SearchableFieldType.Fixed:
						default:
							rtn[index] = new string[] { searchableField.Name, string.Empty };
							break;
					}
				}
				index++;
			}
			return rtn;
		}


		private void LoadFilterCriteriaControls()
		{
			foreach (Business.ExternalInterfaceSearchableField searchableField in _externalTerm.InterfaceConfig.SearchableFields)
			{
				if (searchableField.Visible)
				{
					WebControl c = CreateFilterControl(searchableField);
					if (c != null)
					{
						TableRow row = new TableRow();

						TableCell cellLabel = new TableCell();
						cellLabel.CssClass = Common.Names._STYLE_CSSCLASS_EXTTERM_CAPTIONCELL;
						cellLabel.Width = Unit.Pixel(120);

						Label lblCellLabel = new Label();
						lblCellLabel.CssClass = "FormCaption";
						lblCellLabel.Font.Bold = true;
						lblCellLabel.Text = searchableField.Name;
						cellLabel.Controls.Add(lblCellLabel);

						if (searchableField.FieldType == Kindred.Knect.ITAT.Business.SearchableFieldType.MultiValue)
						{
							cellLabel.Style["padding-top"] = "0";

							CheckBox chkFilterCriteriaSelectAll = new CheckBox();
							chkFilterCriteriaSelectAll.Attributes["onclick"] = string.Format("SelectAllCheckboxes(document.getElementById('{0}'), this.checked);", c.ClientID);
							cellLabel.Controls.Add(chkFilterCriteriaSelectAll);
						}
						row.Cells.Add(cellLabel);
					
						Panel panelControl = new Panel();
						panelControl.CssClass = Common.Names._STYLE_CSSCLASS_EXTTERM_CONTROLPANEL;
						FormatControlPanel(panelControl, c);
						panelControl.Controls.Add(c);

						TableCell cellControl = new TableCell();
						cellControl.CssClass = Common.Names._STYLE_CSSCLASS_EXTTERM_CONTROLCELL;
						cellControl.Controls.Add(panelControl);
						row.Cells.Add(cellControl);

						tblFilterControls.Rows.Add(row);
					}
				}
			}
		}


		private void FormatControlPanel(Panel p, WebControl c)
		{
			MultiSelectGrid msg = c as MultiSelectGrid;
			if (msg != null)
			{
				p.Height = Unit.Pixel(20 * Math.Min(4, msg.Rows.Count));
				msg.Container = p.ID;
			}
			else
				p.Height = Unit.Pixel(24);
		}


		private WebControl CreateFilterControl(Kindred.Knect.ITAT.Business.ExternalInterfaceSearchableField searchableField)
		{
			switch (searchableField.FieldType)
			{
				case Business.SearchableFieldType.Fixed:
					return null;

				case Business.SearchableFieldType.MultiValue:
					Kindred.Knect.ITAT.ExternalTerm.ExternalInterface externalInterface = new Kindred.Knect.ITAT.ExternalTerm.ExternalInterface(_externalTerm.InterfaceConfig.Name, _externalTerm.InterfaceConfig.WebServiceURL);
					Dictionary<string, string> dataSource = null;
					if (string.IsNullOrEmpty(searchableField.Filter))
						dataSource = BindableDataSource(externalInterface.GetFilterList(searchableField.Name, null));
					else
						dataSource = BindableDataSource(externalInterface.GetFilterList(searchableField.Name, searchableField.Filter.Split(new char[] {'|'}, StringSplitOptions.RemoveEmptyEntries)));
					ViewState[_VSKEY_DATASOURCEPREFIX + searchableField.Name] = dataSource;
					MultiSelectGrid msg = MultiSelectSearchControl(searchableField, dataSource);
					_hiddenFieldIds.Add(searchableField.Name, msg.SelectedRowsHiddenFieldID);
					return msg;

				case Business.SearchableFieldType.Entry:
					return TextSearchControl(searchableField);

				default:
					return null;
			}
		}


		private Dictionary<string, string> BindableDataSource(string[][] dataSource)
		{
			string[] values = new string[dataSource.Length];
			Array.Sort<string[]>(dataSource, new ValueComparer());

			Dictionary<string, string> rtn = new Dictionary<string, string>();
			for (int index = 0; index < dataSource.GetLength(0); index++)
				rtn.Add(dataSource[index][0], dataSource[index][1]);
			return rtn;
		}


		private MultiSelectGrid MultiSelectSearchControl(Business.ExternalInterfaceSearchableField searchableField, Dictionary<string,string> dataSource)
		{
			MultiSelectGrid msg = new MultiSelectGrid();
			msg.ID = Helper.ControlID(searchableField.Name);
			msg.CssClass = "MSG";
			msg.CheckboxColumn = 0;
			msg.DataSource = dataSource;
			msg.BoundColumns = "Value";
			msg.Container = string.Empty;
			msg.HeaderContainer = string.Empty;
			msg.AutoGenerateColumns = false;
			msg.RowHighlighting = false;
			msg.EnableClickEvent = false;
			msg.EnableDoubleClickEvent = false;
			msg.EnableHeaderClick = false;
			msg.ShowHeader = false;
			msg.ColumnWidths = "100%";
			msg.TermName =  searchableField.Name;
			if (IsPostBack)
			{
				msg.DataBind();
				PopulateGrid(msg);
			}
			else
			{
				msg.DataKeyNames = new string[] { "Key" };
				msg.DataBind();
			}
			return msg;
		}

		private void PopulateGrid(MultiSelectGrid msg)
		{
			foreach (int rowNumber in msg.SelectedRows)
			{
				CheckBox chk = msg.Rows[rowNumber].Cells[msg.CheckboxColumn].Controls[0] as CheckBox;
				if (chk != null)
				{
					chk.Checked = true;
				}
			}
			
		}


		//private DropDownList SingleSelectSearchControl(Business.ExternalInterfaceSearchableField searchableField, Dictionary<string, string> dataSource)
		//{
		//    //TODO:  validate and test this code!!!
		//    DropDownList ddl = new DropDownList();
		//    ddl.DataSource = dataSource;
		//    ddl.DataTextField = "Value";
		//    ddl.DataValueField = "Key";
		//    ddl.DataBind();
		//    return ddl;
		//}


		private WebControl TextSearchControl(Business.ExternalInterfaceSearchableField searchableField)
		{
			WebControl rtn = Helper.CreateTextControl(searchableField.Name, 80, Unit.Percentage(100.0), 1);
			rtn.Style["margin"] = "2px 4px 0 4px";
			return rtn;
		}


		protected override object SaveViewState()
		{
			ViewState[_VSKEY_EXTERNALTERM] = _externalTerm;
			ViewState[_VSKEY_HIDDENFIELDIDS] = _hiddenFieldIds;
			return base.SaveViewState();
		}

		protected override void LoadViewState(object savedState)
		{
			base.LoadViewState(savedState);
			_externalTerm = (Business.ExternalTerm)ViewState[_VSKEY_EXTERNALTERM];
			_hiddenFieldIds = (Dictionary<string, string>)ViewState[_VSKEY_HIDDENFIELDIDS];
		}

	}

	public class ValueComparer : IComparer<string[]>
	{
		public int Compare(string[] x, string[] y)
		{
			if (x.Length != 2)
				throw new Exception("First item being compared is not an array of length 2.   Length=" + x.Length);
			if (y.Length != 2)
				throw new Exception("Second item being compared is not an array of length 2.   Length=" + y.Length);
			return x[1].CompareTo(y[1]);
		}
	}

}
