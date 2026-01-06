using TaskJournalLinkService.Domain.Models;

namespace TaskJournalLinkService.Repositories
{
    public interface ITaskJournalLinkRepository
    {
        Task<TaskJournalLinkDocument[]> GetLinksByJournalIdAsync(Guid journalId, CancellationToken cancellationToken);
        Task<TaskJournalLinkDocument[]> LinkNewJournalWithTasksAsync(Guid journalId, Guid[] TaskIdsToLink, CancellationToken cancellationToken);
    }
}
