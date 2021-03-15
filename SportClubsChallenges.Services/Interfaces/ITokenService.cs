namespace SportClubsChallenges.Domain.Interfaces
{
    using Microsoft.AspNetCore.Authentication;
    using SportClubsChallenges.Database.Entities;
    using SportClubsChallenges.Model.Strava;

    public interface ITokenService
    {
        StravaToken GetStravaToken(Athlete athlete);

        void UpdateStravaToken(long athleteId, AuthenticationProperties properties);
    }
}
