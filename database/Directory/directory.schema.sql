/*
    Cedar Grove Directory Core Database
     Username: cgbc_user
    Password: XJ$7d{84)sa(kV4
  */ 
/*
USE Master
GO

SET NOCOUNT ON;
 
IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = N'CGBCDirectory') BEGIN
  DECLARE @tail       INT
  DECLARE @basefolder NVARCHAR(MAX)
  DECLARE @datafile   NVARCHAR(MAX)
  DECLARE @logfile    NVARCHAR(MAX)
 
  SET @tail = (SELECT CHARINDEX('\',REVERSE(physical_name)) FROM Master.sys.master_files WHERE name = 'master')
  SET @basefolder = (SELECT SUBSTRING(physical_name,1,LEN(physical_name)-@tail) FROM Master.sys.master_files WHERE name = 'master')
  SET @datafile = @basefolder + '\CGBCDirectory.mdf'
  SET @logfile = @basefolder + '\CGBCDirectory.ldf'
  DECLARE @sql NVARCHAR(MAX)
  SET @sql = 'CREATE DATABASE [CGBCDirectory] ON PRIMARY ' +
  '( NAME = N''CGBCDirectory'', FILENAME = ''' + @datafile + ''', SIZE = 4096KB , MAXSIZE = UNLIMITED, FILEGROWTH = 1024KB ) ' +
  ' LOG ON ' + 
  '( NAME = N''CGBCDirectory_log'', FILENAME = ''' + @logfile + ''', SIZE = 1024KB , MAXSIZE = 2048GB , FILEGROWTH = 10%)'
  EXEC(@sql)
   
  ALTER DATABASE CGBCDirectory SET COMPATIBILITY_LEVEL = 100;
  ALTER DATABASE CGBCDirectory SET ANSI_NULL_DEFAULT OFF;
  ALTER DATABASE CGBCDirectory SET ANSI_NULLS OFF;
  ALTER DATABASE CGBCDirectory SET ANSI_PADDING OFF; 
  ALTER DATABASE CGBCDirectory SET ANSI_WARNINGS OFF; 
  ALTER DATABASE CGBCDirectory SET ARITHABORT OFF; 
  ALTER DATABASE CGBCDirectory SET AUTO_CLOSE OFF; 
  ALTER DATABASE CGBCDirectory SET AUTO_CREATE_STATISTICS ON; 
  ALTER DATABASE CGBCDirectory SET AUTO_SHRINK OFF; 
  ALTER DATABASE CGBCDirectory SET AUTO_UPDATE_STATISTICS ON; 
  ALTER DATABASE CGBCDirectory SET CURSOR_CLOSE_ON_COMMIT OFF; 
  ALTER DATABASE CGBCDirectory SET CURSOR_DEFAULT  GLOBAL; 
  ALTER DATABASE CGBCDirectory SET CONCAT_NULL_YIELDS_NULL OFF; 
  ALTER DATABASE CGBCDirectory SET NUMERIC_ROUNDABORT OFF; 
  ALTER DATABASE CGBCDirectory SET QUOTED_IDENTIFIER OFF; 
  ALTER DATABASE CGBCDirectory SET RECURSIVE_TRIGGERS OFF; 
  ALTER DATABASE CGBCDirectory SET AUTO_UPDATE_STATISTICS_ASYNC OFF; 
  ALTER DATABASE CGBCDirectory SET DATE_CORRELATION_OPTIMIZATION OFF; 
  ALTER DATABASE CGBCDirectory SET TRUSTWORTHY OFF; 
  ALTER DATABASE CGBCDirectory SET ALLOW_SNAPSHOT_ISOLATION OFF; 
  ALTER DATABASE CGBCDirectory SET PARAMETERIZATION SIMPLE; 
  ALTER DATABASE CGBCDirectory SET READ_WRITE; 
  ALTER DATABASE CGBCDirectory SET RECOVERY SIMPLE; 
  ALTER DATABASE CGBCDirectory SET MULTI_USER; 
  ALTER DATABASE CGBCDirectory SET PAGE_VERIFY CHECKSUM;  
  ALTER DATABASE CGBCDirectory SET DB_CHAINING OFF; 
     
  IF(@@ERROR <> 0) BEGIN
    RETURN
  END
END
SET NOCOUNT OFF;
GO
*/

USE CGBCDirectory
GO

SET NOCOUNT ON;

DECLARE @majorVersion INT;
DECLARE @minorVersion INT;
DECLARE @Notes VARCHAR(2000);

-- Start the main transaction: Initialize the database with SchemaVersion table and data
IF NOT EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'[SchemaVersion]')) 
BEGIN
  BEGIN TRANSACTION initialCreate;
    CREATE TABLE [dbo].[SchemaVersion](
      Id           UNIQUEIDENTIFIER  NOT NULL DEFAULT NEWID(),
      MajorVersion INT               NOT NULL,
      MinorVersion INT               NOT NULL,
      InstallDate  DATETIMEOFFSET(7) NOT NULL
    CONSTRAINT PK_SchemaVersion PRIMARY KEY CLUSTERED (
        MajorVersion, MinorVersion ASC
      ) WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
    ) ON [PRIMARY];
 
    IF(@@ERROR <> 0)
    BEGIN
      ROLLBACK TRANSACTION;
      RETURN;
    END
  
    SET @majorVersion = 1;
    SET @minorVersion = 0;
    IF NOT EXISTS(SELECT * FROM dbo.SchemaVersion WHERE (MajorVersion = @majorVersion) AND (MinorVersion = @minorVersion))
    BEGIN
      INSERT INTO dbo.SchemaVersion (Id, MajorVersion, MinorVersion, InstallDate) values (newid(), @majorVersion, @minorVersion, getutcdate());
    END
    
    IF(@@ERROR <> 0)
    BEGIN
      ROLLBACK TRANSACTION;
      RETURN;
    END
  COMMIT TRANSACTION initialCreate;
END

-- Begin database modifications here
SELECT @majorVersion = 1, @minorVersion = 1;
IF NOT EXISTS(SELECT * FROM dbo.SchemaVersion WHERE (MajorVersion = @majorVersion) AND (MinorVersion = @minorVersion))
BEGIN
  BEGIN TRANSACTION Version1_1

    CREATE TABLE [dbo].[AdminRoles](
      [Id]       [int]          NOT NULL IDENTITY(1,1),
      [RoleName] [varchar](50)      NULL,
      [Notes]    [varchar](250)     NULL,
    CONSTRAINT [PK_AdminRoles] PRIMARY KEY CLUSTERED (
        [Id] ASC
      ) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
    ) ON [PRIMARY];
  
    CREATE TABLE [dbo].[AdminUsers](
      [Id]            [int]          NOT NULL IDENTITY(1,1),
      [RoleId]        [int]          NOT NULL,
      [DisplayName]   [varchar](50)      NULL,
      [UserName]      [varchar](50)  NOT NULL,
      [UserPass]      [varchar](256)     NULL,
      [Notes]         [varchar](250)     NULL,
      [Deleted]       [tinyint]      NOT NULL,
      [SuperAdmin]    [bit]          NOT NULL,
      [PasswordReset] [bit]          NOT NULL,
    CONSTRAINT [PK_AdminUsers] PRIMARY KEY CLUSTERED (
        [Id] ASC
      ) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
    ) ON [PRIMARY];

    ALTER TABLE [dbo].[AdminUsers] WITH CHECK ADD FOREIGN KEY([RoleId]) REFERENCES [dbo].[AdminRoles] ([Id]);
    ALTER TABLE [dbo].[AdminUsers] ADD  DEFAULT ((0)) FOR [Deleted];
    ALTER TABLE [dbo].[AdminUsers] ADD  DEFAULT ((0)) FOR [SuperAdmin];
    ALTER TABLE [dbo].[AdminUsers] ADD  DEFAULT ((0)) FOR [PasswordReset];

    CREATE TABLE [dbo].[SystemExceptions](
      [Id]                 [uniqueidentifier]  NOT NULL,
      [ExceptionTimeStamp] [datetime]          NOT NULL,
      [Module]             [varchar](50)       NOT NULL,
      [Exception]          [varchar](255)      NOT NULL,
      [StackTrace]         [varchar](2000)     NOT NULL,
    CONSTRAINT [PK_SystemExceptions] PRIMARY KEY CLUSTERED (
        [Id] ASC
      ) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
    ) ON [PRIMARY];

    ALTER TABLE [dbo].[SystemExceptions] ADD CONSTRAINT [DF_SystemExceptions_Id]  DEFAULT (newid()) FOR [Id];

    CREATE TABLE [dbo].[States] (
      [Id]           INT         NOT NULL IDENTITY(1,1),
      [State]        VARCHAR(50) NOT NULL,
      [Abbreviation] VARCHAR(2)  NOT NULL,
    ) ON [PRIMARY]

    INSERT INTO dbo.States (Abbreviation,State) 
    VALUES ('AL', 'Alabama'),('AK', 'Alaska'),('AZ', 'Arizona'),('AR', 'Arkansas'),('CA', 'California'),('CO', 'Colorado'),('CT', 'Connecticut'),
           ('DE', 'Delaware'),('DC', 'District of Columbia'),('FL', 'Florida'),('GA', 'Georgia'),('HI', 'Hawaii'),('ID', 'Idaho'),('IL', 'Illinois'),
           ('IN', 'Indiana'),('IA', 'Iowa'),('KS', 'Kansas'),('KY', 'Kentucky'),('LA', 'Louisiana'),('ME', 'Maine'),('MD', 'Maryland'),('MA', 'Massachusetts'),
           ('MI', 'Michigan'),('MN', 'Minnesota'),('MS', 'Mississippi'),('MO', 'Missouri'),('MT', 'Montana'),('NE', 'Nebraska'),('NV', 'Nevada'),
           ('NH', 'New Hampshire'),('NJ', 'New Jersey'),('NM', 'New Mexico'),('NY', 'New York'),('NC', 'North Carolina'),('ND', 'North Dakota'),
           ('OH', 'Ohio'),('OK', 'Oklahoma'),('OR', 'Oregon'),('PA', 'Pennsylvania'),('PR', 'Puerto Rico'),('RI', 'Rhode Island'),('SC', 'South Carolina'),
           ('SD', 'South Dakota'),('TN', 'Tennessee'),('TX', 'Texas'),('UT', 'Utah'),('VT', 'Vermont'),('VA', 'Virginia'),('WA', 'Washington'),
           ('WV', 'West Virginia'),('WI', 'Wisconsin'),('WY', 'Wyoming'),('AS', 'American Samoa'),('FM', 'Micronesia'),('VI', 'U.S. Virgin Islands'),
           ('PW', 'Palau'),('AA', 'U.S. Armed Forces America'),('GU', 'Guam'),('MP', 'Northern Mariana Islands'),('AE', 'U.S. Armed Forces Europe'),
           ('MH', 'Marshall Islands'),('AP', 'U.S. Armed Forces Pacific');    

    DELETE [dbo].[AdminRoles];

    INSERT INTO [dbo].[AdminRoles] (RoleName, Notes) 
    VALUES ('Guest','Guest level access'),
           ('User','User level access'),
           ('System User','System Administrator');
    
    INSERT INTO dbo.SchemaVersion (Id, MajorVersion, MinorVersion, InstallDate) values (newid(), @majorVersion, @minorVersion, getutcdate());
  COMMIT TRANSACTION Version1_1
END

SELECT @majorVersion = 1, @minorVersion = 2;
IF NOT EXISTS(SELECT * FROM dbo.SchemaVersion WHERE (MajorVersion = @majorVersion) AND (MinorVersion = @minorVersion))
BEGIN
  BEGIN TRANSACTION Version1_2

    CREATE TABLE [dbo].[SystemConfigs](
      [Id]           [uniqueidentifier] NOT NULL,
      [MailServer]   [varchar](75)          NULL,
      [ServerPort]   [int]                  NULL,
      [SmtpUser]     [varchar](75)          NULL,
      [SmtpPassword] [varchar](256)         NULL,
      [FromEmail]    [varchar](75)          NULL,
      [FromUsername] [varchar](75)          NULL,
      [RequireSsl]   [bit]              NOT NULL,
      [RequireAuth]  [bit]              NOT NULL
    CONSTRAINT PK_SystemConfigs PRIMARY KEY CLUSTERED (
        [Id] ASC
      ) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
    ) ON [PRIMARY]

    ALTER TABLE [dbo].[SystemConfigs] ADD  CONSTRAINT [DF_SystemConfigs_Id]  DEFAULT (newid()) FOR [Id];
    ALTER TABLE [dbo].[SystemConfigs] ADD  DEFAULT ((1)) FOR [RequireSsl];
    ALTER TABLE [dbo].[SystemConfigs] ADD  DEFAULT ((1)) FOR [RequireAuth];


    INSERT INTO [dbo].[SystemConfigs] ([MailServer],[ServerPort],[SmtpUser],[SmtpPassword],[FromEmail],[FromUsername])
    VALUES ('',25,'','','','');

    INSERT INTO dbo.SchemaVersion (Id, MajorVersion, MinorVersion, InstallDate) values (newid(), @majorVersion, @minorVersion, getutcdate());
  COMMIT TRANSACTION Version1_2
END

SELECT @majorVersion = 1, @minorVersion = 3;
IF NOT EXISTS(SELECT * FROM dbo.SchemaVersion WHERE (MajorVersion = @majorVersion) AND (MinorVersion = @minorVersion))
BEGIN
  BEGIN TRANSACTION Version1_3
    
    CREATE TABLE dbo.Member (
      Id             INT           NOT NULL IDENTITY(1,1),
      Prefix         VARCHAR(10)       NULL,
      FirstName      VARCHAR(150)  NOT NULL,
      MiddleName     VARCHAR(50)       NULL,
      LastName       VARCHAR(150)  NOT NULL,
      Suffix         VARCHAR(10)       NULL,
      ModifiedDate   SMALLDATETIME NOT NULL DEFAULT GETDATE(),
      CreateDate     SMALLDATETIME NOT NULL DEFAULT GETDATE()
    CONSTRAINT PK_Member PRIMARY KEY CLUSTERED (
        [Id] ASC
      ) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
    ) ON [PRIMARY];

    CREATE TABLE dbo.MemberAddress (
      Id             INT           NOT NULL IDENTITY(1,1),
      Address1       VARCHAR(150)  NOT NULL,
      Address2       VARCHAR(150)      NULL,
      City           VARCHAR(150)  NOT NULL,
      StateId        INT           NOT NULL,
      Zip            VARCHAR(10)   NOT NULL,
      ModifiedDate   SMALLDATETIME NOT NULL DEFAULT GETDATE(),
      CreateDate     SMALLDATETIME NOT NULL DEFAULT GETDATE(),
    CONSTRAINT PK_MemberAddress PRIMARY KEY CLUSTERED (
        [Id] ASC
      ) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
    ) ON [PRIMARY];

    CREATE TABLE dbo.PhoneType (
      Id             INT           NOT NULL IDENTITY(1,1),
      PhoneType      VARCHAR(15)   NOT NULL,
    CONSTRAINT PK_PhoneType PRIMARY KEY CLUSTERED (
        [Id] ASC
      ) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
    ) ON [PRIMARY];

    INSERT INTO dbo.PhoneType (PhoneType)
    VALUES ('Main'),('Cellular'),('Home'),('Unknown')

    CREATE TABLE dbo.MemberPhone (
      Id             INT           NOT NULL IDENTITY(1,1),
      MemberId       INT           NOT NULL,
      Phone          VARCHAR(15)   NOT NULL,
      TypeId         INT           NOT NULL,
      ModifiedDate   SMALLDATETIME NOT NULL DEFAULT GETDATE(),
      CreateDate     SMALLDATETIME NOT NULL DEFAULT GETDATE(),
    CONSTRAINT PK_MemberPhone PRIMARY KEY CLUSTERED (
        [Id] ASC
      ) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
    ) ON [PRIMARY];

    ALTER TABLE [dbo].[MemberPhone] WITH CHECK ADD FOREIGN KEY([MemberId]) REFERENCES [dbo].[Member] ([Id]);
    ALTER TABLE [dbo].[MemberPhone] WITH CHECK ADD FOREIGN KEY([TypeId])   REFERENCES [dbo].[PhoneType] ([Id]);
    ALTER TABLE [dbo].[MemberPhone] ADD  DEFAULT ((1)) FOR [TypeId];


    
    INSERT INTO dbo.SchemaVersion (Id, MajorVersion, MinorVersion, InstallDate) values (newid(), @majorVersion, @minorVersion, getutcdate());
  COMMIT TRANSACTION Version1_3
END

SELECT @majorVersion = 1, @minorVersion = 4;
IF NOT EXISTS(SELECT * FROM dbo.SchemaVersion WHERE (MajorVersion = @majorVersion) AND (MinorVersion = @minorVersion))
BEGIN
  BEGIN TRANSACTION Version1_4

    ALTER TABLE dbo.SchemaVersion ADD [Description] VARCHAR(2000) NULL;

    INSERT INTO dbo.SchemaVersion (Id, MajorVersion, MinorVersion, InstallDate) values (newid(), @majorVersion, @minorVersion, getutcdate());
  COMMIT TRANSACTION Version1_4
END

SELECT @majorVersion = 1, @minorVersion = 5;
IF NOT EXISTS(SELECT * FROM dbo.SchemaVersion WHERE (MajorVersion = @majorVersion) AND (MinorVersion = @minorVersion))
BEGIN
  BEGIN TRANSACTION Version1_5

    ALTER TABLE dbo.States ADD CONSTRAINT [PK_State_Id] PRIMARY KEY CLUSTERED (
        [Id] ASC
      ) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY];
    

    ALTER TABLE dbo.MemberAddress DROP CONSTRAINT FK__MemberAdd__Membe__534D60F1;
    ALTER TABLE dbo.MemberAddress DROP COLUMN MemberId;
    -- Need to drop the default constraint for column ModifiedDate.
    ALTER TABLE dbo.MemberAddress DROP CONSTRAINT DF__MemberAdd__Modif__5165187F;
    ALTER TABLE dbo.MemberAddress DROP COLUMN ModifiedDate;
    -- Need to drop the default constraint for column CreateDate.
    ALTER TABLE dbo.MemberAddress DROP CONSTRAINT DF__MemberAdd__Creat__52593CB8;
    ALTER TABLE dbo.MemberAddress DROP COLUMN CreateDate;
    ALTER TABLE dbo.MemberAddress ADD CONSTRAINT FK_State_Id FOREIGN KEY (StateId) REFERENCES dbo.States (Id);
    SELECT @Notes = ' Cleanup MemberAddress table;';

    CREATE TABLE dbo.EmailType (
      Id             INT           NOT NULL IDENTITY(1,1),
      EmailType      VARCHAR(15)   NOT NULL,
    CONSTRAINT PK_EmailType PRIMARY KEY CLUSTERED (
        [Id] ASC
      ) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
    ) ON [PRIMARY];
    SELECT @Notes = @Notes + ' Add EmailType Table;';

    CREATE TABLE dbo.MemberEmail (
      Id             INT           NOT NULL IDENTITY(1,1),
      EmailAddress   VARCHAR(150)  NOT NULL,
      EmailType      INT           NOT NULL,
    CONSTRAINT PK_MemberEmail PRIMARY KEY CLUSTERED (
        [Id] ASC
      ) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
    ) ON [PRIMARY];

    ALTER TABLE dbo.MemberEmail ADD CONSTRAINT FK_MemberEmail_Type FOREIGN KEY (EmailType) REFERENCES dbo.EmailType (Id);
    SELECT @Notes = @Notes + ' Add MemberEmail Table;';

    
    INSERT INTO dbo.SchemaVersion (Id, MajorVersion, MinorVersion, InstallDate, Description) values (newid(), @majorVersion, @minorVersion, getutcdate(), @Notes);
  COMMIT TRANSACTION Version1_5
END
    
SELECT @majorVersion = 1, @minorVersion = 6;
IF NOT EXISTS(SELECT * FROM dbo.SchemaVersion WHERE (MajorVersion = @majorVersion) AND (MinorVersion = @minorVersion))
BEGIN
  BEGIN TRANSACTION Version1_6

    ALTER TABLE dbo.MemberPhone DROP CONSTRAINT FK__MemberPho__Membe__59FA5E80;
    ALTER TABLE dbo.MemberPhone DROP COLUMN MemberId;    
    -- Need to drop the default constraint for column ModifiedDate.
    ALTER TABLE dbo.MemberPhone DROP CONSTRAINT DF__MemberPho__Modif__5812160E;    
    ALTER TABLE dbo.MemberPhone DROP COLUMN ModifiedDate;    
    -- Need to drop the default constraint for column CreateDate.
    ALTER TABLE dbo.MemberPhone DROP CONSTRAINT DF__MemberPho__Creat__59063A47;    
    ALTER TABLE dbo.MemberPhone DROP COLUMN CreateDate;
    SELECT @Notes = ' Cleanup MemberPhone table;';

    CREATE TABLE dbo.Xref_Member_Address (
      MemberId        INT NOT NULL,
      MemberAddressId INT NOT NULL,
      IsPrimary       BIT NOT NULL,
    CONSTRAINT PK_Member_MemberAddress PRIMARY KEY CLUSTERED (
        [MemberId], [MemberAddressId] ASC
      ) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
    ) ON [PRIMARY];

    ALTER TABLE dbo.Xref_Member_Address ADD DEFAULT (0) FOR [IsPrimary];
    SELECT @Notes = @Notes +' Add XRef for Member->Address;';

    CREATE TABLE dbo.Xref_Member_Phone (
      MemberId        INT NOT NULL,
      MemberPhoneId   INT NOT NULL,
    CONSTRAINT PK_Member_MemberPhone PRIMARY KEY CLUSTERED (
        [MemberId], [MemberPhoneId] ASC
      ) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
    ) ON [PRIMARY];
    SELECT @Notes = @Notes +' Add XRef for Member->Phone;';

    CREATE TABLE dbo.Xref_Member_Email (
      MemberId        INT NOT NULL,
      MemberEmailId   INT NOT NULL,
    CONSTRAINT PK_Member_MemberEmail PRIMARY KEY CLUSTERED (
        [MemberId], [MemberEmailId] ASC
      ) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
    ) ON [PRIMARY];
    SELECT @Notes = @Notes +' Add XRef for Member->Email;';

    INSERT INTO dbo.SchemaVersion (Id, MajorVersion, MinorVersion, InstallDate, Description) values (newid(), @majorVersion, @minorVersion, getutcdate(), @Notes);
  COMMIT TRANSACTION Version1_6
END

SELECT @majorVersion = 1, @minorVersion = 7;
IF NOT EXISTS(SELECT * FROM dbo.SchemaVersion WHERE (MajorVersion = @majorVersion) AND (MinorVersion = @minorVersion))
BEGIN
  BEGIN TRANSACTION Version1_7
    SELECT @Notes = '';

    CREATE TABLE dbo.MaritalStatus (
      Id             INT           NOT NULL IDENTITY(1,1),
      MaritalStatus  VARCHAR(25)   NOT NULL,
    CONSTRAINT PK_MaritalStatus PRIMARY KEY CLUSTERED (
        [Id] ASC
      ) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
    ) ON [PRIMARY];

    INSERT INTO dbo.MaritalStatus VALUES ('Unknown'),('Single'),('Separated'),('Married'),('Divorced'),('Widowed');
    SELECT @Notes = @Notes + ' Add MaritalStatus Table;';

    ALTER TABLE dbo.Member ADD 
      DateOfBirth     SMALLDATETIME     NULL,
      MaritalStatusId INT           NOT NULL DEFAULT 1,
      MarriageDate    SMALLDATETIME     NULL;
    ALTER TABLE dbo.Member ADD CONSTRAINT FK_MaritalStatus_Type FOREIGN KEY (MaritalStatusId) REFERENCES dbo.MaritalStatus (Id);
    SELECT @Notes = @Notes + ' Add Dates and MaritalStatus reference to Member Table;';
    
    CREATE TABLE dbo.RelationshipType (
      Id                INT           NOT NULL IDENTITY(1,1),
      RelationshipType  VARCHAR(25)   NOT NULL,
    CONSTRAINT PK_RelationshipType PRIMARY KEY CLUSTERED (
        [Id] ASC
      ) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
    ) ON [PRIMARY];

    INSERT INTO dbo.RelationshipType VALUES ('Spouce'),('Child'),('Sibling');
    SELECT @Notes = @Notes + ' Add RelationshipType Table;';

    CREATE TABLE dbo.Xref_Member_Member (
      MemberId           INT NOT NULL,
      RelatedId          INT NOT NULL,
      RelationshipTypeId INT     NULL,
    ) ON [PRIMARY];
    SELECT @Notes = @Notes +' Add XRef for Member->Member->Relationship;';

    INSERT INTO dbo.SchemaVersion (Id, MajorVersion, MinorVersion, InstallDate, Description) values (newid(), @majorVersion, @minorVersion, getutcdate(), @Notes);
  COMMIT TRANSACTION Version1_7
END

SELECT @majorVersion = 1, @minorVersion = 8;
IF NOT EXISTS(SELECT * FROM dbo.SchemaVersion WHERE (MajorVersion = @majorVersion) AND (MinorVersion = @minorVersion))
BEGIN
  BEGIN TRANSACTION Version1_8
    SELECT @Notes = '';

    CREATE TABLE dbo.Salutation (
      Id             INT           NOT NULL IDENTITY(1,1),
      Salutation     VARCHAR(25)   NOT NULL,
    CONSTRAINT PK_Salutation PRIMARY KEY CLUSTERED (
        [Id] ASC
      ) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
    ) ON [PRIMARY];

    INSERT INTO dbo.Salutation VALUES ('Mr'),('Mrs'),('Ms'),('Dr');
    SELECT @Notes = @Notes + ' Add Salutation Table;';

    ALTER TABLE dbo.Member ADD 
      SalutationId INT NOT NULL DEFAULT 1,
      Gender       INT NOT NULL DEFAULT 0;

    ALTER TABLE dbo.Member ADD CONSTRAINT FK_Salutation_Type FOREIGN KEY (SalutationId) REFERENCES dbo.Salutation (Id);
    SELECT @Notes = @Notes + ' Add Salutation and Gender reference to Member Table;';
    
    INSERT INTO dbo.SchemaVersion (Id, MajorVersion, MinorVersion, InstallDate, Description) values (newid(), @majorVersion, @minorVersion, getutcdate(), @Notes);
  COMMIT TRANSACTION Version1_8
END

SELECT @majorVersion = 1, @minorVersion = 9;
IF NOT EXISTS(SELECT * FROM dbo.SchemaVersion WHERE (MajorVersion = @majorVersion) AND (MinorVersion = @minorVersion))
BEGIN
  BEGIN TRANSACTION Version1_9
    SELECT @Notes = '';

    ALTER TABLE dbo.MemberAddress ADD 
      IsPrimary BIT NOT NULL DEFAULT 1;
    SELECT @Notes = @Notes + ' Add Primary Flag to MemberAddress Table;';
    
    INSERT INTO dbo.SchemaVersion (Id, MajorVersion, MinorVersion, InstallDate, Description) values (newid(), @majorVersion, @minorVersion, getutcdate(), @Notes);
  COMMIT TRANSACTION Version1_9
END

SELECT @majorVersion = 1, @minorVersion = 10;
IF NOT EXISTS(SELECT * FROM dbo.SchemaVersion WHERE (MajorVersion = @majorVersion) AND (MinorVersion = @minorVersion))
BEGIN
  BEGIN TRANSACTION Version1_10
    SELECT @Notes = '';

    ALTER TABLE dbo.MemberAddress DROP COLUMN IsPrimary;
    SELECT @Notes = @Notes + ' Remove Primary Flag from MemberAddress Table;';
    
    ALTER TABLE dbo.Xref_Member_Phone ADD 
      IsPrimary BIT NOT NULL DEFAULT 0;
    SELECT @Notes = @Notes + ' Add Primary Flag to Xref_Member_Phone Table;';

    INSERT INTO dbo.SchemaVersion (Id, MajorVersion, MinorVersion, InstallDate, Description) values (newid(), @majorVersion, @minorVersion, getutcdate(), @Notes);
  COMMIT TRANSACTION Version1_10
END

SELECT @majorVersion = 1, @minorVersion = 11;
IF NOT EXISTS(SELECT * FROM dbo.SchemaVersion WHERE (MajorVersion = @majorVersion) AND (MinorVersion = @minorVersion))
BEGIN
  BEGIN TRANSACTION Version1_11
    SELECT @Notes = '';

    CREATE TABLE dbo.MemberNotes (
      Id             INT           NOT NULL IDENTITY(1,1),
      MemberId       INT           NOT NULL,
      Notes          VARCHAR(2000)     NULL,
      CreateDate     SMALLDATETIME NOT NULL DEFAULT GETDATE(),
    CONSTRAINT PK_MemberNotes PRIMARY KEY CLUSTERED (
        [Id] ASC
      ) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
    ) ON [PRIMARY];

    SELECT @Notes = @Notes + ' Add notes table for Members;';
    
    INSERT INTO dbo.SchemaVersion (Id, MajorVersion, MinorVersion, InstallDate, Description) values (newid(), @majorVersion, @minorVersion, getutcdate(), @Notes);
  COMMIT TRANSACTION Version1_11
END

SELECT @majorVersion = 1, @minorVersion = 12;
IF NOT EXISTS(SELECT * FROM dbo.SchemaVersion WHERE (MajorVersion = @majorVersion) AND (MinorVersion = @minorVersion))
BEGIN
  BEGIN TRANSACTION Version1_12
    SELECT @Notes = '';

    ALTER TABLE dbo.MemberEmail DROP COLUMN EmailType;
    DROP TABLE dbo.EmailType;
    SELECT @Notes = @Notes + ' Removing email types for Members;';
    
    INSERT INTO dbo.SchemaVersion (Id, MajorVersion, MinorVersion, InstallDate, Description) values (newid(), @majorVersion, @minorVersion, getutcdate(), @Notes);
  COMMIT TRANSACTION Version1_12
END

SELECT @majorVersion = 1, @minorVersion = 13;
IF NOT EXISTS(SELECT * FROM dbo.SchemaVersion WHERE (MajorVersion = @majorVersion) AND (MinorVersion = @minorVersion))
BEGIN
  BEGIN TRANSACTION Version1_13
    SELECT @Notes = '';

    ALTER TABLE dbo.MemberNotes ADD
      UserId INT NOT NULL DEFAULT 1;
    ALTER TABLE dbo.MemberNotes ADD CONSTRAINT FK_AdminUser_Id FOREIGN KEY (UserId) REFERENCES dbo.AdminUsers (Id);
    SELECT @Notes = @Notes + ' Removing email types for Members;';
    
    INSERT INTO dbo.SchemaVersion (Id, MajorVersion, MinorVersion, InstallDate, Description) values (newid(), @majorVersion, @minorVersion, getutcdate(), @Notes);
  COMMIT TRANSACTION Version1_13
END
/* 
  Use this model to create database changes
  Just change NEWVERSION to the next number in the sequence

SELECT @majorVersion = 1, @minorVersion = NEWVERSION;
IF NOT EXISTS(SELECT * FROM dbo.SchemaVersion WHERE (MajorVersion = @majorVersion) AND (MinorVersion = @minorVersion))
BEGIN
  BEGIN TRANSACTION Version1_NEWVERSION
    SELECT @Notes = '';

    INSERT INTO dbo.SchemaVersion (Id, MajorVersion, MinorVersion, InstallDate, Description) values (newid(), @majorVersion, @minorVersion, getutcdate(), @Notes);
  COMMIT TRANSACTION Version1_NEWVERSION
END
*/