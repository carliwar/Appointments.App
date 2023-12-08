using SQLite;

namespace Appointments.App.Models.DataModels
{
    public class Setting
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public string Catalog { get; set; }
        public string Name { get; set; }
        public string Value { get; set; }
    }
}
