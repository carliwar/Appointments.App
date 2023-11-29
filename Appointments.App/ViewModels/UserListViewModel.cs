using Appointments.App.Models;
using Appointments.App.Services;
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
            Users = new ObservableCollection<User>();
        }
        #region Temp Properties
        private readonly IDataService _dataService;

        #endregion

        #region Properties
        private int _id;
        private string _personValue;
        private DateTime _givenDate;
        private ObservableCollection<User> _users;

        public int Id
        {
            get => _id;
            set => SetProperty(ref _id, value);
        }

        public string PersonValue
        {
            get => _personValue;
            set => SetProperty(ref _personValue, value);
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

        #endregion        

        #region Commands

        //SearchUserCommand
        public ICommand SearchUserCommand => new Command(async (item) =>  await SearchUserAsync(item));
        public ICommand CreateUserCommand => new Command(async (item) => await CreateUser());

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

        private async Task CreateUser()
        {
            await Application.Current.MainPage.Navigation.PushAsync(new CreateUserPage());
        }

        #endregion
    }
}
