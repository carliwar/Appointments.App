using Appointments.App.Models.DataModels;
using System;
using System.Threading.Tasks;

namespace Appointments.App.Services
{
    public interface IDeviceCalendarService
    {
        Task<bool> AddEventToCalendar(AndroidAppointment appointment);
    }
}
