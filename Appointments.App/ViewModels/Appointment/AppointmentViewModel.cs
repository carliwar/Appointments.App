using Appointments.App.Models.DataModels;
using Appointments.App.Models.Enum;
using Appointments.App.Services;
using Plugin.Calendars;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Essentials;
using Xamarin.Forms.MultiSelectListView;
using Xamarin.Forms;
using Plugin.Calendars.Abstractions;
using Acr.UserDialogs;
using Appointments.App.Models;

namespace Appointments.App.ViewModels.Appointments
{
    public class AppointmentViewModel : BasePageViewModel
    {
        readonly System.Globalization.CultureInfo cultureInfo = new System.Globalization.CultureInfo("es-EC");
        public AppointmentViewModel() : base()
        {
            _dataService = new DataService();
            Users = new ObservableCollection<Models.DataModels.User>();
        }
        #region Temp Properties

        private readonly IDataService _dataService;
        #endregion

        #region Properties

        private int _id;
        private string _userFullName;
        private DateTime _givenDate;
        private TimeSpan _givenTime;
        private TimeSpan _endTime;
        private ObservableCollection<Models.DataModels.User> _users = new ObservableCollection<Models.DataModels.User>();
        private MultiSelectObservableCollection<Models.DataModels.AppointmentType> _appointmentTypes = new MultiSelectObservableCollection<Models.DataModels.AppointmentType>();
        private ObservableCollection<Models.DataModels.AppointmentType> _filterAppointmentTypes = new ObservableCollection<Models.DataModels.AppointmentType>();
        private ObservableCollection<Models.DataModels.AppointmentType> _selectedAppointmentTypes = new ObservableCollection<Models.DataModels.AppointmentType>();
        private Models.DataModels.User _selectedUser;
        private bool _showError = false;
        private bool _isEdit = false;
        private int _appointmentTypesHeight;
        private int _appointmentHours;
        private int _appointmentMinutes;
        private Models.DataModels.AppointmentType _selectedAppointmentTypeFilter;

        public int Id
        {
            get => _id;
            set => SetProperty(ref _id, value);
        }

        public string UserFullName
        {
            get => _userFullName;
            set => SetProperty(ref _userFullName, value);
        }

        public string ScreenTitle
        {
            get
            {
                return $"Datos del {ConstantValues.USER_DENOMINATION}";
            }
        }

        public string SearchPlaceholder
        {
            get
            {
                return $"Buscar por {ConstantValues.USER_DENOMINATION}";
            }
        }

        public DateTime GivenDate
        {
            get => _givenDate;
            set => SetProperty(ref _givenDate, value);
        }

        public TimeSpan GivenTime
        {
            get => _givenTime;
            set
            {
                SetProperty(ref _givenTime, value);
                if (SelectedAppointmentTypes.Any())
                {
                    var maxAppointmentDuration = SelectedAppointmentTypes.Max(u => u.DefaultDuration);
                    AssignAppointmentDurationFromEnum(maxAppointmentDuration);
                }
                else
                {
                    EndTime = GivenTime.Add(TimeSpan.FromMinutes(30));
                }
            }
        }

        public TimeSpan EndTime
        {
            get => _endTime;
            set => SetProperty(ref _endTime, value);
        }

        public ObservableCollection<Models.DataModels.User> Users
        {
            get => _users;
            set => SetProperty(ref _users, value);
        }
        public MultiSelectObservableCollection<Models.DataModels.AppointmentType> AppointmentTypes
        {
            get => _appointmentTypes;
            set => SetProperty(ref _appointmentTypes, value);
        }

        public ObservableCollection<Models.DataModels.AppointmentType> FilterAppointmentTypes
        {
            get => _filterAppointmentTypes;
            set => SetProperty(ref _filterAppointmentTypes, value);
        }

        public ObservableCollection<Models.DataModels.AppointmentType> SelectedAppointmentTypes
        {
            get => _selectedAppointmentTypes;
            set => SetProperty(ref _selectedAppointmentTypes, value);
        }
        public Models.DataModels.User SelectedUser
        {
            get => _selectedUser;
            set
            {
                SetProperty(ref _selectedUser, value);
            }
        }
        public bool ShowError
        {
            get => _showError;
            set => SetProperty(ref _showError, value);
        }

        public int AppointmentTypesHeight
        {
            get => _appointmentTypesHeight;
            set => SetProperty(ref _appointmentTypesHeight, value);
        }

        public int AppointmentHours
        {
            get => _appointmentHours;
            set => SetProperty(ref _appointmentHours, value);
        }

        public int AppointmentMinutes
        {
            get => _appointmentMinutes;
            set => SetProperty(ref _appointmentMinutes, value);
        }
        public Models.DataModels.AppointmentType SelectedAppointmentTypeFilter
        {
            get => _selectedAppointmentTypeFilter;
            set
            {
                SetProperty(ref _selectedAppointmentTypeFilter, value);

                var searchValue = string.Empty;

                if (SelectedAppointmentTypeFilter.Id != 0)
                {
                    searchValue = SelectedAppointmentTypeFilter.Name;
                }

                SearchUserAsync(searchValue);
            }
        }

        public bool IsEdit
        {
            get => _isEdit;
            set => SetProperty(ref _isEdit, value);
        }

        public string EmailAccount { get; set; }
        public string BrandName { get; set; }
        #endregion

        #region Commands
        //SearchUserCommand
        public ICommand SearchUserCommand => new Command((item) => SearchUserAsync(item));
        public ICommand SaveAppointmentCommand => new Command(async (item) => await SaveAppointment(item));
        public ICommand DeleteAppointmentCommand => new Command(async () => await DeleteAppointmentAction());
        public ICommand FillSelectedAppointmentTypesCommand => new Command(async (item) => await FillSelectedAppointmentTypes(item));

        private void SearchUserAsync(object sender)
        {

            if (sender == null)
            {
                Task.Run(() => LoadUsers()).Wait();
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

                Task.Run(() => LoadUsers(searchText)).Wait();
            }
        }

        private async Task SaveAppointment(object sender)
        {
            ValidateRequired();

            if (ShowError)
            {
                return;
            }

            var appointmentDuration = (AppointmentHours * 60) + AppointmentMinutes;

            var appointment = new Appointment
            {
                Id = Id,
                UserId = SelectedUser.Id,
                AppointmentDate = GivenDate.Date.Add(GivenTime),
                AppointmentEnd = GivenDate.Date.Add(EndTime),
                UserInformation = SelectedUser.UserFullName,
                Attended = true,
                AppointmentTypes = SelectedAppointmentTypes?.ToList(),
                UserPhone = SelectedUser.Phone
            };

            UserDialogs.Instance.ShowLoading();

            try
            {
                var result = await _dataService.CreateValidatedAppointment(appointment);

                if (result != null)
                {
                    if (result.Success)
                    {                        
                        await SendAppointmentNotification(appointment);

                        var statusRead = await Permissions.CheckStatusAsync<Permissions.CalendarRead>();
                        var statusWrite = await Permissions.CheckStatusAsync<Permissions.CalendarWrite>();

                        if (statusRead == PermissionStatus.Granted && statusWrite == PermissionStatus.Granted)
                        {
                            await CreateDeviceAppointment(appointment);
                        }
                        else
                        {
                            UserDialogs.Instance.HideLoading();

                            var requestR = await Permissions.RequestAsync<Permissions.CalendarRead>();
                            var requestW = await Permissions.RequestAsync<Permissions.CalendarWrite>();

                            if (requestR == PermissionStatus.Granted && requestW == PermissionStatus.Granted)
                            {
                                UserDialogs.Instance.ShowLoading();
                                await CreateDeviceAppointment(appointment);
                            }
                        }

                        UserDialogs.Instance.HideLoading();

                        var updatedMessage = IsEdit ? "actualizada" : "creada";

                        await Application.Current.MainPage.DisplayAlert("Éxito!", $"Cita {updatedMessage}.", "Ok");
                        await Application.Current.MainPage.Navigation.PopAsync();
                    }
                    else
                    {
                        UserDialogs.Instance.HideLoading();

                        // display all errors from list as a single string from result
                        await Application.Current.MainPage.DisplayAlert("Errores: ", string.Join(" / ", result.Errors), "Ok");
                    }
                }
                else
                {
                    UserDialogs.Instance.HideLoading();
                    await Application.Current.MainPage.DisplayAlert("Error", "Contacte al administrador", "Ok");
                }
            }
            catch (Exception e)
            {
                UserDialogs.Instance.HideLoading();
                await Application.Current.MainPage.DisplayAlert("Error", $"Contacte al administrador: {e.Message}", "Ok");
            }

            UserDialogs.Instance.HideLoading();
        }

        private async Task SendAppointmentNotification(Appointment appointment)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(SelectedUser.Email))
                {
                    var appointmentInformation = $"{appointment.AppointmentDate.ToString("dd-MMMM-yyyy", cultureInfo)} a las {appointment.AppointmentDate.ToString("HH:mm")}";

                    var password = await _dataService.GetSettingByNameAndCatalog("password", "basic");
                    var email = await _dataService.GetSettingByNameAndCatalog("email", "basic");
                    var signatureData = await _dataService.GetSettingsByCatalog("signature");
                    var brand = await _dataService.GetSettingByNameAndCatalog("brand", "basic");

                    if (password != null && email != null)
                    {
                        string estadoCita = "agendado";
                        if (IsEdit)
                        {
                            estadoCita = "modificado";
                        }

                        var notification = new AppEmail
                        {
                            To = SelectedUser.Email,
                            Subject = $"Cita Odontológica JEDENT: {appointmentInformation}",
                            Body = $"Se ha <strong>{estadoCita}</strong> su cita en JeDent para el día <strong>{appointmentInformation}</strong>. Por favor, asista con anticipación. <br><br>Comuníquese con la doctora para comunicar cambios o cancelaciones.",
                            Sender = email.Value,
                            Password = password.Value
                        };

                        try
                        {
                            var signatureModel = new SignatureModel
                            {
                                Name = signatureData.FirstOrDefault(t => t.Name == "Name").Value,
                                Title = signatureData.FirstOrDefault(t => t.Name == "Title").Value,
                                Email = email.Value,
                                Phone = signatureData.FirstOrDefault(t => t.Name == "Phone").Value,
                                Address = signatureData.FirstOrDefault(t => t.Name == "Address").Value,
                                Facebook = signatureData.FirstOrDefault(t => t.Name == "Facebook").Value,
                                Website = signatureData.FirstOrDefault(t => t.Name == "Website").Value,
                                Company = brand.Value
                            };

                            EmailService.Send(notification, signatureModel);
                        }
                        catch (Exception ex)
                        {
                            await Application.Current.MainPage.DisplayAlert("Notificación", $"No se pudo enviar la notificación por EMAIL.", "Ok");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Notificación", $"No se pudo enviar la notificación por EMAIL.", "Ok");
            }
        }

        private async Task DeleteAppointmentAction()
        {
            var confirm = await UserDialogs.Instance.ConfirmAsync("Desea eliminar la cita?", null, "Si", "No");

            if (confirm)
            {
                UserDialogs.Instance.ShowLoading();
                try
                {
                    var result = await _dataService.DeleteAppointment(Id);

                    if (result == 1)
                    {
                        await DeleteExistingCalendarReminder(Id);

                        await Application.Current.MainPage.DisplayAlert("Éxito!", $"Cita eliminada.", "Ok");
                        await Application.Current.MainPage.Navigation.PopAsync();
                    }
                }
                catch (Exception e)
                {
                    UserDialogs.Instance.HideLoading();
                    await Application.Current.MainPage.DisplayAlert("Error", $"Contacte al administrador: {e.Message}", "Ok");
                }
                UserDialogs.Instance.Loading().Hide();
            }
        }

        private async Task FillSelectedAppointmentTypes(object sender)
        {
            if (sender is Models.DataModels.AppointmentType selectedAppointmentType)
            {
                var totalAppointmentMinutes = selectedAppointmentType.DefaultDuration;

                AssignAppointmentDurationFromEnum(totalAppointmentMinutes);

                if (!SelectedAppointmentTypes.Any(t => t.Id == selectedAppointmentType.Id))
                {
                    SelectedAppointmentTypes.Add(selectedAppointmentType);
                }
                else
                {
                    SelectedAppointmentTypes.Remove(selectedAppointmentType);
                }

                if (SelectedAppointmentTypes.Any())
                {
                    var maxAppointmentDuration = SelectedAppointmentTypes.Max(u => u.DefaultDuration);
                    AssignAppointmentDurationFromEnum(maxAppointmentDuration);

                }
                else
                {
                    AppointmentHours = 0; AppointmentMinutes = 0;
                }

            }
        }

        private async Task CreateDeviceAppointment(Appointment appointment)
        {

            IList<Calendar> calendars = await CrossCalendars.Current.GetCalendarsAsync();

            var appCalendar = calendars.FirstOrDefault(t => t.Name == EmailAccount);

            if (appCalendar == null)
            {
                appCalendar = new Calendar
                {
                    AccountName = BrandName,
                };

                await CrossCalendars.Current.AddOrUpdateCalendarAsync(appCalendar);

                appCalendar = calendars.FirstOrDefault(t => t.Name == BrandName);
            }
            else
            {
                // delete existent Reminder if exists for current appointment
                await DeleteExistingCalendarReminder(appointment.Id);

                var appointmentTypes = string.Join(", ", appointment.AppointmentTypes.Select(t => t.Name));

                var androidAppointment = new AndroidAppointment
                {
                    Title = $"{appointmentTypes}: {appointment.UserInformation} Tel: {appointment.UserPhone}",
                    StartDate = appointment.AppointmentDate,
                    EndDate = appointment.AppointmentEnd,
                    Location = BrandName,
                    AppointmentTypes = appointmentTypes,
                    ReminderMinutes = 10,
                    CalendarID = Convert.ToInt32(appCalendar.ExternalID)
                };

                var result = await DependencyService.Get<IDeviceCalendarService>().AddEventToCalendar(androidAppointment);

                if (result.IsSuccess)
                {
                    var calendarEventLog = new CalendarEventLog
                    {
                        AppointmentId = appointment.Id,
                        EventId = result.EventID,
                        ReminderId = result.ReminderID
                    };

                    await _dataService.AddCalendarEventLog(calendarEventLog);
                }
            }
        }

        // Look if possible to move to a global context
        private async Task InitializeSettings()
        {
            var account = await _dataService.GetSettingByNameAndCatalog("email", "basic");
            var brandName = await _dataService.GetSettingByNameAndCatalog("brand", "basic");

            EndTime = GivenTime.Add(TimeSpan.FromMinutes(30));

            if (account != null)
            {
                EmailAccount = account.Value;
                BrandName = brandName.Value;
            }
        }

        private async Task DeleteExistingCalendarReminder(int appointmentId)
        {
            var existingReminders = await _dataService.GetCalendarEventLog(appointmentId);
            if (existingReminders != null)
            {
                await _dataService.DeleteCalendarEventLog(appointmentId);
                await DependencyService.Get<IDeviceCalendarService>().DeleteEventFromCalendar(existingReminders);
            }
        }

        public async Task LoadUsers(string searchText = "", Models.DataModels.User user = null)
        {
            Users.Clear();

            var users = await _dataService.GetUsersByType(UserTypeEnum.Paciente, searchText);
            users = users.OrderBy(t => t.LastName).ToList();

            foreach (var person in users)
            {
                Users.Add(person);
            }

            if (user != null)
            {
                var userFromList = users.FirstOrDefault(t => t.Id == user.Id);
                SelectedUser = userFromList;
            }

        }

        public async Task Initialize(Models.DataModels.User user = null)
        {
            UserDialogs.Instance.ShowLoading();

            try
            {
                await LoadUsers(user: user);
                await InitializeAppointmentTypes();
                await InitializeSettings();
            }
            catch (Exception e)
            {
                UserDialogs.Instance.HideLoading();
                await Application.Current.MainPage.DisplayAlert("Error", $"Contacte al administrador: {e.Message}", "Ok");
            }

            UserDialogs.Instance.HideLoading();

        }

        private async Task InitializeAppointmentTypes()
        {
            AppointmentTypes.Clear();
            FilterAppointmentTypes.Clear();

            var appointmentTypes = await _dataService.GetAppointmentTypes();

            if (appointmentTypes.Any())
            {
                FilterAppointmentTypes.Add(new Models.DataModels.AppointmentType
                {
                    Name = "Todos"
                });
            }

            foreach (var appointmentType in appointmentTypes)
            {
                AppointmentTypes.Add(appointmentType);
                FilterAppointmentTypes.Add(appointmentType);
            }

            if (appointmentTypes.Count > 3)
            {
                AppointmentTypesHeight = 125;
            }
            else
            {
                AppointmentTypesHeight = 75;
            }
        }

        #endregion

        public async Task LoadAppointment(int id)
        {
            if (id != 0)
            {
                UserDialogs.Instance.ShowLoading();

                try
                {
                    var appointment = await _dataService.GetAppointment(id);

                    Id = appointment.Id;
                    GivenDate = appointment.AppointmentDate.Date;
                    GivenTime = appointment.AppointmentDate.TimeOfDay;
                    var appointmentDuration = appointment.AppointmentEnd - appointment.AppointmentDate;
                    var totalAppointmentMinutes = Convert.ToInt32(appointmentDuration.TotalMinutes);

                    AssignAppointmentDuration(totalAppointmentMinutes);

                    var typesFromAppointment = AppointmentTypes.Where(t => appointment.AppointmentTypes.Any(u => u.Id == t.Data.Id));

                    foreach (var appointmentType in typesFromAppointment)
                    {
                        appointmentType.IsSelected = true;
                        SelectedAppointmentTypes.Add(appointmentType.Data);
                    }

                    var selectedUser = Users.FirstOrDefault(t => t.Id == appointment.UserId);
                    var selectedUserIndex = Users.IndexOf(selectedUser);

                    SelectedUser = Users[selectedUserIndex];

                    IsEdit = true;
                }
                catch (Exception e)
                {
                    UserDialogs.Instance.HideLoading();
                    await Application.Current.MainPage.DisplayAlert("Error", $"Contacte al administrador: {e.Message}", "Ok");
                }
                UserDialogs.Instance.HideLoading();
            }
        }
        private void ValidateRequired()
        {
            if (SelectedUser == null)

            {
                ShowError = true;
            }
            else
            {

                ShowError = false;
            }

            //todo validate time entry and mapping
            if (AppointmentHours == 0 && AppointmentMinutes == 0)
            {
                ShowError = true;
            }
        }
        private void AssignAppointmentDurationFromEnum(AppointmentDurationEnum appointmentDurationInMinutes)
        {
            AssignAppointmentDuration((int)appointmentDurationInMinutes);
        }

        private void AssignAppointmentDuration(int appointmentDurationInMinutes)
        {
            AppointmentHours = appointmentDurationInMinutes / 60;
            AppointmentMinutes = appointmentDurationInMinutes % 60;
            EndTime = GivenTime.Add(TimeSpan.FromMinutes(AppointmentHours * 60 + appointmentDurationInMinutes));
        }
    }
}
