using SharedLib.DTOs.Task;
using SharedLib.DTOs.User;

namespace OrchestratorService.Application.DTOs
{
    //public record DashboardDto
    //{
    //    public UserAccountDto? User { get; set; }
    //    public IList<TaskItemDto> Tasks { get; set; } = [];
    //}

    public record DashboardDto(UserAccountDto? User, IList<TaskItemDto> Tasks);
}
