using System;
namespace SDTPortal {
  public partial class MyAccount :BasePage {
    protected void Page_Load(object sender, EventArgs e) {
      if(SessionInfo.CurrentUser.IsNullOrEmpty() || !SessionInfo.IsAuthenticated) Response.Redirect("/");
      SessionInfo.CurrentPage = PageNames.MyAccount;
      TitleTag.Text = SessionInfo.DisplayCurrentPage;
      SiteApplicationTitle.Text = SessionInfo.Settings.SystemName;
    }
    protected void RadButton3_OnClick(object sender, EventArgs e) {
      try {
        if (!NewPassword.Text.IsNullOrEmpty() && !NewPassword.Text.Trim().Equals(ConfirmPassword.Text.Trim())) {
          throw new ApplicationException("New Password and Confirmation do not match");
        }
        if(!NewPassword.Text.IsNullOrEmpty() && NewPassword.Text.Trim().Length < 6) {
          throw new ApplicationException("New Password must be at least 6 characters");
        }
        SessionInfo.CurrentUser.Notes += "| Password updated {0}".FormatWith(DateTime.Now.ToShortDateString());
        SessionInfo.CurrentUser.SaveUserDetails();
        if(!NewPassword.Text.Trim().IsNullOrEmpty()) {
          SessionInfo.CurrentUser.SetUserPassword(SessionInfo.CurrentUser.Id, NewPassword.Text.Trim());
        }
        lErrorMessage.Text = "Your password has been updated (Please Logout now and Login with your new password)";
        lErrorMessage.CssClass = "successMessageDisplay";
        RadButton3.Visible = false;
        UserNav.DisableControls();
      } catch(Exception ex) {
        lErrorMessage.Text = "Your password was not updated";
        lErrorMessage.CssClass = "errorMessageDisplay";
        SessionInfo.Settings.LogError("User: Update Password", ex);
      }
    }
    protected void RadPageLayout4_OnPreRender(object sender, EventArgs e) {
      LoginId.Text = SessionInfo.CurrentUser.UserName;
      DisplayName.Text = SessionInfo.CurrentUser.DisplayName;
    }
    protected void rbLogout_OnClick(object sender, EventArgs e) { Response.Redirect("~/Logout.aspx"); }
  }
}