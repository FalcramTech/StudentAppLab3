using SQLite;
using StudentApp.Models;

namespace StudentApp.Repositories
{
    public class SyncQueueRepository : ISyncQueueRepository
    {
        private readonly SQLiteAsyncConnection _database;
        private readonly Task _initialization;

        public SyncQueueRepository()
        {
            string dbPath = Path.Combine(
                FileSystem.AppDataDirectory,
                "students.db3"
            );

            _database = new SQLiteAsyncConnection(dbPath);

            _initialization =
                _database.CreateTableAsync<SyncQueueItem>();
        }

        public async Task<List<SyncQueueItem>> GetAllAsync()
        {
            await _initialization;

            return await _database
                .Table<SyncQueueItem>()
                .OrderBy(item => item.Id)
                .ToListAsync();
        }

        public async Task<int> AddAsync(SyncQueueItem item)
        {
            await _initialization;

            return await _database.InsertAsync(item);
        }

        public async Task<int> DeleteAsync(SyncQueueItem item)
        {
            await _initialization;

            return await _database.DeleteAsync(item);
        }
    }
}