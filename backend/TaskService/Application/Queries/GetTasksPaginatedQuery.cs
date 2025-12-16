using MediatR;
using SharedLib.Application.Models;
using SharedLib.DTOs.Task;
using TaskService.Application.Common.Enums;


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
