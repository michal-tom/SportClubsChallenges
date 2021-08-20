namespace SportClubsChallenges.AzureFunctions.Schedule
{
    using System;
    using System.Threading.Tasks;
    using Microsoft.Azure.Functions.Worker;
    using Microsoft.Extensions.Logging;
    using AutoMapper;
    using SportClubsChallenges.Database.Data;
    using SportClubsChallenges.Domain.Interfaces;
    using SportClubsChallenges.Jobs;
    using SportClubsChallenges.Strava;

    public class SyncAthletesData
    {
        private readonly SportClubsChallengesDbContext db;

        private readonly IActivityService activityService;

        private readonly IStravaApiWrapper stravaWrapper;

        private readonly ITokenService tokenService;

        private readonly IMapper mapper;

        public SyncAthletesData(SportClubsChallengesDbContext db, IActivityService activityService, IStravaApiWrapper stravaWrapper, ITokenService tokenService, IMapper mapper)
        {
            this.db = db;
            this.activityService = activityService;
            this.stravaWrapper = stravaWrapper;
            this.tokenService = tokenService;
            this.mapper = mapper;
        }

        [Function("SyncAthletesData")]
        public async Task Run(
            [TimerTrigger("0 0 1 * * *")]TimerInfo myTimer,
            FunctionContext context)
        {
            var logger = context.GetLogger(nameof(SyncAthletesData));
            logger.LogInformation($"Timer trigger function {nameof(SyncAthletesData)} executed at: {DateTime.Now}");

            var job = new GetAthletesActivitiesJob(this.db, this.activityService, this.stravaWrapper, this.tokenService, this.mapper);
            await job.Run();
        }
    }
}