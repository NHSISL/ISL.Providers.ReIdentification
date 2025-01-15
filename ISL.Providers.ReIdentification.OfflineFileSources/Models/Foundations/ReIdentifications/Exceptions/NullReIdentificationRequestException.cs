﻿// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using Xeptions;

namespace ISL.Providers.ReIdentification.OfflineFileSources.Models.Foundations.ReIdentifications.Exceptions
{
    public class NullReIdentificationRequestException : Xeption
    {
        public NullReIdentificationRequestException(string message)
            : base(message)
        { }
    }
}
