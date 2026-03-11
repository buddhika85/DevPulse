namespace SharedLib.DTOs.Journal
{
    public record JournalEntryWithFeedbackDto(JournalEntryDto journal, JournalFeedbackDto? feedback);
}
