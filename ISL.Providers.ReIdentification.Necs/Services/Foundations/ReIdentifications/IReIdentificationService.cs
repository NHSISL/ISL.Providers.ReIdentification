// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System.Threading.Tasks;
using ISL.Providers.ReIdentification.Abstractions.Models;

namespace ISL.Providers.ReIdentification.Necs.Services.Foundations.ReIdentifications
{
    public interface IReIdentificationService
    {
        ValueTask<ReIdentificationRequest> ProcessReIdentificationRequest(
            ReIdentificationRequest reIdentificationRequest);
    }
}
