using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Acr.UserDialogs;
using Appointments.App.Models.Enum;
using Appointments.App.Services;
using Appointments.App.Services.VMServices;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace Appointments.App.ViewModels.User
{
    public class UserViewModel : BasePageViewModel
    {
        public UserViewModel(IUserService userService)
        {
            _dataService = new DataService();
            _userService = userService;
        }

        #region Properties
        private int _id;
        private string _contificoId;
        private string _deviceContactId;
        private string _identification;
        private string _phone;
        private string _firstName;
        private string _lastName;
        private string _email;
        private DateTime _birthDate = DateTime.Today;
        private readonly IDataService _dataService;
        private readonly IUserService _userService;
        private UserTypeEnum _selectedUserType;
        private bool _isImported = false;
        private bool _isEdit = false;
        private ObservableCollection<Models.DataModels.AppointmentType> _appointmentTypes =
            new ObservableCollection<Models.DataModels.AppointmentType>();
        private Models.DataModels.AppointmentType _selectedAppointmentType;

        public int Id
        {
            get => _id;
            set => SetProperty(ref _id, value);
        }
        public string ContificoId
        {
            get => _contificoId;
            set => SetProperty(ref _contificoId, value);
        }
        public string DeviceContactId
        {
            get => _deviceContactId;
            set => SetProperty(ref _deviceContactId, value);
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
        public ICommand ImportContactCommand =>
            new Command(async (item) => await ImportContactAsync(item));

        private async Task ImportContactAsync(object item)
        {
            var status = await Permissions.CheckStatusAsync<Permissions.ContactsRead>();

            if (status != PermissionStatus.Granted)
            {
                var request = await Permissions.RequestAsync<Permissions.ContactsRead>();

                if (request != PermissionStatus.Granted)
                {
                    await Application.Current.MainPage.DisplayAlert(
                        "Error:",
                        "No se puede acceder a los contactos. Por favor, agrega el permiso desde la Configuración > Apps.",
                        "Ok"
                    );
                }
            }

            try
            {
                var contact = await Contacts.PickContactAsync();
                if (contact != null)
                {
                    DeviceContactId = contact.Id;
                    FirstName = contact.GivenName;
                    LastName = contact.FamilyName;
                    Phone = contact.Phones.FirstOrDefault()?.PhoneNumber?.Replace(" ", "");
                    IsImported = true;
                    Email = contact.Emails?.FirstOrDefault()?.EmailAddress;
                }
            }
            catch (TaskCanceledException)
            {

            }
            catch (Exception)
            {
                await Application.Current.MainPage.DisplayAlert("Error:", "Contacte al administrador.", "Ok");
                throw;
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
                DeviceContactId = DeviceContactId,
                ContificoId = ContificoId,
                Name = FirstName,
                LastName = LastName,
                BirthDate = BirthDate,
                Email = Email,
                Phone = Phone,
                UserType = UserTypeEnum.Paciente,
                AppointmentType = SelectedAppointmentType,
                AppointmentTypeId = SelectedAppointmentType?.Id
            };

            UserDialogs.Instance.ShowLoading();            

            try
            {
                var result = await _userService.SaveUser(user);

                if (result.Success)
                {
                    await Application.Current.MainPage.DisplayAlert(
                        "Operación Exitosa!",
                        "Usuario creado",
                        "Ok"
                    );
                    await Application.Current.MainPage.Navigation.PopAsync();
                }
                else
                {
                    UserDialogs.Instance.HideLoading();

                    await Application.Current.MainPage.DisplayAlert(
                        "Errores: ",
                        string.Join(" / ", result.Errors),
                        "Ok"
                    );
                }
            }
            catch (Exception e)
            {
                UserDialogs.Instance.HideLoading();
                await Application.Current.MainPage.DisplayAlert(
                    "Error",
                    $"Contacte al administrador: {e.Message}",
                    "Ok"
                );
            }

            UserDialogs.Instance.HideLoading();
        }

        public async Task LoadUser(int id)
        {
            if (id != 0)
            {
                UserDialogs.Instance.ShowLoading();

                var user = await _userService.GetUser(id);

                if (user != null)
                {
                    Id = user.Id;
                    DeviceContactId = user.DeviceContactId;
                    ContificoId = user.ContificoId;
                    Identification = user.Identification;
                    FirstName = user.Name;
                    LastName = user.LastName;
                    Email = user.Email;
                    Phone = user.Phone;
                    BirthDate = user.BirthDate ?? DateTime.UtcNow;
                    SelectedUserType = user.UserType;

                    if (user.AppointmentType != null)
                    {
                        SelectedAppointmentType = AppointmentTypes.Single(t =>
                            t.Id == user.AppointmentType.Id
                        );
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
    }
}
