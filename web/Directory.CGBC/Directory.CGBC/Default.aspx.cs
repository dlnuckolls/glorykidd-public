using GloryKidd.WebCore.BaseObjects;
using GloryKidd.WebCore.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Directory.CGBC {
  public partial class Default : BasePage {
    protected void Page_Load(object sender, EventArgs e) {
      SessionInfo.CurrentPage = PageNames.Home;
      TitleTag.Text = SessionInfo.DisplayCurrentPage;
      lErrorMessage.Text = string.Empty;
    }
    protected void SubmitLogin_OnClick(object sender, EventArgs e) {
      var locationRedirect = string.Empty;
      try {
        //SessionInfo.CurrentUser = new SystemUser();
        SessionInfo.CurrentUser.AuthenticateUser(userName.Text.Trim(), password.Text.Trim().EncryptString());
        if(!SessionInfo.IsAuthenticated) { lErrorMessage.Text = "Username or password do not match"; SessionInfo.Settings.LogError("Login: Login Failed", "Invalid credentials"); return; }
        locationRedirect = (SessionInfo.IsAdmin) ? "~/Administration.aspx" : (SessionInfo.CurrentUser.UserPassReset) ? "~/MyAccount.aspx" : "~/MyUploads.aspx";
        SessionInfo.CurrentPage = (SessionInfo.IsAdmin) ? PageNames.Admin : (SessionInfo.CurrentUser.UserPassReset) ? PageNames.ResetPassword : PageNames.Home;
      } catch(Exception ex) {
        lErrorMessage.Text = "Login failed; please verify your username and password";
        SessionInfo.Settings.LogError("Login: Login Failed", ex);
      }
      if(!locationRedirect.IsNullOrEmpty()) Response.Redirect(locationRedirect);
    }
    protected void ForgotPassword_OnClick(object sender, EventArgs e) {
      SessionInfo.CurrentPage = PageNames.ForgotPassword;
      Response.Redirect("~/ForgotPassword.aspx");
    }

  }
}