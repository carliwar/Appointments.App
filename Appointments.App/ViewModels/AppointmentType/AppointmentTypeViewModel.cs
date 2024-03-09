using Acr.UserDialogs;
using Appointments.App.Models.DataModels;
using Appointments.App.Models.Enum;
using Appointments.App.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
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

            foreach (AppointmentDurationEnum enumValue in Enum.GetValues(typeof(Models.Enum.AppointmentDurationEnum)))
            {
                string customString = GetEnumDescription(enumValue);

                var appointmentDuration = new Models.DataModels.AppointmentDuration
                {
                    Name = enumValue,
                    Description = customString
                };
                AppointmentDurations.Add(appointmentDuration);
            }
        }

        #region Properties

        private Models.DataModels.AppointmentType _appointmentType;
        private int _id;
        private string _name;
        private string _description;
        private Models.DataModels.AppointmentDuration _selectedAppointmentDuration;
        private string _color;
        private Color _colorApp;
        private bool _enabled;
        private bool _isEdit = false;
        private ObservableCollection<AppointmentDuration> _appointmentDurations = new ObservableCollection<AppointmentDuration>();


        public Models.DataModels.AppointmentType AppointmentType { get => _appointmentType; set => SetProperty(ref _appointmentType, value); }
        public int Id { get => _id; set => SetProperty(ref _id, value); }
        public string Name { get => _name; set => SetProperty(ref _name, value); }
        public string AppointmentTypeDescription { get => _description; set => SetProperty(ref _description, value); }
        public Models.DataModels.AppointmentDuration SelectedAppointmentDuration { get => _selectedAppointmentDuration; set => SetProperty(ref _selectedAppointmentDuration, value); }
        public string Color { get => _color; set => SetProperty(ref _color, value); }
        public Color ColorApp { get => _colorApp; set => SetProperty(ref _colorApp, value); }
        public bool Enabled { get => _enabled; set => SetProperty(ref _enabled, value); }
        public bool IsEdit { get => _isEdit; set => SetProperty(ref _isEdit, value); }
        public ObservableCollection<AppointmentDuration> AppointmentDurations { get => _appointmentDurations; set => SetProperty(ref _appointmentDurations, value); }
        #endregion

        #region Commands
        public ICommand SaveAppointmentTypeCommand => new Command(async (item) => await Save());

        public async Task Save()
        {
            if (Id == 0)
            {
                Enabled = true;
            }

            UserDialogs.Instance.ShowLoading();

            try
            {
                var appointmentType = new Models.DataModels.AppointmentType
                {
                    Id = Id,
                    Name = Name,
                    Description = AppointmentTypeDescription,
                    DefaultDuration = SelectedAppointmentDuration.Name,
                    ColorCode = ColorApp.ToHex(),
                    Enabled = Enabled
                };

                var result = await _dataService.SaveAppointmentType(appointmentType);

                if (result.Success)
                {
                    UserDialogs.Instance.HideLoading();

                    var tipoOperacion = IsEdit ? "actualizada" : "creada";

                    await Application.Current.MainPage.DisplayAlert("Operación Exitosa!", $"Tipo de cita {tipoOperacion}.", "Ok");
                    await Application.Current.MainPage.Navigation.PopAsync();
                }
                else
                {
                    UserDialogs.Instance.HideLoading();

                    await Application.Current.MainPage.DisplayAlert("Errores: ", string.Join(" / ", result.Errors), "Ok");
                }
            }
            catch (Exception e)
            {
                UserDialogs.Instance.HideLoading();
                await Application.Current.MainPage.DisplayAlert("Error", $"Contacte al administrador: {e.Message}", "Ok");
            }
        }

        #endregion

        #region Methods

        public async Task LoadAppointmentType(int id)
        {
            if (id > 0)
            {
                UserDialogs.Instance.ShowLoading();

                try
                {
                    IsEdit = true;

                    AppointmentType = await _dataService.GetAppointmentType(id);

                    if (AppointmentType != null)
                    {
                        Id = AppointmentType.Id;
                        Name = AppointmentType.Name;
                        AppointmentTypeDescription = AppointmentType.Description;
                        SelectedAppointmentDuration = AppointmentDurations.SingleOrDefault(t => t.Name == AppointmentType.DefaultDuration);
                        Color = AppointmentType.ColorCode;
                        ColorApp = Xamarin.Forms.Color.FromHex(AppointmentType.ColorCode);
                        Enabled = AppointmentType.Enabled;

                        IsEdit = true;
                    }
                }
                catch (Exception e)
                {
                    UserDialogs.Instance.HideLoading();
                    await Application.Current.MainPage.DisplayAlert("Error", $"Contacte al administrador: {e.Message}", "Ok");
                }

                UserDialogs.Instance.HideLoading();
            }
        }

        #endregion

        private string GetEnumDescription(Models.Enum.AppointmentDurationEnum value)
        {
            var fieldInfo = value.GetType().GetField(value.ToString());

            var attributes = (DescriptionAttribute[])fieldInfo.GetCustomAttributes(
                typeof(DescriptionAttribute), false);

            return attributes.Length > 0 ? attributes[0].Description : value.ToString();
        }
    }
}
