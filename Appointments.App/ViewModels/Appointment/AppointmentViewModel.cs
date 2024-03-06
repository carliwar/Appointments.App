using Appointments.App.Models.DataModels;
using Appointments.App.Models.Enum;
using Appointments.App.Services;
using Plugin.Calendars;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Essentials;
using Xamarin.Forms.MultiSelectListView;
using Xamarin.Forms;
using Plugin.Calendars.Abstractions;
using Appointments.App.Utils;

namespace Appointments.App.ViewModels.Appointments
{
    public class AppointmentViewModel : BasePageViewModel
    {
        public AppointmentViewModel() : base()
        {
            _dataService = new DataService();
            
            foreach (AppointmentDurationEnum enumValue in Enum.GetValues(typeof(AppointmentDurationEnum)))
            {
                string customString = EnumDescriptor.GetEnumDescription(enumValue);

                var appointmentDuration = new AppointmentDuration
                {
                    Name = enumValue,
                    Description = customString
                };
                AppointmentDurations.Add(appointmentDuration);
            }

            Users = new ObservableCollection<Models.DataModels.User>();

        }
        #region Temp Properties

        private readonly IDataService _dataService;
        #endregion

        #region Properties

        private int _id;
        private string _userFullName;
        private DateTime _givenDate;
        private TimeSpan _givenTime;
        private ObservableCollection<Models.DataModels.User> _users = new ObservableCollection<Models.DataModels.User>();
        private ObservableCollection<AppointmentDuration> _appointmentDurations = new ObservableCollection<AppointmentDuration>();
        private MultiSelectObservableCollection<Models.DataModels.AppointmentType> _appointmentTypes = new MultiSelectObservableCollection<Models.DataModels.AppointmentType>();
        private ObservableCollection<Models.DataModels.AppointmentType> _filterAppointmentTypes = new ObservableCollection<Models.DataModels.AppointmentType>();
        private ObservableCollection<Models.DataModels.AppointmentType> _selectedAppointmentTypes = new ObservableCollection<Models.DataModels.AppointmentType>();        
        private AppointmentDuration _selectedAppointmentDuration;
        private Models.DataModels.User _selectedUser;
        private bool _showError = false;
        private int _appointmentTypesHeight;
        private Models.DataModels.AppointmentType _selectedAppointmentTypeFilter;

        public int Id
        {
            get => _id;
            set => SetProperty(ref _id, value);
        }

        public string UserFullName
        {
            get => _userFullName;
            set => SetProperty(ref _userFullName, value);
        }

        public DateTime GivenDate
        {
            get => _givenDate;
            set => SetProperty(ref _givenDate, value);
        }

        public TimeSpan GivenTime
        {
            get => _givenTime;
            set => SetProperty(ref _givenTime, value);
        }

        public ObservableCollection<Models.DataModels.User> Users
        {
            get => _users;
            set => SetProperty(ref _users, value);
        }

        public ObservableCollection<AppointmentDuration> AppointmentDurations
        {
            get => _appointmentDurations;
            set => SetProperty(ref _appointmentDurations, value);
        }
        public MultiSelectObservableCollection<Models.DataModels.AppointmentType> AppointmentTypes
        {
            get => _appointmentTypes;
            set => SetProperty(ref _appointmentTypes, value);
        }

        public ObservableCollection<Models.DataModels.AppointmentType> FilterAppointmentTypes
        {
            get => _filterAppointmentTypes;
            set => SetProperty(ref _filterAppointmentTypes, value);
        }

        public ObservableCollection<Models.DataModels.AppointmentType> SelectedAppointmentTypes
        {
            get => _selectedAppointmentTypes;
            set => SetProperty(ref _selectedAppointmentTypes, value);
        }

        public AppointmentDuration SelectedAppointmentDuration
        {
            get => _selectedAppointmentDuration;
            set => SetProperty(ref _selectedAppointmentDuration, value);
        }
        public Models.DataModels.User SelectedUser
        {
            get => _selectedUser;
            set => SetProperty(ref _selectedUser, value);
        }
        public bool ShowError
        {
            get => _showError;
            set => SetProperty(ref _showError, value);
        }

        public int AppointmentTypesHeight
        {
            get => _appointmentTypesHeight;
            set => SetProperty(ref _appointmentTypesHeight, value);
        }
        public Models.DataModels.AppointmentType SelectedAppointmentTypeFilter
        {
            get => _selectedAppointmentTypeFilter;
            set
            {
                SetProperty(ref _selectedAppointmentTypeFilter, value);

                var searchValue = string.Empty;

                if (SelectedAppointmentTypeFilter.Id != 0)
                {
                    searchValue = SelectedAppointmentTypeFilter.Name;
                }

                SearchUserAsync(searchValue);
            }
        }

        public string EmailAccount { get; set; }
        #endregion

        #region Commands
        //SearchUserCommand
        public ICommand SearchUserCommand => new Command((item) => SearchUserAsync(item));
        public ICommand SaveAppointmentCommand => new Command(async (item) => await SaveAppointment(item));
        public ICommand FillSelectedAppointmentTypesCommand => new Command(async (item) => await FillSelectedAppointmentTypes(item));

        private void SearchUserAsync(object sender)
        {

            if (sender == null)
            {
                Task.Run(() => LoadUsers()).Wait();
            }
            else
            {
                var searchText = string.Empty;

                if (sender is TextChangedEventArgs search)
                {
                    searchText = search.NewTextValue;
                }
                else if (sender is string @string)
                {
                    searchText = @string;
                }

                Task.Run(() => LoadUsers(searchText)).Wait();
            }
        }

        private async Task SaveAppointment(object sender)
        {
            if (SelectedUser == null)
            {
                ShowError = true;
            }
            else
            {
                ShowError = false;
            }

            if (ShowError)
            {
                return;
            }

            var appointment = new Appointment
            {
                UserId = SelectedUser.Id,
                AppointmentDate = GivenDate.Date.Add(GivenTime),
                AppointmentEnd = GivenDate.Date.Add(GivenTime).AddMinutes((double)SelectedAppointmentDuration.Name),
                UserInformation = SelectedUser.UserFullName,
                Attended = true,
                AppointmentTypes = SelectedAppointmentTypes?.ToList(),
                UserPhone = SelectedUser.Phone
            };

            var result = await _dataService.CreateValidatedAppointment(appointment);

            if (result != null)
            {
                if (result.Success)
                {
                    var statusRead = await Permissions.CheckStatusAsync<Permissions.CalendarRead>();
                    var statusWrite = await Permissions.CheckStatusAsync<Permissions.CalendarWrite>();

                    if (statusRead == PermissionStatus.Granted && statusWrite == PermissionStatus.Granted)
                    {
                        await CreateDeviceAppointment(appointment);
                    }
                    else
                    {
                        var requestR = await Permissions.RequestAsync<Permissions.CalendarRead>();
                        var requestW = await Permissions.RequestAsync<Permissions.CalendarWrite>();

                        if (requestR == PermissionStatus.Granted && requestW == PermissionStatus.Granted)
                        {
                            await CreateDeviceAppointment(appointment);
                        }
                    }

                    await Application.Current.MainPage.DisplayAlert("Éxito!", "Cita agendada", "Ok");
                    await Application.Current.MainPage.Navigation.PopAsync();
                }
                else
                {
                    // display all errors from list as a single string from result
                    await Application.Current.MainPage.DisplayAlert("Errores: ", string.Join(" / ", result.Errors), "Ok");
                }
            }
            else
            {
                await Application.Current.MainPage.DisplayAlert("Error", "Contacte al administrador", "Ok");
            }
        }

        private async Task FillSelectedAppointmentTypes(object sender)
        {
            if (sender is Models.DataModels.AppointmentType selectedAppointmentType)
            {
                SelectedAppointmentDuration = AppointmentDurations.SingleOrDefault(t => t.Name == selectedAppointmentType.DefaultDuration);

                if (!SelectedAppointmentTypes.Any(t => t.Id == selectedAppointmentType.Id))
                {
                    SelectedAppointmentTypes.Add(selectedAppointmentType);
                }
                else
                {
                    SelectedAppointmentTypes.Remove(selectedAppointmentType);
                }

                if (SelectedAppointmentTypes.Any())
                {
                    SelectedAppointmentDuration = AppointmentDurations.SingleOrDefault(t => t.Name == SelectedAppointmentTypes.Max(u => u.DefaultDuration));
                }
                else
                {
                    SelectedAppointmentDuration = null;
                }

            }
        }
        private async Task CreateDeviceAppointment(Models.DataModels.Appointment appointment)
        {

            var account = await _dataService.GetSettingByNameAndCatalog("email", "basic");

            if (account != null)
            {
                EmailAccount = account.Value;
            }

            IList<Calendar> calendars = await CrossCalendars.Current.GetCalendarsAsync();

            var appCalendar = calendars.FirstOrDefault(t => t.Name == EmailAccount);

            if (appCalendar == null)
            {
                await Application.Current.MainPage.DisplayAlert("Advertencia!", "Se generó la cita en la app. Pero no se pueden crear citas en el calendario sin configurar el Email en Configuraciones.", "Ok");
                await Application.Current.MainPage.Navigation.PopAsync();
            }
            else
            {
                var appointmentTypes = string.Join(", ", appointment.AppointmentTypes.Select(t => t.Name));

                var androidAppointment = new AndroidAppointment
                {
                    Title = $"{appointmentTypes}: {appointment.UserInformation} Tel: {appointment.UserPhone}",
                    StartDate = appointment.AppointmentDate,
                    EndDate = appointment.AppointmentEnd,
                    Location = ConstantValues.APPOINTMENT_BRAND,
                    AppointmentTypes = appointmentTypes,
                    ReminderMinutes = 10,
                    CalendarID = Convert.ToInt32(appCalendar.ExternalID)
                };

                await DependencyService.Get<IDeviceCalendarService>().AddEventToCalendar(androidAppointment);
            }
        }

        public async Task LoadUsers(string searchText = "", Models.DataModels.User user = null)
        {
            Users.Clear();

            var users = await _dataService.GetUsersByType(UserTypeEnum.Paciente, searchText);
            users = users.OrderBy(t => t.LastName).ToList();

            foreach (var person in users)
            {
                Users.Add(person);
            }

            if (user != null)
            {
                var userFromList = users.FirstOrDefault(t => t.Id == user.Id);
                SelectedUser = userFromList;
            }

        }

        public async Task Initialize(Models.DataModels.User user = null)
        {
            await LoadUsers(user: user);
            await InitializeAppointmentTypes();

        }

        private async Task InitializeAppointmentTypes()
        {
            AppointmentTypes.Clear();
            FilterAppointmentTypes.Clear();

            var appointmentTypes = await _dataService.GetAppointmentTypes();

            if (appointmentTypes.Any())
            {
                FilterAppointmentTypes.Add(new Models.DataModels.AppointmentType
                {
                    Name = "Todos"
                });
            }

            foreach (var appointmentType in appointmentTypes)
            {
                AppointmentTypes.Add(appointmentType);
                FilterAppointmentTypes.Add(appointmentType);
            }

            if (appointmentTypes.Count > 3)
            {
                AppointmentTypesHeight = 125;
            }
            else
            {
                AppointmentTypesHeight = 75;
            }
        }

        #endregion

        public async Task LoadAppointment(int id)
        {
            if (id != 0) 
            {
                var appointment = await _dataService.GetAppointment(id);

                Id = appointment.Id;
                GivenDate = appointment.AppointmentDate.Date;
                GivenTime = appointment.AppointmentDate.TimeOfDay;
                var appointmentDuration = appointment.AppointmentEnd - appointment.AppointmentDate;
                var appointmentDurationEnum = (AppointmentDurationEnum) appointmentDuration.TotalMinutes;
                SelectedAppointmentDuration = AppointmentDurations.First(t => t.Name == appointmentDurationEnum);                

                var typesFromAppointment = AppointmentTypes.Where(t => appointment.AppointmentTypes.Any(u => u.Id == t.Data.Id)) ;

                foreach (var appointmentType in typesFromAppointment)
                {
                    appointmentType.IsSelected = true;
                }

                SelectedUser = Users.First(t => t.Id == appointment.UserId);
            }
        }
    }
}
