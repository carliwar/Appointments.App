using Android.Content;
using Android.Provider;
using Appointments.App.Droid.Services;
using Appointments.App.Droid.Utils;
using Appointments.App.Services;
using System;
using System.Threading.Tasks;
using Appointments.App.Models.DataModels;
using Appointments.App.Models.TransactionModels;

[assembly: Xamarin.Forms.Dependency(typeof(AndroidCalendarService))]
namespace Appointments.App.Droid.Services
{
    public class AndroidCalendarService : IDeviceCalendarService
    {
        const string _reminderUriString = "content://com.android.calendar/reminders";
        ContentResolver contentResolver = Android.App.Application.Context.ContentResolver;

        public async Task<DeviceCalendarEventResult> AddEventToCalendar(AndroidAppointment appointment)
        {
            var result = new DeviceCalendarEventResult();

            try
            {

                ContentValues appointmentCalendarEvent = new ContentValues();
                appointmentCalendarEvent.Put(CalendarContract.Events.InterfaceConsts.CalendarId, appointment.CalendarID); // Default calendar
                appointmentCalendarEvent.Put(CalendarContract.Events.InterfaceConsts.Title, appointment.Title);
                appointmentCalendarEvent.Put(CalendarContract.Events.InterfaceConsts.Description, appointment.Description);
                appointmentCalendarEvent.Put(CalendarContract.Events.InterfaceConsts.EventTimezone, "UTC");
                appointmentCalendarEvent.Put(CalendarContract.Events.InterfaceConsts.EventEndTimezone, "UTC");
                appointmentCalendarEvent.Put(CalendarContract.Events.InterfaceConsts.Dtstart, AppConstant.GetDateTimeMS(appointment.StartDate));
                appointmentCalendarEvent.Put(CalendarContract.Events.InterfaceConsts.Dtend, AppConstant.GetDateTimeMS(appointment.EndDate));
                appointmentCalendarEvent.Put(CalendarContract.Events.InterfaceConsts.EventLocation, appointment.Location);
                appointmentCalendarEvent.Put(CalendarContract.Events.InterfaceConsts.AllDay, false);

                Android.Net.Uri eventUri = contentResolver.Insert(CalendarContract.Events.ContentUri, appointmentCalendarEvent);

                // Get the event ID from the inserted event URI
                long eventId = long.Parse(eventUri.LastPathSegment);
                result.EventID = eventId.ToString();


                ContentValues reminderValues = new ContentValues();
                reminderValues.Put(CalendarContract.Reminders.InterfaceConsts.EventId, eventId);
                reminderValues.Put(CalendarContract.Reminders.InterfaceConsts.Method, (int) RemindersMethod.Alert);
                reminderValues.Put(CalendarContract.Reminders.InterfaceConsts.Minutes, appointment.ReminderMinutes); // Set the reminder time in minutes
                Android.Net.Uri url = Android.Net.Uri.Parse(_reminderUriString);

                // Insert the reminder into the reminders table
                result.ReminderID = contentResolver.Insert(CalendarContract.Reminders.ContentUri, reminderValues).LastPathSegment;
            }
            catch (Exception ex)
            {
                result.Error = ex.Message;
            }
            return await Task.FromResult(result);
        }

        public async Task<bool> DeleteEventFromCalendar(CalendarEventLog calendarEventLog)
        {
            try
            {
                Android.Net.Uri reminderUri = Android.Net.Uri.Parse($"{_reminderUriString}/{calendarEventLog.ReminderId}");
                var deletedReminders = contentResolver.Delete(reminderUri, null);

                Android.Net.Uri eventUri = Android.Net.Uri.Parse($"{CalendarContract.Events.ContentUri}/{calendarEventLog.EventId}");
                var deletedEvents = contentResolver.Delete(eventUri, null);

                return await Task.FromResult(true);
            }
            catch (Exception)
            {
                return await Task.FromResult(false); ;
            }
        }
    }
}