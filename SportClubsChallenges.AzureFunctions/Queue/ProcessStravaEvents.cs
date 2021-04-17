namespace SportClubsChallenges.AzureFunctions
{
    using System;
    using System.Threading.Tasks;
    using AutoMapper;
    using Microsoft.Azure.WebJobs;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Logging;
    using Microsoft.WindowsAzure.Storage.Queue;
    using SportClubsChallenges.Database.Data;
    using SportClubsChallenges.Database.Entities;
    using SportClubsChallenges.Domain.Interfaces;
    using SportClubsChallenges.Model.Strava;
    using SportClubsChallenges.Strava;
    using SportClubsChallenges.Utils.Helpers;

    public class ProcessStravaEvents
    {
        private readonly SportClubsChallengesDbContext db;

        private readonly IActivityService activityService;

        private readonly IStravaApiWrapper stravaWrapper;

        private readonly ITokenService tokenService;

        private readonly IMapper mapper;

        public ProcessStravaEvents(SportClubsChallengesDbContext db, IActivityService activityService, IStravaApiWrapper stravaWrapper, ITokenService tokenService, IMapper mapper)
        {
            this.db = db;
            this.activityService = activityService;
            this.stravaWrapper = stravaWrapper;
            this.tokenService = tokenService;
            this.mapper = mapper;
        }

        [FunctionName("ProcessStravaEvents")]
        public async Task Run(
            [QueueTrigger("strava-events", Connection = "ConnectionStrings:SportClubsChallengeStorage")] string queueItem,
            [Queue("athlete-challenges-update")] CloudQueue updateAthleteChallengesQueue,
            ILogger log)
        {
            log.LogInformation($"Queue trigger function {nameof(ProcessStravaEvents)} processed with item: {queueItem}");

            var stravaEvent = JsonHelper.Deserialize<StravaEvent>(queueItem);
            if (stravaEvent == null || stravaEvent.ObjectId == default(long) || stravaEvent.AthleteId == default(long))
            {
                log.LogWarning($"Received an unrecognized event from Strava: {queueItem}.");
                return;
            }

            var athlete = await this.db.Athletes.AsNoTracking().Include(p => p.AthleteStravaToken).FirstOrDefaultAsync(p => p.Id == stravaEvent.AthleteId);
            if (athlete == null)
            {
                log.LogWarning($"Athlete with id={stravaEvent.AthleteId} does not exists.");
                return;
            }

            switch (stravaEvent.ObjectType)
            {
                case ObjectType.Activity:
                    await this.ProcessStravaActivityEvent(stravaEvent, athlete, log);
                    break;
                case ObjectType.Athlete:
                    // TODO: Deauthorize athlete
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(stravaEvent.ObjectType), "Unsupported type of Strava event object");
            }

            log.LogInformation($"Queuing update classification of athlete {athlete.FirstName} {athlete.LastName}.");
            await updateAthleteChallengesQueue.CreateIfNotExistsAsync();
            await updateAthleteChallengesQueue.AddMessageAsync(new CloudQueueMessage(stravaEvent.AthleteId.ToString()));
        }

        private async Task ProcessStravaActivityEvent(StravaEvent stravaEvent, Athlete athlete, ILogger log)
        {
            switch (stravaEvent.ActionType)
            {
                case ActionType.Create:
                    log.LogInformation($"Adding activity with id={stravaEvent.ObjectId} for athlete id={stravaEvent.AthleteId}.");
                    await this.AddActivity(stravaEvent, athlete, log);
                    break;
                case ActionType.Update:
                    log.LogInformation($"Updating activity with id={stravaEvent.ObjectId} for athlete id={stravaEvent.AthleteId}.");
                    await this.UpdateActivity(stravaEvent, log);
                    break;
                case ActionType.Delete:
                    log.LogInformation($"Removing activity with id={stravaEvent.ObjectId} for athlete id={stravaEvent.AthleteId}.");
                    await this.DeleteActivity(stravaEvent, log);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(stravaEvent.ActionType), "Unsupported type of Strava activity aspect type");
            }
        }

        private async Task AddActivity(StravaEvent stravaEvent, Athlete athlete, ILogger log)
        {
            var stravaToken = this.tokenService.GetStravaToken(athlete);

            var stravaActivity = await this.stravaWrapper.GetActivity(stravaToken, stravaEvent.ObjectId);
            if (stravaActivity == null)
            {
                log.LogWarning($"Activity with id={stravaEvent.ObjectId} does not exists or is private.");
                return;
            }

            var activity = this.mapper.Map<Activity>(stravaActivity);
            await this.activityService.AddActivity(activity);

            log.LogInformation($"Activity with id={stravaEvent.ObjectId} was successfully processed.");
        }

        private async Task UpdateActivity(StravaEvent stravaEvent, ILogger log)
        {
            if (stravaEvent.Updates == null || (string.IsNullOrEmpty(stravaEvent.Updates.Title) && string.IsNullOrEmpty(stravaEvent.Updates.Type) && !stravaEvent.Updates.Private.HasValue))
            {
                log.LogWarning($"Activity with id={stravaEvent.ObjectId} was not updated.");
                return;
            }
            
            await this.activityService.UpdateActivity(stravaEvent.AthleteId, stravaEvent.ObjectId, stravaEvent.Updates.Title, stravaEvent.Updates.Type, stravaEvent.Updates.Private);

            log.LogInformation($"Activity with id={stravaEvent.ObjectId} was successfully updated.");
        }

        private async Task DeleteActivity(StravaEvent stravaEvent, ILogger log)
        {
            await this.activityService.DeleteActivity(stravaEvent.AthleteId, stravaEvent.ObjectId);

            log.LogInformation($"Activity with id={stravaEvent.ObjectId} was successfully deleted.");
        }
    }
}
