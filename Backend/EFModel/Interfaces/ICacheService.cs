namespace EFModel.Interfaces
{
    public interface ICacheService
    {
        Task<T> GetOrCreatePermanentAsync<T>(string key, Func<Task<T>> factory);
        T GetOrCreatePermanent<T>(string key, Func<T> factory);
        void SetPermanent<T>(string key, T value);
        bool TryGet<T>(string key, out T value);
        Task RemoveAsync(string key);
        void Remove(string key);
    }
}