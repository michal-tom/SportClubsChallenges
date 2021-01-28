namespace SportClubsChallenges.Domain.Interfaces
{
    using SportClubsChallenges.Model.Dto;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface IClubService
    {
        Task<List<ClubDto>> GetAllClubs();

        Task<ClubDto> GetClub(long id);

        Task AddClub(ClubDto dto);

        Task UpdatClub(ClubDto dto);
    }
}
