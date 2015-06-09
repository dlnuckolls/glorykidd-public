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

namespace Kindred.Knect.ITAT.Web
{
	public partial class DocumentDialog : BasePage
	{
		protected void Page_Load(object sender, EventArgs e)
		{
            string showDefaultDocument = default(string);
			Business.ITATSystem itatSystem = null;

			string systemId = Request.QueryString[Common.Names._QS_ITAT_SYSTEM_ID];
			if (!string.IsNullOrEmpty(systemId))
				itatSystem = Business.ITATSystem.Get(new Guid(systemId));

			string action = Request.QueryString[Common.Names._QS_DOC_DLG_ACTION];

            if (Request.QueryString[Common.Names._QS_SHOW_DEFAULT_DOCUMENT] != null)
            {
                showDefaultDocument = Request.QueryString[Common.Names._QS_SHOW_DEFAULT_DOCUMENT];
            }


			switch (action)
			{
				case Common.Names._QS_DOC_DLG_ACTION_VIEW:
					{
						string managedItemId = Request.QueryString[Common.Names._QS_MANAGED_ITEM_ID];
                        string ITATDocumentID = Request.QueryString[Common.Names._QS_ITAT_DOCUMENT_ID];
						ifr.Attributes["src"] = "ShowDoc.ashx" + Utility.TextHelper.QueryString(true, "type", "manageditemview", Common.Names._QS_MANAGED_ITEM_ID, managedItemId, Common.Names._QS_MANAGED_ITEM_NAME, itatSystem.ManagedItemName,Common.Names._QS_ITAT_DOCUMENT_ID, ITATDocumentID);
						break;
					}
				
				case Common.Names._QS_DOC_DLG_ACTION_SUMMARY:
					{
						string managedItemId = Request.QueryString[Common.Names._QS_MANAGED_ITEM_ID];
						ifr.Attributes["src"] = "ShowDoc.ashx" + Utility.TextHelper.QueryString(true, "type", "summary", Common.Names._QS_MANAGED_ITEM_ID, managedItemId, Common.Names._QS_MANAGED_ITEM_NAME, itatSystem.ManagedItemName);
						break;
					}
				
				case Common.Names._QS_DOC_DLG_ACTION_PREVIEW:
					{
						string managedItemId = Request.QueryString[Common.Names._QS_MANAGED_ITEM_ID];
                        string ITATDocumentID = Request.QueryString[Common.Names._QS_ITAT_DOCUMENT_ID];
                        
						string queryString;
						if (!string.IsNullOrEmpty(managedItemId))
                            queryString = Utility.TextHelper.QueryString(true, "type", "preview", Common.Names._QS_MANAGED_ITEM_ID, managedItemId, Common.Names._QS_ITAT_SYSTEM_ID, systemId, Common.Names._QS_ITAT_DOCUMENT_ID, ITATDocumentID, Common.Names._QS_SHOW_DEFAULT_DOCUMENT, showDefaultDocument);
						else
						{
							string templateId = Request.QueryString[Common.Names._QS_TEMPLATE_ID];
                            queryString = Utility.TextHelper.QueryString(true, "type", "preview", Common.Names._QS_TEMPLATE_ID, templateId, Common.Names._QS_ITAT_SYSTEM_ID, systemId, Common.Names._QS_ITAT_DOCUMENT_ID, ITATDocumentID, Common.Names._QS_SHOW_DEFAULT_DOCUMENT, showDefaultDocument);
						}
						ifr.Attributes["src"] = "ShowDoc.ashx" + queryString;
						break;
					}

				case Common.Names._QS_DOC_DLG_ACTION_LOOKUP:
					{
						ifr.Attributes["src"] = string.Format("InterfaceLookupDialog.aspx?interface={0}&param={1}", Request.QueryString["interface"], Request.QueryString["param"]); // +Utility.TextHelper.QueryString(true, Common.Names._QS_TEMPLATE_ID, templateId, Common.Names._QS_MANAGED_ITEM_NAME, itemname);
						break;
					}
				default:
					throw new Exception("Unknown Action Requested: " + action);
			}
		}




		internal override HtmlGenericControl HTMLBody()
		{
			return htmlBody;
		}

		internal override Control ResizablePanel()
		{
			return null;
		}
	}
}
