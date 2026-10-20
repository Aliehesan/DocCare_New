-- Refunds of the online advance on cancelled appointments: when, by whom, and a note (e.g. UPI reference).
-- A refund is due when an appointment is Cancelled, an advance was paid, and RefundedAt is still empty.
-- Safe to run more than once.
SET QUOTED_IDENTIFIER ON;
SET ANSI_NULLS ON;

IF COL_LENGTH('dbo.Appointments', 'RefundedAt') IS NULL
    ALTER TABLE dbo.Appointments ADD RefundedAt DATETIME NULL, RefundedBy NVARCHAR(100) NULL, RefundNote NVARCHAR(200) NULL;
GO
