using Appointments.App.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Appointments.App.Views.Users
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class UsersPage : ContentPage
    { 
        public UsersPage()
        {
            InitializeComponent();
        }        

        protected override async void OnAppearing()
        {            
            await (BindingContext as UserListViewModel)?.InitializeUsers();            
            await (BindingContext as UserListViewModel)?.InitializeAppointmentTypes();            
        }

    }
}
