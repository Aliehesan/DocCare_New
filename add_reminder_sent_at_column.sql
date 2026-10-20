-- When the day-before reminder email was sent (ReminderService). NULL = not reminded yet.
-- Safe to run more than once.

IF COL_LENGTH('dbo.Appointments', 'ReminderSentAt') IS NULL
    ALTER TABLE dbo.Appointments ADD ReminderSentAt DATETIME NULL;
