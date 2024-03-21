using Appointments.App.ViewModels.AppointmentType;



namespace Appointments.App.Views.Settings.AppointmentType
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class AppointmentTypeDetailPage : ContentPage
	{
        public Models.DataModels.AppointmentType AppointmentType { get; set; }
        public AppointmentTypeDetailPage (Models.DataModels.AppointmentType appointmentType)
		{
			InitializeComponent ();
			AppointmentType = appointmentType;
		}

        protected override void OnAppearing()
        {
            (BindingContext as AppointmentTypeViewModel)?.LoadAppointmentType(AppointmentType.Id);
        }
    }
}