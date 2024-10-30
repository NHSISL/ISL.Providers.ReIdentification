// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using Xeptions;

namespace ISL.Providers.ReIdentification.OfflineFileSources.Models.Foundations.ReIdentifications.Exceptions
{
    public class InvalidReIdentificationRequestException : Xeption
    {
        public InvalidReIdentificationRequestException(string message)
            : base(message)
        { }
    }
}
