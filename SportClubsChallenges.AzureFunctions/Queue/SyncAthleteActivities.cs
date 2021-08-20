namespace SportClubsChallenges.AzureFunctions.Queue
{
    using System.Threading.Tasks;
    using Microsoft.Azure.Functions.Worker;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Logging;
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

        [Function("SyncAthleteActivities")]
        [QueueOutput("athlete-challenges-update")]
        public async Task<string> Run(
            [QueueTrigger("athletes-activities-sync")] string queueItem,
            FunctionContext context)
        {
            var logger = context.GetLogger(nameof(SyncAthleteActivities));
            logger.LogInformation($"Queue trigger function {nameof(SyncAthleteActivities)} processed with item: {queueItem}");

            if (string.IsNullOrEmpty(queueItem) || !long.TryParse(queueItem, out long athleteId))
            {
                logger.LogError($"Cannot parse '{queueItem}' to athlete identifier.");
                return null;
            }

            var athlete = await this.db.Athletes.AsNoTracking().FirstOrDefaultAsync(p => p.Id == athleteId);
            if (athlete == null)
            {
                logger.LogWarning($"Athlete with id={athleteId} does not exists.");
                return null;
            }

            var job = new GetAthletesActivitiesJob(this.db, this.activityService, this.stravaWrapper, this.tokenService, this.mapper);
            await job.Run(athleteId);

            logger.LogInformation($"Queuing update classification of athlete {athlete.FirstName} {athlete.LastName}.");
            return athleteId.ToString();
        }
    }
}