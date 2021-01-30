namespace SportClubsChallenges.Domain.Interfaces
{
    using System.Threading.Tasks;
    using SportClubsChallenges.Model;

    public interface IStravaTokenService
    {
        Task<StravaToken> RetrieveAccessTokenAsync(StravaToken token);
    }
}
