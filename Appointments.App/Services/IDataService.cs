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
        Task<User> GetUser(int userId);
        Task<UserCreationResponse> SaveUser(User user);
        Task<IEnumerable<User>> GetUsers();
        Task<IEnumerable<User>> GetUsersByType(UserTypeEnum userType, string searchText="");

        // Appointment
        Task<Appointment> GetAppointment(int id);
        Task<Appointment> CreateAppointment(Appointment appointment);
        Task<Appointment> UpdateAppointment(Appointment appointment);
        Task<AppointmentCreationResponse> CreateValidatedAppointment(Appointment appointment);
        Task<List<Appointment>> GetAppointments(DateTime start, DateTime end);
        Task<CalendarEventLog> AddCalendarEventLog(CalendarEventLog calendarEventLog);
        Task<CalendarEventLog> GetCalendarEventLog(int appointmentId);
        Task<int> DeleteCalendarEventLog(int appointmentId);
        Task<int> DeleteAppointment(int appointmentId);

        // User Appointments
        Task<List<Appointment>> GetAppointmentsByUser(User user, DateTime? start, DateTime? end);

        // Settings
        Task<Setting> GetSetting(int id);
        Task<Setting> GetSettingByNameAndCatalog(string name, string catalog);
        Task<Setting> SaveSetting(Setting setting);
        Task<List<Setting>> GetSettingsByCatalog(string catalog);
        Task<List<Setting>> GetAllSettings(string searchText = "");
        Task<List<AppointmentType>> GetAppointmentTypes(string searchText = "");
        Task<AppointmentType> GetAppointmentType(int id);
        Task<AppointmentTypeSaveResponse> SaveAppointmentType(AppointmentType appointmentType);


    }
}
