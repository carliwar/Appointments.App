using Appointments.App.Views.Users;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Appointments.App
{
	public partial class AppShell : Shell
    {
		public AppShell ()
		{
			InitializeComponent();
            Routing.RegisterRoute(nameof(MainPage), typeof(MainPage));
            Routing.RegisterRoute(nameof(UsersPage), typeof(UsersPage));
        }
	}
}