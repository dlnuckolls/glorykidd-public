using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SDTPortal {
  public partial class UserNav :BaseControl {
    public void DisableControls() {
      RadAccountMenu.Enabled = false;
    }

    protected void Page_Load(object sender, EventArgs e) {
      switch(SessionInfo.CurrentPage) {
        case PageNames.Uploads:
          RadAccountMenu.Items[0].Selected = true;
          break;
        case PageNames.Transactions:
          RadAccountMenu.Items[2].Selected = true;
          break;
      }
    }
  }
}