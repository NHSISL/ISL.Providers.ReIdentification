// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System.Threading.Tasks;
using FluentAssertions;
using ISL.Providers.ReIdentification.Abstractions.Models;
using ISL.Providers.ReIdentification.Abstractions.Models.Exceptions;
using ISL.Providers.ReIdentification.Abstractions.Tests.Unit.Models.Exceptions;
using Moq;
using Xeptions;

namespace ISL.Providers.ReIdentification.Abstractions.Tests.Unit
{
    public partial class ReIdentificationAbstractionTests
    {
        [Fact]
        public async Task ShouldThrowValidationExceptionWhenTypeINotificationValidationException()
        {
            // given
            var someException = new Xeption();

            var someNotificationValidationException =
                new SomeReIdentificationValidationException(
                    message: "Some re-identification validation exception occurred",
                    innerException: someException,
                    data: someException.Data);

            ReIdentificationProviderValidationException expectedNotificationValidationProviderException =
                new ReIdentificationProviderValidationException(
                    message: "Re-identification validation errors occurred, please try again.",
                    innerException: someNotificationValidationException);

            this.reIdentificationMock.Setup(provider =>
                provider.ReIdentifyAsync(It.IsAny<ReIdentificationRequest>()))
                    .ThrowsAsync(someNotificationValidationException);

            // when
            ValueTask<ReIdentificationRequest> reIdentifyTask =
                this.notificationAbstractionProvider
                    .ReIdentifyAsync(It.IsAny<ReIdentificationRequest>());

            ReIdentificationProviderValidationException actualNotificationValidationProviderException =
                await Assert.ThrowsAsync<ReIdentificationProviderValidationException>(testCode: reIdentifyTask.AsTask);

            // then
            actualNotificationValidationProviderException.Should().BeEquivalentTo(
                expectedNotificationValidationProviderException);

            this.reIdentificationMock.Verify(provider =>
                provider.ReIdentifyAsync(It.IsAny<ReIdentificationRequest>()),
                    Times.Once);

            this.reIdentificationMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowDependencyValidationExceptionWhenTypeINotificationDependencyValidationException()
        {
            // given
            var someException = new Xeption();

            var someNotificationValidationException =
                new SomeNotificationDependencyValidationException(
                    message: "Some re-identification dependency validation exception occurred",
                    innerException: someException,
                    data: someException.Data);

            ReIdentificationProviderValidationException expectedNotificationValidationProviderException =
                new ReIdentificationProviderValidationException(
                    message: "Re-identification validation errors occurred, please try again.",
                    innerException: someNotificationValidationException);

            this.reIdentificationMock.Setup(provider =>
                provider.ReIdentifyAsync(It.IsAny<ReIdentificationRequest>()))
                    .ThrowsAsync(someNotificationValidationException);

            // when
            ValueTask<ReIdentificationRequest> reIdentifyTask =
                this.notificationAbstractionProvider
                    .ReIdentifyAsync(It.IsAny<ReIdentificationRequest>());

            ReIdentificationProviderValidationException actualNotificationValidationProviderException =
                await Assert.ThrowsAsync<ReIdentificationProviderValidationException>(testCode: reIdentifyTask.AsTask);

            // then
            actualNotificationValidationProviderException.Should().BeEquivalentTo(
                expectedNotificationValidationProviderException);

            this.reIdentificationMock.Verify(provider =>
                provider.ReIdentifyAsync(It.IsAny<ReIdentificationRequest>()),
                    Times.Once);

            this.reIdentificationMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowDependencyExceptionWhenTypeINotificationDependencyException()
        {
            // given
            var someException = new Xeption();

            var someNotificationValidationException =
                new SomeNotificationDependencyException(
                    message: "Some re-identification dependency exception occurred",
                    innerException: someException);

            ReIdentificationProviderDependencyException expectedNotificationDependencyProviderException =
                new ReIdentificationProviderDependencyException(
                    message: "Re-identification dependency error occurred, contact support.",
                    innerException: someNotificationValidationException);

            this.reIdentificationMock.Setup(provider =>
                provider.ReIdentifyAsync(It.IsAny<ReIdentificationRequest>()))
                    .ThrowsAsync(someNotificationValidationException);

            // when
            ValueTask<ReIdentificationRequest> reIdentifyTask =
                this.notificationAbstractionProvider
                    .ReIdentifyAsync(It.IsAny<ReIdentificationRequest>());

            ReIdentificationProviderDependencyException actualNotificationDependencyProviderException =
                await Assert.ThrowsAsync<ReIdentificationProviderDependencyException>(testCode: reIdentifyTask.AsTask);

            // then
            actualNotificationDependencyProviderException.Should().BeEquivalentTo(
                expectedNotificationDependencyProviderException);

            this.reIdentificationMock.Verify(provider =>
                provider.ReIdentifyAsync(It.IsAny<ReIdentificationRequest>()),
                    Times.Once);

            this.reIdentificationMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowServiceExceptionWhenTypeINotificationServiceException()
        {
            // given
            var someException = new Xeption();

            var someNotificationValidationException =
                new SomeReIdentificationServiceException(
                    message: "Some re-identification service exception occurred",
                    innerException: someException);

            ReIdentificationProviderServiceException expectedNotificationServiceProviderException =
                new ReIdentificationProviderServiceException(
                    message: "Re-identification service error occurred, contact support.",
                    innerException: someNotificationValidationException);

            this.reIdentificationMock.Setup(provider =>
                provider.ReIdentifyAsync(It.IsAny<ReIdentificationRequest>()))
                    .ThrowsAsync(someNotificationValidationException);

            // when
            ValueTask<ReIdentificationRequest> reIdentifyTask =
                this.notificationAbstractionProvider
                    .ReIdentifyAsync(It.IsAny<ReIdentificationRequest>());

            ReIdentificationProviderServiceException actualNotificationServiceProviderException =
                await Assert.ThrowsAsync<ReIdentificationProviderServiceException>(testCode: reIdentifyTask.AsTask);

            // then
            actualNotificationServiceProviderException.Should().BeEquivalentTo(
                expectedNotificationServiceProviderException);

            this.reIdentificationMock.Verify(provider =>
                provider.ReIdentifyAsync(It.IsAny<ReIdentificationRequest>()),
                    Times.Once);

            this.reIdentificationMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowUncatagorizedServiceExceptionWhenTypeIsNotExpected()
        {
            // given
            var someException = new Xeption();

            var uncatagorizedNotificationProviderException =
                new UncatagorizedReIdentificationProviderException(
                    message: "Re-identification provider not properly implemented. Uncatagorized errors found, " +
                            "contact the notification provider owner for support.",
                    innerException: someException,
                    data: someException.Data);

            ReIdentificationProviderServiceException expectedNotificationServiceProviderException =
                new ReIdentificationProviderServiceException(
                    message: "Uncatagorized re-identification service error occurred, contact support.",
                    innerException: uncatagorizedNotificationProviderException);

            this.reIdentificationMock.Setup(provider =>
                provider.ReIdentifyAsync(It.IsAny<ReIdentificationRequest>()))
                    .ThrowsAsync(someException);

            // when
            ValueTask<ReIdentificationRequest> reIdentifyTask =
                this.notificationAbstractionProvider
                    .ReIdentifyAsync(It.IsAny<ReIdentificationRequest>());

            ReIdentificationProviderServiceException actualNotificationServiceProviderException =
                await Assert.ThrowsAsync<ReIdentificationProviderServiceException>(testCode: reIdentifyTask.AsTask);

            // then
            actualNotificationServiceProviderException.Should().BeEquivalentTo(
                expectedNotificationServiceProviderException);

            this.reIdentificationMock.Verify(provider =>
                provider.ReIdentifyAsync(It.IsAny<ReIdentificationRequest>()),
                    Times.Once);

            this.reIdentificationMock.VerifyNoOtherCalls();
        }
    }
}
