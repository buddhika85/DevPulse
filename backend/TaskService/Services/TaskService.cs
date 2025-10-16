using System.Threading.Tasks;
using TaskService.Application.Commands;
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

        public TaskService(ITaskRepository repository)
        {
            _taskRepository = repository;
        }

        public async Task<IReadOnlyList<TaskItemDto>> GetAllTasksAsync(CancellationToken cancellationToken)
        {
            var entities = await _taskRepository.GetAllAsync(cancellationToken);

            var dtos = entities.Select(task => new TaskItemDto
            {
                Id = task.Id,
                Title = task.Title,
                Description = task.Description,
                IsCompleted = task.IsCompleted,
                CreatedAt = task.CreatedAt
            }).ToList();

            return dtos;
        }

        public async Task<TaskItemDto?> GetTaskByIdAsync(GetTaskByIdQuery query, CancellationToken cancellationToken)
        {
            var entity = await _taskRepository.GetByIdAsync(query.Id, cancellationToken);
            if (entity is null)
                return null;
            return new TaskItemDto
            {
                Id = entity.Id,
                Title = entity.Title,
                Description = entity.Description,
                IsCompleted = entity.IsCompleted,
                CreatedAt = entity.CreatedAt
            };
        }


        public async Task<Guid> CreateTaskAsync(CreateTaskCommand command, CancellationToken cancellationToken)
        {
            var task = new TaskItem
            {
                Id = Guid.NewGuid(),
                Title = command.Title,
                Description = command.Description ?? string.Empty,
                IsCompleted = false,
                CreatedAt = DateTime.UtcNow
            };

            _taskRepository.Add(task);
            await _taskRepository.SaveChangesAsync(cancellationToken);

            return task.Id;
        }

        public async Task<bool> UpdateTaskAsync(UpdateTaskCommand command, CancellationToken cancellationToken)
        {
            var entity = await _taskRepository.GetByIdAsync(command.Id, cancellationToken);
            if (entity is null)
                return false;

            entity.Title = command.Title;
            entity.Description = command.Description;
            entity.IsCompleted = command.IsCompleted;
            _taskRepository.Update(entity);
            await _taskRepository.SaveChangesAsync(cancellationToken);
            return true;
        }

        public async Task<bool> DeleteTaskAsync(DeleteTaskCommand command, CancellationToken cancellationToken)
        {
            var entity = await _taskRepository.GetByIdAsync(command.Id, cancellationToken);
            if (entity is null)
                return false;

            _taskRepository.Delete(entity);
            await _taskRepository.SaveChangesAsync(cancellationToken);
            return true;
        }

        public async Task<PaginatedResult<TaskItemDto>> GetTasksPaginatedAsync(GetTasksPaginatedQuery query, CancellationToken cancellationToken)
        {
            var pagedEntities = await _taskRepository.GetTasksPaginatedAsync(query, cancellationToken);
            return new PaginatedResult<TaskItemDto>
            {
                PageItems = pagedEntities.PageItems.Select(task => new TaskItemDto {
                    Id = task.Id,
                    Title = task.Title,
                    Description = task.Description,
                    IsCompleted = task.IsCompleted,
                    CreatedAt = task.CreatedAt
                }).ToList(),
                PageNumber = pagedEntities.PageNumber,
                PageSize = pagedEntities.PageSize,
                TotalCount = pagedEntities.TotalCount,
            };
        }
    }
}
