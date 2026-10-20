-- Permanent, unique Patient ID shown to staff and patients (e.g. DC-00029), made from PatientID.
-- Computed by the database, so every patient (old and new) has one and it never changes.
-- Safe to run more than once.
SET QUOTED_IDENTIFIER ON;
SET ANSI_NULLS ON;
SET ANSI_PADDING ON;
SET ANSI_WARNINGS ON;
SET ARITHABORT ON;
SET CONCAT_NULL_YIELDS_NULL ON;
SET NUMERIC_ROUNDABORT OFF;

IF COL_LENGTH('dbo.Patients', 'PatientCode') IS NULL
    ALTER TABLE dbo.Patients ADD PatientCode AS
        ('DC-' + CASE WHEN PatientID < 100000 THEN RIGHT('0000' + CAST(PatientID AS VARCHAR(10)), 5)
                      ELSE CAST(PatientID AS VARCHAR(10)) END) PERSISTED;
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'UX_Patients_PatientCode' AND object_id = OBJECT_ID('dbo.Patients'))
    CREATE UNIQUE INDEX UX_Patients_PatientCode ON dbo.Patients (PatientCode);
GO
