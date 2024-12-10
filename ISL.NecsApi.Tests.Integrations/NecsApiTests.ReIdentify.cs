// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using ISL.NecsApi.Tests.Integrations;
using ISL.NecsApi.Tests.Integrations.Models.NECS.Requests;
using ISL.NecsApi.Tests.Integrations.Models.NECS.Responses;

namespace ISL.Providers.ReIdentification.Necs.Tests.Integrations
{
    public partial class NecsProviderTests
    {
        [ReleaseCandidateFact]
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

        [Theory]
        [InlineData(10)]
        [InlineData(20)]
        [InlineData(50)]
        [InlineData(100)]
        [InlineData(150)]
        [InlineData(200)]
        [InlineData(300)]
        [InlineData(350)]
        [InlineData(400)]
        [InlineData(450)]
        [InlineData(500)]
        [InlineData(750)]
        [InlineData(1000)]
        [InlineData(1500)]
        [InlineData(2000)]
        [InlineData(5000)]
        [InlineData(10000)]
        public async Task ShouldMeasureReIdentificationPerformanceForBatchAsync(int records)
        {
            // Given
            NecsReIdentificationRequest randomReIdentificationRequest =
                CreateRandomNecsReIdentificationRequest(count: records);

            // When
            var result =
                await apiClient.PostContentAsync<NecsReIdentificationRequest, NecsReIdentificationResponse>
                    (necsConfiguration.ApiUrl, randomReIdentificationRequest);

            // Then
            result.Should().NotBeNull();
            result.ProcessedCount.Should().Be(records);
            output.WriteLine($"Items in the request: {records}");
            output.WriteLine($"ElapsedTime: {result.ElapsedTime}");
        }


        [Theory]
        [InlineData(150)]
        [InlineData(200)]
        [InlineData(250)]
        [InlineData(500)]
        [InlineData(750)]
        [InlineData(1000)]
        [InlineData(1500)]
        [InlineData(3000)]
        [InlineData(5000)]
        [InlineData(10000)]
        public async Task ShouldMeasureReIdentificationRepeatPerformanceAsync(int records)
        {
            // Given
            int testIterations = 10; // Number of consecutive tests to run
            List<TimeSpan> elapsedTimes = new List<TimeSpan>();

            NecsReIdentificationRequest randomReIdentificationRequest =
                CreateRandomNecsReIdentificationRequest(count: records);

            // When
            for (int i = 0; i < testIterations; i++)
            {
                randomReIdentificationRequest.RequestId = Guid.NewGuid();
                var stopwatch = Stopwatch.StartNew();

                var result =
                    await apiClient.PostContentAsync<NecsReIdentificationRequest, NecsReIdentificationResponse>
                        (necsConfiguration.ApiUrl, randomReIdentificationRequest);

                stopwatch.Stop();

                // Collect elapsed time
                elapsedTimes.Add(stopwatch.Elapsed);

                // Assert each iteration result
                result.Should().NotBeNull();
            }

            // Then
            TimeSpan minTime = elapsedTimes.Min();
            TimeSpan maxTime = elapsedTimes.Max();
            TimeSpan averageTime = TimeSpan.FromMilliseconds(elapsedTimes.Average(et => et.TotalMilliseconds));

            output.WriteLine($"Items in the request: {records}");
            output.WriteLine($"Performance over {testIterations} iterations:");
            output.WriteLine($"Minimum Time: {minTime}");
            output.WriteLine($"Maximum Time: {maxTime}");
            output.WriteLine($"Average Time: {averageTime}");
        }

        [Theory]
        [InlineData(5)]
        [InlineData(10)]
        [InlineData(20)]
        [InlineData(50)]
        [InlineData(75)]
        [InlineData(100)]
        public async Task ShouldPerformConcurrentLoadTestAsync(int requests)
        {
            // Given
            int numberOfRequests = requests; // Number of simultaneous requests
            int randomCount = 100;
            List<Task<(bool isSuccess, TimeSpan elapsedTime)>> tasks = new List<Task<(bool, TimeSpan)>>();

            NecsReIdentificationRequest randomReIdentificationRequest =
                CreateRandomNecsReIdentificationRequest(count: randomCount);

            // When
            for (int i = 0; i < numberOfRequests; i++)
            {
                tasks.Add(Task.Run(async () =>
                {
                    var stopwatch = Stopwatch.StartNew();

                    try
                    {
                        randomReIdentificationRequest.RequestId = Guid.NewGuid();

                        var result =
                            await apiClient.PostContentAsync<NecsReIdentificationRequest, NecsReIdentificationResponse>
                                (necsConfiguration.ApiUrl, randomReIdentificationRequest);

                        stopwatch.Stop();
                        return (result != null, stopwatch.Elapsed);
                    }
                    catch
                    {
                        stopwatch.Stop();
                        return (false, stopwatch.Elapsed);
                    }
                }));
            }

            // Wait for all tasks to complete
            var results = await Task.WhenAll(tasks);

            // Then
            int successCount = results.Count(r => r.isSuccess);
            int failureCount = results.Count(r => !r.isSuccess);
            var elapsedTimes = results.Select(r => r.elapsedTime).ToList();

            TimeSpan minTime = elapsedTimes.Min();
            TimeSpan maxTime = elapsedTimes.Max();
            TimeSpan averageTime = TimeSpan.FromMilliseconds(elapsedTimes.Average(et => et.TotalMilliseconds));

            output.WriteLine($"Load Test Results for {numberOfRequests} requests:");
            output.WriteLine($"Pseudo numbers in each batch {randomCount}:");
            output.WriteLine($"Successful Requests: {successCount}");
            output.WriteLine($"Failed Requests: {failureCount}");
            output.WriteLine($"Minimum Time: {minTime}");
            output.WriteLine($"Maximum Time: {maxTime}");
            output.WriteLine($"Average Time: {averageTime}");

            successCount.Should().Be(numberOfRequests, because: "all requests should succeed under expected load");
        }

    }
}
