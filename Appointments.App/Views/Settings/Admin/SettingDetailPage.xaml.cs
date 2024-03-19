using Appointments.App.ViewModels.Settings.Admin;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Appointments.App.Views.Settings.Admin
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class SettingDetailPage : ContentPage
	{
        public int SettingId { get; set; }
        public SettingDetailPage (int settingId)
		{
			InitializeComponent();
            SettingId = settingId;
        }

        protected override async void OnAppearing()
        {
            await (BindingContext as SettingViewModel)?.LoadSetting(SettingId);
        }
    }
}