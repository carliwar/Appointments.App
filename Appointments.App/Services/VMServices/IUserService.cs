using Appointments.App.Models.DataModels;
using Appointments.App.Models.TransactionModels;
using System.Threading.Tasks;

namespace Appointments.App.Services.VMServices
{
    public interface IUserService
    {
        Task<User> GetUser(int id);
        Task<UserSaveResponse> SaveUser(User user);
    }
}
