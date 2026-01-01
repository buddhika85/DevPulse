using SharedLib.DTOs.TaskJournalLink;

namespace OrchestratorService.Infrastructure.HttpClients.TaskJournalLinkMicroService
{
    public interface ITaskJournalLinkService
    {
        Task<TaskJournalLinkDto[]> LinkNewJournalWithTasks(Guid? jounralId, Guid[] linkedTaskIds);
    }
}
