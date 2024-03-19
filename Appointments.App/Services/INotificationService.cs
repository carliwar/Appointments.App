using System;

namespace Appointments.App.Services
{
    public interface INotificationService
    {
        event EventHandler NotificationReceived;
        void Initialize();
        void LocalNotification(string title, string body, int id, DateTime notifyTime);
        void Cancel();
        void ReceiveNotification(string title, string message);

        void DeleteNotification();
    }
}
