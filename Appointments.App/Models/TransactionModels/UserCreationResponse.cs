using System.Collections.Generic;
using System.Linq;

namespace Appointments.App.Models.TransactionModels
{
    public class UserCreationResponse
    {
        public UserCreationResponse()
        {
            Errors = new List<string>();
        }
        public List<string> Errors { get; set; }

        public bool Success { get => !Errors.Any(); }
    }
}
