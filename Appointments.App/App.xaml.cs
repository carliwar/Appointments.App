using Appointments.App.Services;
using Appointments.App.Services.VMServices;
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
            var serviceCollection =  new ServiceCollection();

            serviceCollection.AddScoped<IHttpHelperService, HttpHelperService>();
            serviceCollection.AddScoped<IDataService, DataService>();
            serviceCollection.AddScoped<IUserService, UserService>();

            serviceCollection.AddHttpClient("CONTIFICO", client =>
            {
                client.BaseAddress = new Uri("https://ms-contifico-integration.us-east-1.elasticbeanstalk.com/");                
            });

            ServiceProvider = serviceCollection.BuildServiceProvider();

        }
        #endregion
    }
}
