using SharedLib.DTOs.User;

namespace OrchestratorService.Infrastructure.HttpClients.UserMicroService
{
    public interface IUserServiceClient
    {
        Task<IReadOnlyList<Guid>> GetTeamMembersIdsForManager(Guid managerId, CancellationToken cancellationToken);
        Task<IReadOnlyList<UserAccountDto>> GetTeamMembersForManager(Guid managerId, CancellationToken cancellationToken);
        Task<UserAccountDto> GetUserAsync(string userId, CancellationToken cancellationToken);
    }
}
