using UserService.Application.Commands;
using UserService.Application.Dtos;
using UserService.Domain.Entities;
using UserService.Repositories;

namespace UserService.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly ILogger<UserService> _logger;

        public UserService(IUserRepository userRepository, ILogger<UserService> logger)
        {
            _userRepository = userRepository;
            _logger = logger;
        }

        public Task<IReadOnlyList<UserAccountDto>> GetAllUsersAsync(CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public async Task<Guid?> RegisterUserAsync(RegisterUserCommand command, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Registering a new user with email: {Email}, display name: {DisplayName} at {Time}", command.Email, command.DisplayName, DateTime.UtcNow);

                // create domain object and raise event
                var userAccount = UserAccount.Create(command.Email, command.DisplayName, command.Role);

                var result = await _userRepository.AddAsync(userAccount, cancellationToken);

                if (result is not null)
                {
                    _logger.LogInformation("User account created successfully email: {Email}, display name: {DisplayName} at {Time}", command.Email, command.DisplayName, DateTime.UtcNow);
                    return result.Id;
                }

                _logger.LogWarning("user account creation failed for email: {Email}, display name: {DisplayName} at {Time}", command.Email, command.DisplayName, DateTime.UtcNow);
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while creating a user account with email: {Email}, display name: {DisplayName} at {Time}", command.Email, command.DisplayName, DateTime.UtcNow);
                return null;
            }
        }
    }
}
