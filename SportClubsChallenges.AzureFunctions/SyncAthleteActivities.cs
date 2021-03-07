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
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation($"C# HTTP trigger function {nameof(SyncAthleteActivities)}");

            string id = req.Query["id"];

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);
            id = id ?? data?.id;

            if (string.IsNullOrEmpty(id) || !long.TryParse(id, out long athleteId))
            {
                return new NotFoundResult();
            }

            var job = new GetSpecifiedAthleteActivitiesJob(this.db, this.stravaWrapper, this.tokenService, this.mapper);
            await job.Run(athleteId);

            return new OkObjectResult("OK");
        }
    }
}
