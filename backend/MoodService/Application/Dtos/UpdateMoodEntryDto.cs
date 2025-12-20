namespace MoodService.Application.Dtos
{
    public record UpdateMoodEntryDto(Guid Id, DateTime Day, string MoodTime, string MoodLevel, string? Note)
    {
    }
}
