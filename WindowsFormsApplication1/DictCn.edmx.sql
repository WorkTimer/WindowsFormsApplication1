
-- --------------------------------------------------
-- Entity Designer DDL Script for SQL Server 2005, 2008, and Azure
-- --------------------------------------------------
-- Date Created: 06/30/2010 13:20:16
-- Generated from EDMX file: C:\Users\Administrator\Documents\Visual Studio 2010\Projects\WindowsFormsApplication1\WindowsFormsApplication1\DictCn.edmx
-- --------------------------------------------------

SET QUOTED_IDENTIFIER OFF;
GO
USE [C:\Users\Administrator\Documents\Visual Studio 2010\Projects\WindowsFormsApplication1\WindowsFormsApplication1\DictCn.mdf];
GO
IF SCHEMA_ID(N'dbo') IS NULL EXECUTE(N'CREATE SCHEMA [dbo]');
GO

-- --------------------------------------------------
-- Dropping existing FOREIGN KEY constraints
-- --------------------------------------------------

IF OBJECT_ID(N'[dbo].[FK_分类课本]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[课本集] DROP CONSTRAINT [FK_分类课本];
GO
IF OBJECT_ID(N'[dbo].[FK_课本分组]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[分组集] DROP CONSTRAINT [FK_课本分组];
GO
IF OBJECT_ID(N'[dbo].[FK_分组分页]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[分页集] DROP CONSTRAINT [FK_分组分页];
GO
IF OBJECT_ID(N'[dbo].[FK_分页单词]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[单词集] DROP CONSTRAINT [FK_分页单词];
GO
IF OBJECT_ID(N'[dbo].[FK_单词扫描指针]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[扫描指针集] DROP CONSTRAINT [FK_单词扫描指针];
GO
IF OBJECT_ID(N'[dbo].[FK_分类扫描指针]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[扫描指针集] DROP CONSTRAINT [FK_分类扫描指针];
GO
IF OBJECT_ID(N'[dbo].[FK_课本扫描指针]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[扫描指针集] DROP CONSTRAINT [FK_课本扫描指针];
GO
IF OBJECT_ID(N'[dbo].[FK_分组扫描指针]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[扫描指针集] DROP CONSTRAINT [FK_分组扫描指针];
GO
IF OBJECT_ID(N'[dbo].[FK_分页扫描指针]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[扫描指针集] DROP CONSTRAINT [FK_分页扫描指针];
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
IF OBJECT_ID(N'[dbo].[扫描指针集]', 'U') IS NOT NULL
    DROP TABLE [dbo].[扫描指针集];
GO
IF OBJECT_ID(N'[dbo].[分组集]', 'U') IS NOT NULL
    DROP TABLE [dbo].[分组集];
GO
IF OBJECT_ID(N'[dbo].[分页集]', 'U') IS NOT NULL
    DROP TABLE [dbo].[分页集];
GO

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

-- Creating table '单词集'
CREATE TABLE [dbo].[单词集] (
    [ID] int IDENTITY(1,1) NOT NULL,
    [拼写] nvarchar(max)  NOT NULL,
    [音标] nvarchar(max)  NOT NULL,
    [解释] nvarchar(max)  NOT NULL,
    [读音] nvarchar(max)  NOT NULL,
    [分页_ID] int  NOT NULL
);
GO

-- Creating table '课本集'
CREATE TABLE [dbo].[课本集] (
    [ID] int IDENTITY(1,1) NOT NULL,
    [名称] nvarchar(max)  NOT NULL,
    [地址] nvarchar(max)  NOT NULL,
    [单词数量] int  NOT NULL,
    [分组方式] tinyint  NOT NULL,
    [分类_ID] int  NOT NULL
);
GO

-- Creating table '扫描指针集'
CREATE TABLE [dbo].[扫描指针集] (
    [ID] int IDENTITY(1,1) NOT NULL,
    [扫描日期] nvarchar(max)  NOT NULL,
    [当前地址] nvarchar(max)  NOT NULL,
    [单词_ID] int  NULL,
    [分类_ID] int  NULL,
    [课本_ID] int  NULL,
    [分组_ID] int  NULL,
    [分页_ID] int  NULL
);
GO

-- Creating table '分组集'
CREATE TABLE [dbo].[分组集] (
    [ID] int IDENTITY(1,1) NOT NULL,
    [名称] nvarchar(max)  NOT NULL,
    [地址] nvarchar(max)  NOT NULL,
    [单词数量] int  NOT NULL,
    [页数] int  NOT NULL,
    [课本_ID] int  NOT NULL
);
GO

-- Creating table '分页集'
CREATE TABLE [dbo].[分页集] (
    [ID] int IDENTITY(1,1) NOT NULL,
    [名称] nvarchar(max)  NOT NULL,
    [地址] nvarchar(max)  NOT NULL,
    [单词数量] int  NOT NULL,
    [分组_ID] int  NOT NULL
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

-- Creating primary key on [ID] in table '扫描指针集'
ALTER TABLE [dbo].[扫描指针集]
ADD CONSTRAINT [PK_扫描指针集]
    PRIMARY KEY CLUSTERED ([ID] ASC);
GO

-- Creating primary key on [ID] in table '分组集'
ALTER TABLE [dbo].[分组集]
ADD CONSTRAINT [PK_分组集]
    PRIMARY KEY CLUSTERED ([ID] ASC);
GO

-- Creating primary key on [ID] in table '分页集'
ALTER TABLE [dbo].[分页集]
ADD CONSTRAINT [PK_分页集]
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

-- Creating foreign key on [课本_ID] in table '分组集'
ALTER TABLE [dbo].[分组集]
ADD CONSTRAINT [FK_课本分组]
    FOREIGN KEY ([课本_ID])
    REFERENCES [dbo].[课本集]
        ([ID])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_课本分组'
CREATE INDEX [IX_FK_课本分组]
ON [dbo].[分组集]
    ([课本_ID]);
GO

-- Creating foreign key on [分组_ID] in table '分页集'
ALTER TABLE [dbo].[分页集]
ADD CONSTRAINT [FK_分组分页]
    FOREIGN KEY ([分组_ID])
    REFERENCES [dbo].[分组集]
        ([ID])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_分组分页'
CREATE INDEX [IX_FK_分组分页]
ON [dbo].[分页集]
    ([分组_ID]);
GO

-- Creating foreign key on [分页_ID] in table '单词集'
ALTER TABLE [dbo].[单词集]
ADD CONSTRAINT [FK_分页单词]
    FOREIGN KEY ([分页_ID])
    REFERENCES [dbo].[分页集]
        ([ID])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_分页单词'
CREATE INDEX [IX_FK_分页单词]
ON [dbo].[单词集]
    ([分页_ID]);
GO

-- Creating foreign key on [单词_ID] in table '扫描指针集'
ALTER TABLE [dbo].[扫描指针集]
ADD CONSTRAINT [FK_单词扫描指针]
    FOREIGN KEY ([单词_ID])
    REFERENCES [dbo].[单词集]
        ([ID])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_单词扫描指针'
CREATE INDEX [IX_FK_单词扫描指针]
ON [dbo].[扫描指针集]
    ([单词_ID]);
GO

-- Creating foreign key on [分类_ID] in table '扫描指针集'
ALTER TABLE [dbo].[扫描指针集]
ADD CONSTRAINT [FK_分类扫描指针]
    FOREIGN KEY ([分类_ID])
    REFERENCES [dbo].[分类集]
        ([ID])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_分类扫描指针'
CREATE INDEX [IX_FK_分类扫描指针]
ON [dbo].[扫描指针集]
    ([分类_ID]);
GO

-- Creating foreign key on [课本_ID] in table '扫描指针集'
ALTER TABLE [dbo].[扫描指针集]
ADD CONSTRAINT [FK_课本扫描指针]
    FOREIGN KEY ([课本_ID])
    REFERENCES [dbo].[课本集]
        ([ID])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_课本扫描指针'
CREATE INDEX [IX_FK_课本扫描指针]
ON [dbo].[扫描指针集]
    ([课本_ID]);
GO

-- Creating foreign key on [分组_ID] in table '扫描指针集'
ALTER TABLE [dbo].[扫描指针集]
ADD CONSTRAINT [FK_分组扫描指针]
    FOREIGN KEY ([分组_ID])
    REFERENCES [dbo].[分组集]
        ([ID])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_分组扫描指针'
CREATE INDEX [IX_FK_分组扫描指针]
ON [dbo].[扫描指针集]
    ([分组_ID]);
GO

-- Creating foreign key on [分页_ID] in table '扫描指针集'
ALTER TABLE [dbo].[扫描指针集]
ADD CONSTRAINT [FK_分页扫描指针]
    FOREIGN KEY ([分页_ID])
    REFERENCES [dbo].[分页集]
        ([ID])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_分页扫描指针'
CREATE INDEX [IX_FK_分页扫描指针]
ON [dbo].[扫描指针集]
    ([分页_ID]);
GO

-- --------------------------------------------------
-- Script has ended
-- --------------------------------------------------