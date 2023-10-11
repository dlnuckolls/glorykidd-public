using Directory.CGBC.Helpers;
using Directory.CGBC.Objects;
using GloryKidd.WebCore.Helpers;
using System;
using System.Linq;
using System.Web.UI;
using Telerik.Web.UI;

namespace Directory.CGBC {
  public partial class EditMember: BasePage {
    protected void Page_Load(object sender, EventArgs e) {
      if(Page.IsPostBack) return;
      if(SessionInfo.CurrentUser.IsNullOrEmpty() || !SessionInfo.IsAuthenticated) Response.Redirect("~/");
      SessionInfo.CurrentPage = PageNames.EditMember;
      TitleTag.Text = SessionInfo.DisplayCurrentPage;
      PasswordChange.OpenerElementID = rbPassword.ClientID;
      PasswordChange.DestroyOnClose = true;
      SuccessLabel.Text = string.Empty;
      SiteApplicationTitle.Text = "Cedar Grove Baptist Church Online Directory";
      CurrentUser.Text = $"Welcome {SessionInfo.CurrentUser.DisplayName}";
      PopulateDdl();
      PopulateMember();
      if(!SessionInfo.IsAdmin) {
        NewMember.Visible = true;
      } else {
        NewMember.Visible = false;
      }
    }
    private void PopulateMember() {
      Member member = new Member();
      if(!SessionInfo.CurrentMember.IsNullOrEmpty()) {
        member = (Member)SessionInfo.CurrentMember;
        NewMember.Visible = false;
      }
      rddSalutation.SelectedValue = member.Salutation.Id.ToString();
      tMemberFirstName.Text = member.FirstName;
      tMemberMiddleName.Text = !member.MiddleName.IsNullOrEmpty() ? member.MiddleName : tMemberMiddleName.EmptyMessage = string.Empty;
      tMemberLastName.Text = member.LastName;
      tMemberSuffix.Text = !member.Suffix.IsNullOrEmpty() ? member.Suffix : tMemberSuffix.EmptyMessage = string.Empty;
      rddMaritalStatus.SelectedValue = member.MaritalStatus.Id.ToString();
      tMemberAddress1.Text = member.AddressList.Address1;
      tMemberAddress2.Text = member.AddressList.Address2;
      tMemberCity.Text = member.AddressList.City;
      rddStates.SelectedValue = member.AddressList.State.Id.ToString();
      tMemberZip.Text = member.AddressList.ZipCode;
      tMemberCell.Text = member.CellPhone.FormatPhone();
      tMemberHome.Text = member.HomePhone.FormatPhone();
      tMemberEmail1.Text = member.Email1;
      tMemberEmail2.Text = member.Email2;
      member.MemberNotes.ForEach(n => {
        tMemberHistoricalNotes.Text += "<b><i>{0} - {1}</i></b><br />".FormatWith(n.UserName, n.NoteDate.ToShortDateString());
        tMemberHistoricalNotes.Text += "{0}<br /><br />".FormatWith(n.NoteText);
      });
      if(member.DateOfBirth != DateTime.MinValue)
        dpMemberBirthdate.SelectedDate = member.DateOfBirth;
      if(member.MarriageDate != DateTime.MinValue)
        dpMemberMarriage.SelectedDate = member.MarriageDate;

      tMemberLastUpdate.Text = member.Modified != DateTime.MinValue ? member.Modified.ToShortDateString() : string.Empty;
      SessionInfo.CurrentMember = member;
    }
    private void PopulateDdl() {
      rddSalutation.Items.Clear();
      SqlDataLoader.Salutations().ForEach(s => {
        rddSalutation.Items.Add(new DropDownListItem(s.Name, s.Id.ToString()));
      });
      rddMaritalStatus.Items.Clear();
      SqlDataLoader.MaritalStatuses().ForEach(s => {
        rddMaritalStatus.Items.Add(new DropDownListItem(s.Name, s.Id.ToString()));
      });
      rddStates.Items.Clear();
      SqlDataLoader.States().ForEach(s => {
        rddStates.Items.Add(new DropDownListItem(s.Name, s.Id.ToString()));
      });
      dpMemberBirthdate.MinDate = "01/01/1900".GetAsDate();
      dpMemberMarriage.MinDate = "01/01/1900".GetAsDate();
    }
    protected void rbLogout_OnClick(object sender, EventArgs e) { SessionInfo.CurrentUser.LogoutUser(); Response.Redirect("~/"); }
    protected void ConfirmChangePassword_Click(object sender, EventArgs e) {
      if(!Page.IsValid) return;
      SessionInfo.CurrentUser.SetUserPassword(SessionInfo.CurrentUser.Id, NewPassword.Text.Trim());
      ConfirmChangePassword.Visible = false;
      SuccessLabel.Text = "Success!";
    }
    protected void rbdirectory_Click(object sender, EventArgs e) {
      SqlDataLoader.ReloadMemberRelations();
      SessionInfo.CurrentMember = null;
      Response.Redirect("~/MainDirectory.aspx");
    }
    protected void UpdateMember_Click(object sender, EventArgs e) {
      var member = (Member)SessionInfo.CurrentMember;
      member.Salutation = SqlDataLoader.Salutations().FirstOrDefault(s => s.Id == rddSalutation.SelectedValue.GetInt32());
      member.FirstName = tMemberFirstName.Text.Trim();
      member.MiddleName = tMemberMiddleName.Text.Trim();
      member.LastName = tMemberLastName.Text.Trim();
      member.Suffix = tMemberSuffix.Text.Trim();
      member.MaritalStatus = SqlDataLoader.MaritalStatuses().FirstOrDefault(s => s.Id == rddMaritalStatus.SelectedValue.GetInt32());
      member.AddressList.Address1 = tMemberAddress1.Text.Trim();
      member.AddressList.Address2 = tMemberAddress2.Text.Trim();
      member.AddressList.City = tMemberCity.Text.Trim();
      member.AddressList.State = SqlDataLoader.States().FirstOrDefault(s => s.Id == rddStates.SelectedValue.GetInt32());
      member.AddressList.ZipCode = tMemberZip.Text.Trim();
      member.CellPhone = tMemberCell.Text.Trim();
      member.HomePhone = tMemberHome.Text.Trim();
      member.Email1 = tMemberEmail1.Text.Trim();
      member.Email2 = tMemberEmail2.Text.Trim();
      if(dpMemberBirthdate.SelectedDate.HasValue)
        member.DateOfBirth = dpMemberBirthdate.SelectedDate.Value;
      if(dpMemberMarriage.SelectedDate.HasValue)
        member.MarriageDate = dpMemberMarriage.SelectedDate.Value;
      var memberNote = tMemberNotes.Text.Trim();
      member.SaveMember(memberNote, SessionInfo.CurrentUser.Id);
      NewMember.Visible = true;
      UpdateMember.Visible = false;
      CancelUpdate.Text = "Done";
    }

    protected void MemberRelations_NeedDataSource(object sender, GridNeedDataSourceEventArgs e) { MemberRelations.DataSource = ((Member)SessionInfo.CurrentMember).RelatedMembersList; }

    protected void MemberRelations_InsertCommand(object sender, GridCommandEventArgs e) {
      var editableItem = ((GridEditableItem)e.Item);
      //populate its properties
      var relation = (RadDropDownList)editableItem.FindControl("rdRelatedMember");
      var relationship = (RadDropDownList)editableItem.FindControl("rdRelationType");
      var rm = new RelatedMember();
      rm.Id = relation.SelectedValue.GetInt32();
      rm.DisplayName = SqlDataLoader.AllMemberRelations().FirstOrDefault(r => r.Id == rm.Id).Name;
      rm.Relationship = SqlDataLoader.RelationshipTypes().FirstOrDefault(r => r.Id == relationship.SelectedValue.GetInt32());
      ((Member)SessionInfo.CurrentMember).RelatedMembersList.Add(rm);
    }

    protected void MemberRelations_DeleteCommand(object sender, GridCommandEventArgs e) {
      var i = (int)((GridDataItem)e.Item).GetDataKeyValue("Id");
      var rm = ((Member)SessionInfo.CurrentMember).RelatedMembersList.FirstOrDefault(r => r.Id == i);
      ((Member)SessionInfo.CurrentMember).RelatedMembersList.Remove(rm);
    }

    protected void NewMember_Click(object sender, EventArgs e) {
      SqlDataLoader.ReloadMemberRelations();
      SessionInfo.CurrentMember = null;
      PopulateDdl();
      PopulateMember();
      NewMember.Visible = false;
      UpdateMember.Visible = true;
      CancelUpdate.Text = "Cancel";
    }
  }
}