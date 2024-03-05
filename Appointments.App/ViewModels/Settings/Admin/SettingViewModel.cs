using Appointments.App.Models.DataModels;
using Appointments.App.Models.Enum;
using Appointments.App.Services;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Essentials;
using Xamarin.Forms;
using static Xamarin.Essentials.Permissions;

namespace Appointments.App.ViewModels.Settings.Admin
{
    public class SettingViewModel : BasePageViewModel
    {
        public SettingViewModel()
        {
            _dataService = new DataService();
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
        #endregion

        #region Commands

        public ICommand SaveSettingCommand => new Command(async () =>  await SaveSetting());

        private async Task SaveSetting()
        {
            var setting = new Setting
            {
                Id = Id,
                Name = Name,
                Value = Value,
                Catalog = Catalog
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
                    Catalog = setting.Catalog;
                    Name = setting.Name;
                    Value = setting.Value;
                    IsEdit = true;
                }
            }
        }
        #endregion
    }
}
