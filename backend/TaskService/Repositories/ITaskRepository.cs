using SharedLib.Application.Interfaces;
using SharedLib.Application.Models;
using TaskService.Application.Queries;
using TaskService.Domain.Entities;

namespace TaskService.Repositories
{
    public interface ITaskRepository : IBaseRepository<TaskItem>
    {
        Task<IReadOnlyList<TaskItem>> GetTasksByIdsAsync(Guid[] taskIds, bool includeDeleted, CancellationToken cancellationToken);
        Task<IReadOnlyList<TaskItem>> GetTasksByUserIdAsync(Guid userId, CancellationToken cancellationToken, bool includeDeleted = false);
        Task<PaginatedResult<TaskItem>> GetTasksPaginatedAsync(GetTasksPaginatedQuery query, CancellationToken cancellationToken);
        Task<bool> RestoreAsync(Guid id, CancellationToken cancellationToken);
    }
}
