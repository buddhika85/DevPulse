namespace SharedLib.DTOs.Task
{
    public record TaskItemWithUserDto : TaskItemDto
    {
        public string UserDisplayName { get; init; } = string.Empty;
    }
}
