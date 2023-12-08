using System.Collections.Generic;
using System.Linq;

namespace Appointments.App.Models.TransactionModels
{
    public class AppointmentCreationResponse
    {
        public AppointmentCreationResponse()
        {
            Errors = new List<string>();
            Suggestions = new List<string>();
        }
        public List<string> Errors { get; set; }
        public List<string> Suggestions { get; set; }

        public bool Success { get => !Errors.Any(); }

    }
}
