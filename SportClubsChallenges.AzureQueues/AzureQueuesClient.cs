namespace SportClubsChallenges.AzureQueues
{
    using System.Threading.Tasks;

    public class AzureQueuesClient
    {
        private readonly IAzureStorageRepository storageRepository;

        private const string SyncAthleteActivitiesQueueName = "athletes-activities-sync";
        private const string SyncAthleteClubsQueueName = "athletes-clubs-sync";
        private const string UpdateChallengeRankQueueName = "challenges-rank-update";

        public AzureQueuesClient(IAzureStorageRepository storageRepository)
        {
            this.storageRepository = storageRepository;
        }

        public async Task SyncAthleteActivities(long athleteId)
        {
            await this.storageRepository.CreateMessage(SyncAthleteActivitiesQueueName, athleteId.ToString());
        }

        public async Task SyncAthleteClubs(long athleteId)
        {
            await this.storageRepository.CreateMessage(SyncAthleteClubsQueueName, athleteId.ToString());
        }

        public async Task UpdateChallengeRank(long challengeId)
        {
            await this.storageRepository.CreateMessage(UpdateChallengeRankQueueName, challengeId.ToString());
        }
    }
}