namespace SportClubsChallenges.Strava
{
    using Microsoft.AspNetCore.WebUtilities;
    using System.Collections.Generic;
    using System.Net.Http;
    using System.Threading.Tasks;

    public class StravaSubscriptionService : IStravaSubscriptionService
    {
        private readonly string StravaSubscriptionUrl = "https://www.strava.com/api/v3/push_subscriptions";

        private readonly IHttpClientFactory httpClientFactory;

        public StravaSubscriptionService(IHttpClientFactory httpClientFactory)
        {
            this.httpClientFactory = httpClientFactory;
        }

        public async Task<string> CreateSubscription(string callbackUrl, string token)
        {
            var parameters = new Dictionary<string, string> { 
                { "client_id", "60033" }, 
                { "client_secret", "45b4066142165ecd3dee2d28556da83d77081bea" },
                { "callback_url", callbackUrl },
                { "verify_token", token }
            };
            var encodedContent = new FormUrlEncodedContent(parameters);

            var httpClient = this.httpClientFactory.CreateClient();
                    
            var response = await httpClient.PostAsync(this.StravaSubscriptionUrl, encodedContent);
            // response.EnsureSuccessStatusCode();

            return await response.Content.ReadAsStringAsync();
        }

        public async Task<string> ViewSubscription()
        {
            var parameters = new Dictionary<string, string> {
                { "client_id", "60033" },
                { "client_secret", "45b4066142165ecd3dee2d28556da83d77081bea" }
            };

            var url = QueryHelpers.AddQueryString(this.StravaSubscriptionUrl, parameters);

            var httpClient = this.httpClientFactory.CreateClient();

            var response = await httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();

            return await response.Content.ReadAsStringAsync();
        }

        public void DeleteSubscription()
        {
            // TODO
            //var request = new RestRequest($"api/v3/push_subscriptions/{subscriptionId}", Method.DELETE);
            //request.AddQueryParameter("client_id", clientId.ToString());
            //request.AddQueryParameter("client_secret", clientSecret);
            //var response = _restClient.ExecuteWithRetry(request);

            //response.ThrowExceptionIfNotSuccessful();
            //return response.Content;
        }
    }
}
