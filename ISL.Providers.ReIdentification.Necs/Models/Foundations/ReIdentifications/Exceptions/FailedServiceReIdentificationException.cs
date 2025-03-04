﻿// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Collections;
using Xeptions;

namespace ISL.Providers.ReIdentification.Necs.Models.Foundations.ReIdentifications.Exceptions
{
    public class FailedServiceReIdentificationException : Xeption
    {
        public FailedServiceReIdentificationException(string message, Exception innerException, IDictionary data)
            : base(message, innerException, data)
        { }
    }
}
