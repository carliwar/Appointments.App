using Appointments.App.Models.DataModels;
using Appointments.App.ViewModels;



namespace Appointments.App.Views.UserAppointment
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class UserAppointmentsPage : ContentPage
    {
        public User SelectedUser2 { get; set; }
        public UserAppointmentsPage(User user)
        {
            InitializeComponent();
            SelectedUser2 = user;
        }

        protected override async void OnAppearing()
        {
            UserAppointmentsViewModel userAppointmentsViewModel = (BindingContext as UserAppointmentsViewModel);
            userAppointmentsViewModel.SelectedUser = SelectedUser2;
            await (BindingContext as UserAppointmentsViewModel)?.GetAppointments();
        }
    }
}