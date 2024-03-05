using Appointments.App.Models;
using Appointments.App.Models.DataModels;
using Appointments.App.Models.Enum;
using Appointments.App.Services;
using Appointments.App.Views.Appointments;
using Appointments.App.Views.Settings.AppointmentType;
using Appointments.App.Views.Users;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.NetworkInformation;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace Appointments.App.ViewModels
{
    public class UserAppointmentsViewModel : BasePageViewModel
    {
        public UserAppointmentsViewModel() : base()
        {
            _dataService = new DataService();
            Appointments = new ObservableCollection<Appointment>();
        }
        #region Temp Properties
        private readonly IDataService _dataService;

        #endregion

        #region Properties
        private int _id;
        private string _userValue;
        private DateTime _givenDate;
        private Models.DataModels.User _selectedUser;
        private ObservableCollection<Appointment> _appointments;
        private bool _showNoAppointmentsMessage;
        private bool _hasDefaultAppointmentType;
        private string _userDefaultAppointmentType;
        private string _userDefaultAppointmentTypeColor;

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

        public ObservableCollection<Appointment> Appointments
        {
            get => _appointments;
            set => SetProperty(ref _appointments, value);
        }

        public Models.DataModels.User SelectedUser
        {
            get => _selectedUser;
            set => SetProperty(ref _selectedUser, value);
        }

        public bool ShowNoAppointmentsMessage
        {
            get => _showNoAppointmentsMessage;
            set => SetProperty(ref _showNoAppointmentsMessage, value);
        }

        public bool HasDefaultAppointmentType
        {
            get => _hasDefaultAppointmentType;
            set => SetProperty(ref _hasDefaultAppointmentType, value);
        }
        public string UserDefaultAppointmentType
        {
            get => _userDefaultAppointmentType;
            set => SetProperty(ref _userDefaultAppointmentType, value);
        }
        #endregion        

        #region Commands

        //SearchUserCommand
        public ICommand SearchAppointmentCommand => new Command(async (item) => await SearchAppointment(item));
        public ICommand AddAppointmentCommand => new Command(async () => await NewAppointment());
        public ICommand ContactUserCommand => new Command(async (item) => await ExecuteContactUserCommand(item));
        public ICommand EditUserCommand => new Command(async (item) => await GoToUserDetails());


        private async Task GoToUserDetails()
        {
            await Application.Current.MainPage.Navigation.PushAsync(new UserDetailPage(SelectedUser.Id));
        }
        // TODO
        private async Task SearchAppointment(object sender)
        {

            if (sender == null)
            {
                await GetAppointments();
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

                await GetAppointments();
            }
        }

        public async Task GetAppointments(string searchText = null)
        {
            Appointments.Clear();
            if(SelectedUser == null)
            {
                return;
            }

            var appointments = await _dataService.GetAppointmentsByUser(SelectedUser, null, null);
            appointments = appointments.OrderBy(t => t.AppointmentDate).ToList();

            if(SelectedUser.AppointmentType != null)
            {
                HasDefaultAppointmentType = true;
                UserDefaultAppointmentType = $"Especialidad: {SelectedUser.AppointmentType.Name}";
            }

            if(!appointments.Any())
            {
                ShowNoAppointmentsMessage = true;
            }

            foreach (var appointment in appointments)
            {
                var appointmentColor = Color.FromHex("#2196F3");

                if(appointment.AppointmentTypes.Any())
                {
                    appointmentColor = Color.FromHex(appointment.AppointmentTypes.FirstOrDefault().ColorCode);                    
                }

                appointment.AppointmentColor = appointmentColor;


                Appointments.Add(appointment);
            }
        }

        public async Task NewAppointment()
        {
            await Application.Current.MainPage.Navigation.PushAsync(new AppointmentDetailPage(DateTime.Now, SelectedUser));
        }

        private async Task ExecuteContactUserCommand(object item)
        {
            if (SelectedUser != null)
            {
                //create a list of strings

                var options = new List<string>();

                if (SelectedUser.Phone != null)
                {
                    options = new List<string> { ConstantValues.CALL_OPTION, ConstantValues.CONTACT_WHATSAPP_OPTION };

                    string action = await App.Current.MainPage.DisplayActionSheet($"{SelectedUser.UserFullName}", "", "Cerrar",
                    options.ToArray());


                    var phone = new string(SelectedUser.Phone.ToString().Where(c => char.IsDigit(c)).ToArray());

                    switch (action)
                    {
                        case ConstantValues.CONTACT_WHATSAPP_OPTION:
                            await Browser.OpenAsync(new Uri($"https://wa.me/{phone}"), BrowserLaunchMode.SystemPreferred);
                            break;
                        case ConstantValues.CALL_OPTION:
                            PhoneDialer.Open(phone);
                            break;
                    }
                }
                else
                {
                    await Application.Current.MainPage.DisplayAlert("Error!", "El paciente no tiene número de contacto asignado.", "Ok");
                }
            }
        }
        #endregion
    }
}
