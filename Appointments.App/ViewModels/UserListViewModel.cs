using Appointments.App.Models;
using Appointments.App.Services;
using Appointments.App.Views.Appointment;
using Appointments.App.Views.Users;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace Appointments.App.ViewModels
{
    public class UserListViewModel : BasePageViewModel
    {
        #region Temp Properties
        private readonly IDataService _dataService;

        #endregion

        #region Properties
        public int Id { get; set; }
        public string PersonValue { get; set; }
        public DateTime GivenDate { get; set; }
        public ObservableCollection<User> People { get; set; } = new ObservableCollection<User>();

        #endregion
        public UserListViewModel() : base()
        {
            _dataService = new DataService();
        }

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
            
            var users = await _dataService.GetUsers();

            users = users.Where(t => t.UserType == UserType.Paciente).ToList();

            users = users.OrderBy(t => t.LastName).ToList();
            foreach (var person in users)
            {
                People.Add(person);
            }
        }

        #endregion
    }
}
