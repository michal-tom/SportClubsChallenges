namespace SportClubsChallenges.Domain.Interfaces
{
    using Microsoft.AspNetCore.Authentication;
    using System.Security.Claims;
    using System.Threading.Tasks;

    public interface IAthleteService
    {
        Task OnAthleteLogin(ClaimsIdentity identity, AuthenticationProperties properties);
    }
}
