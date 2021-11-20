namespace SportClubsChallenges.AzureFunctions.Queue
{
    using System.Threading.Tasks;
    using AutoMapper;
    using Microsoft.Azure.Functions.Worker;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Logging;
    using SportClubsChallenges.Database.Data;
    using SportClubsChallenges.Domain.Interfaces;
    using SportClubsChallenges.Jobs;
    using SportClubsChallenges.Strava;

    public class SyncAthleteClubs
    {
        private readonly SportClubsChallengesDbContext db;

        private readonly IStravaApiWrapper stravaWrapper;

        private readonly ITokenService tokenService;

        private readonly IMapper mapper;

        public SyncAthleteClubs(SportClubsChallengesDbContext db, IStravaApiWrapper stravaWrapper, ITokenService tokenService, IMapper mapper)
        {
            this.db = db;
            this.stravaWrapper = stravaWrapper;
            this.tokenService = tokenService;
            this.mapper = mapper;
        }

        [Function("SyncAthleteClubs")]
        public async Task Run(
            [QueueTrigger("athletes-clubs-sync")] string queueItem,
            FunctionContext context)
        {
            var logger = context.GetLogger(nameof(SyncAthleteClubs));
            logger.LogInformation($"Queue trigger function {nameof(SyncAthleteClubs)} processed with item: {queueItem}");

            if (string.IsNullOrEmpty(queueItem) || !long.TryParse(queueItem, out long athleteId))
            {
                logger.LogError($"Cannot parse '{queueItem}' to athlete identifier");
                return;
            }

            var athlete = await this.db.Athletes.AsNoTracking().FirstOrDefaultAsync(p => p.Id == athleteId);
            if (athlete == null)
            {
                logger.LogWarning($"Athlete with id={athleteId} does not exists.");
                return;
            }

            var job = new GetAthletesClubsJob(this.db, this.stravaWrapper, this.tokenService, this.mapper);
            await job.Run(athleteId);
        }
    }
}