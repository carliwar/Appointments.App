using Acr.UserDialogs;
using Appointments.App.Services;
using Microsoft.Extensions.DependencyInjection;
using System;
using Xamarin.Forms;

namespace Appointments.App
{
    public partial class App : Application
    {
        protected static IServiceProvider ServiceProvider { get; set; }        
        public App()
        {
            InitializeComponent();

            SetupServices();

            MainPage = new AppShell();
        }

        #region Overrides
        protected override void OnStart()
        {
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
        #endregion

        #region Private Methods
        private void SetupServices()
        {
            
        } 
        #endregion
    }
}
