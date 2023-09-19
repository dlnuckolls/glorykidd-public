using GloryKidd.WebCore.BaseObjects;
using GloryKidd.WebCore.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using static Telerik.Web.UI.GridHeaderContextMenu.GridContextFilterTemplate;

namespace Directory.CGBC.Objects {
  public class Address {
    public int Id;
    public string Address1;
    public string Address2;
    public string City;
    public int StateId;
    public string State;
    public string Abbreviation;
    public string ZipCode;
    public bool IsPrimary;

    public Address() { }

  }
}