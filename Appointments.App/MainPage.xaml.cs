using Appointments.App.ViewModels;

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
