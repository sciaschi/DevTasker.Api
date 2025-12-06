USE [DevTaskerDB];
GO

DELETE FROM [dbo].[work_logs];
DBCC CHECKIDENT ('dbo.work_logs', RESEED, 0);
GO

DELETE FROM [dbo].[task_items];
DBCC CHECKIDENT ('dbo.task_items', RESEED, 0);
GO

DELETE FROM [dbo].[projects];
DBCC CHECKIDENT ('dbo.projects', RESEED, 1000);
GO
