using GloryKidd.WebCore.Helpers;
using System;

namespace ElusiveSoftware.net {
  public partial class Default : BasePage {
    protected void Page_Load(object sender, EventArgs e) {
      SessionInfo.CurrentPage = PageNames.Home;
      TitleTag.Text = SessionInfo.DisplayCurrentPage;
    }
  }
}