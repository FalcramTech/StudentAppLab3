using StudentApp.Models;
using StudentApp.Repositories;
using StudentApp.Services;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace StudentApp.ViewModels
{
    public class StudentViewModel : INotifyPropertyChanged, IQueryAttributable
    {
        private Student _student = new Student();

        private readonly IAppStateService _appStateService;
        private readonly IStudentRepository _repository;

        private readonly SyncQueueService _syncQueueService;


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

        public string StudentCardNumber
        {
            get => _student.StudentCardNumber;
            set
            {
                if (_student.StudentCardNumber != value)
                {
                    _student.StudentCardNumber = value;
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

        public ICommand UpdateStudentCommand { get; }

        public ICommand DeleteStudentCommand { get; }

        public StudentViewModel(IAppStateService appStateService, IStudentRepository repository, SyncQueueService syncQueueService)
        {
            _appStateService = appStateService;
            _repository = repository;
            _syncQueueService = syncQueueService;

            AddStudentCommand = new Command(
                async () => await AddStudentAsync(),
                CanAddStudent
            );

            OpenDetailsCommand = new Command<Student>(
                async student => await OpenDetailsAsync(student)
            );

            LoadCommand = new Command(
                async () => await LoadStudentsAsync()
            );

            UpdateStudentCommand = new Command<Student>(
                async student => await UpdateStudentAsync(student)
            );

            DeleteStudentCommand = new Command<Student>(
                async student => await DeleteStudentAsync(student)
            );
        }

        private async Task LoadStudentsAsync()
        {
            try
            {
                var students = await _repository.GetAllAsync();

                Students.Clear();

                foreach (var student in students)
                {
                    Students.Add(student);
                }

                Debug.WriteLine("Дані студентів завантажено з SQLite");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Помилка завантаження: {ex}");
            }
        }

        private bool CanAddStudent()
        {
            return !string.IsNullOrWhiteSpace(FullName);
        }


        private async Task AddStudentAsync()
        {
            if (!CanAddStudent())
                return;

            var newStudent = new Student
            {
                FullName = FullName,
                Group = Group,
                AverageScore = AverageScore,
                StudentCardNumber = StudentCardNumber
            };

            try
            {
                await _repository.SaveAsync(newStudent);

                await _syncQueueService.AddChangeAsync(
                    newStudent, "Insert");

                await LoadStudentsAsync();

                FullName = string.Empty;
                Group = string.Empty;
                AverageScore = 0;
                StudentCardNumber = string.Empty;

                Debug.WriteLine("Студента збережено в SQLite. Номер квитка зашифровано.");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Помилка додавання: {ex}");
            }
        }


        private async Task UpdateStudentAsync(Student student)
        {
            if (student == null)
                return;

            try
            {
                await _repository.SaveAsync(student);

                await LoadStudentsAsync();

                Debug.WriteLine("Дані студента оновлено в SQLite");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Помилка оновлення: {ex}");
            }
        }

        private async Task DeleteStudentAsync(Student student)
        {
            if (student == null)
                return;

            try
            {
                await _repository.DeleteAsync(student);

                Students.Remove(student);

                Debug.WriteLine("Студента видалено з SQLite");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Помилка видалення: {ex}");
            }
        }

        private async Task OpenDetailsAsync(Student student)
        {
            if (student == null)
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
            if (query.TryGetValue(
                "UpdatedStudent", out var value)
                && value is Student updatedStudent)
            {
                var index = Students
                    .ToList()
                    .FindIndex(s => s.Id == updatedStudent.Id);

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
