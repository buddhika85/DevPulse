using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Data;
using TaskService.Infrastructure.Common.Extensions;
using UserService.Application.Common.Enums;
using UserService.Application.Common.Models;
using UserService.Application.Queries;
using UserService.Domain.Entities;
using SharedLib.Domain.ValueObjects;
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

        // Register new user
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
      
        // Check if email exist
        public async Task<bool> ExistsByEmailAsync(string email, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Checking if a user exists with email:{Email} at:{Time}", email, DateTime.UtcNow);
            try
            {
                var exists = await _dbContext.UserAccounts.AsNoTracking().AnyAsync(x => x.Email.Equals(email, StringComparison.OrdinalIgnoreCase), cancellationToken);
                return exists;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception occurred while checking if a user with email:{Email} exists at {Time}", email, DateTime.UtcNow);
                throw;
            }
        }

        //  List all users (admin-only)
        public async Task<IReadOnlyList<UserAccount>> GetAllAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Attempting to retrieve all users at {Time}", DateTime.UtcNow);
            try
            {
                var entities = await _dbContext.UserAccounts
                    .AsNoTracking()
                    .Where(x => !x.IsDeleted)
                    .OrderByDescending(x => x.CreatedAt)
                    .ToListAsync(cancellationToken);

                _logger.LogInformation("Retrieved {UserCount} users at {Time}", entities.Count, DateTime.UtcNow);
                return entities;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception occurred while retrieving all users at {Time}", DateTime.UtcNow);
                throw;
            }
        }


        public async Task<IReadOnlyList<UserAccount>> GetAllAsync(bool includeDeleted, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Attempting to retrieve all users with includeDeleted: {IncludeDeleted} at {Time}", includeDeleted, DateTime.UtcNow);
            try
            {
                var entities = await _dbContext.UserAccounts
                    .AsNoTracking()
                    .Where(x => x.IsDeleted == includeDeleted)
                    .OrderByDescending(x => x.CreatedAt)
                    .ToListAsync(cancellationToken);

                _logger.LogInformation("Retrieved {UserCount} users with includeDeleted: {IncludeDeleted} at {Time}", entities.Count, includeDeleted, DateTime.UtcNow);
                return entities;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception occurred while retrieving all users with includeDeleted: {IncludeDeleted} at {Time}", includeDeleted, DateTime.UtcNow);
                throw;
            }
        }

        // List all users by role
        public async Task<IReadOnlyList<UserAccount>> GetAllAsync(UserRole role, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Attempting to retrieve users in role {Role} at {Time}", role.Value, DateTime.UtcNow);
            try
            {
                var entities = await _dbContext.UserAccounts
                    .AsNoTracking()
                    .Include(x => x.Manager)
                    .Where(x => !x.IsDeleted && 
                                x.Role == role)
                    .OrderByDescending(x => x.CreatedAt)
                    .ToListAsync(cancellationToken);

                _logger.LogInformation("Retrieved {UserCount} {Role} users at {Time}", entities.Count, role.Value, DateTime.UtcNow);
                return entities;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception occurred while retrieving {Role} users at {Time}", role.Value, DateTime.UtcNow);
                throw;
            }
        }

        // Get user by ID (for profile or auth)
        public async Task<UserAccount?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Attempting to retrieve a user with Id:{Id} at {Time}", id, DateTime.UtcNow);
            try
            {
                var entity = await _dbContext.UserAccounts.Include(x => x.Manager).SingleOrDefaultAsync(x => x.Id == id, cancellationToken);
                if (entity is null)                
                    _logger.LogWarning("No user with Id: {Id} at {Time}", id, DateTime.UtcNow);
                else
                    _logger.LogInformation("Successfully retrieved a user with email '{Email}' with Id: {Id} at {Time}", entity.Email, id, DateTime.UtcNow);

                return entity;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception occurred while retrieving a user with Id:{Id} at {Time}", id, DateTime.UtcNow);
                throw;
            }
        }

        //  Update user profile
        public async Task<bool> UpdateAsync(Guid id, UserAccount updated, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Saving updated user with Id: {Id} at {Time}", updated.Id, DateTime.UtcNow);

                // if entity was received from AsNoTracking() query, we need to manualy re attach it
                if (_dbContext.Entry(updated).State == EntityState.Detached)
                {
                    _dbContext.UserAccounts.Update(updated);
                }

                var writeCount = await SaveChangesAsync(cancellationToken);
                if (writeCount > 0)
                {
                    _logger.LogInformation("Successfully persisted user with Id: {Id}, Email: {Email} at {Time}", updated.Id, updated.Email, DateTime.UtcNow);
                    return true;
                }

                _logger.LogWarning("No changes persisted for user with Id: {Id}, Email: {Email} at {Time}", updated.Id, updated.Email, DateTime.UtcNow);
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception occurred while updating User with Id: {Id}, Email: {Email} at {Time}", updated.Id, updated.Email, DateTime.UtcNow);
                throw;
            }
        }



        // Soft delete user
        public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Attempting to soft delete a user with Id:{Id} at {Time}", id, DateTime.UtcNow);
                var user = await _dbContext.UserAccounts.FindAsync([id], cancellationToken);
                if (user is null)
                {
                    _logger.LogError("No user with Id: {Id} at {Time}. Deletion aborted.", id, DateTime.UtcNow);
                    return false;
                }

                if (user.IsDeleted)
                {
                    _logger.LogWarning("User with Id: {Id} is already soft-deleted. No action taken at {Time}.", id, DateTime.UtcNow);
                    return false;
                }

                user.SoftDelete();
                var writeCount = await SaveChangesAsync(cancellationToken);
                if (writeCount > 0)
                {
                    _logger.LogInformation("Successfully soft deleted user with Id: {Id}, Email: {Email} at {Time}", id, user.Email, DateTime.UtcNow);
                    return true;
                }

                _logger.LogWarning("Soft deleting User with Id: {Id}, Email: {Email} at {Time} did not affect any records.", id, user.Email, DateTime.UtcNow);
                return false;

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception occurred while soft deleting User with Id: {Id} at {Time}", id, DateTime.UtcNow);
                throw;
            }
        }

        public async Task<bool> SoftDeleteUserAsync(UserAccount userToSoftDelete, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Attempting to soft delete a user with Id:{Id} at {Time}", userToSoftDelete.Id, DateTime.UtcNow);

                // if entity was received from AsNoTracking() query, we need to manualy re attach it
                if (_dbContext.Entry(userToSoftDelete).State == EntityState.Detached)
                {
                    _dbContext.UserAccounts.Update(userToSoftDelete);
                }

                var writeCount = await SaveChangesAsync(cancellationToken);
                if (writeCount > 0)
                {
                    _logger.LogInformation("Successfully soft deleted user with Id: {Id}, Email: {Email} at {Time}", userToSoftDelete.Id, userToSoftDelete.Email, DateTime.UtcNow);
                    return true;
                }

                _logger.LogWarning("Soft deleting User with Id: {Id}, Email: {Email} at {Time} did not affect any records.", userToSoftDelete.Id, userToSoftDelete.Email, DateTime.UtcNow);
                return false;

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception occurred while soft deleting User with Id: {Id} at {Time}", userToSoftDelete.Id, DateTime.UtcNow);
                throw;
            }
        }

        // Undo soft delete / restore user
        public async Task<bool> RestoreUserAsync(UserAccount userToRestore, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Attempting to restore a user with Id:{Id} at {Time}", userToRestore.Id, DateTime.UtcNow);
                // if entity was received from AsNoTracking() query, we need to manualy re attach it
                if (_dbContext.Entry(userToRestore).State == EntityState.Detached)
                {
                    _dbContext.UserAccounts.Update(userToRestore);
                }

                var writeCount = await SaveChangesAsync(cancellationToken);
                if (writeCount > 0)
                {
                    _logger.LogInformation("Successfully restored user with Id: {Id}, Email: {Email} at {Time}", userToRestore.Id, userToRestore.Email, DateTime.UtcNow);
                    return true;
                }

                _logger.LogWarning("Restoring User with Id: {Id}, Email: {Email} at {Time} did not affect any records.", userToRestore.Id, userToRestore.Email, DateTime.UtcNow);
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception occurred while restoring User with Id: {Id} at {Time}", userToRestore.Id, DateTime.UtcNow);
                throw;
            }
        }


        private async Task<int> SaveChangesAsync(CancellationToken cancellationToken)
        {
            try
            {
                var writeCount = await _dbContext.SaveChangesAsync(cancellationToken);

                // Dispatches all domain events raised by tracked entities
                // Happens after the database commit, ensuring events only fire if persistence succeeds
                await _mediator.DispatchDomainEventsAsync(_dbContext, _logger);
                return writeCount;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred during SaveChangesAsync or domain event dispatch.");
                throw;
            }
        }

       

        public async Task<PaginatedResult<UserAccount>> GetUserAccountsPaginatedAsync(GetUsersPaginatedQuery query, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation(
                    "Attempting to retrieve paginated Users with filters: Email={Email}, DisplayName={DisplayName}, Role={Role}, Page={PageNumber}, Size={PageSize}, SortBy={SortBy}, Desc={SortDescending} at {Time}",
                    query.Email, query.DisplayName, query.Role, query.PageNumber, query.PageSize, query.SortBy, query.SortDescending, DateTime.UtcNow);

                var queryable = _dbContext.UserAccounts.AsNoTracking().Include(x => x.Manager).AsQueryable()
                    .Where(x => !x.IsDeleted);

                if (!string.IsNullOrWhiteSpace(query.Email))
                    queryable = queryable.Where(x => EF.Functions.Contains(x.Email, query.Email));
                if (!string.IsNullOrWhiteSpace(query.DisplayName))
                    queryable = queryable.Where(x => EF.Functions.Contains(x.DisplayName, query.DisplayName));
                if (!string.IsNullOrWhiteSpace(query.Role))
                {
                    try
                    {
                        var role = UserRole.From(query.Role);
                        queryable = queryable.Where(x => x.Role == role);
                    }
                    catch (ArgumentException ex)
                    {
                        _logger.LogWarning(ex, "Invalid UserRole filter value: {Role}", query.Role);
                    }
                }

                if (query.SortBy is not null)
                {
                    queryable = query.SortBy switch
                    {
                        UserSortField.Email => query.SortDescending ? queryable.OrderByDescending(x => x.Email) : queryable.OrderBy(x => x.Email),
                        UserSortField.DisplayName => query.SortDescending ? queryable.OrderByDescending(x => x.DisplayName) : queryable.OrderBy(x => x.DisplayName),
                        _ => query.SortDescending ? queryable.OrderByDescending(x => x.CreatedAt) : queryable.OrderBy(x => x.CreatedAt),
                    };
                }
                else
                {
                    queryable = queryable.OrderBy(x => x.CreatedAt);
                }

                // this is to avoid sending 2 DB calls for counting and getting paged results
                var projectedResult = await (from x in queryable
                                      group x by 1 into fakeGroup                   // group x by 1 into fakeGroup - aggregate all matching records into a single group, enabling us to project both the total count and paginated items in one query
                                             select new
                                      {
                                          TotalCount = fakeGroup.Count(),
                                          PagedItems = fakeGroup.Skip((query.PageNumber - 1) * query.PageSize).Take(query.PageSize).ToList(),
                                      }).SingleAsync(cancellationToken);
                _logger.LogInformation("Successfully retrieved {Count} Users for page {PageNumber} at {Time}", projectedResult.PagedItems.Count, query.PageNumber, DateTime.UtcNow);

                return new PaginatedResult<UserAccount>
                {
                    PageNumber = query.PageNumber,
                    PageSize = query.PageSize,
                    TotalCount = projectedResult.TotalCount,
                    PageItems = projectedResult.PagedItems,
                };

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception occurred while retrieving paginated Users at {Time}", DateTime.UtcNow);
                throw;
            }
        }

        // if user exists with id and with in the given role
        public async Task<bool> IsUserExistsAsync(Guid userId, UserRole role, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Checking if user exists with Id={Id} on role={Role} at {Time}", userId, role, DateTime.UtcNow);
            try
            {
                return await _dbContext.UserAccounts
                    .AsNoTracking()
                    .AnyAsync(x => x.Id == userId && x.Role == role, 
                        cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception occurred while checking if user exists with Id={Id} on role={Role} at {Time}", userId, role, DateTime.UtcNow);
                throw;
            }
        }
    }
}
