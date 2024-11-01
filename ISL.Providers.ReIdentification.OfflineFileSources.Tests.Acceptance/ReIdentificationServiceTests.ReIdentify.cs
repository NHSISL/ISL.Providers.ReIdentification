// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System.Threading.Tasks;
using FluentAssertions;
using Force.DeepCloner;
using ISL.Providers.ReIdentification.Abstractions.Models;

namespace ISL.NotificationClient.Tests.Acceptance
{
    public partial class ReIdentificationServiceTests
    {
        [Fact]
        public async Task ShouldReIdentifyAsync()
        {
            // given
            ReIdentificationRequest randomRequest = CreateRandomReIdentificationRequest();
            ReIdentificationRequest reIdentificationRequest = randomRequest.DeepClone();
            ReIdentificationRequest randomResponse = CreateRandomReIdentificationResponse(randomRequest);
            ReIdentificationRequest expectedResponse = randomResponse.DeepClone();

            // when
            ReIdentificationRequest actualResponse =
                await this.offlineFileSourceReIdentificationProvider.ReIdentifyAsync(reIdentificationRequest);

            // then
            actualResponse.Should().BeEquivalentTo(expectedResponse);
        }
    }
}
