namespace SportClubsChallenges.AzureFunctions.Schedule
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.Azure.Functions.Worker;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Logging;
    using SportClubsChallenges.Database.Data;
    using SportClubsChallenges.Jobs;

    public class UpdateChallenges
    {
        private readonly SportClubsChallengesDbContext db;

        public UpdateChallenges(SportClubsChallengesDbContext db)
        {
            this.db = db;
        }

        [Function("UpdateChallenges")]
        public async Task RunAsync(
            [TimerTrigger("0 0 2 * * *")] TimerInfo myTimer,
            FunctionContext context)
        {
            var logger = context.GetLogger(nameof(UpdateChallenges));
            logger.LogInformation($"Timer trigger function {nameof(UpdateChallenges)} executed at: {DateTime.Now}");

            var activeChallenges = await this.db.Challenges.Where(p => p.IsActive).ToListAsync();
            foreach (var challenge in activeChallenges)
            {
                logger.LogDebug($"Update classification of '{challenge.Name}' challenge");
                var job = new UpdateChallengesClassificationsJob(this.db);
                await job.Run(challenge.Id);
            }

            logger.LogDebug($"Deactivate past challenges");
            var deactivateJob = new DeactivateChallengesJob(this.db);
            await deactivateJob.Run();
        }
    }
}