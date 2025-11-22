using MediatR;
using SharedLib.DTOs.Task;

namespace TaskService.Application.Queries
{
    // Fluent Validator is GetTaskByIdQueryValidator
    public record GetTaskByIdQuery(Guid Id) : IRequest<TaskItemDto?>;
}
