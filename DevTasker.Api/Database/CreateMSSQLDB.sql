CREATE DATABASE [DevTaskerDB];

USE [DevTaskerDB];

CREATE TABLE [dbo].[projects] (
    [Id]          INT           IDENTITY (1001, 1) NOT NULL,
    [Name]        VARCHAR (50)  NOT NULL,
    [Description] NTEXT         NULL,
    [is_archived] BIT           DEFAULT ((0)) NOT NULL,
    [created_at]  DATETIME2 (7) DEFAULT CURRENT_TIMESTAMP NOT NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC)
);

CREATE TABLE [dbo].[task_items] (
    [id]           INT           IDENTITY (1, 1) NOT NULL,
    [project_id]   INT           NOT NULL,
    [title]        VARCHAR (50)  NOT NULL,
    [description]  TEXT          NULL,
    [status]       INT           NOT NULL,
    [priority]     INT           NOT NULL,
    [due_date]     DATETIME      NULL,
    [completed_at] DATETIME      NULL,
    [created_at]   DATETIME2 (7) DEFAULT CURRENT_TIMESTAMP NOT NULL,
    [is_archived]  BIT           DEFAULT ((0)) NOT NULL,
    PRIMARY KEY CLUSTERED ([id] ASC),
    CONSTRAINT [FK_task_items_ToProjects] FOREIGN KEY ([project_id]) REFERENCES [dbo].[projects] ([Id])
);

CREATE TABLE [dbo].[work_logs] (
    [id]           INT           IDENTITY (1, 1) NOT NULL,
    [task_item_id] INT           NOT NULL,
    [started_at]   DATETIME      NULL,
    [ended_at]     DATETIME      NULL,
    [comment]      TEXT          NULL,
    [created_at]   DATETIME2 (7) DEFAULT CURRENT_TIMESTAMP NOT NULL
    PRIMARY KEY CLUSTERED ([id] ASC),
    CONSTRAINT [FK_work_logs_ToTaskItem] FOREIGN KEY ([task_item_id]) REFERENCES [dbo].[task_items] ([id])
);
