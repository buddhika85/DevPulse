using TaskService.Application.Commands;
using TaskService.Application.Dtos;
using TaskService.Domain.Entities;

namespace TaskService.Application.Common.Mappers
{
    public static class TaskMapper
    {
        public static TaskItem ToEntity(CreateTaskCommand command)
        {
            return new TaskItem
            {
                Title = command.Title,
                Description = command.Description ?? string.Empty
            };
        }

        public static void MapToEntity(TaskItem entity, UpdateTaskCommand command)
        {
            entity.Title = command.Title;
            entity.Description = command.Description;
            entity.IsCompleted = command.IsCompleted;
        }

        public static TaskItemDto ToDto(TaskItem entity) 
        {
            return new TaskItemDto
            {
                Id = entity.Id,
                Title = entity.Title,
                Description = entity.Description,
                CreatedAt = entity.CreatedAt,
                IsCompleted = entity.IsCompleted,
            };
        }

        public static IEnumerable<TaskItemDto> ToDtosList(IEnumerable<TaskItem> entities)
        {
            return entities.Select(ToDto);
        }
    }
}
