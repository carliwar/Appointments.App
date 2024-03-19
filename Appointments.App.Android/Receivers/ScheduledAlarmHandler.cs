using Android.App;
using Android.Content;
using Android.Media;
using AndroidX.Core.App;
using Appointments.App.Droid.Services;
using Appointments.App.Models.TransactionModels;
using System;
using System.IO;
using System.Xml.Serialization;

namespace Appointments.App.Droid.Receivers
{
    [BroadcastReceiver(Enabled = true, Label = "Local Notifications Broadcast Receiver")]
    public class ScheduledAlarmHandler : BroadcastReceiver
    {

        public const string LocalNotificationKey = "LocalNotification";

        public override void OnReceive(Context context, Intent intent)
        {
            var extra = intent.GetStringExtra(LocalNotificationKey);
            var notification = DeserializeNotification(extra);
            //Generating notification      
            var builder = new NotificationCompat.Builder(Application.Context)
                .SetContentTitle(notification.Title)
                .SetContentText(notification.Body)
                .SetSmallIcon(notification.IconId)
                .SetSound(RingtoneManager.GetDefaultUri(RingtoneType.Ringtone))
                .SetAutoCancel(true);

            var resultIntent = AndroidNotificationService.GetLauncherActivity();
            resultIntent.SetFlags(ActivityFlags.NewTask | ActivityFlags.ClearTask);
            var stackBuilder = AndroidX.Core.App.TaskStackBuilder.Create(Application.Context);
            stackBuilder.AddNextIntent(resultIntent);

            Random random = new Random();
            int randomNumber = random.Next(9999 - 1000) + 1000;

            var resultPendingIntent =
                stackBuilder.GetPendingIntent(randomNumber, (int)PendingIntentFlags.Immutable);
            builder.SetContentIntent(resultPendingIntent);
            // Sending notification      
            var notificationManager = NotificationManagerCompat.From(Application.Context);
            notificationManager.Notify(randomNumber, builder.Build());
        }

        private AppNotification DeserializeNotification(string notificationString)
        {

            var xmlSerializer = new XmlSerializer(typeof(AppNotification));
            using (var stringReader = new StringReader(notificationString))
            {
                var notification = (AppNotification)xmlSerializer.Deserialize(stringReader);
                return notification;
            }
        }
    }
}