-- When the front desk last emailed the patient their appointment details
-- (Appointments list / Doctor's Appointments -> "Notify Patient"). NULL = not notified yet.
-- Safe to run more than once.

IF COL_LENGTH('dbo.Appointments', 'NotifiedAt') IS NULL
    ALTER TABLE dbo.Appointments ADD NotifiedAt DATETIME NULL;
