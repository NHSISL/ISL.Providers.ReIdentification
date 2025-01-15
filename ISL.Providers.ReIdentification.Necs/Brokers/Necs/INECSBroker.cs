// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System.Threading.Tasks;
using ISL.Providers.ReIdentification.Necs.Models.Brokers.NECS.Requests;
using ISL.Providers.ReIdentification.Necs.Models.Brokers.NECS.Responses;

namespace ISL.Providers.ReIdentification.Necs.Brokers.Necs
{
    internal interface INECSBroker
    {
        ValueTask<NecsReIdentificationResponse> ReIdAsync(NecsReIdentificationRequest necsReIdentificationRequest);
    }
}
