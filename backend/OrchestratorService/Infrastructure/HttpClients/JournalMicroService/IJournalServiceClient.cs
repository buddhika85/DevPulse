using SharedLib.DTOs.Journal;

namespace OrchestratorService.Infrastructure.HttpClients.JournalMicroService
{
    public interface IJournalServiceClient
    {
        Task<Guid?> AddJournalEntryAsync(AddJournalEntryDto addJournalEntryDto, CancellationToken cancellationToken);
        Task DeleteJournalEntryAsync(Guid? jounralId, CancellationToken cancellationToken);
        Task<JournalEntryDto?> GetJournalByIdAsync(Guid id, CancellationToken cancellationToken);
    }
}
