/*
  Database starter script
    to use this script rename the file to match you database 
    and replace [databasename] with your database name
 */

USE Master;
SET NOCOUNT ON;

IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = N'[databasename]') BEGIN
	DECLARE @tail       INT
	DECLARE @basefolder NVARCHAR(MAX)
	DECLARE @datafile   NVARCHAR(MAX)
	DECLARE @logfile    NVARCHAR(MAX)

	SET @tail = (SELECT CHARINDEX('\',REVERSE(physical_name)) FROM Master.sys.master_files WHERE name = 'master')

	SET @basefolder = (SELECT SUBSTRING(physical_name,1,LEN(physical_name)-@tail) FROM Master.sys.master_files WHERE name = 'master')

	SET @datafile = @basefolder + '\[databasename].mdf'
	SET @logfile = @basefolder + '\[databasename]_log.ldf'

	DECLARE @sql NVARCHAR(MAX)
	SET @sql = 'CREATE DATABASE [[databasename]] ON PRIMARY ' +
	'( NAME = N''[databasename]'', FILENAME = ''' + @datafile + ''', SIZE = 4096KB , MAXSIZE = UNLIMITED, FILEGROWTH = 1024KB ) ' +
	 ' LOG ON ' + 
	'( NAME = N''[databasename]_log'', FILENAME = ''' + @logfile + ''', SIZE = 1024KB , MAXSIZE = 2048GB , FILEGROWTH = 10%)'
	EXEC(@sql)
	
	ALTER DATABASE [databasename] SET COMPATIBILITY_LEVEL = 100;
	ALTER DATABASE [databasename] SET ANSI_NULL_DEFAULT OFF;
	ALTER DATABASE [databasename] SET ANSI_NULLS OFF;
	ALTER DATABASE [databasename] SET ANSI_PADDING OFF; 
	ALTER DATABASE [databasename] SET ANSI_WARNINGS OFF; 
	ALTER DATABASE [databasename] SET ARITHABORT OFF; 
	ALTER DATABASE [databasename] SET AUTO_CLOSE OFF; 
	ALTER DATABASE [databasename] SET AUTO_CREATE_STATISTICS ON; 
	ALTER DATABASE [databasename] SET AUTO_SHRINK OFF; 
	ALTER DATABASE [databasename] SET AUTO_UPDATE_STATISTICS ON; 
	ALTER DATABASE [databasename] SET CURSOR_CLOSE_ON_COMMIT OFF; 
	ALTER DATABASE [databasename] SET CURSOR_DEFAULT  GLOBAL; 
	ALTER DATABASE [databasename] SET CONCAT_NULL_YIELDS_NULL OFF; 
	ALTER DATABASE [databasename] SET NUMERIC_ROUNDABORT OFF; 
	ALTER DATABASE [databasename] SET QUOTED_IDENTIFIER OFF; 
	ALTER DATABASE [databasename] SET RECURSIVE_TRIGGERS OFF; 
	ALTER DATABASE [databasename] SET AUTO_UPDATE_STATISTICS_ASYNC OFF; 
	ALTER DATABASE [databasename] SET DATE_CORRELATION_OPTIMIZATION OFF; 
	ALTER DATABASE [databasename] SET TRUSTWORTHY OFF; 
	ALTER DATABASE [databasename] SET ALLOW_SNAPSHOT_ISOLATION OFF; 
	ALTER DATABASE [databasename] SET PARAMETERIZATION SIMPLE; 
	ALTER DATABASE [databasename] SET READ_WRITE; 
	ALTER DATABASE [databasename] SET RECOVERY SIMPLE; 
	ALTER DATABASE [databasename] SET MULTI_USER; 
	ALTER DATABASE [databasename] SET PAGE_VERIFY CHECKSUM;  
	ALTER DATABASE [databasename] SET DB_CHAINING OFF; 
		
     IF(@@ERROR <> 0) BEGIN
       RETURN
     END
END
GO

-- Start the main transaction: Initialize the database with SchemaVersion table and data
BEGIN TRANSACTION initialCreate;
	USE [databasename];

	IF NOT EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'[SchemaVersion]')) 
	BEGIN
		CREATE TABLE [dbo].[SchemaVersion](
			[Id] [uniqueidentifier] NOT NULL default newid(),
			[MajorVersion] [int] NOT NULL,
			[MinorVersion] [int] NOT NULL,
			[InstallDate] [datetimeoffset](7) NOT NULL
		 CONSTRAINT [PK_SchemaVersion] PRIMARY KEY CLUSTERED 
			([Id] ASC ) WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
		) ON [PRIMARY]
	END
	
	IF(@@ERROR <> 0)
	BEGIN
		ROLLBACK TRANSACTION;
		RETURN;
	END

	DECLARE @majorVersion INT;
	DECLARE @minorVersion INT;

	SET @majorVersion = 1;
	SET @minorVersion = 0;
	IF(NOT(EXISTS(SELECT (1) FROM SchemaVersion WHERE (MajorVersion = @majorVersion) AND (MinorVersion = @minorVersion))))
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
IF(NOT(EXISTS(SELECT (1) FROM SchemaVersion WHERE (MajorVersion = 1) AND (MinorVersion = 1))))
BEGIN
	BEGIN TRANSACTION Version1_1

		-- Do DDL Work...

		INSERT INTO SchemaVersion values (newid(), 1, 1, getutcdate());
	COMMIT TRANSACTION Version1_1
END