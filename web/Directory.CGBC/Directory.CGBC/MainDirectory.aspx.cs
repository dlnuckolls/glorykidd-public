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
    }
    protected void rbPassword_OnClick(object sender, EventArgs e) { return; }
    protected void rbLogout_OnClick(object sender, EventArgs e) { SessionInfo.CurrentUser.LogoutUser(); Response.Redirect("~/"); }
  }
}