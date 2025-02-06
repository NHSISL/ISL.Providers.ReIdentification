// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Threading.Tasks;
using FluentAssertions;
using ISL.Providers.ReIdentification.Abstractions.Models;
using ISL.Providers.ReIdentification.DemoData.Models.Foundations.ReIdentifications.Exceptions;
using ISL.Providers.ReIdentification.DemoData.Services.Foundations.ReIdentifications;
using Moq;

namespace ISL.Providers.ReIdentification.DemoData.Tests.Unit.Services.Foundations.Notifications
{
    public partial class ReIdentificationServiceTests
    {
        [Fact]
        public async Task ShouldThrowServiceExceptionOnProcessIfServiceErrorOccurredAndLogItAsync()
        {
            // given
            var mockReIdentificationService = new Mock<ReIdentificationService>(
                this.demoDataReIdentificationConfigurations)
            {
                CallBase = true
            };

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

            mockReIdentificationService.Setup(service =>
                service.ValidateIdentificationRequestOnProcess(It.IsAny<ReIdentificationRequest>()))
                    .Throws(serviceException);

            // when
            ValueTask<ReIdentificationRequest> processIdentificationRequestTask =
                mockReIdentificationService.Object.ProcessReIdentificationRequest(someIdentificationRequest);

            ReIdentificationServiceException actualReIdentificationServiceException =
                await Assert.ThrowsAsync<ReIdentificationServiceException>(
                    testCode: processIdentificationRequestTask.AsTask);

            // then
            actualReIdentificationServiceException.Should().BeEquivalentTo(
                expectedReIdentificationServiceException);

            mockReIdentificationService.Verify(service =>
                service.ValidateIdentificationRequestOnProcess(It.IsAny<ReIdentificationRequest>()),
                    Times.Once);
        }
    }
}
