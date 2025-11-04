using MediatR;
using UserService.Application.Dtos;
using UserService.Application.Queries;
using UserService.Infrastructure.Identity;
using UserService.Services;


namespace UserService.Application.Handlers.QueryHandlers
{
    public class GetUserByObjectIdQueryHandler : IRequestHandler<GetUserByObjectIdQuery, UserAccountDto?>
    {
        private readonly ILogger<GetUserByObjectIdQueryHandler> _logger;
        private readonly IUserService _userService;

        public GetUserByObjectIdQueryHandler(ILogger<GetUserByObjectIdQueryHandler> logger, IUserService userService)
        {
            _logger = logger;
            _userService = userService;
        }

        public async Task<UserAccountDto?> Handle(GetUserByObjectIdQuery query, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Handling GetUserByIdQuery at {Time}", DateTime.UtcNow);
            return await _userService.ResolveOrCreateAsync(query.ObjectId, cancellationToken);
        }
    }
}
