using GloryKidd.WebCore.Helpers;
using System;
using System.Web.UI.WebControls;
using Telerik.Web.UI;

namespace Directory.CGBC {
  public partial class Default : BasePage {
    protected void Page_Load(object sender, EventArgs e) {
      SessionInfo.CurrentPage = PageNames.Home;
      TitleTag.Text = SessionInfo.DisplayCurrentPage;
      lErrorMessage.Text = string.Empty;
    }
    protected void ForgotPassword_OnClick(object sender, EventArgs e) {
      SessionInfo.CurrentPage = PageNames.ForgotPassword;
      Response.Redirect("~/ForgotPassword.aspx");
    }
    protected void Login2_Authenticate(object sender, AuthenticateEventArgs e) {
      lErrorMessage.Text = string.Empty;
      var locationRedirect = string.Empty;
      try {
        //SessionInfo.CurrentUser = new SystemUser();
        SessionInfo.CurrentUser.AuthenticateUser(((RadTextBox)Login2.FindControl("UserName")).Text.Trim(), ((RadTextBox)Login2.FindControl("Password")).Text.Trim().EncryptString());
        if(!SessionInfo.IsAuthenticated) { return; }
        SessionInfo.CurrentPage = (SessionInfo.IsAdmin) ? PageNames.Home : (SessionInfo.CurrentUser.UserPassReset) ? PageNames.ResetPassword : PageNames.Home;
        locationRedirect = "~/MainDirectory.aspx";
        e.Authenticated= true;
      } catch(Exception ex) {
        //lErrorMessage.Text = "Login failed; please verify your username and password";
        SessionInfo.Settings.LogError("Login: Login Failed", ex);
        e.Authenticated = false;
        lErrorMessage.Text = "Login failed.<br/>Please check your username and password.";
      }
      if(!locationRedirect.IsNullOrEmpty()) Response.Redirect(locationRedirect);
    }
  }
}