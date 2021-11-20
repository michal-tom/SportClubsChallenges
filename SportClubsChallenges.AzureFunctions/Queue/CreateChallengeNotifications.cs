namespace SportClubsChallenges.AzureFunctions.Queue
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.Azure.Functions.Worker;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Logging;
    using SportClubsChallenges.Database.Data;
    using SportClubsChallenges.Domain.Interfaces;

    public class CreateChallengeNotifications
    {
        private readonly SportClubsChallengesDbContext db;

        private readonly INotificationService notificationService;

        public CreateChallengeNotifications(SportClubsChallengesDbContext db, INotificationService notificationService)
        {
            this.db = db;
            this.notificationService = notificationService;
        }

        [Function("CreateChallengeNotifications")]
        public async Task Run(
            [QueueTrigger("challenges-notifications-create", Connection = "")] string queueItem,
            FunctionContext context)
        {
            var logger = context.GetLogger(nameof(CreateChallengeNotifications));
            logger.LogInformation($"Queue trigger function {nameof(CreateChallengeNotifications)} processed with item: {queueItem}");

            if (string.IsNullOrEmpty(queueItem) || !long.TryParse(queueItem, out long challengeId))
            {
                logger.LogError($"Cannot parse '{queueItem}' to challenge identifier");
                return;
            }

            var challenge = await this.db.Challenges.Include(p => p.Club.ClubMembers).AsNoTracking().FirstOrDefaultAsync(p => p.Id == challengeId);
            if (challenge == null)
            {
                logger.LogWarning($"Challenge with id={challengeId} does not exists.");
                return;
            }

            var clubMembers = challenge.Club.ClubMembers;
            if (!clubMembers.Any())
            {
                return;
            }

            var athletesIds = clubMembers.Select(p => p.AthleteId).ToList();
            await this.notificationService.CreateNewChallengesNotification(challenge.Id, challenge.Name, challenge.Club.Name, athletesIds);
        }
    }
}
