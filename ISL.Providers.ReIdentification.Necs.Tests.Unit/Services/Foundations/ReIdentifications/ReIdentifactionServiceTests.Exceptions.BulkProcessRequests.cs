// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Threading.Tasks;
using FluentAssertions;
using ISL.Providers.ReIdentification.Abstractions.Models;
using ISL.Providers.ReIdentification.Necs.Models.Foundations.ReIdentifications.Exceptions;
using ISL.Providers.ReIdentification.Necs.Services.Foundations.ReIdentifications;
using Moq;
using Xeptions;

namespace ISL.Providers.ReIdentification.Necs.Tests.Unit.Services.Foundations.ReIdentifications
{
    public partial class ReIdentificationServiceTests
    {
        [Theory]
        [MemberData(nameof(DependencyValidationExceptions))]
        public async Task ShouldThrowAggregateExceptionForDependencyValidationErrorsOnBulkProcessIfRequestsFailsAsync(
            Xeption dependancyValidationException)
        {
            // Given
            Guid randomIdentifier = Guid.NewGuid();
            int batchSize = 1;
            int randomCount = 1;

            ReIdentificationRequest randomReIdentificationRequest =
                CreateRandomReIdentificationRequest(count: randomCount);

            ReIdentificationRequest inputReIdentificationRequest = randomReIdentificationRequest;

            Mock<ReIdentificationService> reIdentificationServiceMock =
                new Mock<ReIdentificationService>(
                    this.necsBrokerMock.Object,
                    this.identifierBrokerMock.Object,
                    this.necsReIdentificationConfigurations)
                { CallBase = true };

            ReIdentificationService service = reIdentificationServiceMock.Object;

            this.identifierBrokerMock.Setup(broker =>
                broker.GetIdentifierAsync())
                    .ThrowsAsync(dependancyValidationException);

            var serverException =
                new Exception(message: GetRandomString());

            var failedClientReIdentificationException = new FailedClientReIdentificationException(
                message: "Failed NECS client error occurred, please contact support.",
                innerException: dependancyValidationException,
                data: dependancyValidationException.Data);

            var reIdentificationDependencyValidationException = new ReIdentificationDependencyValidationException(
                message: "Re-identification dependency validation error occurred, fix errors and try again.",
                innerException: failedClientReIdentificationException);

            var expectedAggregateException = new AggregateException(
                message: $"Unable to process items in 1 of the batch(es) from {inputReIdentificationRequest.RequestId}",
                innerExceptions: reIdentificationDependencyValidationException);

            // When
            ValueTask<ReIdentificationRequest> reReIdentificationRequestTask = service
                .BulkProcessRequestsAsync(inputReIdentificationRequest, batchSize);

            AggregateException actualAggregateException =
                await Assert.ThrowsAsync<AggregateException>(
                    testCode: reReIdentificationRequestTask.AsTask);

            // Then
            actualAggregateException.Should().BeEquivalentTo(expectedAggregateException);

            this.identifierBrokerMock.Verify(broker =>
                broker.GetIdentifierAsync(),
                    Times.Once);

            this.necsBrokerMock.VerifyNoOtherCalls();
            this.identifierBrokerMock.VerifyNoOtherCalls();
        }

        [Theory]
        [MemberData(nameof(DependencyExceptions))]
        public async Task ShouldThrowAggregateExceptionForDependencyExceptionsOnBulkProcessIfRequestsFailsAsync(
            Xeption dependancyValidationException)
        {
            // Given
            Guid randomIdentifier = Guid.NewGuid();
            int batchSize = 1;
            int randomCount = 1;

            ReIdentificationRequest randomReIdentificationRequest =
                CreateRandomReIdentificationRequest(count: randomCount);

            ReIdentificationRequest inputReIdentificationRequest = randomReIdentificationRequest;

            Mock<ReIdentificationService> reIdentificationServiceMock =
                new Mock<ReIdentificationService>(
                    this.necsBrokerMock.Object,
                    this.identifierBrokerMock.Object,
                    this.necsReIdentificationConfigurations)
                { CallBase = true };

            ReIdentificationService service = reIdentificationServiceMock.Object;

            this.identifierBrokerMock.Setup(broker =>
                broker.GetIdentifierAsync())
                    .ThrowsAsync(dependancyValidationException);

            var serverException =
                new Exception(message: GetRandomString());

            var failedServerReIdentificationException = new FailedServerReIdentificationException(
                message: "Failed NECS server error occurred, please contact support.",
                innerException: dependancyValidationException,
                data: dependancyValidationException.Data);

            var reIdentificationDependencyException = new ReIdentificationDependencyException(
                message: "Re-identification dependency error occurred, contact support.",
                innerException: failedServerReIdentificationException);

            var expectedAggregateException = new AggregateException(
                message: $"Unable to process addresses in 1 of the batch(es) from " +
                    $"{inputReIdentificationRequest.RequestId}",
                innerExceptions: reIdentificationDependencyException);

            // When
            ValueTask<ReIdentificationRequest> reReIdentificationRequestTask = service
                .BulkProcessRequestsAsync(inputReIdentificationRequest, batchSize);

            AggregateException actualAggregateException =
                await Assert.ThrowsAsync<AggregateException>(
                    testCode: reReIdentificationRequestTask.AsTask);

            // Then
            actualAggregateException.Should().BeEquivalentTo(expectedAggregateException);

            this.identifierBrokerMock.Verify(broker =>
                broker.GetIdentifierAsync(),
                    Times.Once);

            this.necsBrokerMock.VerifyNoOtherCalls();
            this.identifierBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowAggregateExceptionForServiceExceptionsOnBulkProcessIfRequestsFailsAsync()
        {
            // Given
            Guid randomIdentifier = Guid.NewGuid();
            int batchSize = 1;
            int randomCount = 1;

            ReIdentificationRequest randomReIdentificationRequest =
                CreateRandomReIdentificationRequest(count: randomCount);

            ReIdentificationRequest inputReIdentificationRequest = randomReIdentificationRequest;

            Mock<ReIdentificationService> reIdentificationServiceMock =
                new Mock<ReIdentificationService>(
                    this.necsBrokerMock.Object,
                    this.identifierBrokerMock.Object,
                    this.necsReIdentificationConfigurations)
                { CallBase = true };

            ReIdentificationService service = reIdentificationServiceMock.Object;
            var serviceException = new Exception();

            this.identifierBrokerMock.Setup(broker =>
                broker.GetIdentifierAsync())
                    .ThrowsAsync(serviceException);

            var serverException =
                new Exception(message: GetRandomString());

            var failedServiceReIdentificationRequestException =
                new FailedServiceReIdentificationException(
                    message: "Failed re-identification service error occurred, please contact support.",
                    innerException: serviceException,
                    data: serviceException.Data);

            var reIdentificationServiceException = new ReIdentificationServiceException(
                message: "Re-identification service error occurred, please contact support.",
                innerException: failedServiceReIdentificationRequestException);

            var expectedAggregateException = new AggregateException(
                message: $"Unable to process items in 1 of the batch(es) from " +
                    $"{inputReIdentificationRequest.RequestId}",
                innerExceptions: reIdentificationServiceException);

            // When
            ValueTask<ReIdentificationRequest> reReIdentificationRequestTask = service
                .BulkProcessRequestsAsync(inputReIdentificationRequest, batchSize);

            AggregateException actualAggregateException =
                await Assert.ThrowsAsync<AggregateException>(
                    testCode: reReIdentificationRequestTask.AsTask);

            // Then
            actualAggregateException.Should().BeEquivalentTo(expectedAggregateException);

            this.identifierBrokerMock.Verify(broker =>
                broker.GetIdentifierAsync(),
                    Times.Once);

            this.necsBrokerMock.VerifyNoOtherCalls();
            this.identifierBrokerMock.VerifyNoOtherCalls();
        }
    }
}
