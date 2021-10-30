using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SDTPortal {
  public partial class MyThankYou :BasePage {
    protected void Page_Load(object sender, EventArgs e) {
      if(SessionInfo.CurrentUser.IsNullOrEmpty() || !SessionInfo.IsAuthenticated) Response.Redirect("/");
      SessionInfo.CurrentPage = PageNames.Thanks;
      TitleTag.Text = SessionInfo.DisplayCurrentPage;
      SiteApplicationTitle.Text = SessionInfo.Settings.SystemName;
      SiteApplicationInstructions.Text = SessionInfo.Settings.ThankyouMessage;
    }
    protected void rbPassword_OnClick(object sender, EventArgs e) { Response.Redirect("~/MyAccount.aspx"); }
    protected void rbLogout_OnClick(object sender, EventArgs e) { Response.Redirect("~/Logout.aspx"); }
  }
}