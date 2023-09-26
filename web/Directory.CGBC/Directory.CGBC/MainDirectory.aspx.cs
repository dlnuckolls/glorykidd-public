using Directory.CGBC.Helpers;
using Directory.CGBC.Objects;
using GloryKidd.WebCore.Helpers;
using System;
using System.Drawing;
using System.Web.UI;
using Telerik.Web.UI;

namespace Directory.CGBC {
  public partial class MainDirectory: BasePage {
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
        var member = new Member();
        member.LoadMember((int)((GridDataItem)e.Item).GetDataKeyValue("Id"));
        PopulateMemberDetails(member);
        NewMember.Text = "Edit Member";
        NewMember.CommandName = "EditMember";
        e.Item.Selected = true;
      }
    }

    protected void PopulateMemberDetails(Member member) {
      ClearMemberDetails();
      //View Details
      MemberName.Text = member.DisplayName;
      MemberStatus.Text = member.MaritalStatus.Name;
      MemberAddress.Text = member.PrimaryAddress;
      member.PhoneList.ForEach(p => { MemberPhone.Text += "{0} ({1})<br />".FormatWith(p.FormattedPhoneNumber, p.PhoneType.Name); });
      member.RelatedMembersList.ForEach(r => { MemberRelation.Text += "{0} ({1})<br />".FormatWith(r.DisplayName, r.Relationship.Name); });
      member.EmailList.ForEach(e => { MemberEmails.Text += "{0}<br />".FormatWith(e.Name); });
    }

    protected void ClearMemberDetails() {
      MemberName.Text = string.Empty;
      MemberStatus.Text = string.Empty;
      MemberAddress.Text = string.Empty;
      MemberPhone.Text = string.Empty;
      MemberRelation.Text = string.Empty;
      MemberEmails.Text = string.Empty;
    }

    protected void CancelEdit_Click(object sender, EventArgs e) {
      MemberList.Rebind();
      ClearMemberDetails();
      NewMember.Text = "New Member";
      NewMember.CommandName = "NewMember";
    }

    protected void NewMember_Click(object sender, EventArgs e) {
      if(((RadButton)sender).CommandName.Equals("NewMember")) {
        //DisplayMemberDetails.Visible = false;
        //EditMemberDetails.Visible = true;
        NewMember.Text = "Save";
        NewMember.CommandName = "UpdateMember";
        //CancelEdit.Visible = true;
      } else if(((RadButton)sender).CommandName.Equals("EditMember")) {
        //DisplayMemberDetails.Visible = false;
        //EditMemberDetails.Visible = true;
        NewMember.Text = "Save";
        NewMember.CommandName = "UpdateMember";
        //CancelEdit.Visible = true;
      }
      CancelEdit_Click(sender, e);
    }
  }
}