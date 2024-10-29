// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System.Collections;
using ISL.Providers.ReIdentification.Abstractions.Models.Exceptions;
using Xeptions;

namespace ISL.Providers.ReIdentification.Abstractions.Tests.Unit.Models.Exceptions
{
    public class SomeNotificationDependencyValidationException : Xeption, IReIdentificationProviderDependencyValidationException
    {
        public SomeNotificationDependencyValidationException(string message, Xeption innerException, IDictionary data)
            : base(message, innerException, data)
        { }
    }
}
