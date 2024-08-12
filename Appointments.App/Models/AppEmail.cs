namespace Appointments.App.Models
{
    public class AppEmail
    {
        public string To { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        public string Sender { get; set; }
        public string Password { get; set; }
        
    }

    public class SignatureModel
    {
        public string Name { get; set; }
        public string Title { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
        public string Facebook { get; set; }
        public string Website { get; set; }
        public string Company { get; set; }
    }
}
