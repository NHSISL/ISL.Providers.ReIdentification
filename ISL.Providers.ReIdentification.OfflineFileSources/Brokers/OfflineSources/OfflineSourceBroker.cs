﻿// ---------------------------------------------------------
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

        public OfflineSourceBroker(OfflineSourceConfiguration offlineSourceConfiguration) =>
            this.IdentificationPairs = InitializeAsync(offlineSourceConfiguration).Result;

        public async ValueTask<List<IdentificationPair>> GetIdentificationPairsAsync() =>
            this.IdentificationPairs;

        private async ValueTask<List<IdentificationPair>> InitializeAsync(
            OfflineSourceConfiguration offlineSourceConfiguration)
        {
            string fileContents =
                await File.ReadAllTextAsync(offlineSourceConfiguration.FilePath);

            var csvClient = new CsvClient();

            Dictionary<string, int> fieldMappings = new Dictionary<string, int>
            {
                { nameof(IdentificationPair.PseudoNumber), 0 },
                { nameof(IdentificationPair.NhsNumber), 1 },
            };

            return await csvClient.MapCsvToObjectAsync<IdentificationPair>(
                fileContents, offlineSourceConfiguration.HasHeaderRecord, fieldMappings);
        }

    }
}