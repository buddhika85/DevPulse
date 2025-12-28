using JournalService.Domain.Entities;
using SharedLib.Application.Interfaces;

namespace JournalService.Repositories
{
    public interface IJournalFeedbackRepository : IBaseRepository<JournalFeedback>
    {
        // for simplicity 
        // journal feedback cannot be deleted or updated in this version

        // business rule - journal entry can have exctly one feedback
        // service layer must execute this before adding a new jounrnal feedback 
        Task<bool> IsFeedbackGiven(Guid journalId, CancellationToken cancellationToken);

        Task<IReadOnlyList<JournalFeedback>> GetJournalFeedbacksByManagerIdAsync(Guid managerId, CancellationToken cancellationToken, bool includeJounral = false);
        Task<bool> IsJounrnalFeedbackExistsAsync(Guid jounralFeedbackId, CancellationToken cancellationToken);
        Task<bool> MarkAsSeenByAsync(JournalFeedback entity, CancellationToken cancellationToken);
    }
}
