// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System.Collections;
using Xeptions;

namespace ISL.Providers.ReIdentification.Abstractions.Models.Exceptions
{
    public class ReIdentificationProviderValidationException : Xeption
    {
        public ReIdentificationProviderValidationException(string message, Xeption innerException)
            : base(message, innerException)
        { }

        public ReIdentificationProviderValidationException(string message, Xeption innerException, IDictionary data)
            : base(message, innerException, data)
        { }
    }
}
