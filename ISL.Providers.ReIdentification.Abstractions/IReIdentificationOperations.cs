// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System.Threading.Tasks;
using ISL.Providers.ReIdentification.Abstractions.Models;

namespace ISL.Providers.ReIdentification.Abstractions
{
    public interface IReIdentificationOperations
    {
        /// <summary>
        /// Re-identifies a patient from a list of pseudo identifiers.
        /// </summary>
        /// <returns>
        /// A re-identification request where the pseudo identfiers has been replaced by real identifiers.
        /// If the re-identification could not happen due to pseudo identifiers being valid, the identifier will be
        /// replaced by 0000000000 and the message field will be populated with a reason.
        /// </returns>
        ValueTask<ReIdentificationRequest> ReIdentifyAsync(ReIdentificationRequest reIdentificationRequest);
    }
}
