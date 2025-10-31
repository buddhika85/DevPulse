using Microsoft.Azure.Cosmos;
using UserService.Domain.Entities;
using UserService.Domain.Events;

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
            try
            {
                var evt = new
                {
                    id = Guid.NewGuid().ToString(), // required by Cosmos DB
                    eventType = "UserCreated",
                    userId = user.Id,
                    email = user.Email,
                    displayName = user.DisplayName,
                    cosmosLoggedAt = DateTime.UtcNow
                };

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

        // UserDisplaNameChanged
        public async Task LogUserDisplayNameChangedAsync(UserDisplayNameChangedDomainEvent notification)
        {
            try
            {
                var evt = new
                {
                    id = Guid.NewGuid().ToString(),
                    eventType = "UserDisplayNameChanged",
                    userId = notification.UpdatedAccount.Id,
                    email = notification.UpdatedAccount.Email,
                    oldDisplayName = notification.OldDisplayName,
                    newDisplayName = notification.NewDisplayName,
                    cosmosLoggedAt = DateTime.UtcNow
                };

                await _container.CreateItemAsync(evt, new PartitionKey(notification.UpdatedAccount.Id.ToString()));

                _logger.LogInformation("UserDisplayNameChanged event logged to Cosmos DB for UserId={UserId}, Email={Email}, OldDisplayName={Old}, NewDisplayName={New}",
                    notification.UpdatedAccount.Id,
                    notification.UpdatedAccount.Email,
                    notification.OldDisplayName,
                    notification.NewDisplayName);
            }
            catch (CosmosException ex)
            {
                _logger.LogError(ex, "Failed to log UserDisplayNameChanged event for UserId={UserId}, Email={Email}. StatusCode={StatusCode}",
                    notification.UpdatedAccount.Id,
                    notification.UpdatedAccount.Email,
                    ex.StatusCode);
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while logging UserDisplayNameChanged event for UserId={UserId}, Email={Email}",
                    notification.UpdatedAccount.Id,
                    notification.UpdatedAccount.Email);
                throw;
            }
        }

        // updated
        public async Task LogUserUpdatedAsync(UserUpdatedDomainEvent notification)
        {
            try
            {
                var evt = new
                {
                    id = Guid.NewGuid().ToString(),
                    eventType = "UserUpdated",
                    userId = notification.UpdatedUserAccount.Id,
                    email = notification.UpdatedUserAccount.Email,
                    displayName = notification.UpdatedUserAccount.DisplayName,
                    isDeleted = notification.UpdatedUserAccount.IsDeleted,
                    cosmosLoggedAt = DateTime.UtcNow
                };

                await _container.CreateItemAsync(evt, new PartitionKey(notification.UpdatedUserAccount.Id.ToString()));

                _logger.LogInformation("UserUpdated event logged to Cosmos DB for UserId={UserId}, Email={Email}, DisplayName={DisplayName}, IsDeleted={IsDeleted}",
                    notification.UpdatedUserAccount.Id,
                    notification.UpdatedUserAccount.Email,
                    notification.UpdatedUserAccount.DisplayName,
                    notification.UpdatedUserAccount.IsDeleted);
            }
            catch (CosmosException ex)
            {
                _logger.LogError(ex, "Failed to log UserUpdated event for UserId={UserId}, Email={Email}. StatusCode={StatusCode}",
                    notification.UpdatedUserAccount.Id,
                    notification.UpdatedUserAccount.Email,
                    ex.StatusCode);
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while logging UserUpdated event for UserId={UserId}, Email={Email}",
                    notification.UpdatedUserAccount.Id,
                    notification.UpdatedUserAccount.Email);
                throw;
            }
        }

        // Logged In
        public async Task LogUserLoggedInAsync(UserLoggedInDomainEvent notification)
        {
            try
            {
                var evt = new
                {
                    id = Guid.NewGuid().ToString(),
                    eventType = "UserLoggedIn",
                    userId = notification.UserAccount.Id,
                    email = notification.UserAccount.Email,
                    displayName = notification.UserAccount.DisplayName,
                    loginTime = notification.LoginTime,
                    cosmosLoggedAt = DateTime.UtcNow
                };

                await _container.CreateItemAsync(evt, new PartitionKey(notification.UserAccount.Id.ToString()));

                _logger.LogInformation("UserLoggedIn event logged to Cosmos DB for UserId={UserId}, Email={Email}, LoginTime={LoginTime}",
                    notification.UserAccount.Id, notification.UserAccount.Email, notification.LoginTime);
            }
            catch (CosmosException ex)
            {
                _logger.LogError(ex, "Failed to log UserLoggedIn event for UserId={UserId}, Email={Email}. StatusCode={StatusCode}",
                    notification.UserAccount.Id, notification.UserAccount.Email, ex.StatusCode);
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while logging UserLoggedIn event for UserId={UserId}, Email={Email}",
                    notification.UserAccount.Id, notification.UserAccount.Email);
                throw;
            }
        }

        // Email update
        public async Task LogUserEmailChangedAsync(UserEmailChangedDomainEvent notification)
        {
            try
            {
                var evt = new
                {
                    id = Guid.NewGuid().ToString(),
                    eventType = "UserEmailChanged",
                    userId = notification.UserAccount.Id,
                    previousEmail = notification.PreviousEmail,
                    newEmail = notification.NewEmail,
                    cosmosLoggedAt = DateTime.UtcNow
                };

                await _container.CreateItemAsync(evt, new PartitionKey(notification.UserAccount.Id.ToString()));

                _logger.LogInformation("UserEmailChanged event logged to Cosmos DB for UserId={UserId}, PreviousEmail={PreviousEmail}, NewEmail={NewEmail}",
                    notification.UserAccount.Id, notification.PreviousEmail, notification.NewEmail);
            }
            catch (CosmosException ex)
            {
                _logger.LogError(ex, "Failed to log UserEmailChanged event for UserId={UserId}. StatusCode={StatusCode}",
                    notification.UserAccount.Id, ex.StatusCode);
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while logging UserEmailChanged event for UserId={UserId}",
                    notification.UserAccount.Id);
                throw;
            }
        }

        // Logged out
        public async Task LogUserLoggedOutAsync(UserLoggedOutDomainEvent notification)
        {
            try
            {
                var evt = new
                {
                    id = Guid.NewGuid().ToString(),
                    eventType = "UserLoggedOut",
                    userId = notification.UserAccount.Id,
                    email = notification.UserAccount.Email,
                    displayName = notification.UserAccount.DisplayName,
                    loggedOutTime = notification.LoggedOutTime,
                    cosmosLoggedAt = DateTime.UtcNow
                };

                await _container.CreateItemAsync(evt, new PartitionKey(notification.UserAccount.Id.ToString()));

                _logger.LogInformation("UserLoggedOut event logged to Cosmos DB for UserId={UserId}, Email={Email}, LoggedOutTime={LoggedOutTime}",
                    notification.UserAccount.Id, notification.UserAccount.Email, notification.LoggedOutTime);
            }
            catch (CosmosException ex)
            {
                _logger.LogError(ex, "Failed to log UserLoggedOut event for UserId={UserId}, Email={Email}. StatusCode={StatusCode}",
                    notification.UserAccount.Id, notification.UserAccount.Email, ex.StatusCode);
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while logging UserLoggedOut event for UserId={UserId}, Email={Email}",
                    notification.UserAccount.Id, notification.UserAccount.Email);
                throw;
            }
        }

        // Restored
        public async Task LogUserRestoredAsync(UserRestoredDomainEvent notification)
        {
            try
            {
                var evt = new
                {
                    id = Guid.NewGuid().ToString(),
                    eventType = "UserRestored",
                    userId = notification.RestoredUserAccount.Id,
                    email = notification.RestoredUserAccount.Email,
                    displayName = notification.RestoredUserAccount.DisplayName,
                    cosmosLoggedAt = DateTime.UtcNow
                };

                await _container.CreateItemAsync(evt, new PartitionKey(notification.RestoredUserAccount.Id.ToString()));

                _logger.LogInformation("UserRestored event logged to Cosmos DB for UserId={UserId}, Email={Email}",
                    notification.RestoredUserAccount.Id, notification.RestoredUserAccount.Email);
            }
            catch (CosmosException ex)
            {
                _logger.LogError(ex, "Failed to log UserRestored event for UserId={UserId}, Email={Email}. StatusCode={StatusCode}",
                    notification.RestoredUserAccount.Id, notification.RestoredUserAccount.Email, ex.StatusCode);
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while logging UserRestored event for UserId={UserId}, Email={Email}",
                    notification.RestoredUserAccount.Id, notification.RestoredUserAccount.Email);
                throw;
            }
        }

        // Role change
        public async Task LogUserRoleChangedAsync(UserRoleChangedDomainEvent notification)
        {
            try
            {
                var evt = new
                {
                    id = Guid.NewGuid().ToString(),
                    eventType = "UserRoleChanged",
                    userId = notification.UserAccount.Id,
                    previousRole = notification.PreviousRole,
                    newRole = notification.NewRole,
                    cosmosLoggedAt = DateTime.UtcNow
                };

                await _container.CreateItemAsync(evt, new PartitionKey(notification.UserAccount.Id.ToString()));

                _logger.LogInformation("UserRoleChanged event logged to Cosmos DB for UserId={UserId}, PreviousRole={PreviousRole}, NewRole={NewRole}",
                    notification.UserAccount.Id, notification.PreviousRole, notification.NewRole);
            }
            catch (CosmosException ex)
            {
                _logger.LogError(ex, "Failed to log UserRoleChanged event for UserId={UserId}. StatusCode={StatusCode}",
                    notification.UserAccount.Id, ex.StatusCode);
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while logging UserRoleChanged event for UserId={UserId}",
                    notification.UserAccount.Id);
                throw;
            }
        }

        // Soft delete
        public async Task LogUserSoftDeletedAsync(UserSoftDeletedDomainEvent notification)
        {
            try
            {
                var evt = new
                {
                    id = Guid.NewGuid().ToString(),
                    eventType = "UserSoftDeleted",
                    userId = notification.UserAccountDeleted.Id,
                    email = notification.UserAccountDeleted.Email,
                    displayName = notification.UserAccountDeleted.DisplayName,
                    cosmosLoggedAt = DateTime.UtcNow
                };

                await _container.CreateItemAsync(evt, new PartitionKey(notification.UserAccountDeleted.Id.ToString()));

                _logger.LogInformation("UserSoftDeleted event logged to Cosmos DB for UserId={UserId}, Email={Email}",
                    notification.UserAccountDeleted.Id, notification.UserAccountDeleted.Email);
            }
            catch (CosmosException ex)
            {
                _logger.LogError(ex, "Failed to log UserSoftDeleted event for UserId={UserId}, Email={Email}. StatusCode={StatusCode}",
                    notification.UserAccountDeleted.Id, notification.UserAccountDeleted.Email, ex.StatusCode);
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while logging UserSoftDeleted event for UserId={UserId}, Email={Email}",
                    notification.UserAccountDeleted.Id, notification.UserAccountDeleted.Email);
                throw;
            }
        }
    }
}
