// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

namespace ISL.Providers.ReIdentification.OfflineFileSources.Models
{
    public class OfflineSourceReIdentificationConfiguration
    {
        public string FilePath { get; set; }
        public bool HasHeaderRecord { get; set; } = false;
        public string DefaultIdentifier { get; set; } = "0000000000";
    }
}
