// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Threading.Tasks;

namespace ISL.Providers.ReIdentification.Necs.Brokers.Identifiers
{
    internal interface IIdentifierBroker
    {
        ValueTask<Guid> GetIdentifierAsync();
    }
}
