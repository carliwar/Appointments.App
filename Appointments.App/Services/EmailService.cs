using System;
using Appointments.App.Models;
using MimeKit;
using MailKit.Net.Smtp;
using MimeKit.Text;
using Appointments.App.Utils.Templates;

namespace Appointments.App.Services
{
    public static class EmailService
    {
        private const string _smtpServer = "smtp.gmail.com";
        //private const int _smtpPort = 465;


        public static void Send(AppEmail email, SignatureModel signatureModel)
        {
            if (email == null)
            {
                throw new ArgumentNullException(nameof(email));
            }

            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("JeDent", email.Sender));
            message.To.Add(new MailboxAddress(email.To, email.To));
            message.Subject = email.Subject;

            message.Body = new TextPart(TextFormat.Html)
            {
                Text = email.Body + "<br><br>" + Signature.GetSignature(signatureModel.Name, signatureModel.Title, signatureModel.Email, signatureModel.Phone,
                signatureModel.Address, signatureModel.Facebook, signatureModel.Website, signatureModel.Company)
            };

            using (var client = new SmtpClient())
            {
                client.Connect(_smtpServer, 587, false);

                // Note: only needed if the SMTP server requires authentication
                client.Authenticate(email.Sender, email.Password);

                client.Send(message);
                client.Disconnect(true);
            }            
        }
    }
}
