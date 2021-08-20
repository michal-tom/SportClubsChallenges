namespace SportClubsChallenges.AzureFunctions.Queue
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.Azure.Functions.Worker;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Logging;
    using SportClubsChallenges.Database.Data;

    public class UpdateAthleteChallengesClassifications
    {
        private readonly SportClubsChallengesDbContext db;

        public UpdateAthleteChallengesClassifications(SportClubsChallengesDbContext db)
        {
            this.db = db;
        }

        [Function("UpdateAthleteChallengesClassifications")]
        [QueueOutput("challenges-rank-update")]
        public async Task<IEnumerable<string>> Run(
            [QueueTrigger("athlete-challenges-update")] string queueItem,
            FunctionContext context)
        {
            var logger = context.GetLogger(nameof(UpdateAthleteChallengesClassifications));
            logger.LogInformation($"Queue trigger function {nameof(UpdateAthleteChallengesClassifications)} processed with item: {queueItem}");

            if (string.IsNullOrEmpty(queueItem) || !long.TryParse(queueItem, out long athleteId))
            {
                logger.LogError($"Cannot parse '{queueItem}' to athlete identifier.");
                return null;
            }

            var athlete = await this.db.Athletes.AsNoTracking().FirstOrDefaultAsync(p => p.Id == athleteId);
            if (athlete == null)
            {
                logger.LogWarning($"Athlete with id={athleteId} does not exists.");
                return null;
            }

            var activeChallengesIds = this.db.ChallengeParticipants
                .Include(p => p.Challenge)
                .AsNoTracking()
                .Where(p => p.AthleteId == athleteId && p.Challenge.IsActive)
                .Select(p => p.Challenge.Id);

            return activeChallengesIds.Select(p => p.ToString());
        }
    }
}
