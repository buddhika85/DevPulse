using OrchestratorService.Application.DTOs;
using SharedLib.DTOs.Journal;

namespace OrchestratorService.Application.Services
{
    public interface IJournalService
    {
        Task<Guid?> AddJournalEntryWithTaskLinksAsync(CreateJournalDto dto, CancellationToken cancellationToken);
        Task<JournalWithTasksDto?> GetJournalByIdAsync(Guid id, CancellationToken cancellationToken);
    }
}