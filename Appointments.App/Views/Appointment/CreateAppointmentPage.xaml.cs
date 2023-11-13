using Appointments.App.ViewModels;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Appointments.App.Views.Appointment
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class CreateAppointmentPage : ContentPage
    {

        #region Properties

        #endregion

        public CreateAppointmentPage(DateTime date)
        {            
            InitializeComponent();
            var viewModel = new CreateAppointmentViewModel();
            viewModel.GivenDate = date;

            Content.BindingContext = viewModel;
        }
    }
}
