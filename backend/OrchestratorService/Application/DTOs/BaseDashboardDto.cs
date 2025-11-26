using SharedLib.DTOs.User;

namespace OrchestratorService.Application.DTOs
{
    public record BaseDashboardDto(UserAccountDto? User, string dashBoardType);


    public record AdminDashboardDto(UserAccountDto? User) : BaseDashboardDto(User, "Admin Dashboard");
    public record ManagerDashboardDto(UserAccountDto? User) : BaseDashboardDto(User, "Manager Dashboard");

    public record UserDashboardDto(UserAccountDto? User) : BaseDashboardDto(User, "Developer Dashboard");
}
