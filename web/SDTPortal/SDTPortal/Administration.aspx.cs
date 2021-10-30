using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Telerik.Web.UI;
namespace SDTPortal {
  public partial class Administration: BasePage {
    protected void Page_Load(object sender, EventArgs e) {
      if(SessionInfo.CurrentUser.IsNullOrEmpty() || !SessionInfo.IsAuthenticated) Response.Redirect("~/");
      if(!SessionInfo.IsAdmin) Response.Redirect("~/MyUploads.aspx");
      SiteApplicationTitle.Text = SessionInfo.Settings.SystemName;
      SiteApplicationInstructions.Text = (SessionInfo.CurrentUser.IsSuperAdmin) ? SessionInfo.Settings.SuperAdminMessage : SessionInfo.Settings.AdminMessage;
      HideSuperAdminTabs();
      SuperAdmin.Text = (SessionInfo.CurrentUser.IsSuperAdmin) ? "Super&nbsp;{0}&nbsp;".FormatWith(SessionInfo.CurrentUser.Role) : "{0}&nbsp;".FormatWith(SessionInfo.CurrentUser.Role);
      RadTabStrip1.Tabs[6].Visible = false;
      RadTabStrip1.Tabs[7].Visible = false;
      MessageDisplay.Text = string.Empty;
      MessageDisplay.CssClass = string.Empty;
    }
    private void HideSuperAdminTabs() {
      if(!SessionInfo.CurrentUser.IsSuperAdmin) {
        RadTabStrip1.Tabs[2].Visible = false;
        RadTabStrip1.Tabs[3].Visible = false;
        RadTabStrip1.Tabs[4].Visible = false;
        RadTabStrip1.Tabs[5].Visible = false;
      }
    }
    #region Client and Admin Account Tabs
    protected void ClientAccountsList_OnNeedDataSource(object sender, GridNeedDataSourceEventArgs e) { ((RadGrid)sender).DataSource = SqlDatasets.GetClientList(); }
    protected void AdminAccountList_OnNeedDataSource(object sender, GridNeedDataSourceEventArgs e) { ((RadGrid)sender).DataSource = SqlDatasets.GetAdminList(); }
    protected void ClientAccountsList_OnUpdateCommand(object sender, GridCommandEventArgs e) {
      try {
        var clientId = (Guid)((GridDataItem)e.Item).GetDataKeyValue("Id");
        var editableItem = ((GridEditableItem)e.Item);
        Hashtable values = new Hashtable();
        editableItem.ExtractValues(values);
        var usrRec = new SystemUser();
        usrRec.LoadUserDetails(clientId.ToString());
        usrRec.DisplayName = values["DisplayName"].ToString();
        usrRec.UserName = values["UserName"].ToString();
        usrRec.Notes = (!values["Notes"].IsNullOrEmpty()) ? values["Notes"].ToString() : string.Empty;
        usrRec.SaveUserDetails();
        MessageDisplay.Text = "Account Updated";
        MessageDisplay.CssClass = "successMessageDisplay";
      } catch(Exception ex) {
        MessageDisplay.Text = "Account Failed to Update";
        MessageDisplay.CssClass = "errorMessageDisplay";
        SessionInfo.Settings.LogError("Admin: Update User", ex);
      }
    }
    protected void ClientAccountsList_OnUpdateCommand2(object sender, GridCommandEventArgs e) {
      try {
        var clientId = (Guid)((GridDataItem)e.Item).GetDataKeyValue("Id");
        var editableItem = ((GridEditableItem)e.Item);
        Hashtable values = new Hashtable();
        editableItem.ExtractValues(values);
        var usrRec = new SystemUser();
        usrRec.LoadUserDetails(clientId.ToString());
        usrRec.DisplayName = values["DisplayName"].ToString();
        usrRec.UserName = values["UserName"].ToString();
        usrRec.Notes = (!values["Notes"].IsNullOrEmpty()) ? values["Notes"].ToString() : string.Empty;
        usrRec.SuperAdmin = values["SuperAdmin"].ToString().GetAsBool();
        usrRec.SaveAdminUserDetails();
        MessageDisplay.Text = "Account Updated";
        MessageDisplay.CssClass = "successMessageDisplay";
      } catch(Exception ex) {
        MessageDisplay.Text = "Account Failed to Update";
        MessageDisplay.CssClass = "errorMessageDisplay";
        SessionInfo.Settings.LogError("Admin: Update User", ex);
      }
    }
    protected void ClientAccountsList_OnInsertCommand(object sender, GridCommandEventArgs e) {
      try {
        var editableItem = ((GridEditableItem)e.Item);
        Hashtable values = new Hashtable();
        editableItem.ExtractValues(values);
        var usrRec = new SystemUser();
        usrRec.DisplayName = values["DisplayName"].ToString();
        usrRec.UserName = values["UserName"].ToString();
        usrRec.Notes = (!values["Notes"].IsNullOrEmpty()) ? values["Notes"].ToString() : string.Empty;
        usrRec.SaveUserDetails();
        usrRec.ResetUserPassword(usrRec.Id);
        var tempPassword = usrRec.ResetUserPassword(usrRec.Id);
        SessionInfo.SendResetEmail(usrRec, tempPassword);
        MessageDisplay.Text = "User Account Created";
        MessageDisplay.CssClass = "successMessageDisplay";
      } catch(Exception ex) {
        MessageDisplay.Text = "User Account Not Created";
        MessageDisplay.CssClass = "errorMessageDisplay";
        SessionInfo.Settings.LogError("Admin: Create User", ex);
      }
    }
    protected void ClientAccountsList_OnInsertCommand2(object sender, GridCommandEventArgs e) {
      try {
        var editableItem = ((GridEditableItem)e.Item);
        Hashtable values = new Hashtable();
        editableItem.ExtractValues(values);
        var usrRec = new SystemUser();
        usrRec.DisplayName = values["DisplayName"].ToString();
        usrRec.UserName = values["UserName"].ToString();
        usrRec.Notes = (!values["Notes"].IsNullOrEmpty()) ? values["Notes"].ToString() : string.Empty;
        usrRec.SuperAdmin = values["SuperAdmin"].ToString().GetAsBool();
        usrRec.SaveAdminUserDetails();
        usrRec.ResetUserPassword(usrRec.Id);
        var tempPassword = usrRec.ResetUserPassword(usrRec.Id);
        SessionInfo.SendResetEmail(usrRec, tempPassword);
        MessageDisplay.Text = "Admin Account Created";
        MessageDisplay.CssClass = "successMessageDisplay";
      } catch(Exception ex) {
        MessageDisplay.Text = "Admin Account Not Created";
        MessageDisplay.CssClass = "errorMessageDisplay";
        SessionInfo.Settings.LogError("Admin: Create Admin User", ex);
      }
    }
    protected void ClientAccountsList_OnItemCommand(object sender, GridCommandEventArgs e) {
      if(e.CommandName == "ResetPassword") {
        try {
          var clientId = (Guid)((GridDataItem)e.Item).GetDataKeyValue("Id");
          var usrRec = new SystemUser();
          usrRec.LoadUserDetails(clientId.ToString());
          var tempPassword = usrRec.ResetUserPassword(clientId.ToString());
          SessionInfo.SendResetEmail(usrRec, tempPassword);
          MessageDisplay.Text = "Account Password Reset Sent";
          MessageDisplay.CssClass = "successMessageDisplay";
        } catch(Exception ex) {
          MessageDisplay.Text = "Account Password Reset Failed";
          MessageDisplay.CssClass = "errorMessageDisplay";
          SessionInfo.Settings.LogError("Admin: Reset Account Password", ex);
        }
      }
    }
    protected void Account_OnDeleteCommand(object sender, GridCommandEventArgs e) {
      try {
        var contactId = (Guid)((GridDataItem)e.Item).GetDataKeyValue("Id");
        (new SystemUser()).DeleteUser(contactId.ToString());
        MessageDisplay.Text = "Account Removed";
        MessageDisplay.CssClass = "successMessageDisplay";
      } catch(Exception ex) {
        MessageDisplay.Text = "Account Remove Failed";
        MessageDisplay.CssClass = "errorMessageDisplay";
        SessionInfo.Settings.LogError("Admin: Delete Account", ex);
      }
    }
    #endregion
    #region Contact Tab
    protected void EmailContacts_OnNeedDataSource(object sender, GridNeedDataSourceEventArgs e) { ((RadGrid)sender).DataSource = SqlDatasets.GetContactList(); }
    protected void EmailContacts_OnUpdateCommand(object sender, GridCommandEventArgs e) {
      try {
        var contactId = (Guid)((GridDataItem)e.Item).GetDataKeyValue("Id");
        var editableItem = ((GridEditableItem)e.Item);
        Hashtable values = new Hashtable();
        editableItem.ExtractValues(values);
        var contact = new SystemContact(contactId.ToString()) {
          UserName = values["UserName"].ToString(),
          UserEmail = values["UserEmail"].ToString(),
          Notes = (!values["Notes"].IsNullOrEmpty()) ? values["Notes"].ToString() : string.Empty
        };
        contact.SaveContactDetails();
        MessageDisplay.Text = "Contact Updated";
        MessageDisplay.CssClass = "successMessageDisplay";
      } catch(Exception ex) {
        MessageDisplay.Text = "Contact Update Failed";
        MessageDisplay.CssClass = "errorMessageDisplay";
        SessionInfo.Settings.LogError("Admin: Update Contact", ex);
      }
    }
    protected void EmailContacts_OnInsertCommand(object sender, GridCommandEventArgs e) {
      try {
        var editableItem = ((GridEditableItem)e.Item);
        Hashtable values = new Hashtable();
        editableItem.ExtractValues(values);
        var contact = new SystemContact {
          UserName = values["UserName"].ToString(),
          UserEmail = values["UserEmail"].ToString(),
          Notes = (!values["Notes"].IsNullOrEmpty()) ? values["Notes"].ToString() : string.Empty
        };
        contact.SaveContactDetails();
        MessageDisplay.Text = "Contact Created";
        MessageDisplay.CssClass = "successMessageDisplay";
      } catch(Exception ex) {
        MessageDisplay.Text = "Contact Create Failed";
        MessageDisplay.CssClass = "errorMessageDisplay";
        SessionInfo.Settings.LogError("Admin: Create Contact", ex);
      }
    }
    protected void EmailContacts_OnDeleteCommand(object sender, GridCommandEventArgs e) {
      try {
        var contactId = (Guid)((GridDataItem)e.Item).GetDataKeyValue("Id");
        (new SystemContact()).DeleteContact(contactId.ToString());
        MessageDisplay.Text = "Contact Removed";
        MessageDisplay.CssClass = "successMessageDisplay";
      } catch(Exception ex) {
        MessageDisplay.Text = "Contact Remove Failed";
        MessageDisplay.CssClass = "errorMessageDisplay";
        SessionInfo.Settings.LogError("Admin: Delete Contact", ex);
      }
    }
    #endregion
    #region Transactions Tab
    protected void Transactions_OnNeedDataSource(object sender, GridNeedDataSourceEventArgs e) { ((RadGrid)sender).DataSource = SqlDatasets.GetAllMessages(); }
    protected void Transactions_ItemCommand(object sender, GridCommandEventArgs e) {
      if(e.Item.ItemType == GridItemType.Item || e.Item.ItemType == GridItemType.AlternatingItem) {
        var documentId = (Guid)((GridDataItem)e.Item).GetDataKeyValue("Id");
        var docAttachments = new DocumentAttachmentLinks(documentId.ToString());
        DocumentSubjectLine.Text = docAttachments.SubjectLine;
        DocumentLink.Text = string.Empty;
        docAttachments.Attachments.ForEach(att => {
          DocumentLink.Text += "<a href='{0}' target='_blank'>{1}</a><br />".FormatWith(att.FileLocation, att.FileName);
        });
        string script = "function f(){$find(\"" + modalPopup.ClientID + "\").show(); Sys.Application.remove_load(f);}Sys.Application.add_load(f);";
        ScriptManager.RegisterStartupScript(Page, Page.GetType(), "key", script, true);
      }
    }
    protected void Transactions_ItemCreated(object sender, GridItemEventArgs e) {
      if(e.Item.ItemType == GridItemType.Item || e.Item.ItemType == GridItemType.AlternatingItem) {
        var btn = (Button)((GridDataItem)e.Item)["viewDocs"].Controls[0];
        btn.Enabled = SessionInfo.CurrentUser.IsSuperAdmin;
      }
    }
    #endregion
    #region Site Details Tab
    protected void RadButton1_OnClick(object sender, EventArgs e) {
      try {
        SessionInfo.Settings.SystemName = SystemName.Text.Trim();
        SessionInfo.Settings.AdminMessage = AdminMessage.Text.Trim();
        SessionInfo.Settings.SuperAdminMessage = SuperAdminMessage.Text.Trim();
        SessionInfo.Settings.UploadMessage = UploadMessage.Text.Trim();
        SessionInfo.Settings.TransactionMessage = TransactionMessage.Text.Trim();
        SessionInfo.Settings.ThankyouMessage = ThankyouMessage.Text.Trim();
        SessionInfo.Settings.UpdateSystemSettings();
        Response.Redirect("~/Administration.aspx");
        MessageDisplay.Text = "Site Details Updated";
        MessageDisplay.CssClass = "successMessageDisplay";
      } catch(Exception ex) {
        MessageDisplay.Text = "Site Details Failed to Update";
        MessageDisplay.CssClass = "errorMessageDisplay";
        SessionInfo.Settings.LogError("Admin: Update Site Details", ex);
      }
    }
    protected void SiteDetailsTab_OnPreRender(object sender, EventArgs e) {
      SystemName.Text = SessionInfo.Settings.SystemName;
      AdminMessage.Text = SessionInfo.Settings.AdminMessage;
      SuperAdminMessage.Text = SessionInfo.Settings.SuperAdminMessage;
      UploadMessage.Text = SessionInfo.Settings.UploadMessage;
      TransactionMessage.Text = SessionInfo.Settings.TransactionMessage;
      ThankyouMessage.Text = SessionInfo.Settings.ThankyouMessage;
    }
    #endregion
    #region Email Server Tab
    protected void EmailServerTab_OnPreRender(object sender, EventArgs e) {
      ServerHost.Text = SessionInfo.Settings.MailServer;
      ServerPort.Text = SessionInfo.Settings.ServerPort.ToString();
      SmtpUser.Text = SessionInfo.Settings.SmtpUser;
      FromEmail.Text = SessionInfo.Settings.FromEmail;
      FromUsername.Text = SessionInfo.Settings.FromUsername;
      RequireAuth.Checked = SessionInfo.Settings.RequireAuth;
      RequireSSL.Checked = SessionInfo.Settings.RequireSsl;
    }
    protected void RadButton2_OnClick(object sender, EventArgs e) {
      try {
        SessionInfo.Settings.MailServer = ServerHost.Text.Trim();
        SessionInfo.Settings.ServerPort = ServerPort.Text.GetInt32();
        SessionInfo.Settings.SmtpUser = SmtpUser.Text.Trim();
        if(!SmtpPassword.Text.IsNullOrEmpty() && !SmtpPassword.Text.Trim().Equals(smtpPasswordConfirm.Text.Trim())) {
          throw new ApplicationException("New Password and Confirmation do not match");
        }
        if(!SmtpPassword.Text.IsNullOrEmpty() && SmtpPassword.Text.Trim().Length < 6) {
          throw new ApplicationException("New Password must be at least 6 characters");
        }
        if(!SmtpPassword.Text.Trim().IsNullOrEmpty()) SessionInfo.Settings.SmtpPassword = SmtpPassword.Text.Trim();
        SessionInfo.Settings.FromEmail = FromEmail.Text.Trim();
        SessionInfo.Settings.FromUsername = FromUsername.Text.Trim();
        SessionInfo.Settings.RequireAuth = RequireAuth.Checked ?? false;
        SessionInfo.Settings.RequireSsl = RequireSSL.Checked ?? false;
        SessionInfo.Settings.UpdateMailSettings();
        Response.Redirect("~/Administration.aspx");
        MessageDisplay.Text = "Email Details Updated";
        MessageDisplay.CssClass = "successMessageDisplay";
      } catch(Exception ex) {
        MessageDisplay.Text = "Email Details Failed to Update: {0}".FormatWith(ex.Message);
        MessageDisplay.CssClass = "errorMessageDisplay";
        SessionInfo.Settings.LogError("Admin: Update Email Details", ex);
      }
    }
    #endregion
    #region User Password Tab
    protected void RadButton3_OnClick(object sender, EventArgs e) {
      try {
        if(!NewPassword.Text.IsNullOrEmpty() && !NewPassword.Text.Trim().Equals(ConfirmPassword.Text.Trim())) {
          throw new ApplicationException("New Password and Confirmation do not match");
        }
        if(!NewPassword.Text.IsNullOrEmpty() && NewPassword.Text.Trim().Length < 6) {
          throw new ApplicationException("New Password must be at least 6 characters");
        }
        SessionInfo.CurrentUser.Notes = "Acct updated {0}| {1}".FormatWith(DateTime.Now.ToShortDateString(), SessionInfo.CurrentUser.Notes);
        SessionInfo.CurrentUser.SaveUserDetails();
        if(!NewPassword.Text.Trim().IsNullOrEmpty()) {
          SessionInfo.CurrentUser.SetUserPassword(SessionInfo.CurrentUser.Id, NewPassword.Text.Trim());
        }
        MessageDisplay.Text = "Your password was Updated";
        MessageDisplay.CssClass = "successMessageDisplay";
      } catch(Exception ex) {
        MessageDisplay.Text = "Your password failed to update";
        MessageDisplay.CssClass = "errorMessageDisplay";
        SessionInfo.Settings.LogError("Admin: Update Your Details", ex);
      }
    }
    protected void RadPageLayout5_OnPreRender(object sender, EventArgs e) {
      LoginId.Text = SessionInfo.CurrentUser.UserName;
      DisplayName.Text = SessionInfo.CurrentUser.DisplayName;
    }
    #endregion
    protected void RadTabStrip1_OnTabClick(object sender, RadTabStripEventArgs e) {
      MessageDisplay.Text = string.Empty;
      MessageDisplay.CssClass = string.Empty;
    }
    protected void rbLogout_OnClick(object sender, EventArgs e) { Response.Redirect("~/Logout.aspx"); }
    protected void rbPassword_OnClick(object sender, EventArgs e) {
      MessageDisplay.Text = string.Empty;
      MessageDisplay.CssClass = string.Empty;
      RadPageLayout5_OnPreRender(sender, e);
      RadMultiPage1.SelectedIndex = 6;
      RadTabStrip1.Tabs[6].Selected = true;
    }

  }
}