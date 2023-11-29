using Appointments.App.Models;
using Appointments.App.Services;
using System;
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
            //Initialize Dictionary with all values from enum AppointmentType
            _dataService = new DataService();
            Types = new ObservableCollection<AppointmentType>(Enum.GetValues(typeof(AppointmentType)).OfType<AppointmentType>().ToList());
            Users = new ObservableCollection<User>();
            Task.Run(() => InitializeUsers()).Wait();
        }
        #region Temp Properties

        private readonly IDataService _dataService;        
        #endregion

        #region Properties

        private int _id;
        private string _userValue;
        private DateTime _givenDate;
        private ObservableCollection<User> _users = new ObservableCollection<User>();
        private ObservableCollection<AppointmentType> _types = new ObservableCollection<AppointmentType>();
        private AppointmentType _selectedType;      

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
        public AppointmentType SelectedType
        {
            get => _selectedType;
            set => SetProperty(ref _selectedType, value);
        }
        #endregion
       
        #region Commands

        //SearchUserCommand
        public ICommand SearchUserCommand => new Command(async (item) =>  await SearchUserAsync(item));

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
            Users.Clear();

            var users = await _dataService.GetUsersByType(UserType.Paciente, searchText);
            users = users.OrderBy(t => t.LastName).ToList();

            foreach (var person in users)
            {
                Users.Add(person);
            }
        }

        #endregion
    }
}
