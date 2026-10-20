-- "Delete" on the staff Appointments list only hides an appointment; the record is kept
-- for the Appointment Report and the patient's history. NULL = shown in the list.
-- Safe to run more than once.

IF COL_LENGTH('dbo.Appointments', 'DeletedAt') IS NULL
    ALTER TABLE dbo.Appointments ADD DeletedAt DATETIME NULL;
