using Appointments.App.Models.DataModels;
using Appointments.App.ViewModels;
using Appointments.App.ViewModels.Appointments;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Appointments.App
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
        }

        protected override void OnAppearing()
        {
            (BindingContext as MainPageViewModel)?.GetEvents();
        }
    }
}
