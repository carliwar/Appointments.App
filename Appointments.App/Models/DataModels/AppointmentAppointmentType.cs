using SQLiteNetExtensions.Attributes;

namespace Appointments.App.Models.DataModels
{
    public class AppointmentAppointmentType
    {
        [ForeignKey(typeof(Appointment))]
        public int AppointmentId { get; set; }
        [ForeignKey(typeof(AppointmentType))]
        public int AppointmentTypeId { get; set; }
    }
}
