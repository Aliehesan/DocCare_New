-- DocCare Clinic: creates an EMPTY database (tables, keys, indexes, stored procedures). No patient data.
-- Generated from the live database structure. Run it on a new, empty database, then run seed_admin.sql
-- (or create the first admin account yourself). The add_*.sql scripts are not needed after this one.
SET ANSI_NULLS ON;
SET QUOTED_IDENTIFIER ON;
SET ANSI_PADDING ON;
GO

IF OBJECT_ID('dbo.Admin', 'U') IS NULL
CREATE TABLE [dbo].[Admin] (
    [UserID] INT IDENTITY(1, 1) NOT NULL,
    [Email] VARCHAR(100) NOT NULL,
    [Password] VARCHAR(100) NOT NULL,
    [FullName] VARCHAR(100) NOT NULL,
    [CreatedAt] DATETIME NULL CONSTRAINT [DF__Admin__CreatedAt__4CA06362] DEFAULT (getdate()),
    CONSTRAINT [PK__Admin__1788CCAC4AB81AF0] PRIMARY KEY CLUSTERED ([UserID])
);
GO

IF OBJECT_ID('dbo.Appointments', 'U') IS NULL
CREATE TABLE [dbo].[Appointments] (
    [ID] INT IDENTITY(1, 1) NOT NULL,
    [PatientID] INT NOT NULL,
    [DoctorID] INT NOT NULL,
    [TokenNo] NVARCHAR(20) NOT NULL,
    [AppointDate] DATE NOT NULL CONSTRAINT [DF__Appointme__Appoi__3E52440B] DEFAULT (getdate()),
    [AppointTime] TIME(7) NULL,
    [Status] NVARCHAR(50) NULL,
    [UserID] INT NULL,
    [PaidAmount] DECIMAL(10, 2) NULL,
    [DueAmount] DECIMAL(10, 2) NULL,
    [TotalFee] DECIMAL(10, 2) NULL,
    [PaymentId] NVARCHAR(100) NULL,
    [PaymentStatus] NVARCHAR(50) NULL,
    [BookedByID] INT NULL,
    [BookedByRole] NVARCHAR(20) NULL,
    [NotifiedAt] DATETIME NULL,
    [DeletedAt] DATETIME NULL,
    [ReminderSentAt] DATETIME NULL,
    [RefundedAt] DATETIME NULL,
    [RefundedBy] NVARCHAR(100) NULL,
    [RefundNote] NVARCHAR(200) NULL,
    CONSTRAINT [PK__Appointm__3214EC273C69FB99] PRIMARY KEY CLUSTERED ([ID])
);
GO

IF OBJECT_ID('dbo.Departments', 'U') IS NULL
CREATE TABLE [dbo].[Departments] (
    [DepartmentID] INT IDENTITY(1, 1) NOT NULL,
    [Department] NVARCHAR(100) NULL,
    [DepartmentHead] NVARCHAR(100) NULL,
    [Description] NVARCHAR(255) NULL,
    [Date] DATE NULL,
    [Status] NVARCHAR(50) NULL,
    CONSTRAINT [PK__Departme__B2079BCD6D0D32F4] PRIMARY KEY CLUSTERED ([DepartmentID])
);
GO

IF OBJECT_ID('dbo.DoctorLeave', 'U') IS NULL
CREATE TABLE [dbo].[DoctorLeave] (
    [LeaveID] INT IDENTITY(1, 1) NOT NULL,
    [DoctorID] INT NOT NULL,
    [LeaveDate] DATE NOT NULL,
    [Reason] NVARCHAR(200) NULL,
    [CreatedAt] DATETIME NOT NULL CONSTRAINT [DF_DoctorLeave_CreatedAt] DEFAULT (getdate()),
    CONSTRAINT [PK_DoctorLeave] PRIMARY KEY CLUSTERED ([LeaveID]),
    CONSTRAINT [UQ_DoctorLeave] UNIQUE NONCLUSTERED ([DoctorID], [LeaveDate])
);
GO

IF OBJECT_ID('dbo.DoctorProfile', 'U') IS NULL
CREATE TABLE [dbo].[DoctorProfile] (
    [ProfileID] INT IDENTITY(1, 1) NOT NULL,
    [DoctorID] INT NULL,
    [AboutMe] NVARCHAR(MAX) NULL,
    [Designation] NVARCHAR(100) NULL,
    [Location] NVARCHAR(100) NULL,
    [Year_of_Completion] NVARCHAR(200) NULL,
    [Experience] NVARCHAR(50) NULL,
    [Hospital] NVARCHAR(100) NULL,
    [Institute] NVARCHAR(100) NULL,
    CONSTRAINT [PK__DoctorPr__290C888475A278F5] PRIMARY KEY CLUSTERED ([ProfileID])
);
GO

IF OBJECT_ID('dbo.Doctors', 'U') IS NULL
CREATE TABLE [dbo].[Doctors] (
    [DoctorID] INT IDENTITY(1, 1) NOT NULL,
    [Name] NVARCHAR(100) NOT NULL,
    [Specialization] NVARCHAR(100) NOT NULL,
    [Degree] NVARCHAR(100) NOT NULL,
    [Mobile] NVARCHAR(20) NOT NULL,
    [Email] NVARCHAR(100) NOT NULL,
    [Password] NVARCHAR(255) NOT NULL,
    [Charges] DECIMAL(10, 2) NOT NULL,
    [JoinDate] DATE NOT NULL,
    [ImageUrl] NVARCHAR(255) NULL,
    [CreatedAt] DATETIME NULL CONSTRAINT [DF__Doctors__Created__1920BF5C] DEFAULT (getdate()),
    [Gender] VARCHAR(10) NULL,
    [IsDeleted] BIT NOT NULL CONSTRAINT [DF__Doctors__isdelet__20C1E124] DEFAULT ((0)),
    [DepartmentID] INT NULL,
    CONSTRAINT [PK__Doctors__2DC00EDF173876EA] PRIMARY KEY CLUSTERED ([DoctorID])
);
GO

IF OBJECT_ID('dbo.DoctorSchedule', 'U') IS NULL
CREATE TABLE [dbo].[DoctorSchedule] (
    [ScheduleID] INT IDENTITY(1, 1) NOT NULL,
    [DoctorID] INT NOT NULL,
    [ScheduleDate] DATE NOT NULL,
    [StartTime] TIME(7) NULL,
    [EndTime] TIME(7) NOT NULL,
    [SlotDuration] INT NOT NULL,
    [MaxTokens] INT NOT NULL,
    CONSTRAINT [PK__DoctorSc__9C8A5B690E6E26BF] PRIMARY KEY CLUSTERED ([ScheduleID])
);
GO

IF OBJECT_ID('dbo.DoctorWorkingHours', 'U') IS NULL
CREATE TABLE [dbo].[DoctorWorkingHours] (
    [DoctorID] INT NOT NULL,
    [DayOfWeek] TINYINT NOT NULL,
    [IsWorking] BIT NOT NULL CONSTRAINT [DF_DoctorWorkingHours_IsWorking] DEFAULT ((0)),
    [StartTime] TIME(7) NULL,
    [EndTime] TIME(7) NULL,
    [SlotDuration] INT NULL,
    [MaxPatients] INT NULL,
    [BreakStart] TIME(7) NULL,
    [BreakEnd] TIME(7) NULL,
    CONSTRAINT [PK_DoctorWorkingHours] PRIMARY KEY CLUSTERED ([DoctorID], [DayOfWeek]),
    CONSTRAINT [CK_DoctorWorkingHours_Day] CHECK ([DayOfWeek]>=(0) AND [DayOfWeek]<=(6)),
    CONSTRAINT [CK_DoctorWorkingHours_Times] CHECK ([IsWorking]=(0) OR [StartTime]<[EndTime] AND [SlotDuration]>(0) AND [MaxPatients]>(0)),
    CONSTRAINT [CK_DoctorWorkingHours_Break] CHECK ([BreakStart] IS NULL AND [BreakEnd] IS NULL OR [BreakStart]<[BreakEnd])
);
GO

IF OBJECT_ID('dbo.PatientHealth', 'U') IS NULL
CREATE TABLE [dbo].[PatientHealth] (
    [PatientID] INT NOT NULL,
    [BloodGroup] NVARCHAR(5) NULL,
    [Allergies] NVARCHAR(500) NULL,
    [Conditions] NVARCHAR(500) NULL,
    [CurrentMedicines] NVARCHAR(500) NULL,
    [PastSurgeries] NVARCHAR(500) NULL,
    [UpdatedBy] NVARCHAR(100) NULL,
    [UpdatedAt] DATETIME NULL,
    CONSTRAINT [PK__PatientH__970EC3464610BAB4] PRIMARY KEY CLUSTERED ([PatientID])
);
GO

IF OBJECT_ID('dbo.Patients', 'U') IS NULL
CREATE TABLE [dbo].[Patients] (
    [PatientID] INT IDENTITY(1, 1) NOT NULL,
    [PatientName] NVARCHAR(100) NOT NULL,
    [DateOfBirth] DATE NOT NULL,
    [Age] INT NOT NULL,
    [Gender] NVARCHAR(10) NOT NULL,
    [IsDeleted] BIT NULL CONSTRAINT [DF__Patients__IsDele__25869641] DEFAULT ((0)),
    [UserID] INT NULL,
    [Status] NVARCHAR(50) NULL,
    [DoctorID] INT NULL,
    [Mobile] NVARCHAR(20) NULL,
    [Email] NVARCHAR(100) NULL,
    [Address] NVARCHAR(255) NULL,
    [PatientCode] AS ('DC-'+case when [PatientID]<(100000) then right('0000'+CONVERT([varchar](10),[PatientID],0),(5)) else CONVERT([varchar](10),[PatientID],0) end) PERSISTED,
    [MergedIntoPatientID] INT NULL,
    [MergedAt] DATETIME NULL,
    [MergedBy] NVARCHAR(100) NULL,
    CONSTRAINT [PK__Patients__970EC346239E4DCF] PRIMARY KEY CLUSTERED ([PatientID])
);
GO

IF OBJECT_ID('dbo.PrescriptionItems', 'U') IS NULL
CREATE TABLE [dbo].[PrescriptionItems] (
    [ID] INT IDENTITY(1, 1) NOT NULL,
    [AppointmentID] INT NOT NULL,
    [SortOrder] INT NOT NULL CONSTRAINT [DF__Prescript__SortO__76619304] DEFAULT ((0)),
    [Medicine] NVARCHAR(150) NOT NULL,
    [Dose] NVARCHAR(60) NULL,
    [Frequency] NVARCHAR(60) NULL,
    [Days] INT NULL,
    [Instructions] NVARCHAR(200) NULL,
    CONSTRAINT [PK__Prescrip__3214EC27EED15F1D] PRIMARY KEY CLUSTERED ([ID])
);
GO

IF OBJECT_ID('dbo.ReceptionistDoctorAssignments', 'U') IS NULL
CREATE TABLE [dbo].[ReceptionistDoctorAssignments] (
    [AssignmentID] INT IDENTITY(1, 1) NOT NULL,
    [ReceptionistID] INT NULL,
    [DoctorID] INT NULL,
    CONSTRAINT [PK__Receptio__32499E571BFD2C07] PRIMARY KEY CLUSTERED ([AssignmentID])
);
GO

IF OBJECT_ID('dbo.Receptionists', 'U') IS NULL
CREATE TABLE [dbo].[Receptionists] (
    [ReceptionistID] INT IDENTITY(1, 1) NOT NULL,
    [Name] NVARCHAR(50) NULL,
    [Mobile] NVARCHAR(15) NULL,
    [Email] NVARCHAR(50) NULL,
    [Status] NVARCHAR(50) NULL CONSTRAINT [DF__Reception__Statu__145C0A3F] DEFAULT ('Active'),
    [Password] NVARCHAR(50) NULL,
    CONSTRAINT [PK__Receptio__0F8C20481273C1CD] PRIMARY KEY CLUSTERED ([ReceptionistID])
);
GO

IF OBJECT_ID('dbo.Users', 'U') IS NULL
CREATE TABLE [dbo].[Users] (
    [UserID] INT IDENTITY(1, 1) NOT NULL,
    [Email] VARCHAR(100) NULL,
    [Password] VARCHAR(100) NULL,
    [Name] VARCHAR(100) NULL,
    [Mobile] VARCHAR(20) NULL,
    [Address] VARCHAR(250) NULL,
    [CreatedAt] DATETIME NOT NULL CONSTRAINT [DF__Users__CreatedAt__5AEE82B9] DEFAULT (getdate()),
    CONSTRAINT [PK__Users__1788CCAC59063A47] PRIMARY KEY CLUSTERED ([UserID])
);
GO

IF OBJECT_ID('dbo.VisitNotes', 'U') IS NULL
CREATE TABLE [dbo].[VisitNotes] (
    [AppointmentID] INT NOT NULL,
    [BloodPressure] NVARCHAR(15) NULL,
    [Pulse] INT NULL,
    [TemperatureF] DECIMAL(4, 1) NULL,
    [WeightKg] DECIMAL(5, 1) NULL,
    [SpO2] INT NULL,
    [Complaint] NVARCHAR(1000) NULL,
    [Diagnosis] NVARCHAR(1000) NULL,
    [Advice] NVARCHAR(2000) NULL,
    [FollowUpDate] DATE NULL,
    [VitalsBy] NVARCHAR(100) NULL,
    [VitalsAt] DATETIME NULL,
    [NotesByDoctorID] INT NULL,
    [NotesAt] DATETIME NULL,
    [FollowUpReminderSentAt] DATETIME NULL,
    [FollowUpClosedAt] DATETIME NULL,
    [FollowUpClosedBy] NVARCHAR(100) NULL,
    CONSTRAINT [PK__VisitNot__8ECDFCA2700D7299] PRIMARY KEY CLUSTERED ([AppointmentID])
);
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'UX_Patients_PatientCode' AND object_id = OBJECT_ID('dbo.Patients'))
CREATE UNIQUE NONCLUSTERED INDEX [UX_Patients_PatientCode] ON [dbo].[Patients] ([PatientCode]);
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'UX_Appointments_ActiveSlot' AND object_id = OBJECT_ID('dbo.Appointments'))
CREATE UNIQUE NONCLUSTERED INDEX [UX_Appointments_ActiveSlot] ON [dbo].[Appointments] ([DoctorID], [AppointDate], [AppointTime]) WHERE ([Status]<>'Cancelled' AND [AppointTime] IS NOT NULL);
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_PrescriptionItems_Appointment' AND object_id = OBJECT_ID('dbo.PrescriptionItems'))
CREATE NONCLUSTERED INDEX [IX_PrescriptionItems_Appointment] ON [dbo].[PrescriptionItems] ([AppointmentID], [SortOrder]);
GO

IF OBJECT_ID('dbo.FK__DoctorSch__Docto__10566F31', 'F') IS NULL
ALTER TABLE [dbo].[DoctorSchedule] ADD CONSTRAINT [FK__DoctorSch__Docto__10566F31] FOREIGN KEY ([DoctorID]) REFERENCES [dbo].[Doctors] ([DoctorID]);
GO

IF OBJECT_ID('dbo.FK__Reception__Recep__1DE57479', 'F') IS NULL
ALTER TABLE [dbo].[ReceptionistDoctorAssignments] ADD CONSTRAINT [FK__Reception__Recep__1DE57479] FOREIGN KEY ([ReceptionistID]) REFERENCES [dbo].[Receptionists] ([ReceptionistID]);
GO

IF OBJECT_ID('dbo.FK__Reception__Docto__1ED998B2', 'F') IS NULL
ALTER TABLE [dbo].[ReceptionistDoctorAssignments] ADD CONSTRAINT [FK__Reception__Docto__1ED998B2] FOREIGN KEY ([DoctorID]) REFERENCES [dbo].[Doctors] ([DoctorID]);
GO

IF OBJECT_ID('dbo.FK__Appointme__Patie__412EB0B6', 'F') IS NULL
ALTER TABLE [dbo].[Appointments] ADD CONSTRAINT [FK__Appointme__Patie__412EB0B6] FOREIGN KEY ([PatientID]) REFERENCES [dbo].[Patients] ([PatientID]);
GO

IF OBJECT_ID('dbo.FK__Appointme__Docto__4222D4EF', 'F') IS NULL
ALTER TABLE [dbo].[Appointments] ADD CONSTRAINT [FK__Appointme__Docto__4222D4EF] FOREIGN KEY ([DoctorID]) REFERENCES [dbo].[Doctors] ([DoctorID]);
GO

IF OBJECT_ID('dbo.FK_DoctorWorkingHours_Doctors', 'F') IS NULL
ALTER TABLE [dbo].[DoctorWorkingHours] ADD CONSTRAINT [FK_DoctorWorkingHours_Doctors] FOREIGN KEY ([DoctorID]) REFERENCES [dbo].[Doctors] ([DoctorID]);
GO

IF OBJECT_ID('dbo.FK_DoctorLeave_Doctors', 'F') IS NULL
ALTER TABLE [dbo].[DoctorLeave] ADD CONSTRAINT [FK_DoctorLeave_Doctors] FOREIGN KEY ([DoctorID]) REFERENCES [dbo].[Doctors] ([DoctorID]);
GO

IF OBJECT_ID('dbo.FK_Patients_Users', 'F') IS NULL
ALTER TABLE [dbo].[Patients] ADD CONSTRAINT [FK_Patients_Users] FOREIGN KEY ([UserID]) REFERENCES [dbo].[Users] ([UserID]);
GO

IF OBJECT_ID('dbo.FK_VisitNotes_Appointments', 'F') IS NULL
ALTER TABLE [dbo].[VisitNotes] ADD CONSTRAINT [FK_VisitNotes_Appointments] FOREIGN KEY ([AppointmentID]) REFERENCES [dbo].[Appointments] ([ID]);
GO

IF OBJECT_ID('dbo.FK_PrescriptionItems_Appointments', 'F') IS NULL
ALTER TABLE [dbo].[PrescriptionItems] ADD CONSTRAINT [FK_PrescriptionItems_Appointments] FOREIGN KEY ([AppointmentID]) REFERENCES [dbo].[Appointments] ([ID]);
GO

IF OBJECT_ID('dbo.FK__DoctorPro__Docto__778AC167', 'F') IS NULL
ALTER TABLE [dbo].[DoctorProfile] ADD CONSTRAINT [FK__DoctorPro__Docto__778AC167] FOREIGN KEY ([DoctorID]) REFERENCES [dbo].[Doctors] ([DoctorID]) ON DELETE CASCADE;
GO

IF OBJECT_ID('dbo.FK__Doctors__Departm__787EE5A0', 'F') IS NULL
ALTER TABLE [dbo].[Doctors] ADD CONSTRAINT [FK__Doctors__Departm__787EE5A0] FOREIGN KEY ([DepartmentID]) REFERENCES [dbo].[Departments] ([DepartmentID]);
GO

IF OBJECT_ID('dbo.FK_PatientHealth_Patients', 'F') IS NULL
ALTER TABLE [dbo].[PatientHealth] ADD CONSTRAINT [FK_PatientHealth_Patients] FOREIGN KEY ([PatientID]) REFERENCES [dbo].[Patients] ([PatientID]);
GO

IF OBJECT_ID('dbo.FK_Appointments_Users', 'F') IS NULL
ALTER TABLE [dbo].[Appointments] ADD CONSTRAINT [FK_Appointments_Users] FOREIGN KEY ([UserID]) REFERENCES [dbo].[Users] ([UserID]) ON DELETE CASCADE;
GO

IF OBJECT_ID('dbo.FK_Patients_Doctors', 'F') IS NULL
ALTER TABLE [dbo].[Patients] ADD CONSTRAINT [FK_Patients_Doctors] FOREIGN KEY ([DoctorID]) REFERENCES [dbo].[Doctors] ([DoctorID]);
GO

IF OBJECT_ID('dbo.sp_GetAdminName') IS NULL EXEC('CREATE PROCEDURE [dbo].[sp_GetAdminName] AS ' + 'RETURN 0');
GO
ALTER PROCEDURE sp_GetAdminName
    @Email NVARCHAR(100)
AS
BEGIN
    SET NOCOUNT ON;
    SELECT FullName FROM Admin WHERE Email = @Email
END
GO

IF OBJECT_ID('dbo.sp_GetDoctorByEmail') IS NULL EXEC('CREATE PROCEDURE [dbo].[sp_GetDoctorByEmail] AS ' + 'RETURN 0');
GO
ALTER PROCEDURE sp_GetDoctorByEmail
    @Email    NVARCHAR(100),
    @Password NVARCHAR(255)
AS
BEGIN
    SET NOCOUNT ON;
    SELECT DoctorID, Name FROM Doctors
    WHERE Email = @Email AND Password = @Password AND IsDeleted = 0
END
GO

IF OBJECT_ID('dbo.sp_GetPatientByEmail') IS NULL EXEC('CREATE PROCEDURE [dbo].[sp_GetPatientByEmail] AS ' + 'RETURN 0');
GO
ALTER PROCEDURE sp_GetPatientByEmail
    @Email NVARCHAR(100)
AS
BEGIN
    SET NOCOUNT ON;
    SELECT UserID, Name FROM Users WHERE Email = @Email
END
GO

IF OBJECT_ID('dbo.sp_GetReceptionistByEmail') IS NULL EXEC('CREATE PROCEDURE [dbo].[sp_GetReceptionistByEmail] AS ' + 'RETURN 0');
GO
ALTER PROCEDURE sp_GetReceptionistByEmail
    @Email    NVARCHAR(100),
    @Password NVARCHAR(255)
AS
BEGIN
    SET NOCOUNT ON;
    SELECT ReceptionistID, Name FROM Receptionists
    WHERE Email = @Email AND Password = @Password
END
GO

IF OBJECT_ID('dbo.sp_LoginUser') IS NULL EXEC('CREATE PROCEDURE [dbo].[sp_LoginUser] AS ' + 'RETURN 0');
GO
ALTER PROCEDURE sp_LoginUser
    @Email    NVARCHAR(100),
    @Password NVARCHAR(255)
AS
BEGIN
    SET NOCOUNT ON;

    SELECT TOP 1 UserID, 'Admin' AS UserType
    FROM Admin
    WHERE Email = @Email AND Password = @Password

    UNION ALL

    SELECT TOP 1 DoctorID, 'Doctor'
    FROM Doctors
    WHERE Email = @Email AND Password = @Password AND IsDeleted = 0

    UNION ALL

    SELECT TOP 1 ReceptionistID, 'Receptionist'
    FROM Receptionists
    WHERE Email = @Email AND Password = @Password

    UNION ALL

    SELECT TOP 1 UserID, 'User'
    FROM Users
    WHERE Email = @Email AND Password = @Password
END
GO
