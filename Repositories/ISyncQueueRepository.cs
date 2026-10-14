using StudentApp.Models;

namespace StudentApp.Repositories
{
    public interface ISyncQueueRepository
    {
        Task<List<SyncQueueItem>> GetAllAsync();

        Task<int> AddAsync(SyncQueueItem item);

        Task<int> DeleteAsync(SyncQueueItem item);
    }
}