using Directory.CGBC.Helpers;
using GloryKidd.WebCore.BaseObjects;
using GloryKidd.WebCore.Helpers;
using System;
using System.Collections;
using Telerik.Web.UI;

namespace Directory.CGBC {
  public partial class AdminUsers: BasePage {
    protected void Page_Load(object sender, EventArgs e) {
      if(SessionInfo.CurrentUser.IsNullOrEmpty() || !SessionInfo.CurrentUser.IsSuperAdmin) Response.Redirect("~/");
      if(Page.IsPostBack) return;
      SessionInfo.CurrentPage = PageNames.Home;
      TitleTag.Text = SessionInfo.DisplayCurrentPage;
      PasswordChange.OpenerElementID = rbPassword.ClientID;
      PasswordChange.DestroyOnClose = true;
      SuccessLabel.Text = string.Empty;
      SiteApplicationTitle.Text = "Cedar Grove Baptist Church Online Directory - Admin";
      CurrentUser.Text = $"Welcome {SessionInfo.CurrentUser.DisplayName}";
      lErrorMessage.Text = string.Empty;
      lErrorMessage.CssClass = "errorMessageDisplay";
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
      try {
        var userId = (int)((GridDataItem)e.Item).GetDataKeyValue("Id");
        var sysUser = new SystemUser();
        sysUser.LoadUserDetails(userId);
        var editableItem = ((GridEditableItem)e.Item);
        //populate its properties
        Hashtable values = new Hashtable();
        editableItem.ExtractValues(values);
        sysUser.RoleId = values["RoleId"].ToString();
        sysUser.DisplayName = values["DisplayName"].ToString();
        sysUser.UserName = values["UserName"].ToString();
        sysUser.Notes += "| Update to user record on {0}".FormatWith(DateTime.Now.ToShortDateString());
        sysUser.MemberId = values["MemberId"].ToString().GetInt32();
        sysUser.SaveUserDetails();
        lErrorMessage.Text = "Success";
        lErrorMessage.CssClass = "successMessageDisplay";
      } catch(Exception ex) {
        SessionInfo.Settings.LogError("UserAdmin", ex);
        lErrorMessage.Text = "Update Failed";
        lErrorMessage.CssClass = "errorMessageDisplay";
      }
    }

    protected void UserList_InsertCommand(object sender, GridCommandEventArgs e) {
      try {
        var editableItem = ((GridEditableItem)e.Item);
        //populate its properties
        Hashtable values = new Hashtable();
        editableItem.ExtractValues(values);
        var sysUser = new SystemUser();
        sysUser.RoleId = values["RoleId"].ToString();
        sysUser.DisplayName = values["DisplayName"].ToString();
        sysUser.UserName = values["UserName"].ToString();
        sysUser.Notes += "| New user record added on {0}".FormatWith(DateTime.Now.ToShortDateString());
        sysUser.MemberId = values["MemberId"].ToString().GetInt32();
        sysUser.SaveUserDetails();
        lErrorMessage.Text = "Success";
        lErrorMessage.CssClass = "successMessageDisplay";
      } catch(Exception ex) {
        SessionInfo.Settings.LogError("UserAdmin", ex);
        lErrorMessage.Text = "Save Failed";
        lErrorMessage.CssClass = "errorMessageDisplay";
      }
    }

    protected void UserList_DeleteCommand(object sender, GridCommandEventArgs e) {
      try {
        var i = (int)((GridDataItem)e.Item).GetDataKeyValue("Id");
        var sysUser = new SystemUser();
        sysUser.Id = i;
        sysUser.RemoveUser();
        lErrorMessage.Text = "Success";
        lErrorMessage.CssClass = "successMessageDisplay";
      } catch(Exception ex) {
        SessionInfo.Settings.LogError("UserAdmin", ex);
        lErrorMessage.Text = "Delete Failed";
        lErrorMessage.CssClass = "errorMessageDisplay";
      }
    }

    protected void UserList_ItemCommand(object sender, GridCommandEventArgs e) {
      if(e.CommandName == "ResetPassword") {
        try {
          var clientId = (int)((GridDataItem)e.Item).GetDataKeyValue("Id");
          var usrRec = new SystemUser();
          usrRec.LoadUserDetails(clientId);
          var tempPassword = usrRec.ResetUserPassword(clientId);
          SessionInfo.SendResetEmail(usrRec, tempPassword);
          lErrorMessage.Text = "Reset Sent";
          lErrorMessage.CssClass = "successMessageDisplay";
        } catch(Exception ex) {
          SessionInfo.Settings.LogError("Admin: Reset Account Password", ex);
          lErrorMessage.Text = "Reset Send Failed";
          lErrorMessage.CssClass = "errorMessageDisplay";
        }
      }
    }
  }
}