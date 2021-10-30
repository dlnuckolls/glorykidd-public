using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace AISWeb {
  public partial class Default : BasePage {
    protected void Page_Load(object sender, EventArgs e) {
      TitleTag.Text = "<title>AIS Web</title>";
    }
    protected void SdpApplication_OnClick(object sender, EventArgs e) { Response.Redirect("https://sdp.aisweb.com"); }
    protected void AisRetirement_OnClick(object sender, EventArgs e) { Response.Redirect("http://www.aisretirement.com"); }
  }
}