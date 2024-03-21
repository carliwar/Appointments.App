namespace Appointments.App.Models
{
    public class EventModel
    {
        public int Id { get; set; }
        public string AppointmentType { get; set; }
        public string UserInformation { get; set; }
        public string UserPhone { get; set; }
        public DateTime EventDate { get; set; }
        public DateTime EndDate { get; set; }
        public Color AppointmentColor { get; set; }
        public string Time 
        {   get 
            {
                return $"{EventDate.ToString("HH:mm")} - {EndDate.ToString("HH:mm")}";
            } 
        }
    }
}
