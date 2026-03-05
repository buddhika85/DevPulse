using MediatR;
using UserService.Application.Queries;
using UserService.Services;


namespace UserService.Application.Handlers.QueryHandlers
{
    public class GetTeamMemberGuidsForManagerQueryHandler : IRequestHandler<GetTeamMemberGuidsForManagerQuery, IReadOnlyList<Guid>>
    {
        private readonly ILogger<GetTeamMemberGuidsForManagerQueryHandler> _logger;
        private readonly IUserService _service;

        public GetTeamMemberGuidsForManagerQueryHandler(ILogger<GetTeamMemberGuidsForManagerQueryHandler> logger, IUserService service)
        {
            _logger = logger;
            _service = service;
        }

        public async Task<IReadOnlyList<Guid>> Handle(GetTeamMemberGuidsForManagerQuery query, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Handling GetTeamMemberGuidsForManagerQuery at {Time}", DateTime.UtcNow);
            return await _service.GetTeamMemberGuidsForManagerAsync(query, cancellationToken);
        }
    }
}
