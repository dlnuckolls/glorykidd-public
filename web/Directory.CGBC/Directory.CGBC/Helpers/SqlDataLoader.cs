using Directory.CGBC.Objects;
using GloryKidd.WebCore.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using Telerik.Web.UI;
using static Telerik.Web.UI.GridHeaderContextMenu.GridContextFilterTemplate;

namespace Directory.CGBC.Helpers {
  public static class SqlDataLoader {

    public static Member GetMember(int Id) {
      var row = SqlHelpers.Select(SqlStatements.SQL_GET_SINGLE_MEMBERS.FormatWith(Id)).Rows[0];
      return new Member() {
        Id = row["Id"].ToString().GetInt32(),
        PreFix = row["Prefix"].ToString(),
        FirstName = row["FirstName"].ToString(),
        MiddleName = row["MiddleName"].ToString(),
        LastName = row["LastName"].ToString(),
        Suffix = row["Suffix"].ToString(),
        MaritalStatus = (MaritalStatus)Enum.Parse(typeof(MaritalStatus), row["MaritalStatusId"].ToString()),
        MarriageDate = row["MarriageDate"].ToString().GetAsDate(),
        DateOfBirth = row["DateOfBirth"].ToString().GetAsDate(),
        Modified = row["ModifiedDate"].ToString().GetAsDate(),
        Created = row["CreateDate"].ToString().GetAsDate()
      };
    }
  }
}