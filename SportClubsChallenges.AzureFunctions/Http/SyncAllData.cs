namespace SportClubsChallenges.AzureFunctions.Http
{
    using System.Threading.Tasks;
    using AutoMapper;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Azure.WebJobs;
    using Microsoft.Azure.WebJobs.Extensions.Http;
    using Microsoft.AspNetCore.Http;
    using Microsoft.Extensions.Logging;
    using SportClubsChallenges.Database.Data;
    using SportClubsChallenges.Domain.Interfaces;
    using SportClubsChallenges.Jobs;
    using SportClubsChallenges.Strava;
    using System.Linq;
    using Microsoft.EntityFrameworkCore;

    public class SyncAllData
    {
        private readonly SportClubsChallengesDbContext db;

        private readonly IStravaApiWrapper stravaWrapper;

        private readonly ITokenService tokenService;

        private readonly IMapper mapper;

        public SyncAllData(SportClubsChallengesDbContext db, IStravaApiWrapper stravaWrapper, ITokenService tokenService, IMapper mapper)
        {
            this.db = db;
            this.stravaWrapper = stravaWrapper;
            this.tokenService = tokenService;
            this.mapper = mapper;
        }

        [FunctionName("SyncAllData")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Admin, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function SyncAllData a request.");

            var activitiesJob = new GetAthletesActivitiesJob(this.db, this.stravaWrapper, this.tokenService, this.mapper);
            await activitiesJob.Run();

            var athletes = await this.db.Athletes.ToListAsync();
            foreach (var athlete in athletes)
            {
                var clubsJob = new GetAthletesClubsJob(this.db, this.stravaWrapper, this.tokenService, this.mapper);
                await clubsJob.Run(athlete.Id);
            }

            var activeChallenges = await this.db.Challenges.Where(p => p.IsActive).ToListAsync();
            foreach (var challenge in activeChallenges)
            {
                var classificationsJob = new UpdateChallengesClassificationsJob(this.db);
                await classificationsJob.Run(challenge.Id);
            }

            return new OkObjectResult("OK");
        }
    }
}