namespace SharedLib.DTOs.Mood
{
    public record MoodEntryDto(Guid Id, DateTime Day, string MoodTime, string MoodTimeRange, string MoodLevel, int MoodScore, string Note, DateTime CreatedAt, Guid UserId)
    {
        public string DayStr => Day.ToString("yyyy-MM-dd");
        public string CreatedAtStr => CreatedAt.ToShortDateString();
    }
}
