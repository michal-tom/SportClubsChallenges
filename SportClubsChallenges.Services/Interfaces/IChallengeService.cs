namespace SportClubsChallenges.Domain.Interfaces
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using SportClubsChallenges.Model.Dto;

    public interface IChallengeService
    {
        Task<List<ChallengeOverviewDto>> GetAllChallenges();

        Task<List<ChallengeOverviewDto>> GetAvailableChallenges(long athleteId);

        Task<List<ChallengeParticipationDto>> GetActiveChallengeParticipations(long athleteId);

        Task<List<ChallengeRankPositionDto>> GetChallengeRank(long challengeId, long athleteId);

        Task<ChallengeDetailsDto> GetChallenge(long challengeId);

        Task AddOrEditChallenge(ChallengeDetailsDto dto);

        Task DeleteChallenge(long challengeId);

        Task LeaveChallenge(long athleteId, long challengeId);

        Task JoinChallenge(long athleteId, long challengeId);

        Task UpdateChallengeRank(long challengeId);

        Dictionary<byte, string> GetAvailableActivityTypes();

        Dictionary<byte, string> GetBikeActivityTypes();

        Dictionary<byte, string> GetRunningActivityTypes();

        Dictionary<byte, string> GetAvailableActivityTypeProfiles();

        Dictionary<byte, string> GetAvailableChallengeCompetitionTypes();
    }
}