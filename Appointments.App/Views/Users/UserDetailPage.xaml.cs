using Appointments.App.Models.DataModels;
using Appointments.App.ViewModels.AppointmentType;
using Appointments.App.ViewModels.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Appointments.App.Views.Users
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class UserDetailPage : ContentPage
    {
        public int UserId { get; set; }
        public UserDetailPage(int userId)
        {
            InitializeComponent();
            UserId = userId;
        }

        protected override void OnAppearing()
        {
            (BindingContext as UserViewModel)?.LoadUser(UserId);
        }
    }
}