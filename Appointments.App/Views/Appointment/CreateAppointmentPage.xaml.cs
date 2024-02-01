using Appointments.App.Models.DataModels;
using Appointments.App.ViewModels;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Appointments.App.Views.Appointment
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class CreateAppointmentPage : ContentPage
    {
        public User SelectedUser { get; set; }
        public DateTime GivenDate { get; set; }
        public TimeSpan GivenTime { get; set; }
        public CreateAppointmentPage(DateTime date, User user = null)
        {            
            InitializeComponent();
            SelectedUser = user;
            GivenDate = date;
            GivenTime = new TimeSpan(8, 0, 0);
        }

        protected override void OnAppearing()
        {
            CreateAppointmentViewModel userAppointmentsViewModel = (BindingContext as CreateAppointmentViewModel);
            userAppointmentsViewModel.SelectedUser = SelectedUser;
            userAppointmentsViewModel.GivenDate = GivenDate;
            userAppointmentsViewModel.GivenTime = GivenTime;
            (BindingContext as CreateAppointmentViewModel)?.Initialize(user:SelectedUser);


        }
    }
}
