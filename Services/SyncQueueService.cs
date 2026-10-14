using StudentApp.Models;
using StudentApp.Repositories;
using System.Text.Json;
using System.Diagnostics;

namespace StudentApp.Services
{
    public class SyncQueueService
    {
        private readonly ISyncQueueRepository _queueRepository;

        public SyncQueueService(
            ISyncQueueRepository queueRepository)
        {
            _queueRepository = queueRepository;
        }

        public async Task AddChangeAsync(
            Student student,
            string operation)
        {
            var item = new SyncQueueItem
            {
                EntityType = "Student",
                Operation = operation,
                PayloadJson = JsonSerializer.Serialize(student),
                CreatedAt = DateTime.UtcNow
            };

            await _queueRepository.AddAsync(item);

            Debug.WriteLine($"Зміну {operation} додано до SyncQueue");
        }
    }
}