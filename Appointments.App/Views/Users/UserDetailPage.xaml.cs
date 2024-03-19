using Appointments.App.ViewModels.User;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Appointments.App.Views.Users
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class UserDetailPage : ContentPage
    {
        public int UserId { get; set; }
        public UserDetailPage(int userId)
        {
            InitializeComponent();
            UserId = userId;
        }

        protected override async void OnAppearing()
        {
            await (BindingContext as UserViewModel)?.InitializeAppointmentTypes();
            await (BindingContext as UserViewModel)?.LoadUser(UserId);
        }
    }
}