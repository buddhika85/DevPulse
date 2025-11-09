using MediatR;
using SharedLib.DTOs.Task;
using TaskService.Application.Common.Enums;
using TaskService.Application.Common.Models;


namespace TaskService.Application.Queries
{
    // Fluent Validator is GetTasksPaginatedQueryValidator
    public record GetTasksPaginatedQuery(
        Guid? TaskId,
        string? Title,
        string? Description,        
        int PageNumber,
        int PageSize,
        TaskSortField? SortBy,
        bool SortDescending,
        string? Status = "Pending"
    ) : IRequest<PaginatedResult<TaskItemDto>>;
}
