using MediatR;
using TaskService.Infrastructure.Common.Extensions;
using UserService.Domain.Entities;
using UserService.Infrastructure.Persistence;

namespace UserService.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly UserDbContext _dbContext;
        private readonly ILogger<UserRepository> _logger;
        private readonly IMediator _mediator;

        public UserRepository(UserDbContext dbContext, ILogger<UserRepository> logger, IMediator mediator)
        {
            _dbContext = dbContext;
            _logger = logger;
            _mediator = mediator;
        }

        public async Task<UserAccount?> AddAsync(UserAccount entity, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Attempting to add new Task with email: {Email}, display name: {DisplayName} at {Time}", entity.Email, entity.DisplayName, DateTime.UtcNow);
            try
            {
                await _dbContext.UserAccounts.AddAsync(entity);
                var result = await SaveChangesAsync(cancellationToken);

                if (result > 0)
                {
                    _logger.LogInformation("Successfully added UserAccount with email: {Email}, display name: {DisplayName} at {Time}", entity.Email, entity.DisplayName, DateTime.UtcNow);
                    return entity;
                }

                _logger.LogWarning("No records added for UserAccounts with email: {Email}, display name: {DisplayName} at {Time}", entity.Email, entity.DisplayName, DateTime.UtcNow);
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception occurred while adding UserAccount with email: {Email}, display name: {DisplayName} at {Time}", entity.Email, entity.DisplayName, DateTime.UtcNow);
                throw;
            }
        }

        private async Task<int> SaveChangesAsync(CancellationToken cancellationToken)
        {
            var writeCount = await _dbContext.SaveChangesAsync(cancellationToken);

            // Dispatches all domain events raised by tracked entities
            // Happens after the database commit, ensuring events only fire if persistence succeeds
            await _mediator.DispatchDomainEventsAsync(_dbContext);
            return writeCount;
        }

        public Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<bool> ExistsByEmailAsync(string email, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<IReadOnlyList<UserAccount>> GetAllAsync(CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<UserAccount?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<bool> UpdateAsync(Guid id, UserAccount entity, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
