using MediatR;
using SharedLib.DTOs.User;
using UserService.Application.Queries;
using UserService.Services;


namespace UserService.Application.Handlers.QueryHandlers
{
    public class GetAllUsersHandler : IRequestHandler<GetAllUsersQuery, IReadOnlyList<UserAccountDto>>
    {
        private readonly ILogger<GetAllUsersHandler> _logger;
        private readonly IUserService _service;

        public GetAllUsersHandler(ILogger<GetAllUsersHandler> logger, IUserService service)
        {
            _logger = logger;
            _service = service;
        }

        public Task<IReadOnlyList<UserAccountDto>> Handle(GetAllUsersQuery query, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Handling GetAllTasksQuery at {Time}", DateTime.UtcNow);
            return _service.GetAllUsersAsync(query, cancellationToken);
        }
    }
}
