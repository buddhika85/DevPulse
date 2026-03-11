namespace TaskJournalLinkService.Mapper
{
    public static class TaskJournalLinkMapper
    {

        public static IEnumerable<SharedLib.DTOs.TaskJournalLink.TaskJournalLinkDocument> ToDtos(IEnumerable<Domain.Models.TaskJournalLinkDocument> entities)
        {
            return entities.Select(ToDto);
        }

        private static SharedLib.DTOs.TaskJournalLink.TaskJournalLinkDocument ToDto(Domain.Models.TaskJournalLinkDocument entity)
        {
            return new SharedLib.DTOs.TaskJournalLink.TaskJournalLinkDocument { Id =  entity.Id, JounrnalId = Guid.Parse(entity.JournalId), TaskId = entity.TaskId, CreatedAt = entity.CreatedAt };
        }
    }
}
