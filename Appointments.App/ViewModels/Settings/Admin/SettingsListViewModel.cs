using Acr.UserDialogs;
using Appointments.App.Models.DataModels;
using Appointments.App.Models.Enum;
using Appointments.App.Services;
using Appointments.App.Views.Settings.Admin;
using Appointments.App.Views.UserAppointment;
using Appointments.App.Views.Users;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace Appointments.App.ViewModels.Settings.Admin
{
    public class SettingsListViewModel : BasePageViewModel
    {
        public SettingsListViewModel() : base()
        {
            _dataService = new DataService();
        }

        #region Temp Properties
        private readonly IDataService _dataService;

        #endregion

        #region Properties
        private ObservableCollection<Setting> _settings = new ObservableCollection<Setting>();
        private ObservableCollection<string> _settingsCatalogs = new ObservableCollection<string>();
        private Setting _selectedSettingCatalog;
        private bool _adminIn;

        public ObservableCollection<string> SettingCatalogs
        {
            get => _settingsCatalogs;
            set => SetProperty(ref _settingsCatalogs, value);
        }

        public ObservableCollection<Setting> Settings
        {
            get => _settings;
            set => SetProperty(ref _settings, value);
        }

        public bool AdminIn
        {
            get => _adminIn;
            set => SetProperty(ref _adminIn, value);
        }

        public Setting SelectedSettingCatalog
        {
            get => _selectedSettingCatalog;
            set
            {
                SetProperty(ref _selectedSettingCatalog, value);

                var searchValue = string.Empty;

                if (SelectedSettingCatalog.Id != 0)
                {
                    searchValue = SelectedSettingCatalog.Catalog;
                }

                InitializeSettings(searchValue).ConfigureAwait(false);
            }
        }
        #endregion

        #region Commands

        public ICommand SearchSettingCommand => new Command(async (item) => await InitializeSettings(item));
        public ICommand CreateSettingCommand => new Command(async (item) => await CreateSetting());
        public ICommand LoadSettingCommand => new Command(async (item) => await LoadSetting(item));

        #endregion
        public async Task InitializeSettings(object searchText = null)
        {
            if (!AdminIn)
            {
                var admintAccessPromt = await UserDialogs.Instance.PromptAsync("Ingrese el código de administrador:", "Solo Administrador!", "Ok", "Salir");
                if (admintAccessPromt != null && admintAccessPromt.Text == "4924")
                {
                    AdminIn = true;
                }
                else
                {
                    await Application.Current.MainPage.DisplayAlert("Acceso Negado", $"Código inválido", "Regresar al Inicio");
                    await Application.Current.MainPage.Navigation.PushAsync(new MainPage());
                }
            }

            Settings.Clear();
            SettingCatalogs.Clear();

            var settings = await _dataService.GetAllSettings((string)searchText);
            settings = settings.OrderBy(t => t.Name).ToList();

            foreach (var setting in settings)
            {
                Settings.Add(setting);
            }

            var catalogs = settings.Select(t => t.Catalog).Distinct();

            SettingCatalogs.Add("Todos");

            foreach (var catalog in catalogs)
            {
                SettingCatalogs.Add(catalog);
            }
        }

        private async Task CreateSetting()
        {
            await Application.Current.MainPage.Navigation.PushAsync(new SettingDetailPage(0));
        }

        private async Task LoadSetting(object setting)
        {
            if (setting is Setting givenSetting)
            {
                await Application.Current.MainPage.Navigation.PushAsync(new SettingDetailPage(givenSetting.Id));
            }

        }

    }
}
