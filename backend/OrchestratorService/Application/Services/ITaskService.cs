using SharedLib.DTOs.Task;

namespace OrchestratorService.Application.Services
{
    public interface ITaskService
    {
        Task<IReadOnlyList<TaskItemWithUserDto>> GetTasksByTeam(Guid managerId, bool includeDeleted, CancellationToken cancellationToken);
    }
}
