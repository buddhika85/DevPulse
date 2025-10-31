using Microsoft.Azure.Cosmos;
using UserService.Domain.Entities;

namespace UserService.Infrastructure.Persistence.ComosEvents
{
    /// <summary>
    /// Logs user related events on Azure Cosmos DB
    /// </summary>
    public class UserCosmosEventService
    {
        private readonly CosmosClient _client;
        private readonly Container _container;
        private readonly ILogger<UserCosmosEventService> _logger;

        public UserCosmosEventService(IConfiguration config, ILogger<UserCosmosEventService> logger)
        {
            _client = new CosmosClient(
                config["CosmosDb:AccountEndpoint"],
                config["CosmosDb:AccountKey"]
            );
            _container = _client.GetContainer(
                config["CosmosDb:DatabaseName"],
                config["CosmosDb:ContainerName"]
            );
            _logger = logger;
        }

        // user created event logged in azure cosmos DB
        public async Task LogUserCreatedAsync(UserAccount user)
        {
            var evt = new
            {
                eventType = "UserCreated",
                userId = user.Id,
                email = user.Email,
                displayName = user.DisplayName,
                createdAt = DateTime.UtcNow
            };

            try
            {
                await _container.CreateItemAsync(evt, new PartitionKey(user.Id.ToString()));
                _logger.LogInformation("UserCreated event logged to Cosmos DB for UserId={UserId}, Email={Email}", user.Id, user.Email);
            }
            catch (CosmosException ex)
            {
                _logger.LogError(ex, "Failed to log UserCreated event for UserId={UserId}, Email={Email}. StatusCode={StatusCode}", user.Id, user.Email, ex.StatusCode);
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while logging UserCreated event for UserId={UserId}, Email={Email}", user.Id, user.Email);
                throw;
            }
        }
    }
}
