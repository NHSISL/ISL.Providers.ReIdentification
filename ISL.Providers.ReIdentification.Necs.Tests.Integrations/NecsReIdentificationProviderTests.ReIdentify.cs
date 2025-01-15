// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using Force.DeepCloner;
using ISL.Providers.ReIdentification.Abstractions.Models;
<<<<<<< HEAD
using ISL.ReIdentification.Core.Models.Brokers.NECS.Requests;
=======
>>>>>>> CODE RUB: Fixed merge issues

namespace ISL.Providers.ReIdentification.Necs.Tests.Integrations
{
    public partial class NecsProviderTests
    {
        [Fact]
        public async Task ShouldReIdentifyAsync()
        {
            // given
            ReIdentificationRequest inputRequest = new ReIdentificationRequest
            {
                Organisation = GetRandomString(),
                Reason = GetRandomString(),
                RequestId = Guid.NewGuid(),
                UserIdentifier = GetRandomString(),
                ReIdentificationItems = new List<ReIdentificationItem> {
                    new ReIdentificationItem {
                        RowNumber = "1",
                        Identifier = "1111112000",
                    }
                }
            };

            ReIdentificationRequest expectedResponse = inputRequest.DeepClone();
            expectedResponse.ReIdentificationItems[0].Identifier = "DEC1111112";
            expectedResponse.ReIdentificationItems[0].Message = "OK";

            // when
            ReIdentificationRequest actualResponse =
                await this.necsReIdentificationProvider.ReIdentifyAsync(reIdentificationRequest: inputRequest);

            // then
            actualResponse.Should().BeEquivalentTo(expectedResponse);
        }
    }
}
