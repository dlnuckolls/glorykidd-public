using System;
using System.Collections.Generic;

namespace Kindred.Knect.ITAT.Web
{
	public partial class RoleSelector : System.Web.UI.UserControl
	{

		private const string _KH_VS_SELECTEDROLES = "_kh_vs_SelectedRoles";
		private int[] _widths;

		#region Properties
		
		public Web.BaseSystemPage SystemPage
		{
			get { return (BaseSystemPage)this.Page; }
		}

		public string Caption
		{
			get { return litCaption.Text; }
			set { litCaption.Text = value; }
		}

		public Kindred.Knect.ITAT.Business.RoleType RoleType
		{
			get;
			set;
		}

		public List<Business.Role> SelectedRoles
		{
			get;
			private set;
		}

		public bool HasChanged
		{
			get;
			private set;
		}

		public string Widths
		{
			get
			{
				return GetWidths();
			}
			set
			{
				SetWidths(value);
			}
		}

		public int Rows
		{
			get;
			set;
		}

		#endregion

		#region event handlers

		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);
		}

		public void Initialize(List<Business.Role> roles)
		{
			SelectedRoles = roles;
			Helper.LoadRoles(lstRoles, SystemPage.ITATSystem, RoleType, SelectedRoles);
		}

		public bool Update()
		{
			return Helper.UpdateList(lstRoles, SelectedRoles);
		}

		protected override void OnPreRender(EventArgs e)
		{
			if (_widths.Length > 0)
				td1.Width = _widths[0].ToString() + "px";
			if (_widths.Length > 1)
				td2.Width = _widths[1].ToString() + "px";
			if (Rows == 0)
				lstRoles.Rows = 6;
			else
				lstRoles.Rows = Rows;
			base.OnPreRender(e);
			btnClear.OnClientClick = string.Format("_kh_deselectListItems(document.getElementById('{0}'))", lstRoles.ClientID);
		}

		#endregion


		protected override void LoadViewState(object savedState)
		{
			base.LoadViewState(savedState);
			SelectedRoles = (List<Business.Role>)ViewState[_KH_VS_SELECTEDROLES];
		}

		protected override object SaveViewState()
		{
			ViewState[_KH_VS_SELECTEDROLES] = SelectedRoles;
			return base.SaveViewState();
		}


		private void SetWidths(string value)
		{
			string[] values = value.Split(',');
			int widthNumber;
			_widths = new int[values.Length];
			for (int i = 0; i < values.Length; i++)
			{
				if (int.TryParse(values[i], out widthNumber))
					_widths[i] = widthNumber;
				else
					throw new ArgumentException("Widths must be a comma-delimited string containing integers.  The value was " + value);
			}
		}

		private string GetWidths()
		{
			System.Text.StringBuilder sb = new System.Text.StringBuilder();
			for (int i = 0; i < _widths.Length - 1; i++)
				sb.Append((int)_widths[i]).Append(",");
			sb.Append((int)_widths[_widths.Length - 1]);
			return sb.ToString();
		}


	}
}