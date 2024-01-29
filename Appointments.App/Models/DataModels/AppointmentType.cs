using Appointments.App.Models.Enum;
using SQLite;
using SQLiteNetExtensions.Attributes;
using System.Collections.Generic;

namespace Appointments.App.Models.DataModels
{
    public class AppointmentType
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public AppointmentDurationEnum DefaultDuration { get; set; }
        public string ColorCode { get; set; }
        public bool Enabled { get; set; }

        [ManyToMany(typeof(Appointment))]
        public List<Appointment> Appointments { get; set; }
    }
}
