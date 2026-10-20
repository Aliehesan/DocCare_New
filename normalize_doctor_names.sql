-- Doctor names are stored without a title; pages and emails add "Dr. " when showing them.
-- Removes a leading "Dr." / "Dr " so names like "Dr. Sara Khan" no longer show as "Dr. Dr. Sara Khan".
-- (Names such as "Drake" are not touched: a dot or space must follow "Dr".) Safe to run more than once.

UPDATE dbo.Doctors SET Name = LTRIM(STUFF(Name, 1, 3, '')) WHERE Name LIKE 'Dr.%';
UPDATE dbo.Doctors SET Name = LTRIM(STUFF(Name, 1, 3, '')) WHERE Name LIKE 'Dr %';
