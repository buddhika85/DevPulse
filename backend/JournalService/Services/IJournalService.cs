using JournalService.Application.Commands.Journal;
using JournalService.Application.Queries.Journal;
using SharedLib.DTOs.Journal;

namespace JournalService.Services
{
    public interface IJournalService
    {
        Task<IReadOnlyList<JournalEntryDto>> GetAllJournalEntriesAsync(CancellationToken cancellationToken);

        Task<JournalEntryDto?> GetJournalEntryByIdAsync(GetJournalEntryByIdQuery query, CancellationToken cancellationToken);

        Task<bool> IsJournalEntryExistsByIdAsync(IsJournalEntryExistsByIdQuery query, CancellationToken cancellationToken);

        Task<IReadOnlyList<JournalEntryDto>> GetJournalEntriesByUserIdAsync(GetJournalEntriesByUserIdQuery query, CancellationToken cancellationToken);

        // business rule - journal entry can have exctly one feedback        
        // Task<bool> AttachJournalFeedback(JournalFeedback journalFeedback, CancellationToken cancellationToken); 

        Task<Guid?> AddJournalEntryAsync(AddJournalEntryCommand command, CancellationToken cancellationToken);

        Task<bool> UpdateJournalEntryAsync(UpdateJournalEntryCommand command, CancellationToken cancellationToken);

        Task<bool> DeleteAsync(DeleteJournalEntryCommand command, CancellationToken cancellationToken);

        Task<bool> RestoreAsync(RestoreJournalEntryCommand command, CancellationToken cancellationToken);
    }
}
