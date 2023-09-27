using GloryKidd.WebCore.BaseObjects;
using System.Net;
using System.Net.Mail;

namespace GloryKidd.WebCore.Helpers {
  public class SessionManager {
    #region Sitewide Details
    /// <summary>
    /// System Settings Instance
    /// </summary>
    public SystemSettings Settings => SystemSettings.StaticInstance;
    #endregion

    #region Page Header Titles
    /// <summary>
    /// Tracking for current page within the application
    /// </summary>
    public PageNames CurrentPage { get; set; }
    public string DisplayCurrentPage => "{0}".FormatWith(CurrentPage.TextValue());
    #endregion

    #region Current Member
    public object CurrentMember;
    #endregion

    #region Current User Details
    /// <summary>
    /// System User Instance
    /// </summary>
    public SystemUser CurrentUser => SystemUser.StaticInstance;
    /// <summary>
    /// Authentication Indicator
    /// </summary>
    public bool IsAuthenticated => CurrentUser.IsAuthenticated;
    /// <summary>
    /// Administration User Setting
    /// </summary>
    public bool IsAdmin => CurrentUser.IsAdmin;
    #endregion

    #region Email Settings
    /// <summary>
    /// Set all settings for SMTP service calls
    /// </summary>
    /// <returns></returns>
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
    /// <summary>
    /// Send a password reset email to users
    /// </summary>
    /// <param name="user"></param>
    /// <param name="tempPassword"></param>
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
    /// <summary>
    /// Send a pre-generated email message to the user
    /// </summary>
    /// <param name="msg"></param>
    public void SendContactEmail(ref MailMessage msg) {
      SmtpClient smtp = SetMailServerSettings();
      smtp.Send(msg);
    }
    #endregion
  }
}