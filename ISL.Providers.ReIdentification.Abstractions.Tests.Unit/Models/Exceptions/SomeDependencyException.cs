// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using ISL.Providers.ReIdentification.Abstractions.Models.Exceptions;
using Xeptions;

namespace ISL.Providers.ReIdentification.Abstractions.Tests.Unit.Models.Exceptions
{
    public class SomeNotificationDependencyException : Xeption, IReIdentificationProviderDependencyException
    {
        public SomeNotificationDependencyException(string message, Xeption innerException)
            : base(message, innerException)
        { }
    }
}
