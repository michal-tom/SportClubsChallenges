namespace SportClubsChallenges.Api.Controllers
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using SportClubsChallenges.Domain.Interfaces;
    using SportClubsChallenges.Model.Dto;

    [ApiController]
    [Route("[controller]")]
    public class AthletesController : ControllerBase
    {
        private readonly IAthleteService athleteService;

        public AthletesController(IAthleteService athleteService)
        {
            this.athleteService = athleteService;
        }

        [HttpGet]
        public async Task<IEnumerable<AthleteDto>> GetAthletes()
        {
            return await this.athleteService.GetAllAthletes();
        }

        [HttpGet("{id}")]
        public async Task<AthleteDto> GetAthlete(int id)
        {
            return await this.athleteService.GetAthlete(id);
        }

        [HttpGet("{id}/stats")]
        public OverallStatsDto GetAthleteActivitiesTotalStats(int id)
        {
            return this.athleteService.GetAthleteActivitiesTotalStats(id);
        }
    }
}
