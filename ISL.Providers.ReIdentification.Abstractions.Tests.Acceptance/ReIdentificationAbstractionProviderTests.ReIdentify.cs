// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System.Threading.Tasks;
using FluentAssertions;
using Force.DeepCloner;
using ISL.Providers.ReIdentification.Abstractions.Models;
using Moq;
using Xunit;

namespace ISL.Providers.ReIdentification.Abstractions.Tests.Acceptance
{
    public partial class ReIdentificationAbstractionProviderTests
    {
        [Fact]
        public async Task ShouldReIdentifyAsync()
        {
            // given
            ReIdentificationRequest randomInputReIdentificationRequest = CreateRandomReIdentificationRequest();
            ReIdentificationRequest inputReIdentificationRequest = randomInputReIdentificationRequest.DeepClone();
            ReIdentificationRequest randomOutputReIdentificationRequest = CreateRandomReIdentificationRequest();
            ReIdentificationRequest outputReIdentificationRequest = randomOutputReIdentificationRequest.DeepClone();
            ReIdentificationRequest expectedReIdentificationRequest = outputReIdentificationRequest.DeepClone();

            this.reIdentificationProviderMock.Setup(provider =>
                provider.ReIdentifyAsync(inputReIdentificationRequest))
                    .ReturnsAsync(outputReIdentificationRequest);

            // when
            ReIdentificationRequest actualReIdentificationRequest =
                await this.reIdentificationAbstractionProvider.ReIdentifyAsync(inputReIdentificationRequest);

            // then
            actualReIdentificationRequest.Should().BeEquivalentTo(expectedReIdentificationRequest);
        }
    }
}
