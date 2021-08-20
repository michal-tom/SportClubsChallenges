namespace SportClubsChallenges.AzureFunctions.Http
{
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Azure.Functions.Worker;
    using Microsoft.Azure.Functions.Worker.Http;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Logging;
    using SportClubsChallenges.AzureFunctions.Consts;
    using SportClubsChallenges.Strava;

    public class CreateStravaSubscription
    {
        private readonly IStravaSubscriptionService stravaSubscriptionService;

        private readonly string HostName;

        private readonly string ClientId;

        private readonly string ClientSecret;

        public CreateStravaSubscription(IStravaSubscriptionService stravaSubscriptionService, IConfiguration configuration)
        {
            this.stravaSubscriptionService = stravaSubscriptionService;
            this.HostName = configuration["SportClubsChallengeAzureFunctionsUrl"];
            this.ClientId = configuration["StravaClientId"];
            this.ClientSecret = configuration["StravaClientSecret"];
        }

        [Function("CreateStravaSubscription")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Admin, "get", "post", Route = FunctionsConsts.CreateSubscriptionRoute)] HttpRequestData req,
            FunctionContext context)
        {
            var logger = context.GetLogger(nameof(CreateStravaSubscription));
            logger.LogInformation("HTTP trigger function {0}.", nameof(CreateStravaSubscription));

            var callbackUrl = $"{this.HostName}/api/{FunctionsConsts.EventsRoute}";

            logger.LogInformation($"Creating subscription for callback url: {callbackUrl}.");

            var responseMessage = await this.stravaSubscriptionService.CreateSubscription(callbackUrl, FunctionsConsts.SubscriptionCallbackToken, this.ClientId, this.ClientSecret);

            return new OkObjectResult(responseMessage);
        }
    }
}