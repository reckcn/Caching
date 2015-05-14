// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
#if DNXCORE50
using System.Threading;
#elif NET45 || DNX451 || DNXCORE50
using System.Runtime.Remoting;
using System.Runtime.Remoting.Messaging;
#endif

namespace Microsoft.Framework.Caching.Memory
{
    public static class EntryLinkHelpers
    {
#if DNXCORE50
        private static readonly AsyncLocal<EntryLinkScope> _entryLinkScope = new AsyncLocal<EntryLinkScope>();
        public static EntryLinkScope CurrentScope
        {
            get
            {
                return _entryLinkScope.Value;
            }
            set
            {
                _entryLinkScope.Value = value;
            }
        }
#elif NET45 || DNX451
        private static readonly string _entryLinkScopeKey = "klr.host.EntryLinkHelpers.ContextLink";

        public static EntryLinkScope CurrentScope
        {
            get
            {
                var objectHandle = CallContext.LogicalGetData(_entryLinkScopeKey) as ObjectHandle;
                return objectHandle != null ? objectHandle.Unwrap() as EntryLinkScope : null;
            }
            set
            {
                CallContext.LogicalSetData(_entryLinkScopeKey, new ObjectHandle(value));
            }
        }
#else
        public static EntryLinkScope CurrentScope
        {
            get { return null; }
            set { throw new NotImplementedException(); }
        }
#endif
    }
}
