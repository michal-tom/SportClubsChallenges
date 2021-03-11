namespace SportClubsChallenges.AzureQueues
{
    using System.Threading.Tasks;

    public interface IAzureStorageRepository
    {
        Task CreateMessage(string queueName, string message);
    }
}
