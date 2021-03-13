namespace SportClubsChallenges.Strava
{
    using global::Strava.NET.Model;
    using SportClubsChallenges.Model.Strava;
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface IStravaApiWrapper
    {
        Task<List<SummaryActivity>> GetAthleteActivites(StravaToken token, DateTimeOffset startTime, DateTimeOffset? endTime = null);

        Task<List<SummaryClub>> GetAthleteClubs(StravaToken token);
    }
}
