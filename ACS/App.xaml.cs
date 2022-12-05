using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

using ACS.Contracts.Services;
using ACS.Contracts.Views;
using ACS.Core.Contracts.Services;
using ACS.Core.Data;
using ACS.Core.Models;
using ACS.Core.Models.Events;
using ACS.Core.Services;
using ACS.Core.Services.Repositories;
using ACS.Core.Services.Repositories.Events;
using ACS.Models;
using ACS.Services;
using ACS.Views;
using ACS.Views.EditPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace ACS
{
    // For more information about application lifecycle events see https://docs.microsoft.com/dotnet/framework/wpf/app-development/application-management-overview

    // WPF UI elements use language en-US by default.
    // If you need to support other cultures make sure you add converters and review dates and numbers in your UI to ensure everything adapts correctly.
    // Tracking issue for improving this is https://github.com/dotnet/wpf/issues/1946
    public partial class App : Application
    {
        private IHost _host;
        public IConfiguration Configuration { get; private set; }

        public T GetService<T>()
            where T : class
            => _host.Services.GetService(typeof(T)) as T;

        public App()
        {
        }

        private async void OnStartup(object sender, StartupEventArgs e)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

            Configuration = builder.Build();

            var appLocation = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);

            // For more information about .NET generic host see  https://docs.microsoft.com/aspnet/core/fundamentals/host/generic-host?view=aspnetcore-3.0
            _host = Host.CreateDefaultBuilder(e.Args)
                    .ConfigureAppConfiguration(c =>
                    {
                        c.SetBasePath(appLocation);
                    })
                    .ConfigureServices(ConfigureServices)
                    .Build();


            await _host.StartAsync();
        }

        private void ConfigureServices(HostBuilderContext context, IServiceCollection services)
        {
            services.AddDbContext<AccessControlDbContext>(options =>
                options.UseNpgsql(
                    Configuration.GetConnectionString("AccessControlConnectionString")), ServiceLifetime.Transient);

            services.AddSingleton(Configuration);

            // App Host
            services.AddHostedService<ApplicationHostService>();

            // Core Services
            services.AddSingleton<IFileService, FileService>();

            // Services
            //Repos
            services.AddTransient<GenericAPIPoster<User>>();
            services.AddTransient<GenericAPIPoster<KeyCard>>();
            services.AddTransient<GenericAPIPoster<AccessPoint>>();
            services.AddTransient<GenericAPIPoster<Camera>>();
            services.AddTransient<GenericAPIPoster<ParkingLot>>();
            services.AddTransient<GenericAPIPoster<AccessEvent>>();
            services.AddTransient<GenericAPIPoster<FaceRecognizedEvent>>();
            services.AddTransient<GenericAPIPoster<ParkingLotStateChangedEvent>>();
            services.AddTransient(typeof(GenericAPIPoster<>));
            //Others
            services.AddSingleton<IApplicationInfoService, ApplicationInfoService>();
            services.AddSingleton<ISystemService, SystemService>();
            services.AddSingleton<IPersistAndRestoreService, PersistAndRestoreService>();
            services.AddSingleton<IThemeSelectorService, ThemeSelectorService>();
            services.AddSingleton<INavigationService, NavigationService>();

            // Views
            //Edit Pages (probably should be a template but who cares)
            services.AddTransient<AccessPointEditPage>();
            services.AddTransient<CameraEditPage>();
            services.AddTransient<KeyCardEditPage>();
            services.AddTransient<ParkingLotEditPage>();
            services.AddTransient<UserEditPage>();
            services.AddTransient<CarEditPage>();
            services.AddTransient<IShellWindow, ShellWindow>();

            //Table views
            services.AddTransient<MainPage>();
            services.AddTransient<UsersPage>();
            services.AddTransient<KeyCardsPage>();
            services.AddTransient<AccessPointsPage>();
            services.AddTransient<CamerasPage>();
            services.AddTransient<ParkingLotPage>();
            services.AddTransient<EventsPage>();
            services.AddTransient<SettingsPage>();

            // Configuration
            services.Configure<AppConfig>(context.Configuration.GetSection(nameof(AppConfig)));
        }

        private async void OnExit(object sender, ExitEventArgs e)
        {
            await _host.StopAsync();
            _host.Dispose();
            _host = null;
        }

        private void OnDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            // Please log and handle the exception as appropriate to your scenario
            // For more info see https://docs.microsoft.com/dotnet/api/system.windows.application.dispatcherunhandledexception?view=netcore-3.0
        }
    }
}
