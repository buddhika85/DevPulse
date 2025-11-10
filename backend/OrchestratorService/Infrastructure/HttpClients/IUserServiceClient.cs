using SharedLib.DTOs.User;


namespace OrchestratorService.Infrastructure.HttpClients
{
    public interface IUserServiceClient
    {
        Task<UserAccountDto> GetUserAsync(string userId, CancellationToken cancellationToken);
    }
}
