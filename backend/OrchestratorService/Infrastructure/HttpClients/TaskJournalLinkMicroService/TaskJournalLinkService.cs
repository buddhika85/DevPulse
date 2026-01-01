using SharedLib.DTOs.TaskJournalLink;

namespace OrchestratorService.Infrastructure.HttpClients.TaskJournalLinkMicroService
{
    public class TaskJournalLinkService : ITaskJournalLinkService
    {
        public Task<TaskJournalLinkDto[]> LinkNewJournalWithTasks(Guid? jounralId, Guid[] linkedTaskIds)
        {
            throw new NotImplementedException();
        }
    }
}
