// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System.Collections.Generic;
using System.Threading.Tasks;
using ISL.Providers.ReIdentification.OfflineFileSources.Models;

namespace ISL.Providers.ReIdentification.OfflineFileSources.Brokers.OfflineSources
{
    internal interface IOfflineSourceBroker
    {
        ValueTask<List<IdentificationPair>> GetIdentificationPairsAsync();
    }
}
