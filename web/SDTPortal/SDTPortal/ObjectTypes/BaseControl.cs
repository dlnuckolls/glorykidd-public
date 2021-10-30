using System;
using System.Web.UI;

namespace SDTPortal {
  public class BaseControl: UserControl {
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