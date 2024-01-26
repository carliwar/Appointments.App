using Appointments.App.Models.DataModels;
using Appointments.App.Models.Enum;
using Appointments.App.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using static Xamarin.Forms.Device;

namespace Appointments.App.ViewModels.AppointmentType
{
    public class AppointmentTypeViewModel : BasePageViewModel
    {
        private readonly IDataService _dataService;        
        public AppointmentTypeViewModel()
        {
            _dataService = new DataService();
        }

        #region Properties

        private Models.DataModels.AppointmentType _appointmentType;
        private int _id;
        private string _name;
        private string _description;
        private AppointmentDurationEnum _appointmentDurationEnum;
        private string _color;
        private bool _enabled;
        private bool _isEdit = false;
        private ObservableCollection<AppointmentDurationEnum> _appointmentDurations = new ObservableCollection<AppointmentDurationEnum>();

        public Models.DataModels.AppointmentType AppointmentType { get => _appointmentType; set => SetProperty(ref _appointmentType, value); }
        public int Id { get => _id; set => SetProperty(ref _id, value); }
        public string Name { get => _name; set => SetProperty(ref _name, value); }
        public string AppointmentTypeDescription { get => _description; set => SetProperty(ref _description, value); }
        public AppointmentDurationEnum AppointmentDurationEnum { get => _appointmentDurationEnum; set => SetProperty(ref _appointmentDurationEnum, value); }
        public string Color { get => _color; set => SetProperty(ref _color, value); }
        public bool Enabled { get => _enabled; set => SetProperty(ref _enabled, value); }
        public bool IsEdit { get => _isEdit; set => SetProperty(ref _isEdit, value); }
        public ObservableCollection<AppointmentDurationEnum> AppointmentDurations { get => _appointmentDurations; set => SetProperty(ref _appointmentDurations, value); }
        #endregion

        #region Commands
        public ICommand SaveAppointmentTypeCommand => new Command(async (item) => await Save());

        public async Task Save()
        {
            var appointmentType = new Models.DataModels.AppointmentType
            {
                Id = Id,
                Name = Name,
                Description = AppointmentTypeDescription,
                DefaultDuration = AppointmentDurationEnum,
                ColorCode = Color,
                Enabled = Enabled
            };

            var result = await _dataService.SaveAppointmentType(appointmentType);

            if (result.Success)
            {
                var tipoOperacion = IsEdit ? "creada" : "actualizada";

                await Application.Current.MainPage.DisplayAlert("Operación Exitosa!", $"Tipo de cita {tipoOperacion}.", "Ok");
                await Application.Current.MainPage.Navigation.PopAsync();
            }
            else
            {
                await Application.Current.MainPage.DisplayAlert("Errores: ", string.Join(" / ", result.Errors), "Ok");
            }
        }

        #endregion

            #region Methods

        public async Task LoadAppointmentType(int id)
        {
            if(id > 0)
            {
                AppointmentType = await _dataService.GetAppointmentType(id);

                if(AppointmentType != null)
                {
                    Id = AppointmentType.Id; 
                    Name = AppointmentType.Name; 
                    AppointmentTypeDescription = AppointmentType.Description;
                    AppointmentDurationEnum = AppointmentType.DefaultDuration;
                    Color = AppointmentType.ColorCode;
                    Enabled = AppointmentType.Enabled;

                    IsEdit = true;
                }
            }
        }

        #endregion
    }
}
