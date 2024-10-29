// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using ISL.Providers.ReIdentification.Abstractions.Models;
using ISL.Providers.ReIdentification.Necs.Brokers.Identifiers;
using ISL.Providers.ReIdentification.Necs.Brokers.Necs;
using ISL.Providers.ReIdentification.Necs.Models.Brokers.NECS;
using ISL.Providers.ReIdentification.Necs.Models.Brokers.NECS.Requests;
using ISL.Providers.ReIdentification.Necs.Models.Brokers.NECS.Responses;
using ISL.Providers.ReIdentification.Necs.Services.Foundations.ReIdentifications;
using ISL.ReIdentification.Core.Models.Brokers.NECS.Requests;
using KellermanSoftware.CompareNetObjects;
using Moq;
using RESTFulSense.Exceptions;
using Tynamix.ObjectFiller;
using Xeptions;

namespace ISL.Providers.ReIdentification.Necs.Tests.Unit.Services.Foundations.ReIdentifications
{
    public partial class ReIdentificationServiceTests
    {
        private readonly Mock<INECSBroker> necsBrokerMock = new Mock<INECSBroker>();
        private readonly Mock<IIdentifierBroker> identifierBrokerMock = new Mock<IIdentifierBroker>();
        private NecsReIdentificationConfigurations necsReIdentificationConfigurations;
        private readonly IReIdentificationService reIdentificationService;
        private readonly ICompareLogic compareLogic;

        public ReIdentificationServiceTests()
        {
            this.compareLogic = new CompareLogic();
            this.necsBrokerMock = new Mock<INECSBroker>();
            this.identifierBrokerMock = new Mock<IIdentifierBroker>();
            this.necsReIdentificationConfigurations = new NecsReIdentificationConfigurations
            {
                ApiUrl = GetRandomString(),
                ApiKey = GetRandomString(),
                ApiMaxBatchSize = GetRandomNumber()
            };

            this.reIdentificationService = new ReIdentificationService(
                necsBroker: this.necsBrokerMock.Object,
                identifierBroker: this.identifierBrokerMock.Object,
                necsReIdentificationConfigurations: necsReIdentificationConfigurations);
        }

        private Expression<Func<NecsReIdentificationRequest, bool>> SameNecsReIdentificationRequestAs(
            NecsReIdentificationRequest expectedNecsReIdentificationRequest)
        {
            return actualNecsReIdentificationRequest =>
                this.compareLogic.Compare(expectedNecsReIdentificationRequest, actualNecsReIdentificationRequest)
                    .AreEqual;
        }

        public static TheoryData<Xeption> DependencyValidationExceptions()
        {
            string randomMessage = GetRandomString();
            string exceptionMessage = randomMessage;
            var innerException = new Xeption(exceptionMessage);

            return new TheoryData<Xeption>
            {
                new HttpResponseUnauthorizedException(),
                new HttpResponseUrlNotFoundException(),
                new HttpResponseBadRequestException()
            };
        }

        public static TheoryData<Xeption> DependencyExceptions()
        {
            string randomMessage = GetRandomString();
            string exceptionMessage = randomMessage;
            var innerException = new Xeption(exceptionMessage);

            return new TheoryData<Xeption>
            {
                new HttpResponseInternalServerErrorException()
            };
        }

        private (List<NecsReIdentificationRequest> requests, List<NecsReIdentificationResponse> responses)
            CreateBatchedItems(ReIdentificationRequest reIdentificationRequest, int batchSize, Guid identifier)
        {
            List<NecsReIdentificationRequest> requests = new List<NecsReIdentificationRequest>();
            List<NecsReIdentificationResponse> responses = new List<NecsReIdentificationResponse>();

            for (int i = 0; i < reIdentificationRequest.ReIdentificationItems.Count; i += batchSize)
            {
                NecsReIdentificationRequest necsReIdentificationRequest = new NecsReIdentificationRequest
                {
                    RequestId = identifier,
                    UserIdentifier = reIdentificationRequest.UserIdentifier,
                    Organisation = reIdentificationRequest.Organisation,
                    Reason = reIdentificationRequest.Reason,
                    PseudonymisedNumbers = reIdentificationRequest.ReIdentificationItems.Skip(i)
                    .Take(batchSize).ToList().Select(item =>
                        new NecsPseudonymisedItem { RowNumber = item.RowNumber, Psuedo = item.Identifier })
                            .ToList()
                };

                requests.Add(necsReIdentificationRequest);

                NecsReIdentificationResponse necsReIdentificationResponse = new NecsReIdentificationResponse
                {
                    UniqueRequestId = identifier,
                    ElapsedTime = GetRandomNumber(),
                    ProcessedCount = necsReIdentificationRequest.PseudonymisedNumbers.Count,
                    Results = reIdentificationRequest.ReIdentificationItems.Skip(i)
                        .Take(batchSize).ToList().Select(item =>
                            new NecsReidentifiedItem
                            {
                                RowNumber = item.RowNumber,
                                NhsNumber = $"{item.Identifier}R",
                                Message = $"{item.Message}M",
                            })
                        .ToList()
                };

                responses.Add(necsReIdentificationResponse);
            }

            return (requests, responses);
        }

        private static Expression<Func<Xeption, bool>> SameExceptionAs(Xeption expectedException) =>
            actualException => actualException.SameExceptionAs(expectedException);

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

        private static ReIdentificationRequest CreateRandomReIdentificationRequest(int count) =>
            CreateReIdentificationRequestFiller(count).Create();

        private static Filler<ReIdentificationRequest> CreateReIdentificationRequestFiller(int count)
        {
            var filler = new Filler<ReIdentificationRequest>();

            filler.Setup()
                .OnProperty(request => request.ReIdentificationItems).Use(CreateRandomReIdentificationItems(count));
            return filler;
        }

        private static List<ReIdentificationItem> CreateRandomReIdentificationItems(int count)
        {
            return CreateIdentificationItemFiller()
                .Create(count)
                    .ToList();
        }

        private static Filler<ReIdentificationItem> CreateIdentificationItemFiller()
        {
            var filler = new Filler<ReIdentificationItem>();

            filler.Setup()
                .OnProperty(address => address.Identifier).Use(GetRandomStringWithLengthOf(10));

            return filler;
        }
    }
}
