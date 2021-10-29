using GloryKidd.WebCore.Helpers;
using System;

namespace ElusiveSoftware.net.pages {
  public partial class Consulting : BasePage {
    protected void Page_Load(object sender, EventArgs e) {
      SessionInfo.CurrentPage = PageNames.Consulting;
      TitleTag.Text = SessionInfo.DisplayCurrentPage;
    }
  }
}