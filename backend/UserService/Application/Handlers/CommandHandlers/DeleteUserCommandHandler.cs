using MediatR;
using UserService.Application.Commands;
using UserService.Services;

namespace UserService.Application.Handlers.CommandHandlers
{
    public class DeleteUserCommandHandler : IRequestHandler<DeleteUserCommand, bool>
    {
        private readonly Logger<DeleteUserCommand> _logger;
        private readonly IUserService _userService;

        public DeleteUserCommandHandler(Logger<DeleteUserCommand> logger, IUserService userService)
        {
            _logger = logger;
            _userService = userService;
        }

        public async Task<bool> Handle(DeleteUserCommand command, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Handling DeleteUserCommand at {Time}", DateTime.UtcNow);
            return await _userService.DeleteUserAsync(command, cancellationToken);
        }
    }
}
