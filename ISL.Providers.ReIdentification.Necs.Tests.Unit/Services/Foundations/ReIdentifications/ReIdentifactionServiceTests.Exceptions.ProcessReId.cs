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

namespace ISL.Providers.ReIdentification.Necs.Tests.Unit.Services.Foundations.ReIdentifications
{
    public partial class ReIdentificationServiceTests
    {
        [Fact]
        public async Task ShouldThrowAggregateServiceExceptionOnProcessIfAggregateOccurredAndLogItAsync()
        {
            // given
            int randomCount = GetRandomNumber();
            var aggregateException = new AggregateException();

            ReIdentificationRequest someIdentificationRequest =
                CreateRandomReIdentificationRequest(count: randomCount);

            var failedServiceReIdentificationException =
                new FailedServiceReIdentificationException(
                    message: "Failed re-identification aggregate service error occurred, please contact support.",
                    innerException: aggregateException,
                    data: aggregateException.Data);

            var expectedReIdentificationServiceException =
                new ReIdentificationServiceException(
                    message: "Re-identification service error occurred, please contact support.",
                    innerException: failedServiceReIdentificationException);

            Mock<ReIdentificationService> reIdentificationServiceMock =
                new Mock<ReIdentificationService>(
                    this.necsBrokerMock.Object,
                    this.identifierBrokerMock.Object,
                    this.necsReIdentificationConfigurations)
                { CallBase = true };

            reIdentificationServiceMock.Setup(service =>
                service.BulkProcessRequestsAsync(It.IsAny<ReIdentificationRequest>(), It.IsAny<int>()))
                    .ThrowsAsync(aggregateException);

            IReIdentificationService service = reIdentificationServiceMock.Object;

            // when
            ValueTask<ReIdentificationRequest> processIdentificationRequestTask =
                service.ProcessReIdentificationRequest(someIdentificationRequest);

            ReIdentificationServiceException actualReIdentificationServiceException =
                await Assert.ThrowsAsync<ReIdentificationServiceException>(
                    testCode: processIdentificationRequestTask.AsTask);

            // then
            actualReIdentificationServiceException.Should().BeEquivalentTo(
                expectedReIdentificationServiceException);

            reIdentificationServiceMock.Verify(service =>
                service.BulkProcessRequestsAsync(It.IsAny<ReIdentificationRequest>(), It.IsAny<int>()),
                    Times.Once);

            this.necsBrokerMock.VerifyNoOtherCalls();
            this.identifierBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowServiceExceptionOnProcessIfServiceErrorOccurredAndLogItAsync()
        {
            // given
            int randomCount = GetRandomNumber();
            var serviceException = new Exception();

            ReIdentificationRequest someIdentificationRequest =
                CreateRandomReIdentificationRequest(count: randomCount);

            var failedServiceReIdentificationException =
                new FailedServiceReIdentificationException(
                    message: "Failed re-identification service error occurred, please contact support.",
                    innerException: serviceException,
                    data: serviceException.Data);

            var expectedReIdentificationServiceException =
                new ReIdentificationServiceException(
                    message: "Re-identification service error occurred, please contact support.",
                    innerException: failedServiceReIdentificationException);

            Mock<ReIdentificationService> reIdentificationServiceMock =
                new Mock<ReIdentificationService>(
                    this.necsBrokerMock.Object,
                    this.identifierBrokerMock.Object,
                    this.necsReIdentificationConfigurations)
                { CallBase = true };

            reIdentificationServiceMock.Setup(service =>
                service.BulkProcessRequestsAsync(It.IsAny<ReIdentificationRequest>(), It.IsAny<int>()))
                    .ThrowsAsync(serviceException);

            IReIdentificationService service = reIdentificationServiceMock.Object;

            // when
            ValueTask<ReIdentificationRequest> processIdentificationRequestTask =
                service.ProcessReIdentificationRequest(someIdentificationRequest);

            ReIdentificationServiceException actualReIdentificationServiceException =
                await Assert.ThrowsAsync<ReIdentificationServiceException>(
                    testCode: processIdentificationRequestTask.AsTask);

            // then
            actualReIdentificationServiceException.Should().BeEquivalentTo(
                expectedReIdentificationServiceException);

            reIdentificationServiceMock.Verify(service =>
                service.BulkProcessRequestsAsync(It.IsAny<ReIdentificationRequest>(), It.IsAny<int>()),
                    Times.Once);

            this.necsBrokerMock.VerifyNoOtherCalls();
            this.identifierBrokerMock.VerifyNoOtherCalls();
        }
    }
}
