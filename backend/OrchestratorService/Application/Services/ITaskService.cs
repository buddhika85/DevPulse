using SharedLib.DTOs.Task;

namespace OrchestratorService.Application.Services
{
    public interface ITaskService
    {
        Task<IReadOnlyList<TaskItemDto>> GetTasksByTeam(Guid managerId, bool includeDeleted, CancellationToken cancellationToken);
    }
}
