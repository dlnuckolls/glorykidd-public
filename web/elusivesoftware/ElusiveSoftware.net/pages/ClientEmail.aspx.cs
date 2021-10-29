using GloryKidd.WebCore.Helpers;
using System;
using System.Net.Mail;
using Telerik.Web.UI;

namespace ElusiveSoftware.net.pages {
  public partial class ClientEmail : BasePage {
    protected void Page_Load(object sender, EventArgs e) {

      RadCaptcha1.CaptchaImage.LineNoise = CaptchaLineNoiseLevel.Low;
      RadCaptcha1.CaptchaImage.FontWarp = CaptchaFontWarpFactor.Low;
      RadCaptcha1.MinTimeout = 15;
      RadCaptcha1.ProtectionMode = RadCaptcha.ProtectionStrategies.Captcha;
      RadCaptcha1.CaptchaImage.TextLength = 6;
      RadCaptcha1.CaptchaImage.FontFamily = "Verdana";
      RadCaptcha1.CaptchaImage.TextColor = System.Drawing.Color.FromName("Green");
      RadCaptcha1.CaptchaImage.BackgroundColor = System.Drawing.Color.FromName("White");
      RadCaptcha1.CaptchaImage.BackgroundNoise = CaptchaBackgroundNoiseLevel.Low;

      if (Request.QueryString["client"] != null && Request.QueryString["client"].Equals("vteng")) {
        SendEmails.Visible = false;
        Varitech.Visible = true;
      } else {
        SendEmails.Visible = true;
        Varitech.Visible = false;
      }
      ThanksMessage.Visible = false;
    }

    protected void SendMessage_Click(object sender, EventArgs e) {
      if (((RadButton)sender).CommandArgument == "vteng") {
        if (!Page.IsValid) return;
        var txt = "<h3>You have a new email from Varitech's website</h3>";
        txt += "<p>User: {0}<br />".FormatWith(vName.Text);
        txt += "Phone: {0}<br />".FormatWith(vPhone.Text);
        txt += "Email: {0}<br />".FormatWith(vEmail.Text);
        txt += "Address: {0}<br />".FormatWith(vAddress.Text);
        if (vCBList.Items[0].Selected) { txt += "  I am interested in learning more about Generator Systems <br />"; }
        if (vCBList.Items[1].Selected) { txt += "  I am interested in learning more about CNG / Compressed Natural Gas <br />"; }
        if (vCBList.Items[2].Selected) { txt += "  I am interested in learning more about Other Topics <br />"; }
        txt += "Message: {0}</p>".FormatWith(vMessage.Text);
        txt += "<p>Thanks,<br /> Vteng Website </p>";

        var msg = new MailMessage {
          IsBodyHtml = true,
          From = new MailAddress(vEmail.Text.Trim(), vName.Text.Trim()),
          Subject = "Contact Email from www.vteng.com",
          Body = txt
        };
        msg.To.Add(new MailAddress("gregb@vteng.com", "Varitech Contact"));
        SessionInfo.SendContactEmail(ref msg);
      }
      SendEmails.Visible = false;
      Varitech.Visible = false;
      ThanksMessage.Visible = true;
    }
  }
}