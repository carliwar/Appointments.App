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
        public AppointmentType? AppointmentType { get; set; }
        public string UserInformation { get; set; }
        public int UserId { get; set; }

        [Ignore]
        public string UserPhone { get; set; }
        [Ignore]
        public string UserName { get; set; }
    }
}
