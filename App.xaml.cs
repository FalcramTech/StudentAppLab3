using Microsoft.Extensions.DependencyInjection;
using StudentApp.ViewModels;
using System.Diagnostics;
using StudentApp.Services;

namespace StudentApp
{
    public partial class App : Application
    {
        private readonly SyncService _syncService;
        private readonly AppShell _shell;

        public App(AppShell shell, SyncService syncService)
        {
            InitializeComponent();

            _shell = shell;
            _syncService = syncService;

            bool isDark = Preferences.Get("DarkTheme", false);

            UserAppTheme = isDark
                ? AppTheme.Dark
                : AppTheme.Light;

            _syncService.StartMonitoring();
        }

        protected override Window CreateWindow(
            IActivationState? activationState)
        {
            var window = new Window(_shell);

            window.Stopped += (sender, e) =>
            {
                Debug.WriteLine("Застосунок згорнуто");

                if (_shell.CurrentPage?.BindingContext
                    is StudentViewModel vm)
                {
                    vm.SaveState();

                    Debug.WriteLine(
                        "Дані студента збережено");
                }
            };

            window.Resumed += (sender, e) =>
            {
                Debug.WriteLine("Застосунок відновлено");

                if (_shell.CurrentPage?.BindingContext
                    is StudentViewModel vm)
                {
                    vm.RestoreState();

                    Debug.WriteLine("Дані студента відновлено");
                }
            };

            window.Destroying += (sender, e) =>
            {
                Debug.WriteLine("Застосунок завершено");
            };

            return window;
        }
    }
}