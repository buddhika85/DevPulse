namespace TaskJournalLinkService.Repositories
{
    public interface ITaskJournalLinkRepository
    {       
        Task<Domain.Models.TaskJournalLinkDocument[]> GetLinksByJournalIdAsync(Guid journalId, CancellationToken cancellationToken);
        Task<IReadOnlyList<Domain.Models.TaskJournalLinkDocument>> GetLinksByJournalIdAsync(IReadOnlyList<Guid> journalIds, CancellationToken cancellationToken);
        Task<Domain.Models.TaskJournalLinkDocument[]> LinkNewJournalWithTasksAsync(Guid journalId, HashSet<Guid> TaskIdsToLink, CancellationToken cancellationToken);
        Task<bool> RearrangeTaskJournalLinksAsync(Guid journalId, List<Domain.Models.TaskJournalLinkDocument> removeSet, List<Domain.Models.TaskJournalLinkDocument> addSet, CancellationToken cancellationToken);       
    }
}
