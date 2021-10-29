using GloryKidd.WebCore.Helpers;
using System;

namespace ElusiveSoftware.net.pages {
  public partial class SoftwareServices : BasePage {
    protected void Page_Load(object sender, EventArgs e) {
      SessionInfo.CurrentPage = PageNames.Software;
      TitleTag.Text = SessionInfo.DisplayCurrentPage;
    }
  }
}