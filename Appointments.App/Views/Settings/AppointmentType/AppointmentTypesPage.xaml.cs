using Appointments.App.ViewModels;
using Appointments.App.ViewModels.AppointmentType;



namespace Appointments.App.Views.Settings
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class AppointmentTypesPage : ContentPage
	{
		public AppointmentTypesPage ()
		{
			InitializeComponent ();
		}

        protected override void OnAppearing()
        {
            (BindingContext as AppointmentTypesListViewModel)?.InitializeAppointmentTypes();
        }
    }
}