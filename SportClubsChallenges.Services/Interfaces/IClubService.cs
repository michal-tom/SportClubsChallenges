namespace SportClubsChallenges.Domain.Interfaces
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using SportClubsChallenges.Model.Dto;

    public interface IClubService
    {
        Task<List<ClubDto>> GetAllClubs();

        Task<List<ClubDto>> GetAthleteClubs(long athleteId);

        Task<ClubDto> GetClub(long id);

        Task EditClub(ClubDto dto);

        Task<List<AthleteDto>> GetMembers(long id);

        Task<List<ChallengeOverviewDto>> GetChallenges(long id);
    }
}