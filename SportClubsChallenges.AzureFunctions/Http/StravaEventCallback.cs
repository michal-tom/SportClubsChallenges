namespace SportClubsChallenges.AzureFunctions.Http
{
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Azure.Functions.Worker;
    using Microsoft.Azure.Functions.Worker.Http;
    using Microsoft.Extensions.Logging;
    using SportClubsChallenges.AzureFunctions.Consts;

    public static class StravaEventCallback
    {
        [Function("StravaEventCallback")]
        public static async Task<StravaEventCallbackFunctionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = FunctionsConsts.EventsRoute)] HttpRequestData req,
            FunctionContext context)
        {
            var logger = context.GetLogger(nameof(StravaEventCallback));
            logger.LogInformation("HTTP trigger function {0}.", nameof(StravaEventCallback));

            var eventData = await req.ReadAsStringAsync();
            if (string.IsNullOrWhiteSpace(eventData))
            {
                return new StravaEventCallbackFunctionResult
                {
                    Message = string.Empty,
                    HttpReponse = new BadRequestResult()
                };
            }

            logger.LogInformation($"Received event data from Strava subscription:{System.Environment.NewLine}{eventData}" );

            logger.LogInformation("Adding a message to queue 'strava-events'.");

            return new StravaEventCallbackFunctionResult
            {
                Message = eventData,
                HttpReponse = new OkResult()
            };
        }
    }

    public class StravaEventCallbackFunctionResult
    {
        [QueueOutput("strava-events")]
        public string Message { get; set; }

        public IActionResult HttpReponse { get; set; }
    }
}