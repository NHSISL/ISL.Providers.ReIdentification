// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Force.DeepCloner;
using ISL.Providers.ReIdentification.Abstractions.Models;
using ISL.Providers.ReIdentification.OfflineFileSources.Models;
using Moq;

namespace ISL.Providers.ReIdentification.Necs.Tests.Unit.Services.Foundations.Notifications
{
    public partial class ReIdentificationServiceTests
    {
        [Fact]
        public async Task ShouldProcessReIdentificationRequest()
        {
            // given
            int randomCount = GetRandomNumber();
            List<IdentificationPair> randomIdentificationPairs = CreateRandomIdentificationPairs(randomCount);
            List<string> pseudoNumbers = randomIdentificationPairs.Select(pair => pair.PseudoNumber).ToList();
            List<string> nhsNumbers = randomIdentificationPairs.Select(pair => pair.NhsNumber).ToList();

            ReIdentificationRequest randomReIdentificationRequest =
                CreateRandomReIdentificationRequest(randomIdentificationPairs);

            ReIdentificationRequest randomReIdentificationResponse =
                CreateRandomReIdentificationResponse(randomReIdentificationRequest, randomIdentificationPairs);

            ReIdentificationRequest input = randomReIdentificationRequest.DeepClone();
            ReIdentificationRequest output = randomReIdentificationResponse.DeepClone();
            ReIdentificationRequest expectedResponse = output.DeepClone();

            this.offlineSourceBrokerMock.Setup(broker =>
                broker.GetIdentificationPairsAsync())
                    .ReturnsAsync(randomIdentificationPairs);

            // when
            ReIdentificationRequest actualResponse = await this.reIdentificationService
                .ProcessReIdentificationRequest(reIdentificationRequest: input);

            // then
            actualResponse.Should().BeEquivalentTo(expectedResponse);

            this.offlineSourceBrokerMock.Verify(broker =>
                broker.GetIdentificationPairsAsync(),
                    Times.Exactly(input.ReIdentificationItems.Count));

            this.offlineSourceBrokerMock.VerifyNoOtherCalls();
        }
    }
}
