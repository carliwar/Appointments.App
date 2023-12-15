using Appointments.App.Models.DataModels;
using Appointments.App.Models.Enum;
using Appointments.App.Services;
using Appointments.App.Views.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace Appointments.App.ViewModels
{
    public class CreateUserRequestViewModel : BasePageViewModel
    {
        public CreateUserRequestViewModel()
        {
            //UserTypes = new ObservableCollection<UserType>(Enum.GetValues(typeof(UserType)).OfType<UserType>().ToList());
            _dataService = new DataService();
        }

        #region Properties
        private string _identification;
        private string _phone;
        private string _firstName;
        private string _lastName;
        private DateTime _birthDate = DateTime.Today;
        private readonly IDataService _dataService;
        private UserType _selectedUserType;
        private bool _isImported = false;



        public string Identification
        {
            get => _identification;
            set => SetProperty(ref _identification, value);
        }
        public string Phone
        {
            get => _phone;
            set => SetProperty(ref _phone, value);
        }

        public string FirstName
        {
            get => _firstName;
            set => SetProperty(ref _firstName, value);
        }
        public string LastName
        {
            get => _lastName;
            set => SetProperty(ref _lastName, value);
        }
        public DateTime BirthDate
        {
            get => _birthDate;
            set => SetProperty(ref _birthDate, value);
        }

        public UserType SelectedUserType
        {
            get => _selectedUserType;
            set => SetProperty(ref _selectedUserType, value);
        }

        public bool IsImported
        {
            get => _isImported;
            set => SetProperty(ref _isImported, value);
        }
        #endregion

        #region Commands
        public ICommand CreateUserCommand => new Command((item) => CreateUser(item));
        public ICommand SelectUserTypeCommand => new Command((item) => SelectUserType(item));
        public ICommand ImportContactCommand => new Command(async (item) => await ImportContactAsync(item));

        private async Task ImportContactAsync(object item)
        {
            var contact = await Contacts.PickContactAsync();
            if (contact != null)
            {
                FirstName = contact.GivenName;
                LastName = contact.FamilyName;
                Phone = contact.Phones.FirstOrDefault()?.PhoneNumber?.Replace(" ", "");
                IsImported = true;
            }
        }
        private void SelectUserType(object item)
        {
            SelectedUserType = (UserType)item;
        }

        private async void CreateUser(object item)
        {
            var user = new User
            {
                Identification = Identification,
                Name = FirstName,
                LastName = LastName,
                BirthDate = BirthDate,
                Phone = FormatPhone(Phone),
                UserType = UserType.Paciente
            };
            var result = await _dataService.CreateValidatedUser(user);

            if (result.Success)
            {
                //create user as a new phone contact
                var deviceContact = new Contact
                {
                    GivenName = FirstName,
                    FamilyName = LastName,
                    Phones = new List<ContactPhone>
                    {
                        new ContactPhone
                        {
                            PhoneNumber = user.Phone
                        }
                    }
                };

                try
                {
                    if (!IsImported)
                    {
                        DependencyService.Get<IDeviceContactService>().CreateContact(deviceContact);
                    }
                }
                catch (Exception ex)
                {
                    await Application.Current.MainPage.DisplayAlert("Alerta: ", "Creado correctamente en la App pero no en el Dispositivo.", "Ok");
                    await Application.Current.MainPage.Navigation.PopAsync();
                }

                await Application.Current.MainPage.DisplayAlert("Operación Exitosa!", "Usuario creado", "Ok");
                await Application.Current.MainPage.Navigation.PopAsync();
            }
            else
            {
                await Application.Current.MainPage.DisplayAlert("Errores: ", string.Join(" / ", result.Errors), "Ok");
            }
        }


        private string FormatPhone(string phone)
        {
            string formattedString = phone.Trim();

            if(formattedString.Length > 0)
            {
                // if phone starts with 09 replace that with +5939
                if (phone.StartsWith("09"))
                {
                    formattedString = "+5939" + phone.Substring(2);
                    Console.WriteLine(formattedString);
                }
            }            
            return formattedString;
        }


        #endregion

        #region Private Methods


        #endregion
    }
}
