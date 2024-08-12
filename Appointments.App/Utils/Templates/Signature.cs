using static Xamarin.Essentials.Permissions;
using System.Reflection;
using Xamarin.Essentials;

namespace Appointments.App.Utils.Templates
{
    public static class Signature
    {
        public static string GetSignature(string name, string title, string email, string phone, string address, string facebook, string website, string company)
        {
            return $"</br><table style=\"width: 420px; font-size: 10pt; font-family: Arial, sans-serif; background: transparent !important;\" cellpadding=\"0\" cellspacing=\"0\" border=\"0\">" +
                $"\r\n<tbody>\r\n    " +
                $"<tr>\r\n       " +
                $" <td style=\"font-size: 10pt; font-family: Arial, sans-serif; border-right: 1px solid #00a78d; width:200px; padding-right: 10px; vertical-align: top;  padding-bottom: 20px;\" valign=\"top\">\r\n" +
                $"<p style=\"margin-bottom:25px; padding-bottom: 0px; line-height:1.0\">\r\n " +
                $"<strong><span style=\"font-size: 12pt; font-family: Arial, sans-serif; color:#00a78d; line-height: 18pt;\">{name}</span></strong>\r\n " +
                $"<span style=\"font-family: Arial, sans-serif; font-size:9pt; color:#717171;  line-height: 14pt;\"><br>{title}</span>\r\n</p>\r\n" +
                $"<span>\r\n <a href=\"{website}\" target=\"_blank\"></a>\r\n\r\n</span>\r\n</td>\r\n  \r\n" +
                $"<td valign=\"top\" style=\"padding-left: 30px; padding-bottom: 20px;\"> \r\n\r\n" +
                $"<span><span style=\"color: #262626;\"><strong>Email:</strong></span><a href=\"mailto:{email}\" style=\"text-decoration: none; font-size: 9pt; font-family: Arial, sans-serif; color:#262626;\">" +
                $"<span style=\"text-decoration: none; font-size: 9pt; font-family: Arial, sans-serif; color:#262626;\">{email}</span></a><br></span>\r\n" +
                $"<span><span style=\"color: #262626;\"><strong>Tel:</strong></span><span style=\"font-size: 9pt; font-family: Arial, sans-serif; color:#262626;\"> {phone}<br></span></span>\r\n<span>\r\n\r\n" +
                $"<span style=\"color: #262626;\"><strong>A:</strong></span>\r\n<span style=\"font-family: Arial, sans-serif; font-size:9pt; color:#262626;\">{company}<span>,</span></span>" +
                $"\r\n<span>\r\n<span style=\"font-size: 9pt; font-family: Arial, sans-serif; color: #262626;\">{address}<span></span>" +
                $"</span>\r\n</span>\t\t\r\n\r\n</td>\r\n</tr>\r\n<tr>\r\n<td style=\"border-right: 1px solid #00a78d;vertical-align: top;\" valign=\"top\">\r\n<a href=\"{website}\" target=\"_blank\" rel=\"noopener\" style=\"font-size: 9pt; font-family: Arial, sans-serif; text-decoration:none; color: #00a78d; font-weight: bold;\"><span style=\"font-size: 9pt; font-family: Arial, sans-serif; text-decoration:none; color: #00a78d; font-weight: bold;\">{website}</span></a>\r\n</td>\r\n" +
                $"<td valign=\"top\" style=\"padding-left: 30px;\"> \r\n<span><a href=\"{facebook}\" target=\"_blank\" rel=\"noopener\"><img border=\"0\" width=\"26\" src=\"https://cdn-icons-png.freepik.com/512/145/145802.png\" alt=\"facebook icon\" style=\"border:0; height:26px; width:26px\"></a>&nbsp;</span>" +
                $"</td>\r\n</tr>\r\n</tbody>\r\n</table>";
        }        
    }
}
