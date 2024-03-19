using System;

namespace Appointments.App.Models.TransactionModels
{
    public class AppNotification
    {
        public string Title { get; set; }

        public string Body { get; set; }

        public int Id { get; set; }

        public int IconId { get; set; }

        public DateTime NotifyTime { get; set; }
    }
}
