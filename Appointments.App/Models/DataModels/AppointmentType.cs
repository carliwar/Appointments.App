using Appointments.App.Models.Enum;
using SQLite;

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
    }
}
