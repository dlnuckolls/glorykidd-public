﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AISWeb {
  public class BasePage :System.Web.UI.Page {
    //internal PortalSession SessionInfo;

    protected override void OnInit(EventArgs e) {
      base.OnInit(e);
      LoadFromSession();
    }

    protected override void OnUnload(EventArgs e) {
      StoreInSession();
      base.OnUnload(e);
    }

    internal void LoadFromSession() {
      //if(Session["PortalSession"] == null) SessionInfo = new PortalSession();
      //else SessionInfo = (PortalSession)Session["PortalSession"];
    }

    internal void StoreInSession() {
      //Session["PortalSession"] = SessionInfo;
    }
  }
}