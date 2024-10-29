// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using ISL.Providers.ReIdentification.Abstractions.Models.Exceptions;
using Xeptions;

namespace ISL.Providers.ReIdentification.Necs.Models.Providers.Exceptions
{
    /// <summary>
    /// This exception is thrown when a dependency error occurs while using the notification provider.
    /// For example, if a required dependency is unavailable or incompatible.
    /// </summary>
    public class NecsReIdentificationProviderDependencyException
        : Xeption, IReIdentificationProviderDependencyException
    {
        public NecsReIdentificationProviderDependencyException(string message, Xeption innerException)
            : base(message, innerException)
        { }
    }
}
