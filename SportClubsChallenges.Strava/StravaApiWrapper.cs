namespace SportClubsChallenges.Strava
{
    using System;
    using System.Collections.Generic;
    using System.Net.Http;
    using System.Threading.Tasks;
    using AspNet.Security.OAuth.Strava;
    using global::Strava.NET.Api;
    using global::Strava.NET.Client;
    using global::Strava.NET.Model;
    using IdentityModel.Client;

    public class StravaApiWrapper : IStravaApiWrapper
    {
        private readonly IHttpClientFactory httpClientFactory;

        private static readonly DateTimeOffset MinDataTime = (new DateTimeOffset(2021, 1, 1, 0, 0, 0, TimeSpan.Zero));

        public StravaApiWrapper(IHttpClientFactory httpClientFactory)
        {
            this.httpClientFactory = httpClientFactory;
        }

        public async Task<StravaToken> RetrieveAccessTokenAsync(StravaToken token)
        {
            // check if strava token should be refreshed 
            if ((token.ExpirationDate.AddSeconds(-60)).ToUniversalTime() > DateTimeOffset.UtcNow)
            {
                return token;
            }

            var httpClient = this.httpClientFactory.CreateClient();

            // TODO: read client id and secret from config file
            var refreshResponse = await httpClient.RequestRefreshTokenAsync(
               new RefreshTokenRequest
               {
                   Address = StravaAuthenticationDefaults.TokenEndpoint,
                   ClientId = "60033",
                   ClientSecret = "45b4066142165ecd3dee2d28556da83d77081bea",
                   RefreshToken = token.RefreshToken
               });

            token.AccessToken = refreshResponse.AccessToken;
            token.RefreshToken = refreshResponse.RefreshToken;
            token.TokenType = refreshResponse.TokenType;
            token.ExpirationDate = DateTimeOffset.UtcNow.AddSeconds(refreshResponse.ExpiresIn);

            return token;
        }

        public List<SummaryActivity> GetAthleteActivites(StravaToken token, DateTimeOffset? startTime = null, DateTimeOffset? endTime = null)
        {
            startTime = startTime == null || startTime < MinDataTime ? MinDataTime : startTime;

            Configuration.DefaultApiClient.AccessToken = token.AccessToken;

            var apiInstance = new ActivitiesApi();
            var before = endTime.HasValue ? (int) endTime.Value.ToUnixTimeSeconds() : (int?) null;
            var after = startTime.HasValue ? (int) startTime.Value.ToUnixTimeSeconds() : (int?) null;
            var page = 1;
            var perPage = 200;

            try
            {
                return apiInstance.GetLoggedInAthleteActivities(before, after, page, perPage);
            }
            catch(Exception)
            {
                // TODO: log error
                throw;
            }
        }
    }
}
