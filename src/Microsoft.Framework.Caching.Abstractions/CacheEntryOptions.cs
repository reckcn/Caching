﻿// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using Microsoft.Framework.Caching.Memory;

namespace Microsoft.Framework.Caching
{
    public class CacheEntryOptions
    {
        private DateTimeOffset? _absoluteExpiration;
        private TimeSpan? _absoluteExpirationRelativeToNow;
        private TimeSpan? _slidingExpiration;

        public CacheEntryOptions()
        {
            Triggers = new List<IExpirationTrigger>();
            PostEvictionCallbacks = new List<PostEvictionCallbackRegistration>();
        }

        /// <summary>
        /// Gets or sets an absolute expiration date for the cache entry.
        /// </summary>
        public DateTimeOffset? AbsoluteExpiration
        {
            get
            {
                return _absoluteExpiration;
            }
            set
            {
                if (AbsoluteExpirationRelativeToNow != null)
                {
                    throw new InvalidOperationException(
                        $"Cannot set both the properties '{nameof(AbsoluteExpirationRelativeToNow)}' " +
                        $"and '{nameof(AbsoluteExpiration)}' for absolute expiration time.");
                }

                _absoluteExpiration = value;
            }
        }

        /// <summary>
        /// Gets or sets an absolute expiration time, relative to now.
        /// </summary>
        public TimeSpan? AbsoluteExpirationRelativeToNow
        {
            get
            {
                return _absoluteExpirationRelativeToNow;
            }
            set
            {
                if (AbsoluteExpiration != null)
                {
                    throw new InvalidOperationException(
                        $"Cannot set both the properties '{nameof(AbsoluteExpirationRelativeToNow)}' " +
                        $"and '{nameof(AbsoluteExpiration)}' for absolute expiration time.");
                }

                if (value <= TimeSpan.Zero)
                {
                    throw new ArgumentOutOfRangeException(
                        nameof(AbsoluteExpirationRelativeToNow),
                        value,
                        "The relative expiration value must be positive.");
                }

                _absoluteExpirationRelativeToNow = value;
            }
        }

        /// <summary>
        /// Gets or sets how long a cache entry can be inactive (e.g. not accessed) before it will be removed.
        /// This will not extend the entry lifetime beyond the absolute expiration (if set).
        /// </summary>
        public TimeSpan? SlidingExpiration
        {
            get
            {
                return _slidingExpiration;
            }
            set
            {
                if (value <= TimeSpan.Zero)
                {
                    throw new ArgumentOutOfRangeException(
                        nameof(SlidingExpiration),
                        value,
                        "The sliding expiration value must be positive.");
                }

                _slidingExpiration = value;
            }
        }

        /// <summary>
        /// Gets or sets the events which are fired when the cache entry expires.
        /// </summary>
        public IList<IExpirationTrigger> Triggers { get; set; }

        /// <summary>
        /// Gets or sets the callbacks will be fired after the cache entry is evicted from the cache.
        /// </summary>
        public IList<PostEvictionCallbackRegistration> PostEvictionCallbacks { get; set; }

        /// <summary>
        /// Gets or sets the priority for keeping the cache entry in the cache during a
        /// memory pressure triggered cleanup.
        /// </summary>
        public CacheItemPriority Priority { get; set; }
    }
}