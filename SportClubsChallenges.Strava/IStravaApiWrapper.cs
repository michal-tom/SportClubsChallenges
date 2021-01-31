namespace SportClubsChallenges.Strava
{
    using global::Strava.NET.Model;
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface IStravaApiWrapper
    {
        Task<StravaToken> RetrieveAccessTokenAsync(StravaToken token);

        List<SummaryActivity> GetAthleteActivites(StravaToken token, DateTimeOffset? startTime = null, DateTimeOffset? endTime = null);
    }
}
