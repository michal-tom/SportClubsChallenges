namespace SportClubsChallenges.Api.Controllers
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using SportClubsChallenges.Domain.Interfaces;
    using SportClubsChallenges.Model.Dto;

    [ApiController]
    [Route("[controller]")]
    public class ClubsController : ControllerBase
    {
        private readonly IClubService clubService;

        public ClubsController(IClubService clubService)
        {
            this.clubService = clubService;
        }

        [HttpGet]
        public async Task<IEnumerable<ClubDto>> GetClubs()
        {
            return await this.clubService.GetAllClubs();
        }

        [HttpGet("{id}")]
        public async Task<ClubDto> GetClub(int id)
        {
            return await this.clubService.GetClub(id);
        }

        [HttpGet("{id}/members")]
        public async Task<IEnumerable<AthleteDto>> GetClubMembers(int id)
        {
            return await this.clubService.GetMembers(id);
        }

        [HttpGet("{id}/challenges")]
        public async Task<IEnumerable<ChallengeOverviewDto>> GetClubChallenges(int id)
        {
            return await this.clubService.GetChallenges(id);
        }
    }
}