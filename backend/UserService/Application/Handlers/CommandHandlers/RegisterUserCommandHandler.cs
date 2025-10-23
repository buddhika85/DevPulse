using MediatR;
using UserService.Application.Commands;
using UserService.Services;

namespace UserService.Application.Handlers.CommandHandlers
{
    public class RegisterUserCommandHandler : IRequestHandler<RegisterUserCommand, Guid?>
    {
        private readonly ILogger<RegisterUserCommandHandler> _logger;
        private readonly IUserService _userService;

        public RegisterUserCommandHandler(ILogger<RegisterUserCommandHandler> logger, IUserService userService)
        {
            _logger = logger;
            _userService = userService;
        }

        public async Task<Guid?> Handle(RegisterUserCommand command, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Handling RegisterUserCommand at {Time}", DateTime.UtcNow);
            return await _userService.RegisterUserAsync(command, cancellationToken);
        }
    }
}
