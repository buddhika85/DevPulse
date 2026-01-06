using SharedLib.DTOs.Journal;
using SharedLib.DTOs.TaskJournalLink;
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
                dto.TaskIdsToLink.Length);

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
    }
}
