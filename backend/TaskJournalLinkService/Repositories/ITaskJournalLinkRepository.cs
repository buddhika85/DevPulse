using TaskJournalLinkService.Domain.Models;

namespace TaskJournalLinkService.Repositories
{
    public interface ITaskJournalLinkRepository
    {       
        Task<TaskJournalLinkDocument[]> GetLinksByJournalIdAsync(Guid journalId, CancellationToken cancellationToken);     
        Task<TaskJournalLinkDocument[]> LinkNewJournalWithTasksAsync(Guid journalId, HashSet<Guid> TaskIdsToLink, CancellationToken cancellationToken);
        Task<bool> RearrangeTaskJournalLinksAsync(Guid journalId, List<TaskJournalLinkDocument> removeSet, List<TaskJournalLinkDocument> addSet, CancellationToken cancellationToken);       
    }
}
