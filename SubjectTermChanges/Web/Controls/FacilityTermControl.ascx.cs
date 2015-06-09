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
	public partial class FacilityTermControl : BaseProfileControl
	{

		#region private members
		private Business.FacilityTerm _facilityTerm;
		[NonSerialized]
		private Business.FacilityCollection _facilities;
		[NonSerialized]
		private List<Data.FacilityDataRow> _sortedFacilities;
		[NonSerialized]
		private Business.SecurityHelper _securityHelper;
		private const string _VS_GRIDSELECTEDROWS = "_kh_vs_grdSelectedRows";
		#endregion


		#region base class overrides



		public override Business.Term Term
		{
			get
			{
				return _facilityTerm;
			}
			set
			{
				_facilityTerm = value as Business.FacilityTerm;
				if (_facilityTerm == null)
					throw new NullReferenceException("Unable to cast Term as a FacilityTerm");
			}
		}

		public Business.FacilityCollection Facilities
		{
			get { return _facilities; }
		}


		public Business.SecurityHelper SecurityHelper
		{
			get { return _securityHelper; }
			set { _securityHelper = value; }
		}


		#endregion


		#region control lifecycle events

		protected override void OnInit(EventArgs e)
		{
			base.OnInit(e);
			RegisterChildControlEventHandlers();
			//If grd is shown then it must be created on every Page_load (not just when IsPostBack is false)
			if (CanEdit && (_facilityTerm.MultiSelect ?? false))
				InitializeGridControl();
		}

		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);
			//The facility data is not persisted in ViewState (it would be large!).  Instead it is re-created on each postback.
			_facilityTerm = (Business.FacilityTerm)this.Term;
			_facilities = GetFacilities();
			_sortedFacilities = _facilities.SortedList(_facilityTerm.SortField);

			if (IsPostBack)
			{
				if (CanEdit && (_facilityTerm.MultiSelect ?? false))
				{
					LoadMultiSelectGrid();
					SelectGridRows();
				}
			}
			else
			{
				SetInitialData();
			}
		}


		protected override void OnPreRender(EventArgs e)
		{
			base.OnPreRender(e);
			SetChildControlVisibility();
		}

		#endregion


		#region constituent control event handlers

		void ddl_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (_facilityTerm.MultiSelect ?? false)
				throw new ArgumentOutOfRangeException("MultiSelect", "A DropDownList should only be displayed if MultiSelect == false.");
			_facilityTerm.SelectedFacilityIDs.Clear();
			_facilityTerm.SelectedFacilityIDs.Add(int.Parse(ddl.SelectedValue));
		}

		#endregion


		#region private methods


		private void InitializeGridControl()
		{
			grd.CssClass = "MSG";
			grd.CheckboxColumn = 0;
			grd.BoundColumns = "FacilityId SapFacilityId FacilityName";
			grd.ColumnWidths = "0px 60px 100%"; 
			grd.Container = grdContainer.ID;
			grd.HeaderContainer = "";
			grd.AutoGenerateColumns = false;
			grd.EnableClickEvent = false;
			grd.RowHighlighting = false;
			grd.EnableDoubleClickEvent = false;
			grd.EnableHeaderClick = false;
			grd.ShowHeader = false;
			grd.Enabled = CanEdit;
			grd.TermName = _facilityTerm.Name;
		}

		private Business.FacilityCollection GetFacilities()
		{
			if (_facilityTerm.UseUserSecurity ?? false)
				if (_facilityTerm.IncludeChildren ?? false)
					return Business.FacilityCollection.FilteredFacilityList(_securityHelper.AllUserFacilities, _facilityTerm);
				else
					return Business.FacilityCollection.FilteredFacilityList(_securityHelper.UserFacilities, _facilityTerm);
			else
				return Business.FacilityCollection.FilteredFacilityList(Business.FacilityCollection.GetAll(), _facilityTerm);
		}

		private void SetInitialData()
		{
			if (CanEdit)
			{
				if (_facilityTerm.MultiSelect ?? false)  // multi-select: use MultiSelectGrid control
				{
					LoadMultiSelectGrid();
					SelectGridRows();
				}
				else   //single-select: use DropDownList
				{
					int facId = 0;
					if (_facilityTerm.SelectedFacilityIDs.Count == 1)
						facId = _facilityTerm.SelectedFacilityIDs[0];
					Helper.LoadListControl(ddl, _sortedFacilities, "SapIdPlusName", "FacilityId", facId.ToString(), true, "(Select a Facility)", "0");
					//ddl.Attributes["onchange"] = string.Format("ControlOnFocus('{0}','{1}');", EncodeName(_facilityTerm.Name), EncodeName(ddl.UniqueID));
				}
			}
			else  // read-only: use Label
			{
				lbl.Text = _facilityTerm.DisplayValue(Business.FacilityTerm.DefaultDisplayValueFormat); 
			}
		}


		private void LoadMultiSelectGrid()
		{
			//Load facility list
			//grd.ControlHasFocus = ControlHasFocus;
			grd.DataSource = _sortedFacilities;
			grd.DataBind();
		}


		private void SetChildControlVisibility()
		{
			if (CanEdit)
			{
				if (_facilityTerm.MultiSelect ?? false)  // Multi-Select:  use MultiSelectGrid control
				{
					grd.Visible = true;
					grdContainer.Visible = true;
					ddl.Visible = false;
					lbl.Visible = false;
					//Set grdContainer height
					grdContainer.Height = Unit.Pixel(4 + 20 * Math.Min(4, _facilities.Count));
					grdContainer.Width = Unit.Percentage(100.0);
				}
				else    // single-select: use DropDownList
				{
					grd.Visible = false;
					grdContainer.Visible = false;
					ddl.Visible = true;
					lbl.Visible = false;
				}
			}
			else   // read-only:  use Label
			{
				lbl.Visible = true;
				if (this.Parent is HtmlControl)
					((HtmlControl)this.Parent).Style["white-space"] = "normal";
				if (this.Parent is WebControl)
					((WebControl)this.Parent).Style["white-space"] = "normal";
				grd.Visible = false;
				grdContainer.Visible = false;
				ddl.Visible = false;
			}
		}


		private void SelectGridRows()
		{
			//Find and select SelectedFacilityIDs
			grd.SelectedRows.Clear();
			grd.DisabledRows.Clear();
			if (_facilityTerm.SelectedFacilityIDs.Count > 0)
			{
				for (int i = 0, j = _sortedFacilities.Count; i < j; i++)
				{
					int facID = _sortedFacilities[i].FacilityId;
					if (_facilityTerm.SelectedFacilityIDs.Contains(facID))
					{
						grd.SelectedRows.Add(i);
						if (_facilityTerm.OwningFacilityIDs.Contains(facID))
							grd.DisabledRows.Add(i);
					}
				}
			}
		}
		




		private void RegisterChildControlEventHandlers()
		{
//			ddl.SelectedIndexChanged += new EventHandler(ddl_SelectedIndexChanged);
		}


		#endregion


		public override void UpdateTermValue(string termGroupContainerName)
		{
			string prefix = (string.IsNullOrEmpty(termGroupContainerName) ? "" : termGroupContainerName + "$");
			if (CanEdit)
			{
				if (_facilityTerm.MultiSelect ?? false)  // Multi-Select:  use MultiSelectGrid control
				{
					string value = Request.Form[prefix + grd.SelectedRowsHiddenFieldID];
					string[] values = value.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);

					//  then convert selected Rows to selected FacilityIDs
					Business.FacilityCollection facilities = GetFacilities();
					List<Data.FacilityDataRow> sortedFacilities = facilities.SortedList(_facilityTerm.SortField);
					Data.FacilityDataRow[] facArray = new Kindred.Knect.ITAT.Data.FacilityDataRow[sortedFacilities.Count];
					sortedFacilities.CopyTo(facArray, 0);
					_facilityTerm.SelectedFacilityIDs.Clear();
					for (int i = 0; i < values.Length; i++)
						_facilityTerm.SelectedFacilityIDs.Add(facArray[int.Parse(values[i])].FacilityId);

				}
				else    // single-select: use DropDownList
				{
					string value = Request.Form[prefix + ddl.UniqueID];
					_facilityTerm.SelectedFacilityIDs.Clear();
					int facId;
					if (int.TryParse(value, out facId))
						_facilityTerm.SelectedFacilityIDs.Add(facId);
				}
			}
		}


		//public override bool TryUpdateTermValue(ref System.Collections.Generic.List<string> validationErrors)
		//{
		//   return true;
		//}


	}
}