using Appointments.App.Models.Enum;
using Appointments.App.Services;
using System.Collections.ObjectModel;

namespace Appointments.App.ViewModels
{
    public class CreateSettingViewModel : BasePageViewModel
    {
        public CreateSettingViewModel()
        {
           _dataService = new DataService();
        }

        #region Properties

        private string _catalog;
        private string _name;
        private string _value;
        private ObservableCollection<AppointmentTypeEnum> _appointmentTypes = new ObservableCollection<AppointmentTypeEnum>();
        private readonly IDataService _dataService;

        public string Catalog
        {
            get => _catalog;
            set => SetProperty(ref _catalog, value);
        }
        public string Name
        {
            get => _name;
            set => SetProperty(ref _name, value);
        }

        public string Value
        {
            get => _value;
            set => SetProperty(ref _value, value);
        }

        public ObservableCollection<AppointmentTypeEnum> AppointmentTypes
        {
            get => _appointmentTypes;
            set => SetProperty(ref _appointmentTypes, value);
        }
        #endregion

    }
}
