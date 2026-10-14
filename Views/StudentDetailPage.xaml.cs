
using StudentApp.ViewModels;

namespace StudentApp.Views
{
    public partial class StudentDetailPage : ContentPage
    {
        public StudentDetailPage(
            StudentDetailViewModel viewModel)
        {
            InitializeComponent();

            BindingContext = viewModel;
        }
    }
}
