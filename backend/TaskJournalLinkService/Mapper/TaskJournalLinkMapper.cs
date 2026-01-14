using SharedLib.DTOs.TaskJournalLink;
using TaskJournalLinkService.Domain.Models;

namespace TaskJournalLinkService.Mapper
{
    public static class TaskJournalLinkMapper
    {

        public static IEnumerable<TaskJournalLinkDto> ToDtos(IEnumerable<TaskJournalLinkDocument> entities)
        {
            return entities.Select(ToDto);
        }

        private static TaskJournalLinkDto ToDto(TaskJournalLinkDocument entity)
        {
            return new TaskJournalLinkDto { Id =  entity.Id, JounrnalId = Guid.Parse(entity.JournalId), TaskId = entity.TaskId, CreatedAt = entity.CreatedAt };
        }
    }
}
