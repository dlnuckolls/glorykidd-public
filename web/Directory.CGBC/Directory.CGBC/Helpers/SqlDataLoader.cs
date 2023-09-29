using Directory.CGBC.Objects;
using GloryKidd.WebCore.BaseObjects;
using GloryKidd.WebCore.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Data.SqlClient;

namespace Directory.CGBC.Helpers {
  public static class SqlDataLoader {

    private static List<State> _stateList = null;
    private static List<RelationshipType> _relationshipTypeList = null;
    private static List<Salutation> _salutationList = null;
    private static List<MaritalStatus> _maritalStatusList = null;

    public static List<State> States() => _stateList.IsNullOrEmpty() || _stateList.Count == 0 ? _stateList = GetStates() : _stateList;
    public static List<RelationshipType> RelationshipTypes() => _relationshipTypeList.IsNullOrEmpty() || _relationshipTypeList.Count == 0 ? _relationshipTypeList = GetRelationshipTypes() : _relationshipTypeList;
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
  }
}