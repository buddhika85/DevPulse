using SharedLib.DTOs.Task;

namespace SharedLib.DTOs.Journal
{
    public record JournalEntryWithTasksAndFeedbackDto(Guid Id, Guid UserId, DateTime CreatedAt, string Title, string Content, bool IsDeleted, 
        JournalFeedbackDto? feedback, string? feedbackManager, IReadOnlyList<TaskItemDto> LinkedTasks)
    {
        public string IdSnippet => Id.ToString()[..6];
        public string IsDeletedStr => IsDeleted ? "Deleted" : "Active";
        public string CreatedAtStr => CreatedAt.ToShortDateString();
        public bool IsFeedbackGiven => feedback is not null;
        public string IsFeedbackGivenStr => IsFeedbackGiven ? "Yes" : "No";

        public string IsFeedbackSeenByUser => IsFeedbackGiven && feedback is not null ? feedback.SeenByUserStr : "-";

        public string[] LinkedTaskTitles => [.. LinkedTasks.Select(x => x.Title)];
        public string LinkedTaskTitlesCsv => LinkedTaskTitles != null && LinkedTaskTitles.Any() ? string.Join(", ", LinkedTaskTitles) : string.Empty;
        public string ContentSnippet => Content.Length <= 120 ? Content : Content.Substring(0, 120) + "...";        // If journals can be long, sending full content for every row is wasteful.
       
    }
}
