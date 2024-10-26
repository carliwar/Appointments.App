using Appointments.App.ViewModels.User;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Appointments.App.Views.Users
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class UserDetailPage : ContentPage
    {
        public int UserId { get; set; }
        public UserViewModel ViewModel => BindingContext as UserViewModel;
        public UserDetailPage(int userId)
        {
            InitializeComponent();
            UserId = userId;
        }

        protected override async void OnAppearing()
        {
            await ViewModel.InitializeAppointmentTypes();
            await ViewModel.LoadUser(UserId);
        }
    }
}