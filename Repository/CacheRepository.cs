﻿using Microsoft.Extensions.Caching.Memory;
using SampleAppWithCaching.Interface;
using SampleAppWithCaching.Models;

namespace SampleAppWithCaching.Repository
{
    public class CacheProvider : ICacheProvider
    {
        private readonly IMemoryCache _cache;

        public CacheProvider(IMemoryCache cache)
        {
            _cache = cache;
        }

        public T GetFromCache<T>(string key) where T : class
        {
            _cache.TryGetValue(key, out T cachedResponse);
            return cachedResponse as T;
        }

        public void SetCache<T>(string key, T value, MemoryCacheEntryOptions options) where T : class
        {
            _cache.Set(key, value, options);
        }

        public void ClearCache(string key)
        {
            _cache.Remove(key);
        }
    }
}
