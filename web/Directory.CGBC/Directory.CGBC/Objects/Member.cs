using Directory.CGBC.Helpers;
using GloryKidd.WebCore.BaseObjects;
using GloryKidd.WebCore.Helpers;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Directory.CGBC.Objects {
  public class Member {
    public int Id;
    public Salutation Salutation;
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
    public Address AddressList;
    public string CellPhone;
    public string HomePhone;
    public List<RelatedMember> RelatedMembersList;
    public string Email1;
    public string Email2;
    public List<MemberNote> MemberNotes;

    public string DisplayName {
      get {
        var rtn = "{0} ".FormatWith(Salutation.Name);
        rtn += FirstName.IsNullOrEmpty() ? string.Empty : "{0} ".FormatWith(FirstName);
        rtn += MiddleName.IsNullOrEmpty() ? string.Empty : "{0} ".FormatWith(MiddleName);
        rtn += LastName.IsNullOrEmpty() ? string.Empty : "{0} ".FormatWith(LastName);
        rtn += Suffix.IsNullOrEmpty() ? string.Empty : "{0} ".FormatWith(Suffix);
        return rtn;
      }
    }
    public Member() {
      Id = 0;
      Salutation = SqlDataLoader.Salutations().FirstOrDefault();
      FirstName = string.Empty;
      MiddleName = string.Empty;
      LastName = string.Empty;
      Suffix = string.Empty;
      Gender = Gender.Male;
      MaritalStatus = SqlDataLoader.MaritalStatuses().FirstOrDefault();
      MarriageDate = DateTime.MinValue;
      DateOfBirth = DateTime.MinValue;
      Modified = DateTime.MinValue;
      Created = DateTime.MinValue;
      AddressList = new Address();
      CellPhone = string.Empty;
      HomePhone = string.Empty;
      RelatedMembersList = new List<RelatedMember>();
      Email1 = string.Empty;
      Email2 = string.Empty;
      MemberNotes = new List<MemberNote>();
    }
    #region Load Member Data 
    public void LoadMember(int memberId) {
      try {
        var row = SqlHelpers.Select(SqlStatements.SQL_GET_SINGLE_MEMBERS.FormatWith(memberId)).Rows[0];
        Id = row["Id"].ToString().GetInt32();
        Salutation = SqlDataLoader.Salutations().FirstOrDefault(s => s.Id == row["SalutationId"].ToString().GetInt32());
        FirstName = row["FirstName"].ToString();
        MiddleName = row["MiddleName"].ToString();
        LastName = row["LastName"].ToString();
        Suffix = row["Suffix"].ToString();
        Gender = (row["Gender"].ToString().GetInt32() == 0) ? Gender.Male : Gender.Female;
        MaritalStatus = SqlDataLoader.MaritalStatuses().FirstOrDefault(ms => ms.Id == row["MaritalStatusId"].ToString().GetInt32());
        MarriageDate = row["MarriageDate"].ToString().GetAsDate() == Convert.ToDateTime("1900-01-01") ? DateTime.MinValue : row["MarriageDate"].ToString().GetAsDate();
        DateOfBirth = row["DateOfBirth"].ToString().GetAsDate() == Convert.ToDateTime("1900-01-01") ? DateTime.MinValue : row["DateOfBirth"].ToString().GetAsDate();
        Modified = row["ModifiedDate"].ToString().GetAsDate();
        Created = row["CreateDate"].ToString().GetAsDate();
        AddressList = new Address() {
          Address1 = row["Address1"].ToString(),
          Address2 = row["Address2"].ToString(),
          City = row["City"].ToString(),
          State = SqlDataLoader.States().FirstOrDefault(s => s.Id == row["StateId"].ToString().GetInt32()),
          ZipCode = row["Zip"].ToString()
        };
        CellPhone = row["CellPhone"].ToString();
        HomePhone = row["HomePhone"].ToString();
        Email1 = row["EmailAddress1"].ToString();
        Email2 = row["EmailAddress2"].ToString();
        LoadRelations();
        LoadNotes();
      } catch(Exception ex) {
        GlobalErrorLogging.LogError("Member", ex);
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
          _relatedMember.DisplayName = "{0} ".FormatWith(SqlDataLoader.Salutations().FirstOrDefault(s => s.Id == row["SalutationId"].ToString().GetInt32()).Name);
          _relatedMember.DisplayName += row["FirstName"].IsNullOrEmpty() ? string.Empty : "{0} ".FormatWith(row["FirstName"].ToString());
          _relatedMember.DisplayName += row["MiddleName"].IsNullOrEmpty() ? string.Empty : "{0} ".FormatWith(row["MiddleName"].ToString());
          _relatedMember.DisplayName += row["LastName"].IsNullOrEmpty() ? string.Empty : "{0} ".FormatWith(row["LastName"].ToString());
          _relatedMember.DisplayName += row["Suffix"].IsNullOrEmpty() ? string.Empty : "{0} ".FormatWith(row["Suffix"].ToString());
          _relatedMember.Relationship = SqlDataLoader.RelationshipTypes().FirstOrDefault(r => r.Id == row["RelationshipTypeId"].ToString().GetInt32());
          RelatedMembersList.Add(_relatedMember);
        }
      }
    }
    private void LoadNotes() {
      MemberNotes = new List<MemberNote>();
      var rows = SqlHelpers.Select(SqlStatements.SQL_GET_MEMBER_NOTES.FormatWith(Id)).Rows;
      if(!rows.IsNullOrEmpty()) {
        foreach(DataRow row in rows) {
          MemberNotes.Add(new MemberNote() {
            Id = row["Id"].ToString().GetInt32(),
            UserName = row["DisplayName"].ToString(),
            NoteDate = row["CreateDate"].ToString().GetAsDate(),
            NoteText = row["Notes"].ToString()
          });
        }
      }
    }
    #endregion
    public void SaveMember(string memberNote, int userId) {
      SqlDataLoader.SaveMember(this, memberNote, userId);
      SqlDataLoader.SaveMemberRelations(this);
    }
  }
}