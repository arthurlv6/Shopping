﻿CREATE TABLE [dbo].[Stocks]
(
	[Id] INT IDENTITY (1, 1) NOT NULL PRIMARY KEY, 
    [Name] NCHAR(100) NOT NULL, 
    [Quantity] INT NOT NULL DEFAULT 1, 
    [Note] NCHAR(1000) NULL
)
