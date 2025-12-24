namespace SharedLib.DTOs.Journal
{
    public record JournalEntryDto(Guid Id, Guid UserId, DateTime CreatedAt, string Title, string Content, bool IsDeleted, Guid? JournalFeedbackId)
    {
        public string CreatedAtStr => CreatedAt.ToShortDateString();
    }
}
