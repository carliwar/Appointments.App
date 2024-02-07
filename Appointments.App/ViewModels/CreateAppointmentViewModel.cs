using Appointments.App.Models.DataModels;
using Appointments.App.Models.Enum;
using Appointments.App.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.MultiSelectListView;

namespace Appointments.App.ViewModels
{
    public class CreateAppointmentViewModel : BasePageViewModel
    {
        public CreateAppointmentViewModel() : base()
        {
            _dataService = new DataService();

            Types = new ObservableCollection<AppointmentTypeEnum>(Enum.GetValues(typeof(AppointmentTypeEnum)).OfType<AppointmentTypeEnum>().ToList());


            foreach (AppointmentDurationEnum enumValue in Enum.GetValues(typeof(Models.Enum.AppointmentDurationEnum)))
            {
                string customString = GetEnumDescription(enumValue);

                var appointmentDuration = new Models.DataModels.AppointmentDuration
                {
                    Name = enumValue,
                    Description = customString
                };
                AppointmentDurations.Add(appointmentDuration);
            }

            Users = new ObservableCollection<User>();            
        }
        #region Temp Properties

        private readonly IDataService _dataService;
        #endregion

        #region Properties

        private int _id;
        private string _userFullName;
        private DateTime _givenDate;
        private TimeSpan _givenTime;
        private ObservableCollection<User> _users = new ObservableCollection<User>();
        private ObservableCollection<AppointmentTypeEnum> _types = new ObservableCollection<AppointmentTypeEnum>();
        private ObservableCollection<AppointmentDuration> _appointmentDurations = new ObservableCollection<AppointmentDuration>();
        private MultiSelectObservableCollection<Models.DataModels.AppointmentType> _appointmentTypes = new MultiSelectObservableCollection<Models.DataModels.AppointmentType>();
        private ObservableCollection<Models.DataModels.AppointmentType> _selectedAppointmentTypes = new ObservableCollection<Models.DataModels.AppointmentType>();
        private AppointmentTypeEnum? _selectedType;
        private AppointmentDuration _selectedAppointmentDuration;
        private User _selectedUser;
        private bool _showError = false;
        private int _appointmentTypesHeight;

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

        public ObservableCollection<User> Users
        {
            get => _users;
            set => SetProperty(ref _users, value);
        }

        public ObservableCollection<AppointmentTypeEnum> Types
        {
            get => _types;
            set => SetProperty(ref _types, value);
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

        public ObservableCollection<Models.DataModels.AppointmentType> SelectedAppointmentTypes
        {
            get => _selectedAppointmentTypes;
            set => SetProperty(ref _selectedAppointmentTypes, value);
        }

        public AppointmentTypeEnum? SelectedType
        {
            get => _selectedType;
            set => SetProperty(ref _selectedType, value);
        }
        public AppointmentDuration SelectedAppointmentDuration
        {
            get => _selectedAppointmentDuration;
            set => SetProperty(ref _selectedAppointmentDuration, value);
        }
        public User SelectedUser
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
        #endregion

        #region Commands
        //SearchUserCommand
        public ICommand SearchUserCommand => new Command(async (item) => await SearchUserAsync(item));
        public ICommand CreateAppointmentCommand => new Command(async (item) => await CreateAppointment(item));
        public ICommand FillSelectedAppointmentTypesCommand => new Command(async (item) => await FillSelectedAppointmentTypes(item));

        private async Task SearchUserAsync(object sender)
        {

            if (sender == null)
            {
                await LoadUsers();
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

                await LoadUsers(searchText);
            }
        }

        private async Task CreateAppointment(object sender)
        {
            if (SelectedType == null || SelectedUser == null)
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
                AppointmentType = SelectedType,
                Attended = true
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
            if(sender is Models.DataModels.AppointmentType selectedAppointmentType)
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
        private static async Task CreateDeviceAppointment(Appointment appointment)
        {
            var androidAppointment = new AndroidAppointment
            {
                Title = $"{appointment.AppointmentType}: {appointment.UserInformation} Tel: {appointment.UserPhone}",
                StartDate = appointment.AppointmentDate,
                EndDate = appointment.AppointmentEnd,
                Location = ConstantValues.APPOINTMENT_BRAND,
                AppointmentType = appointment.AppointmentType,
                ReminderMinutes = 10
            };

            await DependencyService.Get<IDeviceCalendarService>().AddEventToCalendar(androidAppointment);
        }

        public async Task LoadUsers(string searchText = "", User user = null)
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

        public async Task Initialize(User user = null)
        {
            await LoadUsers(user: user);
            await InitializeAppointmentTypes();

        }

        private async Task InitializeAppointmentTypes()
        {
            var appointmentTypes = await _dataService.GetAppointmentTypes();

            foreach(var appointmentType in appointmentTypes)
            {
                AppointmentTypes.Add(appointmentType);
            }

            if(appointmentTypes.Count > 3)
            {
                AppointmentTypesHeight = 125;
            }
            else
            {
                AppointmentTypesHeight = 75;
            }
        }

        #endregion

        private string GetEnumDescription(Models.Enum.AppointmentDurationEnum value)
        {
            var fieldInfo = value.GetType().GetField(value.ToString());

            var attributes = (DescriptionAttribute[])fieldInfo.GetCustomAttributes(
                typeof(DescriptionAttribute), false);

            return attributes.Length > 0 ? attributes[0].Description : value.ToString();
        }
    }

}