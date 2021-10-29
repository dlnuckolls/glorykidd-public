using GloryKidd.WebCore.Helpers;
using System;
using System.Net.Mail;
using Telerik.Web.UI;

namespace ElusiveSoftware.net.pages {
  public partial class Contact : BasePage {
    protected void Page_Load(object sender, EventArgs e) {
      // Set page name in the title section
      SessionInfo.CurrentPage = PageNames.Contact;
      TitleTag.Text = SessionInfo.DisplayCurrentPage;

      RadCaptcha1.CaptchaImage.LineNoise = CaptchaLineNoiseLevel.Low;
      RadCaptcha1.CaptchaImage.FontWarp = CaptchaFontWarpFactor.Low;
      RadCaptcha1.MinTimeout = 15;
      RadCaptcha1.ProtectionMode = RadCaptcha.ProtectionStrategies.Captcha;
      RadCaptcha1.CaptchaImage.TextLength = 6;
      RadCaptcha1.CaptchaImage.FontFamily = "Verdana";
      RadCaptcha1.CaptchaImage.TextColor = System.Drawing.Color.FromName("Green");
      RadCaptcha1.CaptchaImage.BackgroundColor = System.Drawing.Color.FromName("White");
      RadCaptcha1.CaptchaImage.BackgroundNoise = CaptchaBackgroundNoiseLevel.Low;

      SendEmails.Visible = true;
      ThanksMessage.Visible = false;
    }

    protected void SendMessage_Click(object sender, EventArgs e) {
      if (!Page.IsValid) return;
      //_ = SqlHelpers.Insert(SqlStatements.SQL_INSERT_NEWSLETTER_ADDRESS.FormatWith(ContactName.Text.Trim().FixSqlString(), ContactEmail.Text.Trim().FixSqlString()));
      var msg = new MailMessage {
        IsBodyHtml = false,
        From = new MailAddress(ContactEmail.Text.Trim(), ContactName.Text.Trim()),
        Subject = ContactSubject.Text.Trim(),
        Body = ContactMessage.Text.Trim()
      };
      msg.To.Add(new MailAddress("dlnuckolls@elusivesoftware.com", "Elusive Software Contact"));
      SessionInfo.SendContactEmail(ref msg);
      SendEmails.Visible = false;
      ThanksMessage.Visible = true;
    }
  }
}