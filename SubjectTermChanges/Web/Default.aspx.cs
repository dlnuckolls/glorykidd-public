using System;

namespace Kindred.Knect.ITAT.Web
{
    public partial class _Default : System.Web.UI.Page
    {
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            string systemName = Request.QueryString[Common.Names._QS_ITAT_SYSTEM_ID];
            if (string.IsNullOrEmpty(systemName))
            {
                
                this.nav.Attributes["src"] = "/NavBarApp/NavBarMain.aspx?AppName=ITAT";
                this.body.Attributes["src"] = "/itat/message.aspx?message=Select an administrative task from the menu on the left.";
            }
            else
            {
                Business.ITATSystem system = Business.ITATSystem.Get(systemName);
                if (system == null)
                    throw new Exception(string.Format("The system '{0}' cannot be found.", systemName));

                string managedItemID = Request.QueryString[Common.Names._QS_MANAGED_ITEM_ID];
                if (managedItemID != null)
                {
                    //Need to redirect to MIProfile if a MI is povided in the QueryString
                    this.body.Attributes["src"] = string.Format("/itat/manageditemprofile.aspx{0}", Utility.TextHelper.QueryString(true, Common.Names._QS_ITAT_SYSTEM_ID, system.ID.ToString(), Common.Names._QS_MANAGED_ITEM_ID, managedItemID));
                }
                else
                {
                    this.body.Attributes["src"] = "/itat/start.aspx?system=" + systemName;
                }
                //this.nav.Attributes["src"] = "/NavBarApp/NavBarMain.aspx?AppName=" + systemName;
                this.nav.Attributes["src"] = "/NavBarApp/NavBarMain.aspx?AppName=" + system.ApplicationSecurityName;
            }
        }
    }
}
