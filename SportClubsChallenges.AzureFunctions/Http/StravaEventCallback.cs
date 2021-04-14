namespace SportClubsChallenges.AzureFunctions.Http
{
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Azure.WebJobs;
    using Microsoft.Azure.WebJobs.Extensions.Http;
    using Microsoft.AspNetCore.Http;
    using Microsoft.Extensions.Logging;
    using SportClubsChallenges.AzureFunctions.Consts;
    using Microsoft.WindowsAzure.Storage.Queue;

    public static class StravaEventCallback
    {
        [FunctionName("StravaEventCallback")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = FunctionsConsts.EventsRoute)] HttpRequest req,
            ILogger log,
            [Queue("strava-events")] CloudQueue outputQueue)
        {
            log.LogInformation("HTTP trigger function {0}.", nameof(StravaEventCallback));

            var eventData = await req.ReadAsStringAsync();
            if (string.IsNullOrWhiteSpace(eventData))
            {
                return new BadRequestResult();
            }

            log.LogInformation($"Received event data from Strava subscription:{System.Environment.NewLine}{eventData}" );

            await outputQueue.AddMessageAsync(new CloudQueueMessage(eventData));
            log.LogInformation("Added a message to queue 'strava-events'.");
            return new OkResult();
        }
    }
}