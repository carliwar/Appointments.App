using Acr.UserDialogs;
using Appointments.App.Models.DataModels;
using Appointments.App.Models.Enum;
using Appointments.App.Services;
using Appointments.App.Views.Appointments;
using Appointments.App.Views.UserAppointment;
using Appointments.App.Views.Users;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace Appointments.App.ViewModels
{
    public class UserListViewModel : BasePageViewModel
    {
        public UserListViewModel() : base()
        {
            _dataService = new DataService();
            Users = new ObservableCollection<Models.DataModels.User>();
        }
        #region Temp Properties
        private readonly IDataService _dataService;

        #endregion

        #region Properties
        private int _id;
        private string _userValue;
        private DateTime _givenDate;
        private ObservableCollection<Models.DataModels.User> _users;
        private ObservableCollection<Models.DataModels.AppointmentType> _appointmentTypes = new ObservableCollection<Models.DataModels.AppointmentType>();
        private Models.DataModels.AppointmentType _selectedAppointmentType;
        public int Id
        {
            get => _id;
            set => SetProperty(ref _id, value);
        }

        public string UserValue
        {
            get => _userValue;
            set => SetProperty(ref _userValue, value);
        }

        public DateTime GivenDate
        {
            get => _givenDate;
            set => SetProperty(ref _givenDate, value);
        }

        public ObservableCollection<Models.DataModels.User> Users
        {
            get => _users;
            set => SetProperty(ref _users, value);
        }
        public ObservableCollection<Models.DataModels.AppointmentType> AppointmentTypes
        {
            get => _appointmentTypes;
            set => SetProperty(ref _appointmentTypes, value);
        }
        public Models.DataModels.AppointmentType SelectedAppointmentType
        {
            get => _selectedAppointmentType;
            set 
            {
                SetProperty(ref _selectedAppointmentType, value);
                
                var searchValue = string.Empty;

                if (SelectedAppointmentType.Id != 0)
                {
                    searchValue = SelectedAppointmentType.Name;
                }

                InitializeUsers(searchValue).ConfigureAwait(false);
            }
        }
        #endregion        

        #region Commands

        //SearchUserCommand
        public ICommand SearchUserCommand => new Command(async (item) =>  await SearchUserAsync(item));
        public ICommand CreateUserCommand => new Command(async (item) => await CreateUser());
        public ICommand UserAppointmentsCommand => new Command(async (item) => await LoadUserAppointments(item));

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

        public async Task InitializeUsers(string searchText = "")
        {
            UserDialogs.Instance.ShowLoading();
            Users.Clear();

            try
            {
                var users = await _dataService.GetUsersByType(UserTypeEnum.Paciente, searchText);
                users = users.OrderBy(t => t.LastName).ToList();

                foreach (var user in users)
                {
                    Users.Add(user);
                }
            }
            catch (Exception e)
            {
                UserDialogs.Instance.HideLoading();
                await Application.Current.MainPage.DisplayAlert("Error", $"Contacte al administrador: {e.Message}", "Ok");
            }

            UserDialogs.Instance.Loading().Hide();
        }

        private async Task CreateUser()
        {
            await Application.Current.MainPage.Navigation.PushAsync(new UserDetailPage(0));
        }

        private async Task LoadUserAppointments(object user)
        {
            if(user is Models.DataModels.User)
            {
                await Application.Current.MainPage.Navigation.PushAsync(new UserAppointmentsPage((Models.DataModels.User)user));
            }
            
        }
        public async Task InitializeAppointmentTypes()
        {
            if (Users.Any())
            {                
                AppointmentTypes.Clear();

                AppointmentTypes.Add(new Models.DataModels.AppointmentType
                {
                    Name = "Todos"
                });

                var appointmentTypes = Users.Select(t => t.AppointmentType).Distinct().ToList();

                foreach (var appointmentType in appointmentTypes)
                {
                    if (appointmentType != null && !AppointmentTypes.Any(t => t.Id == appointmentType.Id))
                    {
                        AppointmentTypes.Add(appointmentType);
                    }                    
                }
            }
            
        }
        #endregion
    }
}
