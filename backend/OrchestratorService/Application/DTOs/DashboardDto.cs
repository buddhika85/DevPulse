using SharedLib.DTOs.Task;
using SharedLib.DTOs.User;

namespace OrchestratorService.Application.DTOs
{
    public class DashboardDto
    {
        public UserAccountDto? User { get; set; }
        public IList<TaskItemDto> Tasks { get; set; } = [];
    }
}
