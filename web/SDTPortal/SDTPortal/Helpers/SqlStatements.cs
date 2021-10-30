using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SDTPortal {
  public static class SqlStatements {
    // System Settings
    public const string SQL_GET_SYSTEM_SETTINGS = "SELECT * FROM dbo.SystemConfigs WHERE Id = 1;";
    public const string SQL_UPDATE_SYSTEM_SETTINGS = "UPDATE dbo.SystemConfigs SET TagLine = '{0}', SystemName = '{1}', SystemMessage = '{2}', TransactionMessage = '{3}', ThankyouMessage = '{4}', SiteName = '{5}', AdminMessage = '{6}', SuperAdminMessage = '{7}' WHERE Id = 1;";
    public const string SQL_UPDATE_MAIL_SETTINGS = "UPDATE dbo.SystemConfigs SET MailServer = '{0}', ServerPort = {1}, SmtpUser = '{2}', SmtpPassword = '{3}', FromEmail = '{4}', FromUsername = '{5}', RequireAuth = {6}, RequireSsl = {7} WHERE Id = 1;";

    // User and authentication
    public const string SQL_AUTHENTICATE_USER = "SELECT * FROM dbo.AdminUsers WHERE UserName = '{0}' AND UserPass = '{1}' AND Deleted = 0;";
    public const string SQL_GET_USER_ROLE = "SELECT RoleName FROM dbo.AdminRoles WHERE Id = '{0}';";
    public const string SQL_GET_USER_DETAILS = "SELECT * FROM dbo.AdminUsers WHERE Id = '{0}';";
    public const string SQL_UPDATE_USER_DETAILS = "UPDATE dbo.AdminUsers SET DisplayName = '{0}', UserName = '{1}', Notes = '{2}' WHERE Id = '{3}';";
    public const string SQL_RESET_USER_PASSWORD = "UPDATE dbo.AdminUsers SET UserPass = '{0}', PasswordReset = 1 WHERE Id = '{1}';";
    public const string SQL_UPDATE_USER_PASSWORD = "UPDATE dbo.AdminUsers SET UserPass = '{0}', PasswordReset = 0 WHERE Id = '{1}';";
    public const string SQL_CREATE_USER_DETAILS = "INSERT INTO dbo.AdminUsers (RoleId, DisplayName, UserName, Notes) OUTPUT inserted.Id  VALUES ('e544d9f4-c6f6-4c25-b828-de13141f37e2', '{0}', '{1}', '{2}');";
    public const string SQL_CREATE_ADMIN_USER_DETAILS = "INSERT INTO dbo.AdminUsers (RoleId, DisplayName, UserName, Notes, SuperAdmin) OUTPUT inserted.Id  VALUES ('1af14abd-5937-4475-8de7-3da0536613c8', '{0}', '{1}', '{2}', {3});";
    public const string SQL_UPDATE_ADMIN_USER_DETAILS = "UPDATE dbo.AdminUsers SET DisplayName = '{0}', UserName = '{1}', Notes = '{2}', SuperAdmin = {3} WHERE Id = '{4}';";
    public const string SQL_DELETE_USER = "UPDATE dbo.AdminUsers SET Deleted = 1 WHERE Id = '{0}';";
    public const string SQL_VALIDATE_USER = "SELECT Id FROM dbo.AdminUsers WHERE UserName = '{0}' AND Deleted = 0;";

    // Datatable selects
    public const string SQL_GET_CONTACT_LIST = "SELECT Id, UserName FROM dbo.SystemContacts WHERE Deleted = 0;";
    public const string SQL_GET_CONTACTS_LIST = "SELECT Id, UserName, UserEmail, Notes FROM dbo.SystemContacts WHERE Deleted = 0;";
    public const string SQL_GET_CONTACT_DETAILS = "SELECT * FROM dbo.SystemContacts WHERE Id = '{0}';";
    public const string SQL_GET_CLIENT_LIST = "SELECT au.Id, au.DisplayName, au.UserName, au.Notes, MAX(sm.SubmitDate) as LastSubmission FROM dbo.AdminUsers au LEFT JOIN dbo.SystemMessages sm ON au.Id = sm.FromUser WHERE au.RoleId = 'e544d9f4-c6f6-4c25-b828-de13141f37e2' AND Deleted = 0 GROUP BY au.Id, au.DisplayName, au.UserName, au.Notes;";
    public const string SQL_GET_ADMIN_LIST = "SELECT au.Id, au.DisplayName, au.UserName, au.Notes, au.SuperAdmin FROM dbo.AdminUsers au WHERE au.RoleId = '1af14abd-5937-4475-8de7-3da0536613c8' AND Deleted = 0;";

    // Document Submissions
    public const string SQL_SAVE_MESSAGE = "INSERT INTO dbo.SystemMessages (FromUser, ToEmail, SubmitDate, SubjectLine) OUTPUT inserted.Id VALUES ('{0}','{1}',{2},'{3}');";
    public const string SQL_SAVE_MESSAGE_ATTACHMENT = "INSERT INTO dbo.SystemMessageAttachments (MessageId, ToEmail, SubmitDate, Attachment) OUTPUT inserted.Id VALUES ('{0}','{1}',{2},'{3}');";
    public const string SQL_GET_USER_MESSAGES = "SELECT sm.Id, sm.SubmitDate, sm.SubjectLine, sc.UserName AS ToUser, (SELECT STUFF((SELECT ', ' + Attachment FROM dbo.SystemMessageAttachments WHERE MessageId = sm.Id FOR XML PATH('')), 1, 1, '')) AS Attachments FROM dbo.SystemMessages sm LEFT JOIN dbo.SystemContacts sc ON sm.ToEmail = sc.Id LEFT JOIN dbo.AdminUsers au ON sm.FromUser = au.Id WHERE sm.FromUser = '{0}' ORDER BY sm.SubmitDate DESC;";
    public const string SQL_GET_ALL_MESSAGES = "SELECT sm.Id, sm.SubmitDate, sm.SubjectLine, sc.UserName AS ToUser, au.DisplayName AS FromUser, (SELECT STUFF((SELECT ', ' + Attachment FROM dbo.SystemMessageAttachments WHERE MessageId = sm.Id FOR XML PATH('')), 1, 1, '')) AS Attachments FROM dbo.SystemMessages sm LEFT JOIN dbo.SystemContacts sc ON sm.ToEmail = sc.Id LEFT JOIN dbo.AdminUsers au ON sm.FromUser = au.Id ORDER BY sm.SubmitDate DESC;";
    public const string SQL_GET_DOCUMENT_ATTACHMENTS = "SELECT sm.SubjectLine, sma.Id, sma.SubmitDate, sma.Attachment FROM SystemMessageAttachments sma INNER JOIN SystemMessages sm ON sm.Id = sma.MessageId WHERE sm.Id = '{0}';";

    // Contact Management
    public const string SQL_CREATE_CONTACT_DETAILS = "INSERT INTO dbo.SystemContacts(UserName, UserEmail, Notes) OUTPUT Inserted.Id VALUES('{0}', '{1}', '{2}');";
    public const string SQL_UPDATE_CONTACT_DETAILS = "UPDATE dbo.SystemContacts SET UserName = '{0}', UserEmail = '{1}', Notes = '{2}' WHERE Id = '{3}';";
    public const string SQL_DELETE_CONTACT = "UPDATE dbo.SystemContacts SET Deleted = 1 WHERE Id = '{0}';";

    // Exceptions
    public const string SQL_LOG_EXCEPTION = "INSERT INTO dbo.SystemExceptions (ExceptionTimeStamp, Module, Exception, StackTrace, SubmissionId) VALUES ({0}, '{1}', '{2}', '{3}', {4});";
    public const string SQL_READ_EXCEPTIONS = "SELECT * FROM dbo.SystemExceptions ORDER BY ExceptionTimeStamp DESC;";
  }
}