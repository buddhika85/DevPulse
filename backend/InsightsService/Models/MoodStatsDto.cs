namespace InsightsService.Models;

public record MoodStatsDto(
    Guid UserId,
    int DaysBack,
    int MoodEntriesCount,
    double TotalMoodScore,
    double AvgMoodScore,
    string AvgMoodString
);
