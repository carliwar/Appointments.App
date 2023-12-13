using Appointments.App.Models.Enum;
using SQLite;
using System;

namespace Appointments.App.Models.DataModels
{
    public class User
    {
        public string UserFullName
        {
            get
            {
                var id = string.Empty;
                if(Identification != null)
                {
                    id = $"{Identification} - ";
                }
                return id + Name + " " + LastName;
            }
        }

        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public string Identification { get; set; }
        public string Phone { get; set; }
        public string Name { get; set; }
        public string LastName { get; set; }
        public DateTime? BirthDate { get; set; }
        public UserType UserType { get; set; }
    }
}
