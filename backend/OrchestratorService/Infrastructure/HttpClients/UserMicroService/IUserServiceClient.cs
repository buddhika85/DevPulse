using SharedLib.DTOs.User;

namespace OrchestratorService.Infrastructure.HttpClients.UserMicroService
{
    public interface IUserServiceClient
    {
        Task<IReadOnlyList<Guid>> GetTeamMembersForManager(Guid managerId, CancellationToken cancellationToken);
        Task<UserAccountDto> GetUserAsync(string userId, CancellationToken cancellationToken);
    }
}
