using SharedLib.DTOs.TaskJournalLink;

namespace OrchestratorService.Infrastructure.HttpClients.TaskJournalLinkMicroService
{
    public interface ITaskJournalLinkServiceClient
    {
        Task<TaskJournalLinkDto[]> GetLinksByJournalIdAsync(Guid id, CancellationToken cancellationToken);
        Task<TaskJournalLinkDto[]> LinkNewJournalWithTasks(Guid jounralId, Guid[] linkedTaskIds, CancellationToken cancellationToken);
        Task<bool> RearrangeTaskJournalLinks(Guid journalEntryId, Guid[] linkedTaskIds, CancellationToken cancellationToken);
    }
}
