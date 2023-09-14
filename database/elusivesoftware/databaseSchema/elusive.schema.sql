/*
    Elusive Software Core Database
 	  Username: elusive_user
    Password: XJ$7d{84)sa(kV4
  */ 
 USE Master;
 SET NOCOUNT ON;
 
 IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = N'ElusiveSoftware') BEGIN
   DECLARE @tail       INT
   DECLARE @basefolder NVARCHAR(MAX)
   DECLARE @datafile   NVARCHAR(MAX)
   DECLARE @logfile    NVARCHAR(MAX)
 
   SET @tail = (SELECT CHARINDEX('\',REVERSE(physical_name)) FROM Master.sys.master_files WHERE name = 'master')
   SET @basefolder = (SELECT SUBSTRING(physical_name,1,LEN(physical_name)-@tail) FROM Master.sys.master_files WHERE name = 'master')
   SET @datafile = @basefolder + '\ElusiveSoftware.mdf'
   SET @logfile = @basefolder + '\ElusiveSoftware.ldf'
   DECLARE @sql NVARCHAR(MAX)
   SET @sql = 'CREATE DATABASE [ElusiveSoftware] ON PRIMARY ' +
   '( NAME = N''ElusiveSoftware'', FILENAME = ''' + @datafile + ''', SIZE = 4096KB , MAXSIZE = UNLIMITED, FILEGROWTH = 1024KB ) ' +
    ' LOG ON ' + 
   '( NAME = N''ElusiveSoftware_log'', FILENAME = ''' + @logfile + ''', SIZE = 1024KB , MAXSIZE = 2048GB , FILEGROWTH = 10%)'
   EXEC(@sql)
   
   ALTER DATABASE ElusiveSoftware SET COMPATIBILITY_LEVEL = 100;
   ALTER DATABASE ElusiveSoftware SET ANSI_NULL_DEFAULT OFF;
   ALTER DATABASE ElusiveSoftware SET ANSI_NULLS OFF;
   ALTER DATABASE ElusiveSoftware SET ANSI_PADDING OFF; 
   ALTER DATABASE ElusiveSoftware SET ANSI_WARNINGS OFF; 
   ALTER DATABASE ElusiveSoftware SET ARITHABORT OFF; 
   ALTER DATABASE ElusiveSoftware SET AUTO_CLOSE OFF; 
   ALTER DATABASE ElusiveSoftware SET AUTO_CREATE_STATISTICS ON; 
   ALTER DATABASE ElusiveSoftware SET AUTO_SHRINK OFF; 
   ALTER DATABASE ElusiveSoftware SET AUTO_UPDATE_STATISTICS ON; 
   ALTER DATABASE ElusiveSoftware SET CURSOR_CLOSE_ON_COMMIT OFF; 
   ALTER DATABASE ElusiveSoftware SET CURSOR_DEFAULT  GLOBAL; 
   ALTER DATABASE ElusiveSoftware SET CONCAT_NULL_YIELDS_NULL OFF; 
   ALTER DATABASE ElusiveSoftware SET NUMERIC_ROUNDABORT OFF; 
   ALTER DATABASE ElusiveSoftware SET QUOTED_IDENTIFIER OFF; 
   ALTER DATABASE ElusiveSoftware SET RECURSIVE_TRIGGERS OFF; 
   ALTER DATABASE ElusiveSoftware SET AUTO_UPDATE_STATISTICS_ASYNC OFF; 
   ALTER DATABASE ElusiveSoftware SET DATE_CORRELATION_OPTIMIZATION OFF; 
   ALTER DATABASE ElusiveSoftware SET TRUSTWORTHY OFF; 
   ALTER DATABASE ElusiveSoftware SET ALLOW_SNAPSHOT_ISOLATION OFF; 
   ALTER DATABASE ElusiveSoftware SET PARAMETERIZATION SIMPLE; 
   ALTER DATABASE ElusiveSoftware SET READ_WRITE; 
   ALTER DATABASE ElusiveSoftware SET RECOVERY SIMPLE; 
   ALTER DATABASE ElusiveSoftware SET MULTI_USER; 
   ALTER DATABASE ElusiveSoftware SET PAGE_VERIFY CHECKSUM;  
   ALTER DATABASE ElusiveSoftware SET DB_CHAINING OFF; 
     
      IF(@@ERROR <> 0) BEGIN
        RETURN
      END
 END
 GO
 
USE ElusiveSoftware;

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
		  [Id]       [uniqueidentifier] NOT NULL,
		  [RoleName] [varchar](50)          NULL,
		  [Notes]    [varchar](250)         NULL,
	  CONSTRAINT [PK_AdminRoles] PRIMARY KEY CLUSTERED (
			  [Id] ASC
		  ) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
	  ) ON [PRIMARY];
	
	  ALTER TABLE [dbo].[AdminRoles] ADD  DEFAULT (newid()) FOR [Id];

	  CREATE TABLE [dbo].[AdminUsers](
		  [Id]            [uniqueidentifier] NOT NULL,
		  [RoleId]        [uniqueidentifier] NOT NULL,
		  [DisplayName]   [varchar](50)          NULL,
		  [UserName]      [varchar](50)      NOT NULL,
		  [UserPass]      [varchar](256)         NULL,
		  [Notes]         [varchar](250)         NULL,
		  [Deleted]       [tinyint]          NOT NULL,
		  [SuperAdmin]    [bit]              NOT NULL,
		  [PasswordReset] [bit]              NOT NULL,
	  CONSTRAINT [PK_AdminUsers] PRIMARY KEY CLUSTERED (
			  [Id] ASC
		  ) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
	  ) ON [PRIMARY];

	  ALTER TABLE [dbo].[AdminUsers] ADD  DEFAULT (newid()) FOR [Id];
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
    ) ON [PRIMARY]

    ALTER TABLE [dbo].[SystemConfigs] ADD  DEFAULT ((1)) FOR [RequireSsl];
    ALTER TABLE [dbo].[SystemConfigs] ADD  DEFAULT ((1)) FOR [RequireAuth];

	  CREATE TABLE [dbo].[States] (
      [Id]           INT         NOT NULL IDENTITY(1,1),
	    [State]        VARCHAR(50) NOT NULL,
      [Abbreviation] VARCHAR(2)  NOT NULL,
      CONSTRAINT PK_States PRIMARY KEY CLUSTERED (
        [Id] ASC
      ) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
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

    INSERT INTO [dbo].[AdminRoles] (Id, RoleName, Notes) 
    VALUES ('a7107c8f-ff15-4d5a-bba1-6db286fcef0a','User','User level access'),
           ('b6522703-8844-4cff-8fc1-916ba90f515b','System User','System Administrator');
    
    -- Create default user
    INSERT INTO [dbo].[AdminUsers] ([Id], [RoleId], [DisplayName], [UserName], [UserPass], [Notes], 
           [Deleted], [SuperAdmin], [PasswordReset])
    VALUES (newid(),'b6522703-8844-4cff-8fc1-916ba90f515b','David Nuckolls','dlnuckolls@gmail.com','5zO9V86nJwv4G04yfZ/V++IhIqOytb8ot3XcpsxjfPw=','System Created',0,1,0);

    INSERT INTO [dbo].[SystemConfigs] ([Id],[MailServer],[ServerPort],[SmtpUser],[SmtpPassword],[FromEmail],[FromUsername],[RequireSsl],[RequireAuth])
    VALUES (NEWID(),'glorykidd.com',25,'social@elusivesoftware.net','','postmaster@elusivesoftware.net','Elusive Software Postmaster',1,1)

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

    CREATE TABLE [dbo].[Appointments] (  
      [Id]                    INT           IDENTITY(1,1) NOT NULL,  
      [Subject]               VARCHAR(255)                NOT NULL,  
      [Description]           VARCHAR(1024)                   NULL,  
      [Start]                 DATETIME                    NOT NULL,  
      [End]                   DATETIME                    NOT NULL,
      [RecurrenceRule]        VARCHAR(1024)                   NULL,  
      [RecurrenceParentId]    INT                             NULL,  
      [Reminder]              VARCHAR(255)                    NULL,  
      [Annotations]           VARCHAR(50)                     NULL,  
      CONSTRAINT [PK_Appointments] PRIMARY KEY CLUSTERED ([Id]),  
      CONSTRAINT [FK_Appointments_ParentAppointments] FOREIGN KEY ([RecurrenceParentId]) REFERENCES [Appointments] ([Id]), 
    ) ON [PRIMARY];

    INSERT INTO SchemaVersion values (newid(), @majorVersion, @minorVersion, getutcdate());
  COMMIT TRANSACTION Version1_1
END

/* 
  Use this model to create database changes
  Just change NEWVERSION to the next number in the sequence

SELECT @majorVersion = 1, @minorVersion = NEWVERSION;
IF NOT EXISTS(SELECT * FROM SchemaVersion WHERE (MajorVersion = @majorVersion) AND (MinorVersion = @minorVersion))
BEGIN
  BEGIN TRANSACTION Version1_NEWVERSION

    INSERT INTO SchemaVersion values (newid(), @majorVersion, @minorVersion, getutcdate());
  COMMIT TRANSACTION Version1_NEWVERSION
END
*/