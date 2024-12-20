USE [master]
GO
/****** Object:  Database [HumanResourcesDepartment]    Script Date: 10.12.2024 10:03:31 ******/
CREATE DATABASE [HumanResourcesDepartment]
 CONTAINMENT = NONE
 ON  PRIMARY 
( NAME = N'Human Resources Department', FILENAME = N'D:\Sql\Human Resources Department.mdf' , SIZE = 73728KB , MAXSIZE = UNLIMITED, FILEGROWTH = 65536KB )
 LOG ON 
( NAME = N'Human Resources Department_log', FILENAME = N'D:\Sql\Human Resources Department_log.ldf' , SIZE = 8192KB , MAXSIZE = 2048GB , FILEGROWTH = 65536KB )
 WITH CATALOG_COLLATION = DATABASE_DEFAULT, LEDGER = OFF
GO
ALTER DATABASE [HumanResourcesDepartment] SET COMPATIBILITY_LEVEL = 160
GO
IF (1 = FULLTEXTSERVICEPROPERTY('IsFullTextInstalled'))
begin
EXEC [HumanResourcesDepartment].[dbo].[sp_fulltext_database] @action = 'enable'
end
GO
ALTER DATABASE [HumanResourcesDepartment] SET ANSI_NULL_DEFAULT OFF 
GO
ALTER DATABASE [HumanResourcesDepartment] SET ANSI_NULLS OFF 
GO
ALTER DATABASE [HumanResourcesDepartment] SET ANSI_PADDING OFF 
GO
ALTER DATABASE [HumanResourcesDepartment] SET ANSI_WARNINGS OFF 
GO
ALTER DATABASE [HumanResourcesDepartment] SET ARITHABORT OFF 
GO
ALTER DATABASE [HumanResourcesDepartment] SET AUTO_CLOSE OFF 
GO
ALTER DATABASE [HumanResourcesDepartment] SET AUTO_SHRINK OFF 
GO
ALTER DATABASE [HumanResourcesDepartment] SET AUTO_UPDATE_STATISTICS ON 
GO
ALTER DATABASE [HumanResourcesDepartment] SET CURSOR_CLOSE_ON_COMMIT OFF 
GO
ALTER DATABASE [HumanResourcesDepartment] SET CURSOR_DEFAULT  GLOBAL 
GO
ALTER DATABASE [HumanResourcesDepartment] SET CONCAT_NULL_YIELDS_NULL OFF 
GO
ALTER DATABASE [HumanResourcesDepartment] SET NUMERIC_ROUNDABORT OFF 
GO
ALTER DATABASE [HumanResourcesDepartment] SET QUOTED_IDENTIFIER OFF 
GO
ALTER DATABASE [HumanResourcesDepartment] SET RECURSIVE_TRIGGERS OFF 
GO
ALTER DATABASE [HumanResourcesDepartment] SET  DISABLE_BROKER 
GO
ALTER DATABASE [HumanResourcesDepartment] SET AUTO_UPDATE_STATISTICS_ASYNC OFF 
GO
ALTER DATABASE [HumanResourcesDepartment] SET DATE_CORRELATION_OPTIMIZATION OFF 
GO
ALTER DATABASE [HumanResourcesDepartment] SET TRUSTWORTHY ON 
GO
ALTER DATABASE [HumanResourcesDepartment] SET ALLOW_SNAPSHOT_ISOLATION OFF 
GO
ALTER DATABASE [HumanResourcesDepartment] SET PARAMETERIZATION SIMPLE 
GO
ALTER DATABASE [HumanResourcesDepartment] SET READ_COMMITTED_SNAPSHOT OFF 
GO
ALTER DATABASE [HumanResourcesDepartment] SET HONOR_BROKER_PRIORITY OFF 
GO
ALTER DATABASE [HumanResourcesDepartment] SET RECOVERY FULL 
GO
ALTER DATABASE [HumanResourcesDepartment] SET  MULTI_USER 
GO
ALTER DATABASE [HumanResourcesDepartment] SET PAGE_VERIFY CHECKSUM  
GO
ALTER DATABASE [HumanResourcesDepartment] SET DB_CHAINING OFF 
GO
ALTER DATABASE [HumanResourcesDepartment] SET FILESTREAM( NON_TRANSACTED_ACCESS = OFF ) 
GO
ALTER DATABASE [HumanResourcesDepartment] SET TARGET_RECOVERY_TIME = 60 SECONDS 
GO
ALTER DATABASE [HumanResourcesDepartment] SET DELAYED_DURABILITY = DISABLED 
GO
ALTER DATABASE [HumanResourcesDepartment] SET ACCELERATED_DATABASE_RECOVERY = OFF  
GO
EXEC sys.sp_db_vardecimal_storage_format N'HumanResourcesDepartment', N'ON'
GO
ALTER DATABASE [HumanResourcesDepartment] SET QUERY_STORE = ON
GO
ALTER DATABASE [HumanResourcesDepartment] SET QUERY_STORE (OPERATION_MODE = READ_WRITE, CLEANUP_POLICY = (STALE_QUERY_THRESHOLD_DAYS = 30), DATA_FLUSH_INTERVAL_SECONDS = 900, INTERVAL_LENGTH_MINUTES = 60, MAX_STORAGE_SIZE_MB = 1000, QUERY_CAPTURE_MODE = AUTO, SIZE_BASED_CLEANUP_MODE = AUTO, MAX_PLANS_PER_QUERY = 200, WAIT_STATS_CAPTURE_MODE = ON)
GO
USE [HumanResourcesDepartment]
GO
/****** Object:  Table [dbo].[Citizenship]    Script Date: 10.12.2024 10:03:32 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Citizenship](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[Country] [nvarchar](50) NULL,
 CONSTRAINT [PK_Citizenship] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Department]    Script Date: 10.12.2024 10:03:32 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Department](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[Title] [nvarchar](50) NULL,
	[Responsibilities] [nvarchar](50) NULL,
 CONSTRAINT [PK_Department] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Education]    Script Date: 10.12.2024 10:03:32 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Education](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[Levell] [nvarchar](50) NULL,
 CONSTRAINT [PK_Education] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Entry_in_the_work_book]    Script Date: 10.12.2024 10:03:32 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Entry_in_the_work_book](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[Id_personal_card] [int] NULL,
	[Date] [date] NULL,
	[Reason] [nvarchar](50) NULL,
	[Id_mixing] [int] NULL,
 CONSTRAINT [PK_Entry_in_the_work_book] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Mixing]    Script Date: 10.12.2024 10:03:32 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Mixing](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[Title] [nvarchar](250) NULL,
 CONSTRAINT [PK_Mixing] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Personal_card]    Script Date: 10.12.2024 10:03:32 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Personal_card](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[Surname] [nvarchar](50) NULL,
	[Name] [nvarchar](50) NULL,
	[Patronymic] [nvarchar](50) NOT NULL,
	[Date_of_birth] [date] NULL,
	[Series_and_number] [nvarchar](50) NULL,
	[Issued_by_whom] [nvarchar](50) NULL,
	[Date_of_issue] [date] NULL,
	[Registration_address] [nvarchar](255) NULL,
	[Telephone] [nvarchar](50) NULL,
	[Children] [bit] NULL,
	[Military_service] [bit] NULL,
	[Photo] [varbinary](max) NULL,
	[Login] [nvarchar](50) NULL,
	[Password] [nvarchar](50) NULL,
	[Id_citizenship] [int] NULL,
	[Id_post] [int] NULL,
	[Id_education] [int] NULL,
	[Birthplace] [nvarchar](250) NULL,
	[Email] [nvarchar](150) NULL,
	[EducationInstitution] [nvarchar](50) NULL,
 CONSTRAINT [PK_Personal_card] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Post]    Script Date: 10.12.2024 10:03:33 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Post](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[Title] [nvarchar](50) NULL,
	[Base_salary] [float] NULL,
	[Id_department] [int] NULL,
 CONSTRAINT [PK_Post] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Salary]    Script Date: 10.12.2024 10:03:33 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Salary](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[Id_personal_card] [int] NULL,
	[Amount] [float] NULL,
	[Date] [date] NULL,
	[Id_salary_type] [int] NULL,
 CONSTRAINT [PK_Salary] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Salary_type]    Script Date: 10.12.2024 10:03:33 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Salary_type](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[Title] [nvarchar](50) NULL,
 CONSTRAINT [PK_Salary_type] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
ALTER TABLE [dbo].[Entry_in_the_work_book]  WITH CHECK ADD  CONSTRAINT [FK_Entry_in_the_work_book_Mixing] FOREIGN KEY([Id_mixing])
REFERENCES [dbo].[Mixing] ([ID])
GO
ALTER TABLE [dbo].[Entry_in_the_work_book] CHECK CONSTRAINT [FK_Entry_in_the_work_book_Mixing]
GO
ALTER TABLE [dbo].[Entry_in_the_work_book]  WITH CHECK ADD  CONSTRAINT [FK_Entry_in_the_work_book_Personal_card] FOREIGN KEY([Id_personal_card])
REFERENCES [dbo].[Personal_card] ([ID])
GO
ALTER TABLE [dbo].[Entry_in_the_work_book] CHECK CONSTRAINT [FK_Entry_in_the_work_book_Personal_card]
GO
ALTER TABLE [dbo].[Personal_card]  WITH CHECK ADD  CONSTRAINT [FK_Personal_card_Citizenship] FOREIGN KEY([Id_citizenship])
REFERENCES [dbo].[Citizenship] ([ID])
GO
ALTER TABLE [dbo].[Personal_card] CHECK CONSTRAINT [FK_Personal_card_Citizenship]
GO
ALTER TABLE [dbo].[Personal_card]  WITH CHECK ADD  CONSTRAINT [FK_Personal_card_Education] FOREIGN KEY([Id_education])
REFERENCES [dbo].[Education] ([ID])
GO
ALTER TABLE [dbo].[Personal_card] CHECK CONSTRAINT [FK_Personal_card_Education]
GO
ALTER TABLE [dbo].[Personal_card]  WITH CHECK ADD  CONSTRAINT [FK_Personal_card_Post1] FOREIGN KEY([Id_post])
REFERENCES [dbo].[Post] ([ID])
GO
ALTER TABLE [dbo].[Personal_card] CHECK CONSTRAINT [FK_Personal_card_Post1]
GO
ALTER TABLE [dbo].[Post]  WITH CHECK ADD  CONSTRAINT [FK_Post_Department] FOREIGN KEY([Id_department])
REFERENCES [dbo].[Department] ([ID])
GO
ALTER TABLE [dbo].[Post] CHECK CONSTRAINT [FK_Post_Department]
GO
ALTER TABLE [dbo].[Salary]  WITH CHECK ADD  CONSTRAINT [FK_Salary_Personal_card] FOREIGN KEY([Id_personal_card])
REFERENCES [dbo].[Personal_card] ([ID])
GO
ALTER TABLE [dbo].[Salary] CHECK CONSTRAINT [FK_Salary_Personal_card]
GO
ALTER TABLE [dbo].[Salary]  WITH CHECK ADD  CONSTRAINT [FK_Salary_Salary_type] FOREIGN KEY([Id_salary_type])
REFERENCES [dbo].[Salary_type] ([ID])
GO
ALTER TABLE [dbo].[Salary] CHECK CONSTRAINT [FK_Salary_Salary_type]
GO
USE [master]
GO
ALTER DATABASE [HumanResourcesDepartment] SET  READ_WRITE 
GO
