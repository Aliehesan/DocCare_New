using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Globalization;

namespace DocCare_Clinic.Config
{
    /// <summary>
    /// Single place that decides which appointment slots a doctor has free on a date.
    /// Availability comes from the doctor's weekly hours (DoctorWorkingHours) minus their
    /// leave days (DoctorLeave). Used by online booking, walk-in booking, the doctor list,
    /// and the doctor's schedule pages so they always agree.
    /// </summary>
    public static class SlotService
    {
        // Online patients need time to travel to the clinic (applies to today only).
        public const int OnlineBufferMinutes = 45;

        // Walk-in patients are already at the desk; only time to finish registration.
        public const int WalkInBufferMinutes = 5;

        // How far ahead patients can book.
        public const int BookingWindowDays = 30;

        static readonly CultureInfo Invariant = CultureInfo.InvariantCulture;

        /// <summary>A doctor's hours on one date (after weekly hours and leave are applied).</summary>
        public class DayHours
        {
            public bool Working;
            public TimeSpan Start, End;
            public TimeSpan? BreakStart, BreakEnd;   // optional (e.g. lunch)
            public int SlotDuration;
            public string NotWorkingReason;   // set when Working is false
        }

        /// <summary>
        /// The appointment times in a working day: every slot that starts at start and finishes by end.
        /// The number of these is how many patients the day can take (no separate maximum to keep in sync).
        /// </summary>
        public static List<TimeSpan> SlotTimes(TimeSpan start, TimeSpan end, int slotMinutes)
        {
            return SlotTimes(start, end, slotMinutes, null, null);
        }

        /// <summary>
        /// As above, with an optional break: a slot that would overlap the break is skipped and
        /// the next slot starts when the break ends (09:00-13:00, 30 min, break 11:00-11:30 ->
        /// 09:00 ... 10:30, 11:30, 12:00, 12:30).
        /// </summary>
        public static List<TimeSpan> SlotTimes(TimeSpan start, TimeSpan end, int slotMinutes, TimeSpan? breakStart, TimeSpan? breakEnd)
        {
            List<TimeSpan> slots = new List<TimeSpan>();
            if (slotMinutes <= 0) return slots;
            TimeSpan length = TimeSpan.FromMinutes(slotMinutes);
            bool hasBreak = breakStart.HasValue && breakEnd.HasValue && breakStart.Value < breakEnd.Value;
            TimeSpan slot = start;
            while (slot + length <= end)
            {
                if (hasBreak && slot < breakEnd.Value && slot + length > breakStart.Value)
                {
                    slot = breakEnd.Value;   // resume after the break
                    continue;
                }
                slots.Add(slot);
                slot += length;
            }
            return slots;
        }

        /// <summary>How many patients fit in a working day (whole slots between start and end, outside the break).</summary>
        public static int Capacity(TimeSpan start, TimeSpan end, int slotMinutes)
        {
            return SlotTimes(start, end, slotMinutes).Count;
        }

        public static int Capacity(TimeSpan start, TimeSpan end, int slotMinutes, TimeSpan? breakStart, TimeSpan? breakEnd)
        {
            return SlotTimes(start, end, slotMinutes, breakStart, breakEnd).Count;
        }

        /// <summary>An upcoming booking, for the warning when a doctor's hours change.</summary>
        public class Booking
        {
            public int ID;
            public DateTime Date;
            public TimeSpan Time;
            public string Patient, Token;

            public override string ToString()
            {
                return Patient + " - " + FormatDate(Date) + ", " + Format(Time) + (string.IsNullOrEmpty(Token) ? "" : " (" + Token + ")");
            }
        }

        /// <summary>
        /// Upcoming Pending/Confirmed bookings that are not on one of the doctor's slot times: in a break,
        /// outside the hours, on a day off, or between slots after the slot length changed.
        /// slotsForWeekday(0 = Sunday) gives that weekday's slot times (empty for a day off), so the
        /// caller can check hours that are not saved yet. Leave days are skipped (marking leave cancels them).
        /// </summary>
        public static List<Booking> BookingsOffSlots(SqlConnection con, int doctorId, Func<int, List<TimeSpan>> slotsForWeekday)
        {
            List<Booking> result = new List<Booking>();
            Dictionary<int, List<TimeSpan>> week = new Dictionary<int, List<TimeSpan>>();
            using (SqlCommand cmd = new SqlCommand(@"
                SELECT a.ID, a.AppointDate, a.AppointTime, p.PatientName, p.PatientCode, a.TokenNo FROM Appointments a
                LEFT JOIN Patients p ON p.PatientID = a.PatientID
                WHERE a.DoctorID = @DoctorID AND a.Status IN ('Pending', 'Confirmed') AND a.AppointTime IS NOT NULL
                  AND (a.AppointDate > @Today OR (a.AppointDate = @Today AND a.AppointTime >= @Now))
                  AND NOT EXISTS (SELECT 1 FROM DoctorLeave l WHERE l.DoctorID = a.DoctorID AND l.LeaveDate = a.AppointDate)
                ORDER BY a.AppointDate, a.AppointTime", con))
            {
                cmd.Parameters.AddWithValue("@DoctorID", doctorId);
                cmd.Parameters.AddWithValue("@Today", DateTime.Today);
                cmd.Parameters.AddWithValue("@Now", DateTime.Now.TimeOfDay);
                using (SqlDataReader r = cmd.ExecuteReader())
                    while (r.Read())
                    {
                        DateTime date = Convert.ToDateTime(r["AppointDate"]).Date;
                        TimeSpan time = (TimeSpan)r["AppointTime"];
                        int weekday = (int)date.DayOfWeek;
                        if (!week.ContainsKey(weekday)) week[weekday] = slotsForWeekday(weekday) ?? new List<TimeSpan>();
                        if (week[weekday].Contains(time)) continue;
                        result.Add(new Booking
                        {
                            ID = Convert.ToInt32(r["ID"]), Date = date, Time = time,
                            Patient = r["PatientName"] == DBNull.Value ? "Patient" : r["PatientName"] + " [" + r["PatientCode"] + "]",
                            Token = r["TokenNo"] == DBNull.Value ? "" : r["TokenNo"].ToString()
                        });
                    }
            }
            return result;
        }

        /// <summary>The doctor's hours on a date, or Working = false with a reason (leave, day off, no hours set).</summary>
        public static DayHours GetDayHours(SqlConnection con, int doctorId, DateTime date)
        {
            DayHours h = new DayHours();
            using (SqlCommand cmd = new SqlCommand(@"
                SELECT d.IsDeleted, w.IsWorking, w.StartTime, w.EndTime, w.SlotDuration, w.BreakStart, w.BreakEnd,
                       l.LeaveID, l.Reason,
                       (SELECT COUNT(*) FROM DoctorWorkingHours x WHERE x.DoctorID = d.DoctorID) AS HoursRows
                FROM Doctors d
                LEFT JOIN DoctorWorkingHours w ON w.DoctorID = d.DoctorID AND w.DayOfWeek = @Day
                LEFT JOIN DoctorLeave l ON l.DoctorID = d.DoctorID AND l.LeaveDate = @Date
                WHERE d.DoctorID = @DoctorID", con))
            {
                cmd.Parameters.AddWithValue("@DoctorID", doctorId);
                cmd.Parameters.AddWithValue("@Date", date.Date);
                cmd.Parameters.AddWithValue("@Day", (int)date.DayOfWeek);
                using (SqlDataReader r = cmd.ExecuteReader())
                {
                    // (no apostrophes in reasons: pages embed them in single-quoted JS strings)
                    if (!r.Read() || (r["IsDeleted"] != DBNull.Value && Convert.ToBoolean(r["IsDeleted"])))
                    {
                        h.NotWorkingReason = "This doctor is not available for appointments.";
                        return h;
                    }
                    if (r["LeaveID"] != DBNull.Value)
                    {
                        string why = r["Reason"] == DBNull.Value ? "" : r["Reason"].ToString().Trim();
                        h.NotWorkingReason = "The doctor is not available on " + FormatDate(date) + (why == "" ? "." : " (" + why.Replace("'", "") + ").");
                        return h;
                    }
                    if (Convert.ToInt32(r["HoursRows"]) == 0)
                    {
                        h.NotWorkingReason = "This doctor has not set working hours yet.";
                        return h;
                    }
                    if (r["IsWorking"] == DBNull.Value || !Convert.ToBoolean(r["IsWorking"]))
                    {
                        h.NotWorkingReason = "The doctor does not work on " + date.ToString("dddd", Invariant) + "s.";
                        return h;
                    }
                    h.Working = true;
                    h.Start = (TimeSpan)r["StartTime"];
                    h.End = (TimeSpan)r["EndTime"];
                    h.SlotDuration = Convert.ToInt32(r["SlotDuration"]);
                    h.BreakStart = r["BreakStart"] == DBNull.Value ? (TimeSpan?)null : (TimeSpan)r["BreakStart"];
                    h.BreakEnd = r["BreakEnd"] == DBNull.Value ? (TimeSpan?)null : (TimeSpan)r["BreakEnd"];
                }
            }
            return h;
        }

        /// <summary>
        /// Free slots on a date, in time order, or an empty list with a reason the patient can read.
        /// A slot is free when nobody holds it (cancelled appointments free their slot) and, today only,
        /// it starts at least bufferMinutes from now. A day is full when every slot is taken.
        /// Dates must be within today .. today + BookingWindowDays.
        /// </summary>
        public static List<TimeSpan> GetFreeSlots(SqlConnection con, int doctorId, DateTime date, int bufferMinutes, out string reason)
        {
            reason = null;
            List<TimeSpan> free = new List<TimeSpan>();
            date = date.Date;
            DateTime today = DateTime.Today;

            if (date < today)
            {
                reason = "Appointments cannot be booked for a past date.";
                return free;
            }
            if (date > today.AddDays(BookingWindowDays))
            {
                reason = "Appointments can be booked up to " + BookingWindowDays + " days ahead.";
                return free;
            }

            DayHours h = GetDayHours(con, doctorId, date);
            if (!h.Working)
            {
                reason = h.NotWorkingReason;
                return free;
            }

            List<TimeSpan> booked = GetBookedTimes(con, doctorId, date);
            List<TimeSpan> slots = SlotTimes(h.Start, h.End, h.SlotDuration, h.BreakStart, h.BreakEnd);
            bool allTaken = slots.TrueForAll(booked.Contains);

            DateTime earliest = DateTime.Now.AddMinutes(bufferMinutes);
            foreach (TimeSpan slot in slots)
            {
                if (date == today && today.Add(slot) < earliest) continue;   // travel buffer: today only
                if (!booked.Contains(slot)) free.Add(slot);
            }

            if (free.Count == 0)
                reason = allTaken
                    ? "All appointments for this doctor are booked on " + FormatDate(date) + "."
                    : date == today
                    ? "No more appointment slots are available for this doctor today."
                    : "No appointment slots are available for this doctor on " + FormatDate(date) + ".";
            return free;
        }

        /// <summary>First free slot on a date, or null with a reason.</summary>
        public static TimeSpan? GetNextAvailableSlot(SqlConnection con, int doctorId, DateTime date, int bufferMinutes, out string reason)
        {
            List<TimeSpan> free = GetFreeSlots(con, doctorId, date, bufferMinutes, out reason);
            if (free.Count > 0) return free[0];
            return null;
        }

        /// <summary>First free slot today (walk-in desk), or null with a reason.</summary>
        public static TimeSpan? GetNextAvailableSlot(SqlConnection con, int doctorId, int bufferMinutes, out string reason)
        {
            return GetNextAvailableSlot(con, doctorId, DateTime.Today, bufferMinutes, out reason);
        }

        /// <summary>True when this exact slot can still be booked on that date (re-checked right before saving).</summary>
        public static bool IsSlotFree(SqlConnection con, int doctorId, DateTime date, TimeSpan slot, int bufferMinutes, out string reason)
        {
            List<TimeSpan> free = GetFreeSlots(con, doctorId, date, bufferMinutes, out reason);
            if (free.Contains(slot)) return true;
            if (reason == null) reason = "The " + Format(slot) + " slot on " + FormatDate(date) + " is no longer available. Please choose another time.";
            return false;
        }

        /// <summary>The first date in the booking window with a free slot (for "Next available" on the doctor list).</summary>
        public static DateTime? GetNextAvailableDate(SqlConnection con, int doctorId, int bufferMinutes, out TimeSpan firstSlot)
        {
            firstSlot = TimeSpan.Zero;
            for (int i = 0; i <= BookingWindowDays; i++)
            {
                string ignored;
                DateTime date = DateTime.Today.AddDays(i);
                List<TimeSpan> free = GetFreeSlots(con, doctorId, date, bufferMinutes, out ignored);
                if (free.Count > 0)
                {
                    firstSlot = free[0];
                    return date;
                }
            }
            return null;
        }

        /// <summary>Slot length on a date (for showing "09:00 AM - 09:30 AM"), or null when the doctor does not work then.</summary>
        public static int? GetSlotDuration(SqlConnection con, int doctorId, DateTime date)
        {
            using (SqlCommand cmd = new SqlCommand(
                "SELECT SlotDuration FROM DoctorWorkingHours WHERE DoctorID = @DoctorID AND DayOfWeek = @Day AND IsWorking = 1", con))
            {
                cmd.Parameters.AddWithValue("@DoctorID", doctorId);
                cmd.Parameters.AddWithValue("@Day", (int)date.DayOfWeek);
                object v = cmd.ExecuteScalar();
                return v == null || v == DBNull.Value ? (int?)null : Convert.ToInt32(v);
            }
        }

        /// <summary>
        /// Why this patient cannot have this appointment, or null when they can. A patient (one person, not
        /// the account) cannot see the same doctor twice in a day, nor be in two appointments whose times
        /// overlap with any doctors (each doctor's slot length is used, e.g. 10:00-10:30 clashes with 10:15).
        /// ignoreAppointmentId: the appointment being moved (reschedule). Opens its own connection and does not
        /// read the Patients row, so it is safe to call while a booking transaction holds that row.
        /// </summary>
        public static string PatientConflict(int patientId, string patientName, int doctorId, DateTime date, TimeSpan start, int? ignoreAppointmentId)
        {
            string who = string.IsNullOrWhiteSpace(patientName) ? "This patient" : patientName.Trim();
            string cs = System.Configuration.ConfigurationManager.ConnectionStrings["DocCareDB"].ConnectionString;
            using (SqlConnection con = new SqlConnection(cs))
            {
                con.Open();
                int newLength = GetSlotDuration(con, doctorId, date) ?? 30;
                TimeSpan newEnd = start + TimeSpan.FromMinutes(newLength);

                using (SqlCommand cmd = new SqlCommand(@"
                    SELECT a.DoctorID, a.AppointTime, a.Status, d.Name AS DoctorName, ISNULL(w.SlotDuration, 30) AS SlotDuration
                    FROM Appointments a
                    JOIN Doctors d ON d.DoctorID = a.DoctorID
                    LEFT JOIN DoctorWorkingHours w ON w.DoctorID = a.DoctorID AND w.DayOfWeek = @Day
                    WHERE a.PatientID = @PatientID AND a.AppointDate = @Date AND a.Status <> 'Cancelled'
                      AND (@IgnoreID IS NULL OR a.ID <> @IgnoreID)
                    ORDER BY a.AppointTime", con))
                {
                    cmd.Parameters.AddWithValue("@PatientID", patientId);
                    cmd.Parameters.AddWithValue("@Date", date.Date);
                    cmd.Parameters.AddWithValue("@Day", (int)date.DayOfWeek);
                    cmd.Parameters.AddWithValue("@IgnoreID", ignoreAppointmentId.HasValue ? (object)ignoreAppointmentId.Value : DBNull.Value);
                    System.Data.DataTable others = new System.Data.DataTable();
                    others.Load(cmd.ExecuteReader());

                    // Same doctor first: then no time that day would do, so say that rather than a single clash
                    foreach (System.Data.DataRow r in others.Rows)
                        if (Convert.ToInt32(r["DoctorID"]) == doctorId)
                            return who + " already has an appointment with Dr. " + DoctorName.WithoutTitle(r["DoctorName"].ToString()) +
                                   " on " + FormatDate(date) + ".";

                    // Another doctor: only a clash when the times overlap (a missed visit no longer holds the time)
                    foreach (System.Data.DataRow r in others.Rows)
                    {
                        if (r["AppointTime"] == DBNull.Value || r["Status"].ToString() == AppointmentStatus.NoShow) continue;
                        TimeSpan otherStart = (TimeSpan)r["AppointTime"];
                        TimeSpan otherEnd = otherStart + TimeSpan.FromMinutes(Convert.ToInt32(r["SlotDuration"]));
                        if (otherStart < newEnd && start < otherEnd)
                            return who + " already has an appointment with Dr. " + DoctorName.WithoutTitle(r["DoctorName"].ToString()) +
                                   " at " + Format(otherStart) + " on " + FormatDate(date) + ". Please choose a time that does not overlap it.";
                    }
                }
            }
            return null;
        }

        /// <summary>Next token for a doctor on a date: T-001, T-002 ... restarting for each doctor each day.</summary>
        public static string NextToken(SqlConnection con, SqlTransaction tx, int doctorId, DateTime date)
        {
            using (SqlCommand cmd = new SqlCommand(@"
                SELECT ISNULL(MAX(TRY_CAST(SUBSTRING(TokenNo, 3, 10) AS INT)), 0) + 1
                FROM Appointments
                WHERE DoctorID = @DoctorID AND AppointDate = @Date AND TokenNo LIKE 'T-%'", con, tx))
            {
                cmd.Parameters.AddWithValue("@DoctorID", doctorId);
                cmd.Parameters.AddWithValue("@Date", date.Date);
                return "T-" + Convert.ToInt32(cmd.ExecuteScalar()).ToString("D3");
            }
        }

        static List<TimeSpan> GetBookedTimes(SqlConnection con, int doctorId, DateTime date)
        {
            List<TimeSpan> booked = new List<TimeSpan>();
            using (SqlCommand cmd = new SqlCommand(@"SELECT AppointTime FROM Appointments
                                   WHERE DoctorID = @DoctorID AND AppointDate = @Date AND Status <> 'Cancelled'", con))
            {
                cmd.Parameters.AddWithValue("@DoctorID", doctorId);
                cmd.Parameters.AddWithValue("@Date", date.Date);
                using (SqlDataReader r = cmd.ExecuteReader())
                {
                    while (r.Read())
                        if (r["AppointTime"] != DBNull.Value) booked.Add((TimeSpan)r["AppointTime"]);
                }
            }
            return booked;
        }

        /// <summary>Formats a slot for display, e.g. "11:30 AM".</summary>
        public static string Format(TimeSpan slot)
        {
            return DateTime.Today.Add(slot).ToString("hh:mm tt", Invariant);
        }

        /// <summary>Formats a date for messages, e.g. "Tue, 22 Oct 2026".</summary>
        public static string FormatDate(DateTime date)
        {
            return date.ToString("ddd, dd MMM yyyy", Invariant);
        }
    }
}
