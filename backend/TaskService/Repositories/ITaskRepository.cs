using SharedLib.Application.Interfaces;
using TaskService.Application.Common.Models;
using TaskService.Application.Queries;
using TaskService.Domain.Entities;

namespace TaskService.Repositories
{
    public interface ITaskRepository : IBaseRepository<TaskItem>
    {
        Task<IReadOnlyList<TaskItem>> GetTasksByUserIdAsync(Guid userId, CancellationToken cancellationToken);
        Task<PaginatedResult<TaskItem>> GetTasksPaginatedAsync(GetTasksPaginatedQuery query, CancellationToken cancellationToken);
    }
}
