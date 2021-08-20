namespace SportClubsChallenges.AzureFunctions.Http
{
    using System.Web;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Azure.Functions.Worker;
    using Microsoft.Azure.Functions.Worker.Http;
    using Microsoft.Extensions.Logging;
    using Newtonsoft.Json;
    using SportClubsChallenges.AzureFunctions.Consts;

    public static class ValidateStravaSubscriptionCallback
    {
        [Function("ValidateStravaSubscriptionCallback")]
        public static IActionResult Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = FunctionsConsts.EventsRoute)] HttpRequestData req,
            FunctionContext context)
        {
            var logger = context.GetLogger(nameof(ValidateStravaSubscriptionCallback));
            logger.LogInformation("HTTP trigger function {0}.", nameof(ValidateStravaSubscriptionCallback));

            var query = HttpUtility.ParseQueryString(req.Url.Query);
            var token = query["hub.verify_token"];
            var challenge = query["hub.challenge"];

            if (string.IsNullOrEmpty(token) || string.IsNullOrEmpty(challenge))
            {
                logger.LogError($"Received callback token or challenge is empty.");
                return new BadRequestResult();
            }

            if (!token.Equals(FunctionsConsts.SubscriptionCallbackToken))
            {
                logger.LogError($"Received callback token '{token}' is not valid with expected '{FunctionsConsts.SubscriptionCallbackToken}'.");
                return new BadRequestResult();
            }

            logger.LogInformation("Request validated.");

            return new JsonResult(new ValidateCallbackResponse { HubChallenge = challenge });
        }

        private class ValidateCallbackResponse
        {
            [JsonProperty("hub.challenge")]
            public string HubChallenge { get; set; }
        }
    }
}