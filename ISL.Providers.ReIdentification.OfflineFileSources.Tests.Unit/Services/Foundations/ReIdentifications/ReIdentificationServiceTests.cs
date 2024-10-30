// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Force.DeepCloner;
using ISL.Providers.ReIdentification.Abstractions.Models;
using ISL.Providers.ReIdentification.Necs.Services.Foundations.ReIdentifications;
using ISL.Providers.ReIdentification.OfflineFileSources.Brokers.OfflineSources;
using ISL.Providers.ReIdentification.OfflineFileSources.Models;
using ISL.Providers.ReIdentification.OfflineFileSources.Services.Foundations.ReIdentifications;
using ISL.ReIdentification.Core.Models.Brokers.NECS.Requests;
using KellermanSoftware.CompareNetObjects;
using Moq;
using Tynamix.ObjectFiller;

namespace ISL.Providers.ReIdentification.Necs.Tests.Unit.Services.Foundations.Notifications
{
    public partial class ReIdentificationServiceTests
    {
        private readonly Mock<IOfflineSourceBroker> offlineSourceBrokerMock;
        private readonly OfflineSourceConfiguration offlineSourceConfiguration;
        private readonly IReIdentificationService reIdentificationService;
        private readonly ICompareLogic compareLogic;

        public ReIdentificationServiceTests()
        {
            this.offlineSourceBrokerMock = new Mock<IOfflineSourceBroker>();
            this.offlineSourceConfiguration = GetRandomConfigurations();
            this.compareLogic = new CompareLogic();
            this.reIdentificationService = new ReIdentificationService(
                offlineSourceBroker: offlineSourceBrokerMock.Object,
                offlineSourceConfiguration: this.offlineSourceConfiguration);
        }

        private static string GetRandomString() =>
            new MnemonicString().GetValue();

        private static string GetRandomString(int length)
        {
            string result = new MnemonicString(wordCount: 1, wordMinLength: length, wordMaxLength: length).GetValue();

            return result.Length > length ? result.Substring(0, length) : result;
        }

        private static int GetRandomNumber() =>
            new IntRange(max: 15, min: 2).GetValue();

        private static string GetRandomStringWithLengthOf(int length)
        {
            string result = new MnemonicString(wordCount: 1, wordMinLength: length, wordMaxLength: length).GetValue();

            return result.Length > length ? result.Substring(0, length) : result;
        }

        private static string GenerateRandom10DigitNumber()
        {
            Random random = new Random();
            var randomNumber = random.Next(1000000000, 2000000000).ToString();

            return randomNumber;
        }

        private static OfflineSourceConfiguration GetRandomConfigurations() =>
            CreateConfigurationsFiller().Create();

        private static Filler<OfflineSourceConfiguration> CreateConfigurationsFiller()
        {
            var filler = new Filler<OfflineSourceConfiguration>();
            filler.Setup();

            return filler;
        }

        private Expression<Func<Dictionary<string, dynamic>, bool>> SameDictionaryAs(
            Dictionary<string, dynamic> expectedDictionary) =>
            actualDictionary => this.compareLogic.Compare(expectedDictionary, actualDictionary).AreEqual;

        private static ReIdentificationRequest CreateRandomReIdentificationResponse(
            ReIdentificationRequest request, List<IdentificationPair> identificationPairs)
        {
            var response = request.DeepClone();

            foreach (var item in response.ReIdentificationItems)
            {
                item.Identifier = identificationPairs
                    .First(identifier => identifier.PseudoNumber == item.Identifier).NhsNumber;

                item.Message = "OK";
            }

            return response;
        }

        private static ReIdentificationRequest CreateRandomReIdentificationRequest()
        {
            int randomCount = GetRandomNumber();
            List<IdentificationPair> randomIdentificationPairs = CreateRandomIdentificationPairs(randomCount);

            return CreateRandomReIdentificationRequest(randomIdentificationPairs);
        }

        private static ReIdentificationRequest CreateRandomReIdentificationRequest(
            List<IdentificationPair> identificationPairs) =>
            CreateReIdentificationRequestFiller(identificationPairs).Create();

        private static Filler<ReIdentificationRequest> CreateReIdentificationRequestFiller(
            List<IdentificationPair> identificationPairs)
        {
            var filler = new Filler<ReIdentificationRequest>();
            List<string> pseudoNumbers = identificationPairs.Select(pair => pair.PseudoNumber).ToList();

            filler.Setup()
                .OnProperty(request => request.ReIdentificationItems)
                    .Use(CreateRandomReIdentificationItems(pseudoNumbers));

            return filler;
        }

        private static List<ReIdentificationItem> CreateRandomReIdentificationItems(List<string> identifiers)
        {
            List<ReIdentificationItem> reIdentificationItems = new List<ReIdentificationItem>();

            foreach (var item in identifiers)
            {
                reIdentificationItems.Add(new ReIdentificationItem
                {
                    RowNumber = GetRandomString(),
                    Identifier = item
                });
            }

            return reIdentificationItems;
        }

        private static Filler<ReIdentificationItem> CreateIdentificationItemFiller()
        {
            var filler = new Filler<ReIdentificationItem>();
            filler.Setup().OnProperty(address => address.Identifier).Use(GetRandomStringWithLengthOf(10));

            return filler;
        }

        private static List<IdentificationPair> CreateRandomIdentificationPairs(int count) =>
            CreateIdentificationPair().Create(count).ToList();

        private static Filler<IdentificationPair> CreateIdentificationPair()
        {
            var filler = new Filler<IdentificationPair>();

            filler.Setup()
                .OnProperty(identifierPair => identifierPair.PseudoNumber).Use(GenerateRandom10DigitNumber())
                .OnProperty(identifierPair => identifierPair.NhsNumber).Use(GenerateRandom10DigitNumber());

            return filler;
        }
    }
}
