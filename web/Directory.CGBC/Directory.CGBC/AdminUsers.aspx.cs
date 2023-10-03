using Directory.CGBC.Helpers;
using GloryKidd.WebCore.Helpers;
using System;
using Telerik.Web.UI;

namespace Directory.CGBC {
  public partial class AdminUsers: BasePage {
    protected void Page_Load(object sender, EventArgs e) {
      if(SessionInfo.CurrentUser.IsNullOrEmpty() || !SessionInfo.CurrentUser.IsSuperAdmin) Response.Redirect("~/");
      SessionInfo.CurrentPage = PageNames.Home;
      TitleTag.Text = SessionInfo.DisplayCurrentPage;
      PasswordChange.OpenerElementID = rbPassword.ClientID;
      PasswordChange.DestroyOnClose = true;
      SuccessLabel.Text = string.Empty;
      SiteApplicationTitle.Text = "Cedar Grove Baptist Church Online Directory - Admin";
      CurrentUser.Text = $"Welcome {SessionInfo.CurrentUser.DisplayName}";
    }
    protected void rbLogout_OnClick(object sender, EventArgs e) { SessionInfo.CurrentUser.LogoutUser(); Response.Redirect("~/"); }
    protected void ConfirmChangePassword_Click(object sender, EventArgs e) {
      if(!Page.IsValid) return;
      SessionInfo.CurrentUser.SetUserPassword(SessionInfo.CurrentUser.Id, NewPassword.Text.Trim());
      ConfirmChangePassword.Visible = false;
      SuccessLabel.Text = "Success!";
    }
    protected void rbdirectory_Click(object sender, EventArgs e) {
      SessionInfo.CurrentMember = null;
      Response.Redirect("~/MainDirectory.aspx");
    }

    protected void UserList_NeedDataSource(object sender, Telerik.Web.UI.GridNeedDataSourceEventArgs e) { ((RadGrid)sender).DataSource = SqlDataLoader.GetAdminUsers(); }

    protected void UserList_EditCommand(object sender, GridCommandEventArgs e) {

    }

    protected void UserList_InsertCommand(object sender, GridCommandEventArgs e) {

    }
  }
}