﻿using Appointments.App.Models.DataModels;
using Appointments.App.Models.Enum;
using Appointments.App.Services;
using Appointments.App.Views.Users;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace Appointments.App.ViewModels
{
    public class CreateUserRequestViewModel : BasePageViewModel
    {
        public CreateUserRequestViewModel()
        {
            //UserTypes = new ObservableCollection<UserType>(Enum.GetValues(typeof(UserType)).OfType<UserType>().ToList());
            _dataService = new DataService();
        }

        #region Properties
        private string _identification;
        private string _phone;
        private string _firstName;
        private string _lastName;
        private DateTime _birthDate = DateTime.Today;
        private readonly IDataService _dataService;
        private UserType _selectedUserType;

        public string Identificacion
        {
            get => _identification;
            set => SetProperty(ref _identification, value);
        }
        public string Phone
        {
            get => _phone;
            set => SetProperty(ref _phone, value);
        }

        public string FirstName
        {
            get => _firstName;
            set => SetProperty(ref _firstName, value);
        }
        public string LastName
        {
            get => _lastName;
            set => SetProperty(ref _lastName, value);
        }
        public DateTime BirthDate
        {
            get => _birthDate;
            set => SetProperty(ref _birthDate, value);
        }

        public UserType SelectedUserType
        {
            get => _selectedUserType;
            set => SetProperty(ref _selectedUserType, value);
        }
        #endregion

        #region Commands
        public ICommand CreatePersonCommand => new Command((item) => CreatePerson(item));
        public ICommand SelectUserTypeCommand => new Command((item) => SelectUserType(item));
        public ICommand ImportContactCommand => new Command(async (item) => await ImportContactAsync(item));

        private async Task ImportContactAsync(object item)
        {
            var contact = await Contacts.PickContactAsync();
            if(contact != null)
            {
                FirstName = contact.GivenName;
                LastName = contact.FamilyName;
                Phone = contact.Phones.FirstOrDefault()?.PhoneNumber?.Replace(" ", "");                
            }
        }
        private void SelectUserType(object item)
        {
            SelectedUserType = (UserType)item;
        }

        private async void CreatePerson(object item)
        {
            var person = new User
            {
                Identification = Identificacion,
                Name = FirstName,
                LastName = LastName,
                BirthDate = BirthDate,
                Phone = Phone,
                UserType = UserType.Paciente
            };
            var result = await _dataService.CreateUser(person);

            if (result != null)
            {
                await Application.Current.MainPage.DisplayAlert("Operación Exitosa!", "Usuario creado", "Ok");
                await Application.Current.MainPage.Navigation.PopAsync();
            }
            else
            {
                await Application.Current.MainPage.DisplayAlert("Error", "Contacte al administrador", "Ok");
            }
        }



        #endregion

        #region Private Methods


        #endregion
    }
}
