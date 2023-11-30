using System;

namespace Appointments.App.Models
{
    public class Appointment
    {
        public int Id { get; set; }
        public DateTime AppointmentDate { get; set; }
        public AppointmentType AppointmentType { get; set; }
        public string UserId { get; set; }
    }
}
