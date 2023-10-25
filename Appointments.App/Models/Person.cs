
namespace Appointments.App.Models
{
    public class Person
    {
        public string PersonValue
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
    }
}
