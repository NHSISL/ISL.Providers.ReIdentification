// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System.Threading.Tasks;
using FluentAssertions;
using Force.DeepCloner;
using ISL.Providers.ReIdentification.Abstractions.Models;
using ISL.Providers.ReIdentification.Necs.Models.Brokers.NECS.Requests;
using ISL.Providers.ReIdentification.Necs.Models.Brokers.NECS.Responses;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;

namespace ISL.NotificationClient.Tests.Acceptance
{
    public partial class NecsProviderTests
    {
        [Fact]
        public async Task ShouldReIdentifyAsync()
        {
            // given
            int reidentificationItems = GetRandomNumber();
            ReIdentificationRequest randomRequest = CreateRandomReIdentificationRequest(count: reidentificationItems);
            ReIdentificationRequest randomResponse = randomRequest.DeepClone();
            randomResponse.ReIdentificationItems.ForEach(item => item.Identifier = GetRandomString());
            ReIdentificationRequest request = randomRequest;
            ReIdentificationRequest response = randomResponse;
            ReIdentificationRequest expectedResponse = response.DeepClone();
            NecsReIdentificationRequest reIdentificationRequest = MapToNecsReIdentificationRequest(randomRequest);
            NecsReIdentificationResponse reIdentificationResponse = MapToNecsReIdentificationResponse(randomResponse);
            var path = "/api/Reid/Process";

            this.wireMockServer
                .Given(
                    Request.Create()
                        .WithPath(path)
                        .UsingPost()
                        .WithHeader("X-API-KEY", this.necsReIdentificationConfigurations.ApiKey))
                .RespondWith(
                    Response.Create()
                        .WithSuccess()
                        .WithHeader("Content-Type", "application/json")
                        .WithBodyAsJson(reIdentificationResponse));

            // when
            ReIdentificationRequest actualResponse =
                await this.necsReIdentificationProvider.ReIdentifyAsync(reIdentificationRequest: request);

            // then
            actualResponse.Should().BeEquivalentTo(expectedResponse);
        }
    }
}
