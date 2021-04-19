namespace SportClubsChallenges.AzureFunctions.Http
{
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Azure.WebJobs;
    using Microsoft.Azure.WebJobs.Extensions.Http;
    using Microsoft.AspNetCore.Http;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Logging;
    using SportClubsChallenges.AzureFunctions.Consts;
    using SportClubsChallenges.Strava;

    public class ViewStravaSubscription
    {
        private readonly IStravaSubscriptionService stravaSubscriptionService;

        private readonly string ClientId;

        private readonly string ClientSecret;

        public ViewStravaSubscription(IStravaSubscriptionService stravaSubscriptionService, IConfiguration configuration)
        {
            this.stravaSubscriptionService = stravaSubscriptionService;
            this.ClientId = configuration["StravaClientId"];
            this.ClientSecret = configuration["StravaClientSecret"];
        }

        [FunctionName("ViewStravaSubscription")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Admin, "get", "post", Route = FunctionsConsts.ViewSubscriptionRoute)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("HTTP trigger function {0}.", nameof(ViewStravaSubscription));

            var responseMessage = await this.stravaSubscriptionService.ViewSubscription(this.ClientId, this.ClientSecret);
            
            return new OkObjectResult(responseMessage);
        }
    }
}