using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GloryKidd.WebCore.Helpers {
  public static class SqlStatements {
    // System Settings
    public const string SQL_GET_MAIL_SETTINGS = "SELECT Id, MailServer, ServerPort, SmtpUser, SmtpPassword, FromEmail, FromUsername, RequireAuth, RequireSsl FROM dbo.SystemConfigs;";
    public const string SQL_UPDATE_MAIL_SETTINGS = "UPDATE dbo.SystemConfigs SET MailServer = '{0}', ServerPort = {1}, SmtpUser = '{2}', SmtpPassword = '{3}', FromEmail = '{4}', FromUsername = '{5}', RequireAuth = {6}, RequireSsl = {7} WHERE Id = '{8}';";

    // User and authentication
    public const string SQL_AUTHENTICATE_USER = "SELECT * FROM dbo.AdminUsers WHERE UserName = '{0}' AND UserPass = '{1}' AND Deleted = 0;";
    public const string SQL_GET_USER_ROLE = "SELECT RoleName FROM dbo.AdminRoles WHERE Id = '{0}';";
    public const string SQL_GET_USER_DETAILS = "SELECT * FROM dbo.AdminUsers WHERE Id = '{0}';";
    public const string SQL_UPDATE_USER_DETAILS = "UPDATE dbo.AdminUsers SET DisplayName = '{0}', UserName = '{1}', Notes = '{2}' WHERE Id = '{3}';";
    public const string SQL_RESET_USER_PASSWORD = "UPDATE dbo.AdminUsers SET UserPass = '{0}', PasswordReset = 1 WHERE Id = '{1}';";
    public const string SQL_UPDATE_USER_PASSWORD = "UPDATE dbo.AdminUsers SET UserPass = '{0}', PasswordReset = 0 WHERE Id = '{1}';";
    public const string SQL_CREATE_USER_DETAILS = "INSERT INTO dbo.AdminUsers (RoleId, DisplayName, UserName, Notes) OUTPUT inserted.Id  VALUES ('e544d9f4-c6f6-4c25-b828-de13141f37e2', '{0}', '{1}', '{2}');";
    public const string SQL_DELETE_USER = "DELETE dbo.AdminUsers WHERE Id = '{0}';";
    public const string SQL_VALIDATE_USER = "SELECT Id FROM dbo.AdminUsers WHERE UserName = '{0}' AND Deleted = 0;";

    // Exceptions
    public const string SQL_LOG_EXCEPTION = "INSERT INTO dbo.SystemExceptions (ExceptionTimeStamp, Module, Exception, StackTrace) VALUES ({0}, '{1}', '{2}', '{3}');";
    public const string SQL_READ_EXCEPTIONS = "SELECT * FROM dbo.SystemExceptions ORDER BY ExceptionTimeStamp DESC;";

    // Page Content Blocks
    public const string SQL_GET_PAGE_LOCATIONS = "SELECT [Id],[Description] FROM [dbo].[PageLocations];";
    public const string SQL_GET_PAGE_CONTENTS = "SELECT ISNULL([Description],'') [Description] FROM [dbo].[PageContent] WHERE [PageLocation] = '{0}';";
    public const string SQL_GET_PAGE_CONTENT_FOR_DISPLAY = "SELECT ISNULL(a.[Description],'') [Description] FROM [dbo].[PageContent] a WHERE a.[PageLocation] = '{0}';";
    public const string SQL_SAVE_PAGE_CONTENTS = "UPDATE [dbo].[PageContent] SET [Description] = '{0}' WHERE [PageLocation] = '{1}';";
  }
}