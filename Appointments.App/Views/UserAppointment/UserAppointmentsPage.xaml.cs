using Appointments.App.Models.DataModels;
using Appointments.App.ViewModels;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

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

        protected override void OnAppearing()
        {
            UserAppointmentsViewModel userAppointmentsViewModel = (BindingContext as UserAppointmentsViewModel);
            userAppointmentsViewModel.SelectedUser = SelectedUser2;
            (BindingContext as UserAppointmentsViewModel)?.GetAppointments();
        }
    }
}