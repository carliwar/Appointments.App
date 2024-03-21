using System;
using System.Globalization;


namespace Appointments.App.Utils.ValueConverters
{
    public class BackgroundColorValueConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var colorValue = "#c6e1f7";

            if (value is Models.DataModels.AppointmentType appType && appType.ColorCode != null)
            {
                colorValue = appType.ColorCode;
            }

            return colorValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
