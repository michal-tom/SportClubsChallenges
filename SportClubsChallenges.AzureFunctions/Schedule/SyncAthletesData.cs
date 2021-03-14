namespace SportClubsChallenges.AzureFunctions.Schedule
{
    using System;
    using Microsoft.Azure.WebJobs;
    using Microsoft.Extensions.Logging;
    using AutoMapper;
    using SportClubsChallenges.Database.Data;
    using SportClubsChallenges.Domain.Interfaces;
    using SportClubsChallenges.Jobs;
    using SportClubsChallenges.Strava;

    public class SyncAthletesData
    {
        private readonly SportClubsChallengesDbContext db;

        private readonly IStravaApiWrapper stravaWrapper;

        private readonly ITokenService tokenService;

        private readonly IMapper mapper;

        public SyncAthletesData(SportClubsChallengesDbContext db, IStravaApiWrapper stravaWrapper, ITokenService tokenService, IMapper mapper)
        {
            this.db = db;
            this.stravaWrapper = stravaWrapper;
            this.tokenService = tokenService;
            this.mapper = mapper;
        }

        [FunctionName("SyncAthletesData")]
        public async void Run([TimerTrigger("0 0 1 * * *")]TimerInfo myTimer, ILogger log)
        {
            log.LogInformation($"Timer trigger function {nameof(SyncAthletesData)} executed at: {DateTime.Now}");

            var job = new GetAthletesActivitiesJob(this.db, this.stravaWrapper, this.tokenService, this.mapper);
            await job.Run();
        }
    }
}