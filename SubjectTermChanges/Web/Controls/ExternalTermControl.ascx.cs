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
	public partial class ExternalTermControl : BaseProfileControl
	{

		#region private fields
		private Business.ExternalTerm _externalTerm;
		#endregion


		#region page lifecycle event handlers
		
		protected override void OnInit(EventArgs e)
		{
			base.OnInit(e);
			Page.ClientScript.RegisterHiddenField(GetHiddenFieldName(), "");
			InitializeGridControl();
		}

		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);
			if (!IsPostBack)
				if (this.Page is BaseManagedItemPage)
					if (!_externalTerm.ValuesLoaded)
						_externalTerm.LoadValues();
			if (CanEdit)
			{
				BindGrid();
				//if (_externalTerm.InterfaceConfig.SelectionMode == Kindred.Knect.ITAT.Business.ExternalInterfaceSelectionMode.MultiValued)
				//    BindGrid();
				//else
				//    BindDropDown();
			}
			else
			{
				lblText.Text = _externalTerm.DisplayValue("");
			}
		}

		//private void BindDropDown()
		//{
		//    ddlValue.DataSource = _externalTerm.SelectedItemValues();
		//    ddlValue.DataTextField = "DisplayValue";
		//    ddlValue.DataValueField = "Key";
		//    ddlValue.DataBind();
		//    ddlValue.Items.Insert(0, new ListItem("(Select One)", ""));
		//}

		private void BindGrid()
		{
			grd.DataSource = _externalTerm.SelectedItemValues();
			grd.DataBind();
		}

		protected override void OnPreRender(EventArgs e)
		{
			SetVisibility();
			RegisterEnableRemoveButtonScript();
			RegisterPopupScript();
			base.OnPreRender(e);
		}

		private void RegisterEnableRemoveButtonScript()
		{
			Type t = this.Page.GetType();
			string scriptName = "_kh_RegisterEnableRemoveButton";
			if (!this.Page.ClientScript.IsClientScriptBlockRegistered(t, scriptName))
			{
				System.IO.StringWriter sw1 = new System.IO.StringWriter();
				sw1.WriteLine("function EnableRemoveButton(removeButtonId, hfId) ");
				sw1.WriteLine("{ ");
				sw1.WriteLine("	var hf = document.getElementById(hfId); ");
				sw1.WriteLine("	if (hf); ");
				sw1.WriteLine("		document.getElementById(removeButtonId).disabled = (hf.value == '||' || hf.value == '|');");
				sw1.WriteLine("} ");
				this.Page.ClientScript.RegisterClientScriptBlock(t, scriptName, sw1.ToString(), true);
			}
		}

		#endregion


		#region properties

		public override Kindred.Knect.ITAT.Business.Term Term
		{
			get
			{
				return _externalTerm;
			}
			set
			{
				_externalTerm = value as Business.ExternalTerm;
				if (_externalTerm == null)
					throw new NullReferenceException("Unable to cast Term as a ExternalTerm");
				//_externalTerm.ITATSystem = ((BaseSystemPage)this.Page).ITATSystem;
			}
		}


		public string GetHiddenFieldName()
		{
			return this.ClientID.Replace(' ', '_') + "__NewValues";
		}


		#endregion


		public override void UpdateTermValue(string termGroupContainerName)
		{

			if (_externalTerm.SelectedItems == null)
				_externalTerm.InitializeSelectedItems();

			//Do NOT call GetHiddenFieldName here; this.ClientID does not containg the containing ManagedItemProfilePanel 
			//because this control is not nested in the parent panel yet.
			string prefix = (string.IsNullOrEmpty(termGroupContainerName) ? "" : termGroupContainerName + "_");
			string hiddenFieldName = prefix + this.ID + "__NewValues";  
			string hiddenFieldValue = Request.Form[hiddenFieldName];

			if (!string.IsNullOrEmpty(hiddenFieldValue))
			{
				string[] rows = hiddenFieldValue.Split(new char[] { '~' }, StringSplitOptions.RemoveEmptyEntries);
				foreach (string row in rows)
				{
					Business.ExternalInterfaceListItem listItem = new Business.ExternalInterfaceListItem(_externalTerm.InterfaceConfig);
					string[] values = row.Split(new char[] { '|' }, StringSplitOptions.None);
					listItem.Key = values[0];
					//check to see if listItem.Key is already in SelectedItems; if so, don't add the new item
					bool listItemAlreadyExists = false;
					foreach (Business.ExternalInterfaceListItem selectedItem in _externalTerm.SelectedItems)
						if (selectedItem.Key == listItem.Key)
						{
							listItemAlreadyExists = true;
							break;
						}
					if (!listItemAlreadyExists)
					{
						for (int i = 1; i < values.Length; i++)
						{
							string[] keyValuePair = values[i].Split(new char[] { '=' });
							string fieldName = HttpUtility.HtmlDecode(keyValuePair[0]);
							string fieldValue = HttpUtility.HtmlDecode(keyValuePair[1]);
							listItem.FieldValues.Add(fieldName, fieldValue);
						}
						_externalTerm.SelectedItems.Add(listItem);
					}
				}
				SetPageDirty(true);
			}
		}


		private void SetPageDirty(bool value)
		{
			BaseManagedItemPage baseMIPage = this.Page as BaseManagedItemPage;
			if (baseMIPage != null)
				baseMIPage.IsChanged = value;
		}


		//Roland Robertson 4/25/2008
		//The comments marked //@@@ are commented because the values of the ExternalInterfaceSelectionMode other than MultiValued are not implemented at this time.
		private void SetVisibility()
		{
			if (CanEdit)
			{
				pnlReadOnlyText.Visible = false;
				pnlButtons.Visible = true;
				//@@@if (_externalTerm.InterfaceConfig.SelectionMode == Business.ExternalInterfaceSelectionMode.MultiValued)
				//@@@{
					pnlGrid.Visible = true;
					pnlGridHeader.Visible = true;
					//@@@pnlSingleValueDropdown.Visible = false;
					pnlRemoveButton.Visible = true;
					//Set grdContainer height
					pnlGrid.Height = Unit.Pixel(85);   //  4 rows * 20 pixels/row + 5 pixels for padding
					pnlGrid.Width = Unit.Percentage(100.0);
					pnlGrid.Style.Add("vertical-align", "top");
				//@@@}
					//@@@else
					//@@@{
					//@@@    pnlGrid.Visible = false;
					//@@@   pnlGridHeader.Visible = false;
					//@@@   pnlSingleValueDropdown.Visible = true;
					//@@@    pnlRemoveButton.Visible = false;
					//@@@}
			}
			else
			{
				pnlReadOnlyText.Visible = true;
				pnlButtons.Visible = false;
				pnlGrid.Visible = false;
				pnlGridHeader.Visible = false;
				//@@@ pnlSingleValueDropdown.Visible = false;
			}
		}

	

		private void InitializeGridControl()
		{
			grd.CssClass = "MSG";
			grd.CheckboxColumn = 0;
			List<Business.ExternalInterfaceAvailableField> displayedFields = _externalTerm.InterfaceConfig.DisplayedFields();
			List<string> displayWidths = new List<string>();
			grd.ColumnHeaders.Clear();
			foreach (Business.ExternalInterfaceAvailableField displayedField in displayedFields)
			{
				grd.ColumnHeaders.Add(displayedField.DisplayName);
				displayWidths.Add(displayedField.DisplayWidth);
			}
			grd.ColumnWidths = string.Join(" ", displayWidths.ToArray());

			string boundColumns = _externalTerm.InterfaceConfig.DisplayedFields(" ");
			grd.BoundColumns = boundColumns;

			grd.DataKeyNames = new string[] { "Key" };
			grd.Width = Unit.Percentage(100.0);
			grd.Container = pnlGrid.ID;
			grd.HeaderContainer = pnlGridHeader.ID;
			grd.AutoGenerateColumns = false;
			grd.EnableClickEvent = false;
			grd.RowHighlighting = false;
			grd.EnableDoubleClickEvent = false;
			grd.EnableHeaderClick = false;
			grd.HeaderRowSize = 1;
			grd.ShowHeader = true;
			grd.Enabled = CanEdit;
			grd.TermName = _externalTerm.Name;
			grd.RowDataBound += new GridViewRowEventHandler(grd_RowDataBound);
		}


		protected void grd_RowDataBound(object sender, GridViewRowEventArgs e)
		{
			CheckBox chk = e.Row.Cells[grd.CheckboxColumn].Controls[0] as CheckBox;
			if (chk != null)
			{
				chk.Attributes["onclick"] += string.Format("EnableRemoveButton('{0}', '{1}');", btnRemove.ClientID, grd.SelectedRowsHiddenFieldID);
			}
		}


		protected void btnRemoveOnCommand(object sender, CommandEventArgs e)
		{
			//ensure that we remove the rows in reverse order, so that the index values do not get messed up.
			grd.SelectedRows.Reverse(); 
			foreach (int rowNumber in grd.SelectedRows)
				_externalTerm.SelectedItems.RemoveAt(rowNumber);
			grd.SelectedRows.Clear();
			BindGrid();
			SetPageDirty(true);
		}

		protected void btnClearOnCommand(object sender, CommandEventArgs e)
		{
			_externalTerm.SelectedItems.Clear();
			grd.SelectedRows.Clear();
			BindGrid();
			SetPageDirty(true);
		}


		private void RegisterPopupScript()
		{
			BaseSystemPage systemPage = this.Page as BaseSystemPage;
			BaseManagedItemPage managedItemPage = this.Page as BaseManagedItemPage;
			if (systemPage == null)
				throw new NullReferenceException("page is null because ExternalTermControl.Page does not derive from BaseSystemPage.");
			Type t = this.Page.GetType();

			string scriptName = "_kh_ExternalTermLookup";
			string jsFunctionName = string.Format("ExternalTermAdd_{0}", this.ClientID.Replace(' ', '_'));
			string hiddenFieldName = GetHiddenFieldName();
			System.IO.StringWriter sw1 = new System.IO.StringWriter();
			string dlgProps = "center=yes; help=no; resizable=yes; dialogHeight=500px; dialogWidth=720px;";
			sw1.WriteLine("function {0}() ", jsFunctionName);
			sw1.WriteLine("{ ");
			if (managedItemPage != null)
			{
				managedItemPage.IsChanged = false;
				sw1.WriteLine("  var retVal = window.showModalDialog('ExternalTermFilter.aspx?system={0}&item={1}&term={2}', '{3}', '{4}');", managedItemPage.ITATSystem.ID, managedItemPage.ManagedItem.ManagedItemID, Term.ID, string.Empty, dlgProps);
			}
			else
			{
				sw1.WriteLine("  var retVal = window.showModalDialog('ExternalTermFilter.aspx?system={0}&eic={1}', '{2}', '{3}');", systemPage.ITATSystem.ID, ((Business.ExternalTerm)Term).InterfaceConfig.Name, string.Empty, dlgProps);
			}
			sw1.WriteLine("  if (retVal && retVal.length > 0) ");
			sw1.WriteLine("  { ");
			sw1.WriteLine("	    document.getElementById('{0}').value = retVal; ", hiddenFieldName);
			sw1.WriteLine("	    document.forms(0).submit(); ");
			sw1.WriteLine("  } ");
			sw1.WriteLine("} ");
			if (!this.Page.ClientScript.IsClientScriptBlockRegistered(t, scriptName))
				this.Page.ClientScript.RegisterClientScriptBlock(t, scriptName, sw1.ToString(), true);

			btnAdd.OnClientClick = string.Format("{0}(); return false;", jsFunctionName);
		}

	}
}