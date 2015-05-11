// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading.Tasks;
using Microsoft.Framework.Caching.Memory;
using Microsoft.Framework.Internal;

namespace Microsoft.Framework.Caching.Distributed
{
    public class LocalCache : IDistributedCache
    {
        private static readonly Task CompletedTask = Task.FromResult<object>(null);

        private readonly IMemoryCache _memCache;

        public LocalCache([NotNull] IMemoryCache memoryCache)
        {
            _memCache = memoryCache;
        }

        public byte[] Get([NotNull] string key)
        {
            return (byte[])_memCache.Get(key);
        }

        public Task<byte[]> GetAsync([NotNull] string key)
        {
            return Task.FromResult(Get(key));
        }

        public bool TryGetValue([NotNull] string key, out byte[] value)
        {
            return _memCache.TryGetValue(key, out value);
        }

        public Task<bool> TryGetValueAsync([NotNull] string key, out byte[] value)
        {
            return Task.FromResult(TryGetValue(key, out value));
        }

        public void Set([NotNull] string key, byte[] value, CacheEntryOptions options)
        {
            _memCache.Set(key, value, options);
        }

        public Task SetAsync([NotNull] string key, byte[] value, CacheEntryOptions options)
        {
            Set(key, value, options);
            return CompletedTask;
        }

        public void Refresh([NotNull] string key)
        {
            object value;
            _memCache.TryGetValue(key, out value);
        }

        public void Remove([NotNull] string key)
        {
            _memCache.Remove(key);
        }
    }
}