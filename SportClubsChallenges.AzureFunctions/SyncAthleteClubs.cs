namespace SportClubsChallenges.AzureFunctions
{
    using AutoMapper;
    using Microsoft.Azure.WebJobs;
    using Microsoft.Extensions.Logging;
    using SportClubsChallenges.Database.Data;
    using SportClubsChallenges.Domain.Interfaces;
    using SportClubsChallenges.Jobs.Clubs;
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
        public async Task Run([QueueTrigger("athletes-clubs-sync", Connection = "ConnectionStrings:AthletesClubsSyncQueue")]string myQueueItem, ILogger log)
        {
            log.LogInformation($"Queue trigger function {nameof(SyncAthleteActivities)} processed with item: {myQueueItem}");

            if (string.IsNullOrEmpty(myQueueItem) || !long.TryParse(myQueueItem, out long athleteId))
            {
                log.LogError($"Cannot parse '{myQueueItem}' to athlete identifier");
                return;
            }

            var job = new GetAthleteClubsJob(this.db, this.stravaWrapper, this.tokenService, this.mapper);
            await job.Run(athleteId);
        }
    }
}
