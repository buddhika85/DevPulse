using TaskService.Application.Commands;
using TaskService.Application.Common.Mappers;
using TaskService.Application.Common.Models;
using TaskService.Application.Dtos;
using TaskService.Application.Queries;
using TaskService.Domain.Entities;
using TaskService.Repositories;

namespace TaskService.Services
{
    public class TaskService : ITaskService
    {
        private readonly ITaskRepository _taskRepository;
        private readonly ILogger<TaskService> _logger;

        public TaskService(ITaskRepository repository, ILogger<TaskService> logger)
        {
            _taskRepository = repository;
            _logger = logger;
        }

        public async Task<IReadOnlyList<TaskItemDto>> GetAllTasksAsync(CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Fetching all tasks...");
                var entities = await _taskRepository.GetAllAsync(cancellationToken);
                var dtos = TaskMapper.ToDtosList(entities);
                _logger.LogInformation("Retrieved {Count} tasks.", dtos.Count());
                return dtos.ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching all tasks.");
                return [];
            }
        }

        public async Task<TaskItemDto?> GetTaskByIdAsync(GetTaskByIdQuery query, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Fetching task by ID: {Id}", query.Id);
                var entity = await _taskRepository.GetByIdAsync(query.Id, cancellationToken);
                if (entity is null)
                {
                    _logger.LogWarning("Task not found for ID: {Id}", query.Id);
                    return null;
                }
                _logger.LogInformation("Task found for ID: {Id}", query.Id);
                return TaskMapper.ToDto(entity);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching task ID: {Id}", query.Id);
                return null;
            }
        }

        public async Task<Guid?> CreateTaskAsync(CreateTaskCommand command, CancellationToken cancellationToken)
        {
            try
            {
                var taskId = Guid.NewGuid();
                _logger.LogInformation("Creating new task: {Title}", command.Title);

                var task = new TaskItem
                {
                    Id = taskId,
                    Title = command.Title,
                    Description = command.Description ?? string.Empty,
                    TaskStatus = Domain.ValueObjects.TaskStatus.Pending,
                    CreatedAt = DateTime.UtcNow
                };

                var result = await _taskRepository.AddAsync(task, cancellationToken);
                if (result is not null)
                {
                    _logger.LogInformation("Task created successfully with ID: {Id}", result.Id);
                    return result.Id;
                }

                _logger.LogWarning("Task creation failed for Title: {Title}", command.Title);
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while creating task: {Title}", command.Title);
                return null;
            }
        }

        public async Task<bool> UpdateTaskAsync(UpdateTaskCommand command, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Updating task ID: {Id}", command.Id);
                var entity = await _taskRepository.GetByIdAsync(command.Id, cancellationToken);
                if (entity is null)
                {
                    _logger.LogWarning("Task not found for update: {Id}", command.Id);
                    return false;
                }

                TaskMapper.MapToEntity(entity, command);
                var success = await _taskRepository.UpdateAsync(command.Id, entity, cancellationToken);
                _logger.LogInformation("Task update {Status} for ID: {Id}", success ? "succeeded" : "failed", command.Id);
                return success;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating task ID: {Id}", command.Id);
                return false;
            }
        }

        public async Task<bool> DeleteTaskAsync(DeleteTaskCommand command, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Deleting task ID: {Id}", command.Id);
                var success = await _taskRepository.DeleteAsync(command.Id, cancellationToken);
                _logger.LogInformation("Task deletion {Status} for ID: {Id}", success ? "succeeded" : "failed", command.Id);
                return success;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deleting task ID: {Id}", command.Id);
                return false;
            }
        }

        public async Task<PaginatedResult<TaskItemDto>> GetTasksPaginatedAsync(GetTasksPaginatedQuery query, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Fetching paginated tasks: Page {PageNumber}, Size {PageSize}", query.PageNumber, query.PageSize);
                var pagedEntities = await _taskRepository.GetTasksPaginatedAsync(query, cancellationToken);
                var dtos = TaskMapper.ToDtosList(pagedEntities.PageItems).ToList();
                _logger.LogInformation("Retrieved {Count} tasks for page {PageNumber}", dtos.Count, pagedEntities.PageNumber);

                return new PaginatedResult<TaskItemDto>
                {
                    PageItems = dtos,
                    PageNumber = pagedEntities.PageNumber,
                    PageSize = pagedEntities.PageSize,
                    TotalCount = pagedEntities.TotalCount,
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching paginated tasks: Page {PageNumber}", query.PageNumber);
                return new PaginatedResult<TaskItemDto>
                {
                    PageItems =[],
                    PageNumber = query.PageNumber,
                    PageSize = query.PageSize,
                    TotalCount = 0
                };
            }
        }
    }
}
