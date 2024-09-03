using GloryKidd.WebCore.Helpers;
using System;
using System.Web.UI;

namespace Directory.CGBC {
  public partial class PrintMember: BasePage {
    protected void Page_Load(object sender, EventArgs e) {
      if(Page.IsPostBack) return;
      TitleTag.Text = SessionInfo.DisplayCurrentPage;
      var relativePath = "~/api/export/file";
      RadClientExportManager1.PdfSettings.ProxyURL = ResolveUrl(relativePath);
      RadClientExportManager1.PdfSettings.Fonts.Add("fontawesome", "http://cdnjs.cloudflare.com/ajax/libs/font-awesome/4.2.0/fonts/fontawesome-webfont.ttf");
    }

    protected void RadPdfViewer1_Load(object sender, EventArgs e) {

    }
  }
}