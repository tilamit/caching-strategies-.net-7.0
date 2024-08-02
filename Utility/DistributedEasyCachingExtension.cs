using EasyCaching.Core;
using SampleAppWithCaching.Models;

namespace SampleAppWithCaching.Utility
{
    public static class DistributedEasyCachingExtension
    {
        public static async Task<T> GetOrSetAsync<T>(this IEasyCachingProviderFactory _factory, string cacheProvider, string key, Func<Task<T>> task, double duration)
        {
            var provider = _factory.GetCachingProvider(cacheProvider);

            if (await provider.ExistsAsync(key))
            {
                var cachedVal = await provider.GetAsync<T>(key);
                return cachedVal.Value;
            }

            var val = await task();

            await provider.SetAsync(key, val, TimeSpan.FromSeconds(duration));

            return await Task.Run(() => val);
        }

        public static async void RemoveCache(this IEasyCachingProviderFactory _factory, string cacheProvider, string key)
        {
            var provider = _factory.GetCachingProvider(cacheProvider);
            await Task.Run(() => provider.Remove(key));
        }
    }
}
