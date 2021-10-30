using System;
using System.IO;
using System.Net;
using System.Net.Mail;

namespace SDTPortal {
  internal class PortalSession {
    // System settings and properties
    public SystemSettings Settings => SystemSettings.StaticInstance;

    // Authentication indicator and user record
    public SystemUser CurrentUser { get; set; }
    public bool IsAuthenticated => CurrentUser.IsAuthenticated;
    public bool IsAdmin => CurrentUser.IsAdmin;

    // Tracking for current page within the application
    public PageNames CurrentPage { get; set; }
    public string DisplayCurrentPage => "<title>{0}</title>".FormatWith(CurrentPage.TextValue());

    // Document with Attachment(s)
    public DocumentSubmission EmailDocument { get; set; }

    public void SubmitEmail() {
      if(CurrentUser.IsNullOrEmpty() || EmailDocument.IsNullOrEmpty()) return;
      try {
        var mail = new MailMessage {
          From = new MailAddress(Settings.FromEmail, Settings.FromUsername),
          IsBodyHtml = true,
          Subject = "AIS Secure Data Portal - New Transmission Recieved",
          Body = "{1} has sent file(s) through the {0}.<br /><br />{2}".FormatWith(Settings.SystemName, CurrentUser.DisplayName, EmailDocument.SubjectLine)
        };
        var contact = new SystemContact(EmailDocument.ToEmail);
        mail.ReplyToList.Add(new MailAddress(CurrentUser.UserName, CurrentUser.DisplayName));
        mail.To.Add(new MailAddress(contact.UserEmail,contact.UserName));
        EmailDocument.SavedDocs.ForEach(sd => { mail.Attachments.Add(new Attachment(new FileStream(sd.FileLocation, FileMode.Open), sd.FileName)); });
        SmtpClient smtp = SetMailServerSettings();
        smtp.Send(mail);
      } catch(Exception ex) {
        Settings.LogError("SubmitEmail: Send Email", ex, EmailDocument.Id);
      }
    }

    private SmtpClient SetMailServerSettings() {
      var smtp = new SmtpClient {
        Port = Settings.ServerPort,
        EnableSsl = Settings.RequireSsl,
        DeliveryMethod = SmtpDeliveryMethod.Network,
        Host = Settings.MailServer
      };
      if(Settings.RequireAuth) {
        smtp.UseDefaultCredentials = false;
        smtp.Credentials = new NetworkCredential(Settings.SmtpUser, Settings.SmtpPassword);
      }
      return smtp;
    }

    public void SendResetEmail(SystemUser user, string tempPassword) {
      var mail = new MailMessage {
        From = new MailAddress(Settings.FromEmail, Settings.FromUsername),
        IsBodyHtml = true,
        Subject = "Alexander Investment Services Secure Data Portal - Password Reset Confirmation",
        Body = "Your new password has been set to: {0}<br /><br />Please login to change your password to something you can remember.<br /><br />Thank You,<br />{1}".FormatWith(tempPassword, Settings.FromUsername)
      };
      mail.To.Add(new MailAddress(user.UserName));
      SmtpClient smtp = SetMailServerSettings();
      smtp.Send(mail);
    }

  }
}