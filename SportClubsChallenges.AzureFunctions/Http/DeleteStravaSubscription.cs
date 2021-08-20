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

    public class DeleteStravaSubscription
    {
        private readonly IStravaSubscriptionService stravaSubscriptionService;

        private readonly string ClientId;

        private readonly string ClientSecret;

        public DeleteStravaSubscription(IStravaSubscriptionService stravaSubscriptionService, IConfiguration configuration)
        {
            this.stravaSubscriptionService = stravaSubscriptionService;
            this.ClientId = configuration["StravaClientId"];
            this.ClientSecret = configuration["StravaClientSecret"];
        }

        [Function("DeleteStravaSubscription")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Admin, "get", "post", Route = FunctionsConsts.DeleteSubscriptionRoute)] HttpRequestData req,
            FunctionContext context,
            long id)
        {
            var logger = context.GetLogger(nameof(DeleteStravaSubscription));
            logger.LogInformation("HTTP trigger function {0}.", nameof(DeleteStravaSubscription));

            if (id <= 0)
            {
                return new BadRequestObjectResult("Subscription id required.");
            }

            logger.LogInformation($"Removing subscription with id: {id}.");

            var responseMessage = await this.stravaSubscriptionService.DeleteSubscription(id, this.ClientId, this.ClientSecret);

            return new OkObjectResult(responseMessage);
        }
    }
}