namespace SportClubsChallenges.AzureFunctions.Queue
{
    using System.Threading.Tasks;
    using Microsoft.Azure.WebJobs;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Logging;
    using Microsoft.WindowsAzure.Storage.Queue;
    using AutoMapper;
    using SportClubsChallenges.Database.Data;
    using SportClubsChallenges.Domain.Interfaces;
    using SportClubsChallenges.Jobs;
    using SportClubsChallenges.Strava;

    public class SyncAthleteActivities
    {
        private readonly SportClubsChallengesDbContext db;

        private readonly IActivityService activityService;

        private readonly IStravaApiWrapper stravaWrapper;

        private readonly ITokenService tokenService;

        private readonly IMapper mapper;

        public SyncAthleteActivities(SportClubsChallengesDbContext db, IActivityService activityService, IStravaApiWrapper stravaWrapper, ITokenService tokenService, IMapper mapper)
        {
            this.db = db;
            this.activityService = activityService;
            this.stravaWrapper = stravaWrapper;
            this.tokenService = tokenService;
            this.mapper = mapper;
        }

        [FunctionName("SyncAthleteActivities")]
        public async Task Run(
            [QueueTrigger("athletes-activities-sync", Connection = "ConnectionStrings:SportClubsChallengeStorage")] string queueItem,
            [Queue("athlete-challenges-update")] CloudQueue updateAthleteChallengesQueue,
            ILogger log)
        {
            log.LogInformation($"Queue trigger function {nameof(SyncAthleteActivities)} processed with item: {queueItem}");

            if (string.IsNullOrEmpty(queueItem) || !long.TryParse(queueItem, out long athleteId))
            {
                log.LogError($"Cannot parse '{queueItem}' to athlete identifier");
                return;
            }

            var athlete = await this.db.Athletes.AsNoTracking().FirstOrDefaultAsync(p => p.Id == athleteId);
            if (athlete == null)
            {
                log.LogWarning($"Athlete with id={athleteId} does not exists.");
                return;
            }

            var job = new GetAthletesActivitiesJob(this.db, this.activityService, this.stravaWrapper, this.tokenService, this.mapper);
            await job.Run(athleteId);

            log.LogInformation($"Queuing update classification of athlete {athlete.FirstName} {athlete.LastName}.");
            await updateAthleteChallengesQueue.CreateIfNotExistsAsync();
            await updateAthleteChallengesQueue.AddMessageAsync(new CloudQueueMessage(athleteId.ToString()));
        }
    }
}