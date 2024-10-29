// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ISL.Providers.ReIdentification.Abstractions.Models;
using ISL.Providers.ReIdentification.Necs.Brokers.Identifiers;
using ISL.Providers.ReIdentification.Necs.Brokers.Necs;
using ISL.Providers.ReIdentification.Necs.Models.Brokers.NECS;
using ISL.Providers.ReIdentification.Necs.Models.Brokers.NECS.Requests;
using ISL.Providers.ReIdentification.Necs.Models.Brokers.NECS.Responses;

namespace ISL.Providers.ReIdentification.Necs.Services.Foundations.ReIdentifications
{
    public partial class ReIdentificationService : IReIdentificationService
    {
        private readonly INECSBroker necsBroker;
        private readonly IIdentifierBroker identifierBroker;
        private readonly NecsReIdentificationConfigurations necsReIdentificationConfigurations;

        public ReIdentificationService(
            INECSBroker necsBroker,
            IIdentifierBroker identifierBroker,
            NecsReIdentificationConfigurations necsReIdentificationConfigurations)
        {
            this.necsBroker = necsBroker;
            this.identifierBroker = identifierBroker;
            this.necsReIdentificationConfigurations = necsReIdentificationConfigurations;
        }

        public ValueTask<ReIdentificationRequest> ProcessReIdentificationRequest(
            ReIdentificationRequest reIdentificationRequest) =>
            TryCatch(async () =>
            {
                ValidateIdentificationRequestOnProcess(reIdentificationRequest);

                ReIdentificationRequest processedItems =
                    await BulkProcessRequestsAsync(
                        reIdentificationRequest,
                        batchSize: necsReIdentificationConfigurations.ApiMaxBatchSize);

                return processedItems;
            });

        virtual internal async ValueTask<ReIdentificationRequest> BulkProcessRequestsAsync(
            ReIdentificationRequest reIdentificationRequest, int batchSize)
        {
            int totalRecords = reIdentificationRequest.ReIdentificationItems.Count;
            var exceptions = new List<Exception>();

            for (int i = 0; i < totalRecords; i += batchSize)
            {
                try
                {
                    await TryCatch(async () =>
                    {
                        NecsReIdentificationRequest necsReIdentificationRequest = new NecsReIdentificationRequest
                        {
                            RequestId = await this.identifierBroker.GetIdentifierAsync(),
                            UserIdentifier = reIdentificationRequest.UserIdentifier,
                            Organisation = reIdentificationRequest.Organisation,
                            Reason = reIdentificationRequest.Reason,
                        };

                        List<NecsPseudonymisedItem> batch = reIdentificationRequest.ReIdentificationItems.Skip(i)
                            .Take(batchSize).ToList().Select(item =>
                                new NecsPseudonymisedItem { RowNumber = item.RowNumber, Psuedo = item.Identifier })
                                    .ToList();

                        necsReIdentificationRequest.PseudonymisedNumbers.AddRange(batch);

                        NecsReIdentificationResponse necsReIdentificationResponse =
                            await necsBroker.ReIdAsync(necsReIdentificationRequest);

                        foreach (var item in necsReIdentificationResponse.Results)
                        {
                            var record = reIdentificationRequest.ReIdentificationItems
                                .FirstOrDefault(i => i.RowNumber == item.RowNumber);

                            if (record != null)
                            {
                                record.Identifier = item.NhsNumber;
                                record.Message = item.Message;
                            }
                        }
                    });
                }
                catch (Exception ex)
                {
                    exceptions.Add(ex);
                }
            }

            if (exceptions.Any())
            {
                throw new AggregateException(
                    message: $"Unable to process addresses in 1 of the batch(es) from " +
                        $"{reIdentificationRequest.RequestId}",
                    innerExceptions: exceptions);
            }

            return reIdentificationRequest;
        }
    }
}
