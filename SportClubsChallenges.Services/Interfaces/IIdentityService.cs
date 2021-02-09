namespace SportClubsChallenges.Domain.Interfaces
{
    using System.Security.Claims;
    using SportClubsChallenges.Database.Entities;

    public interface IIdentityService
    {
        long GetAthleteIdFromIdentity(ClaimsIdentity identity);

        void UpdateIdentity(ClaimsIdentity identity, Athlete athlete);

        void UpdateAthleteDataFromIdentity(ClaimsIdentity identity, Athlete athlete);
    }
}
