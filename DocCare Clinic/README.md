# DocCare Clinic project layout

ASP.NET Web Forms application targeting .NET Framework 4.0. Each page and its code-behind/designer files are grouped together by feature. Update the `.csproj` whenever files are added or moved.

- `Features/Accounts` ? login, registration, profiles, and password flows
- `Features/Admin` ? admin dashboard and master page
- `Features/Appointments` ? appointment booking, management, and reports
- `Features/Departments` ? department management
- `Features/Doctors` ? doctor pages, profiles, dashboard, and master page
- `Features/Patients` ? patient pages and profiles
- `Features/Receptionists` ? receptionist pages and master page
- `Features/Scheduling` ? schedules
- `Features/Shared` ? shared master page and seed page
- `Features/Site` ? public site pages
- `api/` ? payment HTTP handlers
- `webConfig/` ? payment and email support services
- `App_Data/` ? LocalDB database
- `assets/`, `assets_web/` ? static files and templates

Open `DocCare Clinic.sln` in Visual Studio. The default page and LocalDB connection point to `Features/Site/Default.aspx` and `App_Data/DocCare.mdf`.
