using GloryKidd.WebCore.Helpers;
using System;

namespace ElusiveSoftware.net.pages {
  public partial class OurPlans : BasePage {
    protected void Page_Load(object sender, EventArgs e) {
      SessionInfo.CurrentPage = PageNames.Plans;
      TitleTag.Text = SessionInfo.DisplayCurrentPage;
    }
  }
}