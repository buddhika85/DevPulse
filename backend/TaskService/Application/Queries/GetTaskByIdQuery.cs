using MediatR;
using TaskService.Application.Dtos;

namespace TaskService.Application.Queries
{
    // Fluent Validator is GetTaskByIdQueryValidator
    public record GetTaskByIdQuery(Guid Id) : IRequest<TaskItemDto?>
    {
    }
}
