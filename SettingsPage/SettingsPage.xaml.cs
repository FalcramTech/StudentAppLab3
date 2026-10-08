namespace StudentApp.Views
{
    public partial class SettingsPage : ContentPage
    {
        public SettingsPage()
        {
            InitializeComponent();

            bool isDark = Preferences.Get("DarkTheme", false);

            ThemeSwitch.Toggled -= ThemeSwitch_Toggled;
            ThemeSwitch.IsToggled = isDark;
            ThemeSwitch.Toggled += ThemeSwitch_Toggled;
        }

        private void ThemeSwitch_Toggled(
            object? sender,
            ToggledEventArgs e)
        {
            bool isDark = e.Value;

            if (Application.Current != null)
            {
                Application.Current.UserAppTheme =
                    isDark ? AppTheme.Dark : AppTheme.Light;
            }

            Preferences.Set("DarkTheme", isDark);
        }
    }
}