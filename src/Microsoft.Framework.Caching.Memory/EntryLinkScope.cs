// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;

namespace Microsoft.Framework.Caching.Memory
{
    public class EntryLinkScope : IDisposable
    {
        public EntryLinkScope()
        {
            Parent = EntryLinkHelpers.CurrentScope;
            EntryLink = new EntryLink();
        }

        public EntryLinkScope Parent { get; }

        public IEntryLink EntryLink { get; }

        private bool _disposedValue = false; // To detect redundant calls
        public void Dispose()
        {
            if (!_disposedValue)
            {
                RemoveScope();
            }
            _disposedValue = true;
        }

        private void RemoveScope()
        {
            for (var scope = EntryLinkHelpers.CurrentScope; scope != null; scope = scope.Parent)
            {
                if (ReferenceEquals(scope, this))
                {
                    EntryLinkHelpers.CurrentScope = Parent;
                }
            }
        }
    }
}
