using SharedLib.DTOs.Task;


namespace OrchestratorService.Infrastructure.HttpClients
{
    public interface ITaskServiceClient
    {
        Task<List<TaskItemDto>> GetTasksAsync(string userId, CancellationToken cancellationToken);
    }

}
