-- Medical records: vitals and the doctor's notes per visit, prescription lines, and each patient's
-- permanent health facts (allergies, blood group, ...). Safe to run more than once.
SET QUOTED_IDENTIFIER ON;
SET ANSI_NULLS ON;

-- One row per appointment. Vitals can be entered by reception or the doctor; the rest only by the doctor.
IF OBJECT_ID('dbo.VisitNotes', 'U') IS NULL
    CREATE TABLE dbo.VisitNotes (
        AppointmentID   INT            NOT NULL PRIMARY KEY
                                       CONSTRAINT FK_VisitNotes_Appointments REFERENCES dbo.Appointments (ID),
        BloodPressure   NVARCHAR(15)   NULL,     -- e.g. 120/80
        Pulse           INT            NULL,     -- beats per minute
        TemperatureF    DECIMAL(4, 1)  NULL,     -- degrees Fahrenheit
        WeightKg        DECIMAL(5, 1)  NULL,
        SpO2            INT            NULL,     -- percent
        Complaint       NVARCHAR(1000) NULL,
        Diagnosis       NVARCHAR(1000) NULL,
        Advice          NVARCHAR(2000) NULL,
        FollowUpDate    DATE           NULL,
        VitalsBy        NVARCHAR(100)  NULL,
        VitalsAt        DATETIME       NULL,
        NotesByDoctorID INT            NULL,
        NotesAt         DATETIME       NULL
    );
GO

IF OBJECT_ID('dbo.PrescriptionItems', 'U') IS NULL
    CREATE TABLE dbo.PrescriptionItems (
        ID            INT IDENTITY(1, 1) PRIMARY KEY,
        AppointmentID INT           NOT NULL CONSTRAINT FK_PrescriptionItems_Appointments REFERENCES dbo.Appointments (ID),
        SortOrder     INT           NOT NULL DEFAULT 0,
        Medicine      NVARCHAR(150) NOT NULL,
        Dose          NVARCHAR(60)  NULL,     -- e.g. 500 mg, 1 tablet
        Frequency     NVARCHAR(60)  NULL,     -- e.g. 1-0-1
        Days          INT           NULL,
        Instructions  NVARCHAR(200) NULL      -- e.g. after food
    );
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_PrescriptionItems_Appointment' AND object_id = OBJECT_ID('dbo.PrescriptionItems'))
    CREATE INDEX IX_PrescriptionItems_Appointment ON dbo.PrescriptionItems (AppointmentID, SortOrder);
GO

-- Permanent facts shown at the top of the patient's profile; kept up to date by the doctor
IF OBJECT_ID('dbo.PatientHealth', 'U') IS NULL
    CREATE TABLE dbo.PatientHealth (
        PatientID        INT           NOT NULL PRIMARY KEY CONSTRAINT FK_PatientHealth_Patients REFERENCES dbo.Patients (PatientID),
        BloodGroup       NVARCHAR(5)   NULL,
        Allergies        NVARCHAR(500) NULL,
        Conditions       NVARCHAR(500) NULL,   -- long-term: diabetes, BP, asthma ...
        CurrentMedicines NVARCHAR(500) NULL,
        PastSurgeries    NVARCHAR(500) NULL,
        UpdatedBy        NVARCHAR(100) NULL,
        UpdatedAt        DATETIME      NULL
    );
GO

-- Follow-ups: one reminder email per follow-up, and staff can close one that is not needed.
-- A follow-up is open until the patient has a later (not cancelled, not missed) visit with the same doctor.
IF COL_LENGTH('dbo.VisitNotes', 'FollowUpReminderSentAt') IS NULL
    ALTER TABLE dbo.VisitNotes ADD FollowUpReminderSentAt DATETIME NULL;
IF COL_LENGTH('dbo.VisitNotes', 'FollowUpClosedAt') IS NULL
    ALTER TABLE dbo.VisitNotes ADD FollowUpClosedAt DATETIME NULL, FollowUpClosedBy NVARCHAR(100) NULL;
GO
