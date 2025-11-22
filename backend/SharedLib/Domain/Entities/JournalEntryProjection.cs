namespace SharedLib.Domain.Entities
{
    public class JournalEntryProjection
    {
        public Guid Id { get; set; }                                    // Unique journal ID
        public string Title { get; private set; } = string.Empty;       // journal title
        public string Content { get; private set; } = string.Empty;     // journal description
        public DateTime CreatedAt { get; set; }
        public bool CheckedByManager { get; set; }
        public Guid UserId { get; set; }                                // Owner reference

        public JournalFeedbackProjection? JournalFeedbackProjection { get; set; }           // if the feedback is already given
    }
}
