namespace SportClubsChallenges.AzureFunctions.Queue
{
    using System.Threading.Tasks;
    using Microsoft.Azure.Functions.Worker;
    using Microsoft.EntityFrameworkCore;
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

        [Function("UpdateChallengeClassification")]
        public async Task Run(
            [QueueTrigger("challenges-rank-update")] string queueItem,
            FunctionContext context)
        {
            var logger = context.GetLogger(nameof(UpdateChallengeClassification));
            logger.LogInformation($"Queue trigger function {nameof(UpdateChallengeClassification)} processed with item: {queueItem}");

            if (string.IsNullOrEmpty(queueItem) || !long.TryParse(queueItem, out long challengeId))
            {
                logger.LogError($"Cannot parse '{queueItem}' to challenge identifier.");
                return;
            }

            var challenge = await this.db.Challenges.AsNoTracking().FirstOrDefaultAsync(p => p.Id == challengeId);
            if (challenge == null)
            {
                logger.LogWarning($"Challenge with id={challengeId} does not exists.");
                return;
            }

            logger.LogInformation($"Updating classification of challenge '{challenge.Name}'");

            var job = new UpdateChallengesClassificationsJob(this.db);
            await job.Run(challengeId);
        }
    }
}