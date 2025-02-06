// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Force.DeepCloner;
using ISL.Providers.ReIdentification.Abstractions.Models;
using ISL.Providers.ReIdentification.DemoData.Models;
using ISL.Providers.ReIdentification.DemoData.Services.Foundations.ReIdentifications;
using KellermanSoftware.CompareNetObjects;
using Tynamix.ObjectFiller;

namespace ISL.Providers.ReIdentification.DemoData.Tests.Unit.Services.Foundations.Notifications
{
    public partial class ReIdentificationServiceTests
    {
        private readonly DemoDataReIdentificationConfigurations demoDataReIdentificationConfigurations;
        private readonly IReIdentificationService reIdentificationService;
        private readonly ICompareLogic compareLogic;

        public ReIdentificationServiceTests()
        {
            this.demoDataReIdentificationConfigurations = GetRandomConfigurations();
            this.compareLogic = new CompareLogic();
            this.reIdentificationService = new ReIdentificationService(
                demoDataReIdentificationConfigurations: this.demoDataReIdentificationConfigurations);
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

        private static DemoDataReIdentificationConfigurations GetRandomConfigurations() =>
            CreateConfigurationsFiller().Create();

        private static Filler<DemoDataReIdentificationConfigurations> CreateConfigurationsFiller()
        {
            var filler = new Filler<DemoDataReIdentificationConfigurations>();
            filler.Setup()
                .OnProperty(config => config.DefaultIdentifier).Use("0000000000")
                .OnProperty(config => config.DemoPrefix).Use(GetRandomStringWithLengthOf(length: 3).ToUpper());

            return filler;
        }

        private Expression<Func<Dictionary<string, dynamic>, bool>> SameDictionaryAs(
            Dictionary<string, dynamic> expectedDictionary) =>
            actualDictionary => this.compareLogic.Compare(expectedDictionary, actualDictionary).AreEqual;

        private static ReIdentificationRequest CreateRandomReIdentificationResponse(
            ReIdentificationRequest request, string prefix)
        {
            var response = request.DeepClone();

            foreach (var item in response.ReIdentificationItems)
            {
                string demoNhsNumber = prefix + item.Identifier.Substring(0, 7);
                item.Identifier = demoNhsNumber;
                item.Message = "OK";
            }

            return response;
        }

        private static ReIdentificationRequest CreateRandomReIdentificationRequest(int count) =>
            CreateReIdentificationRequestFiller(count).Create();

        private static Filler<ReIdentificationRequest> CreateReIdentificationRequestFiller(int count)
        {
            var filler = new Filler<ReIdentificationRequest>();

            List<string> pseudoNumbers = Enumerable.Range(1, count)
                .Select(i => $"{GenerateRandom10DigitNumber()}").ToList();

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
