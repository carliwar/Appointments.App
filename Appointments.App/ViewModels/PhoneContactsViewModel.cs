using Appointments.App.Models.DataModels;
using Appointments.App.Models.Enum;
using Appointments.App.Services;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace Appointments.App.ViewModels
{
    public class PhoneContactsViewModel : BasePageViewModel
    {
        public PhoneContactsViewModel() : base()
        {
            _dataService = new DataService();
            Users = new ObservableCollection<User>();
        }
        #region Temp Properties
        private readonly IDataService _dataService;

        #endregion

        #region Properties
        private int _id;
        private string _userValue;
        private DateTime _givenDate;
        private ObservableCollection<User> _users;

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

        #endregion        

        #region Commands

        //SearchUserCommand
        public ICommand SearchUserCommand => new Command(async (item) => await SearchUserAsync(item));        

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

            var contacts = await Contacts.PickContactAsync();
            

            //if(contacts != null)
            //{
            //    if (!string.IsNullOrWhiteSpace(searchText))
            //    {
            //        contacts = contacts.Where(t => (t.FamilyName != null && t.FamilyName.ToLower().Contains(searchText))
            //        || (t.GivenName != null && t.GivenName.ToLower().Contains(searchText))
            //        || (t.Phones.Any() && t.Phones.Any(u => u.PhoneNumber.Contains(searchText))));
            //    }

            //    foreach (var contact in contacts)
            //    {
            //        Users.Add(new User
            //        {
            //            Phone = contact.Phones.FirstOrDefault()?.PhoneNumber,
            //            Name = contact.GivenName,
            //            LastName = contact.FamilyName,
            //            UserType = UserType.Paciente
            //        });
            //    }
            //}
        }
        #endregion
    }

}
