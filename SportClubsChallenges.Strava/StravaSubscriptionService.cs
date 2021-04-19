namespace SportClubsChallenges.Strava
{
    using System.Collections.Generic;
    using System.Net.Http;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.WebUtilities;
    using SportClubsChallenges.Utils.Consts;

    public class StravaSubscriptionService : IStravaSubscriptionService
    {
        private readonly IHttpClientFactory httpClientFactory;

        public StravaSubscriptionService(IHttpClientFactory httpClientFactory)
        {
            this.httpClientFactory = httpClientFactory;
        }

        public async Task<string> CreateSubscription(string callbackUrl, string token, string clientId, string clientSecret)
        {
            var parameters = new Dictionary<string, string> { 
                { "client_id", clientId }, 
                { "client_secret", clientSecret },
                { "callback_url", callbackUrl },
                { "verify_token", token }
            };
            var encodedContent = new FormUrlEncodedContent(parameters);

            var httpClient = this.httpClientFactory.CreateClient();
                    
            var response = await httpClient.PostAsync(StravaConsts.SubscriptionUrlAddress, encodedContent);
            response.EnsureSuccessStatusCode();

            return await response.Content.ReadAsStringAsync();
        }

        public async Task<string> ViewSubscription(string clientId, string clientSecret)
        {
            var parameters = new Dictionary<string, string> {
                { "client_id", clientId },
                { "client_secret", clientSecret }
            };

            var url = QueryHelpers.AddQueryString(StravaConsts.SubscriptionUrlAddress, parameters);

            var httpClient = this.httpClientFactory.CreateClient();

            var response = await httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();

            return await response.Content.ReadAsStringAsync();
        }

        public async Task<string> DeleteSubscription(long id, string clientId, string clientSecret)
        {
            var parameters = new Dictionary<string, string> {
                { "client_id", clientId },
                { "client_secret", clientSecret }
            };

            var url = QueryHelpers.AddQueryString($"{StravaConsts.SubscriptionUrlAddress}/{id}", parameters);

            var httpClient = this.httpClientFactory.CreateClient();

            var response = await httpClient.DeleteAsync(url);
            response.EnsureSuccessStatusCode();

            return await response.Content.ReadAsStringAsync();
        }
    }
}