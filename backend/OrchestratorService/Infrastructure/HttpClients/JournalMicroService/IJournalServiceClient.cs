using OrchestratorService.Application.DTOs;
using SharedLib.DTOs.Journal;

namespace OrchestratorService.Infrastructure.HttpClients.JournalMicroService
{
    public interface IJournalServiceClient
    {
        Task<Guid?> AddJournalEntryAsync(AddJournalEntryDto addJournalEntryDto, CancellationToken cancellationToken);
        Task DeleteJournalEntryAsync(Guid? jounralId, CancellationToken cancellationToken);
        Task<JournalEntryDto?> GetJournalByIdAsync(Guid id, CancellationToken cancellationToken);
        Task<bool> IsJournalEntryExistsByIdAsync(Guid id, CancellationToken cancellationToken);
        Task<bool> UpdateJournalEntryAsync(UpdateJournalEntryDto dto, CancellationToken cancellationToken);
    }
}
