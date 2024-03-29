namespace SportClubsChallenges.AzureFunctions.Http
{
    using System.Linq;
    using System.Threading.Tasks;
    using AutoMapper;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Azure.Functions.Worker;
    using Microsoft.Azure.Functions.Worker.Http;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Logging;
    using SportClubsChallenges.Database.Data;
    using SportClubsChallenges.Domain.Interfaces;
    using SportClubsChallenges.Jobs;
    using SportClubsChallenges.Strava;

    public class SyncAllData
    {
        private readonly SportClubsChallengesDbContext db;

        private readonly IActivityService activityService;

        private readonly IStravaApiWrapper stravaWrapper;

        private readonly ITokenService tokenService;

        private readonly IMapper mapper;

        public SyncAllData(SportClubsChallengesDbContext db, IActivityService activityService, IStravaApiWrapper stravaWrapper, ITokenService tokenService, IMapper mapper)
        {
            this.db = db;
            this.activityService = activityService;
            this.stravaWrapper = stravaWrapper;
            this.tokenService = tokenService;
            this.mapper = mapper;
        }

        [Function("SyncAllData")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Admin, "get", "post", Route = null)] HttpRequestData req,
            FunctionContext context)
        {
            var logger = context.GetLogger(nameof(SyncAllData));
            logger.LogInformation("HTTP trigger function {0}.", nameof(SyncAllData));

            var activitiesJob = new GetAthletesActivitiesJob(this.db, this.activityService, this.stravaWrapper, this.tokenService, this.mapper);
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

            return new OkResult();
        }
    }
}