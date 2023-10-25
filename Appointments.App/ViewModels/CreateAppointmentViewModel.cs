using Appointments.App.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace Appointments.App.ViewModels
{
    public class CreateAppointmentViewModel : BasePageViewModel
    {
        #region Temp Properties

        List<Person> people = new List<Person>
            {
                //create 5 random Person

                new Person
                {
                    Id = 1,
                    Identification = "1234567890",
                    Name = "Name1",
                    LastName = "LastName1"
                },
                new Person
                {
                    Id = 2,
                    Identification = "333444111",
                    Name = "Juan",
                    LastName = "Armas"
                },
                new Person
                {
                    Id = 3,
                    Identification = "6699003311",
                    Name = "Luis",
                    LastName = "Loza"
                },
                new Person
                {
                    Id = 4,
                    Identification = "7775551112",
                    Name = "Jess",
                    LastName = "Villas"
                }
            };

        #endregion

        #region Properties
        public int Id { get; set; }
        public string PersonValue { get; set; }
        public DateTime GivenDate { get; set; }
        public ObservableCollection<AppointmentType> Types { get; set; } = new ObservableCollection<AppointmentType>();
        public ObservableCollection<Person> People { get; set; }

        public AppointmentType SelectedType { get; set; }

        #endregion
        public CreateAppointmentViewModel() : base()
        {
            //Initialize Dictionary with all values from enum AppointmentType
            Types = new ObservableCollection<AppointmentType>(Enum.GetValues(typeof(AppointmentType)).OfType<AppointmentType>().ToList());

            People = new ObservableCollection<Person>();
            InitializePeople();
        }

        #region Commands

        //SearchUserCommand
        public ICommand SearchUserCommand => new Command((item) =>  SearchUser(item));

        private void SearchUser(object sender)
        {

            if (sender == null)
            {
                InitializePeople();
            }
            else
            {

                var peopleSearched = people.Where(p =>
                    p.Identification.ToUpper().Contains(sender.ToString().ToUpper())
                    || p.Name.ToUpper().Contains(sender.ToString().ToUpper())
                    || p.LastName.ToUpper().Contains(sender.ToString().ToUpper())).ToList();

                People.Clear();

                foreach (var person in peopleSearched)
                {
                    People.Add(person);
                }
            }
        }

        private void InitializePeople()
        {
            People.Clear();
            //create a list with 5 random objects values of type person            

            foreach (var person in people)
            {
                People.Add(person);
            }
        }

        #endregion
    }
}
