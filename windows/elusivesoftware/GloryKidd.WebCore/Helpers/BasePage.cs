﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;

namespace GloryKidd.WebCore.Helpers {
  public class BasePage: Page {
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