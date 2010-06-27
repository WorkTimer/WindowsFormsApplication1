
-- --------------------------------------------------
-- Entity Designer DDL Script for SQL Server 2005, 2008, and Azure
-- --------------------------------------------------
-- Date Created: 06/25/2010 13:29:05
-- Generated from EDMX file: C:\Users\Administrator\Documents\Visual Studio 2010\Projects\WindowsFormsApplication1\WindowsFormsApplication1\DictCn.edmx
-- --------------------------------------------------

SET QUOTED_IDENTIFIER OFF;
GO
USE [C:\Users\Administrator\Documents\Visual Studio 2010\Projects\WindowsFormsApplication1\WindowsFormsApplication1\DictCn.MDF];
GO
IF SCHEMA_ID(N'dbo') IS NULL EXECUTE(N'CREATE SCHEMA [dbo]');
GO

-- --------------------------------------------------
-- Dropping existing FOREIGN KEY constraints
-- --------------------------------------------------

IF OBJECT_ID(N'[dbo].[FK_分类课本]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[课本集] DROP CONSTRAINT [FK_分类课本];
GO
IF OBJECT_ID(N'[dbo].[FK_课本单词]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[单词集] DROP CONSTRAINT [FK_课本单词];
GO

-- --------------------------------------------------
-- Dropping existing tables
-- --------------------------------------------------

IF OBJECT_ID(N'[dbo].[分类集]', 'U') IS NOT NULL
    DROP TABLE [dbo].[分类集];
GO
IF OBJECT_ID(N'[dbo].[单词集]', 'U') IS NOT NULL
    DROP TABLE [dbo].[单词集];
GO
IF OBJECT_ID(N'[dbo].[课本集]', 'U') IS NOT NULL
    DROP TABLE [dbo].[课本集];
GO

-- --------------------------------------------------
-- Creating all tables
-- --------------------------------------------------

-- Creating table '分类集'
CREATE TABLE [dbo].[分类集] (
    [ID] int IDENTITY(1,1) NOT NULL,
    [名称] nvarchar(max)  NOT NULL,
    [地址] nvarchar(max)  NOT NULL,
    [更新日期] datetime  NOT NULL
);
GO

-- Creating table '单词集'
CREATE TABLE [dbo].[单词集] (
    [ID] int IDENTITY(1,1) NOT NULL,
    [拼写] nvarchar(max)  NOT NULL,
    [音标] nvarchar(max)  NOT NULL,
    [更新日期] datetime  NOT NULL,
    [解释] nvarchar(max)  NOT NULL,
    [读音] nvarchar(max)  NOT NULL,
    [本组词数] nvarchar(max)  NOT NULL,
    [组地址] nvarchar(max)  NOT NULL,
    [页地址] nvarchar(max)  NOT NULL,
    [所在组] nvarchar(max)  NOT NULL,
    [所在页] nvarchar(max)  NOT NULL,
    [课本_ID] int  NOT NULL
);
GO

-- Creating table '课本集'
CREATE TABLE [dbo].[课本集] (
    [ID] int IDENTITY(1,1) NOT NULL,
    [更新日期] datetime  NOT NULL,
    [名称] nvarchar(max)  NOT NULL,
    [地址] nvarchar(max)  NOT NULL,
    [单词数量] int  NOT NULL,
    [分组方式] tinyint  NOT NULL,
    [分组总数] int  NOT NULL,
    [分类_ID] int  NOT NULL
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

-- Creating primary key on [ID] in table '单词集'
ALTER TABLE [dbo].[单词集]
ADD CONSTRAINT [PK_单词集]
    PRIMARY KEY CLUSTERED ([ID] ASC);
GO

-- Creating primary key on [ID] in table '课本集'
ALTER TABLE [dbo].[课本集]
ADD CONSTRAINT [PK_课本集]
    PRIMARY KEY CLUSTERED ([ID] ASC);
GO

-- --------------------------------------------------
-- Creating all FOREIGN KEY constraints
-- --------------------------------------------------

-- Creating foreign key on [分类_ID] in table '课本集'
ALTER TABLE [dbo].[课本集]
ADD CONSTRAINT [FK_分类课本]
    FOREIGN KEY ([分类_ID])
    REFERENCES [dbo].[分类集]
        ([ID])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_分类课本'
CREATE INDEX [IX_FK_分类课本]
ON [dbo].[课本集]
    ([分类_ID]);
GO

-- Creating foreign key on [课本_ID] in table '单词集'
ALTER TABLE [dbo].[单词集]
ADD CONSTRAINT [FK_课本单词]
    FOREIGN KEY ([课本_ID])
    REFERENCES [dbo].[课本集]
        ([ID])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_课本单词'
CREATE INDEX [IX_FK_课本单词]
ON [dbo].[单词集]
    ([课本_ID]);
GO

-- --------------------------------------------------
-- Script has ended
-- --------------------------------------------------