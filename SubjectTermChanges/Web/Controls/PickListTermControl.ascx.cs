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
	public partial class PickListTermControl : BaseProfileControl
	{

		#region private members
		private Business.PickListTerm _pickListTerm;
		private const string _VS_GRIDSELECTEDROWS = "_kh_vs_grdSelectedRows";
		private const string _VS_CANEDIT = "_kh_vs_CanEdit";
		private bool _previousCanEditValue = false;
		#endregion

		#region base class overrides

		private void SetChildControlValues()
		{
			if (CanEdit)
			{
				if (_pickListTerm.PickListItems.Count == 1)    //1 picklist item: single check box
				{
					chk.Text = _pickListTerm.PickListItems[0].Value;
					chk.Checked = (_pickListTerm.PickListItems[0].Selected ?? false);
					chk.Enabled = CanEdit;
				}
				else
				{
					if (_pickListTerm.MultiSelect ?? false)  // multi-select picklist: MultiSelectGrid control
					{
						InitializeGridControl();
						SetGridInitialData();
					}
					else
					{
						if (_pickListTerm.PickListItems.Count == 2)   //2 item single-select picklist: RadioButtonList
						{
							Helper.LoadListControl(rdolst, _pickListTerm.PickListItems, "Value", "Value", "");
							for (int i = 0; i < _pickListTerm.PickListItems.Count; i++)
								if (_pickListTerm.PickListItems[i].Selected ?? false)
								{
									rdolst.SelectedIndex = i;
									break;
								}
							rdolst.Enabled = CanEdit;
						}
						else      // 3 or more item single-select list: DropDrownList
						{
							string selectedValue = "";
							for (int i = 0; i < _pickListTerm.PickListItems.Count; i++)
								if (_pickListTerm.PickListItems[i].Selected ?? false)
								{
									selectedValue = _pickListTerm.PickListItems[i].Value;
									break;
								}
							Helper.LoadListControl(ddl, _pickListTerm.PickListItems, "Value", "Value", selectedValue, true, "(Please Select One)", "");
							ddl.Enabled = CanEdit;
						}
					}
				}
			}
		}


		private void SetChildControlVisibility()
		{
			if (CanEdit)
			{
				if (_pickListTerm.PickListItems.Count == 1)    //1 picklist item: single check box
				{
					chk.Visible = true;
					grd.Visible = false;
					grdContainer.Visible = false;
					rdolst.Visible = false;
					ddl.Visible = false;
					lbl.Visible = false;
				}
				else
				{
					if (_pickListTerm.MultiSelect ?? false)  // multi-select picklist: MultiSelectGrid control
					{
						chk.Visible = false;
						grd.Visible = true;
						grdContainer.Visible = true;
						rdolst.Visible = false;
						ddl.Visible = false;
						lbl.Visible = false;
					}
					else
					{
						if (_pickListTerm.PickListItems.Count == 2)   //2 item single-select picklist: RadioButtonList
						{
							chk.Visible = false;
							grd.Visible = false;
							grdContainer.Visible = false;
							rdolst.Visible = true;
							ddl.Visible = false;
							lbl.Visible = false;
						}
						else      // 3 or more item single-select list: DropDrownList
						{
							chk.Visible = false;
							grd.Visible = false;
							grdContainer.Visible = false;
							rdolst.Visible = false;
							ddl.Visible = true;
							lbl.Visible = false;
						}
					}
				}
			}
			else
			{
				lbl.Visible = true;
				grd.Visible = false;
				grdContainer.Visible = false;
				chk.Visible = false;
				rdolst.Visible = false;
				ddl.Visible = false;
			}
		}


		public override Business.Term Term
		{
			get
			{
				return _pickListTerm;
			}
			set
			{
				_pickListTerm = value as Business.PickListTerm;
				if (_pickListTerm == null)
					throw new NullReferenceException("Unable to cast Term as a PickListTerm");
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
			_pickListTerm = (Business.PickListTerm)this.Term;
			if (IsPostBack)
			{
				//If grd is shown, it is not persisted in ViewState, so the postback data is read from the page's hidden fields, and the data in the grid must be re-created
				if ((CanEdit) && (_pickListTerm.PickListItems.Count > 1) && (_pickListTerm.MultiSelect ?? false))  // multi-select picklist: MultiSelectGrid control
				{
					InitializeGridControl();
				}
			}
		}


		protected override void OnPreRender(EventArgs e)
		{
			if (_pickListTerm.Runtime.SetValue != null)
			{
				// note that this assumes that SetValue can only contain a single value
				foreach (Business.PickListItem item in _pickListTerm.PickListItems)
					item.Selected = (item.Value == _pickListTerm.Runtime.SetValue);  
			}
			if (ShouldSetControlValues())
				SetChildControlValues();
			base.OnPreRender(e);
			lbl.Text = _pickListTerm.DisplayValue(Business.XMLNames._TPS_None);
			SetChildControlVisibility();
		}


		/// <summary>
		///		Determine whether or not to recalculate the contents of the control only in certain cases.
		///		Recalculate (i.e., rely on ViewState) only when
		//		- first time in (IsPostBack == false)
		//		- the editability of the term has changed on the postback
		//		- the value of the term is being set because of a Term Dependency
		/// </summary>
		private bool ShouldSetControlValues()
		{
			if (!IsPostBack)
				return true;
			if (_previousCanEditValue != CanEdit)
				return true;
			if (_pickListTerm.Runtime.SetValue != null)
				return true;
			//if ((CanEdit) && (_pickListTerm.PickListItems.Count > 1) && (_pickListTerm.MultiSelect ?? false))  // multi-select picklist: MultiSelectGrid control
			//	return true;
			return false;
		}

		protected override object SaveViewState()
		{
			ViewState[_VS_CANEDIT] = CanEdit;
			return base.SaveViewState();
		}

		protected override void LoadViewState(object savedState)
		{
			base.LoadViewState(savedState);
			if (ViewState[_VS_CANEDIT] != null)
				_previousCanEditValue = (bool)ViewState[_VS_CANEDIT];
			else
				_previousCanEditValue = false;
		}

		#endregion



		#region private methods


		private void InitializeGridControl()
		{
			grdContainer.Height = Unit.Pixel(4 + 20 * Math.Min(4, _pickListTerm.PickListItems.Count));
			grdContainer.Width = Unit.Percentage(100.0); 
			                                                                              
			grd.CheckboxColumn = 0;
			grd.DataSource = _pickListTerm.PickListItems;
			grd.BoundColumns = "Value";
			grd.Container = grdContainer.ID;
			grd.HeaderContainer = null;
			grd.AutoGenerateColumns = false;
			grd.RowHighlighting = false;
			grd.EnableClickEvent = false;
			grd.EnableDoubleClickEvent = false;
			grd.EnableHeaderClick = false;
			grd.ShowHeader = false;
			grd.ColumnWidths = "100%";
			grd.TermName = _pickListTerm.Name;
			//grd.ControlHasFocus = ControlHasFocus;
			grd.DataBind();
			grd.Enabled = CanEdit;
		}

		private void SetGridInitialData()
		{
			for (int i = 0; i < _pickListTerm.PickListItems.Count; i++)
				if (_pickListTerm.PickListItems[i].Selected ?? false)
					grd.SelectedRows.Add(i);
		}


		#endregion


		public override void UpdateTermValue(string termGroupContainerName)
		{
			string prefix = (string.IsNullOrEmpty(termGroupContainerName) ? "" : termGroupContainerName + "$");
			if (_pickListTerm.Runtime.Enabled)
			{
				_pickListTerm.DeselectAll();

				if (_pickListTerm.PickListItems.Count == 1)    //1 picklist item: single check box
				{
					string value = Request.Form[prefix + chk.UniqueID];
					_pickListTerm.PickListItems[0].Selected = !string.IsNullOrEmpty(value);
				}
				else
				{
					if (_pickListTerm.MultiSelect ?? false)  // multi-select picklist: MultiSelectGrid control
					{
						string value = Request.Form[prefix + grd.SelectedRowsHiddenFieldID];
						string[] values = value.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
						for (int i = 0, j = _pickListTerm.PickListItems.Count; i < j; i++)
							_pickListTerm.PickListItems[i].Selected = false;
						for (int i = 0, j = values.Length; i < j; i++)
							_pickListTerm.PickListItems[int.Parse(values[i])].Selected = true;
					}
					else
					{
						if (_pickListTerm.PickListItems.Count == 2)   //2 item single-select picklist: RadioButtonList
						{
							for (int i = 0, j = _pickListTerm.PickListItems.Count; i < j; i++)
								_pickListTerm.PickListItems[i].Selected = false;
							string value = Request.Form[prefix + rdolst.UniqueID];
							Business.PickListItem item = _pickListTerm.FindItem(value);
							if (item != null)
								item.Selected = true;
						}
						else      // 3 or more item single-select list: DropDrownList
						{
							for (int i = 0, j = _pickListTerm.PickListItems.Count; i < j; i++)
								_pickListTerm.PickListItems[i].Selected = false;
							string value = Request.Form[prefix + ddl.UniqueID];
							Business.PickListItem item = _pickListTerm.FindItem(value);
							if (item != null)
								item.Selected = true;
						}
					}
				}
			}
		}



	}
}