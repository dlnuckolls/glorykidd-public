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
	public partial class MSOTermControl : BaseProfileControl
	{


		#region private members
		private Business.MSOTerm _msoTerm;
		#endregion


		#region base class overrides

		public override Business.Term Term
		{
			get
			{
				return _msoTerm;
			}
			set
			{
				_msoTerm = value as Business.MSOTerm;
				if (_msoTerm == null)
					throw new NullReferenceException("Unable to cast Term as an MSOTerm");
			}
		}


			#endregion


		#region control lifecycle events

		protected override void OnInit(EventArgs e)
		{
			base.OnInit(e);
			lstMso.ItemTemplate = new MSOEditTemplate(this.CanEdit);
		}

		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);
			_msoTerm = (Business.MSOTerm)this.Term;
		}


		protected override void OnPreRender(EventArgs e)
		{
			SetChildControlsValues();
			RegisterMSOLookupScript(this);
			SetChildControlVisibility();
			base.OnPreRender(e);
		}

		#endregion


		#region private methods


		private void SetChildControlVisibility()
		{
			btnLookup.Visible = CanEdit;
		}

		private void SetChildControlsValues()
		{
			//Compare "ctl_MSO$lstMso$ctl01$Address1Name" with values (such as) Business.XMLNames._A_MSOName 
			List<Business.MSOKeyValueItem> msoData = new List<Business.MSOKeyValueItem>(7);
			msoData.Add(new Business.MSOKeyValueItem(Business.XMLNames._A_MSOName, _msoTerm.MSOName, _msoTerm.MSOValue, _msoTerm.Name));
			msoData.Add(new Business.MSOKeyValueItem(Business.XMLNames._A_Address1Name, _msoTerm.Address1Name, _msoTerm.Address1Value, _msoTerm.Name));
			msoData.Add(new Business.MSOKeyValueItem(Business.XMLNames._A_Address2Name, _msoTerm.Address2Name, _msoTerm.Address2Value, _msoTerm.Name));
			msoData.Add(new Business.MSOKeyValueItem(Business.XMLNames._A_CityName, _msoTerm.CityName, _msoTerm.CityValue, _msoTerm.Name));
			msoData.Add(new Business.MSOKeyValueItem(Business.XMLNames._A_StateName, _msoTerm.StateName, _msoTerm.StateValue, _msoTerm.Name));
			msoData.Add(new Business.MSOKeyValueItem(Business.XMLNames._A_ZipName, _msoTerm.ZipName, _msoTerm.ZipValue, _msoTerm.Name));
			msoData.Add(new Business.MSOKeyValueItem(Business.XMLNames._A_PhoneName, _msoTerm.PhoneName, Utility.TextHelper.FormatAsPhone(_msoTerm.PhoneValue), _msoTerm.Name));

			lstMso.DataSource = msoData;
			lstMso.DataBind();
		}


		private void RegisterMSOLookupScript(Control c)
		{
			BaseManagedItemPage page = this.Page as BaseManagedItemPage;
			if (page == null)
				throw new NullReferenceException("page is null because the MSOTermControl.Page does not derive from BaseManagedItemPage.");
			Type t = page.GetType();
			Page.ClientScript.RegisterClientScriptInclude(t, "_kh_jsITAT", "Scripts/itat.js");

			string scriptName = "_kh_MSOLookup";
			string clientControlNameBase = c.ClientID;
			System.IO.StringWriter sw1 = new System.IO.StringWriter();
			string dlgProps = "center=yes; help=no; resizable=yes; dialogHeight=500px; dialogWidth=720px;";
			int counter = 0;
			sw1.WriteLine("	function MSOLookup()");
			sw1.WriteLine("	{");
			sw1.WriteLine("		var retVal = window.showModalDialog('DocumentDialog.aspx?action=lookup&interface=MSO&param={0}','{1}','{2}');", page.ManagedItem.OwningFacilityIDs[0], string.Empty, dlgProps);
			sw1.WriteLine("		if (retVal && retVal.length > 0) ");
			sw1.WriteLine("		{ ");
			sw1.WriteLine("			var values = retVal.split('|'); ");
			sw1.WriteLine("			FillDataControl('{0}_ctl{1:D2}_{2}', values[0]); ", lstMso.ClientID, counter++, "MSOName");
			sw1.WriteLine("			FillDataControl('{0}_ctl{1:D2}_{2}', values[1]); ", lstMso.ClientID, counter++, "Address1Name");
			sw1.WriteLine("			FillDataControl('{0}_ctl{1:D2}_{2}', values[2]); ", lstMso.ClientID, counter++, "Address2Name");
			sw1.WriteLine("			FillDataControl('{0}_ctl{1:D2}_{2}', values[3]); ", lstMso.ClientID, counter++, "CityName");
			sw1.WriteLine("			FillDataControl('{0}_ctl{1:D2}_{2}', values[4]); ", lstMso.ClientID, counter++, "StateName");
			sw1.WriteLine("			FillDataControl('{0}_ctl{1:D2}_{2}', values[5]); ", lstMso.ClientID, counter++, "ZipName");
			sw1.WriteLine("			FillDataControl('{0}_ctl{1:D2}_{2}', values[6]); ", lstMso.ClientID, counter++, "PhoneName");
			sw1.WriteLine("		} ");
			sw1.WriteLine("		return false;");
			sw1.WriteLine("	}");
			if (!page.ClientScript.IsClientScriptBlockRegistered(t, scriptName))
				page.ClientScript.RegisterClientScriptBlock(t, scriptName, sw1.ToString(), true);
		}


		#endregion


		public override void UpdateTermValue(string termGroupContainerName)
		{
			string prefix = (string.IsNullOrEmpty(termGroupContainerName) ? "" : termGroupContainerName + "$");
			int counter = 0;
			_msoTerm.MSOValue = Page.Request.Form[string.Format("{3}{0}$ctl{1:D2}${2}", lstMso.UniqueID, counter++, "MSOName", prefix)];
			_msoTerm.Address1Value = Page.Request.Form[string.Format("{3}{0}$ctl{1:D2}${2}", lstMso.UniqueID, counter++, "Address1Name", prefix)];
			_msoTerm.Address2Value = Page.Request.Form[string.Format("{3}{0}$ctl{1:D2}${2}", lstMso.UniqueID, counter++, "Address2Name", prefix)];
			_msoTerm.CityValue = Page.Request.Form[string.Format("{3}{0}$ctl{1:D2}${2}", lstMso.UniqueID, counter++, "CityName", prefix)];
			_msoTerm.StateValue = Page.Request.Form[string.Format("{3}{0}$ctl{1:D2}${2}", lstMso.UniqueID, counter++, "StateName", prefix)];
			_msoTerm.ZipValue = Page.Request.Form[string.Format("{3}{0}$ctl{1:D2}${2}", lstMso.UniqueID, counter++, "ZipName", prefix)];
			_msoTerm.PhoneValue = Page.Request.Form[string.Format("{3}{0}$ctl{1:D2}${2}", lstMso.UniqueID, counter++, "PhoneName", prefix)];
		}


		//public override bool TryUpdateTermValue(ref System.Collections.Generic.List<string> validationErrors)
		//{
		//   return true;
		//}

	}
}