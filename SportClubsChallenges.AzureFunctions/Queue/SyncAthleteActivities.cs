namespace SportClubsChallenges.AzureFunctions.Queue
{
    using System.Threading.Tasks;
    using Microsoft.Azure.WebJobs;
    using Microsoft.Extensions.Logging;
    using AutoMapper;
    using SportClubsChallenges.Database.Data;
    using SportClubsChallenges.Domain.Interfaces;
    using SportClubsChallenges.Jobs;
    using SportClubsChallenges.Strava;

    public class SyncAthleteActivities
    {
        private readonly SportClubsChallengesDbContext db;

        private readonly IStravaApiWrapper stravaWrapper;

        private readonly ITokenService tokenService;

        private readonly IMapper mapper;

        public SyncAthleteActivities(SportClubsChallengesDbContext db, IStravaApiWrapper stravaWrapper, ITokenService tokenService, IMapper mapper)
        {
            this.db = db;
            this.stravaWrapper = stravaWrapper;
            this.tokenService = tokenService;
            this.mapper = mapper;
        }

        [FunctionName("SyncAthleteActivities")]
        public async Task Run(
            [QueueTrigger("athletes-activities-sync", Connection = "ConnectionStrings:SportClubsChallengeStorage")] string queueItem,
            ILogger log)
        {
            log.LogInformation($"C# HTTP trigger function {nameof(SyncAthleteActivities)}");

            if (string.IsNullOrEmpty(queueItem) || !long.TryParse(queueItem, out long athleteId))
            {
                log.LogError($"Cannot parse '{queueItem}' to athlete identifier");
                return;
            }

            var job = new GetAthletesActivitiesJob(this.db, this.stravaWrapper, this.tokenService, this.mapper);
            await job.Run(athleteId);
        }
    }
}
