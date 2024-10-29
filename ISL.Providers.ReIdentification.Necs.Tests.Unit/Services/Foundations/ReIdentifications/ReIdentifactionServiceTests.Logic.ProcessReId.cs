// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System.Threading.Tasks;
using FluentAssertions;
using Force.DeepCloner;
using ISL.Providers.ReIdentification.Abstractions.Models;
using ISL.Providers.ReIdentification.Necs.Services.Foundations.ReIdentifications;
using Moq;

namespace ISL.Providers.ReIdentification.Necs.Tests.Unit.Services.Foundations.ReIdentifications
{
    public partial class ReIdentificationServiceTests
    {
        [Fact]
        public async Task ShouldProcessReReIdentificationRequestsAsync()
        {
            // Given
            int randomCount = GetRandomNumber();
            int batchSize = this.necsReIdentificationConfigurations.ApiMaxBatchSize;

            ReIdentificationRequest randomReIdentificationRequest =
                CreateRandomReIdentificationRequest(count: randomCount);

            ReIdentificationRequest inputReIdentificationRequest = randomReIdentificationRequest;
            ReIdentificationRequest storageReIdentificationRequest = inputReIdentificationRequest.DeepClone();
            ReIdentificationRequest expectedReIdentificationRequest = storageReIdentificationRequest.DeepClone();

            Mock<ReIdentificationService> reIdentificationServiceMock =
                new Mock<ReIdentificationService>(
                    this.necsBrokerMock.Object,
                    this.identifierBrokerMock.Object,
                    this.necsReIdentificationConfigurations)
                { CallBase = true };

            reIdentificationServiceMock.Setup(service =>
                service.BulkProcessRequestsAsync(inputReIdentificationRequest, batchSize))
                    .ReturnsAsync(storageReIdentificationRequest);

            ReIdentificationService service = reIdentificationServiceMock.Object;

            // When
            ReIdentificationRequest actualReIdentificationRequest = await service
                .ProcessReIdentificationRequest(inputReIdentificationRequest);

            // Then
            actualReIdentificationRequest.Should().BeEquivalentTo(expectedReIdentificationRequest);

            reIdentificationServiceMock.Verify(service =>
                service.BulkProcessRequestsAsync(inputReIdentificationRequest, batchSize),
                    Times.Once());

            this.necsBrokerMock.VerifyNoOtherCalls();
            this.identifierBrokerMock.VerifyNoOtherCalls();
        }
    }
}
