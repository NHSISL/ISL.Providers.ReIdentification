// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System.Threading.Tasks;
using FluentAssertions;
using Force.DeepCloner;
using ISL.Providers.ReIdentification.Abstractions.Models;

namespace ISL.Providers.ReIdentification.DemoData.Tests.Unit.Services.Foundations.Notifications
{
    public partial class ReIdentificationServiceTests
    {
        [Fact]
        public async Task ShouldProcessReIdentificationRequest()
        {
            // given
            int randomCount = GetRandomNumber();

            ReIdentificationRequest randomReIdentificationRequest =
                CreateRandomReIdentificationRequest(count: randomCount);

            ReIdentificationRequest randomReIdentificationResponse =
                CreateRandomReIdentificationResponse(
                    request: randomReIdentificationRequest,
                    prefix: demoDataReIdentificationConfigurations.DemoPrefix);

            ReIdentificationRequest input = randomReIdentificationRequest.DeepClone();
            ReIdentificationRequest output = randomReIdentificationResponse.DeepClone();
            ReIdentificationRequest expectedResponse = output.DeepClone();

            // when
            ReIdentificationRequest actualResponse = await this.reIdentificationService
                .ProcessReIdentificationRequest(reIdentificationRequest: input);

            // then
            actualResponse.Should().BeEquivalentTo(expectedResponse);
        }
    }
}
