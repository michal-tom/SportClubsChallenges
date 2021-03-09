namespace SportClubsChallenges.AzureFunctions.Queue
{
    using System.Threading.Tasks;
    using Microsoft.Azure.WebJobs;
    using Microsoft.Extensions.Logging;
    using SportClubsChallenges.Database.Data;
    using SportClubsChallenges.Jobs;

    public class UpdateChallengeClassification
    {
        private readonly SportClubsChallengesDbContext db;

        public UpdateChallengeClassification(SportClubsChallengesDbContext db)
        {
            this.db = db;
        }

        [FunctionName("UpdateChallengeClassification")]
        public async Task Run(
            [QueueTrigger("challenges-rank-update", Connection = "ConnectionStrings:SportClubsChallengeStorage")] string queueItem,
            ILogger log)
        {
            log.LogInformation($"Queue trigger function {nameof(UpdateChallengeClassification)} processed with item: {queueItem}");

            if (string.IsNullOrEmpty(queueItem) || !long.TryParse(queueItem, out long challengeId))
            {
                log.LogError($"Cannot parse '{queueItem}' to challenge identifier");
                return;
            }

            var job = new UpdateChallengesClassificationsJob(this.db);
            await job.Run(challengeId);
        }
    }
}