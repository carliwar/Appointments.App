﻿using Appointments.App.Models;
using Appointments.App.Services;
using Appointments.App.Views.Appointment;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using Xamarin.Plugin.Calendar.Models;

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
            SelectedDateIsLessThanToday = true;            
        }

        #region Constants
        private const string _enabledAddAppointmentButton = "Agendar Cita +";
        private const string _disabledAddAppointmentButton = "Fecha no válida para nueva cita.";
        #endregion

        #region Properties
        private int _month = DateTime.Today.Month;
        private int _year = DateTime.Today.Year;
        private int _day = DateTime.Today.Day;
        private bool _selectedDateIsLessThanToday = false;
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

        public EventCollection Events {
            get => _events;
            set => SetProperty(ref _events, value);
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

        private List<EventModel> GenerateEvents(List<Appointment> appointments)
        {
            var results = new List<EventModel>();
            foreach(Appointment appointment in appointments)
            {
                results.Add(new EventModel 
                { 
                    Description = appointment.UserId,
                    Name = appointment.AppointmentType.ToString(),
                    Type = appointment.AppointmentType
                });
            }

            return results;
        }

        private async Task GetEvents()
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
