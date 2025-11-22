namespace SharedLib.Domain.Entities
{
    public class MoodEntryProjection
    {
        public Guid UserId { get; set; }                                    // Unique journal ID - composite PK
        public DateTime Day { get; set; }                                   // composite PK
        public string? MoodLevel { get; set; }                              // Happy, Stressed...etc
        public string? Note { get; set; }
        public UserAccountProjection? User { get; set; }                    // User who did the mood entry
    }
}
