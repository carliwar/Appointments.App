using Appointments.App.Models.Enum;
using SQLite;
using System;

namespace Appointments.App.Models.DataModels
{
    public class Appointment
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public DateTime AppointmentDate { get; set; }
        public DateTime AppointmentEnd { get; set; }
        public AppointmentTypeEnum? AppointmentType { get; set; }
        public string UserInformation { get; set; }
        public int UserId { get; set; }
        public bool Attended { get; set; }

        [Ignore]
        public string AppointmentInformation { 
            get {
                var attendedFlag = string.Empty;
                if (!Attended)
                {
                    attendedFlag = $"{ConstantValues.NOT_ATTENDED} **";
                }
                return $"{attendedFlag} {AppointmentDate:dd/MM/yyyy} {AppointmentDate:HH:mm} - {AppointmentType}"; 
            } 
        }

        [Ignore]
        public string UserPhone { get; set; }
        
        [Ignore]
        public string UserName { get; set; }
        
        [Ignore]
        public System.Drawing.Color AppointmentColor { get; set; }

        [Ignore]
        public System.Drawing.Color TextColor { get; set; }
    }

    public class AndroidAppointment
    {
        public string Title{ get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Description { get; set; }
        public string Location { get; set; }
        public AppointmentTypeEnum? AppointmentType { get; set; }
        public int ReminderMinutes { get; set; }
    }
}
