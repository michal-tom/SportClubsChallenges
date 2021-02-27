namespace SportClubsChallenges.Domain.Interfaces
{
    using SportClubsChallenges.Model.Dto;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface IClubService
    {
        Task<List<ClubDto>> GetAllClubs();

        Task<List<ClubDto>> GetAthleteClubs(long athleteId);

        Task<ClubDto> GetClub(long id);

        Task<List<AthleteDto>> GetMembers(long id);

        Task<List<ChallengeOverviewDto>> GetChallenges(long id);

        Task AddClub(ClubDto dto);

        Task UpdatClub(ClubDto dto);
    }
}
