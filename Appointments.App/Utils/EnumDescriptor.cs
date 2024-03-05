using System.ComponentModel;

namespace Appointments.App.Utils
{
    public static class EnumDescriptor
    {
        public static string GetEnumDescription(Models.Enum.AppointmentDurationEnum value)
        {
            var fieldInfo = value.GetType().GetField(value.ToString());

            var attributes = (DescriptionAttribute[])fieldInfo.GetCustomAttributes(
                typeof(DescriptionAttribute), false);

            return attributes.Length > 0 ? attributes[0].Description : value.ToString();
        }
    }
}
