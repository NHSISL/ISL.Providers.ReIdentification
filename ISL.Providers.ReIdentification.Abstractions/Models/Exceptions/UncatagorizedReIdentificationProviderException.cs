// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Collections;
using Xeptions;

namespace ISL.Providers.ReIdentification.Abstractions.Models.Exceptions
{
    public class UncatagorizedReIdentificationProviderException : Xeption
    {
        public UncatagorizedReIdentificationProviderException(string message, Exception innerException)
            : base(message, innerException)
        { }

        public UncatagorizedReIdentificationProviderException(
            string message,
            Exception innerException,
            IDictionary data)
            : base(message, innerException, data)
        { }
    }
}
