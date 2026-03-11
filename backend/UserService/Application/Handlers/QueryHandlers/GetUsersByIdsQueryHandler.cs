using MediatR;
using SharedLib.DTOs.User;
using UserService.Application.Queries;
using UserService.Services;


namespace UserService.Application.Handlers.QueryHandlers
{
    // GetUsersByIdsQuery
    public class GetUsersByIdsQueryHandler : IRequestHandler<GetUsersByIdsQuery, IReadOnlyList<UserAccountDto>>
    {
        private readonly ILogger<GetUsersByIdsQueryHandler> _logger;
        private readonly IUserService _service;

        public GetUsersByIdsQueryHandler(ILogger<GetUsersByIdsQueryHandler> logger, IUserService service)
        {
            _logger = logger;
            _service = service;
        }

        public async Task<IReadOnlyList<UserAccountDto>> Handle(GetUsersByIdsQuery query, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Handling GetUsersByIdsQuery at {Time}", DateTime.UtcNow);
            return await _service.GetUsersByIdsAsync(query, cancellationToken);
        }
    }
}
