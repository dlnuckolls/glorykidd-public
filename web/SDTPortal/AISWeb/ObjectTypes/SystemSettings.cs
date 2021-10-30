using System;

namespace AISWeb {
  public class SystemSettings {
    public int Id { get; set; }
    public string TagLine { get; set; }
    public string SiteName { get; set; }
    public string SystemName { get; set; }
    public string AdminMessage { get; set; }
    public string SuperAdminMessage { get; set; }
    public string UploadMessage { get; set; }
    public string ThankyouMessage { get; set; }
    public string TransactionMessage { get; set; }
    public string MailServer { get; set; }
    public int ServerPort { get; set; }
    public bool RequireSsl { get; set; }
    public bool RequireAuth { get; set; }
    public string SmtpUser { get; set; }
    public string SmtpPassword { get; set; }
    public string FromEmail { get; set; }
    public string FromUsername { get; set; }

    private static SystemSettings _instance = null;

    public static SystemSettings StaticInstance => _instance.IsNullOrEmpty() ? _instance = LoadSystemSettings() : _instance;

    private static SystemSettings LoadSystemSettings() {
      try {
        var dataRow = SqlHelpers.Select(SqlStatements.SQL_GET_SYSTEM_SETTINGS).Rows[0];
        return new SystemSettings() {
          Id = dataRow["Id"].ToString().GetInt32(),
          TagLine = dataRow["TagLine"].ToString(),
          SystemName = dataRow["SystemName"].ToString(),
          UploadMessage = dataRow["SystemMessage"].ToString(),
          MailServer = dataRow["MailServer"].ToString(),
          ServerPort = dataRow["ServerPort"].ToString().GetInt32(),
          SmtpUser = dataRow["SmtpUser"].ToString(),
          SmtpPassword = dataRow["SmtpPassword"].ToString().DecryptString(),
          FromEmail = dataRow["FromEmail"].ToString(),
          FromUsername = dataRow["FromUsername"].ToString(),
          AdminMessage = dataRow["AdminMessage"].ToString(),
          SuperAdminMessage = dataRow["SuperAdminMessage"].ToString(),
          SiteName = dataRow["SiteName"].ToString(),
          ThankyouMessage = dataRow["ThankyouMessage"].ToString(),
          TransactionMessage = dataRow["TransactionMessage"].ToString(),
          RequireAuth = dataRow["RequireAuth"].ToString().GetAsBool(),
          RequireSsl = dataRow["RequireSsl"].ToString().GetAsBool()
        };
      } catch(Exception ex) {
        SqlHelpers.Insert(SqlStatements.SQL_LOG_EXCEPTION.FormatWith(DateTime.Now.ConvertSqlDateTime(), "SystemSettings", ex.Message.FixSqlString(), ex.StackTrace.FixSqlString()));
        throw new ApplicationException("Sql Server Not accessible");
      }
    }

    public void UpdateSystemSettings() {
      SqlHelpers.Update(SqlStatements.SQL_UPDATE_SYSTEM_SETTINGS.FormatWith(TagLine.FixSqlString(255), SystemName.FixSqlString(255),
        UploadMessage.FixSqlString(2000), TransactionMessage.FixSqlString(2000), ThankyouMessage.FixSqlString(2000), SiteName.FixSqlString(255),
        AdminMessage.FixSqlString(2000), SuperAdminMessage.FixSqlString(2000)));
      _instance = LoadSystemSettings();
    }
    public void UpdateMailSettings() {
      SqlHelpers.Update(SqlStatements.SQL_UPDATE_MAIL_SETTINGS.FormatWith(MailServer.FixSqlString(), ServerPort,
        SmtpUser.FixSqlString(75), SmtpPassword.EncryptString(), FromEmail.FixSqlString(75), FromUsername.FixSqlString(75),
        (RequireAuth) ? "1" : "0", (RequireSsl) ? "1" : "0"));
      _instance = LoadSystemSettings();
    }

    public void LogError(string module, Exception error) {
      SqlHelpers.Insert(SqlStatements.SQL_LOG_EXCEPTION.FormatWith(DateTime.Now.ConvertSqlDateTime(), module.FixSqlString(), error.Message.FixSqlString(), error.StackTrace.FixSqlString()));
    }
    public void LogError(string module, string error) {
      SqlHelpers.Insert(SqlStatements.SQL_LOG_EXCEPTION.FormatWith(DateTime.Now.ConvertSqlDateTime(), module.FixSqlString(), error.FixSqlString(), string.Empty));
    }
  }
}