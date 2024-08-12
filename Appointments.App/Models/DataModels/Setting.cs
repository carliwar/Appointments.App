using Appointments.App.Models.Enum;
using SQLite;
using Xamarin.Forms;

namespace Appointments.App.Models.DataModels
{
    public class Setting
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public string Catalog { get; set; }
        public string Name { get; set; }
        public string Value { get; set; }

        [Ignore]
        public System.Drawing.Color SettingColor {
            get
            {
                Color settingColor = Color.FromHex(DefaultValues.BUTTON_PRIMARY_COLOR);

                switch (Catalog.ToLower())
                {
                    case "basic":
                        settingColor = Color.FromHex(DefaultValues.BASIC_SETTING_COLOR);
                        break;
                    case "signature":
                        settingColor = Color.FromHex(DefaultValues.SIGNATURE_SETTING_COLOR);
                        break;
                    case "notifications":
                        settingColor = Color.FromHex(DefaultValues.NOTIFICATION_SETTING_COLOR);
                        break;

                }

                return settingColor;
            } 
        }

        [Ignore]
        public string DisplayName { get => $"{Catalog} - {Name}"; }
    }
}
