using SQLite;

namespace Appointments.App.Models.DataModels
{
    public class CalendarEventLog
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public int AppointmentId { get; set; }
        public string EventId { get; set; }
        public string ReminderId { get; set; }
    }
}
