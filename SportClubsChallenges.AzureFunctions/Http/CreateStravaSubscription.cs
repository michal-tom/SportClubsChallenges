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

    public class CreateStravaSubscription
    {
        private readonly IStravaSubscriptionService stravaSubscriptionService;
        private readonly string hostname;

        public CreateStravaSubscription(IStravaSubscriptionService stravaSubscriptionService, IConfiguration configuration)
        {
            this.stravaSubscriptionService = stravaSubscriptionService;
            this.hostname = configuration["SportClubsChallengeAzureFunctionsUrl"];
        }

        [FunctionName("CreateStravaSubscription")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Admin, "get", "post", Route = FunctionsConsts.CreateSubscriptionRoute)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("HTTP trigger function {0}.", nameof(CreateStravaSubscription));

            var callbackUrl = $"{this.hostname}/api/{FunctionsConsts.WebhooksRoute}";

            log.LogInformation($"Creating subscription for callback url: {callbackUrl}.");

            var responseMessage = await this.stravaSubscriptionService.CreateSubscription(callbackUrl, FunctionsConsts.SubscriptionCallbackToken);

            return new OkObjectResult(responseMessage);
        }
    }
}