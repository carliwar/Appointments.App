using Appointments.App.Models;
using Appointments.App.Views.Appointment;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using Xamarin.Plugin.Calendar.Models;

namespace Appointments.App.ViewModels
{
    public class MainPageViewModel : BasePageViewModel
    {
        public MainPageViewModel():base() 
        {
            SelectedDate = DateTime.Today;
            SelectedDateIsLessThanToday = true;

            // testing all kinds of adding events
            // when initializing collection
            Events = new EventCollection
            {
                [DateTime.Now.AddDays(-3)] = new List<EventModel>(GenerateEvents(10, AppointmentType.Endodoncia.ToString())),
                [DateTime.Now.AddDays(4)] = new List<EventModel>(GenerateEvents(2, AppointmentType.Consulta.ToString())),
                [DateTime.Now.AddDays(2)] = new List<EventModel>(GenerateEvents(1, AppointmentType.Consulta.ToString())),
                [DateTime.Now.AddDays(1)] = new List<EventModel>(GenerateEvents(3, AppointmentType.Consulta.ToString())),
            };

            // with add method
            Events.Add(DateTime.Now.AddDays(-1), new List<EventModel>(GenerateEvents(5, AppointmentType.Endodoncia.ToString())));

            // with indexer
            Events[DateTime.Now] = new List<EventModel>(GenerateEvents(2, AppointmentType.Ortodoncia.ToString()));

            Task.Delay(5000).ContinueWith(_ =>
            {
                // indexer - update later
                Events[DateTime.Now] = new ObservableCollection<EventModel>(GenerateEvents(10, AppointmentType.Endodoncia.ToString()));

                // add later
                Events.Add(DateTime.Now.AddDays(3), new List<EventModel>(GenerateEvents(5, AppointmentType.Endodoncia.ToString())));

                // indexer later
                Events[DateTime.Now.AddDays(10)] = new List<EventModel>(GenerateEvents(10, AppointmentType.Ortodoncia.ToString()));

                // add later
                Events.Add(DateTime.Now.AddDays(15), new List<EventModel>(GenerateEvents(10, AppointmentType.Endodoncia.ToString())));
                
            }, TaskScheduler.FromCurrentSynchronizationContext());
        }

        #region Constants
        private const string _enabledAddAppointmentButton = "Agendar Cita +";
        private const string _disabledAddAppointmentButton = "Fecha no válida para nueva cita.";
        #endregion

        #region Properties
        public EventCollection Events { get; }
        private int _month = DateTime.Today.Month;
        private int _year = DateTime.Today.Year;
        private int _day = DateTime.Today.Day;
        private bool _selectedDateIsLessThanToday = false;
        private string _addAppointmentText = _enabledAddAppointmentButton;

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
        public bool SelectedDateIsLessThanToday
        {
            get => _selectedDateIsLessThanToday;
            set => SetProperty(ref _selectedDateIsLessThanToday, value);
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
        #endregion

        #region Commands
        public ICommand EventSelectedCommand => new Command(async (item) => await ExecuteEventSelectedCommand(item));
        public ICommand ButtonClickCommand => new Command(async (item) => await ButtonClicked(item));

        private async Task ExecuteEventSelectedCommand(object item)
        {
            if (item is EventModel eventModel)
            {
                await App.Current.MainPage.DisplayAlert(eventModel.Name, eventModel.Description, "Ok");
            }
        }

        public ICommand DaySelectedCommand
        {            
            get
            {
                return new Command<DateTime>((date) => DaySelected(SelectedDate.Value));
            }
        }        

        private void DaySelected(DateTime date)
        {
            // disable AddAppointmentButton if the date is less than today
            if (date.Date >= DateTime.Today)
            {
                SelectedDateIsLessThanToday = true;
                AddAppointmentText = _enabledAddAppointmentButton;
            }
            else
            {
                SelectedDateIsLessThanToday = false;
                AddAppointmentText = _disabledAddAppointmentButton;
            }
        }

        private async Task ButtonClicked(object sender)
        {
            await Application.Current.MainPage.Navigation.PushAsync(new CreateAppointmentPage(SelectedDate.Value));
        }
        #endregion

        #region Methods
        private IEnumerable<EventModel> GenerateEvents(int count, string name)
        {
            return Enumerable.Range(1, count).Select(x => new EventModel
            {
                Name = $"{name} {x}",
                Description = $"Paciente: {name} {x}",                
            });
        }
        #endregion
    }
}
