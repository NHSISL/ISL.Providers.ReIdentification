// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using ISL.Providers.ReIdentification.Abstractions.Models;
using ISL.Providers.ReIdentification.OfflineFileSources.Models.Foundations.ReIdentifications.Exceptions;
using ISL.ReIdentification.Core.Models.Brokers.NECS.Requests;
using Moq;

namespace ISL.Providers.ReIdentification.Necs.Tests.Unit.Services.Foundations.Notifications
{
    public partial class ReIdentificationServiceTests
    {
        [Fact]
        public async Task ShouldThrowValidationExceptionOnProcessReIdentificationRequestAsync()
        {
            // given
            ReIdentificationRequest nullReIdentificationRequest = null;

            var nullReIdentificationRequestException =
                new NullReIdentificationRequestException(message: "Re-identification request is null.");

            var expectedReIdentificationRequestValidationException =
                new ReIdentificationRequestValidationException(
                    message: "Re-identification validation error occurred, please fix errors and try again.",
                    innerException: nullReIdentificationRequestException);

            // when
            ValueTask<ReIdentificationRequest> processdentificationRequestTask =
                this.reIdentificationService.ProcessReIdentificationRequest(nullReIdentificationRequest);

            ReIdentificationRequestValidationException actualReIdentificationRequestValidationException =
                await Assert.ThrowsAsync<ReIdentificationRequestValidationException>(
                    testCode: processdentificationRequestTask.AsTask);

            // then
            actualReIdentificationRequestValidationException.Should()
                .BeEquivalentTo(expectedReIdentificationRequestValidationException);

            this.offlineSourceBrokerMock.Verify(broker =>
                broker.GetIdentificationPairsAsync(),
                    Times.Never);

            this.offlineSourceBrokerMock.VerifyNoOtherCalls();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public async Task ShouldThrowValidationExceptionOnProcessIfReIdentificationRequestIsInvalidAndLogItAsync(
            string invalidText)
        {
            // given
            var invalidReIdentificationRequest = new ReIdentificationRequest
            {
                RequestId = Guid.Empty,
                UserIdentifier = invalidText,
                Organisation = invalidText,
                Reason = invalidText,
                ReIdentificationItems = new List<ReIdentificationItem>()
            };

            var invalidReIdentificationRequestException =
                new InvalidReIdentificationRequestException(
                    message: "Invalid re-identification request.  Please correct the errors and try again.");

            invalidReIdentificationRequestException.AddData(
                key: nameof(ReIdentificationRequest.RequestId),
                values: "Id is invalid");

            invalidReIdentificationRequestException.AddData(
                key: nameof(ReIdentificationRequest.UserIdentifier),
                values: "Text is invalid");

            invalidReIdentificationRequestException.AddData(
                key: nameof(ReIdentificationRequest.Organisation),
                values: "Text is invalid");

            invalidReIdentificationRequestException.AddData(
                key: nameof(ReIdentificationRequest.Reason),
                values: "Text is invalid");

            invalidReIdentificationRequestException.AddData(
                key: nameof(ReIdentificationRequest.ReIdentificationItems),
                values: $"Items is invalid");

            var expectedReIdentificationRequestValidationException =
                new ReIdentificationRequestValidationException(
                    message: "Re-identification validation error occurred, please fix errors and try again.",
                    innerException: invalidReIdentificationRequestException);

            // when
            ValueTask<ReIdentificationRequest> addReIdentificationRequestTask =
                this.reIdentificationService.ProcessReIdentificationRequest(invalidReIdentificationRequest);

            ReIdentificationRequestValidationException actualReIdentificationRequestValidationException =
                await Assert.ThrowsAsync<ReIdentificationRequestValidationException>(
                    testCode: addReIdentificationRequestTask.AsTask);

            // then
            actualReIdentificationRequestValidationException.Should()
                .BeEquivalentTo(expectedReIdentificationRequestValidationException);

            this.offlineSourceBrokerMock.Verify(broker =>
                broker.GetIdentificationPairsAsync(),
                    Times.Never);

            this.offlineSourceBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowValidationExceptionOnProcessIfDuplicateRowNumbersFoundAndLogItAsync()
        {
            // given
            ReIdentificationRequest randomReIdentificationRequest = CreateRandomReIdentificationRequest();

            randomReIdentificationRequest.ReIdentificationItems
                .AddRange(randomReIdentificationRequest.ReIdentificationItems);

            var invalidReIdentificationRequestException =
                new InvalidReIdentificationRequestException(
                    message: "Invalid re-identification request.  Please correct the errors and try again.");

            invalidReIdentificationRequestException.AddData(
                key: nameof(ReIdentificationRequest.ReIdentificationItems),
                values: $"Items is invalid.  There are duplicate RowNumbers");

            var expectedReIdentificationRequestValidationException =
                new ReIdentificationRequestValidationException(
                    message: "Re-identification validation error occurred, please fix errors and try again.",
                    innerException: invalidReIdentificationRequestException);

            // when
            ValueTask<ReIdentificationRequest> addReIdentificationRequestTask =
                this.reIdentificationService.ProcessReIdentificationRequest(randomReIdentificationRequest);

            ReIdentificationRequestValidationException actualReIdentificationRequestValidationException =
                await Assert.ThrowsAsync<ReIdentificationRequestValidationException>(
                    testCode: addReIdentificationRequestTask.AsTask);

            // then
            actualReIdentificationRequestValidationException.Should()
                .BeEquivalentTo(expectedReIdentificationRequestValidationException);

            this.offlineSourceBrokerMock.Verify(broker =>
                broker.GetIdentificationPairsAsync(),
                    Times.Never);

            this.offlineSourceBrokerMock.VerifyNoOtherCalls();
        }
    }
}
