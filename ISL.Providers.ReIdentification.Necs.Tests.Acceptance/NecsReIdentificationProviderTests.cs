// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System.Collections.Generic;
using System.Linq;
using ISL.Providers.ReIdentification.Abstractions.Models;
using ISL.Providers.ReIdentification.Necs.Models.Brokers.NECS;
using ISL.Providers.ReIdentification.Necs.Models.Brokers.NECS.Requests;
using ISL.Providers.ReIdentification.Necs.Models.Brokers.NECS.Responses;
using ISL.Providers.ReIdentification.Necs.Providers.NecsReIdentifications;
using Microsoft.Extensions.Configuration;
using Tynamix.ObjectFiller;
using WireMock.Server;

namespace ISL.NotificationClient.Tests.Acceptance
{
    public partial class NecsProviderTests
    {
        private readonly INecsReIdentificationProvider necsReIdentificationProvider;
        private readonly NecsReIdentificationConfigurations necsReIdentificationConfigurations;
        private readonly WireMockServer wireMockServer;
        private readonly IConfiguration configuration;

        public NecsProviderTests()
        {
            var configurationBuilder = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile("appsettings.Development.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables();

            configuration = configurationBuilder.Build();
            this.wireMockServer = WireMockServer.Start();

            this.necsReIdentificationConfigurations = configuration
                .GetSection("necsReIdentificationConfigurations").Get<NecsReIdentificationConfigurations>();

            necsReIdentificationConfigurations.ApiUrl = wireMockServer.Url;

            this.necsReIdentificationProvider = new NecsReIdentificationProvider(necsReIdentificationConfigurations);
        }

        private static string GetRandomEmail() =>
            new EmailAddresses().GetValue();

        private static string GetRandomString() =>
            new MnemonicString(wordCount: GetRandomNumber()).GetValue();

        private static int GetRandomNumber() =>
        new IntRange(min: 2, max: 10).GetValue();


        private static ReIdentificationRequest CreateRandomReIdentificationRequest(int count) =>
        CreateReIdentificationRequestFiller(count).Create();

        private static Filler<ReIdentificationRequest> CreateReIdentificationRequestFiller(int count)
        {
            var filler = new Filler<ReIdentificationRequest>();

            filler.Setup()
                .OnProperty(request => request.ReIdentificationItems).Use(CreateRandomReIdentificationItems(count));

            return filler;
        }

        private static List<ReIdentificationItem> CreateRandomReIdentificationItems(int count)
        {
            return CreateReIdentificationItemFiller()
            .Create(count)
                    .ToList();
        }

        private static Filler<ReIdentificationItem> CreateReIdentificationItemFiller()
        {
            string message = null;
            var filler = new Filler<ReIdentificationItem>();

            filler.Setup()
                .OnProperty(request => request.Message).Use(message);

            return filler;
        }

        private NecsReIdentificationRequest MapToNecsReIdentificationRequest(ReIdentificationRequest randomRequest)
        {
            NecsReIdentificationRequest request = new NecsReIdentificationRequest
            {
                RequestId = randomRequest.RequestId,
                UserIdentifier = randomRequest.UserIdentifier,
                Organisation = randomRequest.Organisation,
                Reason = randomRequest.Reason,

                PseudonymisedNumbers = randomRequest.ReIdentificationItems
                    .Select(item => new NecsPseudonymisedItem { RowNumber = item.RowNumber, Pseudo = item.Identifier })
                    .ToList(),
            };

            return request;
        }

        private NecsReIdentificationResponse MapToNecsReIdentificationResponse(ReIdentificationRequest randomResponse)
        {
            NecsReIdentificationResponse response = new NecsReIdentificationResponse
            {
                UniqueRequestId = randomResponse.RequestId,
                ElapsedTime = GetRandomNumber(),
                ProcessedCount = randomResponse.ReIdentificationItems.Count,
                Results = randomResponse.ReIdentificationItems
                    .Select(item =>
                        new NecsReidentifiedItem { RowNumber = item.RowNumber, NhsNumber = item.Identifier })
                            .ToList()
            };

            return response;
        }
    }
}