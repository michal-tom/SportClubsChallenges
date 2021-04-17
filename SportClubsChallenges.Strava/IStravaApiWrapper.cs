namespace SportClubsChallenges.Strava
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using global::Strava.NET.Model;
    using SportClubsChallenges.Model.Strava;

    public interface IStravaApiWrapper
    {
        Task<List<SummaryActivity>> GetAthleteActivites(StravaToken token, DateTimeOffset startTime, DateTimeOffset? endTime = null);

        Task<DetailedActivity> GetActivity(StravaToken token, long activityId);

        Task<List<SummaryClub>> GetAthleteClubs(StravaToken token);
    }
}
