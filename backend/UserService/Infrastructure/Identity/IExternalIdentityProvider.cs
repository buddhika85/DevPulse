using UserService.Application.Dtos;

namespace UserService.Infrastructure.Identity
{
    public interface IExternalIdentityProvider
    {
        Task<UserAccountDto?> GetUserByObjectIdAsync(string objectId, CancellationToken cancellationToken);
    }
}
