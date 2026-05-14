using Microsoft.Extensions.Caching.Memory;
using EFModel.Interfaces;

namespace WebApiDonCho.Services
{
    public class CacheService : ICacheService
    {
        private readonly IMemoryCache _cache;
        private static readonly SemaphoreSlim _lock = new(1, 1);

        public CacheService(IMemoryCache cache) => _cache = cache ?? throw new ArgumentNullException(nameof(cache));

        public void Remove(string key) => _cache.Remove(key);

        public async Task<T> GetOrCreatePermanentAsync<T>(string key, Func<Task<T>> factory)
        {
            if (_cache.TryGetValue(key, out var cached) && cached is T existing)
            {
                return existing;
            }

            await _lock.WaitAsync();
            try
            {
                if (_cache.TryGetValue(key, out cached) && cached is T existingAfterLock)
                {
                    return existingAfterLock;
                }

                var value = await factory();

                var options = new MemoryCacheEntryOptions
                {
                    Priority = CacheItemPriority.NeverRemove
                };

                _cache.Set(key, value, options);

                return value;
            }
            finally
            {
                _lock.Release();
            }
        }

        public void SetPermanent<T>(string key, T value)
        => _cache.Set(key, value, new MemoryCacheEntryOptions
        {
            Priority = CacheItemPriority.NeverRemove
        });

        public bool TryGet<T>(string key, out T value)
        {
            if (_cache.TryGetValue(key, out var cached) && cached is T t)
            {
                value = t;
                return true;
            }

            value = default!;
            return false;
        }

        public Task RemoveAsync(string key)
        {
            _cache.Remove(key);
            return Task.CompletedTask;
        }
    }
}
