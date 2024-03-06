using Appointments.App.Models.DataModels;
using Appointments.App.Models.TransactionModels;
using System.Threading.Tasks;

namespace Appointments.App.Services
{
    public interface IDeviceCalendarService
    {
        Task<DeviceCalendarEventResult> AddEventToCalendar(AndroidAppointment appointment);
        Task<bool> DeleteEventFromCalendar(CalendarEventLog calendarEventLog);
    }
}
