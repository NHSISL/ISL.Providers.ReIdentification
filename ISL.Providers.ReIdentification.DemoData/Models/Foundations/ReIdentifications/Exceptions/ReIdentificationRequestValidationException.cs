// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using Xeptions;

namespace ISL.Providers.ReIdentification.DemoData.Models.Foundations.ReIdentifications.Exceptions
{
    public class ReIdentificationRequestValidationException : Xeption
    {
        public ReIdentificationRequestValidationException(string message, Xeption innerException)
            : base(message, innerException)
        { }
    }
}
