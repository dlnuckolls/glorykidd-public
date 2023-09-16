using GloryKidd.WebCore.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
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
      PasswordChange.OpenerElementID = rbPassword.ClientID;
      //SiteApplicationTitle.Text = SessionInfo.Settings.SystemName;
      //SiteApplicationInstructions.Text = SessionInfo.Settings.UploadMessage;
      CurrentUser.Text = $"Welcome {SessionInfo.CurrentUser.DisplayName}";
    }
    protected void rbLogout_OnClick(object sender, EventArgs e) { SessionInfo.CurrentUser.LogoutUser(); Response.Redirect("~/"); }

    protected void ConfirmChangePassword_Click(object sender, EventArgs e) {
      if(!Page.IsValid) return;
    }
  }
}