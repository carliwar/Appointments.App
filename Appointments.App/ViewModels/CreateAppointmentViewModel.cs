using Appointments.App.Models.DataModels;
using Appointments.App.Models.Enum;
using Appointments.App.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace Appointments.App.ViewModels
{
    public class CreateAppointmentViewModel : BasePageViewModel
    {
        public CreateAppointmentViewModel() : base()
        {
            _dataService = new DataService();
            Types = new ObservableCollection<AppointmentType>(Enum.GetValues(typeof(AppointmentType)).OfType<AppointmentType>().ToList());
            Users = new ObservableCollection<User>();
            AppointmentDurations = new ObservableCollection<AppointmentDuration>(Enum.GetValues(typeof(AppointmentDuration)).OfType<AppointmentDuration>().ToList());            
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
        private ObservableCollection<AppointmentType> _types = new ObservableCollection<AppointmentType>();
        private ObservableCollection<AppointmentDuration>  _appointmentDurations = new ObservableCollection<AppointmentDuration>();
        private AppointmentType? _selectedType;
        private AppointmentDuration? _selectedAppointmentDuration;
        private User _selectedUser;
        private bool _showError = false;

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

        public ObservableCollection<AppointmentType> Types
        {
            get => _types;
            set => SetProperty(ref _types, value);
        }

        public ObservableCollection<AppointmentDuration> AppointmentDurations
        {
            get => _appointmentDurations;
            set => SetProperty(ref _appointmentDurations, value);
        }

        public AppointmentType? SelectedType
        {
            get => _selectedType;
            set => SetProperty(ref _selectedType, value);
        }
        public AppointmentDuration? SelectedAppointmentDuration
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
        #endregion

        #region Commands
        //SearchUserCommand
        public ICommand SearchUserCommand => new Command(async (item) => await SearchUserAsync(item));
        public ICommand CreateAppointmentCommand => new Command(async (item) => await CreateAppointment(item));

        private async Task SearchUserAsync(object sender)
        {

            if (sender == null)
            {
                await InitializeUsers();
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

                await InitializeUsers(searchText);
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
                AppointmentEnd = GivenDate.Date.Add(GivenTime).AddMinutes((double)SelectedAppointmentDuration),
                UserInformation = SelectedUser.UserFullName,
                AppointmentType = SelectedType,
            };

            var result = await _dataService.CreateValidatedAppointment(appointment);

            if (result != null)
            {
                if (result.Success)
                {

                    await Application.Current.MainPage.DisplayAlert("Operación Exitosa!", "Cita agendada", "Ok");
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

        public async Task InitializeUsers(string searchText = "", User user = null)
        {            
            Users.Clear();

            var users = await _dataService.GetUsersByType(UserType.Paciente, searchText);
            users = users.OrderBy(t => t.LastName).ToList();

            foreach (var person in users)
            {
                Users.Add(person);
            }

            if(user != null)
            {
                var userFromList = users.FirstOrDefault(t => t.Id == user.Id);
                SelectedUser = userFromList;
            }
            
        }
        #endregion
    }
}
