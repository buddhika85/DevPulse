using JournalService.Application.Queries.Journal;
using SharedLib.DTOs.Journal;

namespace JournalService.Services
{
    public interface IJournalService
    {
        Task<IReadOnlyList<JournalEntryDto>> GetAllJournalEntriesAsync(CancellationToken cancellationToken);

        Task<JournalEntryDto> GetJournalEntryByIdAsync(GetJournalEntriesByUserIdQuery query, CancellationToken cancellationToken);

        Task<IReadOnlyList<JournalEntryDto>> GetJournalEntriesByUserIdAsync(GetJournalEntriesByUserIdQuery query, CancellationToken cancellationToken);

        // business rule - journal entry can have exctly one feedback        
        // Task<bool> AttachJournalFeedback(JournalFeedback journalFeedback, CancellationToken cancellationToken); 
    }

    //public class JournalService : IJournalService
    //{

    //}
}
