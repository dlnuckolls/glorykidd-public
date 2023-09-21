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
    public List<Phone> PhoneList;
    public List<RelatedMember> RelatedMembersList;

    public string DisplayName {
      get {
        var rtn = "{0} ".FormatWith(Salutation.Name);
        rtn += PreFix.IsNullOrEmpty() ? string.Empty : "{0} ".FormatWith(PreFix);
        rtn += FirstName.IsNullOrEmpty() ? string.Empty : "{0} ".FormatWith(FirstName);
        rtn += MiddleName.IsNullOrEmpty() ? string.Empty : "{0} ".FormatWith(MiddleName);
        rtn += LastName.IsNullOrEmpty() ? string.Empty : "{0} ".FormatWith(LastName);
        rtn += Suffix.IsNullOrEmpty() ? string.Empty : "{0} ".FormatWith(Suffix);
        return rtn;
      }
    }
    public string PrimaryAddress {
      get {
        var address = AddressList.IsNullOrEmpty() || AddressList.Count == 0 ? new Address() : AddressList.FirstOrDefault(a => a.IsPrimary == true);
        return AddressList.Count == 0 ? string.Empty : address.FormattedAddress;
      }
    }

    public string PrimaryPhone {
      get {
        var phone = PhoneList.IsNullOrEmpty() || PhoneList.Count == 0 ? new Phone() : PhoneList.FirstOrDefault(p => p.IsPrimary == true);
        return PhoneList.Count == 0 ? string.Empty : phone.FormattedPhoneNumber;
      }
    }

    public Member() { }

    #region Load Member Data 
    public void LoadMember(int memberId) {
      try {
        var row = SqlHelpers.Select(SqlStatements.SQL_GET_SINGLE_MEMBERS.FormatWith(memberId)).Rows[0];
        Id = row["Id"].ToString().GetInt32();
        Salutation = SqlDataLoader.Salutations.FirstOrDefault(s => s.Id == row["SalutationId"].ToString().GetInt32());
        PreFix = row["Prefix"].ToString();
        FirstName = row["FirstName"].ToString();
        MiddleName = row["MiddleName"].ToString();
        LastName = row["LastName"].ToString();
        Suffix = row["Suffix"].ToString();
        Gender = (row["Gender"].ToString().GetInt32() == 0) ? Gender.Male : Gender.Female;
        MaritalStatus = SqlDataLoader.MaritalStatuses.FirstOrDefault(ms => ms.Id == row["MaritalStatusId"].ToString().GetInt32());
        MarriageDate = row["MarriageDate"].ToString().GetAsDate();
        DateOfBirth = row["DateOfBirth"].ToString().GetAsDate();
        Modified = row["ModifiedDate"].ToString().GetAsDate();
        Created = row["CreateDate"].ToString().GetAsDate();
        LoadAddress();
        LoadPhone();
        LoadRelations();
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
              IsPrimary = row["IsPrimary"].ToString().GetAsBool()
            });
          }
        }
      } catch(Exception ex) {
        GlobalErrorLogging.LogError("Member", ex);
      }
    }

    private void LoadPhone() {
      PhoneList = new List<Phone>();
      var rows = SqlHelpers.Select(SqlStatements.SQL_GET_MEMBER_PHONES.FormatWith(Id)).Rows;
      if(!rows.IsNullOrEmpty()) {
        foreach(DataRow row in rows) {
          PhoneList.Add(new Phone() {
            Id = row["PhoneId"].ToString().GetInt32(),
            PhoneNumber = row["Phone"].ToString(),
            PhoneType = SqlDataLoader.PhoneTypes.FirstOrDefault(p => p.Id == row["TypeId"].ToString().GetInt32()),
            IsPrimary = row["IsPrimary"].ToString().GetAsBool()
          });
        }
      }
    }

    private void LoadRelations() {
      RelatedMembersList = new List<RelatedMember>();
      var rows = SqlHelpers.Select(SqlStatements.SQL_GET_MEMBER_RELATIONS.FormatWith(Id)).Rows;
      if(!rows.IsNullOrEmpty()) {
        RelatedMember _relatedMember = new RelatedMember();
        foreach(DataRow row in rows) {
          _relatedMember = new RelatedMember() {
            Id = row["id"].ToString().GetInt32(),
            Gender = (Gender)Enum.Parse(typeof(Gender), row["Gender"].ToString())
          };
          _relatedMember.DisplayName = "{0} ".FormatWith(SqlDataLoader.Salutations.FirstOrDefault(s => s.Id == row["SalutationId"].ToString().GetInt32()).Name);
          _relatedMember.DisplayName += row["Prefix"].IsNullOrEmpty() ? string.Empty : "{0} ".FormatWith(row["Prefix"].ToString());
          _relatedMember.DisplayName += row["FirstName"].IsNullOrEmpty() ? string.Empty : "{0} ".FormatWith(row["FirstName"].ToString());
          _relatedMember.DisplayName += row["MiddleName"].IsNullOrEmpty() ? string.Empty : "{0} ".FormatWith(row["MiddleName"].ToString());
          _relatedMember.DisplayName += row["LastName"].IsNullOrEmpty() ? string.Empty : "{0} ".FormatWith(row["LastName"].ToString());
          _relatedMember.DisplayName += row["Suffix"].IsNullOrEmpty() ? string.Empty : "{0} ".FormatWith(row["Suffix"].ToString());
          _relatedMember.Relationship = SqlDataLoader.RelationshipTypes.FirstOrDefault(r => r.Id == row["RelationshipTypeId"].ToString().GetInt32());
          RelatedMembersList.Add(_relatedMember);
        }
      }
    }
    #endregion

  }
}