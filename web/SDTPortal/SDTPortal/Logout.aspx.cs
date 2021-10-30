using System;

namespace SDTPortal {
  public partial class Logout :BasePage {
    protected void Page_Load(object sender, EventArgs e) {
      SessionInfo.CurrentPage = PageNames.Login;
      SessionInfo.CurrentUser = null;
      Response.Redirect("~/");
    }
  }
}