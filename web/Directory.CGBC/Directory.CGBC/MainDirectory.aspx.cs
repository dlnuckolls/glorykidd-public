using GloryKidd.WebCore.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Telerik.Web.UI;
namespace Directory.CGBC {
  public partial class MainDirectory :BasePage {
    protected void Page_Load(object sender, EventArgs e) {
      if(SessionInfo.CurrentUser.IsNullOrEmpty() || !SessionInfo.IsAuthenticated) Response.Redirect("/");
      SessionInfo.CurrentPage = PageNames.Home;
      TitleTag.Text = SessionInfo.DisplayCurrentPage;
      //SiteApplicationTitle.Text = SessionInfo.Settings.SystemName;
      //SiteApplicationInstructions.Text = SessionInfo.Settings.UploadMessage;
      CurrentUser.Text = $"Welcome {SessionInfo.CurrentUser.DisplayName}";
      SubmitUserName.Text = SessionInfo.CurrentUser.DisplayName;
      lErrorMessage.Text = string.Empty;
    }
    protected void RadButton1_OnClick(object sender, EventArgs e) {
      if(!Page.IsValid) return;
      lErrorMessage.Text = string.Empty;
      //try {
      //  if(SendToUser.SelectedValue.Equals("--")) {
      //    throw new ApplicationException("Please Select a user to send to");
      //  }
      //  if(FilesToSend.UploadedFiles.Count == 0) {
      //    throw new ApplicationException("Please provide at least one file attachment");
      //  }
      //  if(SubjectLine.Text.IsNullOrEmpty()) {
      //    SubjectLine.Text = "Document(s) Sent.";
      //  }
      //  SessionInfo.EmailDocument = new DocumentSubmission() {
      //    FromUser = SessionInfo.CurrentUser.Id,
      //    ToEmail = SendToUser.SelectedValue,
      //    SubjectLine = SubjectLine.Text.Trim(),
      //    Attachments = new List<UploadedFile>(),
      //    SubmitDate = DateTime.Now
      //  };
      //  foreach(UploadedFile doc in FilesToSend.UploadedFiles) {
      //    SessionInfo.EmailDocument.Attachments.Add(doc);
      //  }
      //  try {
      //    SessionInfo.EmailDocument.SaveDocument(Server.MapPath("~/docUploads"));
      //    SessionInfo.SubmitEmail();
      //  } catch(Exception sex) {
      //    SessionInfo.Settings.LogError("User: Save/Email Doc", sex);
      //  }
      //} catch(Exception ex) {
      //  lErrorMessage.Text = ex.Message;
      //  SessionInfo.Settings.LogError("User: Upload Files", ex);
      //  return;
      //}
      //Response.Redirect("~/MyThankYou.aspx");
    }
    protected void SendToUser_OnDataBound(object sender, EventArgs e) {
      var combo = (RadComboBox)sender;
      combo.Items.Insert(0, new RadComboBoxItem("-- Please Select Recipient --", "--"));
    }
    protected void rbPassword_OnClick(object sender, EventArgs e) { Response.Redirect("~/MyAccount.aspx"); }
    protected void rbLogout_OnClick(object sender, EventArgs e) { Response.Redirect("~/Logout.aspx"); }
  }
}