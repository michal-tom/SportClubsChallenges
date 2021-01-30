namespace SportClubsChallenges.Domain.Interfaces
{
    using Microsoft.AspNetCore.Authentication;
    using System.Collections.Generic;
    using System.Security.Claims;
    using System.Threading.Tasks;

    public interface IAthleteService
    {
        Task OnAthleteLogin(ClaimsIdentity identity, IEnumerable<AuthenticationToken> tokens);
    }
}
