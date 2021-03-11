
namespace SportClubsChallenges.AzureQueues
{
    using Azure.Storage.Queues;
    using Microsoft.Extensions.Configuration;
    using System.Threading.Tasks;

    public class AzureStorageRepository : IAzureStorageRepository
    {
        private readonly string connectionString;

        public AzureStorageRepository(IConfiguration configuration)
        {
            this.connectionString = configuration["ConnectionStrings:SportClubsChallengeStorage"];
        }

        public async Task CreateMessage(string queueName, string message)
        {
            if (string.IsNullOrEmpty(this.connectionString))
            {
                return;
            }
            
            QueueClient queue = new QueueClient(this.connectionString, queueName, new QueueClientOptions { MessageEncoding = QueueMessageEncoding.Base64 });
            await queue.CreateIfNotExistsAsync();

            await queue.SendMessageAsync(message);
        }
    }
}
