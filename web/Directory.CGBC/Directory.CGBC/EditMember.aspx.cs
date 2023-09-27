using Directory.CGBC.Helpers;
using Directory.CGBC.Objects;
using GloryKidd.WebCore.Helpers;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Telerik.Web.UI;

namespace Directory.CGBC {
  public partial class EditMember: BasePage {
    protected void Page_Load(object sender, EventArgs e) {
      if(SessionInfo.CurrentUser.IsNullOrEmpty() || !SessionInfo.IsAuthenticated) Response.Redirect("/");
      SessionInfo.CurrentPage = PageNames.EditMember;
      TitleTag.Text = SessionInfo.DisplayCurrentPage;
      PasswordChange.OpenerElementID = rbPassword.ClientID;
      PasswordChange.DestroyOnClose = true;
      SuccessLabel.Text = string.Empty;
      SiteApplicationTitle.Text = "Cedar Grove Baptist Church Online Directory";
      SiteApplicationInstructions.Text = "Edit Member";
      CurrentUser.Text = $"Welcome {SessionInfo.CurrentUser.DisplayName}";
      PopulateDdl();
      PopulateMember();
    }
    private void PopulateMember() {
      if(!SessionInfo.CurrentMember.IsNullOrEmpty()) {
        Member member = (Member)SessionInfo.CurrentMember;
        rddSalutation.SelectedValue = member.Salutation.Id.ToString();
        tMemberFirstName.Text = member.FirstName;
        tMemberMiddleName.Text = !member.MiddleName.IsNullOrEmpty() ? member.MiddleName : tMemberMiddleName.EmptyMessage = string.Empty;
        tMemberLastName.Text = member.LastName;
        tMemberSuffix.Text = !member.Suffix.IsNullOrEmpty() ? member.Suffix : tMemberSuffix.EmptyMessage = string.Empty;
        rddMaritalStatus.SelectedValue = member.MaritalStatus.Id.ToString();
        if(member.AddressList.Count > 0) {
          tMemberAddress1.Text = member.AddressList[0].Address1;
          tMemberAddress2.Text = member.AddressList[0].Address2;
          tMemberCity.Text = member.AddressList[0].City;
          rddStates.SelectedValue = member.AddressList[0].State.Id.ToString();
        }
        gMemberPhones.DataSource = member.PhoneList;
        gMemberPhones.DataBind();
      }
    }
    private void PopulateDdl() {
      rddSalutation.Items.Clear();
      SqlDataLoader.Salutations.ForEach(s => {
        rddSalutation.Items.Add(new DropDownListItem(s.Name, s.Id.ToString()));
      });
      rddMaritalStatus.Items.Clear();
      SqlDataLoader.MaritalStatuses.ForEach(s => {
        rddMaritalStatus.Items.Add(new DropDownListItem(s.Name, s.Id.ToString()));
      });
      rddStates.Items.Clear();
      SqlDataLoader.States.ForEach(s => {
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
      SessionInfo.CurrentMember = null;
      Response.Redirect("~/MainDirectory.aspx");
    }

    protected void gMemberPhones_ItemDataBound(object sender, GridItemEventArgs e) {

      if(e.Item is GridDataItem) {
        GridDataItem item = (GridDataItem)e.Item;
        var phone = (Phone)e.Item.DataItem;
        item["ddl1"].Text = phone.PhoneType.Name;
      }
    }

  }
}