using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace SDTPortal {
  public static class SqlDatasets {
    public static DataTable GetSystemRecipients() => SqlHelpers.Select(SqlStatements.SQL_GET_CONTACT_LIST);
    public static DataTable GetClientList() => SqlHelpers.Select(SqlStatements.SQL_GET_CLIENT_LIST);
    public static DataTable GetContactList() => SqlHelpers.Select(SqlStatements.SQL_GET_CONTACTS_LIST);
    public static DataTable GetMyMessages(string id) => SqlHelpers.Select(SqlStatements.SQL_GET_USER_MESSAGES.FormatWith(id));
    public static DataTable GetAllMessages() => SqlHelpers.Select(SqlStatements.SQL_GET_ALL_MESSAGES);
    public static DataTable GetAdminList() => SqlHelpers.Select(SqlStatements.SQL_GET_ADMIN_LIST);
  }
}