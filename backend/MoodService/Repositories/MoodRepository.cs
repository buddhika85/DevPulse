using MediatR;
using Microsoft.EntityFrameworkCore;
using MoodService.Domain.Entities;
using MoodService.Domain.ValueObjects;
using MoodService.Infrastructure.Common.Extensions;
using MoodService.Infrastructure.Persistence;

namespace MoodService.Repositories
{
    public class MoodRepository : IMoodRepository
    {
        private readonly MoodDbContext _dbContext;
        private readonly ILogger<MoodRepository> _logger;
        private readonly IMediator _mediator;

        public MoodRepository(MoodDbContext dbContext, ILogger<MoodRepository> logger, IMediator mediator)
        {
            _dbContext = dbContext;
            _logger = logger;
            _mediator = mediator;
        }


        public async Task<MoodEntry?> AddAsync(MoodEntry entity, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Attempting to add new MoodEntry with Mood Level: {MoodLevel} at {Time}", entity.MoodLevel, DateTime.UtcNow);
            try
            {
                await _dbContext.MoodEntries.AddAsync(entity, cancellationToken);
                var result = await SaveChangesAsync(cancellationToken);

                if (result > 0)
                {
                    _logger.LogInformation("Successfully added MoodEntry with  Mood Level: {MoodLevel} at {Time}", entity.MoodLevel, DateTime.UtcNow);
                    return entity;
                }

                _logger.LogWarning("No records added for MoodEntry with Mood Level: {MoodLevel} at {Time}", entity.MoodLevel, DateTime.UtcNow);
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception occurred while adding MoodEntry with Mood Level: {MoodLevel} at {Time}", entity.MoodLevel, DateTime.UtcNow);
                throw;
            }
        }

        public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Attempting to delete MoodEntry with Id: {Id} at {Time}", id, DateTime.UtcNow);
            try
            {
                var entity = await _dbContext.MoodEntries.FindAsync([id], cancellationToken);
                if (entity is null)
                {
                    _logger.LogWarning("No MoodEntry found with Id: {Id} at {Time}. Nothing was deleted.", id, DateTime.UtcNow);
                    return false;
                }

                _dbContext.MoodEntries.Remove(entity);             // this permenently deletes from DB

                var result = await _dbContext.SaveChangesAsync(cancellationToken);

                if (result > 0)
                {
                    _logger.LogInformation("Successfully deleted MoodEntry Mood Level: {MoodLevel} and with Id: {Id} at {Time}", entity.MoodLevel, id, DateTime.UtcNow);
                    return true;
                }

                _logger.LogWarning("Delete operation for MoodEntry with Mood Level: {MoodLevel} and with Id: {Id} at {Time} did not affect any records.", entity.MoodLevel, id, DateTime.UtcNow);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception occurred while deleting MoodEntry with Id: {Id} at {Time}", id, DateTime.UtcNow);
                throw;
            }
        }

       

        public async Task<IReadOnlyList<MoodEntry>> GetAllAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Attempting to retrieve all MoodEntries at {Time}", DateTime.UtcNow);
            try
            {
                var entities = await _dbContext.MoodEntries
                    .AsNoTracking()                  
                    .OrderByDescending(t => t.CreatedAt)
                    .ToListAsync(cancellationToken);

                _logger.LogInformation("Successfully retrieved {Count} TaskItems at {Time}", entities.Count, DateTime.UtcNow);
                return entities;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception occurred while retrieving all MoodEntries at {Time}", DateTime.UtcNow);
                throw;
            }
        }

        public async Task<MoodEntry?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Attempting to retrieve a MoodEntry with Id: {Id} at {Time}", id, DateTime.UtcNow);
            try
            {
                var entity = await _dbContext.MoodEntries.FindAsync(new object[] { id }, cancellationToken);
                if (entity is null)
                {
                    _logger.LogWarning("No MoodEntry found with Id: {Id} at {Time}", id, DateTime.UtcNow);
                    return null;
                }

                _logger.LogInformation("Successfully retrieved a MoodEntry with Mood Level: {MoodLevel} and with Id: {Id} at {Time}", entity.MoodLevel, id, DateTime.UtcNow);
                return entity;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception occurred while retrieving MoodEntry  with Id: {Id} at {Time}", id, DateTime.UtcNow);
                throw;
            }
        }

        public async Task<IReadOnlyList<MoodEntry>> GetMoodEntriesByUserIdAsync(Guid userId, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Attempting to retrieve all MoodEntries for UserId={UserId} at {Time}", userId, DateTime.UtcNow);
            try
            {
                var entities = await _dbContext.MoodEntries
                    .AsNoTracking()
                    .Where(x => x.UserId == userId)
                    .OrderByDescending(t => t.CreatedAt)
                    .ToListAsync(cancellationToken);

                _logger.LogInformation("Successfully retrieved {Count} MoodEntries for UserId={UserId} at {Time}", entities.Count, userId, DateTime.UtcNow);
                return entities;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception occurred while retrieving all TaskItems for UserId={UserId} at {Time}", userId, DateTime.UtcNow);
                throw;
            }
        }


        // A user can have exactly 1 MoodEntry for given day and a given session
        public async Task<bool> IsMoodEntryExists(Guid userId, DateTime day, MoodTime session, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Checking if a mood-entry with for user: {UserId}, Day: {Day}, Mood Time: {MoodTime} at {Time}",
                   userId, day, session, DateTime.UtcNow);

                return await _dbContext.MoodEntries.AnyAsync(x => x.UserId == userId &&
                                            x.Day.Date == day.Date &&
                                            x.MoodTime == session, 
                                            cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception occurred while checking if a mood-entry with for user: {UserId}, Day: {Day}, Mood Time: {MoodTime} at {Time}",
                   userId, day, session, DateTime.UtcNow);
                throw;
            }
        }

        // Are there any other mood entreies with same Guid userId, DateTime day, MoodTime session except for excludeId ?
        // If so we cannot perform this update
        public async Task<MoodEntry?> FindOtherMoodEntry(Guid excludeId, Guid userId, DateTime day, MoodTime session, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Checking if a mood-entry with for user: {UserId}, Day: {Day}, Mood Time: {MoodTime} with exclusion of Mood-Entry: {ExcludeMoodId} at {Time}",
                   userId, day, session, excludeId, DateTime.UtcNow);

                return await _dbContext.MoodEntries.FirstOrDefaultAsync(x => 
                                            x.Id != excludeId &&
                                            x.UserId == userId &&
                                            x.Day.Date == day.Date &&
                                            x.MoodTime == session,
                                            cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception occurred while checking if a mood-entry with for user: {UserId}, Day: {Day}, Mood Time: {MoodTime} with exclusion of Mood-Entry: {ExcludeMoodId} at {Time}",
                   userId, day, session, excludeId, DateTime.UtcNow);
                throw;
            }
        }

        public async Task<bool> UpdateAsync(Guid id, MoodEntry updated, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Saving updated MoodEntry with Id: {Id} at {Time}", updated.Id, DateTime.UtcNow);

                // if entity was received from AsNoTracking() query, we need to manualy re attach it
                if (_dbContext.Entry(updated).State == EntityState.Detached)
                {
                    _dbContext.MoodEntries.Update(updated);
                }

                var writeCount = await SaveChangesAsync(cancellationToken);
                if (writeCount > 0)
                {
                    _logger.LogInformation("Successfully persisted MoodEntry with Id: {Id}, MoodLevel: {MoodLevel} at {Time}", updated.Id, updated.MoodLevel, DateTime.UtcNow);
                    return true;
                }

                _logger.LogWarning("No changes persisted for MoodEntry with Id: {Id}, MoodLevel: {MoodLevel} at {Time}", updated.Id, updated.MoodLevel, DateTime.UtcNow);
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception occurred while updating MoodEntry with Id: {Id}, MoodLevel: {MoodLevel} at {Time}", updated.Id, updated.MoodLevel, DateTime.UtcNow);
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
    }
}
