// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using ISL.Providers.ReIdentification.Abstractions.Models;
using ISL.Providers.ReIdentification.OfflineFileSources.Models;
using ISL.Providers.ReIdentification.OfflineFileSources.Providers.OfflineFileSources;
using Microsoft.Extensions.Configuration;
using Tynamix.ObjectFiller;

namespace ISL.NotificationClient.Tests.Acceptance
{
    public partial class ReIdentificationServiceTests
    {
        private readonly IOfflineFileSourceReIdentificationProvider offlineFileSourceReIdentificationProvider;
        private readonly IConfiguration configuration;

        public ReIdentificationServiceTests()
        {
            var configurationBuilder = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile("appsettings.Development.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables();

            configuration = configurationBuilder.Build();

            OfflineSourceReIdentificationConfigurations offlineSourceReIdentificationConfigurations = configuration
                .GetSection("offlineSourceReIdentificationConfigurations")
                    .Get<OfflineSourceReIdentificationConfigurations>();

            string assembly = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            char separator = Path.DirectorySeparatorChar;

            string inputFilePath = Path.Combine(
                assembly,
                offlineSourceReIdentificationConfigurations.FilePath.Replace('\\', separator).Replace('/', separator));

            offlineSourceReIdentificationConfigurations.FilePath = inputFilePath;

            this.offlineFileSourceReIdentificationProvider =
                new OfflineFileSourceReIdentificationProvider(offlineSourceReIdentificationConfigurations);
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

        private static ReIdentificationRequest CreateRandomReIdentificationRequest()
        {
            ReIdentificationRequest request = new ReIdentificationRequest
            {
                RequestId = Guid.NewGuid(),
                UserIdentifier = GetRandomString(),
                Organisation = GetRandomString(),
                Reason = GetRandomString(),
                ReIdentificationItems = new List<ReIdentificationItem>
                {
                    new ReIdentificationItem { RowNumber = "1", Identifier = "0000000001" },
                    new ReIdentificationItem { RowNumber = "2", Identifier = "0000000002" },
                    new ReIdentificationItem { RowNumber = "3", Identifier = "0000000003" },
                    new ReIdentificationItem { RowNumber = "4", Identifier = "0000000004" },
                    new ReIdentificationItem { RowNumber = "5", Identifier = "0000000005" },
                    new ReIdentificationItem { RowNumber = "6", Identifier = GenerateRandomNumber() },
                    new ReIdentificationItem { RowNumber = "", Identifier =  GenerateRandomNumber() },
                    new ReIdentificationItem { RowNumber = "8", Identifier = GenerateRandomNumber(16) },
                    new ReIdentificationItem { RowNumber = "9", Identifier = GenerateRandomNumber(17) },
                }
            };

            return request;
        }

        private static ReIdentificationRequest CreateRandomReIdentificationResponse(
            ReIdentificationRequest reIdentificationRequest)
        {
            ReIdentificationRequest request = new ReIdentificationRequest
            {
                RequestId = reIdentificationRequest.RequestId,
                UserIdentifier = reIdentificationRequest.UserIdentifier,
                Organisation = reIdentificationRequest.Organisation,
                Reason = reIdentificationRequest.Reason,
                ReIdentificationItems = new List<ReIdentificationItem>
                {
                    new ReIdentificationItem { RowNumber = "1", Identifier = "1111111111", Message = "OK" },
                    new ReIdentificationItem { RowNumber = "2", Identifier = "2222222222", Message = "OK" },
                    new ReIdentificationItem { RowNumber = "3", Identifier = "3333333333", Message = "OK" },
                    new ReIdentificationItem { RowNumber = "4", Identifier = "4444444444", Message = "OK" },
                    new ReIdentificationItem { RowNumber = "5", Identifier = "5555555555", Message = "OK" },

                    new ReIdentificationItem {
                        RowNumber = "6",
                        Identifier =  "0000000000",
                        Message = "Pseudo identifier not found in the offline source." },

                    new ReIdentificationItem {
                        RowNumber = "",
                        Identifier = "0000000000",
                        Message = "Each identifier must have a corresponding row number." },

                    new ReIdentificationItem {
                        RowNumber = "8",
                        Identifier = "0000000000",
                        Message = "Identifier must be less than 15 digits." },

                    new ReIdentificationItem {
                        RowNumber = "9",
                        Identifier = "0000000000",
                        Message = "Identifier must be less than 15 digits." },
                }
            };

            return request;
        }
    }
}