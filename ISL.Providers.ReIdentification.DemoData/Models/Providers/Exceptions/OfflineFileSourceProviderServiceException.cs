// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using ISL.Providers.ReIdentification.Abstractions.Models.Exceptions;
using Xeptions;

namespace ISL.Providers.ReIdentification.DemoData.Models.Providers.Exceptions
{
    /// <summary>
    /// This exception is thrown when a service error occurs while using the provider. 
    /// For example, if there is a problem with the server or any other service failure.
    /// </summary>
    public class DemoDataProviderServiceException : Xeption, IReIdentificationProviderServiceException
    {
        public DemoDataProviderServiceException(string message, Xeption innerException)
            : base(message, innerException)
        { }
    }
}
