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
using Kindred.Knect.ITAT.Business;
using System.Collections.Generic;

namespace Kindred.Knect.ITAT.Web
{
	public partial class LinkTermControl : BaseProfileControl
	{
		#region private members
		private Business.LinkTerm _linkTerm;
		#endregion

		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);
			if (!IsPostBack)
				SetChildControlsInitialValues();
		}

		protected override void OnPreRender(EventArgs e)
		{
			if (_linkTerm.LinkSource == XMLNames._AV_ComplexList)
			{
				lnk.Enabled = CanSeeComplexList(_linkTerm.ComplexListID);
			}
			else
			{
				lnk.Enabled = CanEdit;
			}
			base.OnPreRender(e);
		}


		private bool CanSeeComplexList(Guid complexListTermId)
		{
			BaseManagedItemPage managedItemPage = this.Page as BaseManagedItemPage;
			Dictionary<Guid /*TermGroupID*/, bool /*CanEdit*/> canEditTermGroup = managedItemPage.BuildEditableTermGroups();
			Dictionary<Guid /*TermGroupID*/, bool /*CanView*/> canViewTermGroup = managedItemPage.BuildViewableTermGroups();

			if (canEditTermGroup[managedItemPage.ManagedItem.FindTerm(complexListTermId).TermGroupID])
			{
				return true;
			}
			else
			{
				if (canViewTermGroup[managedItemPage.ManagedItem.FindTerm(complexListTermId).TermGroupID])
				{
					return !(managedItemPage.ITATSystem.ViewersCannotSeeComplexLists);
				}
				else
				{
					return false;
				}
			}
		}


		private void SetChildControlsInitialValues()
		{
			lnk.Text = _linkTerm.Caption;
			string sComplexListName = "";
			if (Business.Term.ValidID(_linkTerm.ComplexListID))
			{
				sComplexListName = (this.Page as BaseManagedItemPage).ManagedItem.FindTerm(_linkTerm.ComplexListID).Name;
				//lnk.NavigateUrl = sComplexListName;
			}
			switch (_linkTerm.LinkSource)
			{
				case XMLNames._AV_URL:
					lnk.NavigateUrl = _linkTerm.URL;
					lnk.Target = "_blank";
					break;
				case XMLNames._AV_ComplexList:
					BaseManagedItemPage pageItem = (this.Page as BaseManagedItemPage);
					lnk.NavigateUrl = "~/ManagedItemComplexList.aspx?" + Common.Names._QS_ITAT_SYSTEM_ID + "=" + pageItem.ITATSystem.ID.ToString() + "&" + Common.Names._QS_MANAGED_ITEM_ID + "=" + pageItem.ManagedItem.ManagedItemID.ToString() + "&list=" + sComplexListName;
					lnk.Target = "";
					break;
				case XMLNames._AV_ReferenceManagedItem:
					//This shouldn't happen since this type of link should only appear in notifications, not on the profile screen.
					break;
				default:
					break;
			}
		}


		#region base class overrides

		public override Business.Term Term
		{
			get
			{
				return _linkTerm;
			}
			set
			{
				_linkTerm = value as Business.LinkTerm;
				if (_linkTerm == null)
					throw new NullReferenceException("Unable to cast Term as a LinkTerm");
			}
		}

		public override void UpdateTermValue(string termGroupContainerName)
		{
			//This method intentionally left blank.
		}



		//public override bool TryUpdateTermValue(ref System.Collections.Generic.List<string> validationErrors)
		//{
		//   return true;
		//}


		#endregion

	}
}