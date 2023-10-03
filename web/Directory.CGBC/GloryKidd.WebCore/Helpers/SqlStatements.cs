namespace GloryKidd.WebCore.Helpers {
  public static class SqlStatements {
    // System Settings
    public const string SQL_GET_MAIL_SETTINGS = "SELECT Id, MailServer, ServerPort, SmtpUser, SmtpPassword, FromEmail, FromUsername, RequireAuth, RequireSsl FROM dbo.SystemConfigs;";
    public const string SQL_UPDATE_MAIL_SETTINGS = "UPDATE dbo.SystemConfigs SET MailServer = '{0}', ServerPort = {1}, SmtpUser = '{2}', SmtpPassword = '{3}', FromEmail = '{4}', FromUsername = '{5}', RequireAuth = {6}, RequireSsl = {7} WHERE Id = '{8}';";

    // User and authentication
    public const string SQL_AUTHENTICATE_USER = "SELECT * FROM dbo.AdminUsers WHERE UserName = '{0}' AND UserPass = '{1}' AND Deleted = 0;";
    public const string SQL_GET_USER_ROLE = "SELECT RoleName FROM dbo.AdminRoles WHERE Id = '{0}';";
    public const string SQL_GET_USER_DETAILS = "SELECT * FROM dbo.AdminUsers WHERE Id = '{0}';";
    public const string SQL_GET_ALL_USERS = @"
SELECT [Id],[RoleId],[DisplayName],[UserName],[Notes],[SuperAdmin],[MemberId]
  FROM [CGBCDirectory].[dbo].[AdminUsers]
 WHERE Deleted = 0
 ORDER BY DisplayName ASC;";
    public const string SQL_GET_ALL_ROLES = "SELECT Id, RoleName FROM dbo.AdminRoles;";
    public const string SQL_GET_ALL_USER_MEMBERS = @"
SELECT m.Id, m.FirstName, m.LastName
  FROM dbo.Member m 
 ORDER BY m.LastName, m.FirstName ASC;";
    public const string SQL_UPDATE_USER_DETAILS = "UPDATE dbo.AdminUsers SET DisplayName = '{0}', UserName = '{1}', Notes = '{2}', MemberId = {3} WHERE Id = '{4}';";
    public const string SQL_RESET_USER_PASSWORD = "UPDATE dbo.AdminUsers SET UserPass = '{0}', PasswordReset = 1 WHERE Id = '{1}';";
    public const string SQL_UPDATE_USER_PASSWORD = "UPDATE dbo.AdminUsers SET UserPass = '{0}', PasswordReset = 0 WHERE Id = '{1}';";
    public const string SQL_CREATE_USER_DETAILS = "INSERT INTO dbo.AdminUsers (RoleId, DisplayName, UserName, Notes, MemberId) OUTPUT inserted.Id  VALUES ({0}, '{1}', '{2}', '{3}', {4});";
    public const string SQL_DELETE_USER = "UPDATE dbo.AdminUsers SET Deleted = 1 WHERE Id = '{0}';";
    public const string SQL_VALIDATE_USER = "SELECT Id FROM dbo.AdminUsers WHERE UserName = '{0}' AND Deleted = 0;";

    // Exceptions
    public const string SQL_LOG_EXCEPTION = "INSERT INTO dbo.SystemExceptions (ExceptionTimeStamp, Module, Exception, StackTrace) VALUES ({0}, '{1}', '{2}', '{3}');";
    public const string SQL_READ_EXCEPTIONS = "SELECT * FROM dbo.SystemExceptions ORDER BY ExceptionTimeStamp DESC;";

    // Page Content Blocks
    public const string SQL_GET_PAGE_LOCATIONS = "SELECT [Id],[Description] FROM [dbo].[PageLocations];";
    public const string SQL_GET_PAGE_CONTENTS = "SELECT ISNULL([Description],'') [Description] FROM [dbo].[PageContent] WHERE [PageLocation] = '{0}';";
    public const string SQL_GET_PAGE_CONTENT_FOR_DISPLAY = "SELECT ISNULL(a.[Description],'') [Description] FROM [dbo].[PageContent] a WHERE a.[PageLocation] = '{0}';";
    public const string SQL_SAVE_PAGE_CONTENTS = "UPDATE [dbo].[PageContent] SET [Description] = '{0}' WHERE [PageLocation] = '{1}';";

    //Member Statements
    public const string SQL_GET_ALL_MEMBERS = @"
SELECT m.Id, m.FirstName, m.LastName, m.CellPhone, m.HomePhone, m.ModifiedDate 
  FROM dbo.Member m 
 ORDER BY m.LastName, m.FirstName ASC;";
    public const string SQL_GET_SINGLE_MEMBERS = @"
SELECT m.Id, m.SalutationId, mg.Salutation, m.FirstName, m.MiddleName, m.LastName, m.Suffix, m.Gender, m.MaritalStatusId, 
       ms.MaritalStatus, m.DateOfBirth, m.MarriageDate, m.Address1, m.Address2, m.City, m.StateId, m.Zip, m.CellPhone, m.HomePhone, 
       m.EmailAddress1, m.EmailAddress2, m.ModifiedDate, m.CreateDate 
  FROM dbo.Member m 
 INNER JOIN dbo.MaritalStatus ms ON ms.Id = m.MaritalStatusId
 INNER JOIN dbo.Salutation mg ON mg.Id = m.SalutationId
 WHERE m.Id = {0}
 ORDER BY m.LastName, m.FirstName ASC;";
    public const string SQL_GET_MEMBER_RELATIONS = @"
SELECT m.Id, m.SalutationId, m.FirstName, m.MiddleName, m.LastName, m.Suffix, m.Gender, mm.RelationshipTypeId
  FROM dbo.Xref_Member_Member mm 
 INNER JOIN dbo.Member m ON mm.RelatedId = m.Id
 INNER JOIN dbo.Salutation mg ON mg.Id = m.SalutationId
 WHERE mm.MemberId = {0} 
 ORDER BY m.LastName, m.FirstName ASC;";
    public const string SQL_GET_MEMBER_RELATION_LIST = @"
SELECT m.Id, mg.Salutation, m.FirstName, m.MiddleName, m.LastName, m.Suffix
  FROM dbo.Member m 
 INNER JOIN dbo.Salutation mg ON mg.Id = m.SalutationId
 ORDER BY m.LastName, m.FirstName ASC;";
    public const string SQL_GET_MEMBER_NOTES = "SELECT mn.Id, au.DisplayName, mn.Notes, mn.CreateDate FROM dbo.MemberNotes mn INNER JOIN dbo.AdminUsers au ON mn.UserId = au.Id WHERE mn.MemberId = {0} ORDER BY mn.CreateDate DESC;";
    public const string SQL_SAVE_MEMBER = @"
INSERT INTO [dbo].[Member] ([SalutationId],[FirstName],[MiddleName],[LastName],[Suffix],[Gender],[Address1],[Address2],
                            [City],[StateId],[Zip],[CellPhone],[HomePhone],[EmailAddress1],[EmailAddress2],[MaritalStatusId],
                            [DateOfBirth],[MarriageDate],[ModifiedDate],[CreateDate])
     VALUES ({0},'{1}','{2}','{3}','{4}',{5},'{6}','{7}','{8}',{9},'{10}','{11}','{12}','{13}','{14}',{15},{16},{17},GETDATE(),GETDATE()); SELECT IDENT_CURRENT('Member');";
    public const string SQL_UPDATE_MEMBER = @"
UPDATE [dbo].[Member] 
   SET [SalutationId] = {0}, [FirstName] = '{1}', [MiddleName] = '{2}', [LastName] = '{3}', [Suffix] = '{4}', [Gender] = {5}, 
       [Address1] = '{6}', [Address2] = '{7}', [City] = '{8}', [StateId] = {9}, [Zip] = '{10}', [CellPhone] = '{11}',
       [HomePhone] = '{12}', [EmailAddress1] = '{13}', [EmailAddress2] = '{14}', [MaritalStatusId] = {15},
       [DateOfBirth] = {16}, [MarriageDate] = {17}, [ModifiedDate] = GETDATE()
 WHERE [Id] = {18};";
    public const string SQL_SAVE_MEMBER_NOTE = "INSERT INTO [dbo].[MemberNotes] ([MemberId], [Notes], [CreateDate], [UserId]) VALUES ({0}, '{1}', GETDATE(), {2});";
    public const string SQL_REMOVE_MEMBER_RELATIONS = "DELETE dbo.Xref_Member_Member WHERE (MemberId = {0} OR RelatedId = {0});";
    public const string SQL_SAVE_MEMBER_RELATIONS = "INSERT INTO dbo.Xref_Member_Member (MemberId,RelatedId,RelationshipTypeId) VALUES ({0}, {1}, {2});";

    //General Statements
    public const string SQL_GET_STATES = "SELECT Id, State, Abbreviation FROM dbo.States;";
    public const string SQL_GET_RELATIONSHIPTYPES = "SELECT Id, RelationshipType FROM dbo.RelationshipType;";
    public const string SQL_GET_SALUTATIONS = "SELECT Id,Salutation FROM dbo.Salutation;";
    public const string SQL_GET_MARITALSTATUSES = "SELECT Id, MaritalStatus FROM dbo.MaritalStatus;";
    
  }
}