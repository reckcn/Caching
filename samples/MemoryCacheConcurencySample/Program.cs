// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Threading.Tasks;
using Microsoft.Framework.Caching;
using Microsoft.Framework.Caching.Memory;

namespace MemoryCacheSample
{
    public class Program
    {
        private const string Key = "MyKey";
        private static readonly Random Random = new Random();
        private CacheEntryOptions _cacheEntryOptions;

        public void Main()
        {
            _cacheEntryOptions = GetCacheEntryOptions();

            IMemoryCache cache = new MemoryCache(new MemoryCacheOptions());

            SetKey(cache, "0");

            PriodicallyReadKey(cache, TimeSpan.FromSeconds(1));

            PeriodciallyRemoveKey(cache, TimeSpan.FromSeconds(11));

            PeriodciallySetKey(cache, TimeSpan.FromSeconds(13));

            Console.ReadLine();
            Console.WriteLine("Shutting down");
        }

        private void SetKey(IMemoryCache cache, string value)
        {
            Console.WriteLine("Setting: " + value);
            cache.Set(Key, value, _cacheEntryOptions);
        }

        private CacheEntryOptions GetCacheEntryOptions()
        {
            return new CacheEntryOptions()
                .SetAbsoluteExpiration(TimeSpan.FromSeconds(7))
                .SetSlidingExpiration(TimeSpan.FromSeconds(3))
                .RegisterPostEvictionCallback(AfterEvicted, state: null);
        }

        private void AfterEvicted(string key, object value, EvictionReason reason, object state)
        {
            Console.WriteLine("Evicted. Value: " + value + ", Reason: " + reason);
        }

        private void PeriodciallySetKey(IMemoryCache cache, TimeSpan interval)
        {
            Task.Run(async () =>
            {
                while (true)
                {
                    await Task.Delay(interval);

                    SetKey(cache, "A");
                }
            });
        }

        private void PriodicallyReadKey(IMemoryCache cache, TimeSpan interval)
        {
            Task.Run(async () =>
            {
                while (true)
                {
                    await Task.Delay(interval);

                    if (Random.Next(3) == 0) // 1/3 chance
                    {
                        // Allow values to expire due to sliding refresh.
                        Console.WriteLine("Read skipped, random choice.");
                    }
                    else
                    {
                        Console.Write("Reading...");
                        var result = cache.Set(Key, "B", _cacheEntryOptions);
                        Console.WriteLine("Read: " + (result ?? "(null)"));
                    }
                }
            });
        }

        private void PeriodciallyRemoveKey(IMemoryCache cache, TimeSpan interval)
        {
            Task.Run(async () =>
            {
                while (true)
                {
                    await Task.Delay(interval);

                    Console.WriteLine("Removing...");
                    cache.Remove(Key);
                }
            });
        }
    }
}
