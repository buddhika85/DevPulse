using MediatR;
using SharedLib.DTOs.User;
using UserService.Application.Queries;
using UserService.Services;


namespace UserService.Application.Handlers.QueryHandlers
{
    public class GetUserByIdQueryHandler : IRequestHandler<GetUserByIdQuery, UserAccountDto?>
    {
        private readonly ILogger<GetUserByIdQueryHandler> _logger;
        private readonly IUserService _service;

        public GetUserByIdQueryHandler(ILogger<GetUserByIdQueryHandler> logger, IUserService service)
        {
            _logger = logger;
            _service = service;
        }

        public async Task<UserAccountDto?> Handle(GetUserByIdQuery query, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Handling GetUserByIdQuery at {Time}", DateTime.UtcNow);
            return await _service.GetUserByIdAsync(query, cancellationToken);
        }
    }
}
