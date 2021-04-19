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
    using Microsoft.Extensions.Configuration;
    using SportClubsChallenges.Model.Strava;
    using SportClubsChallenges.Utils.Consts;

    public class StravaApiWrapper : IStravaApiWrapper
    {
        private readonly IHttpClientFactory httpClientFactory;

        private readonly IConfiguration configuration;

        private static readonly DateTimeOffset ActivitiesMinDataTime = new DateTimeOffset(2021, 1, 1, 0, 0, 0, TimeSpan.Zero);

        public StravaApiWrapper(IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            this.httpClientFactory = httpClientFactory;
            this.configuration = configuration;
        }

        public async Task<List<SummaryActivity>> GetAthleteActivites(StravaToken token, DateTimeOffset startTime, DateTimeOffset? endTime = null)
        {
            var allActivites = new List<SummaryActivity>();

            var minActivityDateTime = startTime> ActivitiesMinDataTime ? startTime : ActivitiesMinDataTime;
            var maxActivityDateTime = endTime ?? DateTimeOffset.UtcNow;
            var minActivityEpochTime = (int) minActivityDateTime.ToUnixTimeSeconds();
            var maxActivityEpochTime = (int) maxActivityDateTime.ToUnixTimeSeconds();
            var pageNumber = 1;

            try
            {
                await this.RefreshAccessTokenIfNeededAsync(token);

                var apiClient = new ApiClient { AccessToken = token.AccessToken };
                var apiInstance = new ActivitiesApi(apiClient);

                while(true)
                {
                    var activites = apiInstance.GetLoggedInAthleteActivities(maxActivityEpochTime, minActivityEpochTime, pageNumber, StravaConsts.MaxApiRecordsPerPage);
                    allActivites.AddRange(activites);

                    if (activites.Count <= StravaConsts.MaxApiRecordsPerPage * 0.9)
                    {
                        break;
                    }
                    else
                    {
                        pageNumber++;
                    }
                }
            }
            catch(Exception)
            {
                // TODO: log error
                throw;
            }

            return allActivites;
        }

        public async Task<DetailedActivity> GetActivity(StravaToken token, long activityId)
        {
            try
            {
                await this.RefreshAccessTokenIfNeededAsync(token);

                var apiClient = new ApiClient { AccessToken = token.AccessToken };
                var apiInstance = new ActivitiesApi(apiClient);

                return apiInstance.GetActivityById(activityId, includeAllEfforts: false);

            }
            catch (Exception)
            {
                // TODO: log error
                return null;
            }
        }

        public async Task<List<SummaryClub>> GetAthleteClubs(StravaToken token)
        {
            var allClubs = new List<SummaryClub>();
            var pageNumber = 1;

            try
            {
                await this.RefreshAccessTokenIfNeededAsync(token);

                var apiClient = new ApiClient { AccessToken = token.AccessToken };
                var apiInstance = new ClubsApi(apiClient);

                while (true)
                {
                    var clubs = apiInstance.GetLoggedInAthleteClubs(pageNumber, StravaConsts.MaxApiRecordsPerPage);
                    allClubs.AddRange(clubs);

                    if (clubs.Count <= StravaConsts.MaxApiRecordsPerPage * 0.9)
                    {
                        break;
                    }
                    else
                    {
                        pageNumber++;
                    }
                }
            }
            catch (Exception)
            {
                // TODO: log error
                throw;
            }

            return allClubs;
        }

        private async Task RefreshAccessTokenIfNeededAsync(StravaToken token)
        {
            // check if strava token should be refreshed
            if ((token.ExpirationDate.AddSeconds(-60)).ToUniversalTime() > DateTimeOffset.UtcNow)
            {
                return;
            }

            var httpClient = this.httpClientFactory.CreateClient();

            var refreshResponse = await httpClient.RequestRefreshTokenAsync(
               new RefreshTokenRequest
               {
                   Address = StravaAuthenticationDefaults.TokenEndpoint,
                   ClientId = configuration["StravaClientId"],
                   ClientSecret = configuration["StravaClientSecret"],
                   RefreshToken = token.RefreshToken
               });

            token.AccessToken = refreshResponse.AccessToken;
            token.RefreshToken = refreshResponse.RefreshToken;
            token.TokenType = refreshResponse.TokenType;
            token.ExpirationDate = DateTimeOffset.UtcNow.AddSeconds(refreshResponse.ExpiresIn);
            token.TokenRefresh();
        }
    }
}
