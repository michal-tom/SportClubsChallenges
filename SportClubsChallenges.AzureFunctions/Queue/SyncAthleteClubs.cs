namespace SportClubsChallenges.AzureFunctions.Queue
{
    using AutoMapper;
    using Microsoft.Azure.WebJobs;
    using Microsoft.Extensions.Logging;
    using SportClubsChallenges.Database.Data;
    using SportClubsChallenges.Domain.Interfaces;
    using SportClubsChallenges.Jobs;
    using SportClubsChallenges.Strava;
    using System.Threading.Tasks;

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

        [FunctionName("SyncAthleteClubs")]
        public async Task Run(
            [QueueTrigger("athletes-clubs-sync", Connection = "ConnectionStrings:SportClubsChallengeStorage")] string queueItem,
            ILogger log)
        {
            log.LogInformation($"Queue trigger function {nameof(SyncAthleteActivities)} processed with item: {queueItem}");

            if (string.IsNullOrEmpty(queueItem) || !long.TryParse(queueItem, out long athleteId))
            {
                log.LogError($"Cannot parse '{queueItem}' to athlete identifier");
                return;
            }

            var job = new GetAthletesClubsJob(this.db, this.stravaWrapper, this.tokenService, this.mapper);
            await job.Run(athleteId);
        }
    }
}