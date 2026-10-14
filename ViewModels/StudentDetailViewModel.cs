using StudentApp.Models;
using StudentApp.Repositories;
using StudentApp.Services;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace StudentApp.ViewModels
{
    public class StudentDetailViewModel : IQueryAttributable, INotifyPropertyChanged
    {
        private readonly IStudentRepository _repository;
        private readonly SyncQueueService _syncQueueService;

        private Student? _selectedStudent;

        private string _fullName = string.Empty;
        private string _group = string.Empty;
        private double _averageScore;
        private string _studentCardNumber = string.Empty;

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

        public string StudentCardNumber
        {
            get => _studentCardNumber;
            set
            {
                _studentCardNumber = value;
                OnPropertyChanged();
            }
        }

        public ICommand GoBackCommand { get; }
        public ICommand SaveCommand { get; }
        public ICommand DeleteCommand { get; }

        public StudentDetailViewModel(
            IStudentRepository repository,
            SyncQueueService syncQueueService)
        {
            _repository = repository;
            _syncQueueService = syncQueueService;

            GoBackCommand = new Command(async () =>
                await Shell.Current.GoToAsync(".."));

            SaveCommand = new Command(async () =>
                await SaveStudentAsync());

            DeleteCommand = new Command(async () =>
                await DeleteStudentAsync());
        }

        public void ApplyQueryAttributes(
            IDictionary<string, object> query)
        {
            if (query.TryGetValue("SelectedStudent", out var value)
                && value is Student student)
            {
                _selectedStudent = student;

                FullName = student.FullName;
                Group = student.Group;
                AverageScore = student.AverageScore;

                StudentCardNumber = student.StudentCardNumber;
            }
        }

        private async Task SaveStudentAsync()
        {
            if (_selectedStudent == null)
                return;

            try
            {
                _selectedStudent.FullName = FullName;
                _selectedStudent.Group = Group;
                _selectedStudent.AverageScore = AverageScore;

                _selectedStudent.StudentCardNumber =
                    StudentCardNumber;

                await _repository.SaveAsync(_selectedStudent);

                await _syncQueueService.AddChangeAsync(
                    _selectedStudent, "Update");

                Debug.WriteLine(
                    "Дані студента оновлено. Номер квитка зашифровано.");

                await Shell.Current.GoToAsync("..");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Помилка збереження: {ex}");
            }
        }

        private async Task DeleteStudentAsync()
        {
            if (_selectedStudent == null)
                return;

            try
            {
                await _repository.DeleteAsync(_selectedStudent);

                await _syncQueueService.AddChangeAsync(
                    _selectedStudent, "Delete");

                Debug.WriteLine("Студента видалено з SQLite");

                await Shell.Current.GoToAsync("..");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Помилка видалення: {ex}");
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
    }
}
