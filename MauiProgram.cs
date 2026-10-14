using Microsoft.Extensions.Logging;
using StudentApp.Services;
using StudentApp.ViewModels;
using StudentApp.Repositories;
using StudentApp.Views;

namespace StudentApp
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();

            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

            builder.Logging.AddDebug();

            builder.Services.AddSingleton<IAppStateService, AppStateService>();

            builder.Services.AddTransient<StudentViewModel>();

            builder.Services.AddSingleton<MainPage>();

            builder.Services.AddSingleton<AppShell>();

            builder.Services.AddSingleton<IStudentRepository, StudentRepository>();
            
            builder.Services.AddTransient<StudentDetailViewModel>();

            builder.Services.AddTransient<StudentDetailPage>();

            builder.Services.AddSingleton<IStudentRepository, StudentRepository>();

            builder.Services.AddSingleton<ISyncQueueRepository, SyncQueueRepository>();

            builder.Services.AddSingleton<SyncQueueService>();

            builder.Services.AddSingleton<SyncService>();

            builder.Services.AddSingleton<EncryptionService>();


            return builder.Build();
        }
    }
}