namespace MoodService.Application.Dtos
{
    public record AddMoodEntryDto(Guid UserId,
                                        DateTime? Day,
                                        string? MoodTime,
                                        string? MoodLevel,
                                        string? Note)
    {
    }
}
