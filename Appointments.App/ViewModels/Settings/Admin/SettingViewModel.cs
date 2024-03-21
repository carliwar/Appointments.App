using Appointments.App.Models.DataModels;
using Appointments.App.Models.Enum;
using Appointments.App.Services;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;


namespace Appointments.App.ViewModels.Settings.Admin
{
    public class SettingViewModel : BasePageViewModel
    {
        public SettingViewModel()
        {
            _dataService = new DataService();

            foreach (SettingCatalogEnum enumValue in Enum.GetValues(typeof(SettingCatalogEnum)))
            {
                string customString = Utils.EnumDescriptor.GetEnumDescription(enumValue);

                var appointmentDuration = new SettingCatalogsEnumModel
                {
                    Name = enumValue,
                    Description = customString
                };

                SettingCatalogs.Add(appointmentDuration);
            }
        }

        #region Temp Properties

        private readonly IDataService _dataService;

        #endregion

        #region Properties

        private int _id;
        private string _catalog;
        private string _name;
        private string _value;
        private bool _isEdit = false;
        private SettingCatalogsEnumModel _selectedSettingCatalog;
        private ObservableCollection<SettingCatalogsEnumModel> _settingCatalogs = new ObservableCollection<SettingCatalogsEnumModel>();

        public int Id
        {
            get => _id;
            set => SetProperty(ref _id, value);
        }
        public string Catalog
        {
            get => _catalog;
            set => SetProperty(ref _catalog, value);
        }

        public string Name
        {
            get => _name;
            set => SetProperty(ref _name, value);
        }

        public string Value
        {
            get => _value;
            set => SetProperty(ref _value, value);
        }

        public bool IsEdit
        {
            get => _isEdit;
            set => SetProperty(ref _isEdit, value);
        }

        public ObservableCollection<SettingCatalogsEnumModel> SettingCatalogs
        {
            get => _settingCatalogs;
            set => SetProperty(ref _settingCatalogs, value);
        }

        public SettingCatalogsEnumModel SelectedSettingCatalog
        {
            get => _selectedSettingCatalog;
            set => SetProperty(ref _selectedSettingCatalog, value);
        }
        #endregion

        #region Commands

        public ICommand SaveSettingCommand => new Command(async () => await SaveSetting());

        private async Task SaveSetting()
        {
            var setting = new Setting
            {
                Id = Id,
                Name = Name,
                Value = Value,
                Catalog = SelectedSettingCatalog.Name.ToString(),
            };

            await _dataService.SaveSetting(setting);

            await Application.Current.MainPage.Navigation.PopAsync();
        }

        public async Task LoadSetting(int id)
        {
            if (id != 0)
            {
                var setting = await _dataService.GetSetting(id);

                if (setting != null)
                {
                    Id = setting.Id;
                    SelectedSettingCatalog = SettingCatalogs.FirstOrDefault(t => t.Name == (SettingCatalogEnum)Enum.Parse(typeof(SettingCatalogEnum), setting.Catalog));
                    Name = setting.Name;
                    Value = setting.Value;
                    IsEdit = true;
                }
            }
        }
        #endregion
    }
}
