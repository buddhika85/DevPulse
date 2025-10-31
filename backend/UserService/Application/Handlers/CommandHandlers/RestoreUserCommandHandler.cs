using MediatR;
using UserService.Application.Commands;
using UserService.Services;

namespace UserService.Application.Handlers.CommandHandlers
{
    public class RestoreUserCommandHandler : IRequestHandler<RestoreUserCommand, bool>
    {
        private readonly Logger<RestoreUserCommandHandler> _logger;
        private readonly IUserService _userService;

        public RestoreUserCommandHandler(Logger<RestoreUserCommandHandler> logger, IUserService userService)
        {
            _logger = logger;
            _userService = userService;
        }

        public async Task<bool> Handle(RestoreUserCommand command, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Handling RestoreUserCommand at {Time}", DateTime.UtcNow);
            return await _userService.RestoreUserAsync(command, cancellationToken);
        }
    }
}
