using StudentApp.Models;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using StudentApp.Services;

namespace StudentApp.ViewModels
{
    public class StudentViewModel : INotifyPropertyChanged, IQueryAttributable
    {
        private Student _student = new Student();

        private readonly IAppStateService _appStateService;

        public string FullName
        {
            get => _student.FullName;
            set
            {
                if (_student.FullName != value)
                {
                    _student.FullName = value;
                    OnPropertyChanged();

                    ((Command)AddStudentCommand).ChangeCanExecute();
                }
            }
        }

        public string Group
        {
            get => _student.Group;
            set
            {
                if (_student.Group != value)
                {
                    _student.Group = value;
                    OnPropertyChanged();
                }
            }
        }

        public double AverageScore
        {
            get => _student.AverageScore;
            set
            {
                if (_student.AverageScore != value)
                {
                    _student.AverageScore = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(IsHighScore));
                }
            }
        }

        public bool IsHighScore => AverageScore >= 4.0;

        public ObservableCollection<Student> Students { get; }
            = new ObservableCollection<Student>();

        public ICommand AddStudentCommand { get; }

        public ICommand OpenDetailsCommand { get; }

        public ICommand LoadCommand { get; }

        public StudentViewModel(IAppStateService appStateService)
        {
            _appStateService = appStateService;

            AddStudentCommand = new Command(
                AddStudent,
                CanAddStudent
            );

            OpenDetailsCommand = new Command<Student>(
                async student => await OpenDetailsAsync(student)
            );

            LoadCommand = new Command(RestoreState);
        }

        private bool CanAddStudent()
        {
            return !string.IsNullOrWhiteSpace(FullName);
        }

        private void AddStudent()
        {
            var newStudent = new Student
            {
                FullName = FullName,
                Group = Group,
                AverageScore = AverageScore
            };

            Students.Add(newStudent);

            FullName = string.Empty;
            Group = string.Empty;
            AverageScore = 0;
        }

        private async Task OpenDetailsAsync(Student student)
        {
            if (student is null)
                return;

            var parameters = new Dictionary<string, object>
            {
                { "SelectedStudent", student }
            };

            await Shell.Current.GoToAsync(
                "studentdetail",
                parameters
            );
        }

        public void ApplyQueryAttributes(
            IDictionary<string, object> query)
        {
            if (query.TryGetValue("UpdatedStudent", out var value)
                && value is Student updatedStudent)
            {
                var index = Students.IndexOf(updatedStudent);

                if (index >= 0)
                {
                    Students[index] = updatedStudent;
                }
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged(
            [CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(
                this,
                new PropertyChangedEventArgs(propertyName)
            );
        }

        public void SaveState()
        {
            _appStateService.FullName = FullName;
            _appStateService.Group = Group;
            _appStateService.AverageScore = AverageScore;

            Preferences.Set("StudentName", FullName);
            Preferences.Set("StudentGroup", Group);
            Preferences.Set("StudentScore", AverageScore);
        }

        public void RestoreState()
        {
            FullName = Preferences.Get("StudentName", "");
            Group = Preferences.Get("StudentGroup", "");
            AverageScore = Preferences.Get("StudentScore", 0.0);

            _appStateService.FullName = FullName;
            _appStateService.Group = Group;
            _appStateService.AverageScore = AverageScore;
        }
    }
}