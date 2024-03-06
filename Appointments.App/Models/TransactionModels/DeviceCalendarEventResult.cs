namespace Appointments.App.Models.TransactionModels
{
    public class DeviceCalendarEventResult
    {
        public bool IsSuccess { get => string.IsNullOrEmpty(Error); }
        public string EventID { get; set; }
        public string ReminderID { get; set; }
        public string Error { get; set; }
    }
}
