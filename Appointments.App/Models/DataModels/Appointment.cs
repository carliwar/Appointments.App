using SQLite;
using SQLiteNetExtensions.Attributes;

namespace Appointments.App.Models.DataModels
{
    public class Appointment
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public DateTime AppointmentDate { get; set; }
        public DateTime AppointmentEnd { get; set; }
        public string UserInformation { get; set; }
        public int UserId { get; set; }
        public bool Attended { get; set; }

        [ManyToMany(typeof(AppointmentAppointmentType), CascadeOperations = CascadeOperation.CascadeRead)]
        public List<AppointmentType> AppointmentTypes { get; set; }

        [Ignore]
        public string AppointmentInformation { 
            get {
                var attendedFlag = string.Empty;

                var appointmentTypes = string.Empty;

                if (!Attended)
                {
                    attendedFlag = $"{ConstantValues.NOT_ATTENDED} **";
                }
                if(AppointmentTypes != null && AppointmentTypes.Any())
                {
                    appointmentTypes = string.Join(", ", AppointmentTypes.Select(t => t.Name));
                }

                return $"{attendedFlag} {AppointmentDate:dd/MM/yyyy} {AppointmentDate:HH:mm} - {appointmentTypes}"; 
            } 
        }

        [Ignore]
        public string UserPhone { get; set; }
        
        [Ignore]
        public string UserName { get; set; }
        
        [Ignore]
        public Color AppointmentColor { get; set; }

        [Ignore]
        public Color TextColor { get; set; }
    }

    public class AndroidAppointment
    {
        public string Title{ get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Description { get; set; }
        public string Location { get; set; }
        public string AppointmentTypes { get; set; }
        public int ReminderMinutes { get; set; }
        public int CalendarID { get; set; }
    }
}
