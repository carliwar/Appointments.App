using Appointments.App.ViewModels;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Appointments.App.Views.Appointment
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class CreateAppointmentPage : ContentPage
    {
        public CreateAppointmentPage(DateTime date)
        {            
            InitializeComponent();

            var viewModel = new CreateAppointmentViewModel
            {
                GivenDate = date,
                GivenTime = new TimeSpan(7, 0, 0)
            };

            Content.BindingContext = viewModel;
        }
    }
}
