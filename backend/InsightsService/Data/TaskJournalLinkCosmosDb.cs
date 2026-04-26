using InsightsService.Options;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Options;

namespace InsightsService.Data
{
    // Cosmos DB - reading insights from Cosmos DB containers
    public class TaskJournalLinkCosmosDb
    {
        private readonly CosmosClient _client;
        private readonly TaskJournalLinkDb _cfg;

        public TaskJournalLinkCosmosDb(IOptions<DatabaseConnections> options)
        {
            _cfg = options.Value.TaskJournalLinkDb;

            _client = new CosmosClient(
                _cfg.AccountEndpoint,
                _cfg.AccountKey
            );
        }

        public Container Container
            => _client.GetContainer(_cfg.DatabaseName, _cfg.ContainerName);
    }


}
