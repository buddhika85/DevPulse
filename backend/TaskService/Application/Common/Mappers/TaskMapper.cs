using SharedLib.DTOs.Task;
using TaskService.Application.Commands;
using TaskService.Domain.Entities;
using TaskService.Domain.ValueObjects;

namespace TaskService.Application.Common.Mappers
{
    public static class TaskMapper
    {
        public static void MapToEntity(TaskItem entity, UpdateTaskCommand command)
        {
            entity.Update(command.Title, command.Description, Domain.ValueObjects.TaskStatus.From(command.Status), TaskPriority.From(command.Priority), command.dueDate);
        }

        public static TaskItemDto ToDto(TaskItem entity) 
        {
            return new TaskItemDto
            {
                Id = entity.Id,
                Title = entity.Title,
                Description = entity.Description,
                CreatedAt = entity.CreatedAt,
                Status = entity.TaskStatus.Value,

                UserId = entity.UserId,
                Priority = entity.TaskPriority.Value,
                DueDate = entity.DueDate
            };
        }

        public static IEnumerable<TaskItemDto> ToDtosList(IEnumerable<TaskItem> entities)
        {
            return entities.Select(ToDto);
        }
    }
}
