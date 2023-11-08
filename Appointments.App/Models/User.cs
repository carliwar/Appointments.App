
using System;

namespace Appointments.App.Models
{
    public class User
    {
        public string UserValue
        {
            get
            {
                return Identification + " - " + Name + " " + LastName;
            }
        }

        public int Id { get; set; }
        public string Identification { get; set; }
        public string Name { get; set; }
        public string LastName { get; set; }
        public DateTime? BirthDate { get; set; }
        public UserType UserType { get; set; }
    }
}
