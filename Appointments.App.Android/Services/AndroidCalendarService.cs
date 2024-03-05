using Android.Content;
using Android.Provider;
using Appointments.App.Droid.Services;
using Appointments.App.Droid.Utils;
using Appointments.App.Services;
using System;
using System.Threading.Tasks;
using Appointments.App.Models.DataModels;

[assembly: Xamarin.Forms.Dependency(typeof(AndroidCalendarService))]
namespace Appointments.App.Droid.Services
{
    public class AndroidCalendarService : IDeviceCalendarService
    {

        public async Task<bool> AddEventToCalendar(AndroidAppointment appointment)
        {
            bool isEventAdded = true;
            try
            {

                ContentResolver contentResolver = Android.App.Application.Context.ContentResolver;

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
                string reminderUriString = "content://com.android.calendar/reminders";

                ContentValues reminderValues = new ContentValues();
                reminderValues.Put(CalendarContract.Reminders.InterfaceConsts.EventId, eventId);
                reminderValues.Put(CalendarContract.Reminders.InterfaceConsts.Method, (int) RemindersMethod.Alert);
                reminderValues.Put(CalendarContract.Reminders.InterfaceConsts.Minutes, appointment.ReminderMinutes); // Set the reminder time in minutes
                Android.Net.Uri url = Android.Net.Uri.Parse(reminderUriString);

                // Insert the reminder into the reminders table
                contentResolver.Insert(CalendarContract.Reminders.ContentUri, reminderValues);

                return await Task.FromResult(isEventAdded);
            }
            catch (Exception ex)
            {
                isEventAdded = false;
            }
            return await Task.FromResult(isEventAdded);
        }
    }
}