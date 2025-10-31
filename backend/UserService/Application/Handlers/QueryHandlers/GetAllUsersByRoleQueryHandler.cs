using MediatR;
using UserService.Application.Dtos;
using UserService.Application.Queries;
using UserService.Services;


namespace UserService.Application.Handlers.QueryHandlers
{
    public class GetAllUsersByRoleQueryHandler : IRequestHandler<GetAllUsersByRoleQuery, IReadOnlyList<UserAccountDto>>
    {
        private readonly ILogger<GetAllUsersByRoleQueryHandler> _logger;
        private readonly IUserService _service;

        public GetAllUsersByRoleQueryHandler(ILogger<GetAllUsersByRoleQueryHandler> logger, IUserService service)
        {
            _logger = logger;
            _service = service;
        }

        public async Task<IReadOnlyList<UserAccountDto>> Handle(GetAllUsersByRoleQuery query, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Handling GetAllUsersByRoleQuery at {Time}", DateTime.UtcNow);
            return await _service.GetAllUsersByRoleAsync(query, cancellationToken);
        }
    }
}
