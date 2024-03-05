using Appointments.App.Models;
using Appointments.App.Models.DataModels;
using Appointments.App.Models.Enum;
using Appointments.App.Services;
using Appointments.App.Views.Appointment;
using Appointments.App.Views.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Plugin.Calendar.Models;
using static Xamarin.Essentials.Permissions;

namespace Appointments.App.ViewModels
{
    public class MainPageViewModel : BasePageViewModel
    {
        #region Temp Properties

        private readonly IDataService _dataService;

        #endregion

        public MainPageViewModel():base() 
        {
            _dataService = new DataService();
            Events = new EventCollection();

            Task.Run(() => GetEvents()).Wait();            

            SelectedDate = DateTime.Today;
            EnableAddAppointmentButton = true;            
        }

        #region Constants
        private const string _enabledAddAppointmentButton = "Agendar Cita +";
        private const string _disabledAddAppointmentButton = "Fecha no válida para nueva cita.";
        #endregion

        #region Properties
        public DateTime Today {  get => DateTime.Now; } 
        private int _month = DateTime.Today.Month;
        private int _year = DateTime.Today.Year;
        private int _day = DateTime.Today.Day;
        private bool _enableAddAppointmentButton = false;
        private string _addAppointmentText = _enabledAddAppointmentButton;
        private EventCollection _events;

        public int Day
        {
            get => _day;
            set => SetProperty(ref _day, value);
        }
        public int Month
        {
            get => _month;
            set => SetProperty(ref _month, value);
        }
        public int Year
        {
            get => _year;
            set => SetProperty(ref _year, value);
        }
        public bool EnableAddAppointmentButton
        {
            get => _enableAddAppointmentButton;
            set => SetProperty(ref _enableAddAppointmentButton, value);
        }

        private DateTime? _selectedDate = DateTime.Today;

        public DateTime? SelectedDate
        {
            get => _selectedDate;
            set => SetProperty(ref _selectedDate, value);
        }

        public string AddAppointmentText
        {
            get => _addAppointmentText;
            set => SetProperty(ref _addAppointmentText, value);
        }

        public EventCollection Events {
            get => _events;
            set => SetProperty(ref _events, value);
        }
        #endregion

        #region Commands
        public ICommand EventSelectedCommand => new Command(async (item) => await ExecuteEventSelectedCommand(item));
        public ICommand ButtonClickCommand => new Command(async (item) => await ButtonClicked(item));
        public ICommand CallPhoneCommand => new Command(async (phone) => await CallPhoneClicked(phone));
        public ICommand TodayCommand => new Command(async (item) => await SetToday());
        public ICommand SettingsCommand => new Command(async (item) => await GoToSettings());

        private async Task SetToday()
        {
            SelectedDate = Today;
        }

        private async Task GoToSettings()
        {
            await Application.Current.MainPage.Navigation.PushAsync(new SettingsPage());
        }

        private async Task ExecuteEventSelectedCommand(object item)
        {
            if (item is EventModel eventModel)
            {
                //create a list of strings
                var options = new List<string> { ConstantValues.MARK_NOT_ATTENDED_OPTION };

                if(eventModel.UserPhone != null)
                {
                    options.Add(ConstantValues.CALL_OPTION);
                    options.Add(ConstantValues.CONTACT_WHATSAPP_OPTION);
                }


                string action = await App.Current.MainPage.DisplayActionSheet($"{eventModel.AppointmentType} - {eventModel.UserInformation}", "", "Cerrar",
                    options.ToArray());


                var phone = new string(eventModel.UserPhone.ToString().Where(c => char.IsDigit(c)).ToArray());

                switch (action)
                {
                    case ConstantValues.CONTACT_WHATSAPP_OPTION:
                        await Browser.OpenAsync(new Uri($"https://wa.me/{phone}"), BrowserLaunchMode.SystemPreferred);
                        break;
                    case ConstantValues.CALL_OPTION:
                        PhoneDialer.Open(phone);
                        break;
                    case ConstantValues.MARK_NOT_ATTENDED_OPTION:
                        var appointment = await _dataService.GetAppointment(eventModel.Id);
                        appointment.Attended = false;
                        await _dataService.UpdateAppointment(appointment);
                        break;
                }
            }
        }

        public ICommand DaySelectedCommand
        {            
            get
            {
                return new Command<DateTime>((date) => DaySelected(date));
            }
        }

        private void DaySelected(DateTime date)
        {
            if(SelectedDate is null)
            {
                EnableAddAppointmentButton = false;
                AddAppointmentText = _disabledAddAppointmentButton;
            }
            // disable AddAppointmentButton if the date is less than today
            if (date.Date >= DateTime.Today)
            {
                EnableAddAppointmentButton = true;
                AddAppointmentText = _enabledAddAppointmentButton;
            }
            else
            {
                EnableAddAppointmentButton = false;
                AddAppointmentText = _disabledAddAppointmentButton;
            }
        }

        private async Task ButtonClicked(object sender)
        {
            if(SelectedDate is null)
            {
                SelectedDate = DateTime.Today;
            }
            await Application.Current.MainPage.Navigation.PushAsync(new CreateAppointmentPage(SelectedDate.Value));
        }

        private async Task CallPhoneClicked(object phone)
        {
            phone = new string(phone.ToString().Where(c => char.IsDigit(c)).ToArray());
            await Browser.OpenAsync(new Uri($"https://wa.me/{phone}"), BrowserLaunchMode.SystemPreferred);
        }
        #endregion

        #region Methods

        private List<EventModel> GenerateEvents(List<Appointment> appointments)
        {
            var results = new List<EventModel>();
            foreach(Appointment appointment in appointments)
            {
                var appointmentColor = Color.FromHex("#2196F3");
                var attendedFlag = string.Empty;
                if (!appointment.Attended)
                {
                    attendedFlag = $" - {ConstantValues.NOT_ATTENDED}";
                    appointmentColor = Color.FromHex("800000");
                }

                var appointmentTypeString = string.Empty;

                if (appointment.AppointmentTypes != null && appointment.AppointmentTypes.Any())
                {
                    appointmentTypeString = string.Join(", ", appointment.AppointmentTypes.Select(c => c.Name));
                    appointmentColor = Color.FromHex(appointment.AppointmentTypes.FirstOrDefault().ColorCode);
                }

                results.Add(new EventModel 
                { 
                    Id = appointment.Id,
                    UserInformation = appointment.UserName,
                    UserPhone = appointment.UserPhone,
                    AppointmentType = $"{appointmentTypeString} {attendedFlag}",
                    EventDate = appointment.AppointmentDate,
                    AppointmentColor = appointmentColor
                });
            }

            if(results.Any())
            {
                results = results.OrderBy(t => t.EventDate.TimeOfDay).ToList();
            }

            return results;
        }

        public async Task GetEvents()
        {
            Events.Clear();

            var results = await _dataService.GetAppointments(DateTime.Today.AddMonths(-1), DateTime.Today.AddMonths(1));

            var datesWithEvents = results.Select(x => x.AppointmentDate.Date).Distinct();

            foreach (var groupDate in datesWithEvents) {

                List<Appointment> events = results.Where(x => x.AppointmentDate.Date == groupDate).ToList();                

                Events.Add(groupDate, GenerateEvents(events));
            }

            if(Events == null)
            {
                Events = new EventCollection();
            }
        }
        #endregion
    }
}
