USE [master]
GO
/****** Object:  Database [Library]    Script Date: 4/19/2022 9:52:37 PM ******/
CREATE DATABASE [Library]
 CONTAINMENT = NONE
 ON  PRIMARY 
( NAME = N'Library', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL15.SQLEXPRESS\MSSQL\DATA\Library.mdf' , SIZE = 8192KB , MAXSIZE = UNLIMITED, FILEGROWTH = 65536KB )
 LOG ON 
( NAME = N'Library_log', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL15.SQLEXPRESS\MSSQL\DATA\Library_log.ldf' , SIZE = 8192KB , MAXSIZE = 2048GB , FILEGROWTH = 65536KB )
 WITH CATALOG_COLLATION = DATABASE_DEFAULT
GO
ALTER DATABASE [Library] SET COMPATIBILITY_LEVEL = 150
GO
IF (1 = FULLTEXTSERVICEPROPERTY('IsFullTextInstalled'))
begin
EXEC [Library].[dbo].[sp_fulltext_database] @action = 'enable'
end
GO
ALTER DATABASE [Library] SET ANSI_NULL_DEFAULT OFF 
GO
ALTER DATABASE [Library] SET ANSI_NULLS OFF 
GO
ALTER DATABASE [Library] SET ANSI_PADDING OFF 
GO
ALTER DATABASE [Library] SET ANSI_WARNINGS OFF 
GO
ALTER DATABASE [Library] SET ARITHABORT OFF 
GO
ALTER DATABASE [Library] SET AUTO_CLOSE OFF 
GO
ALTER DATABASE [Library] SET AUTO_SHRINK OFF 
GO
ALTER DATABASE [Library] SET AUTO_UPDATE_STATISTICS ON 
GO
ALTER DATABASE [Library] SET CURSOR_CLOSE_ON_COMMIT OFF 
GO
ALTER DATABASE [Library] SET CURSOR_DEFAULT  GLOBAL 
GO
ALTER DATABASE [Library] SET CONCAT_NULL_YIELDS_NULL OFF 
GO
ALTER DATABASE [Library] SET NUMERIC_ROUNDABORT OFF 
GO
ALTER DATABASE [Library] SET QUOTED_IDENTIFIER OFF 
GO
ALTER DATABASE [Library] SET RECURSIVE_TRIGGERS OFF 
GO
ALTER DATABASE [Library] SET  DISABLE_BROKER 
GO
ALTER DATABASE [Library] SET AUTO_UPDATE_STATISTICS_ASYNC OFF 
GO
ALTER DATABASE [Library] SET DATE_CORRELATION_OPTIMIZATION OFF 
GO
ALTER DATABASE [Library] SET TRUSTWORTHY OFF 
GO
ALTER DATABASE [Library] SET ALLOW_SNAPSHOT_ISOLATION OFF 
GO
ALTER DATABASE [Library] SET PARAMETERIZATION SIMPLE 
GO
ALTER DATABASE [Library] SET READ_COMMITTED_SNAPSHOT OFF 
GO
ALTER DATABASE [Library] SET HONOR_BROKER_PRIORITY OFF 
GO
ALTER DATABASE [Library] SET RECOVERY SIMPLE 
GO
ALTER DATABASE [Library] SET  MULTI_USER 
GO
ALTER DATABASE [Library] SET PAGE_VERIFY CHECKSUM  
GO
ALTER DATABASE [Library] SET DB_CHAINING OFF 
GO
ALTER DATABASE [Library] SET FILESTREAM( NON_TRANSACTED_ACCESS = OFF ) 
GO
ALTER DATABASE [Library] SET TARGET_RECOVERY_TIME = 60 SECONDS 
GO
ALTER DATABASE [Library] SET DELAYED_DURABILITY = DISABLED 
GO
ALTER DATABASE [Library] SET ACCELERATED_DATABASE_RECOVERY = OFF  
GO
ALTER DATABASE [Library] SET QUERY_STORE = OFF
GO
USE [Library]
GO
/****** Object:  DatabaseRole [db_User]    Script Date: 4/19/2022 9:52:37 PM ******/
CREATE ROLE [db_User]
GO
/****** Object:  DatabaseRole [db_Librarian]    Script Date: 4/19/2022 9:52:37 PM ******/
CREATE ROLE [db_Librarian]
GO
/****** Object:  DatabaseRole [db_Admin]    Script Date: 4/19/2022 9:52:37 PM ******/
CREATE ROLE [db_Admin]
GO
/****** Object:  UserDefinedFunction [dbo].[ufn_GetObjectType]    Script Date: 4/19/2022 9:52:37 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE FUNCTION [dbo].[ufn_GetObjectType] 
(
	@Id INT
)
RETURNS INT
AS
BEGIN
	DECLARE @ObjectType INT
	SELECT @ObjectType = p.ObjectType FROM dbo.Polygraphy p
	WHERE p.Id = @Id

	IF(@ObjectType = 1)
	BEGIN
		RETURN 1
	END

	ELSE IF(@ObjectType = 2)
	BEGIN
		RETURN 2
	END

	ELSE
	BEGIN
		RETURN 3
	END
	RETURN 0
END
GO
/****** Object:  UserDefinedFunction [dbo].[ufn_IsAuthorUnique]    Script Date: 4/19/2022 9:52:37 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE FUNCTION [dbo].[ufn_IsAuthorUnique](
@Firstname NVARCHAR(50), 
@Lastname NVARCHAR(200))
RETURNS INT
AS
BEGIN
	IF(EXISTS(SELECT Firstname, Lastname
		FROM dbo.Author a
		WHERE a.Firstname = @Firstname AND a.Lastname = @Lastname))
	BEGIN
		RETURN 0
	END

	ELSE
		BEGIN
			RETURN 1
		END
	RETURN 0
END
GO
/****** Object:  UserDefinedFunction [dbo].[ufn_IsAuthorUniqueUpdate]    Script Date: 4/19/2022 9:52:37 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE FUNCTION [dbo].[ufn_IsAuthorUniqueUpdate](
@Id INT,
@Firstname NVARCHAR(50), 
@Lastname NVARCHAR(200))
RETURNS INT
AS
BEGIN
	IF(EXISTS(SELECT Firstname, Lastname
		FROM dbo.Author a
		WHERE a.Firstname = @Firstname AND 
		a.Lastname = @Lastname AND 
		a.Id != @Id))
	BEGIN
		RETURN 0
	END

	ELSE
		BEGIN
			RETURN 1
		END
	RETURN 0
END
GO
/****** Object:  UserDefinedFunction [dbo].[ufn_IsBookUnique]    Script Date: 4/19/2022 9:52:37 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE FUNCTION [dbo].[ufn_IsBookUnique](
@ISBN NVARCHAR(50), 
@Name NVARCHAR(300), 
@CreationDate DATE,
@AuthorIds NVARCHAR(MAX))
RETURNS INT
AS
BEGIN
	IF (EXISTS(
		SELECT ISBN FROM dbo.Book b
		WHERE b.ISBN = @ISBN))
	BEGIN
		RETURN 0
	END

	ELSE IF (EXISTS(
		SELECT Name, CreationDate 
		FROM dbo.Polygraphy p
		WHERE p.Name = @Name AND 
		p.CreationDate = @CreationDate))
	BEGIN
		DECLARE @AuthorsFromTable NVARCHAR(MAX)
		SELECT @AuthorsFromTable = (SELECT STUFF(
			(
			SELECT ',' + CONVERT(NVARCHAR(20), AuthorId) 
			FROM dbo.AuthorPolygraphy a
			WHERE a.PolygraphyId = b.Id
			FOR xml path('')
			), 1, 1, '[') + ']')
			FROM dbo.Book b
			IF (@AuthorsFromTable = @AuthorIds)
				BEGIN
					RETURN 0
				END
		RETURN 0
	END

	ELSE
		BEGIN
			RETURN 1
		END
	RETURN 0
END
GO
/****** Object:  UserDefinedFunction [dbo].[ufn_IsBookUniqueUpdate]    Script Date: 4/19/2022 9:52:37 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE FUNCTION [dbo].[ufn_IsBookUniqueUpdate](
@Id INT,
@ISBN NVARCHAR(50), 
@Name NVARCHAR(300), 
@CreationDate DATE,
@AuthorIds NVARCHAR(MAX))
RETURNS INT
AS
BEGIN
	IF (EXISTS(
		SELECT ISBN FROM dbo.Book b
		WHERE b.ISBN = @ISBN AND b.Id != @Id))
	BEGIN
		RETURN 0
	END

	ELSE IF (EXISTS(
		SELECT Name, CreationDate 
		FROM dbo.Polygraphy p
		WHERE p.Name = @Name AND 
		p.CreationDate = @CreationDate AND
		p.Id != @Id))
	BEGIN
		DECLARE @AuthorsFromTable NVARCHAR(MAX)
		SELECT @AuthorsFromTable = (SELECT STUFF(
			(
			SELECT ',' + CONVERT(NVARCHAR(20), AuthorId) 
			FROM dbo.AuthorPolygraphy a
			WHERE a.PolygraphyId = b.Id
			FOR xml path('')
			), 1, 1, '[') + ']')
			FROM dbo.Book b
			IF (@AuthorsFromTable = @AuthorIds)
				BEGIN
					RETURN 0
				END
		RETURN 0
	END

	ELSE
		BEGIN
			RETURN 1
		END
	RETURN 0
END
GO
/****** Object:  UserDefinedFunction [dbo].[ufn_IsNewspaperIssueUnique]    Script Date: 4/19/2022 9:52:37 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE FUNCTION [dbo].[ufn_IsNewspaperIssueUnique]( 
@Name NVARCHAR(300), 
@CreationDate DATE,
@Publisher NVARCHAR(300))
RETURNS INT
AS
BEGIN
	
	IF(EXISTS(SELECT p.Name 
	FROM dbo.Polygraphy p
	LEFT JOIN dbo.NewspaperIssue i
	ON i.Id = p.Id
	WHERE p.Name = @Name AND 
	i.Publisher = @Publisher AND
	p.CreationDate = @CreationDate))
	BEGIN
		RETURN 0
	END

	ELSE
		BEGIN
			RETURN 1
		END
	RETURN 0
END
GO
/****** Object:  UserDefinedFunction [dbo].[ufn_IsNewspaperIssueUniqueUpdate]    Script Date: 4/19/2022 9:52:37 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE FUNCTION [dbo].[ufn_IsNewspaperIssueUniqueUpdate]( 
@Id INT,
@Name NVARCHAR(300), 
@CreationDate DATE,
@Publisher NVARCHAR(300))
RETURNS INT
AS
BEGIN
	
	IF(EXISTS(SELECT p.Name 
	FROM dbo.Polygraphy p
	JOIN dbo.NewspaperIssue i
	ON i.Id = p.Id
	WHERE p.Name = @Name AND 
	i.Publisher = @Publisher AND
	p.CreationDate = @CreationDate AND
	i.Id != @Id))
	BEGIN
		RETURN 0
	END

	ELSE
		BEGIN
			RETURN 1
		END
	RETURN 0
END
GO
/****** Object:  UserDefinedFunction [dbo].[ufn_IsNewspaperUnique]    Script Date: 4/19/2022 9:52:37 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE FUNCTION [dbo].[ufn_IsNewspaperUnique]( 
@Name NVARCHAR(300), 
@ISSN NVARCHAR(50))
RETURNS INT
AS
BEGIN
	IF(EXISTS(SELECT n.Name
		FROM dbo.Newspaper n
		WHERE n.Name = @Name AND n.ISSN = @ISSN))
	BEGIN
		RETURN 0
	END

	ELSE
		BEGIN
			RETURN 1
		END
	RETURN 0
END
GO
/****** Object:  UserDefinedFunction [dbo].[ufn_IsNewspaperUniqueUpdate]    Script Date: 4/19/2022 9:52:37 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE FUNCTION [dbo].[ufn_IsNewspaperUniqueUpdate](
@Id INT,
@Name NVARCHAR(300), 
@ISSN NVARCHAR(50))
RETURNS INT
AS
BEGIN
	IF(EXISTS(SELECT n.Name 
		FROM dbo.Newspaper n
		WHERE n.Name = @Name AND n.ISSN = @ISSN AND n.Id != @Id))
	BEGIN
		RETURN 0
	END

	ELSE
		BEGIN
			RETURN 1
		END
	RETURN 0
END
GO
/****** Object:  UserDefinedFunction [dbo].[ufn_IsPatentUnique]    Script Date: 4/19/2022 9:52:37 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE FUNCTION [dbo].[ufn_IsPatentUnique](
@Number INT, 
@Country NVARCHAR(200))
RETURNS INT
AS
BEGIN
	IF(EXISTS(SELECT PatentNumber, Country
		FROM dbo.Patent p
		WHERE PatentNumber = @Number AND p.Country = @Country))
	BEGIN
		RETURN 0
	END

	ELSE
		BEGIN
			RETURN 1
		END
	RETURN 0
END
GO
/****** Object:  UserDefinedFunction [dbo].[ufn_IsPatentUniqueUpdate]    Script Date: 4/19/2022 9:52:37 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE FUNCTION [dbo].[ufn_IsPatentUniqueUpdate](
@Id INT,
@Number INT, 
@Country NVARCHAR(200))
RETURNS INT
AS
BEGIN
	IF(EXISTS(SELECT PatentNumber, Country
		FROM dbo.Patent p
		WHERE PatentNumber = @Number AND p.Country = @Country AND p.Id != @Id))
	BEGIN
		RETURN 0
	END

	ELSE
		BEGIN
			RETURN 1
		END
	RETURN 0
END
GO
/****** Object:  UserDefinedFunction [dbo].[ufn_IsUserUnique]    Script Date: 4/19/2022 9:52:37 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE FUNCTION [dbo].[ufn_IsUserUnique](
@Username NVARCHAR(50))
RETURNS INT
AS
BEGIN
	IF(EXISTS(SELECT Username
		FROM dbo.[User] u
		WHERE u.Username = @Username))
	BEGIN
		RETURN 0
	END

	ELSE
		BEGIN
			RETURN 1
		END
	RETURN 0
END
GO
/****** Object:  UserDefinedFunction [dbo].[ufn_IsUserUniqueUpdate]    Script Date: 4/19/2022 9:52:37 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE FUNCTION [dbo].[ufn_IsUserUniqueUpdate](
@Id INT,
@Username NVARCHAR(50))
RETURNS INT
AS
BEGIN
	IF(EXISTS(SELECT Username
		FROM dbo.[User] u
		WHERE u.Username = @Username AND u.Id != @Id))
	BEGIN
		RETURN 0
	END

	ELSE
		BEGIN
			RETURN 1
		END
	RETURN 0
END
GO
/****** Object:  Table [dbo].[Author]    Script Date: 4/19/2022 9:52:37 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Author](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Firstname] [nvarchar](50) NOT NULL,
	[Lastname] [nvarchar](200) NOT NULL,
	[STATUS] [bit] NOT NULL,
 CONSTRAINT [PK_Author] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[AuthorPolygraphy]    Script Date: 4/19/2022 9:52:37 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AuthorPolygraphy](
	[AuthorId] [int] NOT NULL,
	[PolygraphyId] [int] NOT NULL,
 CONSTRAINT [PK_AuthorPolygraphy] PRIMARY KEY CLUSTERED 
(
	[AuthorId] ASC,
	[PolygraphyId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Book]    Script Date: 4/19/2022 9:52:37 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Book](
	[Id] [int] NOT NULL,
	[City] [nvarchar](200) NOT NULL,
	[Publisher] [nvarchar](300) NOT NULL,
	[ISBN] [nvarchar](50) NULL
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[LibraryLogs]    Script Date: 4/19/2022 9:52:37 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[LibraryLogs](
	[OperationDate] [datetime2](7) NOT NULL,
	[ObjectType] [nvarchar](50) NOT NULL,
	[ObjectId] [int] NOT NULL,
	[Action] [nvarchar](150) NOT NULL,
	[Username] [nvarchar](50) NOT NULL
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Newspaper]    Script Date: 4/19/2022 9:52:37 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Newspaper](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](300) NOT NULL,
	[ISSN] [nvarchar](50) NULL,
 CONSTRAINT [PK_Newspaper] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[NewspaperIssue]    Script Date: 4/19/2022 9:52:37 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[NewspaperIssue](
	[Id] [int] NOT NULL,
	[NewspaperId] [int] NOT NULL,
	[City] [nvarchar](200) NOT NULL,
	[Publisher] [nvarchar](300) NOT NULL,
	[Number] [int] NULL
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Patent]    Script Date: 4/19/2022 9:52:37 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Patent](
	[Id] [int] NOT NULL,
	[Country] [nvarchar](200) NOT NULL,
	[PatentNumber] [int] NOT NULL,
	[PatentPublishmentDate] [datetime2](7) NOT NULL
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Polygraphy]    Script Date: 4/19/2022 9:52:37 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Polygraphy](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](300) NOT NULL,
	[CreationDate] [datetime2](7) NULL,
	[Footnote] [nvarchar](2000) NOT NULL,
	[TotalPages] [int] NOT NULL,
	[Status] [bit] NOT NULL,
	[ObjectType] [int] NOT NULL,
 CONSTRAINT [PK_Polygraphies] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[User]    Script Date: 4/19/2022 9:52:37 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[User](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Username] [nvarchar](50) NOT NULL,
	[Password] [nvarchar](max) NOT NULL,
	[Role] [nvarchar](50) NOT NULL,
 CONSTRAINT [PK_User] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY],
 CONSTRAINT [UQ_Username] UNIQUE NONCLUSTERED 
(
	[Username] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
ALTER TABLE [dbo].[Book]  WITH CHECK ADD  CONSTRAINT [FK_Book_Polygraphy] FOREIGN KEY([Id])
REFERENCES [dbo].[Polygraphy] ([Id])
GO
ALTER TABLE [dbo].[Book] CHECK CONSTRAINT [FK_Book_Polygraphy]
GO
ALTER TABLE [dbo].[NewspaperIssue]  WITH CHECK ADD  CONSTRAINT [FK_NewspaperIssue_Newspaper1] FOREIGN KEY([NewspaperId])
REFERENCES [dbo].[Newspaper] ([Id])
GO
ALTER TABLE [dbo].[NewspaperIssue] CHECK CONSTRAINT [FK_NewspaperIssue_Newspaper1]
GO
ALTER TABLE [dbo].[NewspaperIssue]  WITH CHECK ADD  CONSTRAINT [FK_NewspaperIssue_Polygraphy1] FOREIGN KEY([Id])
REFERENCES [dbo].[Polygraphy] ([Id])
GO
ALTER TABLE [dbo].[NewspaperIssue] CHECK CONSTRAINT [FK_NewspaperIssue_Polygraphy1]
GO
ALTER TABLE [dbo].[Patent]  WITH CHECK ADD  CONSTRAINT [FK_Patent_Polygraphy] FOREIGN KEY([Id])
REFERENCES [dbo].[Polygraphy] ([Id])
GO
ALTER TABLE [dbo].[Patent] CHECK CONSTRAINT [FK_Patent_Polygraphy]
GO
/****** Object:  StoredProcedure [dbo].[AddAuthorToLibrary]    Script Date: 4/19/2022 9:52:37 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[AddAuthorToLibrary]
	
	@Id INT OUTPUT,
	@Firstname NVARCHAR(50),
	@Lastname NVARCHAR(200),
	@STATUS BIT = 1

AS
BEGIN
	DECLARE @IsUniqueResult INT
	SELECT @IsUniqueResult = dbo.ufn_IsAuthorUnique(@Firstname, @Lastname)
	IF (@IsUniqueResult = 1)
	BEGIN
		BEGIN TRAN
			BEGIN TRY
				INSERT dbo.Author VALUES (@Firstname, @Lastname, @STATUS)
				SET @Id = SCOPE_IDENTITY()
				INSERT dbo.LibraryLogs VALUES (
				GETDATE(),
				'Author', 
				@Id, 
				FORMATMESSAGE('Author %s %s was added', @Firstname, @Lastname), 
				CURRENT_USER
				)
			COMMIT TRAN
		END TRY
		BEGIN CATCH
			ROLLBACK TRAN
		END CATCH
	END
END
GO
/****** Object:  StoredProcedure [dbo].[AddBookToLibrary]    Script Date: 4/19/2022 9:52:37 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[AddBookToLibrary]
	
	@Id INT OUTPUT,
	@Name NVARCHAR(300),
	@AuthorIds NVARCHAR(MAX),
	@CreationDate DATE,
	@Footnote NVARCHAR(2000),
	@TotalPages INT,
	@City NVARCHAR(200),
	@Publisher NVARCHAR(300),
	@ISBN NVARCHAR(50) = NULL

AS
BEGIN
	DECLARE @IsUniqieResult INT
	SELECT @IsUniqieResult = dbo.ufn_IsBookUnique(@ISBN, @Name, @CreationDate, @AuthorIds)
	IF (@IsUniqieResult = 1)
	BEGIN
		BEGIN TRAN
			BEGIN TRY
				INSERT dbo.Polygraphy(Name, CreationDate, Footnote, TotalPages, Status, ObjectType) VALUES (@Name, @CreationDate, @Footnote, @TotalPages, 1, 1)
				SET @Id = SCOPE_IDENTITY()

				INSERT dbo.Book VALUES (@Id, @City, @Publisher, @ISBN)

				INSERT INTO dbo.AuthorPolygraphy (AuthorId, PolygraphyId)
				SELECT AuthorId, @Id
				FROM OPENJSON(@AuthorIds) WITH(AuthorId int '$')
			
				INSERT dbo.LibraryLogs VALUES (
				GETDATE(), 
				'Book', 
				@Id, 
				FORMATMESSAGE('Book %s was added', @Name), 
				CURRENT_USER
				) 
			COMMIT TRAN
		END TRY
		BEGIN CATCH
			ROLLBACK TRAN
		END CATCH
	END
END
GO
/****** Object:  StoredProcedure [dbo].[AddNewspaperIssueToLibrary]    Script Date: 4/19/2022 9:52:37 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[AddNewspaperIssueToLibrary]
	
	@Id INT OUTPUT,
	@NewspaperId INT,
	@Name NVARCHAR(300),
	@CreationDate DATE,
	@Footnote NVARCHAR(2000),
	@TotalPages INT,
	@City NVARCHAR(200),
	@Publisher NVARCHAR(300),
	@Number INT

AS
BEGIN
	DECLARE @IsUniqueResult INT
	SELECT @IsUniqueResult = dbo.ufn_IsNewspaperIssueUnique(@Name, @CreationDate, @Publisher)
	IF (@IsUniqueResult = 1)
	BEGIN
		BEGIN TRAN
			BEGIN TRY
				INSERT dbo.Polygraphy(Name, CreationDate, Footnote, TotalPages, Status, ObjectType) 
				VALUES (@Name, @CreationDate, @Footnote, @TotalPages, 1, 2)
				SET @Id = SCOPE_IDENTITY()

				INSERT dbo.NewspaperIssue(Id, NewspaperId, City, Publisher, Number)
				VALUES (@Id, @NewspaperId, @City, @Publisher, @Number)
			
				INSERT dbo.LibraryLogs VALUES (
				GETDATE(), 
				'NewspaperIssue', 
				@Id, 
				FORMATMESSAGE('NewspaperIssue %d was added', @Id), 
				CURRENT_USER
				) 
			COMMIT TRAN
		END TRY
		BEGIN CATCH
			SELECT ERROR_MESSAGE()
			ROLLBACK TRAN
		END CATCH
	END
END
GO
/****** Object:  StoredProcedure [dbo].[AddNewspaperToLibrary]    Script Date: 4/19/2022 9:52:37 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[AddNewspaperToLibrary]
	@Id INT OUTPUT,
	@Name NVARCHAR(300),
	@ISSN NVARCHAR(50)

AS
BEGIN
	DECLARE @IsUniqueResult INT
	SELECT @IsUniqueResult = dbo.ufn_IsNewspaperUnique(@Name, @ISSN)
	IF (@IsUniqueResult = 1)
	BEGIN
		BEGIN TRAN
			BEGIN TRY

				INSERT dbo.Newspaper(Name, ISSN) 
				VALUES (@Name, @ISSN)
				SET @Id = SCOPE_IDENTITY()
			
				INSERT dbo.LibraryLogs VALUES (
				GETDATE(), 
				'Newspaper', 
				@Id, 
				FORMATMESSAGE('Newspaper %s was added', @Name),
				CURRENT_USER
				) 
			COMMIT TRAN
		END TRY
		BEGIN CATCH
			ROLLBACK TRAN
		END CATCH
	END
END
GO
/****** Object:  StoredProcedure [dbo].[AddPatentToLibrary]    Script Date: 4/19/2022 9:52:37 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[AddPatentToLibrary]
	
	@Id INT OUTPUT,
	@Name NVARCHAR(300),
	@AuthorIds NVARCHAR(MAX),
	@CreationDate DATE,
	@Footnote NVARCHAR(2000),
	@TotalPages INT,
	@Country NVARCHAR(200),
	@PublishmentDate DATE,
	@Number INT

AS
BEGIN
	DECLARE @IsUniqueResult INT
	SELECT @IsUniqueResult = dbo.ufn_IsPatentUnique(@Number, @Country)
	IF (@IsUniqueResult = 1)
	BEGIN
		BEGIN TRAN
			BEGIN TRY
				INSERT dbo.Polygraphy(Name, CreationDate, Footnote, TotalPages, Status, ObjectType) 
				VALUES (@Name, @CreationDate, @Footnote, @TotalPages, 1, 3)
				SET @Id = SCOPE_IDENTITY()

				INSERT dbo.Patent(Id, Country, PatentNumber, PatentPublishmentDate) 
				VALUES (@Id, @Country, @Number, @PublishmentDate)

				INSERT INTO dbo.AuthorPolygraphy (AuthorId, PolygraphyId)
				SELECT AuthorId, @Id
				FROM OPENJSON(@AuthorIds) WITH(AuthorId int '$')
			
				INSERT dbo.LibraryLogs VALUES (
				GETDATE(), 
				'Patent', 
				@Id,
				FORMATMESSAGE('Patent %s was added', @Name), 
				CURRENT_USER
				) 
			COMMIT TRAN
		END TRY
		BEGIN CATCH
			ROLLBACK TRAN
		END CATCH
	END
END
GO
/****** Object:  StoredProcedure [dbo].[AddUser]    Script Date: 4/19/2022 9:52:37 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[AddUser]
	@Id INT OUTPUT,
	@Username NVARCHAR(50),
	@Password NVARCHAR(MAX)
AS
BEGIN
	DECLARE @IsUniqueResult INT
	SELECT @IsUniqueResult = dbo.ufn_IsUserUnique(@Username)
	IF (@IsUniqueResult = 1)
	BEGIN
		BEGIN TRAN
			BEGIN TRY
				INSERT dbo.[User](Username, Password, Role)
				VALUES (@Username, @Password, 'User')
				SET @Id = SCOPE_IDENTITY()
			
				INSERT dbo.LibraryLogs (OperationDate, ObjectType, ObjectId, Action, Username)
				VALUES (
					GETDATE(),
					'User',
					@Id,
					FORMATMESSAGE('User %d was added', @Id),
					CURRENT_USER
				)
			COMMIT TRAN
		END TRY
		BEGIN CATCH
			ROLLBACK TRAN
		END CATCH
	END
END
GO
/****** Object:  StoredProcedure [dbo].[ClearAuthors]    Script Date: 4/19/2022 9:52:37 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[ClearAuthors]
AS
BEGIN
	BEGIN TRAN
		BEGIN TRY
			TRUNCATE TABLE dbo.Author
			TRUNCATE TABLE dbo.AuthorPolygraphy
		COMMIT TRAN
	END TRY
	BEGIN CATCH
		ROLLBACK TRAN
	END CATCH
END
GO
/****** Object:  StoredProcedure [dbo].[ClearLibrary]    Script Date: 4/19/2022 9:52:37 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[ClearLibrary]
AS
BEGIN
	BEGIN TRAN
		BEGIN TRY
			TRUNCATE TABLE dbo.Book
			TRUNCATE TABLE dbo.NewspaperIssue
			TRUNCATE TABLE dbo.Patent
			DELETE FROM dbo.Polygraphy
			TRUNCATE TABLE dbo.AuthorPolygraphy
		COMMIT TRAN
	END TRY
	BEGIN CATCH
		ROLLBACK TRAN
	END CATCH
END
GO
/****** Object:  StoredProcedure [dbo].[ClearNewspapers]    Script Date: 4/19/2022 9:52:37 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[ClearNewspapers]
AS
BEGIN
	BEGIN TRAN
		BEGIN TRY

			DELETE FROM dbo.Newspaper
			TRUNCATE TABLE dbo.NewspaperIssue
			
		COMMIT TRAN
	END TRY
	BEGIN CATCH
		ROLLBACK TRAN
	END CATCH
END
GO
/****** Object:  StoredProcedure [dbo].[ClearUsers]    Script Date: 4/19/2022 9:52:37 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[ClearUsers]
AS
BEGIN
	BEGIN TRAN
		BEGIN TRY
			TRUNCATE TABLE dbo.[User]
		COMMIT TRAN
	END TRY
	BEGIN CATCH
		ROLLBACK TRAN
	END CATCH
END
GO
/****** Object:  StoredProcedure [dbo].[DeleteAuthorFromLibrary]    Script Date: 4/19/2022 9:52:37 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[DeleteAuthorFromLibrary]
	
	@IdToDelete INT

AS
BEGIN
	SET NOCOUNT ON;
	BEGIN TRAN
		BEGIN TRY
			
			DELETE FROM dbo.AuthorPolygraphy
			WHERE AuthorId = @IdToDelete

			DELETE FROM dbo.Author
			WHERE Id = @IdToDelete
			
			INSERT dbo.LibraryLogs VALUES (
			GETDATE(), 
			'Author', 
			@IdToDelete, 
			FORMATMESSAGE('Author #%d was deleted', @IdToDelete),
			CURRENT_USER
			) 

		COMMIT TRAN
	END TRY
	BEGIN CATCH
		ROLLBACK TRAN
	END CATCH
END
GO
/****** Object:  StoredProcedure [dbo].[DeleteFromLibrary]    Script Date: 4/19/2022 9:52:37 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[DeleteFromLibrary] 
	@Id INT
AS
BEGIN
	SET NOCOUNT ON;
	DECLARE @Result INT
	SELECT @Result = dbo.ufn_GetObjectType(@Id)
	IF(@Result = 1)
	BEGIN
		BEGIN TRAN
		BEGIN TRY
			DELETE FROM dbo.Book
			WHERE Id = @Id

			DELETE FROM dbo.Polygraphy
			WHERE Id = @Id

			DELETE FROM dbo.AuthorPolygraphy
			WHERE PolygraphyId = @Id
			
			INSERT dbo.LibraryLogs VALUES (
			GETDATE(), 
			'Book', 
			@Id, 
			FORMATMESSAGE('Book #%d was deleted', @Id),
			CURRENT_USER
			) 
		COMMIT TRAN
		END TRY
		BEGIN CATCH
			ROLLBACK TRAN
		END CATCH
	END

	ELSE IF(@Result = 2)
	BEGIN
		BEGIN TRAN
		BEGIN TRY
			DELETE FROM dbo.Newspaper
			WHERE Id = @Id

			DELETE FROM dbo.Polygraphy
			WHERE Id = @Id
			
			INSERT dbo.LibraryLogs VALUES (
			GETDATE(), 
			'Newspaper', 
			@Id, 
			FORMATMESSAGE('Newspaper #%d was deleted', @Id),
			CURRENT_USER
			) 
		COMMIT TRAN
		END TRY
		BEGIN CATCH
			ROLLBACK TRAN
		END CATCH
	END

	ELSE
	BEGIN
		BEGIN TRAN
		BEGIN TRY
			DELETE FROM dbo.Patent
			WHERE Id = @Id

			DELETE FROM dbo.Polygraphy
			WHERE Id = @Id

			DELETE FROM dbo.AuthorPolygraphy
			WHERE PolygraphyId = @Id
			
			INSERT dbo.LibraryLogs VALUES (
			GETDATE(), 
			'Patent', 
			@Id, 
			FORMATMESSAGE('Patent #%d was deleted', @Id),
			CURRENT_USER
			) 
		COMMIT TRAN
		END TRY
		BEGIN CATCH
			ROLLBACK TRAN
		END CATCH
	END
END
GO
/****** Object:  StoredProcedure [dbo].[DeleteNewspaperFromLibrary]    Script Date: 4/19/2022 9:52:37 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[DeleteNewspaperFromLibrary] 
	@Id INT
AS
BEGIN
	BEGIN TRAN
		BEGIN TRY
			DELETE FROM dbo.Newspaper
			WHERE Id = @Id

			IF (EXISTS(
			SELECT NewspaperId 
			FROM dbo.NewspaperIssue
			WHERE NewspaperId = @Id))
			BEGIN
				DELETE FROM dbo.NewspaperIssue
				WHERE NewspaperId = @Id
			END

			INSERT dbo.LibraryLogs VALUES (
			GETDATE(), 
			'Newspaper', 
			@Id, 
			FORMATMESSAGE('Newspaper #%d was deleted', @Id),
			CURRENT_USER
			) 
	COMMIT TRAN
		END TRY
	BEGIN CATCH
		ROLLBACK TRAN
	END CATCH
END
GO
/****** Object:  StoredProcedure [dbo].[DeleteUserFromLibrary]    Script Date: 4/19/2022 9:52:37 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[DeleteUserFromLibrary]
	
	@IdToDelete INT

AS
BEGIN
	SET NOCOUNT ON;
	BEGIN TRAN
		BEGIN TRY
			
			DELETE FROM dbo.[User]
			WHERE Id= @IdToDelete
			
			INSERT dbo.LibraryLogs VALUES (
			GETDATE(), 
			'User', 
			@IdToDelete, 
			FORMATMESSAGE('User #%d was deleted', @IdToDelete),
			CURRENT_USER
			) 

		COMMIT TRAN
	END TRY
	BEGIN CATCH
		ROLLBACK TRAN
	END CATCH
END
GO
/****** Object:  StoredProcedure [dbo].[GetAllAuthors]    Script Date: 4/19/2022 9:52:37 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[GetAllAuthors]
AS
BEGIN
	SET NOCOUNT ON;
	SELECT Id, Firstname, Lastname 
	FROM dbo.Author
END
GO
/****** Object:  StoredProcedure [dbo].[GetAllBooks]    Script Date: 4/19/2022 9:52:37 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[GetAllBooks] 
	
AS
BEGIN
	SET NOCOUNT ON;
	SELECT (SELECT STUFF(
			(
			SELECT ',' + CONVERT(NVARCHAR(20), AuthorId) 
			FROM dbo.AuthorPolygraphy a
			WHERE a.PolygraphyId = b.Id
			FOR xml path('')
			), 1, 1, '[') + ']') AS AuthorIds,
			b.Id,
			Name, 
			CreationDate, 
			Footnote, 
			TotalPages, 
			City, 
			Publisher,
			ISBN
			FROM dbo.Book b
			JOIN dbo.Polygraphy p
			ON b.Id = p.Id
			WHERE p.Status = 1
END
GO
/****** Object:  StoredProcedure [dbo].[GetAllLibrary]    Script Date: 4/19/2022 9:52:37 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[GetAllLibrary]
AS
BEGIN
	SET NOCOUNT ON;
	SELECT (SELECT STUFF(
			(
			SELECT ',' + CONVERT(NVARCHAR(20), AuthorId) 
			FROM dbo.AuthorPolygraphy a
			WHERE a.PolygraphyId = poly.Id
			FOR xml path('')
			), 1, 1, '[') + ']') AS AuthorIds,
			poly.Id,
			poly.Name, 
			CreationDate, 
			Footnote,
			TotalPages,
			b.City,
			b.Publisher,
			ISBN,
			i.NewspaperId,
			i.Number as NewspaperNumber,
			i.City as NewspaperCity,
			i.Publisher as NewspaperPublisher,
			Country,
			PatentNumber,
			PatentPublishmentDate,
			poly.ObjectType
			FROM dbo.Polygraphy poly
			LEFT JOIN dbo.Book b
			ON b.Id = poly.Id
			LEFT JOIN dbo.NewspaperIssue i
			ON i.Id = poly.Id
			LEFT JOIN dbo.Patent p
			ON p.Id = poly.Id
			WHERE poly.Status = 1
END
GO
/****** Object:  StoredProcedure [dbo].[GetAllNewspaperIssues]    Script Date: 4/19/2022 9:52:37 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[GetAllNewspaperIssues] 
	
AS
BEGIN
	SET NOCOUNT ON;
	SELECT
		i.Id,
		NewspaperId,
		Name, 
		CreationDate, 
		Footnote, 
		TotalPages, 
		City,
		Publisher,
		Number as NewspaperNumber
		FROM dbo.NewspaperIssue i
		JOIN dbo.Polygraphy p
		ON i.NewspaperId = p.Id
		WHERE p.Status = 1
END
GO
/****** Object:  StoredProcedure [dbo].[GetAllNewspapers]    Script Date: 4/19/2022 9:52:37 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[GetAllNewspapers] 
	
AS
BEGIN
	SELECT (SELECT STUFF(
			(
			SELECT ',' + CONVERT(NVARCHAR(20), NewspaperId) 
			FROM dbo.NewspaperIssue i
			WHERE i.NewspaperId = n.Id
			FOR xml path('')
			), 1, 1, '[') + ']') AS IssueIds,
			n.Id,
			n.Name,
			ISSN
			FROM dbo.Newspaper n
END
GO
/****** Object:  StoredProcedure [dbo].[GetAllPatents]    Script Date: 4/19/2022 9:52:37 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[GetAllPatents] 
	
AS
BEGIN
	SET NOCOUNT ON;
	SELECT (SELECT STUFF(
			(
			SELECT ',' + CONVERT(NVARCHAR(20), AuthorId) 
			FROM dbo.AuthorPolygraphy a
			WHERE a.PolygraphyId = pa.Id
			FOR xml path('')
			), 1, 1, '[') + ']') AS AuthorIds,
			pa.Id,
			Name, 
			CreationDate, 
			Footnote, 
			TotalPages, 
			Country, 
			PatentNumber,
			PatentPublishmentDate
			FROM dbo.Patent pa
			JOIN dbo.Polygraphy p
			ON pa.Id = p.Id
			WHERE p.Status = 1
END
GO
/****** Object:  StoredProcedure [dbo].[GetAllUsers]    Script Date: 4/19/2022 9:52:37 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[GetAllUsers]
AS
BEGIN
	BEGIN TRAN
		BEGIN TRY
			SELECT Id, Username, Password, Role 
			FROM dbo.[User]
		COMMIT TRAN
	END TRY
	BEGIN CATCH
		ROLLBACK TRAN
	END CATCH
END
GO
/****** Object:  StoredProcedure [dbo].[GetAuthorById]    Script Date: 4/19/2022 9:52:37 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[GetAuthorById]
	@Id INT
AS
BEGIN
	SET NOCOUNT ON;
	SELECT Id, Firstname, Lastname
	FROM dbo.Author a
	WHERE a.Id = @Id
END
GO
/****** Object:  StoredProcedure [dbo].[GetAuthorsByIds]    Script Date: 4/19/2022 9:52:37 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[GetAuthorsByIds]
	@AuthorIds NVARCHAR(MAX)
AS
BEGIN
	SET NOCOUNT ON;
	SELECT Id, Firstname, Lastname 
	FROM dbo.Author
	WHERE Id IN (SELECT AuthorId 
	FROM OPENJSON(@AuthorIds) WITH(AuthorId int '$'))
END
GO
/****** Object:  StoredProcedure [dbo].[GetBooksByPublisher]    Script Date: 4/19/2022 9:52:37 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[GetBooksByPublisher]
	@Publisher NVARCHAR(300)
AS
BEGIN
	SET NOCOUNT ON;

    SELECT (SELECT STUFF(
			(
			SELECT ',' + CONVERT(NVARCHAR(20), AuthorId) 
			FROM dbo.AuthorPolygraphy a
			WHERE a.PolygraphyId = b.Id
			FOR xml path('')
			), 1, 1, '[') + ']') AS AuthorIds,
			b.Id,
			Name, 
			CreationDate, 
			Footnote, 
			TotalPages, 
			City, 
			Publisher,
			ISBN
			FROM dbo.Book b
			JOIN dbo.Polygraphy p
			ON b.Id = p.Id
			WHERE p.Status = 1 AND b.Publisher LIKE @Publisher+'%'
END
GO
/****** Object:  StoredProcedure [dbo].[GetNewspaperById]    Script Date: 4/19/2022 9:52:37 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[GetNewspaperById]
	@Id INT
AS
BEGIN
	SELECT (SELECT STUFF(
			(
			SELECT ',' + CONVERT(NVARCHAR(20), NewspaperId) 
			FROM dbo.NewspaperIssue i
			WHERE i.NewspaperId = n.Id
			FOR xml path('')
			), 1, 1, '[') + ']') AS IssueIds,
			Id, Name, ISSN
			FROM dbo.Newspaper n
			WHERE n.Id = @Id
END
GO
/****** Object:  StoredProcedure [dbo].[GetPolygraphyById]    Script Date: 4/19/2022 9:52:37 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[GetPolygraphyById]
	@Id INT
AS
BEGIN
	SET NOCOUNT ON;
	DECLARE @ObjectType INT
	SELECT @ObjectType = dbo.ufn_GetObjectType(@Id)

	IF (@ObjectType = 1)
	BEGIN
		SELECT (SELECT STUFF(
			(
			SELECT ',' + CONVERT(NVARCHAR(20), AuthorId) 
			FROM dbo.AuthorPolygraphy a
			WHERE a.PolygraphyId = b.Id
			FOR xml path('')
			), 1, 1, '[') + ']') AS AuthorIds,
			p.Id, Name, 
			CreationDate, 
			Footnote, 
			TotalPages, 
			City, 
			Publisher, 
			ISBN, 
			p.ObjectType
			FROM dbo.Polygraphy p
			JOIN dbo.Book b
			ON p.Id = b.Id
			WHERE p.Id = @Id
	END

	ELSE IF(@ObjectType = 2)
	BEGIN
		SELECT p.Id, Name, CreationDate, Footnote, TotalPages, City, Number, Publisher, p.ObjectType
		FROM dbo.Polygraphy p
		JOIN dbo.NewspaperIssue i
		ON p.Id = i.Id
		WHERE p.Id = @Id
	END

	ELSE IF(@ObjectType = 3)
	BEGIN
		SELECT (SELECT STUFF(
			(
			SELECT ',' + CONVERT(NVARCHAR(20), AuthorId) 
			FROM dbo.AuthorPolygraphy a
			WHERE a.PolygraphyId = pa.Id
			FOR xml path('')
			), 1, 1, '[') + ']') AS AuthorIds,
			p.Id, 
			Name,
			CreationDate, 
			Footnote, 
			TotalPages, 
			Country, 
			PatentPublishmentDate, 
			PatentNumber, 
			p.ObjectType
			FROM dbo.Polygraphy p
			JOIN dbo.Patent pa
			ON p.Id = pa.Id
			WHERE p.Id = @Id
	END
END
GO
/****** Object:  StoredProcedure [dbo].[GetSortedPolygraphies]    Script Date: 4/19/2022 9:52:37 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[GetSortedPolygraphies]
	@Reversed BIT
AS
BEGIN
	SET NOCOUNT ON;
	IF(@Reversed = 0)
BEGIN
	SELECT (SELECT STUFF(
			(
			SELECT ',' + CONVERT(NVARCHAR(20), AuthorId) 
			FROM dbo.AuthorPolygraphy a
			WHERE a.PolygraphyId = poly.Id
			FOR xml path('')
			), 1, 1, '[') + ']') AS AuthorIds,
			poly.Id,
			poly.Name, 
			CreationDate, 
			Footnote,
			TotalPages,
			b.City,
			b.Publisher,
			ISBN,
			i.NewspaperId,
			i.Number as NewspaperNumber,
			i.City as NewspaperCity,
			i.Publisher as NewspaperPublisher,
			Country,
			PatentNumber,
			PatentPublishmentDate,
			poly.ObjectType
			FROM dbo.Polygraphy poly
			LEFT JOIN dbo.Book b
			ON b.Id = poly.Id
			LEFT JOIN dbo.NewspaperIssue i
			ON i.Id = poly.Id
			LEFT JOIN dbo.Patent p
			ON p.Id = poly.Id
			WHERE poly.Status = 1
			ORDER BY poly.CreationDate ASC
END
ELSE
BEGIN
	SELECT (SELECT STUFF(
			(
			SELECT ',' + CONVERT(NVARCHAR(20), AuthorId) 
			FROM dbo.AuthorPolygraphy a
			WHERE a.PolygraphyId = poly.Id
			FOR xml path('')
			), 1, 1, '[') + ']') AS AuthorIds,
			poly.Id,
			poly.Name, 
			CreationDate, 
			Footnote,
			TotalPages,
			b.City,
			b.Publisher,
			ISBN,
			i.NewspaperId,
			i.Number as NewspaperNumber,
			i.City as NewspaperCity,
			i.Publisher as NewspaperPublisher,
			Country,
			PatentNumber,
			PatentPublishmentDate,
			poly.ObjectType
			FROM dbo.Polygraphy poly
			LEFT JOIN dbo.Book b
			ON b.Id = poly.Id
			LEFT JOIN dbo.NewspaperIssue i
			ON i.Id = poly.Id
			LEFT JOIN dbo.Patent p
			ON p.Id = poly.Id
			WHERE poly.Status = 1
			ORDER BY poly.CreationDate DESC
END
END
GO
/****** Object:  StoredProcedure [dbo].[GetUserById]    Script Date: 4/19/2022 9:52:37 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[GetUserById]
	@Id INT
AS
BEGIN
	SET NOCOUNT ON;
	SELECT Id, Username, Password, Role
	FROM dbo.[User] u
	WHERE u.Id = @Id
END
GO
/****** Object:  StoredProcedure [dbo].[GetUserByUsername]    Script Date: 4/19/2022 9:52:37 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[GetUserByUsername]
	@Username NVARCHAR(50)
AS
BEGIN
	SELECT Id, Username, Password, Role 
	FROM dbo.[User]
	WHERE Username = @Username
END
GO
/****** Object:  StoredProcedure [dbo].[GroupByYear]    Script Date: 4/19/2022 9:52:37 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[GroupByYear]
AS
BEGIN
	SET NOCOUNT ON;
	SELECT poly.Id,
	poly.Name, 
	CreationDate, 
	Footnote,
	TotalPages,
	b.City,
	b.Publisher,
	ISBN,
	NewspaperNumber,
	NewspaperPublishmentDate,
	ISSN,
	Country,
	PatentNumber,
	PatentPublishmentDate
	FROM dbo.Polygraphy poly
	LEFT JOIN dbo.Book b
	ON b.Id = poly.Id
	LEFT JOIN dbo.Newspaper n
	ON n.Id = poly.Id
	LEFT JOIN dbo.Patent p
	ON p.Id = poly.Id
	WHERE poly.Status = 1
	GROUP BY CreationDate, 
	poly.Id, 
	poly.Name, 
	Footnote,
	TotalPages,
	b.City,
	b.Publisher,
	ISBN,
	NewspaperNumber,
	NewspaperPublishmentDate,
	ISSN,
	Country,
	PatentNumber,
	PatentPublishmentDate
END
GO
/****** Object:  StoredProcedure [dbo].[MarkAuthorAsDeleted]    Script Date: 4/19/2022 9:52:37 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[MarkAuthorAsDeleted]
	
	@IdToDelete INT

AS
BEGIN
	SET NOCOUNT ON;
	BEGIN TRAN
		BEGIN TRY
			
			UPDATE dbo.Author
			SET STATUS = 0
			WHERE Id = @IdToDelete
			
			INSERT dbo.LibraryLogs VALUES (
			GETDATE(), 
			'Author', 
			@IdToDelete, 
			FORMATMESSAGE('Author #%d was marked as deleted', @IdToDelete),
			CURRENT_USER
			) 

		COMMIT TRAN
	END TRY
	BEGIN CATCH
		ROLLBACK TRAN
	END CATCH
END
GO
/****** Object:  StoredProcedure [dbo].[MarkPolygraphyAsDeleted]    Script Date: 4/19/2022 9:52:37 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[MarkPolygraphyAsDeleted]
	
	@IdToDelete INT

AS
BEGIN
	SET NOCOUNT ON;
	BEGIN TRAN
		BEGIN TRY
			
			UPDATE dbo.Polygraphy
			SET Status = 0
			WHERE Id = @IdToDelete
			
			INSERT dbo.LibraryLogs VALUES (
			GETDATE(), 
			'Polygraphy', 
			@IdToDelete, 
			FORMATMESSAGE('Polygraphy #%d was marked as deleted', @IdToDelete),
			CURRENT_USER
			) 

		COMMIT TRAN
	END TRY
	BEGIN CATCH
		ROLLBACK TRAN
	END CATCH
END
GO
/****** Object:  StoredProcedure [dbo].[SearchByAuthor]    Script Date: 4/19/2022 9:52:37 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[SearchByAuthor]
	@Firstname NVARCHAR(50),
	@Lastname NVARCHAR(200),
	@TypeOfSearch INT
AS
BEGIN
	SET NOCOUNT ON;
	DECLARE @Id INT
	SELECT @Id = (SELECT Id 
	FROM dbo.Author 
	WHERE @Firstname = Firstname AND @Lastname = Lastname)

	IF(@Id IS NULL)
	BEGIN
		RETURN
	END

	IF(@TypeOfSearch = 1)
	BEGIN
		SELECT (SELECT STUFF(
			(
			SELECT ',' + CONVERT(NVARCHAR(20), AuthorId) 
			FROM dbo.AuthorPolygraphy a
			WHERE a.PolygraphyId = b.Id
			FOR xml path('')
			), 1, 1, '[') + ']') AS AuthorIds,
			p.Id, 
			Name, 
			CreationDate, 
			Footnote, 
			TotalPages,
			City,
			Publisher,
			ISBN,
			p.ObjectType
			FROM AuthorPolygraphy a
			JOIN dbo.Polygraphy p
			ON a.PolygraphyId = p.Id
			JOIN dbo.Book b
			ON a.PolygraphyId = b.Id
			WHERE AuthorId = @Id AND Status = 1
	END

	ELSE IF(@TypeOfSearch = 2)
	BEGIN
		SELECT (SELECT STUFF(
			(
			SELECT ',' + CONVERT(NVARCHAR(20), AuthorId) 
			FROM dbo.AuthorPolygraphy a
			WHERE a.PolygraphyId = p.Id
			FOR xml path('')
			), 1, 1, '[') + ']') AS AuthorIds,
			poly.Id, 
			Name, 
			CreationDate, 
			Footnote, 
			TotalPages,
			Country,
			PatentPublishmentDate,
			PatentNumber,
			poly.ObjectType
			FROM AuthorPolygraphy a
			JOIN dbo.Polygraphy poly
			ON a.PolygraphyId = poly.Id
			JOIN dbo.Patent p
			ON a.PolygraphyId = p.Id
			WHERE AuthorId = @Id AND Status = 1
	END

	ELSE IF(@TypeOfSearch = 3)
	BEGIN
		SELECT (SELECT STUFF(
			(
			SELECT ',' + CONVERT(NVARCHAR(20), AuthorId) 
			FROM dbo.AuthorPolygraphy a
			WHERE a.PolygraphyId = poly.Id
			FOR xml path('')
			), 1, 1, '[') + ']') AS AuthorIds,
			poly.Id, 
			Name, 
			CreationDate, 
			Footnote, 
			TotalPages,
			City,
			Publisher,
			ISBN,
			Country,
			PatentPublishmentDate,
			PatentNumber,
			poly.ObjectType
			FROM AuthorPolygraphy a
			JOIN dbo.Polygraphy poly
			ON a.PolygraphyId = poly.Id
			LEFT JOIN dbo.Book b
			ON a.PolygraphyId = b.Id
			LEFT JOIN dbo.Patent p
			ON a.PolygraphyId = p.Id
			WHERE AuthorId = @Id AND Status = 1
	END
END
GO
/****** Object:  StoredProcedure [dbo].[SearchByName]    Script Date: 4/19/2022 9:52:37 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[SearchByName]
	@Name NVARCHAR(300)
AS
BEGIN
	SET NOCOUNT ON;
	SELECT (SELECT STUFF(
			(
			SELECT ',' + CONVERT(NVARCHAR(20), AuthorId) 
			FROM dbo.AuthorPolygraphy a
			WHERE a.PolygraphyId = b.Id
			FOR xml path('')
			), 1, 1, '[') + ']') AS AuthorIds,
			poly.Id,
			poly.Name, 
			CreationDate, 
			Footnote,
			TotalPages,
			b.City,
			b.Publisher,
			ISBN,
			i.Number,
			Country,
			PatentNumber,
			PatentPublishmentDate,
			poly.ObjectType
			FROM dbo.Polygraphy poly
			LEFT JOIN dbo.Book b
			ON b.Id = poly.Id
			LEFT JOIN dbo.NewspaperIssue i
			ON i.Id = poly.Id
			LEFT JOIN dbo.Patent p
			ON p.Id = poly.Id
			WHERE poly.Name = @Name AND poly.Status = 1
END
GO
/****** Object:  StoredProcedure [dbo].[SearchNewspaperByName]    Script Date: 4/19/2022 9:52:37 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[SearchNewspaperByName] 
	@Name NVARCHAR(300)
AS
BEGIN
	SELECT (SELECT STUFF(
			(
			SELECT ',' + CONVERT(NVARCHAR(20), NewspaperId) 
			FROM dbo.NewspaperIssue i
			WHERE i.NewspaperId = n.Id
			FOR xml path('')
			), 1, 1, '[') + ']') AS IssueIds,
			n.Id,
			n.Name,
			ISSN
			FROM dbo.Newspaper n
			WHERE n.Name = @Name
END
GO
/****** Object:  StoredProcedure [dbo].[UpdateAuthorInLibrary]    Script Date: 4/19/2022 9:52:37 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[UpdateAuthorInLibrary]
	
	@Id INT,
	@Firstname NVARCHAR(50),
	@Lastname NVARCHAR(200)
AS
BEGIN
	DECLARE @IsUniqueResult INT
	SELECT @IsUniqueResult = dbo.ufn_IsAuthorUniqueUpdate(@Id, @Firstname, @Lastname)
	IF (@IsUniqueResult = 1)
	BEGIN
		BEGIN TRAN
			BEGIN TRY
				UPDATE dbo.Author
				SET Firstname = @Firstname,
				Lastname = @Lastname
				WHERE Id = @Id
			
				INSERT dbo.LibraryLogs VALUES (
				GETDATE(), 
				'Author', 
				@Id, 
				FORMATMESSAGE('Author #%d was updated', @Id),
				CURRENT_USER
				) 

			COMMIT TRAN
		END TRY
		BEGIN CATCH
			ROLLBACK TRAN
		END CATCH
	END
END
GO
/****** Object:  StoredProcedure [dbo].[UpdateBookInLibrary]    Script Date: 4/19/2022 9:52:37 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[UpdateBookInLibrary]
	
	@Id INT,
	@Name NVARCHAR(300),
	@AuthorIds NVARCHAR(MAX),
	@CreationDate DATE,
	@Footnote NVARCHAR(2000),
	@TotalPages INT,
	@City NVARCHAR(200),
	@Publisher NVARCHAR(300),
	@ISBN NVARCHAR(50)
AS
BEGIN
	DECLARE @IsUniqieResult INT
	SELECT @IsUniqieResult = dbo.ufn_IsBookUniqueUpdate(@Id, @ISBN, @Name, @CreationDate, @AuthorIds)
	IF (@IsUniqieResult = 1)
	BEGIN
		BEGIN TRAN
			BEGIN TRY
				UPDATE dbo.Polygraphy
				SET Name = @Name,
				CreationDate = @CreationDate,
				Footnote = @Footnote,
				TotalPages = @TotalPages
				WHERE Id = @Id

				UPDATE dbo.Book
				SET City = @City,
				Publisher = @Publisher,
				ISBN = @ISBN
				WHERE Id = @Id

				DELETE FROM dbo.AuthorPolygraphy
				WHERE PolygraphyId = @Id

				INSERT INTO dbo.AuthorPolygraphy (AuthorId, PolygraphyId)
				SELECT AuthorId, @Id
				FROM OPENJSON(@AuthorIds) WITH(AuthorId int '$')
			
				INSERT dbo.LibraryLogs VALUES (
				GETDATE(), 
				'Book', 
				@Id, 
				FORMATMESSAGE('Book #%d was updated', @Id),
				CURRENT_USER
				) 

			COMMIT TRAN
		END TRY
		BEGIN CATCH
			ROLLBACK TRAN
		END CATCH
	END
END
GO
/****** Object:  StoredProcedure [dbo].[UpdateNewspaperInLibrary]    Script Date: 4/19/2022 9:52:37 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[UpdateNewspaperInLibrary]
	
	@Id INT,
	@Name NVARCHAR(300),
	@ISSN NVARCHAR(50)
AS
BEGIN
	DECLARE @IsUniqueResult INT
	SELECT @IsUniqueResult = dbo.ufn_IsNewspaperUniqueUpdate(@Id, @Name, @ISSN)
	IF (@IsUniqueResult = 1)
	BEGIN
		BEGIN TRAN
			BEGIN TRY

				UPDATE dbo.Newspaper
				SET Name = @Name,
				ISSN = @ISSN
				WHERE Id = @Id
			
				INSERT dbo.LibraryLogs VALUES (
				GETDATE(), 
				'Newspaper', 
				@Id, 
				FORMATMESSAGE('Newspaper #%d was updated', @Id),
				CURRENT_USER
				) 

			COMMIT TRAN
		END TRY
		BEGIN CATCH
			ROLLBACK TRAN
		END CATCH
	END
END
GO
/****** Object:  StoredProcedure [dbo].[UpdateNewspaperIssueInLibrary]    Script Date: 4/19/2022 9:52:37 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[UpdateNewspaperIssueInLibrary]
	
	@Id INT,
	@NewspaperId INT,
	@Name NVARCHAR(300),
	@CreationDate DATE,
	@Footnote NVARCHAR(2000),
	@TotalPages INT,
	@City NVARCHAR(200),
	@Publisher NVARCHAR(300),
	@Number INT
AS
BEGIN
	DECLARE @IsUniqueResult INT
	SELECT @IsUniqueResult = dbo.ufn_IsNewspaperIssueUniqueUpdate(@Id, @Name, @CreationDate, @Publisher)
	IF (@IsUniqueResult = 1)
	BEGIN
		BEGIN TRAN
			BEGIN TRY
				UPDATE dbo.Polygraphy
				SET Name = @Name,
				CreationDate = @CreationDate,
				Footnote = @Footnote,
				TotalPages = @TotalPages
				WHERE Id = @Id

				UPDATE dbo.NewspaperIssue
				SET NewspaperId = @NewspaperId,
				City = @City,
				Publisher = @Publisher,
				Number = @Number
				WHERE Id = @Id
			
				INSERT dbo.LibraryLogs VALUES (
				GETDATE(), 
				'NewspaperIssue', 
				@Id, 
				FORMATMESSAGE('NewspaperIssue #%d was updated', @Id),
				CURRENT_USER
				) 

			COMMIT TRAN
		END TRY
		BEGIN CATCH
			ROLLBACK TRAN
		END CATCH
	END
END
GO
/****** Object:  StoredProcedure [dbo].[UpdatePatentInLibrary]    Script Date: 4/19/2022 9:52:37 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[UpdatePatentInLibrary]
	
	@Id INT,
	@Name NVARCHAR(300),
	@AuthorIds NVARCHAR(MAX),
	@CreationDate DATE,
	@Footnote NVARCHAR(2000),
	@TotalPages INT,
	@Country NVARCHAR(200),
	@PublishmentDate DATE,
	@Number INT
AS
BEGIN
	DECLARE @IsUniqueResult INT
	SELECT @IsUniqueResult = dbo.ufn_IsPatentUniqueUpdate(@Id, @Number, @Country)
	IF (@IsUniqueResult = 1)
	BEGIN
		BEGIN TRAN
			BEGIN TRY
				UPDATE dbo.Polygraphy
				SET Name = @Name,
				CreationDate = @CreationDate,
				Footnote = @Footnote,
				TotalPages = @TotalPages
				WHERE Id = @Id

				UPDATE dbo.Patent
				SET Country = @Country,
				PatentPublishmentDate = @PublishmentDate,
				PatentNumber = @Number
				WHERE Id = @Id

				UPDATE dbo.AuthorPolygraphy
				SET AuthorId = @AuthorIds
				FROM OPENJSON(@AuthorIds) WITH(AuthorId int '$')
				WHERE PolygraphyId = @Id
			
				INSERT dbo.LibraryLogs VALUES (
				GETDATE(), 
				'Patent', 
				@Id, 
				FORMATMESSAGE('Patent #%d was updated', @Id),
				CURRENT_USER
				) 

			COMMIT TRAN
		END TRY
		BEGIN CATCH
			ROLLBACK TRAN
		END CATCH
	END
END
GO
/****** Object:  StoredProcedure [dbo].[UpdateUserInLibrary]    Script Date: 4/19/2022 9:52:37 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[UpdateUserInLibrary]
	
	@Id INT,
	@Username NVARCHAR(50),
	@Password NVARCHAR(MAX)
	--@Role NVARCHAR(15)
AS
BEGIN
	DECLARE @IsUniqueResult INT
	SELECT @IsUniqueResult = dbo.ufn_IsUserUniqueUpdate(@Id, @Username)
	IF (@IsUniqueResult = 1)
	BEGIN
		BEGIN TRAN
			BEGIN TRY
				UPDATE dbo.[User]
				SET Username = @Username,
				Password = @Password
				--Role = @Role
				WHERE Id = @Id
			
				INSERT dbo.LibraryLogs VALUES (
				GETDATE(), 
				'User', 
				@Id, 
				FORMATMESSAGE('User #%d was updated', @Id),
				CURRENT_USER
				) 

			COMMIT TRAN
		END TRY
		BEGIN CATCH
			ROLLBACK TRAN
		END CATCH
	END
END
GO
USE [master]
GO
ALTER DATABASE [Library] SET  READ_WRITE 
GO
