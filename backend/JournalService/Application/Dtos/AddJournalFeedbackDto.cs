namespace JournalService.Application.Dtos
{
    public record AddJournalFeedbackDto(Guid JounralEntryId,
                                            Guid FeedbackManagerId,
                                            string Comment)
    {
    }
}
