namespace JournalService.Application.Dtos
{
    public record AddJournalEntryDto(Guid UserId,
                                           string Title,
                                           string Content)
    {
    }
}
