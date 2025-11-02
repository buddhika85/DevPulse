using MediatR;
using UserService.Application.Dtos;
using UserService.Application.Queries;
using UserService.Infrastructure.Identity;


namespace UserService.Application.Handlers.QueryHandlers
{
    public class GetUserByObjectIdQueryHandler : IRequestHandler<GetUserByObjectIdQuery, UserAccountDto?>
    {
        private readonly ILogger<GetUserByObjectIdQueryHandler> _logger;
        private readonly IExternalIdentityProvider _identityProvider;               // communicates with Micro Az Entra External ID


        public GetUserByObjectIdQueryHandler(ILogger<GetUserByObjectIdQueryHandler> logger, IExternalIdentityProvider identityProvider)
        {
            _logger = logger;
            _identityProvider = identityProvider;
        }

        public async Task<UserAccountDto?> Handle(GetUserByObjectIdQuery query, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Handling GetUserByIdQuery at {Time}", DateTime.UtcNow);
            return await _identityProvider.GetUserByObjectIdAsync(query.ObjectId, cancellationToken);
        }

    }

}
