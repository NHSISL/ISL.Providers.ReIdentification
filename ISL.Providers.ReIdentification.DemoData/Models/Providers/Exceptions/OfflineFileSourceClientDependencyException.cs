// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using ISL.Providers.ReIdentification.Abstractions.Models.Exceptions;
using Xeptions;

namespace ISL.Providers.ReIdentification.DemoData.Models.Providers.Exceptions
{
    /// <summary>
    /// This exception is thrown when a dependency error occurs while using the provider.
    /// For example, if a required dependency is unavailable or incompatible.
    /// </summary>
    public class OfflineFileSourceProviderDependencyException : Xeption, IReIdentificationProviderDependencyException
    {
        public OfflineFileSourceProviderDependencyException(string message, Xeption innerException)
            : base(message, innerException)
        { }
    }
}
