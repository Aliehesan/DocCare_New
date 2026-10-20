-- Needed for the filtered unique index below (sqlcmd defaults QUOTED_IDENTIFIER to OFF)
SET ANSI_NULLS ON;
SET QUOTED_IDENTIFIER ON;
GO

-- Doctor availability by weekday + leave days (replaces entering a DoctorSchedule row for every date).
-- Patients can book any date in the booking window on which the doctor works and is not on leave.
-- Safe to run more than once. The old DoctorSchedule table is left in place but no longer used.

-- Normal weekly hours, one row per doctor per weekday.
-- DayOfWeek: 0 = Sunday ... 6 = Saturday (same numbering as .NET DayOfWeek).
IF OBJECT_ID('dbo.DoctorWorkingHours', 'U') IS NULL
    CREATE TABLE dbo.DoctorWorkingHours (
        DoctorID     INT     NOT NULL,
        DayOfWeek    TINYINT NOT NULL,
        IsWorking    BIT     NOT NULL CONSTRAINT DF_DoctorWorkingHours_IsWorking DEFAULT 0,
        StartTime    TIME    NULL,
        EndTime      TIME    NULL,
        SlotDuration INT     NULL,   -- minutes
        MaxPatients  INT     NULL,   -- per day
        CONSTRAINT PK_DoctorWorkingHours PRIMARY KEY (DoctorID, DayOfWeek),
        CONSTRAINT FK_DoctorWorkingHours_Doctors FOREIGN KEY (DoctorID) REFERENCES dbo.Doctors (DoctorID),
        CONSTRAINT CK_DoctorWorkingHours_Day CHECK (DayOfWeek BETWEEN 0 AND 6),
        CONSTRAINT CK_DoctorWorkingHours_Times CHECK (IsWorking = 0 OR (StartTime < EndTime AND SlotDuration > 0 AND MaxPatients > 0))
    );

-- Dates a doctor is not available (leave, conference, holiday).
IF OBJECT_ID('dbo.DoctorLeave', 'U') IS NULL
    CREATE TABLE dbo.DoctorLeave (
        LeaveID   INT IDENTITY(1, 1) NOT NULL CONSTRAINT PK_DoctorLeave PRIMARY KEY,
        DoctorID  INT           NOT NULL CONSTRAINT FK_DoctorLeave_Doctors REFERENCES dbo.Doctors (DoctorID),
        LeaveDate DATE          NOT NULL,
        Reason    NVARCHAR(200) NULL,
        CreatedAt DATETIME      NOT NULL CONSTRAINT DF_DoctorLeave_CreatedAt DEFAULT GETDATE(),
        CONSTRAINT UQ_DoctorLeave UNIQUE (DoctorID, LeaveDate)
    );

-- One-time move: doctors who had per-date schedules but no weekly hours yet get their most recent
-- schedule as their hours Monday to Saturday (Sunday off). Doctors can change this on their page.
INSERT INTO dbo.DoctorWorkingHours (DoctorID, DayOfWeek, IsWorking, StartTime, EndTime, SlotDuration, MaxPatients)
SELECT s.DoctorID, d.DayOfWeek, CASE WHEN d.DayOfWeek = 0 THEN 0 ELSE 1 END,
       s.StartTime, s.EndTime, s.SlotDuration, s.MaxTokens
FROM (
    SELECT DoctorID, StartTime, EndTime, SlotDuration, MaxTokens,
           ROW_NUMBER() OVER (PARTITION BY DoctorID ORDER BY ScheduleDate DESC, ScheduleID DESC) AS rn
    FROM dbo.DoctorSchedule
    WHERE StartTime IS NOT NULL AND StartTime < EndTime AND SlotDuration > 0 AND MaxTokens > 0
) s
CROSS JOIN (VALUES (0), (1), (2), (3), (4), (5), (6)) d (DayOfWeek)
WHERE s.rn = 1
  AND NOT EXISTS (SELECT 1 FROM dbo.DoctorWorkingHours w WHERE w.DoctorID = s.DoctorID);

-- One active booking per doctor per time slot. If two patients pay for the same slot at the
-- same moment, the second insert fails and the booking page tells that patient the slot was taken.
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'UX_Appointments_ActiveSlot' AND object_id = OBJECT_ID('dbo.Appointments'))
    CREATE UNIQUE INDEX UX_Appointments_ActiveSlot
        ON dbo.Appointments (DoctorID, AppointDate, AppointTime)
        WHERE Status <> 'Cancelled' AND AppointTime IS NOT NULL;

-- Patients per day is worked out from the hours and slot length (whole slots that finish by the
-- end time); MaxPatients just stores that number for reports. Bring stored numbers in line.
UPDATE dbo.DoctorWorkingHours
SET MaxPatients = DATEDIFF(MINUTE, StartTime, EndTime) / SlotDuration
WHERE StartTime IS NOT NULL AND EndTime > StartTime AND SlotDuration > 0
  AND DATEDIFF(MINUTE, StartTime, EndTime) / SlotDuration >= 1
  AND ISNULL(MaxPatients, -1) <> DATEDIFF(MINUTE, StartTime, EndTime) / SlotDuration;

-- Optional break per weekday (e.g. lunch). Slots overlapping it are skipped; booking resumes at BreakEnd.
IF COL_LENGTH('dbo.DoctorWorkingHours', 'BreakStart') IS NULL
    ALTER TABLE dbo.DoctorWorkingHours ADD BreakStart TIME NULL, BreakEnd TIME NULL;
GO
IF NOT EXISTS (SELECT 1 FROM sys.check_constraints WHERE name = 'CK_DoctorWorkingHours_Break')
    ALTER TABLE dbo.DoctorWorkingHours ADD CONSTRAINT CK_DoctorWorkingHours_Break
        CHECK ((BreakStart IS NULL AND BreakEnd IS NULL) OR (BreakStart < BreakEnd));
