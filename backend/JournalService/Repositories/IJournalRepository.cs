using JournalService.Domain.Entities;
using SharedLib.Application.Interfaces;

namespace JournalService.Repositories
{
    public interface IJournalRepository : IBaseRepository<JournalEntry>
    {
        Task<IReadOnlyList<JournalEntry>> GetJournalEntriesByUserIdAsync(Guid userId, CancellationToken cancellationToken, bool includeDeleted = false, bool includeFeedbacks = false);


        // business rule - journal entry can have exctly one feedback        
        // Task<bool> AttachJournalFeedback(JournalFeedback journalFeedback, CancellationToken cancellationToken);  // we dont need this as its a workflow method, this exists in journal service

        // soft-delete restore
        Task<bool> RestoreAsync(Guid id, CancellationToken cancellationToken);
    }
}
