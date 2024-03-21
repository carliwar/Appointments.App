using Appointments.App.Models.Enum;
using Appointments.App.Services;
using System.Windows.Input;
using Communication = Microsoft.Maui.ApplicationModel.Communication;

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
        private UserTypeEnum _selectedUserType;
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

        public UserTypeEnum SelectedUserType
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
            var status = await Permissions.CheckStatusAsync<Permissions.ContactsRead>();
            
            if (status != PermissionStatus.Granted)
            {
                var request = await Permissions.RequestAsync<Permissions.ContactsRead>();

                if (request != PermissionStatus.Granted)
                {
                    await Application.Current.MainPage.DisplayAlert("Error:", "No se puede acceder a los contactos. Por favor, agrega el permiso desde la Configuración > Apps.", "Ok");
                }
                    
            }
            
            var contact = await Communication.Contacts.Default.PickContactAsync();
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
            SelectedUserType = (UserTypeEnum)item;
        }

        private async void CreateUser(object item)
        {
            var user = new Models.DataModels.User
            {
                Identification = Identification,
                Name = FirstName,
                LastName = LastName,
                BirthDate = BirthDate,
                Phone = FormatPhone(Phone),
                UserType = UserTypeEnum.Paciente
            };
            var result = await _dataService.SaveUser(user);

            if (result.Success)
            {
                //create user as a new phone contact
                var deviceContact = new Contact
                {
                    NamePrefix = UserTypeEnum.Paciente.ToString(),
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
                        var status = await Permissions.CheckStatusAsync<Permissions.ContactsWrite>();

                        if (status != PermissionStatus.Granted)
                        {
                            var request = await Permissions.RequestAsync<Permissions.ContactsWrite>();

                            if(request != PermissionStatus.Granted)
                            {
                                await Application.Current.MainPage.DisplayAlert("Error:", "No se puede crear contactos. Por favor, agrega el permiso desde la Configuración > Apps.", "Ok");
                            }
                            
                        }

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
            if (phone == null)
                return string.Empty;

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
