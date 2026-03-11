using SharedLib.DTOs.TaskJournalLink;

namespace OrchestratorService.Infrastructure.HttpClients.TaskJournalLinkMicroService
{
    public interface ITaskJournalLinkServiceClient
    {
        Task<TaskJournalLinkDocument[]> GetLinksByJournalIdAsync(Guid id, CancellationToken cancellationToken);
        Task<IReadOnlyList<TaskJournalLinkDocument>> GetLinksForJournalIdsAsync(IEnumerable<Guid> journalIds, CancellationToken cancellationToken);
        Task<TaskJournalLinkDocument[]> LinkNewJournalWithTasksAsync(Guid jounralId, HashSet<Guid> linkedTaskIds, CancellationToken cancellationToken);
        Task<bool> RearrangeTaskJournalLinksAsync(Guid journalEntryId, HashSet<Guid> linkedTaskIds, CancellationToken cancellationToken);
    }
}
