# DocCare Clinic

Clinic appointment system: ASP.NET Web Forms (C#, .NET Framework 4.8) with a SQL Server LocalDB database.

Patients book online (any date in the next 30 days, pay the advance with Razorpay); the front desk books walk-ins.
Doctors set weekly working hours, a lunch break and leave days, and the free times are worked out from those.
Every patient has a permanent Patient ID (e.g. `DC-00029`) for quick search, and a medical record:
vitals, the doctor's consultation notes and prescription for each visit (printable), and a health summary with allergies.
Follow-ups the doctor advises are shown to the patient with a booking link, emailed 2 days ahead, and listed for reception.

## Roles

| Role | Can |
|---|---|
| Admin | Dashboard, doctors, receptionists, departments, all appointments, Appointment Report |
| Receptionist | Walk-in booking, appointments, patients, doctors' hours and leave, report |
| Doctor | Own appointments and patients, own working hours and leave |
| Patient | Book, reschedule or cancel online, family members, appointment history |

## Run it locally

1. Open `DocCare Clinic.sln` in Visual Studio 2022.
2. **Secrets:** copy `DocCare Clinic/Secrets.example.config` to `DocCare Clinic/Secrets.config` and fill in
   the Gmail app password and the Razorpay keys. `Secrets.config` is ignored by git.
3. **Database:** the database file is not in the repository (it holds patient data). Put `DocCare.mdf` in
   `DocCare Clinic/App_Data/`, then run these scripts against it, in order (each one is safe to run again):
   - `add_booked_by_columns.sql`
   - `add_notified_at_column.sql`
   - `add_appointment_deleted_at_column.sql`
   - `add_working_hours_and_leave.sql`
   - `normalize_doctor_names.sql`
   - `add_reminder_sent_at_column.sql`
   - `add_patient_code.sql`
   - `add_medical_records.sql`

   ```
   sqlcmd -S "(LocalDb)\MSSQLLocalDB" -E -I -d "<full path to DocCare.mdf>" -i add_patient_code.sql
   ```
4. Run with IIS Express (F5).

Optional: add `<add key="SiteUrl" value="https://your-clinic-address" />` to `Secrets.config` (or Web.config
appSettings) so links in reminder emails point to the right address. Without it the site uses the
address of the first page it served.

## Project layout

- `DocCare Clinic/Features/` - pages, one folder per area (Accounts, Admin, Appointments, Doctors, Patients, ...)
- `DocCare Clinic/webConfig/` - shared services: slots and working hours (`SlotService`), status rules
  (`AppointmentStatus`), emails (`EmailService`, `AppointmentNotifier`, `ReminderService`), Patient IDs (`PatientCode`)
- `DocCare Clinic/Global.asax` - role-based access to every page
