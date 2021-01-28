namespace SportClubsChallenges.Domain.Interfaces
{
    using SportClubsChallenges.Model.Dto;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface IChallengeService
    {
        Task<List<ChallengeDto>> GetAllChallenges();

        Task<ChallengeDto> GetChallenge(long id);

        Task AddChallenge(ChallengeDto dto);

        Task UpdatChallenge(ChallengeDto dto);

        Task DeleteChallenge(long id);

        Task<Dictionary<long, string>> GetAvailableClubs();

        Task<Dictionary<byte, string>> GetAvailableActivityTypes();

        Dictionary<byte, string> GetAvailableChallengeTypes();
    }
}
