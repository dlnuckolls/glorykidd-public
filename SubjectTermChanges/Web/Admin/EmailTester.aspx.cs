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
	public partial class EmailTester : BaseSystemPage
	{


		protected void Page_Load(object sender, EventArgs e)
		{
			_securityHelper = new Kindred.Knect.ITAT.Business.SecurityHelper(_itatSystem);
			if (!IsPostBack)
				LoadLists();
		}


		private void LoadLists()
		{
			Helper.LoadRoles(lstRoles, _itatSystem, Business.RoleType.Security | Business.RoleType.Distribution, new List<Business.Role>());
			Business.FacilityCollection userFacilities = _securityHelper.AllUserFacilities;
			Business.FacilityTerm facilityTerm = _itatSystem.PrimaryFacility;
			Business.FacilityCollection facilities = Business.FacilityCollection.FilteredFacilityList(userFacilities, facilityTerm);
			List<Data.FacilityDataRow> sortedFacilities = facilities.SortedList(_itatSystem.PrimaryFacility.SortField);
			Helper.LoadListControl(ddlFacilities, sortedFacilities, "SapIdPlusName", "FacilityId", "0", true, "(None)", "0");
			
			
			//List<Data.FacilityDataRow> sortedFacilities = facilities.SortedList(_itatSystem.PrimaryFacility.SortField);
			//Helper.LoadListControl(ddlFacilities, sortedFacilities, "SapIdPlusName", "FacilityId", "0");
		}


		protected void btnSubmit_Command(object sender, CommandEventArgs e)
		{
			List<string> roleNames = new List<string>();
			foreach (ListItem item in lstRoles.Items)
				if (item.Selected)
					roleNames.Add(item.Value);

			List<int> owningFacilityIDs = new List<int>();
			foreach (ListItem item in ddlFacilities.Items)
				if ((item.Selected) && (item.Value != "0"))
					owningFacilityIDs.Add(int.Parse(item.Value));

			//this code is copied from the Business.Message.Send() method
			List<int> allRecipientFacilityIDs = new List<int>();
			foreach (string recipient in roleNames)
			{
				List<int> absoluteLevels = null;
				List<int> relativeLevels = null;
				_itatSystem.GetFacilityLevels(recipient, ref absoluteLevels, ref relativeLevels);
				List<int> recipientFacilityIDs =  Business.FacilityCollection.FacilityAncestry(owningFacilityIDs, absoluteLevels, relativeLevels);
				foreach (int facilityID in recipientFacilityIDs)
					if (!allRecipientFacilityIDs.Contains(facilityID))
						allRecipientFacilityIDs.Add(facilityID);
			}
			Kindred.Common.Security.NameEmailPair[] nameEmailPairs = null;
			if (ddlFacilities.SelectedIndex > -1)  
				nameEmailPairs = Business.SecurityHelper.GetEmailRecipients(_itatSystem, roleNames, allRecipientFacilityIDs);
			else
				nameEmailPairs = Business.SecurityHelper.GetEmailRecipients(_itatSystem, roleNames);
			
			if ((string)e.CommandArgument == "true")
			{
				//Generate and send e-mail
				litResults.Text = string.Empty;
				Business.EmailHelper.SendEmail(Business.XMLNames._M_EmailFrom, txtSubject.Text, txtBody.Text, nameEmailPairs, _itatSystem, null, null);
			}
			else
			{
				//Just show the list of recipients
				System.Text.StringBuilder sb = new System.Text.StringBuilder();
				if (nameEmailPairs.Length > 0)
				{
					for (int i = 0, j = nameEmailPairs.Length; i < j; i++)
					{
						Kindred.Common.Security.NameEmailPair nameEmailPair = nameEmailPairs[i];
						sb.AppendFormat(" - <b>{0}</b> ({1})<br />", nameEmailPair.Name, nameEmailPair.Email);
					}
					litResults.Text = sb.ToString();
				}
				else
					litResults.Text = "(no recipients)";
			}
		}



		internal override HtmlGenericControl HTMLBody()
		{
			return body;
		}

		internal override Control ResizablePanel()
		{
			return pnlResults;
		}
	}
}
