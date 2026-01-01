using SharedLib.DTOs.User;


namespace OrchestratorService.Infrastructure.HttpClients.UserMicroService
{
    public interface IUserServiceClient
    {
        Task<UserAccountDto> GetUserAsync(string userId, CancellationToken cancellationToken);
    }
}
