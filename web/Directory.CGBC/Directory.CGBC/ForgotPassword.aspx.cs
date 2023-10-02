using GloryKidd.WebCore.BaseObjects;
using GloryKidd.WebCore.Helpers;
using System;

namespace Directory.CGBC {
  public partial class ForgotPassword :BasePage {
    protected void Page_Load(object sender, EventArgs e) {
      SessionInfo.CurrentPage = PageNames.ForgotPassword;
      TitleTag.Text = SessionInfo.DisplayCurrentPage;
      lErrorMessage.Text = string.Empty;
    }
    protected void SubmitLogin_OnClick(object sender, EventArgs e) {
      try {
        var s = (new SystemUser()).ValidateUser(userName.Text.Trim()).GetInt32();
        if(!s.IsNullOrEmpty()) {
          var usrRec = new SystemUser();
          usrRec.LoadUserDetails(s);
          var tempPassword = usrRec.ResetUserPassword(s);
          SessionInfo.SendResetEmail(usrRec, tempPassword);
          lErrorMessage.Text = "Check your email to complete resetting your password";
          SubmitLogin.Visible = false;
          ReturnToLogin.Text = "Done";
        } else
          lErrorMessage.Text = "Check your email to complete resetting your password";
      } catch(Exception ex) {
        lErrorMessage.Text = ex.Message;
        SessionInfo.Settings.LogError("User: Forgot Password", ex);
      }
    }
    protected void ReturnToLogin_OnClick(object sender, EventArgs e) { Response.Redirect("~/"); }
  }
}