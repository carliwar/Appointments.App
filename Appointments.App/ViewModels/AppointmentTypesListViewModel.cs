using Appointments.App.Models.DataModels;
using Appointments.App.Services;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace Appointments.App.ViewModels
{
    public class AppointmentTypesListViewModel : BasePageViewModel
    {
        #region Temp Properties
        private readonly IDataService _dataService;

        #endregion

        public AppointmentTypesListViewModel()
        {
            _dataService = new DataService();
        }

        #region Methods
        private ObservableCollection<Models.DataModels.AppointmentType> _appointmentTypes;

        public ObservableCollection<Models.DataModels.AppointmentType> AppointmentTypes
        {
            get => _appointmentTypes;
            set => SetProperty(ref _appointmentTypes, value);
        }
        #endregion

        #region Commands
        public ICommand SearchAppointmentTypesCommand => new Command(async (item) => await SearchAppointmentTypeAsync(item));
        public ICommand UserAppointmentsCommand => new Command(async (item) => await LoadAppointmentTypes(item));

        public async Task InitializeAppointmentTypes(string searchText = "")
        {
            AppointmentTypes.Clear();

            var appointmentTypes = await _dataService.GetAppointmentTypes(searchText);
            appointmentTypes = appointmentTypes.OrderBy(t => t.Name).ToList();

            foreach (var user in appointmentTypes)
            {
                AppointmentTypes.Add(user);
            }
        }

        private async Task SearchAppointmentTypeAsync(object sender)
        {

            if (sender == null)
            {
                await InitializeAppointmentTypes();
            }
            else
            {
                var searchText = string.Empty;

                if (sender is TextChangedEventArgs search)
                {
                    searchText = search.NewTextValue;
                }
                else if (sender is string @string)
                {
                    searchText = @string;
                }

                await InitializeAppointmentTypes(searchText);
            }
        }

        private async Task CreateAppointmentType()
        {
            //await Application.Current.MainPage.Navigation.PushAsync(new CreateUserPage());
        }

        private async Task LoadAppointmentTypes(object appointmentType)
        {
            if (appointmentType is Models.DataModels.AppointmentType)
            {
                //await Application.Current.MainPage.Navigation.PushAsync(new UserAppointmentsPage((User)user));
            }

        }
        #endregion
    }
}
