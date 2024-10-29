// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using ISL.Providers.ReIdentification.Abstractions.Models.Exceptions;
using Xeptions;

namespace ISL.Providers.ReIdentification.Abstractions.Tests.Unit.Models.Exceptions
{
    public class SomeReIdentificationServiceException : Xeption, IReIdentificationProviderServiceException
    {
        public SomeReIdentificationServiceException(string message, Xeption innerException)
            : base(message, innerException)
        { }
    }
}
