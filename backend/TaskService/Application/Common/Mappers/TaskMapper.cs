using TaskService.Application.Commands;
using TaskService.Application.Dtos;
using TaskService.Domain.Entities;

namespace TaskService.Application.Common.Mappers
{
    public static class TaskMapper
    {
        public static void MapToEntity(TaskItem entity, UpdateTaskCommand command)
        {
            entity.Update(command.Title, command.Description, Domain.ValueObjects.TaskStatus.From(command.Status));
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
            };
        }

        public static IEnumerable<TaskItemDto> ToDtosList(IEnumerable<TaskItem> entities)
        {
            return entities.Select(ToDto);
        }
    }
}
