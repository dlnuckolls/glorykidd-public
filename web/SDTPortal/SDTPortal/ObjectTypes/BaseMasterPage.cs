using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SDTPortal {
  public class BaseMasterPage :System.Web.UI.MasterPage {
    internal PortalSession SessionInfo;

    protected override void OnInit(EventArgs e) {
      base.OnInit(e);
      LoadFromSession();
    }

    protected override void OnUnload(EventArgs e) {
      StoreInSession();
      base.OnUnload(e);
    }

    internal void LoadFromSession() {
      if(Session["PortalSession"] == null) SessionInfo = new PortalSession();
      else SessionInfo = (PortalSession)Session["PortalSession"];
    }

    internal void StoreInSession() {
      Session["PortalSession"] = SessionInfo;
    }
  }
}