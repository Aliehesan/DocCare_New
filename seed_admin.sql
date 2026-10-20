-- First admin account for a NEW, empty DocCare database (run after create_database.sql).
-- 1. Change the three values below.  2. Run:
--      sqlcmd -S "(LocalDb)\MSSQLLocalDB" -E -I -d "<full path to DocCare.mdf>" -i seed_admin.sql
-- 3. Log in with that email and password, then add doctors and receptionists from the admin panel.
-- The password is stored the way the login page checks it (SHA-256, base64). Safe to run more than once.
SET NOCOUNT ON;

DECLARE @Email    VARCHAR(100) = 'admin@example.com';
DECLARE @FullName VARCHAR(100) = 'Clinic Admin';
DECLARE @Password VARCHAR(100) = 'ChangeMe@123';   -- change this, and change it again after the first login

DECLARE @Bytes VARBINARY(32) = HASHBYTES('SHA2_256', @Password);
DECLARE @Hash  VARCHAR(100) = CAST(N'' AS XML).value('xs:base64Binary(sql:variable("@Bytes"))', 'VARCHAR(100)');

IF EXISTS (SELECT 1 FROM dbo.Admin WHERE Email = @Email)
    PRINT 'An admin with this email already exists; nothing changed.';
ELSE
BEGIN
    INSERT INTO dbo.Admin (Email, Password, FullName, CreatedAt) VALUES (@Email, @Hash, @FullName, GETDATE());
    PRINT 'Admin account created for ' + @Email + '.';
END
GO
