using System;
using System.IO;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Windows;
using Microsoft.Extensions.Configuration;
using UPS.Assessment.Infrastructure.Helpers;
using UPS.Assessment.Infrastructure.Interfaces.Services;
using UPS.Assessment.Models;
using UPS.Assessment.Services;
using UPS.Assessment.Windows;

namespace UPS.Assessment
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public IServiceProvider ServiceProvider { get; private set; }

        public IConfiguration Configuration { get; private set; }
        public static IHost AppHost { get; private set; }

      
      
        protected override async void OnStartup(StartupEventArgs e)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

            Configuration = builder.Build();

            Console.WriteLine();

            var serviceCollection = new ServiceCollection();

            ConfigureServices(serviceCollection);

            ServiceProvider = serviceCollection.BuildServiceProvider();

            var mainWindow = ServiceProvider.GetRequiredService<MainWindow>();
            mainWindow.Show();
        }

        private void ConfigureServices(IServiceCollection services)
        {
            services.AddHttpClient();
            services.AddTransient<IHttpClientHelper, HttpClientHelper>();

            var goRestCred = Configuration.GetSection("GoRest");
            services.Configure<GoRestCred>(goRestCred);
            services.AddTransient<IUserService, UserService>();
            services.AddFormFactory<AddUserWindow>();
            services.AddTransient(typeof(MainWindow));
        }


    }
}
