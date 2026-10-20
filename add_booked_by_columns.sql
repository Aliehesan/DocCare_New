-- Records which staff member booked a walk-in appointment at the front desk.
-- BookedByRole: 'Receptionist' -> Receptionists.ReceptionistID
--               'SuperAdmin'   -> Admin.UserID
--               'Doctor'       -> Doctors.DoctorID
-- Safe to run more than once.

IF COL_LENGTH('dbo.Appointments', 'BookedByID') IS NULL
    ALTER TABLE dbo.Appointments ADD BookedByID INT NULL;

IF COL_LENGTH('dbo.Appointments', 'BookedByRole') IS NULL
    ALTER TABLE dbo.Appointments ADD BookedByRole NVARCHAR(20) NULL;
