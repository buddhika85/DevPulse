using OrchestratorService.Application.DTOs;

namespace OrchestratorService.Application.Services
{
    public interface IJournalService
    {
        Task<Guid?> AddJournalEntryWithTaskLinksAsync(CreateJournalDto dto, CancellationToken cancellationToken);
    }
}