using System;

namespace Appointments.App.ViewModels
{
    public class CreateUserRequestViewModel : BasePageViewModel
    {        
        private string _identification;
        private string _firstName;
        private string _lastName;
        private DateTime _birthDate = DateTime.Today;
        public string Identificacion
        {
            get => _identification;
            set => SetProperty(ref _identification, value);
        }

        public string FirstName
        {
            get => _firstName;
            set => SetProperty(ref _firstName, value);
        }
        public string LastName
        {
            get => _lastName;
            set => SetProperty(ref _firstName, value);
        }
        public DateTime BirthDate
        {
            get => _birthDate;
            set => SetProperty(ref _birthDate, value);
        }
    }
}
