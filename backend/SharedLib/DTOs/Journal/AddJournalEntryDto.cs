namespace SharedLib.DTOs.Journal
{
    public record AddJournalEntryDto(Guid UserId,
                                           string Title,
                                           string Content)
    {
    }
}
