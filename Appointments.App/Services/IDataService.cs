using Appointments.App.Models;
using Appointments.App.Models.DataModels;
using Appointments.App.Models.Enum;
using Appointments.App.Models.TransactionModels;
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
        Task<UserCreationResponse> CreateValidatedUser(User user);
        Task<IEnumerable<User>> GetUsers();
        Task<IEnumerable<User>> GetUsersByType(UserTypeEnum userType, string searchText="");

        // Appointment
        Task<Appointment> GetAppointment(int id);
        Task<Appointment> CreateAppointment(Appointment appointment);
        Task<Appointment> UpdateAppointment(Appointment appointment);
        Task<AppointmentCreationResponse> CreateValidatedAppointment(Appointment appointment);
        Task<List<Appointment>> GetAppointments(DateTime start, DateTime end);

        // User Appointments
        Task<List<Appointment>> GetAppointmentsByUser(User user, DateTime? start, DateTime? end);

        // Settings
        Task<Setting> CreateSetting(Setting setting);
        Task<List<Setting>> GetSettingsByCatalog(string catalog);


    }
}
