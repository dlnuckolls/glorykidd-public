using Directory.CGBC.Helpers;
using Directory.CGBC.Objects;
using GloryKidd.WebCore.Helpers;
using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using Telerik.Web.UI;

namespace Directory.CGBC {
  public partial class MainDirectory: BasePage {
    protected void Page_Load(object sender, EventArgs e) {
      if(SessionInfo.CurrentUser.IsNullOrEmpty() || !SessionInfo.IsAuthenticated) Response.Redirect("~/");
      SessionInfo.CurrentPage = PageNames.Home;
      TitleTag.Text = SessionInfo.DisplayCurrentPage;
      PasswordChange.OpenerElementID = rbPassword.ClientID;
      PasswordChange.DestroyOnClose = true;
      SuccessLabel.Text = string.Empty;
      SiteApplicationTitle.Text = "Cedar Grove Baptist Church Online Directory";
      CurrentUser.Text = $"Welcome {SessionInfo.CurrentUser.DisplayName}";
      if(!SessionInfo.IsAdmin)
        NewMember.Visible = false;
      if(!SessionInfo.CurrentUser.IsSuperAdmin)
        SuperAdmin.Visible = false;
    }
    protected void rbLogout_OnClick(object sender, EventArgs e) { SessionInfo.CurrentUser.LogoutUser(); Response.Redirect("~/"); }
    protected void ConfirmChangePassword_Click(object sender, EventArgs e) {
      if(!Page.IsValid) return;
      SessionInfo.CurrentUser.SetUserPassword(SessionInfo.CurrentUser.Id, NewPassword.Text.Trim());
      ConfirmChangePassword.Visible = false;
      SuccessLabel.Text = "Success!";
    }
    protected void MemberList_NeedDataSource(object sender, GridNeedDataSourceEventArgs e) { ((RadGrid)sender).DataSource = SqlDataLoader.GetDirectoryList(); }
    protected void NewMember_Click(object sender, EventArgs e) {
      SessionInfo.CurrentMember = null;
      Response.Redirect("~/Admin/EditMember.aspx");
    }
    protected void MemberList_ItemCommand(object sender, GridCommandEventArgs e) {
      var member = new Member();
      switch(e.CommandName) {
        case "RowClick":
        case "ExpandCollapse":
          bool lastState = e.Item.Expanded;
          if(e.CommandName == "ExpandCollapse") {
            lastState = !lastState;
          }
          CollapseAllRows();
          e.Item.Expanded = !lastState;
          member.LoadMember((int)((GridDataItem)e.Item).GetDataKeyValue("Id"));
          //View Details
          var viewPanel = ((GridDataItem)e.Item).ChildItem;
          //Clear contents
          ((RadLabel)viewPanel.FindControl("MemberName")).Text = string.Empty;
          ((RadLabel)viewPanel.FindControl("MemberStatus")).Text = string.Empty;
          ((RadLabel)viewPanel.FindControl("MemberAddress")).Text = string.Empty;
          ((RadLabel)viewPanel.FindControl("MemberPhone")).Text = string.Empty;
          ((RadLabel)viewPanel.FindControl("MemberPhone")).Text = string.Empty;
          ((RadLabel)viewPanel.FindControl("MemberEmails")).Text = string.Empty;
          ((RadLabel)viewPanel.FindControl("MemberEmails")).Text = string.Empty;
          ((RadLabel)viewPanel.FindControl("MemberRelation")).Text = string.Empty;
          ((RadLabel)viewPanel.FindControl("MemberNotes")).Text = string.Empty;
          //Set Display
          ((RadLabel)viewPanel.FindControl("MemberName")).Text = member.DisplayName;
          ((RadLabel)viewPanel.FindControl("MemberStatus")).Text = member.MaritalStatus.Name;
          ((RadLabel)viewPanel.FindControl("MemberAddress")).Text = member.AddressList.FormattedAddress;
          ((RadLabel)viewPanel.FindControl("MemberPhone")).Text += !member.CellPhone.IsNullOrEmpty() ? "{0} ({1})<br />".FormatWith(member.CellPhone.FormatPhone(), "Cellular") : string.Empty;
          ((RadLabel)viewPanel.FindControl("MemberPhone")).Text += !member.HomePhone.IsNullOrEmpty() ? "{0} ({1})<br />".FormatWith(member.HomePhone.FormatPhone(), "Home") : string.Empty;
          ((RadLabel)viewPanel.FindControl("MemberEmails")).Text += !member.Email1.IsNullOrEmpty() ? "{0}<br />".FormatWith(member.Email1) : string.Empty;
          ((RadLabel)viewPanel.FindControl("MemberEmails")).Text += !member.Email2.IsNullOrEmpty() ? "{0}<br />".FormatWith(member.Email2) : string.Empty;
          ((RadLabel)viewPanel.FindControl("MemberDob")).Text = member.DateOfBirth == DateTime.MinValue ? string.Empty : member.DateOfBirth.ToShortDateString();
          ((RadLabel)viewPanel.FindControl("MemberMarriage")).Text = member.MarriageDate == DateTime.MinValue ? string.Empty : member.MarriageDate.ToShortDateString();
          member.RelatedMembersList.ForEach(r => { ((RadLabel)viewPanel.FindControl("MemberRelation")).Text += "{0} ({1})<br />".FormatWith(r.DisplayName, r.Relationship.Name); });
          member.MemberNotes.ForEach(n => {
            ((RadLabel)viewPanel.FindControl("MemberNotes")).Text += "{0} - {1}<br />".FormatWith(n.UserName, n.NoteDate.ToShortDateString());
            ((RadLabel)viewPanel.FindControl("MemberNotes")).Text += "{0}<br /><br />".FormatWith(n.NoteText);
          });
          SessionInfo.CurrentMember = member;
          break;
        case "EditRow":
          CollapseAllRows();
          member.LoadMember((int)((GridDataItem)e.Item).GetDataKeyValue("Id"));
          SessionInfo.CurrentMember = member;
          Response.Redirect("~/Admin/EditMember.aspx");
          break;
      }
    }
    private void CollapseAllRows() {
      foreach(GridItem item in MemberList.MasterTableView.Items) {
        item.Expanded = false;
      }
    }

    protected void MemberList_PreRender(object sender, EventArgs e) {
      //if(!SessionInfo.IsAdmin)
      //  MemberList.MasterTableView.GetColumn("EditMemberRow").Visible = false;

      foreach(GridDataItem dataItem in MemberList.MasterTableView.Items) {
        var lButton = ((LinkButton)dataItem["EditMemberRow"].Controls[0]);
        if(SessionInfo.IsAdmin || (int)(dataItem).GetDataKeyValue("Id") == SessionInfo.CurrentUser.MemberId) {
          lButton.Enabled = true;
        } else {
          lButton.Enabled = false;
          lButton.Text = string.Empty;
        }
      }

    }

    protected void SuperAdmin_Click(object sender, EventArgs e) {
      SessionInfo.CurrentMember = null;
      Response.Redirect("~/Admin/AdminUsers.aspx");
    }
  }
}