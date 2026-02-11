using Microsoft.Extensions.Caching.Memory;
using OrchestratorService.Application.DTOs;
using OrchestratorService.Infrastructure.HttpClients.JournalMicroService;
using OrchestratorService.Infrastructure.HttpClients.TaskJournalLinkMicroService;
using OrchestratorService.Infrastructure.HttpClients.TaskMicroService;
using SharedLib.DTOs.Journal;
using SharedLib.DTOs.Task;
using SharedLib.DTOs.TaskJournalLink;

namespace OrchestratorService.Application.Services
{
    public class JournalService : IJournalService
    {
        private readonly IJournalServiceClient _journalServiceClient;
        private readonly ITaskJournalLinkServiceClient _taskJournalLinkService;
        private readonly ITaskServiceClient _taskServiceClient;
        private readonly IMemoryCache _inMemoryCache;
        private readonly ILogger<JournalService> _logger;
        private const byte CachedTimeInMins = 5;                // Cache time in minutes

        public JournalService(IJournalServiceClient journalServiceClient, ITaskJournalLinkServiceClient taskJournalLinkService, ITaskServiceClient taskServiceClient, 
            IMemoryCache inMemoryCache, ILogger<JournalService> logger)
        {
            _journalServiceClient = journalServiceClient;
            _taskJournalLinkService = taskJournalLinkService;
            _taskServiceClient = taskServiceClient;
            _inMemoryCache = inMemoryCache;
            _logger = logger;
        }

        /// <summary>
        /// Creates a new journal via JournalApi and creates task journal links via TaskJournalLinkApi
        /// 1. Create Journal with retry on transient failures
        /// 2. Create Task Links with retry on transient failures
        /// 3. delete journal on failure to create links, not to keep journals which does not link to correct tasks
        /// if success return journal ID
        /// </summary>
        /// <param name="createJournalDto">CreateJournalDto Inputs</param>
        /// <param name="cancellationToken">CancellationToken</param>
        /// <returns>Guid? - ID of the journal if creation is success</returns>
        /// <exception cref="ApplicationException"></exception>
        public async Task<Guid?> AddJournalEntryWithTaskLinksAsync(CreateJournalDto createJournalDto, CancellationToken cancellationToken)
        {
            var now = DateTime.UtcNow;

            _logger.LogInformation(
                "Creating Journal '{Title}' with {TaskCount} linked tasks for UserId {UserId} at {Time}",
                createJournalDto.AddJournalEntryDto.Title,
                createJournalDto.LinkedTaskIds.Length,
                createJournalDto.AddJournalEntryDto.UserId,
                now);

            Guid? journalId = null;

            try
            {
                // 1. Create Journal (Polly handles transient retries)
                journalId = await _journalServiceClient.AddJournalEntryAsync(
                    createJournalDto.AddJournalEntryDto,
                    cancellationToken);

                if (journalId is null)
                    throw new InvalidOperationException("Journal API returned null journalId");

                _logger.LogInformation(
                    "Journal created with Id {JournalId}. Creating {TaskCount} TaskJournalLinks...",
                    journalId,
                    createJournalDto.LinkedTaskIds.Length);

                // 2. Create Task Links (Polly handles transient retries)
                var linksCreated = await _taskJournalLinkService.LinkNewJournalWithTasks(
                    journalId.Value,
                    createJournalDto.LinkedTaskIds,
                    cancellationToken);

                if (linksCreated is null || linksCreated.Length != createJournalDto.LinkedTaskIds.Length)
                {
                    _logger.LogWarning(
                        "Task linking failed for JournalId {JournalId}. Rolling back journal...",
                        journalId);

                    await _journalServiceClient.DeleteJournalEntryAsync(journalId.Value, cancellationToken);

                    throw new InvalidOperationException(
                        $"Failed to create all task links for JournalId {journalId}. Journal rolled back.");
                }

                // 3. Success
                _logger.LogInformation(
                    "Journal {JournalId} created and linked to {TaskCount} tasks for UserId {UserId}",
                    journalId,
                    createJournalDto.LinkedTaskIds.Length,
                    createJournalDto.AddJournalEntryDto.UserId);

                return journalId;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Failed to create Journal '{Title}' with {TaskCount} task links for UserId {UserId}",
                    createJournalDto.AddJournalEntryDto.Title,
                    createJournalDto.LinkedTaskIds.Length,
                    createJournalDto.AddJournalEntryDto.UserId);

                throw;
            }
        }

        public async Task<JournalWithTasksDto?> GetJournalByIdAsync(Guid id, CancellationToken cancellationToken)
        {
            var now = DateTime.UtcNow;

            _logger.LogInformation("Get Journal by Id {Id} at {Time}", id, now);
            try
            {
                // get journal
                var journal = await _journalServiceClient.GetJournalByIdAsync(id, cancellationToken);
                if (journal is null)
                {
                    _logger.LogWarning("Journal with Id {JournalId} not found", id);
                    return null;
                }

                // get links
                var links = await _taskJournalLinkService.GetLinksByJournalIdAsync(id, cancellationToken);
                var taskIds = links.Select(x => x.TaskId);
                if (links.Length == 0 || taskIds == null || !taskIds.Any())
                {
                    return new JournalWithTasksDto(journal, []);
                }

                // get tasks for each link               
                var taskDtos = await _taskServiceClient.GetTasksAsync(taskIds, cancellationToken);
                return new JournalWithTasksDto(journal, taskDtos ?? []);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to Retrieve Journal by Id {Id} at {Time}", id, now);
                throw;
            }
        }

    }
    
}