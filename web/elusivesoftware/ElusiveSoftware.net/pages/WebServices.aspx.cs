using GloryKidd.WebCore.Helpers;
using System;

namespace ElusiveSoftware.net.pages {
  public partial class WebServices : BasePage {
    protected void Page_Load(object sender, EventArgs e) {
      SessionInfo.CurrentPage = PageNames.Websites;
      TitleTag.Text = SessionInfo.DisplayCurrentPage;
    }
  }
}