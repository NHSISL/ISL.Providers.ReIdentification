// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.Configuration;

namespace ISL.NotificationClient.Tests.Acceptance
{
    public partial class NecsProviderTests
    {
        [Fact]
        public async Task ShouldSendEmailAsync()
        {
            // given
            string toEmail = GetRandomEmail();
            string subject = GetRandomString();
            string body = GetRandomString();
            string templateId = configuration.GetValue<string>("notifyConfigurations:templateId");
            Dictionary<string, dynamic> personalisation = new Dictionary<string, dynamic>();
            personalisation.Add("templateId", templateId);

            // when
            string identifier = await this.govukNotifyProvider.SendEmailAsync(
                toEmail,
                subject,
                body,
                personalisation);

            // then
            identifier.Should().NotBeNullOrWhiteSpace();
        }
    }
}
