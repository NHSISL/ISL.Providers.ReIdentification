// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System.Linq;
using System.Threading.Tasks;
using Force.DeepCloner;
using ISL.Providers.ReIdentification.Abstractions.Models;
using ISL.Providers.ReIdentification.OfflineFileSources.Brokers.OfflineSources;
using ISL.Providers.ReIdentification.OfflineFileSources.Models;

namespace ISL.Providers.ReIdentification.OfflineFileSources.Services.Foundations.ReIdentifications
{
    internal partial class ReIdentificationService : IReIdentificationService
    {
        private readonly IOfflineSourceBroker offlineSourceBroker;
        private readonly OfflineSourceConfiguration offlineSourceConfiguration;

        public ReIdentificationService(
            IOfflineSourceBroker offlineSourceBroker,
            OfflineSourceConfiguration offlineSourceConfiguration)
        {
            this.offlineSourceBroker = offlineSourceBroker;
            this.offlineSourceConfiguration = offlineSourceConfiguration;
        }

        public ValueTask<ReIdentificationRequest> ProcessReIdentificationRequest(
            ReIdentificationRequest reIdentificationRequest) =>
            TryCatch(async () =>
            {
                ValidateIdentificationRequestOnProcess(reIdentificationRequest);

                var returnItems = reIdentificationRequest.DeepClone();

                foreach (var item in returnItems.ReIdentificationItems)
                {
                    if (string.IsNullOrWhiteSpace(item.RowNumber))
                    {
                        item.Identifier = offlineSourceConfiguration.DefaultIdentifier;
                        item.Message = "Each identifier must have a corresponding row number.";
                        continue;
                    }

                    if (
                        string.IsNullOrWhiteSpace(item.Identifier)
                        || item.Identifier.Length != 10
                        || item.Identifier.All(char.IsDigit) is false)
                    {
                        item.Identifier = offlineSourceConfiguration.DefaultIdentifier;
                        item.Message = "Identifier must be exactly 10 digits.";
                        continue;
                    }

                    var matchQuery = await offlineSourceBroker.GetIdentificationPairsAsync();
                    var maybeMatch = matchQuery.FirstOrDefault(i => i.PseudoNumber == item.Identifier);

                    if (maybeMatch is null)
                    {
                        item.Identifier = offlineSourceConfiguration.DefaultIdentifier;
                        item.Message = "Pseudo identifier not found in the offline source.";
                        continue;
                    }

                    item.Identifier = maybeMatch.NhsNumber;
                    item.Message = "OK";
                }

                return returnItems;
            });
    }
}
