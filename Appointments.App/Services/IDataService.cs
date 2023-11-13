using Appointments.App.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Appointments.App.Services
{
    public interface IDataService
    {
        Task<IEnumerable<PersonType>> GetPersonTypes();

        Task<User> CreateUser(User person);
        Task<IEnumerable<User>> GetUsers();
        Task<IEnumerable<User>> GetUsersByType(UserType userType);
    }
}
