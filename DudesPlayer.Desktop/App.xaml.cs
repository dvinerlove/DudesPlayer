using DudesPlayer.Desktop.Services;
using DudesPlayer.Desktop.Views;
using DudesPlayer.Desktop.Views.Controls;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using Wpf.Ui.Mvvm.Contracts;
using Wpf.Ui.Mvvm.Services;

namespace DudesPlayer.Desktop
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App
    {
        // The.NET Generic Host provides dependency injection, configuration, logging, and other services.
        // https://docs.microsoft.com/dotnet/core/extensions/generic-host
        // https://docs.microsoft.com/dotnet/core/extensions/dependency-injection
        // https://docs.microsoft.com/dotnet/core/extensions/configuration
        // https://docs.microsoft.com/dotnet/core/extensions/logging
        private static readonly IHost _host = Host
            .CreateDefaultBuilder()
            .ConfigureAppConfiguration(c => { c.SetBasePath(Path.GetDirectoryName(Assembly.GetEntryAssembly()!.Location)); })
            .ConfigureServices((context, services) =>
            {
                // App Host
                services.AddHostedService<ApplicationHostService>();

                // Page resolver service
                services.AddSingleton<IPageService, PageService>();
                services.AddScoped<INavService, NavService>();
                services.AddScoped<IModalWindowService, ModalWindowService>();

                // Theme manipulation
                services.AddSingleton<IThemeService, ThemeService>();

                // TaskBar manipulation
                services.AddSingleton<ITaskBarService, TaskBarService>();

                // Service containing navigation, same as INavigationWindow... but without window
                //services.AddSingleton<INavigationService, NavigationService>();

                // Main window container with navigation
                services.AddScoped<IWindow, Views.Container>();
                //services.AddScoped<ViewModels.ContainerViewModel>();

                //// Views and ViewModels
                services.AddScoped<Views.Pages.RoomsExplorerPage.NewRoomDialog>();
                services.AddScoped<Views.Pages.RoomsExplorerPage.Dialogs.ConnectToRoomDialog>();
                services.AddScoped<Views.Pages.FriendsPage.FriendsPage>();
                services.AddScoped<Views.Pages.RoomsExplorerPage.RoomsExplorerPage>();
                services.AddScoped<Views.Pages.LoginPage>();
                services.AddScoped<Views.Pages.PlayerPage.PlayerPage>();

                //services.AddScoped<ViewModels.DashboardViewModel>();
                //services.AddScoped<Views.Pages.DataPage>();
                //services.AddScoped<ViewModels.DataViewModel>();
                //services.AddScoped<Views.Pages.SettingsPage>();
                //services.AddScoped<ViewModels.SettingsViewModel>();

                // Configuration
                //services.Configure<AppConfig>(context.Configuration.GetSection(nameof(AppConfig)));
            }).Build();

        /// <summary>
        /// Gets registered service.
        /// </summary>
        /// <typeparam name="T">Type of the service to get.</typeparam>
        /// <returns>Instance of the service or <see langword="null"/>.</returns>
        public static T GetService<T>()
            where T : class
        {
            return _host.Services.GetService(typeof(T)) as T;
        }

        /// <summary>
        /// Occurs when the application is loading.
        /// </summary>
        private async void OnStartup(object sender, StartupEventArgs e)
        {
            await _host.StartAsync();
        }

        /// <summary>
        /// Occurs when the application is closing.
        /// </summary>
        private async void OnExit(object sender, ExitEventArgs e)
        {
            await _host.StopAsync();

            _host.Dispose();
        }

        /// <summary>
        /// Occurs when an exception is thrown by an application but not handled.
        /// </summary>
        private void OnDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            // For more info see https://docs.microsoft.com/en-us/dotnet/api/system.windows.application.dispatcherunhandledexception?view=windowsdesktop-6.0
        }
    }
}