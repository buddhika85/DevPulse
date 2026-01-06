namespace SharedLib.DTOs.TaskJournalLink
{
    public record TaskJournalLinkDto
    {
        public required Guid Id { get; init; }
        public required Guid JounrnalId { get; init; }
        public required Guid TaskId { get; init; }
        public required DateTime CreatedAt { get; init; }
        public string CreatedAtStr => CreatedAt.ToShortDateString();
    }
}
