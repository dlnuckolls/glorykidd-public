using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Collections.Generic;

namespace Kindred.Knect.ITAT.Web
{
	public class BaseManagedItemComplexListPage : BaseManagedItemPage
	{
		#region  base class overrides

		internal override Control ResizablePanel()
		{
			return null;
		}

		internal override HtmlGenericControl HTMLBody()
		{
			return null;
		}

		protected override ManagedItemHeader HeaderControl()
		{
			return null;
		}

		#endregion

		protected override void GetManagedItem()
		{
			if ((Context.Items[Common.Names._CNTXT_ManagedItem] == null))
			{
				string qsValue = Request.QueryString[Common.Names._QS_MANAGED_ITEM_ID];
				bool loadComplexLists = true;
				Guid managedItemId;
				try
				{
					managedItemId = new Guid(qsValue);
				}
				catch
				{
					throw new Exception(String.Format("The \"{0}\" query string value \"{1}\" is not a valid GUID.", Common.Names._QS_MANAGED_ITEM_ID, qsValue));
				}
				try
				{
					_managedItem = Business.ManagedItem.Get(managedItemId, loadComplexLists);
				}
				catch (Exception e)
				{
					throw new Exception(String.Format("Error Loading {0} ID: {1}.  Error Message: {2}", _itatSystem.ManagedItemName, managedItemId, e.Message));
				}
			}
			else
			{
				_managedItem = (Business.ManagedItem)Context.Items[Common.Names._CNTXT_ManagedItem];
				//IsChanged related change
				if (Context.Items[Common.Names._CNTXT_IsChanged] != null)
					IsChanged = (bool)Context.Items[Common.Names._CNTXT_IsChanged];
			}
		}

		protected Business.ComplexList GetComplexList(ref int nIndex, bool requireIndex)
		{
			if (_managedItem.ComplexLists == null)
				return null;

			string listName = Request.QueryString[Common.Names._QS_COMPLEXLIST_NAME];
			if (string.IsNullOrEmpty(listName))
				return null;

			if (requireIndex)
			{
				string listIndex = Request.QueryString[Common.Names._QS_COMPLEXLIST_INDEX];
				if (!int.TryParse(listIndex, out nIndex))
					return null;
			}

			Business.ComplexList complexList = null;
			foreach (Business.ComplexList eachComplexList in _managedItem.ComplexLists)
			{
				if (eachComplexList.Name == listName)
				{
					complexList = eachComplexList;
					break;
				}
			}
			return complexList;
		}

		protected Business.StateTermGroup GetStateTermGroup(Business.ComplexList complexList)
		{
			if (complexList == null)
			{
				return null;
			}
			return _managedItem.State.StateTermGroups.Find(stg => stg.TermGroupID == complexList.TermGroupID);
		}
	}
}
