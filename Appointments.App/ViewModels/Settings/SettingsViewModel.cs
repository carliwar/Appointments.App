using Appointments.App.Views.Settings;
using Appointments.App.Views.Settings.Admin;
using System.Windows.Input;


namespace Appointments.App.ViewModels.Settings
{
    public class SettingsViewModel : BasePageViewModel
    {
        public SettingsViewModel() : base()
        {
            
        }

        public ICommand NavigateToPage => new Command(async (item) => await Navigate(item));

        private async Task Navigate(object page)
        {
            if (page != null) {
                switch (page)
                {
                    case "AppointmentTypes":
                        await Application.Current.MainPage.Navigation.PushAsync(new AppointmentTypesPage());
                        break;
                    case "Admin Settings":
                        await Application.Current.MainPage.Navigation.PushAsync(new SettingsListPage());
                        break;
                }
            }
            
        }
    }
}
