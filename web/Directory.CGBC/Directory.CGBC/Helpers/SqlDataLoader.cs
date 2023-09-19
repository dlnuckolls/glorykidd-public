using Directory.CGBC.Objects;
using GloryKidd.WebCore.BaseObjects;
using GloryKidd.WebCore.Helpers;
using System;
using System.Collections.Generic;
using System.Data;

namespace Directory.CGBC.Helpers {
  public static class SqlDataLoader {
    private static List<State> _stateList = null;

    public static List<State> States => _stateList.IsNullOrEmpty() ? _stateList = GetStates() : _stateList;

    private static List<State> GetStates() {
      var states = new List<State>();
      var rows = SqlHelpers.Select(SqlStatements.SQL_GET_STATES).Rows;
      foreach(DataRow row in rows) {
        states.Add(new State() {
          Id = row["Id"].ToString().GetInt32(),
          StateName = row["State"].ToString(),
          Abbreviation = row["Abbreviation"].ToString()
        });
      }
      return states;
    }
  }
}