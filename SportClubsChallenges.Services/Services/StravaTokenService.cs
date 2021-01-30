namespace SportClubsChallenges.Domain.Services
{
    using System;
    using System.Net.Http;
    using System.Threading.Tasks;
    using AspNet.Security.OAuth.Strava;
    using IdentityModel.Client;
    using SportClubsChallenges.Domain.Interfaces;
    using SportClubsChallenges.Model;

    public class StravaTokenService : IStravaTokenService
    {
        private readonly IHttpClientFactory httpClientFactory;

        public StravaTokenService(IHttpClientFactory httpClientFactory)
        {
            this.httpClientFactory = httpClientFactory;
        }

        public async Task<StravaToken> RetrieveAccessTokenAsync(StravaToken token)
        {
            // check if strava token should be refreshed 
            if ((token.ExpiresAt.AddSeconds(-60)).ToUniversalTime() > DateTime.UtcNow)
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
            token.ExpiresAt = DateTime.UtcNow.AddSeconds(refreshResponse.ExpiresIn);

            return token;
        }
    }
}