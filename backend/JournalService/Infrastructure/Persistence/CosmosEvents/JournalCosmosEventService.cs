using JournalService.Domain.Events.JournalEntryEvents;
using JournalService.Domain.Events.JournalFeedbackEvents;
using Microsoft.Azure.Cosmos;

namespace JournalService.Infrastructure.Persistence.CosmosEvents
{
    /// <summary>
    /// Logs journal entry related events on Azure Cosmos DB
    /// </summary>
    public class JournalCosmosEventService
    {
        private readonly CosmosClient _client;
        private readonly Container _container;
        private readonly ILogger<JournalCosmosEventService> _logger;

        public JournalCosmosEventService(IConfiguration config, ILogger<JournalCosmosEventService> logger)
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

        #region journal_entry
        public async Task LogJournalEntryCreatedAsync(JournalEntryCreatedDomainEvent notification)
        {
            try
            {
                var journalEntry = notification.Created;
                var evt = new
                {
                    id = Guid.NewGuid().ToString(), // required by Cosmos DB
                    eventType = "JournalCreated",
                    journalId = journalEntry.Id,
                    userId = journalEntry.UserId,
                    title = journalEntry.Title,
                    content = journalEntry.Content,
                    journalCreatedAt = journalEntry.CreatedAt,
                    cosmosLoggedAt = DateTime.UtcNow
                };
               
                await _container.CreateItemAsync(evt, new PartitionKey(journalEntry.UserId.ToString()));
                _logger.LogInformation("JournalCreated event logged to Cosmos DB for UserId={UserId}, JournalId={JournalId}, Title={Title}", 
                    journalEntry.UserId, journalEntry.Id, journalEntry.Title);
            }
            catch (CosmosException ex)
            {
                _logger.LogError(ex, "Failed to log JournalCreated event to Cosmos DB for UserId={UserId}, JournalId={JournalId}, Title={Title}. StatusCode={StatusCode}",
                    notification.Created.UserId, notification.Created.Id, notification.Created.Title, ex.StatusCode);
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while logging JournalCreated event to Cosmos DB for UserId={UserId}, JournalId={JournalId}, Title={Title}",
                    notification.Created.UserId, notification.Created.Id, notification.Created.Title);
                throw;
            }
        }

        public async Task LogJournalEntryUpdatedAsync(JournalEntryUpdatedDomainEvent notification)
        {
            try
            {
                var journalEntryBefore = notification.BeforeUpdate;
                var evt = new
                {
                    id = Guid.NewGuid().ToString(), // required by Cosmos DB
                    eventType = "JournalUpdated",
                    journalId = journalEntryBefore.Id,
                    userId = journalEntryBefore.UserId,                   
                    journalCreatedAt = journalEntryBefore.CreatedAt,
                    titleBefore = journalEntryBefore.Title,
                    contentBefore = journalEntryBefore.Content,
                    titleAfter = notification.AfterUpdate.Title,
                    contentAfter = notification.AfterUpdate.Content,
                    cosmosLoggedAt = DateTime.UtcNow
                };

                await _container.CreateItemAsync(evt, new PartitionKey(journalEntryBefore.UserId.ToString()));
                _logger.LogInformation("JournalUpdated event logged to Cosmos DB for UserId={UserId}, JournalId={JournalId}, Title={Title}",
                    journalEntryBefore.UserId, journalEntryBefore.Id, journalEntryBefore.Title);
            }
            catch (CosmosException ex)
            {
                _logger.LogError(ex, "Failed to log JournalUpdated event to Cosmos DB for UserId={UserId}, JournalId={JournalId}, Title={Title}. StatusCode={StatusCode}",
                    notification.BeforeUpdate.UserId, notification.BeforeUpdate.Id, notification.BeforeUpdate.Title, ex.StatusCode);
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while logging JournalUpdated event to Cosmos DB for UserId={UserId}, JournalId={JournalId}, Title={Title}",
                    notification.BeforeUpdate.UserId, notification.BeforeUpdate.Id, notification.BeforeUpdate.Title);
                throw;
            }
        }

        public async Task LogJournalFeedbackProvidedAsync(JournalFeedbackProvidedDomainEvent notification)
        {
            try
            {
                var journalEntryWithFeedback = notification.JournalEntryWithFeedback;
                var evt = new
                {
                    id = Guid.NewGuid().ToString(), // required by Cosmos DB
                    eventType = "JournalFeedbackProvided",
                    journalId = journalEntryWithFeedback.Id,
                    userId = journalEntryWithFeedback.UserId,
                    journalCreatedAt = journalEntryWithFeedback.CreatedAt,
                    titleBefore = journalEntryWithFeedback.Title,
                    contentBefore = journalEntryWithFeedback.Content,
                    journalFeedback = journalEntryWithFeedback.JournalFeedbackId,
                    cosmosLoggedAt = DateTime.UtcNow
                };

                await _container.CreateItemAsync(evt, new PartitionKey(journalEntryWithFeedback.UserId.ToString()));
                _logger.LogInformation("JournalFeedbackProvided event logged to Cosmos DB for UserId={UserId}, JournalId={JournalId}, Title={Title}",
                    journalEntryWithFeedback.UserId, journalEntryWithFeedback.Id, journalEntryWithFeedback.Title);
            }
            catch (CosmosException ex)
            {
                _logger.LogError(ex, "Failed to log JournalFeedbackProvided event to Cosmos DB for UserId={UserId}, JournalId={JournalId}, Title={Title}. StatusCode={StatusCode}",
                    notification.JournalEntryWithFeedback.UserId, notification.JournalEntryWithFeedback.Id, notification.JournalEntryWithFeedback.Title, ex.StatusCode);
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while logging JournalFeedbackProvided event to Cosmos DB for UserId={UserId}, JournalId={JournalId}, Title={Title}",
                    notification.JournalEntryWithFeedback.UserId, notification.JournalEntryWithFeedback.Id, notification.JournalEntryWithFeedback.Title);
                throw;
            }
        }

        public async Task LogJournalEntryRestoredAsync(JournalEntryRestoredDomainEvent notification)
        {
            try
            {
                var journalEntry = notification.JournalEntryRestored;
                var evt = new
                {
                    id = Guid.NewGuid().ToString(), // required by Cosmos DB
                    eventType = "JournalRestored",
                    journalId = journalEntry.Id,
                    userId = journalEntry.UserId,
                    title = journalEntry.Title,
                    content = journalEntry.Content,
                    journalCreatedAt = journalEntry.CreatedAt,
                    cosmosLoggedAt = DateTime.UtcNow
                };

                await _container.CreateItemAsync(evt, new PartitionKey(journalEntry.UserId.ToString()));
                _logger.LogInformation("JournalRestored event logged to Cosmos DB for UserId={UserId}, JournalId={JournalId}, Title={Title}",
                    journalEntry.UserId, journalEntry.Id, journalEntry.Title);
            }
            catch (CosmosException ex)
            {
                _logger.LogError(ex, "Failed to log JournalRestored event to Cosmos DB for UserId={UserId}, JournalId={JournalId}, Title={Title}. StatusCode={StatusCode}",
                    notification.JournalEntryRestored.UserId, notification.JournalEntryRestored.Id, notification.JournalEntryRestored.Title, ex.StatusCode);
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while logging JournalRestored event to Cosmos DB for UserId={UserId}, JournalId={JournalId}, Title={Title}",
                    notification.JournalEntryRestored.UserId, notification.JournalEntryRestored.Id, notification.JournalEntryRestored.Title);
                throw;
            }
        }

        public async Task LogJournalEntrySoftDeletedAsync(JournalEntrySoftDeletedDomainEvent notification)
        {
            try
            {
                var journalEntry = notification.JournalEntrySoftDeleted;
                var evt = new
                {
                    id = Guid.NewGuid().ToString(), // required by Cosmos DB
                    eventType = "JournalSoftDeleted",
                    journalId = journalEntry.Id,
                    userId = journalEntry.UserId,
                    title = journalEntry.Title,
                    content = journalEntry.Content,
                    journalCreatedAt = journalEntry.CreatedAt,
                    cosmosLoggedAt = DateTime.UtcNow
                };

                await _container.CreateItemAsync(evt, new PartitionKey(journalEntry.UserId.ToString()));
                _logger.LogInformation("JournalSoftDeleted event logged to Cosmos DB for UserId={UserId}, JournalId={JournalId}, Title={Title}",
                    journalEntry.UserId, journalEntry.Id, journalEntry.Title);
            }
            catch (CosmosException ex)
            {
                _logger.LogError(ex, "Failed to log JournalSoftDeleted event to Cosmos DB for UserId={UserId}, JournalId={JournalId}, Title={Title}. StatusCode={StatusCode}",
                    notification.JournalEntrySoftDeleted.UserId, notification.JournalEntrySoftDeleted.Id, notification.JournalEntrySoftDeleted.Title, ex.StatusCode);
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while logging JournalSoftDeleted event to Cosmos DB for UserId={UserId}, JournalId={JournalId}, Title={Title}",
                    notification.JournalEntrySoftDeleted.UserId, notification.JournalEntrySoftDeleted.Id, notification.JournalEntrySoftDeleted.Title);
                throw;
            }
        }
        #endregion journal_entry

        #region journal_feedback
        public async Task LogJournalFeedbackCreatedAsync(JournalFeedbackCreatedDomainEvent notification)
        {
            try
            {
                var journalFeedback = notification.JournalFeedback;
                var evt = new
                {
                    id = Guid.NewGuid().ToString(), // required by Cosmos DB
                    eventType = "JournalFeedbackCreated",
                    journalFeedbacklId = journalFeedback.Id,
                    journalId = journalFeedback.JournalEntryId,
                    feedbackManagerId = journalFeedback.FeedbackManagerId,
                    comment = journalFeedback.Comment,                    
                    journalFeedbackCreatedAt = journalFeedback.CreatedAt,
                    cosmosLoggedAt = DateTime.UtcNow
                };

                await _container.CreateItemAsync(evt, new PartitionKey(journalFeedback.JournalEntryId.ToString()));
                _logger.LogInformation("JournalFeedbackCreated event logged to Cosmos DB for JournalEntryId={JournalEntryId}, by manager ID={FeedbackManagerId} with ID={JournalFeedbackId}, at={Time}",
                    journalFeedback.JournalEntryId, journalFeedback.FeedbackManagerId, journalFeedback.Id, DateTime.UtcNow);
            }
            catch (CosmosException ex)
            {
                _logger.LogError(ex, "Failed to log JournalFeedbackCreated event to Cosmos DB for JournalEntryId={JournalEntryId}, by manager ID={FeedbackManagerId} with ID={JournalFeedbackId}, at={Time}. StatusCode={StatusCode}",
                    notification.JournalFeedback.JournalEntryId, notification.JournalFeedback.FeedbackManagerId, notification.JournalFeedback.Id, DateTime.UtcNow, ex.StatusCode);
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while logging JournalFeedbackCreated event to Cosmos DB for JournalEntryId={JournalEntryId}, by manager ID={FeedbackManagerId} with ID={JournalFeedbackId}, at={Time}",
                    notification.JournalFeedback.JournalEntryId, notification.JournalFeedback.FeedbackManagerId, notification.JournalFeedback.Id, DateTime.UtcNow);
                throw;
            }
        }

        public async Task JournalFeedbackMarkAsSeenAsync(JournalFeedbackMarkAsSeenDomainEvent notification)
        {
            try
            {
                var journalFeedback = notification.JournalFeedback;
                var evt = new
                {
                    id = Guid.NewGuid().ToString(), // required by Cosmos DB
                    eventType = "JournalFeedbackSeen",
                    journalFeedbacklId = journalFeedback.Id,
                    journalId = journalFeedback.JournalEntryId,
                    feedbackManagerId = journalFeedback.FeedbackManagerId,
                    comment = journalFeedback.Comment,
                    seenByUser = journalFeedback.SeenByUser,
                    journalFeedbackCreatedAt = journalFeedback.CreatedAt,
                    cosmosLoggedAt = DateTime.UtcNow
                };

                await _container.CreateItemAsync(evt, new PartitionKey(journalFeedback.JournalEntryId.ToString()));
                _logger.LogInformation("JournalFeedbackSeen event logged to Cosmos DB for JournalEntryId={JournalEntryId}, by manager ID={FeedbackManagerId} with ID={JournalFeedbackId}, at={Time}",
                    journalFeedback.JournalEntryId, journalFeedback.FeedbackManagerId, journalFeedback.Id, DateTime.UtcNow);
            }
            catch (CosmosException ex)
            {
                _logger.LogError(ex, "Failed to log JournalFeedbackSeen event to Cosmos DB for JournalEntryId={JournalEntryId}, by manager ID={FeedbackManagerId} with ID={JournalFeedbackId}, at={Time}. StatusCode={StatusCode}",
                    notification.JournalFeedback.JournalEntryId, notification.JournalFeedback.FeedbackManagerId, notification.JournalFeedback.Id, DateTime.UtcNow, ex.StatusCode);
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while logging JournalFeedbackSeen event to Cosmos DB for JournalEntryId={JournalEntryId}, by manager ID={FeedbackManagerId} with ID={JournalFeedbackId}, at={Time}",
                    notification.JournalFeedback.JournalEntryId, notification.JournalFeedback.FeedbackManagerId, notification.JournalFeedback.Id, DateTime.UtcNow);
                throw;
            }
        }
        #endregion journal_feedback
    }
}
