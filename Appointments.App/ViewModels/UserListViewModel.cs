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
            People = new ObservableCollection<User>();
        }
        #region Temp Properties
        private readonly IDataService _dataService;

        #endregion

        #region Properties
        private int _id;
        private string _personValue;
        private DateTime _givenDate;
        private ObservableCollection<User> _people;

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

        public ObservableCollection<User> People
        {
            get => _people;
            set => SetProperty(ref _people, value);
        }

        #endregion        

        #region Commands

        //SearchUserCommand
        public ICommand SearchUserCommand => new Command(async (item) =>  await SearchUser(item));
        public ICommand CreateUserCommand => new Command(async (item) => await CreateUser());

        private async Task SearchUser(object sender)
        {

            if (sender == null)
            {
                await InitializePeople();
            }
            else
            {

                var peopleSearched = People.Where(p =>
                    p.Identification.ToUpper().Contains(sender.ToString().ToUpper())
                    || p.Name.ToUpper().Contains(sender.ToString().ToUpper())
                    || p.LastName.ToUpper().Contains(sender.ToString().ToUpper())).OrderBy(t=> t.LastName).ToList();

                People.Clear();

                foreach (var person in peopleSearched)
                {
                    People.Add(person);
                }
            }
        }

        private async Task CreateUser()
        {
            await Application.Current.MainPage.Navigation.PushAsync(new CreateUserPage());
        }

        public async Task InitializePeople()
        {
            People.Clear();
            
            var users = await _dataService.GetUsersByType(UserType.Paciente);
            users = users.OrderBy(t => t.LastName).ToList();

            foreach (var person in users)
            {
                People.Add(person);
            }
        }

        #endregion
    }
}
