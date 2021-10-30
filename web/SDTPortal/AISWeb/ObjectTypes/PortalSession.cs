
using System.Net;
using System.Net.Mail;
namespace AISWeb {
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
      if (CurrentUser.IsNullOrEmpty() || EmailDocument.IsNullOrEmpty()) return;
      var mail = new MailMessage {
        From = new MailAddress(CurrentUser.UserName, CurrentUser.DisplayName),
        IsBodyHtml = true,
        Subject = "Documents Submitted",
        Body = "{1} has sent file(s) through the {0}.<br /><br />{2}".FormatWith(Settings.SystemName, CurrentUser.DisplayName, EmailDocument.SubjectLine)
      };

      var contact = new SystemContact(EmailDocument.ToEmail);
      mail.To.Add(new MailAddress(contact.UserEmail));
      EmailDocument.Attachments.ForEach(doc => { mail.Attachments.Add(new Attachment(doc.InputStream, doc.GetName())); });
      SmtpClient smtp = SetMailServerSettings();
      smtp.Send(mail);
    }

    private SmtpClient SetMailServerSettings() {
      var smtp = new SmtpClient {
        Port = Settings.ServerPort,
        EnableSsl = Settings.RequireSsl,
        DeliveryMethod = SmtpDeliveryMethod.Network,
        Host = Settings.MailServer
      };
      if (Settings.RequireAuth) {
        smtp.UseDefaultCredentials = false;
        smtp.Credentials = new NetworkCredential(Settings.SmtpUser, Settings.SmtpPassword);
      }
      return smtp;
    }

    public void SendResetEmail(SystemUser user, string tempPassword) {
      var mail = new MailMessage {
        From = new MailAddress(Settings.FromEmail, Settings.FromUsername),
        IsBodyHtml = true,
        Subject = "Password Reset Confirmation",
        Body = "Your new password has been set to: {0}<br /><br />Please login to change your password to something you can remember.<br /><br />Thanks,<br />{1}".FormatWith(tempPassword, Settings.FromUsername)
      };
      mail.To.Add(new MailAddress(user.UserName));
      SmtpClient smtp = SetMailServerSettings();
      smtp.Send(mail);
    }

  }
}