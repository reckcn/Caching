// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Threading;
using Microsoft.Framework.Caching;
using Microsoft.Framework.Caching.Memory;

namespace MemoryCacheSample
{
    public class Program
    {
        public void Main()
        {
            IMemoryCache cache = new MemoryCache(new MemoryCacheOptions());
            object result;
            string key = "Key";
            object newObject = new object();
            object state = new object();

            // Basic CRUD operations:

            // Create / Overwrite
            result = cache.Set(key, newObject);
            result = cache.Set(key, new object());

            // Retrieve, null if not found
            result = cache.Get(key);

            // Retrieve
            bool found = cache.TryGetValue(key, out result);

            // Delete
            cache.Remove(key);

            // Cache entry configuration:

            // Stays in the cache as long as possible
            result = cache.Set(
                key,
                new object(),
                new CacheEntryOptions().SetPriority(CacheItemPriority.NeverRemove));

            // Automatically remove if not accessed in the given time
            result = cache.Set(
                key,
                new object(),
                new CacheEntryOptions().SetSlidingExpiration(TimeSpan.FromMinutes(5)));

            // Automatically remove at a certain time
            result = cache.Set(
                key,
                new object(),
                new CacheEntryOptions().SetAbsoluteExpiration(new DateTime(2014, 12, 31)));

            // Automatically remove if not accessed in the given time
            // Automatically remove at a certain time (if it lives that long)
            result = cache.Set(
                key,
                new object(),
                new CacheEntryOptions()
                .SetSlidingExpiration(TimeSpan.FromMinutes(5))
                .SetAbsoluteExpiration(new DateTime(2014, 12, 31)));

            // Callback when evicted
            result = cache.Set(
                key,
                new object(),
                new CacheEntryOptions()
                .RegisterPostEvictionCallback((echoKey, value, reason, substate) =>
                    Console.WriteLine(echoKey + ": '" + value + "' was evicted due to " + reason), state: null));

            // Remove on trigger
            var cts = new CancellationTokenSource();
            result = cache.Set(
                key,
                new object(),
                new CacheEntryOptions()
                .AddExpirationTrigger(new CancellationTokenTrigger(cts.Token)));

            //result = cache.GetOrSet<object>(key, context =>
            //{
            //    var link = new EntryLink();

            //    var inner1 = cache.GetOrSet("subkey1", link, subContext =>
            //    {
            //        return "SubValue1";
            //    });

            //    string inner2;
            //    using (link.FlowContext())
            //    {
            //        inner2 = cache.GetOrSet("subkey2", subContext =>
            //        {
            //            return "SubValue2";
            //        });
            //    }

            //    context.AddEntryLink(link);

            //    return inner1 + inner2;
            //});
        }
    }
}
