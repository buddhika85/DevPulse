namespace SharedLib.DTOs.Journal
{
    public record JournalFeedbackDto(Guid Id, Guid JournalEntryId, Guid FeedbackManagerId, DateTime CreatedAt, string Comment, bool SeenByUser)
    {
        public string CreatedAtStr => CreatedAt.ToShortDateString();
        public string SeenByUserStr => SeenByUser ? "Yes" : "No";
    }
}
