using StudentApp.Models;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace StudentApp.ViewModels
{
    public class StudentDetailViewModel : IQueryAttributable, INotifyPropertyChanged
    {
        private Student? _selectedStudent;

        private string _fullName = string.Empty;
        private string _group = string.Empty;
        private double _averageScore;

        public string FullName
        {
            get => _fullName;
            set
            {
                _fullName = value;
                OnPropertyChanged();
            }
        }

        public string Group
        {
            get => _group;
            set
            {
                _group = value;
                OnPropertyChanged();
            }
        }

        public double AverageScore
        {
            get => _averageScore;
            set
            {
                _averageScore = value;
                OnPropertyChanged();
            }
        }

        public ICommand GoBackCommand { get; }
        public ICommand SaveCommand { get; }

        public StudentDetailViewModel()
        {
            GoBackCommand = new Command(async () =>
                await Shell.Current.GoToAsync(".."));

            SaveCommand = new Command(async () =>
                await SaveStudent());
        }

        public void ApplyQueryAttributes(IDictionary<string, object> query)
        {
            if (query.TryGetValue("SelectedStudent", out var value)
                && value is Student student)
            {
                _selectedStudent = student;

                FullName = student.FullName;
                Group = student.Group;
                AverageScore = student.AverageScore;
            }
        }

        private async Task SaveStudent()
        {
            if (_selectedStudent == null)
                return;

            _selectedStudent.AverageScore = AverageScore;

            var parameters = new Dictionary<string, object>
            {
                { "UpdatedStudent", _selectedStudent }
            };

            await Shell.Current.GoToAsync("..", parameters);
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}