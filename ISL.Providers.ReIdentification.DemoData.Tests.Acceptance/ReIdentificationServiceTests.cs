// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using ISL.Providers.ReIdentification.Abstractions.Models;
using ISL.Providers.ReIdentification.DemoData.Models;
using ISL.Providers.ReIdentification.DemoData.Providers.DemoData;
using Microsoft.Extensions.Configuration;
using Tynamix.ObjectFiller;

namespace ISL.NotificationClient.Tests.Acceptance
{
    public partial class ReIdentificationServiceTests
    {
        private readonly IDemoDataReIdentificationProvider demoDataReIdentificationProvider;
        private readonly DemoDataReIdentificationConfigurations demoDataReIdentificationConfigurations;
        private readonly IConfiguration configuration;

        public ReIdentificationServiceTests()
        {
            var configurationBuilder = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile("appsettings.Development.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables();

            configuration = configurationBuilder.Build();

            demoDataReIdentificationConfigurations = configuration
                .GetSection("demoDataReIdentificationConfigurations")
                    .Get<DemoDataReIdentificationConfigurations>();

            this.demoDataReIdentificationProvider =
                new DemoDataReIdentificationProvider(demoDataReIdentificationConfigurations);
        }

        private static string GetRandomString() =>
            new MnemonicString(wordCount: GetRandomNumber()).GetValue();

        private static string GetRandomStringWithLengthOf(int length)
        {
            string result = new MnemonicString(wordCount: 1, wordMinLength: length, wordMaxLength: length).GetValue();

            return result.Length > length ? result.Substring(0, length) : result;
        }

        private static string GenerateRandomNumber(int digits = 0)
        {
            if (digits <= 0)
            {
                digits = 10;
            }

            if (digits > 18) // Limit to 18 digits to avoid exceeding long.MaxValue
            {
                throw new ArgumentOutOfRangeException(nameof(digits), "Digits must be 18 or fewer.");
            }

            Random random = new Random();
            long minValue = (long)Math.Pow(10, digits - 1);
            long maxValue = (long)Math.Pow(10, digits) - 1;
            long randomNumber = (long)(random.NextDouble() * (maxValue - minValue) + minValue);

            return randomNumber.ToString();
        }

        private static int GetRandomNumber() =>
            new IntRange(min: 2, max: 10).GetValue();

        private static ReIdentificationRequest CreateRandomReIdentificationRequest(int count)
        {
            ReIdentificationRequest request = CreateReIdentificationRequestFiller(count).Create();

            request.ReIdentificationItems.Add(new ReIdentificationItem
            {
                RowNumber = "",
                Identifier = GenerateRandomNumber(10)
            });

            request.ReIdentificationItems.Add(new ReIdentificationItem
            {
                RowNumber = GetRandomStringWithLengthOf(5),
                Identifier = GenerateRandomNumber(9)
            });

            request.ReIdentificationItems.Add(new ReIdentificationItem
            {
                RowNumber = GetRandomStringWithLengthOf(5),
                Identifier = GenerateRandomNumber(11)
            });

            return request;
        }

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
                    RowNumber = GetRandomStringWithLengthOf(5),
                    Identifier = item
                });
            }

            return reIdentificationItems;
        }

        private static string GenerateRandom10DigitNumber()
        {
            Random random = new Random();
            var randomNumber = random.Next(1000000000, 2000000000).ToString();

            return randomNumber;
        }

        private static ReIdentificationRequest CreateRandomReIdentificationResponse(
            ReIdentificationRequest reIdentificationRequest,
            DemoDataReIdentificationConfigurations demoDataReIdentificationConfigurations)
        {
            ReIdentificationRequest request = new ReIdentificationRequest
            {
                RequestId = reIdentificationRequest.RequestId,
                UserIdentifier = reIdentificationRequest.UserIdentifier,
                Organisation = reIdentificationRequest.Organisation,
                Reason = reIdentificationRequest.Reason,
                ReIdentificationItems = new List<ReIdentificationItem>()
            };

            foreach (var item in reIdentificationRequest.ReIdentificationItems)
            {
                var result = new ReIdentificationItem
                {
                    RowNumber = item.RowNumber,
                };

                if (string.IsNullOrWhiteSpace(item.RowNumber))
                {
                    result.Identifier = demoDataReIdentificationConfigurations.DefaultIdentifier;
                    result.Message = "Each identifier must have a corresponding row number.";
                    request.ReIdentificationItems.Add(result);
                    continue;
                }

                if (
                    string.IsNullOrWhiteSpace(item.Identifier)
                    || item.Identifier.Length != 10
                    || item.Identifier.All(char.IsDigit) is false)
                {
                    result.Identifier = demoDataReIdentificationConfigurations.DefaultIdentifier;
                    result.Message = "Identifier must be exactly 10 digits.";
                    request.ReIdentificationItems.Add(result);
                    continue;
                }

                var demoNhsNumber = demoDataReIdentificationConfigurations.DemoPrefix +
                    item.Identifier.Substring(0, 7);

                result.Identifier = demoNhsNumber;
                result.Message = "OK";
                request.ReIdentificationItems.Add(result);
            }

            return request;
        }
    }
}