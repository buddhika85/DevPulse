using SharedLib.DTOs.Task;


namespace OrchestratorService.Infrastructure.HttpClients.TaskMicroService
{
    public interface ITaskServiceClient
    {
        Task<List<TaskItemDto>> GetTasksAsync(string userId, CancellationToken cancellationToken);
    }

}
