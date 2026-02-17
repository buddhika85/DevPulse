using SharedLib.DTOs.TaskJournalLink;

namespace TaskJournalLinkService.Services
{
    public interface ITaskJournalLinkService
    {
        Task<TaskJournalLinkDto[]> GetLinksByJournalIdAsync(Guid journalId, CancellationToken cancellationToken);
        Task<TaskJournalLinkDto[]> LinkNewJournalWithTasksAsync(SharedLib.DTOs.Journal.LinkTasksToJournalDto linkTasksToJournalDto, CancellationToken cancellationToken);
        Task<bool> RearrangeTaskJournalLinksAsync(Guid journalId, HashSet<Guid> tasksToLink, CancellationToken cancellationToken);
    }
}
