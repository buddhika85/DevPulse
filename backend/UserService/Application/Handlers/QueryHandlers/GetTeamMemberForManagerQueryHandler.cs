using MediatR;
using SharedLib.DTOs.User;
using UserService.Application.Queries;
using UserService.Services;


namespace UserService.Application.Handlers.QueryHandlers
{
    public class GetTeamMemberForManagerQueryHandler : IRequestHandler<GetTeamMemberForManagerQuery, IReadOnlyList<UserAccountDto>>
    {
        private readonly ILogger<GetTeamMemberForManagerQueryHandler> _logger;
        private readonly IUserService _service;

        public GetTeamMemberForManagerQueryHandler(ILogger<GetTeamMemberForManagerQueryHandler> logger, IUserService service)
        {
            _logger = logger;
            _service = service;
        }

        public async Task<IReadOnlyList<UserAccountDto>> Handle(GetTeamMemberForManagerQuery query, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Handling GetUserByIdQuery at {Time}", DateTime.UtcNow);
            return await _service.GetTeamMembersForManagerAsync(query, cancellationToken);
        }
    }
}
