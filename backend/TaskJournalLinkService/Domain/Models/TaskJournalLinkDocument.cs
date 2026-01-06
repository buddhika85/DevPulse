namespace TaskJournalLinkService.Domain.Models
{
    // cosmos document model
    public record TaskJournalLinkDocument(Guid Id, Guid TaskId, Guid JournalId, DateTime CreatedAt)
    {
    }
}
