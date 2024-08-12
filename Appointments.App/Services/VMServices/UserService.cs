using System.Threading.Tasks;
using Appointments.App.Models.ContificoModels.Requests;
using Appointments.App.Models.ContificoModels.Responses;
using Appointments.App.Models.DataModels;
using Appointments.App.Models.TransactionModels;

namespace Appointments.App.Services.VMServices
{
    public class UserService : IUserService
    {
        private readonly IHttpHelperService _httpHelperService;
        private readonly IDataService _dataService;

        public UserService(IHttpHelperService httpHelperService, IDataService dataService)
        {
            _httpHelperService = httpHelperService;
            _dataService = dataService;
        }

        public async Task<User> GetUser(int id)
        {
            return await _dataService.GetUser(id);
        }

        public async Task<UserSaveResponse> SaveUser(User user)
        {
            var isNewUser = user.Id == 0;
            var result = await _dataService.SaveUser(user);
            
            if (result.Success)
            {
                // Get contifico User

                if (isNewUser)
                {
                    var contificoUser = await _httpHelperService.GetAsync<ContificoPerson>("CONTIFICO", $"people/?identificacion={user.Identification}");
                    // if not null then update with new data
                    if (contificoUser != null) {
                        var contificoSaveRequest = new ContificoPersonSaveRequest
                        {
                            id = contificoUser.Id,
                            tipo = contificoUser.Tipo,
                            razon_social = user.UserFullName,
                            telefonos = user.Phone,
                            cedula = user.Identification,
                            email = user.Email
                        };

                    }
                }

            }
            throw new System.NotImplementedException();
        }
    }
}
