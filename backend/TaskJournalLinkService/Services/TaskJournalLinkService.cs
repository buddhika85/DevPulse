using Microsoft.EntityFrameworkCore;
using SharedLib.DTOs.Journal;
using SharedLib.DTOs.TaskJournalLink;
using TaskJournalLinkService.Domain.Models;
using TaskJournalLinkService.Mapper;
using TaskJournalLinkService.Repositories;

namespace TaskJournalLinkService.Services
{
    public class TaskJournalLinkService : ITaskJournalLinkService
    {
        private readonly ITaskJournalLinkRepository _repository;
        private readonly ILogger<TaskJournalLinkService> _logger;

        public TaskJournalLinkService(ITaskJournalLinkRepository repository, ILogger<TaskJournalLinkService> logger)
        {
            _repository = repository;
            _logger = logger;
        }


        public async Task<TaskJournalLinkDto[]> GetLinksByJournalIdAsync(Guid journalId, CancellationToken cancellationToken)
        {
            _logger.LogInformation(
                "Retrieving TaskJournalLinks for JournalId {JournalId}",
                journalId);

            try
            {
                // Query repository
                var entities = await _repository.GetLinksByJournalIdAsync(
                    journalId,
                    cancellationToken);

                _logger.LogInformation(
                    "Retrieved {Count} TaskJournalLink documents for JournalId {JournalId}",
                    entities.Length,
                    journalId);

                // Map to DTOs
                var dtos = TaskJournalLinkMapper
                    .ToDtos(entities)
                    .ToArray();

                _logger.LogInformation(
                    "Mapped {Count} TaskJournalLink DTOs for JournalId {JournalId}",
                    dtos.Length,
                    journalId);

                return dtos;
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Error retrieving TaskJournalLinks for JournalId {JournalId}",
                    journalId);

                throw;
            }
        }


        public async Task<TaskJournalLinkDto[]> LinkNewJournalWithTasksAsync(LinkTasksToJournalDto dto, CancellationToken cancellationToken)
        {
            _logger.LogInformation(
                "Linking Journal {JournalId} with {TaskCount} TaskIds",
                dto.JournalId,
                dto.TaskIdsToLink.Count);

            try
            {
                // Call repository to create the link documents
                var entities = await _repository.LinkNewJournalWithTasksAsync(
                    dto.JournalId,
                    dto.TaskIdsToLink,
                    cancellationToken);

                _logger.LogInformation(
                    "Created {LinkCount} TaskJournalLink documents for Journal {JournalId}",
                    entities.Length,
                    dto.JournalId);

                // Map to DTOs
                var dtos = TaskJournalLinkMapper.ToDtos(entities).ToArray();

                _logger.LogInformation(
                    "Mapped {DtoCount} TaskJournalLink DTOs for Journal {JournalId}",
                    dtos.Length,
                    dto.JournalId);

                return dtos;
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Error linking Journal {JournalId} with TaskIds {TaskIds}",
                    dto.JournalId,
                    string.Join(',', dto.TaskIdsToLink));

                throw;
            }
        }

        /// <summary>
        /// Rearrange task journal links.
        /// If there are no task journal links for jounral Id - they will be created.
        /// If there are existing journal links for jounral Id - the diff will be calculated and added. Whatever link unncessary will be removed.
        /// </summary>
        /// <param name="journalId">Guid journalId</param>
        /// <param name="tasksToLink">Guid[] tasksToLink</param>
        /// <param name="cancellationToken">cancellationToken</param>
        /// <returns>bool</returns>
        public async Task<bool> RearrangeTaskJournalLinksAsync(Guid journalId,
                                                                 HashSet<Guid> tasksToLink,
                                                                 CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation(
                    "Starting rearrangement of TaskJournalLinks for JournalId={JournalId} with {TaskCount} tasks",
                    journalId, tasksToLink.Count);

                var existingLinks = await _repository.GetLinksByJournalIdAsync(journalId, cancellationToken);

                // Case 1: No existing links → just create new ones
                if (existingLinks == null)
                {
                    _logger.LogInformation(
                        "No existing links found for JournalId={JournalId}. Creating {TaskCount} new links.",
                        journalId, tasksToLink.Count);

                    var created = await _repository.LinkNewJournalWithTasksAsync(journalId, tasksToLink, cancellationToken);

                    var success = created != null && created.Length == tasksToLink.Count;

                    if (!success)
                    {
                        _logger.LogWarning(
                            "Failed to create all links for JournalId={JournalId}. Expected={Expected}, Created={Created}",
                            journalId, tasksToLink.Count, created?.Length ?? 0);
                    }

                    return success;
                }

                // Case 2: Existing links → compute diff
                var keepSet = existingLinks.Where(x => tasksToLink.Contains(x.TaskId)).ToList();
                var removeSet = existingLinks.Where(x => !tasksToLink.Contains(x.TaskId)).ToList();
                var addSet = tasksToLink.Where(x => !keepSet.Any(curr => curr.TaskId == x))
                    .Select(taskId => new TaskJournalLinkDocument(Guid.NewGuid(), taskId, journalId.ToString(), DateTime.UtcNow))
                    .ToList();

                _logger.LogInformation(
                    "Rearranging links for JournalId={JournalId}: Keep={Keep}, Remove={Remove}, Add={Add}",
                    journalId, keepSet.Count, removeSet.Count, addSet.Count);

                // Check if there is anything to add or remove
                if (removeSet.Count == 0 && addSet.Count == 0)
                {
                    _logger.LogInformation("No TaskJournalLink changes required for JournalId={JournalId}. RemoveSet=0, AddSet=0 — links already in desired state.",
                                                journalId);
                    return true;
                }

                // Remove old links & Add new links
                var rearranged = await _repository.RearrangeTaskJournalLinksAsync(journalId, removeSet, addSet, cancellationToken);
                if (!rearranged)
                {
                    _logger.LogWarning(
                        "Failed to remove or add links for JournalId={JournalId}",
                        journalId);
                    return false;
                }

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Unexpected error while rearranging TaskJournalLinks for JournalId={JournalId}",
                    journalId);

                throw;
            }
        }
    }
}
