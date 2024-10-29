// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using Newtonsoft.Json;

namespace ISL.Providers.ReIdentification.Necs.Models.Brokers.NECS.Requests
{
    public class NecsPseudonymisedItem
    {
        [JsonProperty("rowId")]
        public string RowNumber { get; set; }

        [JsonProperty("psuedo")]
        public string Psuedo { get; set; }
    }
}
