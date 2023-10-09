using Directory.CGBC.Objects;
using GloryKidd.WebCore.Helpers;
using System.Collections.Generic;
using System.Data;

namespace Directory.CGBC.Helpers {
  public static class SqlDataLoader {

    private static List<State> _stateList = null;
    private static List<RelationshipType> _relationshipTypeList = null;
    private static List<Salutation> _salutationList = null;
    private static List<MaritalStatus> _maritalStatusList = null;
    private static List<MemberRelations> _memberRelations = null;

    public static List<State> States() => _stateList.IsNullOrEmpty() || _stateList.Count == 0 ? _stateList = GetStates() : _stateList;
    public static List<RelationshipType> RelationshipTypes() => _relationshipTypeList.IsNullOrEmpty() || _relationshipTypeList.Count == 0 ? _relationshipTypeList = GetRelationshipTypes() : _relationshipTypeList;
    public static List<MemberRelations> AllMemberRelations() => _memberRelations.IsNullOrEmpty() || _memberRelations.Count == 0 ? _memberRelations = GetMemberRelations() : _memberRelations;
    public static List<Salutation> Salutations() => _salutationList.IsNullOrEmpty() || _salutationList.Count == 0 ? _salutationList = GetSalutations() : _salutationList;
    public static List<MaritalStatus> MaritalStatuses() => _maritalStatusList.IsNullOrEmpty() || _maritalStatusList.Count == 0 ? _maritalStatusList = GetMaritalStatuses() : _maritalStatusList;

    public static void SaveMember(Member member, string memberNote, int userId) {
      var queryString = string.Empty;
      if(member.Id == 0) {
        queryString = SqlStatements.SQL_SAVE_MEMBER.FormatWith(member.Salutation.Id, member.FirstName.FixSqlString(), member.MiddleName.FixSqlString(),
          member.LastName.FixSqlString(), member.Suffix.FixSqlString(), member.Gender == Gender.Female ? 1 : 0, member.AddressList.Address1.FixSqlString(), member.AddressList.Address2.FixSqlString(),
          member.AddressList.City.FixSqlString(), member.AddressList.State.Id, member.AddressList.ZipCode.FixSqlZip(), member.CellPhone.FixSqlPhone(), member.HomePhone.FixSqlPhone(),
          member.Email1.FixSqlString(), member.Email2.FixSqlString(), member.MaritalStatus.Id, member.DateOfBirth.ConvertSqlDate(), member.MarriageDate.ConvertSqlDate());
        member.Id = SqlHelpers.SelectScalar(queryString).ToString().GetInt32();
        if(!memberNote.IsNullOrEmpty()) {
          queryString = SqlStatements.SQL_SAVE_MEMBER_NOTE.FormatWith(member.Id, memberNote.FixSqlString(), userId);
          SqlHelpers.Insert(queryString);
        }
      } else {
        queryString = SqlStatements.SQL_UPDATE_MEMBER.FormatWith(member.Salutation.Id, member.FirstName.FixSqlString(), member.MiddleName.FixSqlString(),
          member.LastName.FixSqlString(), member.Suffix.FixSqlString(), member.Gender == Gender.Female ? 1 : 0, member.AddressList.Address1.FixSqlString(), member.AddressList.Address2.FixSqlString(),
          member.AddressList.City.FixSqlString(), member.AddressList.State.Id, member.AddressList.ZipCode.FixSqlZip(), member.CellPhone.FixSqlPhone(), member.HomePhone.FixSqlPhone(),
          member.Email1.FixSqlString(), member.Email2.FixSqlString(), member.MaritalStatus.Id, member.DateOfBirth.ConvertSqlDate(), member.MarriageDate.ConvertSqlDate(), member.Id);
        SqlHelpers.Update(queryString);
        if(!memberNote.IsNullOrEmpty()) {
          queryString = SqlStatements.SQL_SAVE_MEMBER_NOTE.FormatWith(member.Id, memberNote.FixSqlString(), userId);
          SqlHelpers.Insert(queryString);
        }
      }
    }
    public static void SaveMemberRelations(Member member) {
      //Remove Existing Relations
      var queryString = string.Empty;
      queryString = SqlStatements.SQL_REMOVE_MEMBER_RELATIONS.FormatWith(member.Id);
      SqlHelpers.Update(queryString);
      //Add Relations
      member.RelatedMembersList.ForEach(r => {
        //Add Main relation
        queryString = SqlStatements.SQL_SAVE_MEMBER_RELATIONS.FormatWith(member.Id, r.Id, r.Relationship.Id);
        SqlHelpers.Insert(queryString);
        switch(r.Relationship.Id) {
          case 1: //Spouse
          case 3: //Sibling
            queryString = SqlStatements.SQL_SAVE_MEMBER_RELATIONS.FormatWith(r.Id, member.Id, r.Relationship.Id);
            SqlHelpers.Insert(queryString);
            break;
          case 2: //Child
            queryString = SqlStatements.SQL_SAVE_MEMBER_RELATIONS.FormatWith(r.Id, member.Id, 4); //Was Child, Add Parent
            SqlHelpers.Insert(queryString);
            break;
          case 4: //Parent
            queryString = SqlStatements.SQL_SAVE_MEMBER_RELATIONS.FormatWith(r.Id, member.Id, 2); //Was Parent, Add Child
            SqlHelpers.Insert(queryString);
            break;
        }
      });
    }
    public static List<DirectoryList> GetDirectoryList() {
      var list = new List<DirectoryList>();
      var rows = SqlHelpers.Select(SqlStatements.SQL_GET_ALL_MEMBERS).Rows;
      foreach(DataRow row in rows) {
        list.Add(new DirectoryList() {
          Id = row["Id"].ToString().GetInt32(),
          FirstName = row["FirstName"].ToString(),
          LastName = row["LastName"].ToString(),
          CellPhone = row["CellPhone"].ToString().FormatPhone(),
          HomePhone = row["HomePhone"].ToString().FormatPhone(),
          LastUpdate = row["ModifiedDate"].ToString().GetAsDate()
        });
      }
      return list;
    }
    public static DataTable GetAdminUsers() {
      return SqlHelpers.Select(SqlStatements.SQL_GET_ALL_USERS);
    }
    public static List<AdminRoles> GetAdminRoles() {
      var list = new List<AdminRoles>();
      var rows = SqlHelpers.Select(SqlStatements.SQL_GET_ALL_ROLES).Rows;
      foreach(DataRow row in rows) {
        list.Add(new AdminRoles() {
          Id = row["Id"].ToString().GetInt32(),
          Name = row["RoleName"].ToString()
        });
      }
      return list;
    }
    public static List<AdminMembers> GetUserMembers() {
      var members = new List<AdminMembers>();
      members.Add(new AdminMembers() {
        Id = 0,
        Name = "--Select--"
      });
      var rows = SqlHelpers.Select(SqlStatements.SQL_GET_ALL_USER_MEMBERS).Rows;
      foreach(DataRow row in rows) {
        members.Add(new AdminMembers() {
          Id = row["Id"].ToString().GetInt32(),
          Name = "{0} {1}".FormatWith(row["FirstName"].ToString(), row["LastName"].ToString())
        });
      }
      return members;
    }
    public static void ReloadMemberRelations() { if(!_memberRelations.IsNullOrEmpty()) _memberRelations.Clear(); }
    private static List<State> GetStates() {
      var states = new List<State>();
      var rows = SqlHelpers.Select(SqlStatements.SQL_GET_STATES).Rows;
      foreach(DataRow row in rows) {
        states.Add(new State() {
          Id = row["Id"].ToString().GetInt32(),
          Name = row["State"].ToString(),
          Abbreviation = row["Abbreviation"].ToString()
        });
      }
      return states;
    }
    private static List<RelationshipType> GetRelationshipTypes() {
      var relationshipTypes = new List<RelationshipType>();
      var rows = SqlHelpers.Select(SqlStatements.SQL_GET_RELATIONSHIPTYPES).Rows;
      foreach(DataRow row in rows) {
        relationshipTypes.Add(new RelationshipType() {
          Id = row["Id"].ToString().GetInt32(),
          Name = row["RelationshipType"].ToString()
        });
      }
      return relationshipTypes;
    }
    private static List<Salutation> GetSalutations() {
      var salutations = new List<Salutation>();
      var rows = SqlHelpers.Select(SqlStatements.SQL_GET_SALUTATIONS).Rows;
      foreach(DataRow row in rows) {
        salutations.Add(new Salutation() {
          Id = row["Id"].ToString().GetInt32(),
          Name = row["Salutation"].ToString()
        });
      }
      return salutations;
    }
    private static List<MaritalStatus> GetMaritalStatuses() {
      var maritalStatuses = new List<MaritalStatus>();
      var rows = SqlHelpers.Select(SqlStatements.SQL_GET_MARITALSTATUSES).Rows;
      foreach(DataRow row in rows) {
        maritalStatuses.Add(new MaritalStatus() {
          Id = row["Id"].ToString().GetInt32(),
          Name = row["MaritalStatus"].ToString()
        });
      }
      return maritalStatuses;
    }
    private static List<MemberRelations> GetMemberRelations() {
      var memberRelations = new List<MemberRelations>();
      var rows = SqlHelpers.Select(SqlStatements.SQL_GET_MEMBER_RELATION_LIST).Rows;
      if(!rows.IsNullOrEmpty()) {
        MemberRelations _relatedMember = new MemberRelations();
        foreach(DataRow row in rows) {
          _relatedMember = new MemberRelations() {
            Id = row["id"].ToString().GetInt32()
          };
          _relatedMember.Name = "{0} ".FormatWith(row["Salutation"].ToString());
          _relatedMember.Name += row["FirstName"].IsNullOrEmpty() ? string.Empty : "{0} ".FormatWith(row["FirstName"].ToString());
          _relatedMember.Name += row["MiddleName"].IsNullOrEmpty() ? string.Empty : "{0} ".FormatWith(row["MiddleName"].ToString());
          _relatedMember.Name += row["LastName"].IsNullOrEmpty() ? string.Empty : "{0} ".FormatWith(row["LastName"].ToString());
          _relatedMember.Name += row["Suffix"].IsNullOrEmpty() ? string.Empty : "{0} ".FormatWith(row["Suffix"].ToString());
          memberRelations.Add(_relatedMember);
        }
      }
      return memberRelations;
    }
  }
}