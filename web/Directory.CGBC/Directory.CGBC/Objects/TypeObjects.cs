using Directory.CGBC.Helpers;
using GloryKidd.WebCore.Helpers;
using System;
using System.Linq;

namespace Directory.CGBC.Objects {
  public class Address {
    public string Address1;
    public string Address2;
    public string City;
    public State State;
    public string ZipCode;

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

    public Address() {
      Address1 = string.Empty;
      Address2 = string.Empty;
      City = string.Empty;
      State = SqlDataLoader.States().FirstOrDefault(s => s.Id == 18);
      ZipCode = string.Empty;
    }
  }
  public class RelatedMember {
    public int Id { get; set; }
    public string DisplayName { get; set; }
    public Gender Gender { get; set; }
    public RelationshipType Relationship { get; set; }
  }
  public class MemberNote {
    public int Id;
    public DateTime NoteDate;
    public string UserName;
    public string NoteText;

    public MemberNote() {
      Id = 0;
      NoteDate = DateTime.MinValue;
      UserName = string.Empty;
      NoteText = string.Empty;
    }
  }
  public class DirectoryList {
    public int Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string CellPhone { get; set; }
    public string HomePhone { get; set; }
    public DateTime LastUpdate { get; set; }
  }
  public class EnumItemType {
    public int Id { get; set; }
    public string Name { get; set; }
  }
  public class State: EnumItemType {
    public string Abbreviation;
  }
  public class RelationshipType: EnumItemType { }
  public class Salutation: EnumItemType { }
  public class MaritalStatus: EnumItemType { }
  public class EmailAddress: EnumItemType { }
  public class MemberRelations: EnumItemType { }
  public class AdminRoles: EnumItemType { }
  public class AdminMembers: EnumItemType { }
}