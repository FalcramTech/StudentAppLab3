
using StudentApp.Models;
using StudentApp.Views;

namespace StudentApp
{
    public partial class AppShell : Shell
    {
        public AppShell(MainPage mainPage)
        {
            InitializeComponent();

            var mainContent = Items
                .OfType<TabBar>()
                .SelectMany(tabBar => tabBar.Items)
                .SelectMany(tab => tab.Items)
                .FirstOrDefault(content =>
                    content.Route == "MainPage");

            if (mainContent != null)
            {
                mainContent.ContentTemplate =
                    new DataTemplate(() => mainPage);
            }

            Routing.RegisterRoute(
                "studentdetail",
                typeof(StudentDetailPage)
            );
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

#if ANDROID
            var id = MainActivity.ConsumePendingStudentId();

            if (id != null)
            {
                await OpenStudentFromLinkAsync(id);
            }
#endif
        }

        public async Task OpenStudentFromLinkAsync(string id)
        {
            if (id != "1")
            {
                await DisplayAlertAsync("Помилка", "Студента з таким ID не знайдено", "OK");

                return;
            }

            var student = new Student
            {
                Id = 1,
                FullName = "Бабенко Іван",
                Group = "4-14",
                AverageScore = 5.0
            };

            var parameters = new Dictionary<string, object>
            {
                { "SelectedStudent", student }
            };

            await GoToAsync("studentdetail", parameters);
        }
    }
}
