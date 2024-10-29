// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System.Collections;
using ISL.Providers.ReIdentification.Abstractions.Models.Exceptions;
using Xeptions;

namespace ISL.Providers.ReIdentification.Necs.Models.Providers.Exceptions
{
    /// <summary>
    /// This exception is thrown when a validation error occurs while using the notification provider.
    /// For example, if required data is missing or invalid.
    /// </summary>
    public class NecsReIdentificationProviderValidationException : Xeption, IReIdentificationProviderValidationException
    {
        public NecsReIdentificationProviderValidationException(string message, Xeption innerException, IDictionary data)
            : base(message: message, innerException, data)
        { }
    }
}