// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using ISL.Providers.ReIdentification.OfflineFileSources.Models;
using NHSISL.CsvHelperClient.Clients;

namespace ISL.Providers.ReIdentification.OfflineFileSources.Brokers.OfflineSources
{
    internal class OfflineSourceBroker : IOfflineSourceBroker
    {
        private List<IdentificationPair> IdentificationPairs { get; set; }

        public OfflineSourceBroker(OfflineSourceReIdentificationConfigurations offlineSourceReIdentificationConfiguration) =>
            this.IdentificationPairs = InitializeAsync(offlineSourceReIdentificationConfiguration).Result;

        public async ValueTask<List<IdentificationPair>> GetIdentificationPairsAsync() =>
            this.IdentificationPairs;

        private async ValueTask<List<IdentificationPair>> InitializeAsync(
            OfflineSourceReIdentificationConfigurations offlineSourceReIdentificationConfiguration)
        {
            await using var csvClient = new CsvClient();

            await using FileStream fileStream =
                File.OpenRead(offlineSourceReIdentificationConfiguration.FilePath);

            Dictionary<string, int> fieldMappings = new Dictionary<string, int>
            {
                { nameof(IdentificationPair.PseudoNumber), 0 },
                { nameof(IdentificationPair.NhsNumber), 1 },
            };

            var identificationPairs = new List<IdentificationPair>();

            await foreach (IdentificationPair identificationPair in
                csvClient.MapCsvToObjectAsync<IdentificationPair>(
                    fileStream, offlineSourceReIdentificationConfiguration.HasHeaderRecord, fieldMappings))
            {
                identificationPairs.Add(identificationPair);
            }

            return identificationPairs;
        }

    }
}
