using Directory.CGBC.Helpers;
using GloryKidd.WebCore.BaseObjects;
using GloryKidd.WebCore.Helpers;
using System;
using System.Collections.Generic;
using System.Data;
using System.Dynamic;
using System.Linq;

namespace Directory.CGBC.Objects {
  public class Member {
    public int Id;
    public Salutation Salutation;
    public string PreFix;
    public string FirstName;
    public string MiddleName;
    public string LastName;
    public string Suffix;
    public Gender Gender;
    public MaritalStatus MaritalStatus;
    public DateTime MarriageDate;
    public DateTime DateOfBirth;
    public DateTime Modified;
    public DateTime Created;
    public List<Address> AddressList;

    public string DisplayName {
      get {
        var rtn = "{0} ".FormatWith(Salutation.ToString());
        rtn += PreFix.IsNullOrEmpty() ? string.Empty : "{0} ".FormatWith(PreFix);
        rtn += FirstName.IsNullOrEmpty() ? string.Empty : "{0} ".FormatWith(FirstName);
        rtn += MiddleName.IsNullOrEmpty() ? string.Empty : "{0} ".FormatWith(MiddleName);
        rtn += LastName.IsNullOrEmpty() ? string.Empty : "{0} ".FormatWith(LastName);
        rtn += Suffix.IsNullOrEmpty() ? string.Empty : "{0} ".FormatWith(Suffix);
        return rtn;
      }
    }
    public Member() { }

    public void LoadMember(int memberId) {
      try {
        var row = SqlHelpers.Select(SqlStatements.SQL_GET_SINGLE_MEMBERS.FormatWith(memberId)).Rows[0];
        Id = row["Id"].ToString().GetInt32();
        Salutation = (Salutation)Enum.Parse(typeof(Salutation), row["SalutationId"].ToString());
        PreFix = row["Prefix"].ToString();
        FirstName = row["FirstName"].ToString();
        MiddleName = row["MiddleName"].ToString();
        LastName = row["LastName"].ToString();
        Suffix = row["Suffix"].ToString();
        Gender = (Gender)Enum.Parse(typeof(Gender), row["Gender"].ToString());
        MaritalStatus = (MaritalStatus)Enum.Parse(typeof(MaritalStatus), row["MaritalStatusId"].ToString());
        MarriageDate = row["MarriageDate"].ToString().GetAsDate();
        DateOfBirth = row["DateOfBirth"].ToString().GetAsDate();
        Modified = row["ModifiedDate"].ToString().GetAsDate();
        Created = row["CreateDate"].ToString().GetAsDate();
        LoadAddress();
      } catch(Exception ex) {
        GlobalErrorLogging.LogError("Member", ex);
      }
    }

    private void LoadAddress() {
      try {
        AddressList = new List<Address>();
        var rows = SqlHelpers.Select(SqlStatements.SQL_GET_MEMBER_ADDRESSES.FormatWith(Id)).Rows;
        if(!rows.IsNullOrEmpty()) {
          foreach(DataRow row in rows) {
            AddressList.Add(new Address() {
              Id = row["AddressId"].ToString().GetInt32(),
              Address1 = row["Address1"].ToString(),
              Address2 = row["Address2"].ToString(),
              City = row["City"].ToString(),
              State = SqlDataLoader.States.FirstOrDefault(s => s.Id == row["StateId"].ToString().GetInt32()),
              ZipCode = row["Zip"].ToString(),
              IsPrimary = row["IsPriamry"].ToString().GetAsBool()
            });
          }
        }
      } catch(Exception ex) {
        GlobalErrorLogging.LogError("Member", ex);
      }
    }

  }
}