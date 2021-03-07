namespace SportClubsChallenges.AzureFunctions
{
    using System.IO;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Azure.WebJobs;
    using Microsoft.Azure.WebJobs.Extensions.Http;
    using Microsoft.AspNetCore.Http;
    using Microsoft.Extensions.Logging;
    using Newtonsoft.Json;
    using SportClubsChallenges.Database.Data;
    using SportClubsChallenges.Strava;
    using SportClubsChallenges.Domain.Interfaces;
    using AutoMapper;
    using SportClubsChallenges.Jobs;

    public class SyncStravaActivities
    {
        private readonly SportClubsChallengesDbContext db;

        private readonly IStravaApiWrapper stravaWrapper;

        private readonly ITokenService tokenService;

        private readonly IMapper mapper;

        public SyncStravaActivities(SportClubsChallengesDbContext db, IStravaApiWrapper stravaWrapper, ITokenService tokenService, IMapper mapper)
        {
            this.db = db;
            this.stravaWrapper = stravaWrapper;
            this.tokenService = tokenService;
            this.mapper = mapper;
        }

        // TODO: change authorization level
        [FunctionName("ImportStravaActivities")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation($"C# HTTP trigger function {nameof(SyncStravaActivities)}");

            var job = new GetActiveAthletesActivitiesJob(this.db, this.stravaWrapper, this.tokenService, this.mapper);
            await job.Run();

            return new OkObjectResult("OK");
        }
    }
}
