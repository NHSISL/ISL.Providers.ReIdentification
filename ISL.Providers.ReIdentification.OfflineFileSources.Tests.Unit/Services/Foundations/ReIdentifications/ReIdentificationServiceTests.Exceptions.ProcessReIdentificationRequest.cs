// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Threading.Tasks;
using FluentAssertions;
using ISL.Providers.ReIdentification.Abstractions.Models;
using ISL.Providers.ReIdentification.OfflineFileSources.Models.Foundations.ReIdentifications.Exceptions;
using Moq;

namespace ISL.Providers.ReIdentification.OfflineFileSources.Tests.Unit.Services.Foundations.Notifications
{
    public partial class ReIdentificationServiceTests
    {
        [Fact]
        public async Task ShouldThrowServiceExceptionOnProcessIfServiceErrorOccurredAndLogItAsync()
        {
            // given
            int randomCount = GetRandomNumber();
            var serviceException = new Exception();
            ReIdentificationRequest someIdentificationRequest = CreateRandomReIdentificationRequest();

            var failedServiceReIdentificationException =
                new FailedServiceReIdentificationException(
                    message: "Failed re-identification service error occurred, please contact support.",
                    innerException: serviceException,
                    data: serviceException.Data);

            var expectedReIdentificationServiceException =
                new ReIdentificationServiceException(
                    message: "Re-identification service error occurred, please contact support.",
                    innerException: failedServiceReIdentificationException);

            offlineSourceBrokerMock.Setup(service =>
                service.GetIdentificationPairsAsync())
                    .ThrowsAsync(serviceException);

            // when
            ValueTask<ReIdentificationRequest> processIdentificationRequestTask =
                reIdentificationService.ProcessReIdentificationRequest(someIdentificationRequest);

            ReIdentificationServiceException actualReIdentificationServiceException =
                await Assert.ThrowsAsync<ReIdentificationServiceException>(
                    testCode: processIdentificationRequestTask.AsTask);

            // then
            actualReIdentificationServiceException.Should().BeEquivalentTo(
                expectedReIdentificationServiceException);

            offlineSourceBrokerMock.Verify(service =>
                service.GetIdentificationPairsAsync(),
                    Times.Once);

            this.offlineSourceBrokerMock.VerifyNoOtherCalls();
        }
    }
}
