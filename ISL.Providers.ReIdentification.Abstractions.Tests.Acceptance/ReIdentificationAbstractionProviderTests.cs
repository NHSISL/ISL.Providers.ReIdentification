// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using ISL.Providers.ReIdentification.Abstractions.Models;
using Microsoft.Extensions.Configuration;
using Moq;
using Tynamix.ObjectFiller;

namespace ISL.Providers.ReIdentification.Abstractions.Tests.Acceptance
{
    public partial class ReIdentificationAbstractionProviderTests
    {
        private readonly Mock<IReIdentificationProvider> reIdentificationProviderMock;
        private readonly IReIdentificationAbstractionProvider reIdentificationAbstractionProvider;
        private readonly IConfiguration configuration;

        public ReIdentificationAbstractionProviderTests()
        {
            var configurationBuilder = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile("appsettings.Development.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables();

            configuration = configurationBuilder.Build();
            reIdentificationProviderMock = new Mock<IReIdentificationProvider>();

            this.reIdentificationAbstractionProvider =
                new ReIdentificationAbstractionProvider(reIdentificationProviderMock.Object);
        }

        private static string GetRandomString()
        {
            int length = GetRandomNumber();

            return new MnemonicString(wordCount: 1, wordMinLength: length, wordMaxLength: length).GetValue();
        }

        private static int GetRandomNumber() =>
            new IntRange(min: 2, max: 10).GetValue();

        private static ReIdentificationRequest CreateRandomReIdentificationRequest() =>
            CreateRandomReIdentificationRequestFiller().Create();

        private static Filler<ReIdentificationRequest> CreateRandomReIdentificationRequestFiller()
        {
            var filler = new Filler<ReIdentificationRequest>();
            filler.Setup();

            return filler;
        }
    }
}