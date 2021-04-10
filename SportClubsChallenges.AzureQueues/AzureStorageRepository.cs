namespace SportClubsChallenges.AzureQueues
{
    using System.Net;
    using System.Threading.Tasks;
    using Azure.Storage.Queues;
    using Microsoft.Extensions.Configuration;

    public class AzureStorageRepository : IAzureStorageRepository
    {
        private readonly string connectionString;
        private readonly string azureFunctionsUrl;

        public AzureStorageRepository(IConfiguration configuration)
        {
            this.connectionString = configuration["ConnectionStrings:SportClubsChallengeStorage"];
            this.azureFunctionsUrl = configuration["SportClubsChallengeAzureFunctionsUrl"];
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

            // TODO: temporary solution to ping azure functions related with queue
            await PingAzureFunctionHost();
        }

        private async Task PingAzureFunctionHost()
        {
            if (string.IsNullOrEmpty(this.azureFunctionsUrl))
            {
                return;
            }

            await Task.Run(() =>
            {
                var request = (HttpWebRequest) WebRequest.Create(this.azureFunctionsUrl);
                request.GetResponseAsync();
            });
        }
    }
}