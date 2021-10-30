using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AISWeb {
  public class SystemContact {
    public string Id { get; set; }
    public string UserName { get; set; }
    public string UserEmail { get; set; }
    public string Notes { get; set; }

    public SystemContact() { }

    public SystemContact(string id) {
      var dataRow = SqlHelpers.Select(SqlStatements.SQL_GET_CONTACT_DETAILS.FormatWith(id)).Rows[0];
      if(dataRow.IsNullOrEmpty()) return;
      Id = dataRow["Id"].ToString();
      UserName = dataRow["UserName"].ToString();
      UserEmail = dataRow["UserEmail"].ToString();
      Notes = dataRow["Notes"].ToString();
    }

    public void SaveContactDetails() {
      if(Id.IsNullOrEmpty()) {
        // Create New Contact
        Id = SqlHelpers.InsertScalar(SqlStatements.SQL_CREATE_CONTACT_DETAILS.FormatWith(UserName.FixSqlString(), UserEmail.FixSqlString(), Notes.FixSqlString()));
      } else {
        // Update User
        SqlHelpers.Update(SqlStatements.SQL_UPDATE_CONTACT_DETAILS.FormatWith(UserName.FixSqlString(), UserEmail.FixSqlString(), Notes.FixSqlString(), Id));
      }
    }

    public void DeleteContact(string id) { SqlHelpers.Update(SqlStatements.SQL_DELETE_CONTACT.FormatWith(id)); }
  }
}