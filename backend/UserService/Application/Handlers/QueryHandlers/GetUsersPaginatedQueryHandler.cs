using MediatR;
using UserService.Application.Common.Models;
using UserService.Application.Dtos;
using UserService.Application.Queries;
using UserService.Services;


namespace UserService.Application.Handlers.QueryHandlers
{
    public class GetUsersPaginatedQueryHandler : IRequestHandler<GetUsersPaginatedQuery, PaginatedResult<UserAccountDto>>
    {
        private readonly ILogger<GetUsersPaginatedQueryHandler> _logger;
        private readonly IUserService _service;
        public GetUsersPaginatedQueryHandler(ILogger<GetUsersPaginatedQueryHandler> logger, IUserService service)
        {
            _logger = logger;
            _service = service;
        }

        public async Task<PaginatedResult<UserAccountDto>> Handle(GetUsersPaginatedQuery query, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Handling GetUsersPaginatedQuery at {Time}", DateTime.UtcNow);
            return await _service.GetUserAccountsPaginatedAsync(query, cancellationToken);
        }
    }
}
