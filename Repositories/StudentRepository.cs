using SQLite;
using StudentApp.Models;
using StudentApp.Services;

namespace StudentApp.Repositories
{
    public class StudentRepository : IStudentRepository
    {
        private readonly SQLiteAsyncConnection _database;
        private readonly Task _initialization;
        private readonly EncryptionService _encryptionService;

        public StudentRepository(EncryptionService encryptionService)
        {
            _encryptionService = encryptionService;

            string dbPath = Path.Combine(
                FileSystem.AppDataDirectory,
                "students.db3"
            );

            _database = new SQLiteAsyncConnection(dbPath);

            _initialization = InitializeDatabaseAsync();
        }

        private async Task InitializeDatabaseAsync()
        {
            await _database.CreateTableAsync<Student>();
            await _database.CreateTableAsync<SyncQueueItem>();
        }

        private async Task InitializeAsync()
        {
            await _initialization;
        }

        public async Task<List<Student>> GetAllAsync()
        {
            await InitializeAsync();

            var students = await _database
                .Table<Student>()
                .ToListAsync();

            foreach (var student in students)
            {
                await DecryptStudentAsync(student);
            }

            return students;
        }

        public async Task<Student?> GetByIdAsync(int id)
        {
            await InitializeAsync();

            var student = await _database.FindAsync<Student>(id);

            if (student != null)
            {
                await DecryptStudentAsync(student);
            }

            return student;
        }

        public async Task<int> SaveAsync(Student student)
        {
            await InitializeAsync();

            if (!string.IsNullOrWhiteSpace(student.StudentCardNumber))
            {
                student.EncryptedStudentCardNumber =
                    await _encryptionService.EncryptAsync(
                        student.StudentCardNumber);

                System.Diagnostics.Debug.WriteLine("Номер студентського квитка зашифровано");
            }
            else
            {
                student.EncryptedStudentCardNumber = "";
            }

            if (student.Id == 0)
            {
                return await _database.InsertAsync(student);
            }

            return await _database.UpdateAsync(student);
        }

        public async Task<int> DeleteAsync(Student student)
        {
            await InitializeAsync();

            return await _database.DeleteAsync(student);
        }

        private async Task DecryptStudentAsync(Student student)
        {
            if (!string.IsNullOrEmpty(student.EncryptedStudentCardNumber))
            {
                student.StudentCardNumber =
                    await _encryptionService.DecryptAsync(
                        student.EncryptedStudentCardNumber);

                System.Diagnostics.Debug.WriteLine("Номер студентського квитка розшифровано");
            }
            else
            {
                student.StudentCardNumber = "";
            }
        }
    }
}
