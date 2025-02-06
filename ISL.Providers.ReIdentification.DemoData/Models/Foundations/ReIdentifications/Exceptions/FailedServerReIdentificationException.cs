// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Collections;
using Xeptions;

namespace ISL.Providers.ReIdentification.DemoData.Models.Foundations.ReIdentifications.Exceptions
{
    public class FailedServerReIdentificationException : Xeption
    {
        public FailedServerReIdentificationException(string message, Exception innerException, IDictionary data)
            : base(message, innerException, data)
        { }
    }
}
