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

	public partial class ManagedItemHeader : BaseHeader
	{
		public bool HideMenu { get; set; }
		public bool HideButtons { get; set; }
        public string[] Roles { get; set; }

		public BaseManagedItemPage ManagedItemPage
		{
			get 
			{
				BaseManagedItemPage p = this.Page as BaseManagedItemPage;
				if (p == null)
					throw new InvalidCastException(String.Format("{0} is not an instance of, or derived from , BaseItemPage", this.Page.Request.Url));
				else
					return p;
			}
		}

		public Label SubTitle
		{
			get
			{
				return lblSubTitle;
			}
		}

        #region event handlers
		protected override void OnInit(EventArgs e)
		{
			base.OnInit(e);
		}

		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);
			SetButtons();
			SetMenu();
			RegisterSetBanner();
		}

		protected override void OnPreRender(EventArgs e)
		{
			base.OnPreRender(e);
			SetBanner();
			SetSubTitle();
		}

		private void RegisterSetBanner()
		{
			Type t = ManagedItemPage.GetType();
			string scriptName = "_kh_RegisterSetBanner";
			System.IO.StringWriter sw = new System.IO.StringWriter();
			sw.WriteLine("function SetBanner(text) { ");
			sw.WriteLine("  var o=document.getElementById('{0}'); ", lblBanner.ClientID);
			sw.WriteLine("  if (o) { ");
			sw.WriteLine("    o.innerText = '{0} - ' + text; ", this.ManagedItemPage.ITATSystem.ManagedItemName);
			sw.WriteLine("  } ");
			sw.WriteLine("}");
			if (!ManagedItemPage.ClientScript.IsClientScriptBlockRegistered(t, scriptName))
				ManagedItemPage.ClientScript.RegisterClientScriptBlock(ManagedItemPage.GetType(), scriptName, sw.ToString(), true);
		}



		#endregion


		private void SetBanner()
		{
			string managedItemName = ManagedItemPage.ITATSystem.ManagedItemName;
			string banner = string.Empty;

			if (ManagedItemPage is ManagedItemProfile)
				banner = string.Format("{0} - {1}", managedItemName, ManagedItemPage.ActiveTermGroup.Name);
			else if (ManagedItemPage is ManagedItemComplexList)
				banner = ManagedItemPage.Banner;
			else if (ManagedItemPage is ManagedItemComplexListEdit)
				banner = ManagedItemPage.Banner;
			else if (ManagedItemPage is ManagedItemHistory)
				banner = managedItemName + " - History";
			else
				banner = managedItemName;

			lblBanner.Text = banner;
		}

		private void SetSubTitle()
		{
			lblSubTitle.Text = ManagedItemPage.ManagedItem.ItemNumber;
            if (Roles == null || Roles.Length == 0)
            {
                lblRoles.Text = string.Format("You are logged in as: {0}, No Roles Defined", Business.SecurityHelper.LoggedOnUser);
            }
            else
            {
                if (Roles.Length > 1)
                {
                    List<string> roles = new List<string>(Roles);
                    roles.Sort();
                    lblRoles.Text = string.Format("You are logged in as: {0}, {1}", Business.SecurityHelper.LoggedOnUser, string.Join(", ", roles.ToArray()));
                }
                else
                {
                    lblRoles.Text = string.Format("You are logged in as: {0}, {1}", Business.SecurityHelper.LoggedOnUser, Roles[0]);
                }
            }

			SetModifiedSubTitle(ManagedItemPage.IsChanged);
		}

		public void SetModifiedSubTitle(bool modified)
		{
			if (modified)
			{
				if (lblSubTitle.Text.IndexOf('*') == -1)
					lblSubTitle.Text += '*';
			}
			else
			{
				lblSubTitle.Text = lblSubTitle.Text.Replace("*", "");
			}
		}

		private void SetButtons()
		{
			if (HideButtons)
			{
				return;
			}

			ITATButton btn = null;

			btn = new ITATButton("Save", button_Command, 130);
			btn.OnClientClick = "var o=document.getElementById('hfDoValidation'); if (o) o.value='1';";
			if (ManagedItemPage is ManagedItemComplexListEdit)
				btn.Enabled = false;
			if (ManagedItemPage is ManagedItemProfile)
				Helper.AddAttribute(btn.Attributes, "onfocus", "ControlOnFocus();");
			buttons.Controls.Add(btn);

			btn = new ITATButton("Reset", button_Command, 130);
			if (ManagedItemPage is ManagedItemComplexListEdit)
				btn.Enabled = false;
			if (ManagedItemPage is ManagedItemProfile)
				Helper.AddAttribute(btn.Attributes, "onfocus", "ControlOnFocus();");

			buttons.Controls.Add(btn);
		}


		private void RemoveButton(string text)
		{
			Control btn = buttons.FindControl(ITATButton.GetID(text));
			if (btn != null)
				buttons.Controls.Remove(btn);
		}

		private void RemoveMenuGroup(string menuGroupCaption)
		{
			ITATMenuGroup menuGroup = (ITATMenuGroup)menu.FindControl(ITATMenuGroup.GetID(menuGroupCaption));
			if (menuGroup != null)
				menu.Controls.Remove(menuGroup);
		}

		public void SetMenuItemVisibility(string menuGroupCaption, string menuItemCaption, bool bVisible)
		{
			ITATMenuGroup menuGroup = (ITATMenuGroup)menu.FindControl(ITATMenuGroup.GetID(menuGroupCaption));
			if (menuGroup != null)
			{
				string menuItemID = ITATMenuItem.GetID(menuGroupCaption, menuItemCaption);
				Control menuItem = menuGroup.FindControl(menuItemID);
				if (menuItem != null)
				{
					menuItem.Visible = bVisible;
				}
			}
		}

		private void button_Command(object sender, CommandEventArgs e)
		{
			OnHeaderEvent(new HeaderEventArgs(e.CommandName, (string)e.CommandArgument));
		}


		private void SetMenu()
		{
			if (HideMenu)
			{
				return;
			}

			ITATMenuGroup menuGroup1, menuGroup2;
			Type currentPageType = this.Page.GetType();
			string itemName = ManagedItemPage.ITATSystem.ManagedItemName;
			string systemID = ManagedItemPage.ITATSystem.ID.ToString();

			string urlQueryString = Utility.TextHelper.QueryString(
				true,
				Common.Names._QS_ITAT_SYSTEM_ID,
				this.ManagedItemPage.ITATSystem.ID.ToString(),
				Common.Names._QS_MANAGED_ITEM_ID,
				this.ManagedItemPage.ManagedItem.ManagedItemID.ToString()
				);

			//*** IMPORTANT: *** 
			//    Since the menu groups have a style of "float:right", 
			//    they are listed here in order from RIGHT TO LEFT
			//******************
			menuGroup1 = new ITATMenuGroup(menu, "View/Print", "", 130, 130, true);


			bool bDocumentPrinter = Utility.ListHelper.HaveAMatch(ManagedItemPage.ManagedItem.DocumentPrinters.ConvertAll<string>(Role.StringConverter), ManagedItemPage.SecurityHelper.UserRoles);
			bool bAdminViewer = Utility.ListHelper.HaveAMatch(ManagedItemPage.ITATSystem.AllowedRoles(Business.XMLNames._AF_AdminViewer), ManagedItemPage.SecurityHelper.UserRoles);
            bool bUserDocumentPrinter = Utility.ListHelper.HaveAMatch(ManagedItemPage.ManagedItem.UserDocumentPrinters.ConvertAll<string>(Role.StringConverter), ManagedItemPage.SecurityHelper.UserRoles);
            ITATMenuItem documentLink = null;

            //System Document
			if (ManagedItemPage.ManagedItem.CanGenerateDocument && (bDocumentPrinter || bAdminViewer))
			{                
                ITATDocument defaultDocument = ManagedItemPage.ManagedItem.GetDefaultITATDocument();
                if (defaultDocument != null)
                {
                    string urlViewPrintManagedItem = "~/DocumentDialog.aspx" + Utility.TextHelper.QueryString(true, Common.Names._QS_DOC_DLG_ACTION, Common.Names._QS_DOC_DLG_ACTION_VIEW, Common.Names._QS_MANAGED_ITEM_ID, this.ManagedItemPage.ManagedItem.ManagedItemID.ToString(), Common.Names._QS_ITAT_SYSTEM_ID, systemID, Common.Names._QS_ITAT_DOCUMENT_ID, defaultDocument.ITATDocumentID.ToString());
                    documentLink = new ITATMenuItem(menuGroup1.Caption, itemName, urlViewPrintManagedItem, this.menuLink_Command, this.Page == null, true);
                    menuGroup1.Controls.Add(documentLink);
                }
			}



            //User Documents
            if (ManagedItemPage.ManagedItem.CanGenerateUserDocuments && (bUserDocumentPrinter || bAdminViewer))
            {
                List<ITATDocument> userDocuments = ManagedItemPage.ManagedItem.Documents.FindAll(delegate(ITATDocument d) { return d.DefaultDocument==false; });

                foreach (ITATDocument document in userDocuments)
                {
                    string urlViewPrintManagedItem = "~/DocumentDialog.aspx" + Utility.TextHelper.QueryString(true, Common.Names._QS_DOC_DLG_ACTION, Common.Names._QS_DOC_DLG_ACTION_VIEW, Common.Names._QS_MANAGED_ITEM_ID, this.ManagedItemPage.ManagedItem.ManagedItemID.ToString(), Common.Names._QS_ITAT_SYSTEM_ID, systemID, Common.Names._QS_ITAT_DOCUMENT_ID, document.ITATDocumentID.ToString());
                    documentLink = new ITATMenuItem(menuGroup1.Caption, document.DocumentName, urlViewPrintManagedItem, this.menuLink_Command, this.Page == null, true);
                    menuGroup1.Controls.Add(documentLink);
                }
            }


            //Add a seperator style after the last document link.
            if (documentLink != null)
                documentLink.CssClass = "menuLink menuLinkSeparator";


			if (ManagedItemPage.ITATSystem.TrackAudit ?? false)
				menuGroup1.Controls.Add(new ITATMenuItem(menuGroup1.Caption, "History", "~/ManagedItemHistory.aspx" + urlQueryString, this.menuLink_Command, this.Page == null));

			//Item Rollback
			if (ManagedItemPage.ITATSystem.UserIsInRole(Business.XMLNames._AF_RetroAdmin))
			{
				menuGroup1.Controls.Add(new ITATMenuItem(menuGroup1.Caption, string.Format("{0} Rollback", ManagedItemPage.ITATSystem.ManagedItemName), "~/ManagedItemRollback.aspx" + urlQueryString, this.menuLink_Command, this.Page == null));
			}

			string urlViewPrintSummary = "~/DocumentDialog.aspx" + Utility.TextHelper.QueryString(true, Common.Names._QS_DOC_DLG_ACTION, Common.Names._QS_DOC_DLG_ACTION_SUMMARY, Common.Names._QS_MANAGED_ITEM_ID, this.ManagedItemPage.ManagedItem.ManagedItemID.ToString(), Common.Names._QS_ITAT_SYSTEM_ID, systemID);
			menuGroup1.Controls.Add(new ITATMenuItem(menuGroup1.Caption, "Summary", urlViewPrintSummary, this.menuLink_Command, this.Page == null, true));

			if ((Utility.ListHelper.HaveAMatch(ManagedItemPage.ITATSystem.AllowedRoles(Business.XMLNames._AF_EditLanguage), ManagedItemPage.SecurityHelper.UserRoles)))
			{
				bool displayEditLanguage = true;
				if (ManagedItemPage.ITATSystem.AllowRetro)
				{
					if (ManagedItemPage.ManagedItem.RetroModel == Retro.RetroModel.OnWithoutEditLanguage)
					{
						displayEditLanguage = false;
					}
				}
				if (displayEditLanguage)
				{
                    //TODO: If this MI could be Orphaned, should warn the user that a 'Save' will Orphan it
                    bool couldBeOrphaned = !ManagedItemPage.ManagedItem.IsOrphaned && ManagedItemPage.ManagedItem.RetroModel == Retro.RetroModel.OnWithEditLanguage;
					string urlQueryString2 = Utility.TextHelper.QueryString(true, Common.Names._QS_ITAT_SYSTEM_ID, this.ManagedItemPage.ITATSystem.ID.ToString(), Common.Names._QS_MANAGED_ITEM_ID, this.ManagedItemPage.ManagedItem.ManagedItemID.ToString());
					menuGroup1.Controls.Add(new ITATMenuItem(menuGroup1.Caption, "Edit Language", "~/TemplateTerms.aspx" + urlQueryString2, this.menuLink_Command, this.Page == null));
				}
			}

			menuGroup2 = new ITATMenuGroup(menu, itemName, "", 130, 130, true);
			ITATMenuItem mostRecentItem = null;
			if (ManagedItemPage.ManagedItem.SecurityModel == SecurityModel.Advanced)
			{
				if (ManagedItemPage.ManagedItem.TermGroups != null)
				{
					foreach (TermGroup tg in ManagedItemPage.ManagedItem.GetTermGroups(TermGroup.TermGroupType.AdvancedBasicTerm))
					{
						if (ManagedItemPage.ManagedItem.CanAccessTermGroup(tg, ManagedItemPage.SecurityHelper.UserRoles))
						{
							if (ManagedItemPage is ManagedItemProfile)
							{
								mostRecentItem = new ITATMenuItem(menuGroup2.Caption, tg.Name, "", null, false);
								mostRecentItem.Attributes["onclick"] = string.Format("javascript:DisplayTermGroup('{0}', '{1}')", tg.ID, tg.Name);
							}
							else
							{
								string linkUrl = string.Format("~/ManagedItemProfile.aspx{0}&{1}={2}", urlQueryString, Common.Names._QS_MANAGED_ITEM_TERMGROUP, tg.ID);
								mostRecentItem = new ITATMenuItem(menuGroup2.Caption, tg.Name, linkUrl, null, false);
							}
							menuGroup2.Controls.Add(mostRecentItem);
						}
					}
				}
			}
			else
			{
				TermGroup tg = ManagedItemPage.ManagedItem.TermGroups[0];
				string linkUrl = string.Format("~/ManagedItemProfile.aspx{0}&{1}={2}", urlQueryString, Common.Names._QS_MANAGED_ITEM_TERMGROUP, tg.ID);
				mostRecentItem = new ITATMenuItem(menuGroup2.Caption, tg.Name, linkUrl, null, false);
				menuGroup2.Controls.Add(mostRecentItem);
			}
			//set thick border on last TermGroup menu item
			if (mostRecentItem != null)
			{
				mostRecentItem.CssClass = "menuLink menuLinkSeparator";
			}
			//add complex lists
			if (ManagedItemPage.ManagedItem.ComplexLists != null)
			{
				foreach (Business.ComplexList complexList in ManagedItemPage.ManagedItem.ComplexLists)
				{
					if (ManagedItemPage.ManagedItem.CanAccessTermGroup(complexList.TermGroupID, ManagedItemPage.SecurityHelper.UserRoles))
					{
                        ITATMenuItem complexListMenuItem = new ITATMenuItem(menuGroup2.Caption, complexList.Name, string.Format("~/ManagedItemComplexList.aspx{0}&list={1}", urlQueryString, complexList.Name), this.menuLink_Command, this.Page == null);
						menuGroup2.Controls.Add(complexListMenuItem);
					}
				}
			}
		}


		protected void menuLink_Command(object sender, CommandEventArgs e)
		{
			switch (e.CommandName)
			{
				case "Save":
					SaveManagedItem();
					break;
				case "Reset":
					ResetManagedItem();
					break;
				default:
					OnHeaderEvent(new HeaderEventArgs(e.CommandName, (string)e.CommandArgument));
					break;
			}
		}


		private void ResetManagedItem()
		{
			this.ManagedItemPage.RegisterAlert("Reset the Managed Item");
		}

		private void SaveManagedItem()
		{
			this.ManagedItemPage.RegisterAlert("Save the Managed Item");
		}

	}
}