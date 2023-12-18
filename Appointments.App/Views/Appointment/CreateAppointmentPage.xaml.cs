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
        public CreateAppointmentPage(DateTime date, User user = null)
        {            
            InitializeComponent();

            var viewModel = new CreateAppointmentViewModel
            {
                GivenDate = date,
                GivenTime = new TimeSpan(7, 0, 0),
                SelectedUser = user
            };

            Content.BindingContext = viewModel;
        }
    }
}
