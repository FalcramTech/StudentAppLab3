using StudentApp.Repositories;
using System.Diagnostics;
using System.Net.Http.Json;

namespace StudentApp.Services
{
    public class SyncService
    {
        private readonly ISyncQueueRepository _queueRepository;
        private readonly HttpClient _httpClient = new();
        private readonly SemaphoreSlim _syncLock = new(1, 1);

        public SyncService(ISyncQueueRepository queueRepository)
        {
            _queueRepository = queueRepository;
        }

        public void StartMonitoring()
        {
            Debug.WriteLine("SyncService запущено");

            Connectivity.Current.ConnectivityChanged -= OnConnectivityChanged;
            Connectivity.Current.ConnectivityChanged += OnConnectivityChanged;

            _ = SynchronizeAsync();
        }

        private async void OnConnectivityChanged(
            object? sender,
            ConnectivityChangedEventArgs e)
        {
            if (e.NetworkAccess == NetworkAccess.Internet)
            {
                await SynchronizeAsync();
            }
        }

        public async Task SynchronizeAsync()
        {
            Debug.WriteLine("Перевірка інтернет-з'єднання");
            Debug.WriteLine($"Стан мережі: {Connectivity.Current.NetworkAccess}");

            if (Connectivity.Current.NetworkAccess != NetworkAccess.Internet)
            {
                Debug.WriteLine("Інтернет відсутній. Синхронізацію відкладено.");
                return;
            }

            await _syncLock.WaitAsync();

            try
            {
                var queue = await _queueRepository.GetAllAsync();

                Debug.WriteLine($"Записів у черзі: {queue.Count}");

#if ANDROID
                string apiUrl = "http://10.0.2.2:5080/api/sync";
#else
                string apiUrl = "http://127.0.0.1:5080/api/sync";
#endif

                foreach (var item in queue)
                {
                    var request = new
                    {
                        item.EntityType,
                        item.Operation,
                        item.PayloadJson
                    };

                    using var response =
                        await _httpClient.PostAsJsonAsync(
                            apiUrl, request);

                    if (response.IsSuccessStatusCode)
                    {
                        await _queueRepository.DeleteAsync(item);

                        Debug.WriteLine($"Синхронізовано: {item.Operation} {item.EntityType}");
                    }
                    else
                    {
                        Debug.WriteLine($"Помилка API: {response.StatusCode}");

                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Помилка синхронізації: {ex.Message}");
            }
            finally
            {
                _syncLock.Release();
            }
        }
    }
}
