// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Globalization;
using System.Threading;
using Microsoft.AspNet.Testing;
using Microsoft.Framework.Caching.Memory.Infrastructure;
using Xunit;

namespace Microsoft.Framework.Caching.Memory
{
    public class TimeExpirationTests
    {
        private IMemoryCache CreateCache(ISystemClock clock)
        {
            return new MemoryCache(new MemoryCacheOptions()
            {
                Clock = clock,
                CompactOnMemoryPressure = false,
            });
        }

        [Fact]
        public void AbsoluteExpirationInThePastThrows()
        {
            var clock = new TestClock();
            var cache = CreateCache(clock);
            var key = "myKey";
            var obj = new object();

            var expected = clock.UtcNow - TimeSpan.FromMinutes(1);
            ExceptionAssert.ThrowsArgumentOutOfRange(() =>
            {
                var result = cache.Set(key, obj, new CacheEntryOptions().SetAbsoluteExpiration(expected));

            },
            nameof(CacheEntryOptions.AbsoluteExpiration),
            "The absolute expiration value must be in the future.",
            expected.ToString(CultureInfo.CurrentCulture));
        }

        [Fact]
        public void AbsoluteExpirationExpires()
        {
            var clock = new TestClock();
            var cache = CreateCache(clock);
            var key = "myKey";
            var value = new object();

            var result = cache.Set(key, value, new CacheEntryOptions()
                .SetAbsoluteExpiration(clock.UtcNow + TimeSpan.FromMinutes(1)));
            Assert.Same(value, result);

            var found = cache.TryGetValue(key, out result);
            Assert.True(found);
            Assert.Same(value, result);

            clock.Add(TimeSpan.FromMinutes(2));

            found = cache.TryGetValue(key, out result);
            Assert.False(found);
            Assert.Null(result);
        }

        [Fact]
        public void AbsoluteExpirationExpiresInBackground()
        {
            var clock = new TestClock();
            var cache = CreateCache(clock);
            var key = "myKey";
            var value = new object();
            var callbackInvoked = new ManualResetEvent(false);

            var options = new CacheEntryOptions()
                .SetAbsoluteExpiration(clock.UtcNow + TimeSpan.FromMinutes(1))
                .RegisterPostEvictionCallback((subkey, subValue, reason, state) =>
                {
                    // TODO: Verify params
                    var localCallbackInvoked = (ManualResetEvent)state;
                    localCallbackInvoked.Set();
                }, callbackInvoked);
            var result = cache.Set(key, value, options);
            Assert.Same(value, result);

            var found = cache.TryGetValue(key, out result);
            Assert.True(found);
            Assert.Same(value, result);

            clock.Add(TimeSpan.FromMinutes(2));
            var ignored = cache.Get("otherKey"); // Background expiration checks are triggered by misc cache activity.

            Assert.True(callbackInvoked.WaitOne(100), "Callback");

            found = cache.TryGetValue(key, out result);
            Assert.False(found);
            Assert.Null(result);
        }

        [Fact]
        public void NegativeRelativeExpirationThrows()
        {
            var clock = new TestClock();
            var cache = CreateCache(clock);
            var key = "myKey";
            var value = new object();

            ExceptionAssert.ThrowsArgumentOutOfRange(() =>
            {
                var result = cache.Set(key, value, new CacheEntryOptions()
                    .SetAbsoluteExpiration(TimeSpan.FromMinutes(-1)));
            },
            nameof(CacheEntryOptions.AbsoluteExpirationRelativeToNow),
            "The relative expiration value must be positive.",
            TimeSpan.FromMinutes(-1));
        }

        [Fact]
        public void ZeroRelativeExpirationThrows()
        {
            var clock = new TestClock();
            var cache = CreateCache(clock);
            var key = "myKey";
            var value = new object();

            ExceptionAssert.ThrowsArgumentOutOfRange(() =>
            {
                var result = cache.Set(key, value, new CacheEntryOptions().SetAbsoluteExpiration(TimeSpan.Zero));
            },
            nameof(CacheEntryOptions.AbsoluteExpirationRelativeToNow),
            "The relative expiration value must be positive.",
            TimeSpan.Zero);
        }

        [Fact]
        public void RelativeExpirationExpires()
        {
            var clock = new TestClock();
            var cache = CreateCache(clock);
            var key = "myKey";
            var value = new object();

            var result = cache.Set(key, value, new CacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromMinutes(1)));
            Assert.Same(value, result);

            var found = cache.TryGetValue(key, out result);
            Assert.True(found);
            Assert.Same(value, result);

            clock.Add(TimeSpan.FromMinutes(2));

            found = cache.TryGetValue(key, out result);
            Assert.False(found);
            Assert.Null(result);
        }

        [Fact]
        public void NegativeSlidingExpirationThrows()
        {
            var clock = new TestClock();
            var cache = CreateCache(clock);
            var key = "myKey";
            var value = new object();

            ExceptionAssert.ThrowsArgumentOutOfRange(() =>
            {
                var result = cache.Set(key, value, new CacheEntryOptions()
                    .SetSlidingExpiration(TimeSpan.FromMinutes(-1)));
            },
            nameof(CacheEntryOptions.SlidingExpiration),
            "The sliding expiration value must be positive.",
            TimeSpan.FromMinutes(-1));
        }

        [Fact]
        public void ZeroSlidingExpirationThrows()
        {
            var clock = new TestClock();
            var cache = CreateCache(clock);
            var key = "myKey";
            var value = new object();

            ExceptionAssert.ThrowsArgumentOutOfRange(() =>
            {
                var result = cache.Set(key, value, new CacheEntryOptions()
                    .SetSlidingExpiration(TimeSpan.Zero));
            },
            nameof(CacheEntryOptions.SlidingExpiration),
            "The sliding expiration value must be positive.",
            TimeSpan.Zero);
        }

        [Fact]
        public void SlidingExpirationExpiresIfNotAccessed()
        {
            var clock = new TestClock();
            var cache = CreateCache(clock);
            var key = "myKey";
            var value = new object();

            var result = cache.Set(key, value, new CacheEntryOptions().SetSlidingExpiration(TimeSpan.FromMinutes(1)));
            Assert.Same(value, result);

            var found = cache.TryGetValue(key, out result);
            Assert.True(found);
            Assert.Same(value, result);

            clock.Add(TimeSpan.FromMinutes(2));

            found = cache.TryGetValue(key, out result);
            Assert.False(found);
            Assert.Null(result);
        }

        [Fact]
        public void SlidingExpirationRenewedByAccess()
        {
            var clock = new TestClock();
            var cache = CreateCache(clock);
            var key = "myKey";
            var value = new object();

            var result = cache.Set(key, value, new CacheEntryOptions()
                .SetSlidingExpiration(TimeSpan.FromMinutes(1)));
            Assert.Same(value, result);

            var found = cache.TryGetValue(key, out result);
            Assert.True(found);
            Assert.Same(value, result);

            for (int i = 0; i < 10; i++)
            {
                clock.Add(TimeSpan.FromSeconds(15));

                found = cache.TryGetValue(key, out result);
                Assert.True(found);
                Assert.Same(value, result);
            }
        }

        [Fact]
        public void SlidingExpirationRenewedByAccessUntilAbsoluteExpiration()
        {
            var clock = new TestClock();
            var cache = CreateCache(clock);
            var key = "myKey";
            var value = new object();

            var result = cache.Set(key, value, new CacheEntryOptions()
                .SetSlidingExpiration(TimeSpan.FromMinutes(1))
                .SetAbsoluteExpiration(TimeSpan.FromMinutes(2)));
            Assert.Same(value, result);

            var found = cache.TryGetValue(key, out result);
            Assert.True(found);
            Assert.Same(value, result);

            for (int i = 0; i < 7; i++)
            {
                clock.Add(TimeSpan.FromSeconds(15));

                found = cache.TryGetValue(key, out result);
                Assert.True(found);
                Assert.Same(value, result);
            }

            clock.Add(TimeSpan.FromSeconds(15));

            found = cache.TryGetValue(key, out result);
            Assert.False(found);
            Assert.Null(result);
        }
    }
}