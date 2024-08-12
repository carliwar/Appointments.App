using Appointments.App.Models.Enum;
using SQLite;
using SQLiteNetExtensions.Attributes;
using System;

namespace Appointments.App.Models.DataModels
{
    public class User
    {
        public string UserFullName { get; set; }

        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }        
        public string ContificoId { get; set; }
        public string Identification { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string Name { get; set; }
        public string LastName { get; set; }
        public DateTime? BirthDate { get; set; }
        public UserTypeEnum UserType { get; set; }

        [ForeignKey(typeof(AppointmentType))]
        public int? AppointmentTypeId { get; set; }

        [ManyToOne(CascadeOperations = CascadeOperation.CascadeRead)]
        public AppointmentType AppointmentType { get; set; }
    }
}
