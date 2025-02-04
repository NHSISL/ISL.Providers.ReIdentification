// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using ISL.NecsApi.Tests.Integrations.Models.NECS.Requests;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ISL.NecsApi.Tests.Integrations
{
    public partial class NecsProviderTests
    {
        [Fact(DisplayName = "Validation - 2.1 - Body required")]
        public async Task ShouldThrowValidationErrorWhenNoBodyPresentAsync()
        {
            // Given
            var expectedResponse = new
            {
                Type = "https://tools.ietf.org/html/rfc9110#section-15.5.16",
                Title = "Unsupported Media Type",
                Status = 415
            };

            // When
            var response =
                await httpClient.PostAsync(requestUri: necsConfiguration.ApiUrl, content: null);

            string actualContent = await response.Content.ReadAsStringAsync();
            dynamic actualResponse = JsonConvert.DeserializeObject(actualContent);

            // Then
            response.StatusCode.Should().Be(HttpStatusCode.UnsupportedMediaType);
            ((string)actualResponse.type).Should().Be(expectedResponse.Type);
            ((string)actualResponse.title).Should().Be(expectedResponse.Title);
            ((int)actualResponse.status).Should().Be(expectedResponse.Status);
        }

        [Fact(DisplayName = "Validation - 2.2 - Body empty")]
        public async Task ShouldThrowValidationErrorWhenBodyIsEmptyAsync()
        {
            // Given
            var expectedErrors = new Dictionary<string, string[]>
            {
                { "Reason", new[] { "The Reason field is required." } },
                { "RequestId", new[] { "The RequestId field is required.", "The Guid value cannot be null." } },
                { "Organisation", new[] { "The Organisation field is required." } },
                { "UserIdentifier", new[] { "The UserIdentifier field is required." } },
                { "PseudonymisedNumbers", new[] { "The PseudonymisedNumbers field is required." } }
            };

            var expectedResponse = new
            {
                Type = "https://tools.ietf.org/html/rfc9110#section-15.5.1",
                Title = "One or more validation errors occurred.",
                Status = 400
            };

            var jsonContent = new StringContent(
                JsonConvert.SerializeObject(new { }),
                Encoding.UTF8,
                "application/json");

            // When
            var response = await httpClient.PostAsync(necsConfiguration.ApiUrl, jsonContent);

            string actualContent = await response.Content.ReadAsStringAsync();
            var actualResponse = JsonConvert.DeserializeObject<Dictionary<string, object>>(actualContent);

            // Then
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            actualResponse.Should().ContainKey("type").WhoseValue.Should().Be(expectedResponse.Type);
            actualResponse.Should().ContainKey("title").WhoseValue.Should().Be(expectedResponse.Title);
            actualResponse.Should().ContainKey("status").WhoseValue.Should().Be(expectedResponse.Status);
            actualResponse.Should().ContainKey("errors");

            // Deserialize "errors" into JObject since it is a JSON object with keys mapping to arrays
            var actualErrors = JObject.Parse(actualResponse["errors"].ToString());

            // Validate errors dynamically
            actualErrors.Should().NotBeNull();
            actualErrors.Properties().Select(p => p.Name).Should().BeEquivalentTo(expectedErrors.Keys); // Ensure keys match

            foreach (var expectedError in expectedErrors)
            {
                actualErrors.Should().ContainKey(expectedError.Key);
                var actualErrorMessages = actualErrors[expectedError.Key].ToObject<string[]>(); // Convert JToken to string[]
                actualErrorMessages.Should().BeEquivalentTo(expectedError.Value);
            }
        }

        [Fact(DisplayName = "Validation - 2.3 - RequestId required")]
        public async Task ShouldThrowValidationErrorWhenRequestIdIsInvalidAsync()
        {
            // Given
            var expectedErrors = new Dictionary<string, string[]>
            {
                { "RequestId", new[] { "The Guid value cannot be the default Guid." } },
            };

            var expectedResponse = new
            {
                Type = "https://tools.ietf.org/html/rfc9110#section-15.5.1",
                Title = "One or more validation errors occurred.",
                Status = 400
            };

            int randomCount = GetRandomNumber();

            NecsReIdentificationRequest randomReIdentificationRequest =
                CreateRandomNecsReIdentificationRequest(count: randomCount);

            throw new NotImplementedException("Use dynamic object instead");

            randomReIdentificationRequest.RequestId = Guid.Empty;

            var jsonContent = new StringContent(
                JsonConvert.SerializeObject(randomReIdentificationRequest),
                Encoding.UTF8,
                "application/json");

            // When
            var response = await httpClient.PostAsync(necsConfiguration.ApiUrl, jsonContent);

            string actualContent = await response.Content.ReadAsStringAsync();
            var actualResponse = JsonConvert.DeserializeObject<Dictionary<string, object>>(actualContent);

            // Then
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            actualResponse.Should().ContainKey("type").WhoseValue.Should().Be(expectedResponse.Type);
            actualResponse.Should().ContainKey("title").WhoseValue.Should().Be(expectedResponse.Title);
            actualResponse.Should().ContainKey("status").WhoseValue.Should().Be(expectedResponse.Status);
            actualResponse.Should().ContainKey("errors");

            // Deserialize "errors" into JObject since it is a JSON object with keys mapping to arrays
            var actualErrors = JObject.Parse(actualResponse["errors"].ToString());

            // Validate errors dynamically
            actualErrors.Should().NotBeNull();
            actualErrors.Properties().Select(p => p.Name).Should().BeEquivalentTo(expectedErrors.Keys); // Ensure keys match

            foreach (var expectedError in expectedErrors)
            {
                actualErrors.Should().ContainKey(expectedError.Key);
                var actualErrorMessages = actualErrors[expectedError.Key].ToObject<string[]>(); // Convert JToken to string[]
                actualErrorMessages.Should().BeEquivalentTo(expectedError.Value);
            }
        }

        [Fact(DisplayName = "Validation - 2.4 - RequestId must be unique")]
        public async Task ShouldThrowValidationErrorWhenRequestIdIsNotUniqueAsync()
        {
            // Given
            var expectedErrors = new Dictionary<string, string[]>
            {
                { "RequestId", new[] { "RequestId must be unique." } },
            };

            var expectedResponse = new
            {
                Type = "https://tools.ietf.org/html/rfc9110#section-15.5.1",
                Title = "One or more validation errors occurred.",
                Status = 400
            };

            int randomCount = GetRandomNumber();

            NecsReIdentificationRequest randomReIdentificationRequest =
                CreateRandomNecsReIdentificationRequest(count: randomCount);

            var jsonContent = new StringContent(
                JsonConvert.SerializeObject(randomReIdentificationRequest),
                Encoding.UTF8,
                "application/json");

            // When
            await httpClient.PostAsync(necsConfiguration.ApiUrl, jsonContent);
            var response = await httpClient.PostAsync(necsConfiguration.ApiUrl, jsonContent);

            string actualContent = await response.Content.ReadAsStringAsync();
            var actualResponse = JsonConvert.DeserializeObject<Dictionary<string, object>>(actualContent);

            // Then
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            actualResponse.Should().ContainKey("type").WhoseValue.Should().Be(expectedResponse.Type);
            actualResponse.Should().ContainKey("title").WhoseValue.Should().Be(expectedResponse.Title);
            actualResponse.Should().ContainKey("status").WhoseValue.Should().Be(expectedResponse.Status);
            actualResponse.Should().ContainKey("errors");

            // Deserialize "errors" into JObject since it is a JSON object with keys mapping to arrays
            var actualErrors = JObject.Parse(actualResponse["errors"].ToString());

            // Validate errors dynamically
            actualErrors.Should().NotBeNull();
            actualErrors.Properties().Select(p => p.Name).Should().BeEquivalentTo(expectedErrors.Keys); // Ensure keys match

            foreach (var expectedError in expectedErrors)
            {
                actualErrors.Should().ContainKey(expectedError.Key);
                var actualErrorMessages = actualErrors[expectedError.Key].ToObject<string[]>(); // Convert JToken to string[]
                actualErrorMessages.Should().BeEquivalentTo(expectedError.Value);
            }
        }

        [Fact(DisplayName = "Validation - 2.5 - RequestId not default Guid")]
        public async Task ShouldThrowValidationErrorWhenRequestIdIsEmptyGuidAsync()
        {
            // Given
            var expectedErrors = new Dictionary<string, string[]>
            {
                { "RequestId", new[] { "The Guid value cannot be the default Guid." } },
            };

            var expectedResponse = new
            {
                Type = "https://tools.ietf.org/html/rfc9110#section-15.5.1",
                Title = "One or more validation errors occurred.",
                Status = 400
            };

            int randomCount = GetRandomNumber();

            NecsReIdentificationRequest randomReIdentificationRequest =
                CreateRandomNecsReIdentificationRequest(count: randomCount);

            randomReIdentificationRequest.RequestId = Guid.Empty;

            var jsonContent = new StringContent(
                JsonConvert.SerializeObject(randomReIdentificationRequest),
                Encoding.UTF8,
                "application/json");

            // When
            var response = await httpClient.PostAsync(necsConfiguration.ApiUrl, jsonContent);

            string actualContent = await response.Content.ReadAsStringAsync();
            var actualResponse = JsonConvert.DeserializeObject<Dictionary<string, object>>(actualContent);

            // Then
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            actualResponse.Should().ContainKey("type").WhoseValue.Should().Be(expectedResponse.Type);
            actualResponse.Should().ContainKey("title").WhoseValue.Should().Be(expectedResponse.Title);
            actualResponse.Should().ContainKey("status").WhoseValue.Should().Be(expectedResponse.Status);
            actualResponse.Should().ContainKey("errors");

            // Deserialize "errors" into JObject since it is a JSON object with keys mapping to arrays
            var actualErrors = JObject.Parse(actualResponse["errors"].ToString());

            // Validate errors dynamically
            actualErrors.Should().NotBeNull();
            actualErrors.Properties().Select(p => p.Name).Should().BeEquivalentTo(expectedErrors.Keys); // Ensure keys match

            foreach (var expectedError in expectedErrors)
            {
                actualErrors.Should().ContainKey(expectedError.Key);
                var actualErrorMessages = actualErrors[expectedError.Key].ToObject<string[]>(); // Convert JToken to string[]
                actualErrorMessages.Should().BeEquivalentTo(expectedError.Value);
            }
        }


    }
}
