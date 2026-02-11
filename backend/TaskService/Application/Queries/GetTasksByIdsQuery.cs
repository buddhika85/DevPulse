using MediatR;
using SharedLib.DTOs.Task;

namespace TaskService.Application.Queries
{
    // Fluent Validator is GetTasksByIdsQueryValidator
    public record GetTasksByIdsQuery(Guid[] taskIds, bool includeDeleted) : IRequest<IReadOnlyList<TaskItemDto>>;
}
