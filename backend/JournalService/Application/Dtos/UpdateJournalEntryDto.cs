namespace JournalService.Application.Dtos
{
    public record UpdateJournalEntryDto(Guid JournalEntryId,
                                           string Title,
                                           string Content)
    {
    }
}
