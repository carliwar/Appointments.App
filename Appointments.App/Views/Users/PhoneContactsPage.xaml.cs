using Appointments.App.ViewModels;
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
	public partial class PhoneContactsPage : ContentPage
	{
		public PhoneContactsPage ()
		{
			InitializeComponent();
		}

        protected override void OnAppearing()
        {
            (BindingContext as PhoneContactsViewModel)?.InitializeUsers();
        }
    }
}