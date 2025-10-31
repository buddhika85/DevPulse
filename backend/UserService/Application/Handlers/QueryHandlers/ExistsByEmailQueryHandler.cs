using MediatR;
using UserService.Application.Queries;
using UserService.Services;


namespace UserService.Application.Handlers.QueryHandlers
{
    public class ExistsByEmailQueryHandler : IRequestHandler<ExistsByEmailQuery, bool>
    {
        private readonly ILogger<ExistsByEmailQueryHandler> _logger;
        private readonly IUserService _service;

        public ExistsByEmailQueryHandler(ILogger<ExistsByEmailQueryHandler> logger, IUserService service)
        {
            _logger = logger;
            _service = service;
        }

        public async Task<bool> Handle(ExistsByEmailQuery query, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Handling ExistsByEmailQuery at {Time}", DateTime.UtcNow);
            return await _service.ExistsByEmailAsync(query, cancellationToken);
        }
    }
}
