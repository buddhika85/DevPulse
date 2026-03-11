using SharedLib.DTOs.TaskJournalLink;

namespace TaskJournalLinkService.Services
{
    public interface ITaskJournalLinkService
    {
        Task<TaskJournalLinkDocument[]> GetLinksByJournalIdAsync(Guid journalId, CancellationToken cancellationToken);
        Task<IReadOnlyList<TaskJournalLinkDocument>> GetLinksForJournalIdsAsync(IReadOnlyList<Guid> journalIds, CancellationToken cancellationToken);
        Task<TaskJournalLinkDocument[]> LinkNewJournalWithTasksAsync(SharedLib.DTOs.Journal.LinkTasksToJournalDto linkTasksToJournalDto, CancellationToken cancellationToken);
        Task<bool> RearrangeTaskJournalLinksAsync(Guid journalId, HashSet<Guid> tasksToLink, CancellationToken cancellationToken);
    }
}
