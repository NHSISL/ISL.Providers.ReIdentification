// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Net.Http;
using System.Threading.Tasks;
using ISL.Providers.ReIdentification.Necs.Models.Brokers.NECS;
using ISL.Providers.ReIdentification.Necs.Models.Brokers.NECS.Requests;
using ISL.Providers.ReIdentification.Necs.Models.Brokers.NECS.Responses;
using RESTFulSense.Clients;

namespace ISL.Providers.ReIdentification.Necs.Brokers.Necs
{
    public class NECSBroker : INECSBroker
    {
        private readonly NecsReIdentificationConfigurations necsConfiguration;
        private readonly IRESTFulApiFactoryClient apiClient;
        private readonly HttpClient httpClient;

        public NECSBroker(NecsReIdentificationConfigurations necsConfiguration)
        {
            this.necsConfiguration = necsConfiguration;
            httpClient = SetupHttpClient();
            apiClient = SetupApiClient();
        }

        public async ValueTask<NecsReIdentificationResponse> ReIdAsync(
            NecsReIdentificationRequest necsReIdentificationRequest)
        {
            string route = $"api/Reid/Process";

            string path = necsConfiguration.ApiUrl.EndsWith("/")
                ? route
                : $"/{route}";

            var returnedAddress =
                await apiClient.PostContentAsync<NecsReIdentificationRequest, NecsReIdentificationResponse>
                    (path, necsReIdentificationRequest);

            return returnedAddress;
        }

        private HttpClient SetupHttpClient()
        {
            var httpClient = new HttpClient()
            {
                BaseAddress =
                    new Uri(uriString: necsConfiguration.ApiUrl),
            };

            httpClient.DefaultRequestHeaders.Add("X-API-KEY", necsConfiguration.ApiKey);

            return httpClient;
        }

        private IRESTFulApiFactoryClient SetupApiClient() =>
            new RESTFulApiFactoryClient(httpClient);
    }
}
