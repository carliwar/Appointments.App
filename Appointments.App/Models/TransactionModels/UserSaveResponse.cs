using System.Collections.Generic;
using System.Linq;

namespace Appointments.App.Models.TransactionModels
{
    public class UserSaveResponse
    {
        public UserSaveResponse()
        {
            Errors = new List<string>();
        }
        public List<string> Errors { get; set; }

        public bool Success { get => !Errors.Any(); }
    }
}
