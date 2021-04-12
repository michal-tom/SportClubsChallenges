namespace SportClubsChallenges.AzureFunctions.Http
{
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Azure.WebJobs;
    using Microsoft.Azure.WebJobs.Extensions.Http;
    using Microsoft.AspNetCore.Http;
    using Microsoft.Extensions.Logging;
    using Newtonsoft.Json;
    using SportClubsChallenges.AzureFunctions.Consts;

    public static class ValidateStravaSubscriptionCallback
    {
        [FunctionName("ValidateStravaSubscriptionCallback")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = FunctionsConsts.WebhooksRoute)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("HTTP trigger function {0}.", nameof(ValidateStravaSubscriptionCallback));

            string token = req.Query["hub.verify_token"];
            string challenge = req.Query["hub.challenge"];

            if (string.IsNullOrEmpty(token) || string.IsNullOrEmpty(challenge))
            {
                log.LogError($"Received callback token or challenge is empty.");
                return new BadRequestResult();
            }

            if (!token.Equals(FunctionsConsts.SubscriptionCallbackToken))
            {
                log.LogError($"Received callback token '{token}' is not valid with expected '{FunctionsConsts.SubscriptionCallbackToken}'.");
                return new BadRequestResult();
            }

            log.LogInformation("Request validated.");

            return new JsonResult(new ValidateCallbackResponse { HubChallenge = challenge });
        }

        private class ValidateCallbackResponse
        {
            [JsonProperty("hub.challenge")]
            public string HubChallenge { get; set; }
        }
    }
}
