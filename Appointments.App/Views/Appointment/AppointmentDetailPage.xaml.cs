using Appointments.App.Models.DataModels;
using Appointments.App.ViewModels.Appointments;
using System;
using System.Linq;
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

            if(SelectedUser != null)
                userAppointmentsViewModel.SelectedUser = SelectedUser;
            if(GivenDate != null)
                userAppointmentsViewModel.GivenDate = GivenDate;
            if(GivenTime != null) 
                userAppointmentsViewModel.GivenTime = GivenTime;

            (BindingContext as AppointmentViewModel)?.Initialize(user: SelectedUser).ContinueWith((t1) =>
            {
                if (AppointmentId.HasValue)
                {
                    userAppointmentsViewModel.SelectedUser = null;
                    (BindingContext as AppointmentViewModel)?.LoadAppointment(AppointmentId.Value);
                }

            });
        }
    }
}