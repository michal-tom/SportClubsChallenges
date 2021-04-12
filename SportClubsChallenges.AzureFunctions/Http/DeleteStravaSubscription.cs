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

    public class DeleteStravaSubscription
    {
        private readonly IStravaSubscriptionService stravaSubscriptionService;

        public DeleteStravaSubscription(IStravaSubscriptionService stravaSubscriptionService)
        {
            this.stravaSubscriptionService = stravaSubscriptionService;
        }

        [FunctionName("DeleteStravaSubscription")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Admin, "get", "post", Route = FunctionsConsts.DeleteSubscriptionRoute)] HttpRequest req,
            ILogger log,
            long id)
        {
            log.LogInformation("HTTP trigger function {0}.", nameof(DeleteStravaSubscription));

            if (id <= 0)
            {
                return new BadRequestObjectResult("Subscription id required.");
            }

            log.LogInformation($"Removing subscription with id: {id}.");

            var responseMessage = await this.stravaSubscriptionService.DeleteSubscription(id);

            return new OkObjectResult(responseMessage);
        }
    }
}