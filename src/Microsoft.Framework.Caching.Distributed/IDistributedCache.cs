// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading.Tasks;
using Microsoft.Framework.Caching.Memory;

namespace Microsoft.Framework.Caching.Distributed
{
    public interface IDistributedCache
    {
        byte[] Get(string key);

        Task<byte[]> GetAsync(string key);

        bool TryGetValue(string key, out byte[] value);

        Task<bool> TryGetValueAsync(string key, out byte[] value);

        void Set(string key, byte[] value, CacheEntryOptions options);

        Task SetAsync(string key, byte[] value, CacheEntryOptions options);

        void Refresh(string key);

        void Remove(string key);

        //TODO: RemoveAsync??
    }
}