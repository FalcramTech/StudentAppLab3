using SQLite;

using SQLite;

namespace StudentApp.Models
{
    public class SyncQueueItem
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        public string EntityType { get; set; } = "";

        public string Operation { get; set; } = "";

        public string PayloadJson { get; set; } = "";

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
