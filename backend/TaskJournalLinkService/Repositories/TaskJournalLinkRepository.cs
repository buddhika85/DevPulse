using Azure;
using Microsoft.Azure.Cosmos;
using System.Text.Json;
using TaskJournalLinkService.Domain.Models;

namespace TaskJournalLinkService.Repositories
{
    public class TaskJournalLinkRepository : ITaskJournalLinkRepository
    {
        private readonly CosmosClient _client;
        private readonly Container _container;
        private readonly ILogger<TaskJournalLinkRepository> _logger;

        public TaskJournalLinkRepository(IConfiguration config, ILogger<TaskJournalLinkRepository> logger)
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

        /// <summary>
        /// Reads all Task Journal Links by journalId
        /// </summary>
        /// <param name="journalId">journalId</param>
        /// <param name="cancellationToken">cancellationToken</param>
        /// <returns>TaskJournalLinkDocument[]</returns>
        /// <summary>
        /// Reads all Task Journal Links by journalId.
        /// </summary>
        public async Task<TaskJournalLinkDocument[]> GetLinksByJournalIdAsync(Guid journalId, CancellationToken cancellationToken)
        {
            _logger.LogInformation(
                "Querying TaskJournalLinks for JournalId {JournalId}",
                journalId);

            try
            {
                // Build the query
                var query = new QueryDefinition(
                    "SELECT * FROM c WHERE c.JournalId = @journalId")
                    .WithParameter("@journalId", journalId.ToString());

                _logger.LogDebug(
                    "Cosmos DB query: {QueryText}",
                    query.QueryText);

                // Create iterator scoped to the partition key
                var iterator = _container.GetItemQueryIterator<TaskJournalLinkDocument>(
                    query,
                    requestOptions: new QueryRequestOptions
                    {
                        PartitionKey = new PartitionKey(journalId.ToString())
                    });

                var results = new List<TaskJournalLinkDocument>();

                // Read all pages
                while (iterator.HasMoreResults)
                {
                    var response = await iterator.ReadNextAsync(cancellationToken);
                    results.AddRange(response.Resource);
                }

                _logger.LogInformation(
                    "Retrieved {Count} TaskJournalLinks for JournalId {JournalId}",
                    results.Count,
                    journalId);

                return results.ToArray();
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Error querying TaskJournalLinks for JournalId {JournalId}",
                    journalId);

                throw;
            }
        }



        /// <summary>
        /// Links journals with tasks as a single transaction - 
        /// With TransactionalBatch, Cosmos DB guarantees all‑or‑nothing atomicity within a single partition key.
        /// </summary>
        /// <param name="journalId">journalId</param>
        /// <param name="taskIdsToLink">taskIdsToLink []</param>
        /// <param name="cancellationToken">cancellationToken</param>
        /// <returns>TaskJournalLinkDocument[]</returns>
        /// <exception cref="ApplicationException">If Transaction fails on cosmos level</exception>
        public async Task<TaskJournalLinkDocument[]> LinkNewJournalWithTasksAsync(Guid journalId,
                                                                                    Guid[] taskIdsToLink,
                                                                                    CancellationToken cancellationToken)
        {
            var now = DateTime.UtcNow;

            // Log the start of the batch operation
            _logger.LogInformation(
                "Starting transactional batch to link JournalId {JournalId} with {TaskCount} TaskIds at {Time}",
                journalId,
                taskIdsToLink.Length,
                now);

            try
            {
                // Create a transactional batch for a single partition (journalId)
                var batch = _container.CreateTransactionalBatch(new PartitionKey(journalId.ToString()));

                // Add a CreateItem operation for each task link
                foreach (var taskId in taskIdsToLink)
                {
                    _logger.LogDebug(
                        "Adding CreateItem operation to batch for JournalId {JournalId} and TaskId {TaskId}",
                        journalId,
                        taskId);

                    var doc = new TaskJournalLinkDocument(Guid.NewGuid(), taskId, journalId.ToString(), now);
                    var json = JsonSerializer.Serialize(doc);
                    _logger.LogDebug("Serialized document: {Json}", json);
                    batch.CreateItem(doc);
                }

                // Execute the batch as a single atomic operation
                _logger.LogInformation(
                    "Executing transactional batch for JournalId {JournalId} with {TaskCount} operations",
                    journalId,
                    taskIdsToLink.Length);

                var batchResponse = await batch.ExecuteAsync(cancellationToken);

                // If the batch fails, none of the operations were committed
                if (!batchResponse.IsSuccessStatusCode)
                {
                    _logger.LogError(
                        "Transactional batch failed for JournalId {JournalId}. StatusCode: {StatusCode}",
                        journalId,
                        batchResponse.StatusCode);

                    _logger.LogError("Batch failed. Status={Status}, RU={RU}",
                            batchResponse.StatusCode,
                            batchResponse.RequestCharge);


                    var diagnostics = batchResponse.Diagnostics.ToString();
                    _logger.LogError("Cosmos diagnostics: {Diag}", diagnostics);



                    for (int i = 0; i < batchResponse.Count; i++)
                    {
                        var op = batchResponse.GetOperationResultAtIndex<object>(i);

                        _logger.LogError("Operation {Index}: Status={Status}",
                            i,
                            op.StatusCode);
                    }


                    throw new ApplicationException($"Transactional batch failed: {batchResponse.StatusCode}");
                }

                // Prepare to collect the created documents
                _logger.LogInformation(
                    "Transactional batch succeeded for JournalId {JournalId}. Processing {OperationCount} results",
                    journalId,
                    batchResponse.Count);

                var results = new List<TaskJournalLinkDocument>();

                // Process each operation result in the batch
                for (int i = 0; i < batchResponse.Count; i++)
                {
                    var op = batchResponse[i];

                    // Check if the individual operation succeeded
                    if (!op.IsSuccessStatusCode)
                    {
                        _logger.LogError(
                            "Batch operation {Index} failed for JournalId {JournalId}. StatusCode: {StatusCode}",
                            i,
                            journalId,
                            op.StatusCode);

                        throw new Exception($"Operation {i} failed: {op.StatusCode}");
                    }

                    // Deserialize the returned document from the stream
                    using var stream = op.ResourceStream;
                    var doc = await JsonSerializer.DeserializeAsync<TaskJournalLinkDocument>(stream, cancellationToken: cancellationToken);

                    if (doc != null)
                    {
                        _logger.LogDebug(
                            "Deserialized TaskJournalLinkDocument for JournalId {JournalId}, TaskId {TaskId}",
                            doc.JournalId,
                            doc.TaskId);

                        results.Add(doc);
                    }
                    else
                    {
                        _logger.LogWarning(
                            "Operation {Index} returned null document for JournalId {JournalId}",
                            i,
                            journalId);
                    }
                }

                
                _logger.LogInformation(
                    "Successfully created {Count} TaskJournalLink documents for JournalId {JournalId}",
                    results.Count,
                    journalId);

                return results.ToArray();
            }
            catch (Exception ex)
            {
                // Log unexpected errors
                _logger.LogError(
                    ex,
                    "Error while linking JournalId {JournalId} with {TaskCount} TaskIds at {Time}",
                    journalId,
                    taskIdsToLink.Length,
                    now);
                throw;
            }
        }
    }
}
