using System;
using System.Web.Security;
using GloryKidd.WebCore.Helpers;

namespace GloryKidd.WebCore.BaseObjects {
  public class SystemUser {
    public int Id { get; set; }
    public string RoleId { get; set; }
    public string Role { get; set; }
    public bool IsAuthenticated { get; set; }
    public bool IsAdmin { get; set; }
    public string DisplayName { get; set; }
    public string UserName { get; set; }
    public bool UserPassReset { get; set; }
    public string Notes { get; set; }
    public bool IsSuperAdmin { get; set; }
    public int MemberId { get; set; }

    private static SystemUser _instance = null;

    public static SystemUser StaticInstance => _instance.IsNullOrEmpty() ? _instance = new SystemUser() : _instance;

    public SystemUser() {
      DisplayName = "Guest User";
      IsAuthenticated = false;
      IsAdmin = false;
      IsSuperAdmin = false;
    }

    public void LogoutUser() => _instance = null;

    public void LoadUserDetails(int id) {
      try {
        var dataRow = SqlHelpers.Select(SqlStatements.SQL_GET_USER_DETAILS.FormatWith(id)).Rows[0];
        if(dataRow.IsNullOrEmpty()) return;
        Id = dataRow["Id"].ToString().GetInt32();
        DisplayName = dataRow["DisplayName"].ToString();
        UserName = dataRow["UserName"].ToString();
        Notes = dataRow["Notes"].ToString();
        MemberId = dataRow["MemberId"].ToString().GetInt32();
      } catch(Exception ex) {
        LogError("SystemUser: Load User method", ex);
        throw new ApplicationException("Record not found");
      }
    }

    public void AuthenticateUser(string userName, string password) {
      try {
        var dataRow = SqlHelpers.Select(SqlStatements.SQL_AUTHENTICATE_USER.FormatWith(userName, password)).Rows[0];
        if(dataRow.IsNullOrEmpty()) return;
        Id = dataRow["Id"].ToString().GetInt32();
        RoleId = dataRow["RoleId"].ToString();
        DisplayName = dataRow["DisplayName"].ToString();
        UserName = dataRow["UserName"].ToString();
        IsSuperAdmin = dataRow["SuperAdmin"].ToString().GetAsBool();
        UserPassReset = dataRow["PasswordReset"].ToString().GetAsBool();
        Notes = dataRow["Notes"].ToString();
        MemberId = dataRow["MemberId"].ToString().GetInt32();
        Role = SqlHelpers.SelectScalar(SqlStatements.SQL_GET_USER_ROLE.FormatWith(RoleId)).ToString();
        IsAuthenticated = true;
        IsAdmin = Role == "System Admin";
      } catch(Exception ex) {
        LogError("SystemUser: Authenticate User method", ex);
        throw new ApplicationException("Record not found");
      }
    }

    private void LogError(string module, Exception error) {
      SqlHelpers.Insert(SqlStatements.SQL_LOG_EXCEPTION.FormatWith(DateTime.Now.ConvertSqlDateTime(), module.FixSqlString(), error.Message.FixSqlString(), error.StackTrace.FixSqlString()));
    }

    private void LogError(string module, string error) {
      SqlHelpers.Insert(SqlStatements.SQL_LOG_EXCEPTION.FormatWith(DateTime.Now.ConvertSqlDateTime(), module.FixSqlString(), error.FixSqlString(), string.Empty));
    }
    public void RemoveUser() {
      SqlHelpers.Update(SqlStatements.SQL_DELETE_USER.FormatWith(Id));
    }
    public void SaveUserDetails() {
      try {
        if(Id.IsNullOrEmpty() || Id == 0) {
          // Create New User
          Id = SqlHelpers.InsertScalar(SqlStatements.SQL_CREATE_USER_DETAILS.FormatWith(RoleId, DisplayName.FixSqlString(),
            UserName.FixSqlString(), Notes.FixSqlString(250), MemberId == 0 ? "NULL" : MemberId.ToString())).GetInt32();
        } else {
          // Update User
          SqlHelpers.Update(SqlStatements.SQL_UPDATE_USER_DETAILS.FormatWith(DisplayName.FixSqlString(),
            UserName.FixSqlString(), Notes.FixSqlString(250), MemberId == 0 ? "NULL" : MemberId.ToString(), Id));
        }
      } catch(Exception ex) {
        LogError("SystemUser: Save User method", ex);
        throw new ApplicationException("Record not Saved");
      }
    }

    public string ValidateUser(string userName) {
      var rtn = string.Empty;
      var uid = SqlHelpers.SelectScalar(SqlStatements.SQL_VALIDATE_USER.FormatWith(userName));
      if(!uid.IsNullOrEmpty()) rtn = uid.ToString();
      return rtn;
    }

    public string ResetUserPassword(int id) {
      var tempPassword = Membership.GeneratePassword(10, 2);
      SqlHelpers.Update(SqlStatements.SQL_RESET_USER_PASSWORD.FormatWith(tempPassword.EncryptString(), id));
      return tempPassword;
    }

    public void SetUserPassword(int id, string password) {
      SqlHelpers.Update(SqlStatements.SQL_UPDATE_USER_PASSWORD.FormatWith(password.EncryptString(), id));
    }
  }
}