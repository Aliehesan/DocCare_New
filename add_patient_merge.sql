-- Merging duplicate patient records (Patients > Merge Duplicates): the duplicate is kept but hidden,
-- with the record it was merged into and when. Safe to run more than once.
SET QUOTED_IDENTIFIER ON;
SET ANSI_NULLS ON;

IF COL_LENGTH('dbo.Patients', 'MergedIntoPatientID') IS NULL
    ALTER TABLE dbo.Patients ADD MergedIntoPatientID INT NULL, MergedAt DATETIME NULL, MergedBy NVARCHAR(100) NULL;
GO
