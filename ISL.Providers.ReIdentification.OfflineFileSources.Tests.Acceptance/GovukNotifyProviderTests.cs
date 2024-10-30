// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using ISL.Providers.ReIdentification.Necs.Models;
using ISL.Providers.ReIdentification.Necs.Providers.Notifications;
using Microsoft.Extensions.Configuration;
using Tynamix.ObjectFiller;

namespace ISL.NotificationClient.Tests.Acceptance
{
    public partial class NecsProviderTests
    {
        private readonly INecsProvider govukNotifyProvider;
        private readonly IConfiguration configuration;

        public NecsProviderTests()
        {
            var configurationBuilder = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile("appsettings.Development.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables();

            configuration = configurationBuilder.Build();

            NotifyConfigurations notifyConfigurations = configuration
                .GetSection("notifyConfigurations").Get<NotifyConfigurations>();

            this.govukNotifyProvider = new NecsProvider(notifyConfigurations);
        }

        private static string GetRandomEmail() =>
            new EmailAddresses().GetValue();

        private static string GetRandomString() =>
            new MnemonicString(wordCount: GetRandomNumber()).GetValue();

        private static int GetRandomNumber() =>
            new IntRange(min: 2, max: 10).GetValue();
    }
}