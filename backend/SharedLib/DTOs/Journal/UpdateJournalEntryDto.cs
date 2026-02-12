namespace SharedLib.DTOs.Journal
{
    public record UpdateJournalEntryDto(Guid JournalEntryId,
                                           string Title,
                                           string Content)
    {
    }
}
