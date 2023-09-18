using Directory.CGBC.Helpers;
using GloryKidd.WebCore.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Telerik.Web.UI;
namespace Directory.CGBC {
  public partial class MainDirectory : BasePage {
    protected void Page_Load(object sender, EventArgs e) {
      if(SessionInfo.CurrentUser.IsNullOrEmpty() || !SessionInfo.IsAuthenticated) Response.Redirect("/");
      SessionInfo.CurrentPage = PageNames.Home;
      TitleTag.Text = SessionInfo.DisplayCurrentPage;
      PasswordChange.OpenerElementID = rbPassword.ClientID;
      PasswordChange.DestroyOnClose = true;
      SuccessLabel.Text = string.Empty;
      SiteApplicationTitle.Text = "Cedar Grove Baptist Church Online Directory";
      //SiteApplicationInstructions.Text = SessionInfo.Settings.UploadMessage;
      CurrentUser.Text = $"Welcome {SessionInfo.CurrentUser.DisplayName}";
      CancelEdit.Visible = false;
    }
    protected void rbLogout_OnClick(object sender, EventArgs e) { SessionInfo.CurrentUser.LogoutUser(); Response.Redirect("~/"); }

    protected void ConfirmChangePassword_Click(object sender, EventArgs e) {
      if(!Page.IsValid) return;
      SessionInfo.CurrentUser.SetUserPassword(SessionInfo.CurrentUser.Id, NewPassword.Text.Trim());
      ConfirmChangePassword.Visible = false;
      SuccessLabel.Text = "Success!";
    }

    protected void MemberList_NeedDataSource(object sender, GridNeedDataSourceEventArgs e) { ((RadGrid)sender).DataSource = SqlHelpers.Select(SqlStatements.SQL_GET_ALL_MEMBERS); }

    protected void MemberList_EditCommand(object sender, GridCommandEventArgs e) {
      if(e.CommandName.Equals("EditRow")) {
        var member = SqlDataLoader.GetMember((int)((GridDataItem)e.Item).GetDataKeyValue("Id"));
        MemberName.Text = "{0} {1}".FormatWith(member.FirstName, member.LastName);
        NewMember.Text = "Update Member";
        NewMember.CommandName = "UpdateMember";
        CancelEdit.Visible = true;
        e.Item.Selected = true;
      }
    }

    protected void CancelEdit_Click(object sender, EventArgs e) {
      MemberList.Rebind();
      MemberName.Text = string.Empty;
      NewMember.Text = "New Member";
      NewMember.CommandName = "NewMember";
      CancelEdit.Visible = false;
    }

    protected void NewMember_Click(object sender, EventArgs e) {

    }
  }
}