using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SDTPortal {
  public partial class Default : BasePage {
    protected void Page_Load(object sender, EventArgs e) {
      TitleTag.Text = SessionInfo.DisplayCurrentPage;
      lErrorMessage.Text = string.Empty;
      SiteApplicationTitle.Text = SessionInfo.Settings.SystemName;
    }

    protected void SubmitLogin_OnClick(object sender, EventArgs e) {
      var locationRedirect = string.Empty;
      try {
        SessionInfo.CurrentUser = new SystemUser();
        SessionInfo.CurrentUser.AuthenticateUser(userName.Text.Trim(), password.Text.Trim().EncryptString());
        if (!SessionInfo.IsAuthenticated) { lErrorMessage.Text = "Username or password do not match"; SessionInfo.Settings.LogError("Login: Login Failed", "Invalid credentials"); return; }
        locationRedirect = (SessionInfo.IsAdmin) ? "~/Administration.aspx" : (SessionInfo.CurrentUser.UserPassReset) ? "~/MyAccount.aspx" : "~/MyUploads.aspx";
        SessionInfo.CurrentPage = (SessionInfo.IsAdmin) ? PageNames.Administration : (SessionInfo.CurrentUser.UserPassReset) ? PageNames.MyAccount : PageNames.Uploads;
      } catch (Exception ex) {
        lErrorMessage.Text = "Login failed; please verify your username and password";
        SessionInfo.Settings.LogError("Login: Login Failed", ex);
      }
      if (!locationRedirect.IsNullOrEmpty()) Response.Redirect(locationRedirect);
    }
    protected void ForgotPassword_OnClick(object sender, EventArgs e) {
      SessionInfo.CurrentPage = PageNames.Forgot;
      Response.Redirect("~/ForgotPassword.aspx");
    }
  }
}