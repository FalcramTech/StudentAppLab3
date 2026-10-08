using StudentApp.Services;
using StudentApp.ViewModels;
using System.Diagnostics;

namespace StudentApp
{
    public partial class MainPage : ContentPage
    {
        public MainPage(StudentViewModel viewModel)
        {
            InitializeComponent();

            BindingContext = viewModel;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            Debug.WriteLine("Головна сторінка відкрита");

            if (BindingContext is StudentViewModel vm)
            {
                vm.LoadCommand.Execute(null);

                Debug.WriteLine(
                    "Дані студента завантажено");
            }
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();

            Debug.WriteLine("Головна сторінка закрита");
        }
    }
}