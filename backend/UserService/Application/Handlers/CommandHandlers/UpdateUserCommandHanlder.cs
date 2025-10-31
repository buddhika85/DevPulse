using MediatR;
using UserService.Application.Commands;
using UserService.Services;

namespace UserService.Application.Handlers.CommandHandlers
{
    public class UpdateUserCommandHanlder : IRequestHandler<UpdateUserCommand, bool>
    {
        private readonly ILogger<UpdateUserCommandHanlder> _logger;
        private readonly IUserService _userService;

        public UpdateUserCommandHanlder(ILogger<UpdateUserCommandHanlder> logger, IUserService userService)
        {
            _logger = logger;
            _userService = userService;
        }

        public async Task<bool> Handle(UpdateUserCommand command, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Handling UpdateUserCommand at {Time}", DateTime.UtcNow);
            return await _userService.UpdateUserAsync(command, cancellationToken);
        }
    }
}
