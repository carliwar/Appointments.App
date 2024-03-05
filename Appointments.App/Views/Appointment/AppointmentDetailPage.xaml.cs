using Appointments.App.Models.DataModels;
using Appointments.App.ViewModels.Appointments;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Appointments.App.Views.Appointments
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class AppointmentDetailPage : ContentPage
	{
        public int? AppointmentId { get; set; }
        public User SelectedUser { get; set; }
        public DateTime GivenDate { get; set; }
        public TimeSpan GivenTime { get; set; }

        public AppointmentDetailPage(DateTime date, User user = null, int? appointmentId = null)
        {
            InitializeComponent();
            SelectedUser = user;
            GivenDate = date;
            GivenTime = new TimeSpan(8, 0, 0);
            AppointmentId = appointmentId;
        }

        protected override void OnAppearing()
        {
            AppointmentViewModel userAppointmentsViewModel = (BindingContext as AppointmentViewModel);
            userAppointmentsViewModel.SelectedUser = SelectedUser;
            userAppointmentsViewModel.GivenDate = GivenDate;
            userAppointmentsViewModel.GivenTime = GivenTime;
            (BindingContext as AppointmentViewModel)?.Initialize(user: SelectedUser);

            if (AppointmentId.HasValue)
            {
                (BindingContext as AppointmentViewModel)?.LoadAppointment(AppointmentId.Value);
            }


        }
    }
}