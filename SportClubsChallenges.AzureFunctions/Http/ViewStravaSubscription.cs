namespace SportClubsChallenges.AzureFunctions.Http
{
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Azure.WebJobs;
    using Microsoft.Azure.WebJobs.Extensions.Http;
    using Microsoft.AspNetCore.Http;
    using Microsoft.Extensions.Logging;
    using SportClubsChallenges.AzureFunctions.Consts;
    using SportClubsChallenges.Strava;

    public class ViewStravaSubscription
    {
        private readonly IStravaSubscriptionService stravaSubscriptionService;

        public ViewStravaSubscription(IStravaSubscriptionService stravaSubscriptionService)
        {
            this.stravaSubscriptionService = stravaSubscriptionService;
        }

        [FunctionName("ViewStravaSubscription")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Admin, "get", "post", Route = FunctionsConsts.ViewSubscriptionRoute)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("HTTP trigger function {0}.", nameof(ViewStravaSubscription));

            var responseMessage = await this.stravaSubscriptionService.ViewSubscription();
            
            return new OkObjectResult(responseMessage);
        }
    }
}