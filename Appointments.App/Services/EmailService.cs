using System;
using System.Net.Mail;
using System.Net;
using Appointments.App.Models;

namespace Appointments.App.Services
{
    public static class EmailService
    {
        private const string _smtpServer = "smtp.gmail.com";
        private const int _smtpPort = 465;
        private const string _userName = "jesskfm01@gmail.com";
        private const string _password = "clave";


        public static void Send(AppEmail email)
        {
            if (email == null)
                throw new ArgumentNullException(nameof(email));

            using (var client = new SmtpClient(_smtpServer, _smtpPort))
            {
                client.Credentials = new NetworkCredential(_userName, _password);
                client.EnableSsl = true;

                var mailMessage = new MailMessage
                {
                    From = new MailAddress(_userName),
                    Subject = email.Subject,
                    Body = email.Body,
                    IsBodyHtml = true
                };
                mailMessage.To.Add(email.To);

                client.Send(mailMessage);
            }
        }
    }
}
