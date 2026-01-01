using SharedLib.DTOs.Task;

namespace SharedLib.DTOs.Journal
{
    public record JournalWithTasksDto(JournalEntryDto Journal, TaskItemDto[] LinkedTasks)
    {       
    }
}
