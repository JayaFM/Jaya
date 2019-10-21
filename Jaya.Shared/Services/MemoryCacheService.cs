﻿using Jaya.Shared.Contracts;
using Microsoft.Extensions.Caching.Memory;
using System.Composition;

namespace Jaya.Shared.Services
{
    //[Export(typeof(IService))]
    public sealed class MemoryCacheService : IMemoryCacheService // Use dedicated typed service
    {
        readonly MemoryCache _cache;

        public MemoryCacheService()
        {
            var options = new MemoryCacheOptions();

            _cache = new MemoryCache(options);
        }

        ~MemoryCacheService()
        {
            _cache.Dispose();
        }

        public long Count => _cache.Count;

        public bool TryGetValue<T>(object key, out T result)
        {
            return _cache.TryGetValue<T>(key, out result);
        }

        public void Set<T>(object key, T value)
        {
            _cache.Set<T>(key, value);
        }
    }
}
