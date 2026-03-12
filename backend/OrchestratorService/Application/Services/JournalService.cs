using Microsoft.Extensions.Caching.Memory;
using OrchestratorService.Application.DTOs;
using OrchestratorService.Infrastructure.HttpClients.JournalMicroService;
using OrchestratorService.Infrastructure.HttpClients.TaskJournalLinkMicroService;
using OrchestratorService.Infrastructure.HttpClients.TaskMicroService;
using OrchestratorService.Infrastructure.HttpClients.UserMicroService;
using SharedLib.DTOs.Journal;
using SharedLib.DTOs.Task;
using SharedLib.DTOs.TaskJournalLink;
using SharedLib.DTOs.User;
using System.Collections.Generic;

namespace OrchestratorService.Application.Services
{
    public class JournalService : IJournalService
    {
        private readonly IJournalServiceClient _journalServiceClient;
        private readonly ITaskJournalLinkServiceClient _taskJournalLinkServiceClient;
        private readonly ITaskServiceClient _taskServiceClient;
        private readonly IUserServiceClient _userServiceClient;
        private readonly IMemoryCache _inMemoryCache;
        private readonly ILogger<JournalService> _logger;
        private const byte CachedTimeInMins = 5;                // Cache time in minutes

        public JournalService(IJournalServiceClient journalServiceClient, ITaskJournalLinkServiceClient taskJournalLinkService, ITaskServiceClient taskServiceClient, IUserServiceClient userServiceClient,
            IMemoryCache inMemoryCache, ILogger<JournalService> logger)
        {
            _journalServiceClient = journalServiceClient;
            _taskJournalLinkServiceClient = taskJournalLinkService;
            _taskServiceClient = taskServiceClient;
            _userServiceClient = userServiceClient;
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
                createJournalDto.LinkedTaskIds.Count,
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
                    createJournalDto.LinkedTaskIds.Count);

                // 2. Create Task Links (Polly handles transient retries)
                var linksCreated = await _taskJournalLinkServiceClient.LinkNewJournalWithTasksAsync(
                    journalId.Value,
                    createJournalDto.LinkedTaskIds,
                    cancellationToken);

                if (linksCreated is null || linksCreated.Length != createJournalDto.LinkedTaskIds.Count)
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
                    createJournalDto.LinkedTaskIds.Count,
                    createJournalDto.AddJournalEntryDto.UserId);

                return journalId;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Failed to create Journal '{Title}' with {TaskCount} task links for UserId {UserId}",
                    createJournalDto.AddJournalEntryDto.Title,
                    createJournalDto.LinkedTaskIds.Count,
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
                var links = await _taskJournalLinkServiceClient.GetLinksByJournalIdAsync(id, cancellationToken);
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

        public async Task<bool> IsJournalEntryExistsByIdAsync(Guid id, CancellationToken cancellationToken)
        {
            var now = DateTime.UtcNow;
            _logger.LogInformation("Checking if a journal-entry exists by journal ID={JournalId} at {Time}", id, now);
            try
            {
                var isExists = await _journalServiceClient.IsJournalEntryExistsByIdAsync(id, cancellationToken);
                if (!isExists)
                    _logger.LogInformation("Journal-entry does not exits by journal ID={JournalId} at {Time}", id, now);
                return isExists;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to check if a journal-entry exists by journal ID={JournalId} at {Time}", id, now);
                throw;
            }
        }

        public async Task<bool> TryUpdateJournalAndLinksAsync(UpdateJournalDto dto, CancellationToken cancellationToken)
        {
            var now = DateTime.UtcNow;
            var links = $"[{string.Join(",", dto.LinkedTaskIds)}]";
            _logger.LogInformation("Updating journal-entry {Id} with journal-Entry Title={Title}, & TaskLinks={TaskLinks} at {Time}",
                dto.UpdateJournalEntryDto.JournalEntryId, dto.UpdateJournalEntryDto.Title, links, now);

            try
            {
                // 1 update journal entry
                var journalUpdateSuccess = await _journalServiceClient.UpdateJournalEntryAsync(dto.UpdateJournalEntryDto, cancellationToken);                 
                if (!journalUpdateSuccess)
                {
                    _logger.LogError("Failed Updating journal-entry {Id} with journal-Entry Title={Title}, & TaskLinks={TaskLinks} at {Time}",
                        dto.UpdateJournalEntryDto.JournalEntryId, dto.UpdateJournalEntryDto.Title, links, now);
                    return false;
                }

                // 2 update task journal links
                var linkRearrangeSuccess = await _taskJournalLinkServiceClient.RearrangeTaskJournalLinksAsync(
                    dto.UpdateJournalEntryDto.JournalEntryId, dto.LinkedTaskIds, cancellationToken);
                if (!linkRearrangeSuccess)
                {
                    _logger.LogError("Journal with Id:{Id} successfully updated, but task links were not updated properly on cosmos DB at {Time}", 
                        dto.UpdateJournalEntryDto.JournalEntryId, now);
                    return false;
                }

                _logger.LogInformation("Journal with Id:{Id} along with task journal links: {TaskJournalLinks} successfully updated at {Time}",
                       dto.UpdateJournalEntryDto.JournalEntryId, links, now);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating Journal-Entry with ID: {Id} at {Time}", dto.UpdateJournalEntryDto.JournalEntryId, now);
                throw;
            }
        }

        public async Task<IReadOnlyList<JournalWithTasksDto>> GetJournalsByTeamAsync(Guid managerId, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Finding journals for team by manager {ManagerId} at {Time}", managerId, DateTime.UtcNow);

            var cacheKey = $"TeamMembersForManagerId:{managerId}";

            try
            {
                _logger.LogInformation("Step 1 - retrieving team members for manager {ManagerId}", managerId);

                if (!_inMemoryCache.TryGetValue(cacheKey, out IReadOnlyList<UserAccountDto>? teamMembers) || teamMembers is null)
                {
                    _logger.LogInformation("Cache miss for manager {ManagerId}. Requesting from User Microservice.", managerId);
                    teamMembers = await _userServiceClient.GetTeamMembersForManager(managerId, cancellationToken);

                    if (teamMembers is not null)
                    {
                        _logger.LogInformation("Caching {TeamMemberCount} team members for manager {ManagerId}", teamMembers.Count, managerId);
                        _inMemoryCache.Set(cacheKey, teamMembers, TimeSpan.FromMinutes(CachedTimeInMins));
                    }
                }
                else
                {
                    _logger.LogInformation("Cache hit for manager {ManagerId}", managerId);
                }

                if (teamMembers is null || !teamMembers.Any())
                {
                    _logger.LogInformation("No team members found for manager {ManagerId}", managerId);
                    return [];
                }


                var teamMemberIds = teamMembers.Select(x => x.Id);
                var teamMemberIdsStr = string.Join(",", teamMemberIds);
                _logger.LogInformation("Step 2 - retrieving journals for team members: {TeamMembers}", teamMemberIdsStr);

                var journals = await _journalServiceClient.GetJournalsByTeamAsync([.. teamMemberIds], cancellationToken);

                if (journals is null || !journals.Any())
                {
                    _logger.LogInformation("No journals retrived for team members: {TeamMembers}", teamMemberIdsStr);
                    return [];
                }


                _logger.LogInformation("Step 3 - Hydrating journals with Task Ids");

                _logger.LogInformation("Step 4 - Hydrating journals jounral feedbacks");

                return [];
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to retrieve team tasks for manager {ManagerId}", managerId);
                throw;
            }
        }

        /// <summary>
        /// Retrieves all journal entries created by the specified user and returns them in a
        /// fully hydrated form. This includes the journal record, its feedback (if available),
        /// the feedback manager's details, and all tasks linked to each journal entry.
        /// </summary>
        /// <param name="userId">
        /// The unique identifier of the user whose journals should be retrieved and hydrated.
        /// </param>
        /// <param name="cancellationToken">
        /// A token used to observe cancellation requests for the asynchronous operation.
        /// </param>
        /// <returns>
        /// A read-only list of <see cref="JournalEntryWithTasksAndFeedbackDto"/> objects,
        /// where each journal entry is enriched with its linked tasks, feedback information,
        /// and the feedback manager's display name. Returns an empty list if the user has no journals.
        /// </returns>
        public async Task<IReadOnlyList<JournalEntryWithTasksAndFeedbackDto>> GetMyJournalsAsync(Guid userId, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Finding fully hydarted journals for user {UserId} at {Time}", userId, DateTime.UtcNow);
            var cacheKey = $"HydratedJournalsForUserId:{userId}";
            try
            {
                // step 0 - try to find data from cache
                if (_inMemoryCache.TryGetValue(cacheKey, out IReadOnlyList<JournalEntryWithTasksAndFeedbackDto>? cached) && cached is not null)
                {
                    _logger.LogInformation("Cache hit for user {UserId}", userId);
                    return cached;
                }

                _logger.LogInformation("Cache miss for user {UserId}", userId);

                // step 1 - get all journals by userId, with feedbacks for user = from Journal Micro Service
                _logger.LogInformation("Step 1 - get all journals by userId: {UserId}, with feedbacks for user, from Journal Micro Service at {Time}", userId, DateTime.UtcNow);
                IReadOnlyList<JournalEntryWithFeedbackDto> journalsWithFeedbacks = await _journalServiceClient.GetJournalsWithFeedback(userId, cancellationToken);
                if (!journalsWithFeedbacks.Any())
                {
                    _logger.LogInformation("No journals written by user: {UserId}", userId);
                    return [];
                }

                // step 2 - get task journal links for all user journals = from task journal link Micro Service
                var journalIds = journalsWithFeedbacks.Select(x => x.journal.Id);
                IReadOnlyList<TaskJournalLinkDocument> taskJournalLinks = [];
                if (journalIds is not null && journalIds.Any())
                {
                    _logger.LogInformation("Step 2 - get task journal links for user: {UserId} journals from task journal link Micro Service at {Time}", userId, DateTime.UtcNow);
                    taskJournalLinks = await _taskJournalLinkServiceClient.GetLinksForJournalIdsAsync(journalIds, cancellationToken);
                }

                // step 3 - get tasks for all task journal links = from task Micro Service
                var taskIds = taskJournalLinks.Select(x => x.TaskId).Distinct();
                IReadOnlyList<TaskItemDto> tasks = [];
                if (taskIds is not null && taskIds.Any())
                {
                    _logger.LogInformation("Step 3 - get tasks for all task journal links of user: {UserId} from task Micro Service at {Time}", userId, DateTime.UtcNow);
                    tasks = await _taskServiceClient.GetTasksAsync(taskIds, cancellationToken) ?? [];
                }

                // step 4 - get feedback manager details, if feedback available = from user Micro Service
                IEnumerable<Guid> managerIds = journalsWithFeedbacks
                                                .Select(x => x.feedback?.FeedbackManagerId)
                                                .Where(id => id.HasValue)
                                                .Select(id => id!.Value)
                                                .Distinct();
                IReadOnlyList<UserAccountDto> managers = [];
                if (managerIds is not null && managerIds.Any())
                {
                    _logger.LogInformation("Step 4 - get feedback manager details for user: {UserId}, if feedback available = from user Micro Service at {Time}", userId, DateTime.UtcNow);
                    managers = await _userServiceClient.GetUsersByIds(managerIds, cancellationToken);
                }

                // step 5 - hydrate the DTO from all above data coming from different micro services
                _logger.LogInformation("Step 5 - Getting fully hydarted journals for user {UserId} at {Time}", userId, DateTime.UtcNow);
                IReadOnlyList<JournalEntryWithTasksAndFeedbackDto> dtos = HydrateJournalsWithFeedbacks(
                                                        journalsWithFeedbacks, taskJournalLinks, tasks, managers);
                _inMemoryCache.Set(cacheKey, dtos, TimeSpan.FromMinutes(CachedTimeInMins));
                return dtos;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to find fully hydarted journals for user {UserId} at {Time}", userId, DateTime.UtcNow);
                throw;
            }
        }

        /// <summary>
        /// Hydrates journal entries by attaching their linked tasks and feedback metadata,
        /// including resolving the feedback manager's display name. This method combines
        /// journal data, task–journal link documents, task details, and manager accounts
        /// into a fully enriched DTO suitable for UI consumption.
        /// </summary>
        /// <param name="journalsWithFeedbacks">
        /// The list of journal entries along with their associated feedback information.
        /// Each item contains the base journal record and an optional feedback object.
        /// </param>
        /// <param name="taskJournalLinks">
        /// The list of task–journal link documents used to determine which tasks are
        /// associated with each journal entry.
        /// </param>
        /// <param name="tasks">
        /// The list of task DTOs from which task details are selected for each journal.
        /// </param>
        /// <param name="managers">
        /// The list of manager user accounts used to resolve the display name of the
        /// feedback provider for each journal entry.
        /// </param>
        /// <returns>
        /// A read-only list of <see cref="JournalEntryWithTasksAndFeedbackDto"/> objects,
        /// where each journal entry is enriched with its linked tasks, feedback details,
        /// and the feedback manager's display name.
        /// </returns>
        private IReadOnlyList<JournalEntryWithTasksAndFeedbackDto> HydrateJournalsWithFeedbacks(
            IReadOnlyList<JournalEntryWithFeedbackDto> journalsWithFeedbacks, 
            IReadOnlyList<TaskJournalLinkDocument> taskJournalLinks, 
            IReadOnlyList<TaskItemDto> tasks,
            IReadOnlyList<UserAccountDto> managers)
        {
            //IReadOnlyList<JournalEntryWithTasksAndFeedbackDto> result = [.. from journalEntry in journalsWithFeedbacks
            //                                                            join link in taskJournalLinks on journalEntry.journal.Id equals link.JounrnalId
            //                                                            join taskItem in tasks on link.TaskId equals taskItem.Id
            //                                                            join manager in managers on journalEntry?.feedback?.FeedbackManagerId equals manager.ManagerId
            //                                                            select new JournalEntryWithTasksAndFeedbackDto(journalEntry.journal.Id,
            //                                                                journalEntry.journal.UserId,
            //                                                                journalEntry.journal.CreatedAt,
            //                                                               journalEntry.journal.Title,
            //                                                               journalEntry.journal.Content,
            //                                                               journalEntry.journal.IsDeleted,
            //                                                               journalEntry.feedback,
            //                                                                manager?.DisplayName,
            //                                                                [])];


            var result = journalsWithFeedbacks
                                        .Select(journal =>
                                        {
                                            // Find manager given journal feedback
                                            var managerName =
                                                managers.FirstOrDefault(m =>
                                                    m.Id == journal.feedback?.FeedbackManagerId
                                                )?.DisplayName;

                                            // Find linked task Ids for current journal
                                            var linkedTaskIds =
                                                taskJournalLinks
                                                    .Where(l => l.JounrnalId == journal.journal.Id)
                                                    .Select(l => l.TaskId)
                                                    .ToList();

                                            // Find tasks for linked task Ids of current journal
                                            var linkedTasks =
                                                tasks
                                                    .Where(t => linkedTaskIds.Contains(t.Id))
                                                    .ToList();

                                            return new JournalEntryWithTasksAndFeedbackDto(
                                                journal.journal.Id,
                                                journal.journal.UserId,
                                                journal.journal.CreatedAt,
                                                journal.journal.Title,
                                                journal.journal.Content,
                                                journal.journal.IsDeleted,
                                                journal.feedback,
                                                managerName,
                                                linkedTasks
                                            );
                                        })
                                        .ToList();
            return result;
        }
    }    
}