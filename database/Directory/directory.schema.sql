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

-- Start the main transaction: Initialize the database with SchemaVersion table and data
BEGIN TRANSACTION initialCreate;
  IF NOT EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'[SchemaVersion]')) 
  BEGIN
    CREATE TABLE [dbo].[SchemaVersion](
      Id           UNIQUEIDENTIFIER  NOT NULL DEFAULT NEWID(),
      MajorVersion INT               NOT NULL,
      MinorVersion INT               NOT NULL,
      InstallDate  DATETIMEOFFSET(7) NOT NULL
    CONSTRAINT PK_SchemaVersion PRIMARY KEY CLUSTERED 
      (MajorVersion, MinorVersion ASC) WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
    ) ON [PRIMARY]
  END
  
  IF(@@ERROR <> 0)
  BEGIN
    ROLLBACK TRANSACTION;
    RETURN;
  END

  SET @majorVersion = 1;
  SET @minorVersion = 0;
  IF NOT EXISTS(SELECT * FROM SchemaVersion WHERE (MajorVersion = @majorVersion) AND (MinorVersion = @minorVersion))
  BEGIN
    INSERT INTO SchemaVersion values (newid(), @majorVersion, @minorVersion, getutcdate());
  END
  
  IF(@@ERROR <> 0)
  BEGIN
    ROLLBACK TRANSACTION;
    RETURN;
  END
COMMIT TRANSACTION initialCreate;

-- Begin database modifications here
SELECT @majorVersion = 1, @minorVersion = 1;
IF NOT EXISTS(SELECT * FROM SchemaVersion WHERE (MajorVersion = @majorVersion) AND (MinorVersion = @minorVersion))
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

    ALTER TABLE [dbo].[SystemExceptions] ADD  CONSTRAINT [DF_SystemExceptions_Id]  DEFAULT (newid()) FOR [Id];

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
    
    INSERT INTO SchemaVersion values (newid(), @majorVersion, @minorVersion, getutcdate());
  COMMIT TRANSACTION Version1_1
END

SELECT @majorVersion = 1, @minorVersion = 2;
IF NOT EXISTS(SELECT * FROM SchemaVersion WHERE (MajorVersion = @majorVersion) AND (MinorVersion = @minorVersion))
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

    INSERT INTO SchemaVersion values (newid(), @majorVersion, @minorVersion, getutcdate());
  COMMIT TRANSACTION Version1_2
END

/* 
  Use this model to create database changes
  Just change NEWVERSION to the next number in the sequence

SELECT @majorVersion = 1, @minorVersion = NEWVERSION;
IF NOT EXISTS(SELECT * FROM SchemaVersion WHERE (MajorVersion = @majorVersion) AND (MinorVersion = @minorVersion))
BEGIN
  BEGIN TRANSACTION Version1_NEWVERSION

    -- Create default user
    INSERT INTO [dbo].[AdminUsers] ([Id], [RoleId], [DisplayName], [UserName], [UserPass], [Notes], 
           [Deleted], [SuperAdmin], [PasswordReset])
    VALUES (newid(),'b6522703-8844-4cff-8fc1-916ba90f515b','David Nuckolls','dlnuckolls@gmail.com','5zO9V86nJwv4G04yfZ/V++IhIqOytb8ot3XcpsxjfPw=','System Created',0,1,0);

    -- Add tables for dynamic page content
    CREATE TABLE [dbo].[PageLocations] (
      [Id]          UNIQUEIDENTIFIER NOT NULL,
      [Description] VARCHAR(255)         NULL,
      CONSTRAINT [PK_PageLocations] PRIMARY KEY CLUSTERED ( 
        [Id] ASC
      ) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
    ) ON [PRIMARY];

    ALTER TABLE [dbo].[PageLocations] ADD  CONSTRAINT [DF_PageLocations_Id]  DEFAULT (NEWID()) FOR [Id];

    CREATE TABLE [dbo].[PageContent](
      [Id]           UNIQUEIDENTIFIER NOT NULL,
      [PageLocation] UNIQUEIDENTIFIER NOT NULL,
      [Description]  VARCHAR(max)         NULL,
      CONSTRAINT [PK_PageContent] PRIMARY KEY CLUSTERED (
        [Id] ASC
      ) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
    ) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY];

    ALTER TABLE [dbo].[PageContent] ADD  CONSTRAINT [DF_PageContent_Id]  DEFAULT (NEWID()) FOR [Id];

    ALTER TABLE [dbo].[PageContent] WITH CHECK ADD FOREIGN KEY([PageLocation]) REFERENCES [dbo].[PageLocations] ([Id]);

    INSERT INTO SchemaVersion values (newid(), @majorVersion, @minorVersion, getutcdate());
  COMMIT TRANSACTION Version1_NEWVERSION
END
*/
