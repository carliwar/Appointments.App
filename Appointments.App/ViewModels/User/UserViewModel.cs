using Acr.UserDialogs;
using Appointments.App.Models.DataModels;
using Appointments.App.Models.Enum;
using Appointments.App.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.MultiSelectListView;

namespace Appointments.App.ViewModels.User
{
    public class UserViewModel : BasePageViewModel
    {
        public UserViewModel()
        {
            //UserTypes = new ObservableCollection<UserType>(Enum.GetValues(typeof(UserType)).OfType<UserType>().ToList());
            _dataService = new DataService();
        }

        #region Properties
        private int _id;
        private string _identification;
        private string _phone;
        private string _firstName;
        private string _lastName;
        private string _email;
        private DateTime _birthDate = DateTime.Today;
        private readonly IDataService _dataService;
        private UserTypeEnum _selectedUserType;
        private bool _isImported = false;
        private bool _isEdit = false;
        private ObservableCollection<Models.DataModels.AppointmentType> _appointmentTypes = new ObservableCollection<Models.DataModels.AppointmentType>();
        private Models.DataModels.AppointmentType _selectedAppointmentType;


        public int Id
        {
            get => _id;
            set => SetProperty(ref _id, value);
        }
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
        public string Email
        {
            get => _email;
            set => SetProperty(ref _email, value);
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

        public bool IsEdit
        {
            get => _isEdit;
            set => SetProperty(ref _isEdit, value);
        }

        public ObservableCollection<Models.DataModels.AppointmentType> AppointmentTypes
        {
            get => _appointmentTypes;
            set => SetProperty(ref _appointmentTypes, value);
        }

        public Models.DataModels.AppointmentType SelectedAppointmentType
        {
            get => _selectedAppointmentType;
            set => SetProperty(ref _selectedAppointmentType, value);
        }
        #endregion

        #region Commands
        public ICommand SaveUserCommand => new Command((item) => SaveUser(item));
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

            var contact = await Contacts.PickContactAsync();
            if (contact != null)
            {
                FirstName = contact.GivenName;
                LastName = contact.FamilyName;
                Phone = contact.Phones.FirstOrDefault()?.PhoneNumber?.Replace(" ", "");
                IsImported = true;
                Email = contact.Emails?.FirstOrDefault().EmailAddress;
            }
        }
        private void SelectUserType(object item)
        {
            SelectedUserType = (UserTypeEnum)item;
        }

        private async void SaveUser(object item)
        {
            var user = new Models.DataModels.User
            {
                Id = Id,
                Identification = Identification,
                Name = FirstName,
                LastName = LastName,
                BirthDate = BirthDate,
                Email = Email,
                Phone = FormatPhone(Phone),
                UserType = UserTypeEnum.Paciente,
                AppointmentType = SelectedAppointmentType,
                AppointmentTypeId = SelectedAppointmentType?.Id

            };

            UserDialogs.Instance.ShowLoading();

            var result = await _dataService.SaveUser(user);

            try
            {
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
                        if (!IsImported && Id == 0)
                        {
                            var status = await Permissions.CheckStatusAsync<Permissions.ContactsWrite>();

                            if (status != PermissionStatus.Granted)
                            {
                                UserDialogs.Instance.HideLoading();

                                var request = await Permissions.RequestAsync<Permissions.ContactsWrite>();

                                if (request != PermissionStatus.Granted)
                                {
                                    await Application.Current.MainPage.DisplayAlert("Error:", "No se puede crear contactos. Por favor, agrega el permiso desde la Configuración > Apps.", "Ok");
                                }

                            }

                            UserDialogs.Instance.ShowLoading();
                            DependencyService.Get<IDeviceContactService>().CreateContact(deviceContact);
                        }
                    }
                    catch (Exception ex)
                    {
                        UserDialogs.Instance.HideLoading();

                        await Application.Current.MainPage.DisplayAlert("Alerta: ", "Creado correctamente en la App pero no en el Dispositivo.", "Ok");
                        await Application.Current.MainPage.Navigation.PopAsync();
                    }

                    UserDialogs.Instance.HideLoading();

                    await Application.Current.MainPage.DisplayAlert("Operación Exitosa!", "Paciente guardado.", "Ok");
                    await Application.Current.MainPage.Navigation.PopAsync();
                }
                else
                {
                    UserDialogs.Instance.HideLoading();

                    await Application.Current.MainPage.DisplayAlert("Errores: ", string.Join(" / ", result.Errors), "Ok");
                }
            }
            catch (Exception e)
            {
                UserDialogs.Instance.HideLoading();
                await Application.Current.MainPage.DisplayAlert("Error", $"Contacte al administrador: {e.Message}", "Ok");
            }

            UserDialogs.Instance.HideLoading();
        }

        private string FormatPhone(string phone)
        {
            if (phone == null)
                return string.Empty;

            string formattedString = phone.Trim();

            if (formattedString.Length > 0)
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

        public async Task LoadUser(int id)
        {
            if(id != 0)
            {
                UserDialogs.Instance.ShowLoading();

                var user = await _dataService.GetUser(id);

                if (user != null)
                {
                    Id = user.Id;
                    Identification = user.Identification;
                    FirstName = user.Name;
                    LastName = user.LastName;
                    Email = user.Email;
                    Phone = user.Phone;
                    BirthDate = user.BirthDate ?? DateTime.UtcNow;
                    SelectedUserType = user.UserType;

                    if (user.AppointmentType != null)
                    {
                        SelectedAppointmentType = AppointmentTypes.Single(t => t.Id == user.AppointmentType.Id);
                    }

                    IsEdit = true;
                }

                UserDialogs.Instance.HideLoading();
            }
        }

        public async Task InitializeAppointmentTypes()
        {
            var appointmentTypes = await _dataService.GetAppointmentTypes();

            foreach (var appointmentType in appointmentTypes)
            {
                AppointmentTypes.Add(appointmentType);
            }
        }
        #endregion

        #region Private Methods


        #endregion
    }
}
