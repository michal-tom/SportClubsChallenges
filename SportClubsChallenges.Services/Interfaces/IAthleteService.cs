namespace SportClubsChallenges.Domain.Interfaces
{
    using System.Security.Claims;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Authentication;
    using SportClubsChallenges.Model.Dto;

    public interface IAthleteService
    {
        Task OnAthleteLogin(ClaimsIdentity identity, AuthenticationProperties properties);

        Task<AthleteDto> GetAthlete(long id);

        OverallStatsDto GetAthleteActivitiesTotalStats(long id);
    }
}
