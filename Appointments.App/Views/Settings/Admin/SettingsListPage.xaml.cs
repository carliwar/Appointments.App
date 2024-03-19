using Appointments.App.ViewModels.Settings.Admin;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Appointments.App.Views.Settings.Admin
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class SettingsListPage : ContentPage
	{
		public SettingsListPage ()
		{
			InitializeComponent ();
		}

        protected override void OnAppearing()
        {
            (BindingContext as SettingsListViewModel)?.InitializeSettings();
        }
    }
}