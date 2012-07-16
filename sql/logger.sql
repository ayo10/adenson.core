if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[EVENT_LOG]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
drop table [dbo].[EVENT_LOG]
GO
CREATE TABLE [dbo].[EVENT_LOG] (
	[error_id] [int] IDENTITY (1, 1) NOT FOR REPLICATION  NOT NULL ,
	[severity] [nchar] (10) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL ,
	[category] [varchar] (255) COLLATE SQL_Latin1_General_CP1_CI_AS NULL ,
	[error_text] [text] COLLATE SQL_Latin1_General_CP1_CI_AS NULL ,
	[url] [varchar] (1024) COLLATE SQL_Latin1_General_CP1_CI_AS NULL ,
	[created_date] [datetime] NOT NULL 
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
ALTER TABLE [dbo].[EVENT_LOG] ADD 
	CONSTRAINT [DF__EVENT_LOG_severity] DEFAULT (N'Debug') FOR [severity],
	CONSTRAINT [DF__EVENT_LOG_created_date] DEFAULT (getdate()) FOR [created_date]
GO
if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[EVENTLOGGER_INS_ERROR]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[EVENTLOGGER_INS_ERROR]
GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO
/*
  **
  ** CREATION DATE: 02/24/2004
  ** PURPOSE : Logs exceprion
  **
  **
  ** MODIFICATION HISTORY
  **
  **
  **
  **
*/
CREATE PROCEDURE [dbo].[EVENTLOGGER_INS_ERROR] @severity nchar(10), @category varchar(255), @error_text varchar(8000), @url varchar(1024)
AS
INSERT INTO [dbo].[EVENT_LOG](severity, [category], [error_text], url)
VALUES(@severity, @category, @error_text, @url)
GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO