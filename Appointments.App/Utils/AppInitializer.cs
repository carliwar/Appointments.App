using Appointments.App.Models.DataModels;
using Appointments.App.Models.Enum;
using Appointments.App.Services;
using System.Linq;
using System.Threading.Tasks;

namespace Appointments.App.Utils
{
    public class AppInitializer
    {
        private readonly IDataService _dataService;
        public AppInitializer()
        {
            _dataService = new DataService();
        }

        public async Task InitializeApp()
        {
            await InitializeSettings();
        }

        private async Task InitializeSettings()
        {
            var missingSettings = await _dataService.GetBasicSettings();

            foreach (var setting in missingSettings.Where(t => t.Catalog.ToLower() == SettingCatalogEnum.basic.ToString().ToLower()))
            {
                switch (setting.Name.ToLower())
                {
                    case "brand":
                        setting.Value = DefaultValues.APPOINTMENT_BRAND;                        
                        break;
                    case "email":
                        setting.Value = DefaultValues.APPOINTMENT_BRAND;
                        break;
                    case "password":
                        setting.Value = DefaultValues.APPLICATION_PASSWORD;
                        break;
                }

                await _dataService.SaveSetting(setting);                
            }

            foreach (var setting in missingSettings.Where(t => t.Catalog.ToLower() == SettingCatalogEnum.notifications.ToString().ToLower()))
            {
                switch (setting.Name.ToLower())
                {
                    case "notification_time":
                        setting.Value = DefaultValues.NOTIFICATION_TIME;
                        break;
                    case "notification_day_to_show":
                        setting.Value = DefaultValues.NOTIFICATION_DAY;
                        break;
                }

                await _dataService.SaveSetting(setting);
            }

            foreach (var setting in missingSettings.Where(t => t.Catalog.ToLower() == SettingCatalogEnum.signature.ToString().ToLower()))
            {
                switch (setting.Name.ToLower())
                {
                    case "Name":
                        setting.Value = DefaultValues.SIGNATURE_NAME;
                        break;
                    case "Title":
                        setting.Value = DefaultValues.SPECIALIST_TITLE;
                        break;
                    case "Phone":
                        setting.Value = DefaultValues.PHONE;
                        break;
                    case "Address":
                        setting.Value = DefaultValues.ADDRESS;
                        break;
                    case "Facebook":
                        setting.Value = DefaultValues.FACEBOOK;
                        break;
                    case "Website":
                        setting.Value = DefaultValues.WEBSITE;
                        break;
                }

                await _dataService.SaveSetting(setting);
            }
        }
        
    }
}
