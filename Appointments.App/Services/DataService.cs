using Appointments.App.Data;
using Appointments.App.Infrastructure;
using Appointments.App.Models;
using Appointments.App.Models.DataModels;
using Appointments.App.Models.Enum;
using Appointments.App.Models.TransactionModels;
using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace Appointments.App.Services
{
    public class DataService : IDataService
    {
        private SQLiteAsyncConnection _database;
        public DataService()
        {
            _database = new SQLiteAsyncConnection(AppConfiguration.DatabasePath, AppConfiguration.Flags);

        }

        #region Users

        public async Task<User> GetUser(int id)
        {
            await _database.CreateTableAsync<User>();
            var db = new Repository<User>(_database);
            return await db.Get(id);
        }

        public async Task<UserCreationResponse> SaveUser(User user)
        {
            var result = await ValidateUser(user);

            if (result.Success)
            {
                await _database.CreateTableAsync<User>();
                var db = new Repository<User>(_database);

                if (user.Id == 0)
                {
                    await db.Insert(user);
                }
                else
                {
                    await db.Update(user);
                }
            }

            return result;
        }

        private async Task<UserCreationResponse> ValidateUser(User user)
        {
            var result = new UserCreationResponse();

            // validate if user exists in _database.User
            if (string.IsNullOrWhiteSpace(user.Identification))
            {
                if (await UserByIdentification(user.Identification) != null)
                {
                    result.Errors.Add($"Ya existe un paciente con la Cédula/Identificación ingresada. Paciente: {user.UserFullName}");
                }
            }

            if (!string.IsNullOrWhiteSpace(user.Phone))
            {
                if (user.Phone.StartsWith("09") && user.Phone.Length < 10)
                {
                    result.Errors.Add($"Teléfono incorrecto. Paciente: {user.UserFullName}");
                }
            }
            return result;
        }

        public async Task<IEnumerable<User>> GetUsers()
        {
            await _database.CreateTablesAsync<User, User>();
            var db = new Repository<User>(_database);

            return await db.Get();
        }

        public async Task<IEnumerable<User>> GetUsersByType(UserTypeEnum userType, string searchText = "")
        {
            await _database.CreateTableAsync<User>();
            var db = new Repository<User>(_database);
            List<User> users = await db.GetAllWithChildren();

            var formattedSearch = searchText?.ToLower();

            if (!string.IsNullOrWhiteSpace(formattedSearch))
            {
                users = users.Where(
                    t => (t.Identification != null && t.Identification.ToLower().Contains(formattedSearch))
                    || (t.Name != null && t.Name.ToLower().Contains(formattedSearch))
                    || (t.AppointmentType != null && t.AppointmentType.Name.ToLower().Contains(formattedSearch))
                    || (t.LastName != null && t.LastName.ToLower().Contains(formattedSearch))
                ).ToList();
            }

            return users.Where(t => t.UserType == userType).ToList();
        }

        private async Task<User> UserByIdentification(string identification)
        {
            await _database.CreateTablesAsync<User, User>();
            var db = new Repository<User>(_database);
            List<User> users = await db.Get();
            User user = null;

            if (!string.IsNullOrWhiteSpace(identification))
            {
                user = users.FirstOrDefault(t => (!string.IsNullOrWhiteSpace(t.Identification)
                    && t.Identification == identification)
                );
            }

            return user;
        }

        public async Task<IEnumerable<PersonType>> GetPersonTypes()
        {
            await _database.CreateTablesAsync<PersonType, PersonType>();
            var db = new Repository<PersonType>(_database);

            return await db.Get();
        }

        private async Task<User> GetUserById(int id)
        {
            await _database.CreateTablesAsync<User, User>();
            var db = new Repository<User>(_database);
            return await db.Get(id);
        }

        private async Task<List<User>> GetUserByIds(List<int> ids)
        {
            await _database.CreateTablesAsync<User, User>();
            var db = new Repository<User>(_database);
            var users = await db.Get();
            return users.Where(t => ids.Contains(t.Id)).ToList();
        }
        #endregion

        #region Appointments
        public async Task<Appointment> GetAppointment(int id)
        {
            await _database.CreateTablesAsync<Appointment, Appointment>();
            var db = new Repository<Appointment>(_database);
            var appointment = await db.Get(t => t.Id == id);
            return appointment;
        }
        public async Task<Appointment> CreateAppointment(Appointment appointment)
        {
            await _database.CreateTablesAsync<Appointment, Appointment>();
            var db = new Repository<Appointment>(_database);
            await db.Insert(appointment);
            return appointment;
        }

        public async Task<Appointment> UpdateAppointment(Appointment appointment)
        {
            await _database.CreateTablesAsync<Appointment, Appointment>();
            var db = new Repository<Appointment>(_database);
            await db.Update(appointment);
            return appointment;
        }

        public async Task<AppointmentCreationResponse> CreateValidatedAppointment(Appointment appointment)
        {
            var result = await ValidateAppointmentDateTime(appointment);

            if (result.Success)
            {
                await _database.CreateTableAsync<Appointment>();
                var dbAppointment = new Repository<Appointment>(_database);
                await dbAppointment.Insert(appointment);


                await _database.CreateTableAsync<AppointmentAppointmentType>();

                var dbAppointmentType = new Repository<AppointmentAppointmentType>(_database);

                foreach (var appointmentType in appointment.AppointmentTypes)
                {
                    var newAppointmentAppointmentType = new AppointmentAppointmentType
                    {
                        AppointmentId = appointment.Id,
                        AppointmentTypeId = appointmentType.Id,
                    };
                    await dbAppointmentType.Insert(newAppointmentAppointmentType);
                }
            }

            return result;
        }

        public async Task<List<Appointment>> GetAppointments(DateTime start, DateTime end)
        {
            await _database.CreateTableAsync<Appointment>();
            var db = new Repository<Appointment>(_database);
            var appointments = await db.GetAllWithChildren();

            // get appointments from start and end dates
            appointments = appointments.Where(t =>
                    (t.AppointmentDate.Date >= start.Date)
                 && t.AppointmentDate.Date <= end.Date).ToList();

            var users = await GetUserByIds(appointments.Select(t => t.UserId).ToList());

            // set name from user for each appointment
            foreach (var appointment in appointments)
            {
                appointment.UserName = users.FirstOrDefault(t => t.Id == appointment.UserId).UserFullName;
                appointment.UserPhone = users.FirstOrDefault(t => t.Id == appointment.UserId).Phone;
            }

            return appointments;
        }

        private async Task<AppointmentCreationResponse> ValidateAppointmentDateTime(Appointment appointment)
        {
            var response = new AppointmentCreationResponse();
            var appointmentsOfDay = await GetAppointments(appointment.AppointmentDate.Date, appointment.AppointmentDate.Date.AddDays(1));

            // check if appointment.Date is at the start or end of any other appointment
            var sameStartingDateTime = appointmentsOfDay.Any(t => t.AppointmentDate == appointment.AppointmentDate);

            if (sameStartingDateTime)
            {
                response.Errors.Add($"Ya existe una cita para la hora escogida el: {appointment.AppointmentDate:dd/MMM/yyyy}");
            }

            var crossingStartingDateTime = appointmentsOfDay.Where(t => appointment.AppointmentEnd > t.AppointmentDate && appointment.AppointmentEnd <= t.AppointmentEnd).ToList();

            if (crossingStartingDateTime.Any())
            {
                var conflictDates = string.Join("/", crossingStartingDateTime.Select(t => t.AppointmentDate.ToString("HH:mm")));
                response.Errors.Add($"Conflicto con citas en los siguientes horarios: {appointment.AppointmentDate:HH:mm}");
            }

            return response;
        }
        #endregion

        #region Settings
        public async Task<Setting> GetSetting(int id)
        {
            await _database.CreateTableAsync<Setting>();
            var db = new Repository<Setting>(_database);
            return await db.Get(id);
        }

        public async Task<Setting> GetSettingByNameAndCatalog(string name, string catalog)
        {
            name = name.ToLower();
            catalog = catalog.ToLower();

            await _database.CreateTableAsync<Setting>();
            var db = new Repository<Setting>(_database);
            var settings = await db.Get();
            var result = settings.FirstOrDefault(t => t.Name.ToLower() == name && t.Catalog.ToLower() == catalog.ToLower());

            return result;
        }
        
        public async Task<Setting> SaveSetting(Setting setting)
        {
            await _database.CreateTableAsync<Setting>();

            var db = new Repository<Setting>(_database);

            if (setting.Id == 0)
            {
                await db.Insert(setting);
            }
            else
            {
                await db.Update(setting);
            }

            return setting;
        }


        public async Task<List<Setting>> GetAllSettings(string searchText = "")
        {
            await _database.CreateTableAsync<Setting>();
            var db = new Repository<Setting>(_database);
            var appointments = await db.Get();

            var formattedSearch = searchText?.ToLower();

            if (!string.IsNullOrWhiteSpace(formattedSearch))
            {
                appointments = appointments.Where(
                    t => (t.Name != null && t.Name.ToLower().Contains(formattedSearch))
                    || (t.Catalog != null && t.Catalog.ToLower().Contains(formattedSearch))
                ).ToList();
            }

            return appointments;
        }

        public async Task<List<Setting>> GetSettingsByCatalog(string catalog)
        {
            await _database.CreateTableAsync<Setting>();
            var db = new Repository<Setting>(_database);
            var appointments = await db.Get();

            // get appointments from start and end dates
            appointments = appointments.Where(t => t.Catalog == catalog).ToList();

            return appointments;
        }
        #endregion

        #region User Appointments
        public async Task<List<Appointment>> GetAppointmentsByUser(User user, DateTime? start, DateTime? end)
        {
            await _database.CreateTableAsync<Appointment>();
            var db = new Repository<Appointment>(_database);
            var appointments = await db.GetAllWithChildren();

            // get appointments from start and end dates
            appointments = appointments
                .Where(t => t.UserId == user.Id).ToList();

            if (start.HasValue && end.HasValue)
            {
                appointments = appointments
                .Where(t => t.AppointmentDate.Date >= start.Value.Date
                 && t.AppointmentDate.Date <= end.Value.Date).ToList();
            }

            return appointments;
        }
        #endregion

        #region Appointment Types

        public async Task<List<AppointmentType>> GetAppointmentTypes(string searchText = "")
        {
            await _database.CreateTablesAsync<AppointmentType, AppointmentType>();
            var db = new Repository<AppointmentType>(_database);
            var appointmentTypes = await db.Get();

            var formattedSearch = searchText?.ToLower();

            if (!string.IsNullOrWhiteSpace(formattedSearch))
            {
                appointmentTypes = appointmentTypes.Where(
                    t => (t.Name != null && t.Name.ToLower().Contains(formattedSearch))
                    || (t.Description != null && t.Description.ToLower().Contains(formattedSearch))
                ).ToList();
            }

            return appointmentTypes;
        }

        public async Task<AppointmentType> GetAppointmentType(int id)
        {
            await _database.CreateTablesAsync<AppointmentType, AppointmentType>();
            var db = new Repository<AppointmentType>(_database);
            var appointmentType = await db.Get(t => t.Id == id);
            return appointmentType;
        }

        public async Task<AppointmentTypeSaveResponse> SaveAppointmentType(AppointmentType appointmentType)
        {
            var result = await ValidateAppointmentType(appointmentType);

            if (result.Success)
            {
                await _database.CreateTablesAsync<AppointmentType, AppointmentType>();
                var db = new Repository<AppointmentType>(_database);

                if (appointmentType.Id > 0)
                {
                    await db.Update(appointmentType);
                }
                else
                {
                    await db.Insert(appointmentType);
                }
            }

            return result;
        }

        private async Task<AppointmentTypeSaveResponse> ValidateAppointmentType(AppointmentType appointmentType)
        {
            var result = new AppointmentTypeSaveResponse();

            var appointmentTypes = await GetAppointmentTypes();

            // check if appointmentType has an exact duplicated value in appointmentTypes
            if (appointmentTypes.Any())
            {
                var sameType = appointmentTypes
                    .Where(t => t.Name == appointmentType.Name && t.Id != appointmentType.Id)
                    .FirstOrDefault();

                if (sameType != null)
                {
                    result.Errors.Add($"Ya existe un tipo de cita con nombre: {appointmentType.Name} ó descripción: {appointmentType.Description}");
                }
            }

            // TODO: check similarities (fuzzy results)

            return result;
        }

        #endregion

        #region API Call implementation
        //private readonly string _apiURL;
        //private HttpClient _client;
        //public DataService()
        //{
        //    var handler = new HttpClientHandler();
        //    handler.ServerCertificateCustomValidationCallback += (sender, cert, chain, sslPolicyErrors) => true;

        //    _client = new HttpClient(handler);
        //    _apiURL = AppConfiguration.API_URL;
        //}
        //public async Task<IEnumerable<PersonType>> GetPersonTypes()
        //{
        //    var url = $"{"https://192.168.1.17:444/"}{AppConfiguration.USER_ENDPOINT}/getusers";

        //    try
        //    {
        //        HttpResponseMessage response = await _client.GetAsync(url);
        //        if (response.IsSuccessStatusCode)
        //        {
        //            var result = response.Content.ReadAsStringAsync().Result;
        //            var personTypes = JsonConvert.DeserializeObject<IEnumerable<PersonType>>(result);
        //            return personTypes;
        //        }
        //        else
        //        {
        //            throw new Exception("Error al obtener los tipos de personas");
        //        }
        //    }
        //    catch (Exception ex)
        //    {

        //        throw;
        //    }

        //} 
        #endregion
    }
}
