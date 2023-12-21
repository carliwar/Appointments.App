using Appointments.App.Models;
using Appointments.App.Models.DataModels;
using Appointments.App.Models.Enum;
using Appointments.App.Services;
using Appointments.App.Views.Appointment;
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
        private User _selectedUser;
        private ObservableCollection<Appointment> _appointments;
        private bool _showNoAppointmentsMessage;

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

        public User SelectedUser
        {
            get => _selectedUser;
            set => SetProperty(ref _selectedUser, value);
        }

        public bool ShowNoAppointmentsMessage
        {
            get => _showNoAppointmentsMessage;
            set => SetProperty(ref _showNoAppointmentsMessage, value);
        }
        #endregion        

        #region Commands

        //SearchUserCommand
        public ICommand SearchAppointmentCommand => new Command(async (item) => await SearchAppointment(item));
        public ICommand AddAppointmentCommand => new Command(async () => await NewAppointment());

        public ICommand ContactUserCommand => new Command(async (item) => await ExecuteContactUserCommand(item));

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

            if(!appointments.Any())
            {
                ShowNoAppointmentsMessage = true;
            }

            foreach (var appointment in appointments)
            {
                appointment.TextColor = Color.FromHex("#0c0c0c");
                var appointmentColor = Color.FromHex("#2196F3");

                switch (appointment.AppointmentType)
                {
                    case Models.Enum.AppointmentTypeEnum.Descanso:
                        appointmentColor = Color.FromHex("#2196F3");
                        break;
                    case Models.Enum.AppointmentTypeEnum.Extraccion:
                        appointmentColor = Color.FromHex("#ffb3b3");
                        break;
                    case Models.Enum.AppointmentTypeEnum.Consulta:
                        appointmentColor = Color.FromHex("#cbdbe7");
                        break;
                    case Models.Enum.AppointmentTypeEnum.Endodoncia:
                        appointmentColor = Color.FromHex("#b3ffb3");
                        break;
                    case Models.Enum.AppointmentTypeEnum.Ortodoncia:
                        appointmentColor = Color.FromHex("#ffe6ff");
                        break;

                }

                if (!appointment.Attended)
                {
                    appointmentColor = Color.FromHex("#800000");
                    appointment.TextColor = Color.FromHex("#fefefe");
                }


                appointment.AppointmentColor = appointmentColor;

                Appointments.Add(appointment);
            }
        }

        public async Task NewAppointment()
        {
            await Application.Current.MainPage.Navigation.PushAsync(new CreateAppointmentPage(DateTime.Now, SelectedUser));
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
