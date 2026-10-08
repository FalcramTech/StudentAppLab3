using Android.App;
using Android.Content.PM;
using Android.OS;

namespace StudentApp
{
    [Activity(
        Theme = "@style/Maui.SplashTheme",
        MainLauncher = true,
        LaunchMode = LaunchMode.SingleTop,
        ConfigurationChanges =
            ConfigChanges.ScreenSize |
            ConfigChanges.Orientation |
            ConfigChanges.UiMode |
            ConfigChanges.ScreenLayout |
            ConfigChanges.SmallestScreenSize |
            ConfigChanges.Density)]

    [IntentFilter(
        new[] { Android.Content.Intent.ActionView },
        Categories = new[]
        {
            Android.Content.Intent.CategoryDefault,
            Android.Content.Intent.CategoryBrowsable
        },
        DataScheme = "studentapp",
        DataHost = "student")]

    public class MainActivity : MauiAppCompatActivity
    {
        public static string? PendingStudentId { get; private set; }

        protected override void OnCreate(Bundle? savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            HandleDeepLink(Intent);
        }

        protected override void OnNewIntent(
            Android.Content.Intent? intent)
        {
            base.OnNewIntent(intent);

            HandleDeepLink(intent);
        }

        private void HandleDeepLink(
            Android.Content.Intent? intent)
        {
            if (intent?.Action !=
                Android.Content.Intent.ActionView)
                return;

            var id = intent.Data?.GetQueryParameter("id");

            if (string.IsNullOrEmpty(id))
                return;

            PendingStudentId = id;

            MainThread.BeginInvokeOnMainThread(async () =>
            {
                if (Shell.Current is AppShell shell)
                {
                    var studentId = ConsumePendingStudentId();

                    if (studentId != null)
                    {
                        await shell.OpenStudentFromLinkAsync(studentId);
                    }
                }
            });
        }

        public static string? ConsumePendingStudentId()
        {
            var id = PendingStudentId;
            PendingStudentId = null;

            return id;
        }
    }
}