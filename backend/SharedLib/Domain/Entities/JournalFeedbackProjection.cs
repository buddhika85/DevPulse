namespace SharedLib.Domain.Entities
{
    public class JournalFeedbackProjection
    {
        public Guid Id { get; set; }                                        // Unique journal ID
        public string Comment { get; private set; } = string.Empty;         // journal Comment       
        public DateTime CreatedAt { get; set; }
        public bool SeenByUser { get; set; }
        public Guid ManagerId { get; set; }                                 // Manager who gave feedback reference
        public string ManagerDisplayName { get; set; } = string.Empty;         // Manager name
    }
}
