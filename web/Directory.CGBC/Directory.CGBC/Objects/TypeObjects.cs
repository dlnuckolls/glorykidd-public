using GloryKidd.WebCore.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Directory.CGBC.Objects {
  public class Address {
    public int Id;
    public string Address1;
    public string Address2;
    public string City;
    public State State;
    public string ZipCode;
    public bool IsPrimary;
    public string FormattedAddress {
      get {
        var rtn = string.Empty;
        rtn += "{0}<br />".FormatWith(Address1);
        rtn += Address2.IsNullOrEmpty() ? string.Empty : "{0}<br />".FormatWith(Address2);
        rtn += City.IsNullOrEmpty() ? string.Empty : "{0}, ".FormatWith(City);
        rtn += State.Name.IsNullOrEmpty() ? string.Empty : "{0} ".FormatWith(State.Name);
        rtn += ZipCode.IsNullOrEmpty() ? string.Empty : "{0}".FormatWith(ZipCode);
        return rtn;
      }
    }

    public Address() { }
  }

  public class Phone {
    public int Id;
    public string PhoneNumber;
    public PhoneType PhoneType;
    public string FormattedPhoneNumber => PhoneNumber.FormatPhone();
    public bool IsPrimary;
  }

  public class RelatedMember {
    public int Id;
    public string DisplayName;
    public Gender Gender;
    public RelationshipType Relationship;
  }

  public class EnumItemType {
    public int Id;
    public string Name;
  }

  public class State: EnumItemType {
    public string Abbreviation;
  }

  public class PhoneType: EnumItemType { }
  public class RelationshipType: EnumItemType { }
  public class Salutation: EnumItemType { }
  public class MaritalStatus: EnumItemType { }
  public class EmailAddress: EnumItemType { }
}