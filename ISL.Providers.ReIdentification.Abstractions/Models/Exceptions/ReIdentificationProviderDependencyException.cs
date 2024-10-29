// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System.Collections;
using Xeptions;

namespace ISL.Providers.ReIdentification.Abstractions.Models.Exceptions
{
    public class ReIdentificationProviderDependencyException : Xeption
    {
        public ReIdentificationProviderDependencyException(string message, Xeption innerException)
        : base(message, innerException)
        { }

        public ReIdentificationProviderDependencyException(string message, Xeption innerException, IDictionary data)
            : base(message, innerException, data)
        { }
    }
}
