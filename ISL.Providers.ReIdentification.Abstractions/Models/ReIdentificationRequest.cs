// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Collections.Generic;
using ISL.ReIdentification.Core.Models.Brokers.NECS.Requests;

namespace ISL.Providers.ReIdentification.Abstractions.Models
{
    public class ReIdentificationRequest
    {
        public Guid RequestId { get; set; }

        public List<ReIdentificationItem> ReIdentificationItems { get; set; } = new List<ReIdentificationItem>();

        public string UserIdentifier { get; set; }

        public string Organisation { get; set; }

        public string Reason { get; set; }
    }
}
