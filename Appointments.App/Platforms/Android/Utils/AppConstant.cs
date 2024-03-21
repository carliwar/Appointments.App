using Java.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Appointments.App.Platforms.Android.Utils
{
    public static class AppConstant
    {
        public static long GetDateTimeMS(DateTime date)
        {
            Calendar c = Calendar.GetInstance(Java.Util.TimeZone.Default);

            c.Set(Java.Util.CalendarField.DayOfMonth, date.Day);
            c.Set(Java.Util.CalendarField.HourOfDay, date.Hour);
            c.Set(Java.Util.CalendarField.Minute, date.Minute);
            c.Set(Java.Util.CalendarField.Month, date.Month - 1);
            c.Set(Java.Util.CalendarField.Year, date.Year);

            return c.TimeInMillis;
        }

        public static object GetConstantValue<T>(string constantName)
        {
            Type type = typeof(T);
            var fieldInfo = type.GetField(constantName, System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static);

            if (fieldInfo != null)
            {
                return fieldInfo.GetValue(null);
            }
            else
            {
                return null;
            }
        }
    }
}
