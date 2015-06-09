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




	public partial class TemplateHeader : BaseHeader
	{

        private bool _hideButtons;

		public BaseTemplatePage TemplatePage
		{
			get 
			{
				BaseTemplatePage p = this.Page as BaseTemplatePage;
				if (p == null)
					throw new InvalidCastException(String.Format("{0} is not an instance of, or derived from , BaseTemplatePage", this.Page.Request.Url));
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

        public bool HideButtons
        {
            get
            {
                return _hideButtons;
            }
            set
            {
                _hideButtons = value;
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
			AddControls();
		}

		protected override void OnPreRender(EventArgs e)
		{
			base.OnPreRender(e);
			RemoveUnneededControls();
			SetBanner();
			SetSubTitle();
		}

		private void AddControls()
		{
			SetButtons();
			SetMenu();
		}

        //Changed for Multiple Documents
		private void RemoveUnneededControls()
		{
			if (!TemplatePage.Template.CanGenerateDocument)
				RemoveButton("Preview");

            if (!TemplatePage.Template.CanGenerateDocument && !TemplatePage.Template.CanGenerateUserDocuments)
                RemoveMenuGroup("Content");
		}



		#endregion


		private void SetBanner()
		{
			if (TemplatePage.Template.IsManagedItem)
				this.lblBanner.Text = TemplatePage.ITATSystem.ManagedItemName;
			else
                if (TemplatePage.BannerOverride != null)
                    this.lblBanner.Text = TemplatePage.BannerOverride;
                else
                    this.lblBanner.Text = "Template";
        }

		private void SetSubTitle()
		{
            if (TemplatePage.SubTitleOverride != null)
                lblSubTitle.Text = TemplatePage.SubTitleOverride;
            else 
                lblSubTitle.Text = TemplatePage.Template.Name;
			SetModifiedSubTitle(TemplatePage.IsChanged);
		}

		private void SetModifiedSubTitle(bool modified)
		{
			if (modified)
			{
				if (lblSubTitle.Text.IndexOf('*') == -1)
					lblSubTitle.Text += '*';
			}
			else
			{
				lblSubTitle.Text = lblSubTitle.Text.Replace("*","");
			}
		}

		private void SetButtons()
		{
            if (!_hideButtons)
            {

                if ((bool)TemplatePage.ITATSystem.HasContent)
                {
                    buttons.Controls.Add(new ITATButton("Preview", button_Command));
                }
                buttons.Controls.Add(new ITATButton("Save", button_Command));
                buttons.Controls.Add(new ITATButton("Reset", button_Command));
            }
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

		private void RemoveMenuItem(string menuGroupCaption, string menuItemCaption)
		{
			ITATMenuGroup menuGroup = (ITATMenuGroup)menu.FindControl(ITATMenuGroup.GetID(menuGroupCaption));
			if (menuGroup != null)
			{
				string menuItemID = ITATMenuItem.GetID(menuGroupCaption, menuItemCaption);
				Control menuItem = menuGroup.FindControl(menuItemID);
				if (menuItem != null)
					menu.Controls.Remove(menuItem);
			}
		}

		private void button_Command(object sender, CommandEventArgs e)
		{
			OnHeaderEvent(new HeaderEventArgs(e.CommandName, (string)e.CommandArgument));
		}


		private void SetMenu()
		{
			ITATMenuGroup menuGroup;
			Type currentPageType = this.Page.GetType();
			//string pageFileName = GetPageBaseFileName();
			string urlQueryString;
			if (TemplatePage.Template.IsManagedItem)
			{
				ManagedItem managedItem = TemplatePage.Template as ManagedItem;
				urlQueryString = Utility.TextHelper.QueryString(true,
					Common.Names._QS_MANAGED_ITEM_ID, managedItem.ManagedItemID.ToString(),
					Common.Names._QS_ITAT_SYSTEM_ID, this.TemplatePage.ITATSystem.ID.ToString(), 
					Common.Names._QS_TEMPLATE_ID, this.TemplatePage.Template.ID.ToString());
			}
			else
				urlQueryString = Utility.TextHelper.QueryString(true, Common.Names._QS_ITAT_SYSTEM_ID, this.TemplatePage.ITATSystem.ID.ToString(), Common.Names._QS_TEMPLATE_ID, this.TemplatePage.Template.ID.ToString());

			menuGroup = new ITATMenuGroup(menu, "Content", "", 100, 100, true);


            //2011 changes - Multiple documents
            if (TemplatePage.Template.CanGenerateUserDocuments)
                menuGroup.Controls.Add(new ITATMenuItem(menuGroup.Caption, "Documents", "~/TemplateDocuments.aspx" + urlQueryString, this.menuLink_Command, this.Page is TemplateDocuments));                
            else
                menuGroup.Controls.Add(new ITATMenuItem(menuGroup.Caption, "Clauses", "~/TemplateClauses.aspx" + urlQueryString, this.menuLink_Command, this.Page is TemplateClauses));
                

            if (TemplatePage.Template.CanGenerateDocument)
			    menuGroup.Controls.Add(new ITATMenuItem(menuGroup.Caption, "Extensions", "~/TemplateExtensions.aspx" + urlQueryString, this.menuLink_Command, this.Page is TemplateExtensions));

			menuGroup = new ITATMenuGroup(menu, "Tracking", "", 100, 100, true);
			menuGroup.Controls.Add(new ITATMenuItem(menuGroup.Caption, "Workflows", "~/TemplateWorkflows.aspx" + urlQueryString, this.menuLink_Command, this.Page is TemplateWorkflowMain));
			menuGroup.Controls.Add(new ITATMenuItem(menuGroup.Caption, "Notifications", "~/TemplateNotifications.aspx" + urlQueryString, this.menuLink_Command, this.Page is TemplateNotifications));

			menuGroup = new ITATMenuGroup(menu, "Template", "", 110, 100, true);
			if (TemplatePage.Template.IsManagedItem)
				menuGroup.Controls.Add(new ITATMenuItem(menuGroup.Caption, TemplatePage.ITATSystem.ManagedItemName, "~/ManagedItemProfile.aspx" + urlQueryString, this.menuLink_Command, this.Page is TemplateMain));
			else
				menuGroup.Controls.Add(new ITATMenuItem(menuGroup.Caption, "Main", "~/TemplateMain.aspx" + urlQueryString, this.menuLink_Command, this.Page is TemplateMain));
			menuGroup.Controls.Add(new ITATMenuItem(menuGroup.Caption, "Terms", "~/TemplateTerms.aspx" + urlQueryString, this.menuLink_Command, this.Page is TemplateTerms));
            menuGroup.Controls.Add(new ITATMenuItem(menuGroup.Caption, "Dependencies", "~/TemplateTermDependencies.aspx" + urlQueryString, this.menuLink_Command, this.Page is TemplateTermDependencies));

            ITATMenuItem complexListMenuItem = new ITATMenuItem(menuGroup.Caption, "Complex Lists", "~/TemplateComplexLists.aspx" + urlQueryString, this.menuLink_Command, this.Page is TemplateComplexLists);
            menuGroup.Controls.Add(complexListMenuItem);

            

            ITATMenuItem historyMenuItem = null;
            if (!TemplatePage.Template.IsManagedItem && TemplatePage.ITATSystem.UserIsInRole(Business.XMLNames._AF_RetroAdmin))
            {
                historyMenuItem = new ITATMenuItem(menuGroup.Caption, "History", "~/TemplateRollback.aspx" + urlQueryString, this.menuLink_Command, this.Page is TemplateRollback);
                menuGroup.Controls.Add(historyMenuItem);
            }

            //Set the border only if Retro History is shown
            if (historyMenuItem != null)
            {
                complexListMenuItem.CssClass = "menuLink menuLinkSeparator";
            }


            
		}


		protected void menuLink_Command(object sender, CommandEventArgs e)
		{
			OnHeaderEvent(new HeaderEventArgs(e.CommandName, (string)e.CommandArgument));
		}

	}
}