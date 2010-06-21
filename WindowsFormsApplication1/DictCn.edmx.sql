
-- --------------------------------------------------
-- Entity Designer DDL Script for SQL Server 2005, 2008, and Azure
-- --------------------------------------------------
-- Date Created: 06/17/2010 16:19:54
-- Generated from EDMX file: c:\users\administrator\documents\visual studio 2010\Projects\WindowsFormsApplication1\WindowsFormsApplication1\DictCn.edmx
-- --------------------------------------------------

SET QUOTED_IDENTIFIER OFF;
GO
USE [C:\USERS\ADMINISTRATOR\DOCUMENTS\VISUAL STUDIO 2010\PROJECTS\WINDOWSFORMSAPPLICATION1\WINDOWSFORMSAPPLICATION1\DICTCN.MDF];
GO
IF SCHEMA_ID(N'dbo') IS NULL EXECUTE(N'CREATE SCHEMA [dbo]');
GO

-- --------------------------------------------------
-- Dropping existing FOREIGN KEY constraints
-- --------------------------------------------------


-- --------------------------------------------------
-- Dropping existing tables
-- --------------------------------------------------


-- --------------------------------------------------
-- Creating all tables
-- --------------------------------------------------

-- Creating table '分类集'
CREATE TABLE [dbo].[分类集] (
    [ID] int IDENTITY(1,1) NOT NULL,
    [名称] nvarchar(max)  NOT NULL,
    [地址] nvarchar(max)  NOT NULL
);
GO

-- --------------------------------------------------
-- Creating all PRIMARY KEY constraints
-- --------------------------------------------------

-- Creating primary key on [ID] in table '分类集'
ALTER TABLE [dbo].[分类集]
ADD CONSTRAINT [PK_分类集]
    PRIMARY KEY CLUSTERED ([ID] ASC);
GO

-- --------------------------------------------------
-- Creating all FOREIGN KEY constraints
-- --------------------------------------------------

-- --------------------------------------------------
-- Script has ended
-- --------------------------------------------------