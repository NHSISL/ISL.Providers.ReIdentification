// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using Xeptions;

namespace ISL.Providers.ReIdentification.DemoData.Models.Foundations.ReIdentifications.Exceptions
{
    public class ReIdentificationServiceException : Xeption
    {
        public ReIdentificationServiceException(string message, Xeption innerException)
            : base(message, innerException)
        { }
    }
}
