namespace SportClubsChallenges.Domain.Interfaces
{
    using SportClubsChallenges.Model.Dto;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface IChallengeService
    {
        Task<List<ChallengeOverviewDto>> GetAllChallenges();

        Task<List<ChallengeParticipationDto>> GetAthleteChallengeParticipations(long athleteId);

        Task<ChallengeDetailsDto> GetChallenge(long id);

        Task AddChallenge(ChallengeDetailsDto dto);

        Task UpdatChallenge(ChallengeDetailsDto dto);

        Task DeleteChallenge(long id);

        Task<Dictionary<long, string>> GetAvailableClubs();

        Task<Dictionary<byte, string>> GetAvailableActivityTypes();

        Dictionary<byte, string> GetAvailableChallengeTypes();
    }
}
