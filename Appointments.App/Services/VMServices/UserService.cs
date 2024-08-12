using System.Collections.Generic;
using System.Threading.Tasks;
using Appointments.App.Models.ContificoModels.Requests;
using Appointments.App.Models.ContificoModels.Responses;
using Appointments.App.Models.DataModels;
using Appointments.App.Models.Enum;
using Appointments.App.Models.TransactionModels;
using Xamarin.Essentials;

namespace Appointments.App.Services.VMServices
{
    public class UserService : IUserService
    {
        private readonly IHttpHelperService _httpHelperService;
        private readonly IDataService _dataService;
        private readonly IDeviceContactService _deviceContactService;

        public UserService(
            IHttpHelperService httpHelperService,
            IDataService dataService,
            IDeviceContactService deviceContactService
        )
        {
            _httpHelperService = httpHelperService;
            _dataService = dataService;
            _deviceContactService = deviceContactService;
        }

        public async Task<User> GetUser(int id)
        {
            return await _dataService.GetUser(id);
        }

        public async Task<UserSaveResponse> SaveUser(User user)
        {
            var result = new UserSaveResponse();
            var isNewUser = user.Id == 0;
            var localDbSaveResult = await _dataService.SaveUser(user);

            if (localDbSaveResult.Success)
            {
                // TODO Add Id to response
                var resultFromSaveInContifico = await GetContificoSaveResult(user);

                // Get contifico User
                if (resultFromSaveInContifico.Success)
                {
                    var deviceContact = new Contact
                    {
                        Id = user.DeviceContactId,
                        NamePrefix = UserTypeEnum.Paciente.ToString(),
                        GivenName = user.UserFullName,
                        Phones = new List<ContactPhone>
                        {
                            new ContactPhone { PhoneNumber = user.Phone }
                        }
                    };

                    await _deviceContactService.SaveDeviceContact(deviceContact);

                    if (isNewUser)
                    {
                        // Get user from localDB to update the contifico & device ids
                        var localDbUser = await _dataService.GetUser(localDbSaveResult.LocalDbId);
                        localDbUser.ContificoId = resultFromSaveInContifico.ContificoId;
                        localDbUser.DeviceContactId = deviceContact.Id;
                        localDbSaveResult = await _dataService.SaveUser(user);
                    }
                }
                else
                {
                    result.Errors.AddRange(resultFromSaveInContifico.Errors);
                }
            }
            else
            {
                result.Errors.AddRange(localDbSaveResult.Errors);
            }

            return result;
        }

        private async Task<UserSaveResponse> GetContificoSaveResult(User user)
        {
            var result = new UserSaveResponse();

            if (user.ContificoId == null)
            {
                var contificoUser = await _httpHelperService.GetAsync<ContificoPerson>(
                    "CONTIFICO",
                    $"people/?identificacion={user.Identification}"
                );

                var contificoSaveRequest = new ContificoPersonSaveRequest
                {
                    razon_social = user.UserFullName,
                    telefonos = user.Phone,
                    cedula = user.Identification,
                    email = user.Email,
                    direccion = user.Address,
                    es_extranjero = false,
                    es_vendedor = false,
                    es_empleado = false,
                    es_proveedor = false,
                    es_cliente = true
                };

                var contificoResponse = new ContificoTransactionResponse();

                // if null then create with new contifico person
                if (contificoUser == null)
                {
                    contificoSaveRequest.tipo = contificoUser.Tipo;

                    contificoResponse =
                        await _httpHelperService.PostAsync<ContificoTransactionResponse>(
                            "CONTIFICO",
                            "people",
                            contificoSaveRequest
                        );
                }
                else
                {
                    contificoSaveRequest.id = contificoUser.Id;

                    contificoResponse =
                        await _httpHelperService.PutAsync<ContificoTransactionResponse>(
                            "CONTIFICO",
                            "people",
                            contificoSaveRequest
                        );
                }

                if (!contificoResponse.IsValid)
                {
                    result.Errors.Add(contificoResponse.Error.Mensaje);
                }
            }

            return result;
        }
    }
}
