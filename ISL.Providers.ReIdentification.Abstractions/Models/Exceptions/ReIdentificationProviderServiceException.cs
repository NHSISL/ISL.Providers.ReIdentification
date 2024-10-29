// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System.Collections;
using Xeptions;

namespace ISL.Providers.ReIdentification.Abstractions.Models.Exceptions
{
    public class ReIdentificationProviderServiceException : Xeption
    {
        public ReIdentificationProviderServiceException(string message, Xeption innerException)
            : base(message, innerException)
        { }

        public ReIdentificationProviderServiceException(string message, Xeption innerException, IDictionary data)
            : base(message, innerException, data)
        { }
    }
}
