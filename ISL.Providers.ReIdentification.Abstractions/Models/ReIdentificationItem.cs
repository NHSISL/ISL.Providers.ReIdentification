﻿// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;

namespace ISL.Providers.ReIdentification.Abstractions.Models
{
    public class ReIdentificationItem
    {
        public string RowNumber { get; set; }
        public string Identifier { get; set; }
        public string Message { get; set; }
        public Guid AssociatedRequestId { get; set; }
    }
}
