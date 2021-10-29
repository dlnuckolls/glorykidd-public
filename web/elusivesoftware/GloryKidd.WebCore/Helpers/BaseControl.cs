using System;
using System.Web.UI;

namespace GloryKidd.WebCore.Helpers {
  public class BaseControl: UserControl {
    public SessionManager SessionInfo;

    protected override void OnInit(EventArgs e) {
      base.OnInit(e);
      LoadFromSession();
    }

    protected override void OnUnload(EventArgs e) {
      StoreInSession();
      base.OnUnload(e);
    }

    internal void LoadFromSession() {
      if(Session["PortalSession"] == null) SessionInfo = new SessionManager();
      else SessionInfo = (SessionManager)Session["PortalSession"];
    }

    internal void StoreInSession() {
      Session["PortalSession"] = SessionInfo;
    }
  }
}