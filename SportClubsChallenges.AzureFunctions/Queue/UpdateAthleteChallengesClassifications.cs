namespace SportClubsChallenges.AzureFunctions.Queue
{
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.Azure.WebJobs;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Logging;
    using Microsoft.WindowsAzure.Storage.Queue;
    using SportClubsChallenges.Database.Data;

    public class UpdateAthleteChallengesClassifications
    {
        private readonly SportClubsChallengesDbContext db;

        public UpdateAthleteChallengesClassifications(SportClubsChallengesDbContext db)
        {
            this.db = db;
        }

        [FunctionName("UpdateAthleteChallengesClassifications")]
        public async Task Run(
            [QueueTrigger("athlete-challenges-update", Connection = "ConnectionStrings:SportClubsChallengeStorage")] string queueItem,
            [Queue("challenges-rank-update")] CloudQueue updateChallengeQueue,
            ILogger log)
        {
            log.LogInformation($"Queue trigger function {nameof(UpdateAthleteChallengesClassifications)} processed with item: {queueItem}");

            if (string.IsNullOrEmpty(queueItem) || !long.TryParse(queueItem, out long athleteId))
            {
                log.LogError($"Cannot parse '{queueItem}' to athlete identifier.");
                return;
            }

            var athlete = await this.db.Athletes.AsNoTracking().FirstOrDefaultAsync(p => p.Id == athleteId);
            if (athlete == null)
            {
                log.LogWarning($"Athlete with id={athleteId} does not exists.");
                return;
            }

            await updateChallengeQueue.CreateIfNotExistsAsync();

            var activeChallenges = this.db.ChallengeParticipants
                .Include(p => p.Challenge)
                .AsNoTracking()
                .Where(p => p.AthleteId == athleteId && p.Challenge.IsActive)
                .Select(p => p.Challenge);

            foreach(var challenge in activeChallenges)
            {
                log.LogInformation($"Queuing update classification of challenge '{challenge.Name}'.");
                await updateChallengeQueue.AddMessageAsync(new CloudQueueMessage(challenge.Id.ToString()));
            }
        }
    }
}
