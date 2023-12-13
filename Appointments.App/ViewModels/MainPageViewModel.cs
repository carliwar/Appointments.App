using Appointments.App.Models;
using Appointments.App.Models.DataModels;
using Appointments.App.Services;
using Appointments.App.Views.Appointment;
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

        private async Task SetToday()
        {
            SelectedDate = Today;
        }

        private async Task ExecuteEventSelectedCommand(object item)
        {
            if (item is EventModel eventModel)
            {
                var result = await App.Current.MainPage.DisplayAlert(eventModel.AppointmentType, $"{eventModel.UserInformation}", "Contactar", "Cerrar");
                if(result == true)
                {
                    var phone = new string(eventModel.UserPhone.ToString().Where(c => char.IsDigit(c)).ToArray());
                    await Browser.OpenAsync(new Uri($"https://wa.me/{phone}"), BrowserLaunchMode.SystemPreferred);
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
                switch (appointment.AppointmentType)
                {
                    case Models.Enum.AppointmentType.Descanso:
                        appointmentColor = Color.FromHex("#2196F3");
                        break;
                    case Models.Enum.AppointmentType.Extraccion:
                        appointmentColor = Color.FromHex("#ff0000");
                        break;
                    case Models.Enum.AppointmentType.Consulta:
                        appointmentColor = Color.FromHex("#5084ad");
                        break;
                    case Models.Enum.AppointmentType.Endodoncia:
                        appointmentColor = Color.FromHex("#00ff00");
                        break;
                    case Models.Enum.AppointmentType.Ortodoncia:
                        appointmentColor = Color.FromHex("#ff00ff");
                        break;

                }
                results.Add(new EventModel 
                { 
                    UserInformation = appointment.UserName,
                    UserPhone = appointment.UserPhone,
                    AppointmentType = appointment.AppointmentType.ToString(),
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
