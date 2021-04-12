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

        public CreateStravaSubscription(IStravaSubscriptionService stravaSubscriptionService)
        {
            this.stravaSubscriptionService = stravaSubscriptionService;
        }

        [FunctionName("CreateStravaSubscription")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Admin, "get", "post", Route = FunctionsConsts.CreateSubscriptionRoute)] HttpRequest req,
            ILogger log, 
            ConfigurationRoot configuration)
        {
            log.LogInformation("HTTP trigger function {0}.", nameof(CreateStravaSubscription));

            var hostname = configuration["SportClubsChallengeAzureFunctionsUrl"];
            var callbackUrl = $"{hostname}/{FunctionsConsts.WebhooksRoute}";

            log.LogInformation($"Creating subscription for callback url: {callbackUrl}.");

            await stravaSubscriptionService.CreateSubscription(callbackUrl, FunctionsConsts.SubscriptionCallbackToken);

            return new OkResult();
        }
    }
}