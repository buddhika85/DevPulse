using Microsoft.Azure.Cosmos;
using Newtonsoft.Json;
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


        #region CreateTaskJournalLinks

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
                                                                                    HashSet<Guid> taskIdsToLink,
                                                                                    CancellationToken cancellationToken)
        {
            var now = DateTime.UtcNow;

            // Log the start of the batch operation
            _logger.LogInformation(
                "Starting transactional batch to link JournalId {JournalId} with {TaskCount} TaskIds at {Time}",
                journalId,
                taskIdsToLink.Count,
                now);

            try
            {
                // Create Batch
                TransactionalBatch batch = CreateCosmosTaskJournalLinksBatch(journalId, taskIdsToLink, now);

                // Execute the batch as a single atomic operation
                _logger.LogInformation(
                    "Executing transactional batch for JournalId {JournalId} with {TaskCount} operations",
                    journalId,
                    taskIdsToLink.Count);
                var batchResponse = await batch.ExecuteAsync(cancellationToken);

                // If the batch fails, none of the operations were committed
                if (!batchResponse.IsSuccessStatusCode)
                {
                    HandleCosmosBatchFailure(journalId, batchResponse);
                }

                // Prepare to collect the created documents
                _logger.LogInformation(
                    "Transactional batch succeeded for JournalId {JournalId}. Processing {OperationCount} results",
                    journalId,
                    batchResponse.Count);

                // collect 
                List<TaskJournalLinkDocument> results = DeserializeCosmosBatchResponse(journalId, batchResponse);

                _logger.LogInformation(
                    "Successfully created {Count} TaskJournalLink documents for JournalId {JournalId}",
                    results.Count,
                    journalId);

                return [.. results];
            }
            catch (Exception ex)
            {
                // Log unexpected errors
                _logger.LogError(
                    ex,
                    "Error while linking JournalId {JournalId} with {TaskCount} TaskIds at {Time}",
                    journalId,
                    taskIdsToLink.Count,
                    now);
                throw;
            }
        }

        /// <summary>
        /// A helper method to Deserialize Cosmos Batch Response and return the links created. 
        /// Additionaly checks if the individual link document creation on cosmos was succeeded or not.
        /// </summary>
        /// <param name="journalId">journalId</param>
        /// <param name="batchResponse">cosmos batchResponse</param>
        /// <returns>A list of TaskJournalLinkDocument created at cosmos DB</returns>
        /// <exception cref="Exception">Throws if there are any hidden failures at single link document creation for the batch</exception>
        private List<TaskJournalLinkDocument> DeserializeCosmosBatchResponse(Guid journalId, TransactionalBatchResponse batchResponse)
        {
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
                using var reader = new StreamReader(stream);
                using var jsonReader = new JsonTextReader(reader);
                var serializer = Newtonsoft.Json.JsonSerializer.Create();

                var doc = serializer.Deserialize<TaskJournalLinkDocument>(jsonReader);


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

            return results;
        }

        /// <summary>
        /// A helper method to collect & write diagnostic logs for batch failures single link wise for all linkings attempted 
        /// </summary>
        /// <param name="journalId">journalId</param>
        /// <param name="batchResponse">batchResponse returned from Az Cosmos DB</param>
        /// <exception cref="ApplicationException">Finaly theows ApplicationException batchResponse.StatusCode for retries using polly</exception>
        private void HandleCosmosBatchFailure(Guid journalId, TransactionalBatchResponse batchResponse)
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

        /// <summary>
        /// A helper method to create transactional batch containing journal Id and task Id
        /// </summary>
        /// <param name="journalId">journalId - cosmos partition Id</param>
        /// <param name="taskIdsToLink"taskIdsToLink></param>
        /// <param name="now">Time for saving time stamp of link creation in cosmos DB</param>
        /// <returns>TransactionalBatch</returns>
        private TransactionalBatch CreateCosmosTaskJournalLinksBatch(Guid journalId, HashSet<Guid> taskIdsToLink, DateTime now)
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
                var json = JsonConvert.SerializeObject(doc);
                _logger.LogDebug("Serialized document: {Json}", json);
                batch.CreateItem(doc);
            }

            return batch;
        }



        #endregion CreateTaskJournalLinks




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
                    "SELECT * FROM c WHERE c.journalId = @journalId")
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

                return [.. results];
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
        /// Performs an atomic rearrangement of TaskJournalLinks for the specified journal.
        /// Uses a Cosmos DB TransactionalBatch to remove old links and add new ones within
        /// the same partition, ensuring ACID guarantees.
        /// </summary>
        /// <param name="journalId">The journal identifier (also used as the partition key).</param>
        /// <param name="removeSet">The TaskJournalLink documents to remove.</param>
        /// <param name="addSet">The TaskJournalLink documents to add.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        /// <returns>
        /// True if the batch operation succeeds; otherwise, false.
        /// </returns>

        public async Task<bool> RearrangeTaskJournalLinksAsync(Guid journalId,
                                                                    List<TaskJournalLinkDocument> removeSet,
                                                                    List<TaskJournalLinkDocument> addSet,
                                                                    CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation(
                    "Starting transactional batch for JournalId={JournalId}. RemoveCount={RemoveCount}, AddCount={AddCount}",
                    journalId, removeSet.Count, addSet.Count);

                var partitionKey = new PartitionKey(journalId.ToString());
                var batch = _container.CreateTransactionalBatch(partitionKey);

                // Remove old links
                foreach (var item in removeSet)
                {
                    _logger.LogDebug(
                        "Batch delete: JournalId={JournalId}, LinkId={LinkId}, TaskId={TaskId}",
                        journalId, item.Id, item.TaskId);

                    batch.DeleteItem(item.Id.ToString());
                }

                // Add new links
                foreach (var item in addSet)
                {
                    _logger.LogDebug(
                        "Batch create: JournalId={JournalId}, LinkId={LinkId}, TaskId={TaskId}",
                        journalId, item.Id, item.TaskId);

                    batch.CreateItem(item);
                }

                var batchResponse = await batch.ExecuteAsync(cancellationToken);

                if (!batchResponse.IsSuccessStatusCode)
                {
                    HandleCosmosBatchFailure(journalId, batchResponse);

                    _logger.LogWarning(
                        "Transactional batch FAILED for JournalId={JournalId}. StatusCode={StatusCode}",
                        journalId, batchResponse.StatusCode);

                    return false;
                }

                _logger.LogInformation(
                    "Transactional batch SUCCEEDED for JournalId={JournalId}. OperationCount={OperationCount}",
                    journalId, batchResponse.Count);

                return true;
            }
            catch (CosmosException cex)
            {
                _logger.LogError(
                    cex,
                    "CosmosException during transactional batch for JournalId={JournalId}. StatusCode={StatusCode}",
                    journalId, cex.StatusCode);

                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Unexpected error during transactional batch for JournalId={JournalId}",
                    journalId);

                throw;
            }
        }
    }
}
