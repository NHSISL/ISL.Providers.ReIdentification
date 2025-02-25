// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System.Threading.Tasks;
using FluentAssertions;
using ISL.NecsApi.Tests.Integrations.Models.NECS.Requests;
using ISL.NecsApi.Tests.Integrations.Models.NECS.Responses;

namespace ISL.NecsApi.Tests.Integrations
{
    public partial class NecsProviderTests
    {
        [Fact]
        public async Task ShouldReIdentifyAsync()
        {
            // Given
            int randomCount = GetRandomNumber();

            NecsReIdentificationRequest randomReIdentificationRequest =
                CreateRandomNecsReIdentificationRequest(count: randomCount);

            // When
            var result =
                await apiClient.PostContentAsync<NecsReIdentificationRequest, NecsReIdentificationResponse>
                    (necsConfiguration.ApiUrl, randomReIdentificationRequest);

            // Then
            result.Should().NotBeNull();
            result.ProcessedCount.Should().Be(randomCount);
            output.WriteLine($"ElapsedTime: {result.ElapsedTime}");
        }
    }
}
