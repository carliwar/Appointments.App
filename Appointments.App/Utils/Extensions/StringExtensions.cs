using System;

namespace Appointments.App.Utils.Extensions
{
    public static class StringExtensions
    {
        public static string FormatPhoneForWhatsapp(this string phone)
        {
            if (phone == null)
                return string.Empty;

            string formattedString = phone.Trim();

            if (formattedString.Length > 0)
            {
                // if phone starts with 09 replace that with +5939
                if (phone.StartsWith("09"))
                {
                    formattedString = "+5939" + phone.Substring(2);
                    Console.WriteLine(formattedString);
                }
            }
            return formattedString;
        }
    }
}
