using Microsoft.Azure.Cosmos;
using MoodService.Domain.Events;


namespace MoodService.Infrastructure.Persistence.CosmosEvents
{
    /// <summary>
    /// Logs mood entry related events on Azure Cosmos DB
    /// </summary>
    public class MoodCosmosEventService
    {
        private readonly CosmosClient _client;
        private readonly Container _container;
        private readonly ILogger<MoodCosmosEventService> _logger;

        public MoodCosmosEventService(IConfiguration config, ILogger<MoodCosmosEventService> logger)
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

        // mood created
        public async Task LogMoodCreatedAsync(MoodEntryCreatedDomainEvent moodCreatedEvent)
        {
            try
            {
                var mood = moodCreatedEvent.Created;
                var evt = new
                {
                    id = Guid.NewGuid().ToString(), // required by Cosmos DB
                    eventType = "MoodCreated",
                    moodId = mood.Id,
                    userId = mood.UserId,
                    day = mood.Day.Date,
                    moodTime = mood.MoodTime.Value,
                    moodTimeRange = mood.MoodTime.TimeRange,
                    moodLevel = mood.MoodLevel.Value,
                    moodScore = mood.MoodLevel.Score,
                    note = mood.Note,
                    moodCreatedAt = mood.CreatedAt,
                    cosmosLoggedAt = DateTime.UtcNow
                };

                // patition key selection strategy
                // UserId + Day + MoodTime is unique, but it will create millions of tiny partitions which results in cross partition query even for a simple query like - “Show me all moods for this user” which is slow and expensive
                // - using 'userId' as partition key is the correct decision based on query patterns.
                // - “Show me all moods for this user”
                // - “Show me mood history for this user”
                // - “Show me mood trends for this user”
                // Partitioning by UserId makes these queries 'single‑partition', which is fast and cheap.
                await _container.CreateItemAsync(evt, new PartitionKey(mood.UserId.ToString()));
                _logger.LogInformation("MoodCreated event logged to Cosmos DB for UserId={UserId}, MoodId={MoodId}, Day={Day}, Time={Time}", mood.UserId, mood.Id, mood.Day.Date, mood.MoodTime);
            }
            catch (CosmosException ex)
            {
                _logger.LogError(ex, "Failed to log MoodCreated event to Cosmos DB for UserId={UserId}, MoodId={MoodId}, Day={Day}, Time={Time}. StatusCode={StatusCode}",
                    moodCreatedEvent.Created.UserId, moodCreatedEvent.Created.Id, moodCreatedEvent.Created.Day.Date, moodCreatedEvent.Created.MoodTime, ex.StatusCode);
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while logging MoodCreated event for UserId={UserId}, MoodId={MoodId}, Day={Day}, Time={Time}",
                    moodCreatedEvent.Created.UserId, moodCreatedEvent.Created.Id, moodCreatedEvent.Created.Day.Date, moodCreatedEvent.Created.MoodTime);
                throw;
            }
        }

        // mood updated
        public async Task LogMoodUpdatedAsync(MoodEntryUpdatedDomainEvent moodEntryUpdatedEvent)
        {
            try
            {
                var evt = new
                {
                    id = Guid.NewGuid().ToString(), // required by Cosmos DB

                    eventType = "MoodUpdated",

                    moodId = moodEntryUpdatedEvent.Previous.Id,
                    userId = moodEntryUpdatedEvent.Previous.UserId,

                    previousDay = moodEntryUpdatedEvent.Previous.Day.Date,
                    updatedDay = moodEntryUpdatedEvent.Updated.Day.Date,

                    previousMoodTime = moodEntryUpdatedEvent.Previous.MoodTime.Value,
                    updatedMoodTime = moodEntryUpdatedEvent.Updated.MoodTime.Value,

                    previousMoodTimeRange = moodEntryUpdatedEvent.Previous.MoodTime.TimeRange,
                    updatedMoodTimeRange = moodEntryUpdatedEvent.Updated.MoodTime.TimeRange,

                    previousMoodLevel = moodEntryUpdatedEvent.Previous.MoodLevel.Value,
                    updatedMoodLevel = moodEntryUpdatedEvent.Updated.MoodLevel.Value,

                    previousMoodScore = moodEntryUpdatedEvent.Previous.MoodLevel.Score,
                    updatedMoodScore = moodEntryUpdatedEvent.Updated.MoodLevel.Score,

                    previousMoodNote = moodEntryUpdatedEvent.Previous.Note,
                    updatedMoodNote = moodEntryUpdatedEvent.Updated.Note,

                    moodCreatedAt = moodEntryUpdatedEvent.Previous.CreatedAt,
                    cosmosLoggedAt = DateTime.UtcNow
                };

                await _container.CreateItemAsync(evt, new PartitionKey(moodEntryUpdatedEvent.Previous.UserId.ToString()));
                _logger.LogInformation("MoodUpdated event logged to Cosmos DB for UserId={UserId}, MoodId={MoodId}, Day={Day}, Time={Time}",
                    moodEntryUpdatedEvent.Previous.UserId, moodEntryUpdatedEvent.Updated.Id, moodEntryUpdatedEvent.Updated.Day.Date, moodEntryUpdatedEvent.Updated.MoodTime);
            }
            catch (CosmosException ex)
            {
                _logger.LogError(ex, "Failed to log MoodUpdated event to Cosmos DB for UserId={UserId}, MoodId={MoodId}, Day={Day}, Time={Time}. StatusCode={StatusCode}",
                    moodEntryUpdatedEvent.Previous.UserId, moodEntryUpdatedEvent.Updated.Id, moodEntryUpdatedEvent.Updated.Day.Date, moodEntryUpdatedEvent.Updated.MoodTime, ex.StatusCode);
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while logging MoodUpdated event for UserId={UserId}, MoodId={MoodId}, Day={Day}, Time={Time}",
                    moodEntryUpdatedEvent.Previous.UserId, moodEntryUpdatedEvent.Updated.Id, moodEntryUpdatedEvent.Updated.Day.Date, moodEntryUpdatedEvent.Updated.MoodTime);
                throw;
            }
        }

        // mood deleted
        public async Task LogMoodDeletedAsync(MoodEntryDeletedDomainEvent moodEntryDeletedEvent)
        {
            try
            {
                var mood = moodEntryDeletedEvent.Deleted;
                var evt = new
                {
                    id = Guid.NewGuid().ToString(), // required by Cosmos DB
                    eventType = "MoodDeleted",
                    moodId = mood.Id,
                    userId = mood.UserId,
                    day = mood.Day.Date,
                    moodTime = mood.MoodTime.Value,
                    moodTimeRange = mood.MoodTime.TimeRange,
                    moodLevel = mood.MoodLevel.Value,
                    moodScore = mood.MoodLevel.Score,
                    note = mood.Note,
                    moodCreatedAt = mood.CreatedAt,
                    cosmosLoggedAt = DateTime.UtcNow
                };

                await _container.CreateItemAsync(evt, new PartitionKey(mood.UserId.ToString()));
                _logger.LogInformation("MoodDeleted event logged to Cosmos DB for UserId={UserId}, MoodId={MoodId}, Day={Day}, Time={Time}", mood.UserId, mood.Id, mood.Day.Date, mood.MoodTime);
            }
            catch (CosmosException ex)
            {
                _logger.LogError(ex, "Failed to log MoodDeleted event to Cosmos DB for UserId={UserId}, MoodId={MoodId}, Day={Day}, Time={Time}. StatusCode={StatusCode}",
                    moodEntryDeletedEvent.Deleted.UserId, moodEntryDeletedEvent.Deleted.Id, moodEntryDeletedEvent.Deleted.Day.Date, moodEntryDeletedEvent.Deleted.MoodTime, ex.StatusCode);
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while logging MoodDeleted event for UserId={UserId}, MoodId={MoodId}, Day={Day}, Time={Time}",
                    moodEntryDeletedEvent.Deleted.UserId, moodEntryDeletedEvent.Deleted.Id, moodEntryDeletedEvent.Deleted.Day.Date, moodEntryDeletedEvent.Deleted.MoodTime);
                throw;
            }
        }
    }
}
