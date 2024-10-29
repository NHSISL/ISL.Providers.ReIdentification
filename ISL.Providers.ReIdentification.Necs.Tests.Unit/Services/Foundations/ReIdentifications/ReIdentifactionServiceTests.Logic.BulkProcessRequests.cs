// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using Force.DeepCloner;
using ISL.Providers.ReIdentification.Abstractions.Models;
using ISL.Providers.ReIdentification.Necs.Models.Brokers.NECS.Requests;
using ISL.Providers.ReIdentification.Necs.Models.Brokers.NECS.Responses;
using ISL.Providers.ReIdentification.Necs.Services.Foundations.ReIdentifications;
using Moq;

namespace ISL.Providers.ReIdentification.Necs.Tests.Unit.Services.Foundations.ReIdentifications
{
    public partial class ReIdentificationServiceTests
    {
        [Fact]
        public async Task ShouldBulkProcessRequestsAsync()
        {
            // Given
            Guid randomIdentifier = Guid.NewGuid();
            int batchSize = GetRandomNumber();
            int randomCount = (batchSize * GetRandomNumber()) + GetRandomNumber();

            ReIdentificationRequest randomReIdentificationRequest =
                CreateRandomReIdentificationRequest(count: randomCount);

            (List<NecsReIdentificationRequest> requests, List<NecsReIdentificationResponse> responses) =
                CreateBatchedItems(randomReIdentificationRequest, batchSize, randomIdentifier);

            for (int i = 0; i < requests.Count; i++)
            {
                NecsReIdentificationRequest necsReIdentificationRequest = requests[i];
                NecsReIdentificationResponse necsReIdentificationResponse = responses[i];

                this.necsBrokerMock.Setup(broker =>
                    broker.ReIdAsync(It.Is(SameNecsReIdentificationRequestAs(necsReIdentificationRequest))))
                        .ReturnsAsync(necsReIdentificationResponse);
            }

            ReIdentificationRequest inputReIdentificationRequest = randomReIdentificationRequest;
            ReIdentificationRequest outputReIdentificationRequest = inputReIdentificationRequest.DeepClone();

            outputReIdentificationRequest.ReIdentificationItems.ForEach(item =>
                {
                    item.Identifier = $"{item.Identifier}R";
                    item.Message = $"{item.Message}M";
                });

            ReIdentificationRequest expectedReIdentificationRequest = outputReIdentificationRequest.DeepClone();

            Mock<ReIdentificationService> reIdentificationServiceMock =
                new Mock<ReIdentificationService>(
                    this.necsBrokerMock.Object,
                    this.identifierBrokerMock.Object,
                    this.necsReIdentificationConfigurations)
                { CallBase = true };

            ReIdentificationService service = reIdentificationServiceMock.Object;

            this.identifierBrokerMock.Setup(broker =>
                broker.GetIdentifierAsync())
                    .ReturnsAsync(randomIdentifier);

            // When
            ReIdentificationRequest actualReIdentificationRequest = await service
                .BulkProcessRequestsAsync(inputReIdentificationRequest, batchSize);

            // Then
            actualReIdentificationRequest.Should().BeEquivalentTo(expectedReIdentificationRequest);

            for (int i = 0; i < requests.Count; i++)
            {
                NecsReIdentificationRequest necsReIdentificationRequest = requests[i];
                NecsReIdentificationResponse necsReIdentificationResponse = responses[i];

                this.necsBrokerMock.Verify(broker =>
                    broker.ReIdAsync(It.Is(SameNecsReIdentificationRequestAs(necsReIdentificationRequest))),
                        Times.Once);
            }

            this.identifierBrokerMock.Verify(broker =>
                broker.GetIdentifierAsync(),
                    Times.Exactly(requests.Count));

            this.necsBrokerMock.VerifyNoOtherCalls();
            this.identifierBrokerMock.VerifyNoOtherCalls();
        }
    }
}
