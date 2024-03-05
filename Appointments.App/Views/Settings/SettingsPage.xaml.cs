using Appointments.App.Views.Settings.AppointmentType;
using System;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Appointments.App.Views.Settings
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SettingsPage : ContentPage
    {
        public SettingsPage()
        {
            InitializeComponent();
        }

        private void AppointmentTypesClicked(object sender, EventArgs e)
        {
            //var nav = new NavigationPage(new AppointmentTypesPage());
            ////Application.Current.MainPage = nav;
            //Application.Current.MainPage.Navigation.PushAsync(nav);
            //Application.Current.MainPage = new AppointmentTypesPage();
        }
    }
}