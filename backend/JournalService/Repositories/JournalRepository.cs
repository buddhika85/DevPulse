using JournalService.Domain.Entities;
using JournalService.Infrastructure.Common.Extensions;
using JournalService.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace JournalService.Repositories
{
    public class JournalRepository : IJournalRepository
    {
        private readonly JournalDbContext _dbContext;
        private readonly ILogger<JournalRepository> _logger;
        private readonly IMediator _mediator;

        public JournalRepository(JournalDbContext dbContext, ILogger<JournalRepository> logger, IMediator mediator)
        {
            _dbContext = dbContext;
            _logger = logger;
            _mediator = mediator;
        }

        public async Task<IReadOnlyList<JournalEntry>> GetAllAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Attempting to retrieve all JournalEntries at {Time}", DateTime.UtcNow);
            try
            {
                var entities = await _dbContext.JournalEntries
                    .AsNoTracking()
                    .OrderByDescending(t => t.CreatedAt)
                    .ToListAsync(cancellationToken);

                _logger.LogInformation("Successfully retrieved {Count} JournalEntries at {Time}", entities.Count, DateTime.UtcNow);
                return entities;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception occurred while retrieving all MoodEntries at {Time}", DateTime.UtcNow);
                throw;
            }
        }

        public async Task<JournalEntry?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Attempting to retrieve a JournalEntry with Id: {Id} at {Time}", id, DateTime.UtcNow);
            try
            {
                var entity = await _dbContext.JournalEntries.FindAsync(new object[] { id }, cancellationToken);
                if (entity is null)
                {
                    _logger.LogWarning("No JournalEntry found with Id: {Id} at {Time}", id, DateTime.UtcNow);
                    return null;
                }

                _logger.LogInformation("Successfully retrieved a JournalEntry with Id: {Id} at {Time}", entity.Id, DateTime.UtcNow);
                return entity;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception occurred while retrieving JournalEntry  with Id: {Id} at {Time}", id, DateTime.UtcNow);
                throw;
            }
        }

        public async Task<bool> IsJournalEntryExistsByIdAsync(Guid id, CancellationToken cancellationToken, bool includeDeleted = false)
        {
            var now = DateTime.UtcNow;
            _logger.LogInformation("Checking if a JournalEntry exists with Id: {Id} includeDeleted:{includeDeleted} at {Time}", id, includeDeleted, now);
            try
            {
                var isExists = includeDeleted ? 
                    await _dbContext.JournalEntries.AnyAsync(x => x.Id == id, cancellationToken) :
                    await _dbContext.JournalEntries.AnyAsync(x => x.Id == id && !x.IsDeleted, cancellationToken);
                
                if (isExists)                
                    _logger.LogInformation("A JournalEntry found with Id: {Id} includeDeleted:{includeDeleted} at {Time}", id, includeDeleted, now);
                else
                    _logger.LogInformation("No JournalEntry found with Id: {Id} includeDeleted:{includeDeleted} at {Time}", id, includeDeleted, now);
                return isExists;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception occurred while checking if a JournalEntry exists Id: {Id} includeDeleted:{includeDeleted} at {Time}", id, includeDeleted, now);
                throw;
            }
        }

        public async Task<IReadOnlyList<JournalEntry>> GetJournalEntriesByUserIdAsync(Guid userId, CancellationToken cancellationToken, bool includeDeleted = false, bool includeFeedbacks = false)
        {
            _logger.LogInformation("Attempting to retrieve all JournalEntries for UserId={UserId}, with includeDeleted={includeDeleted}, with includeFeedbacks={includeFeedbacks} at {Time}", 
                userId, includeDeleted, includeFeedbacks, DateTime.UtcNow);
            try
            {
                var query =  _dbContext.JournalEntries
                    .AsNoTracking()
                    .Where(x => x.UserId == userId);

                if (!includeDeleted)
                    query = query.Where(x => !x.IsDeleted);

                if (includeFeedbacks)
                    query = query.Include(x => x.JournalFeedback);

                var entities = await query.OrderByDescending(t => t.CreatedAt)
                    .ToListAsync(cancellationToken);

                _logger.LogInformation("Successfully retrieved {Count} JournalEntries for UserId={UserId}, with includeDeleted={includeDeleted}, with includeFeedbacks={includeFeedbacks} at {Time}", 
                    entities.Count, userId, includeDeleted, includeFeedbacks, DateTime.UtcNow);
                return entities;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception occurred while retrieving all JournalEntries for UserId={UserId}, with includeDeleted={includeDeleted}, with includeFeedbacks={includeFeedbacks} at {Time}", 
                    userId, includeDeleted, includeFeedbacks, DateTime.UtcNow);
                throw;
            }
        }
        
        public async Task<JournalEntry?> AddAsync(JournalEntry entity, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Attempting to add new JournalEntry with Journal Title: {JournalTitle} at {Time}", entity.Title, DateTime.UtcNow);
            try
            {
                await _dbContext.JournalEntries.AddAsync(entity, cancellationToken);
                var result = await SaveChangesAsync(cancellationToken);

                if (result > 0)
                {
                    _logger.LogInformation("Successfully added JournalEntry with Journal Title: {JournalTitle} at {Time}", entity.Title, DateTime.UtcNow);
                    return entity;
                }

                _logger.LogWarning("No JournalEntry added for new JournalEntry with Journal Title: {JournalTitle}  at {Time}", entity.Title, DateTime.UtcNow);
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception occurred while adding JournalEntry with Journal Title: {JournalTitle} at {Time}", entity.Title, DateTime.UtcNow);
                throw;
            }
        }

        public async Task<bool> UpdateAsync(Guid id, JournalEntry updated, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Saving updated JournalEntry with Id: {Id} at {Time}", updated.Id, DateTime.UtcNow);

                // if entity was received from AsNoTracking() query, we need to manualy re attach it
                if (_dbContext.Entry(updated).State == EntityState.Detached)
                {
                    _dbContext.JournalEntries.Update(updated);
                }

                var writeCount = await SaveChangesAsync(cancellationToken);
                if (writeCount > 0)
                {
                    _logger.LogInformation("Successfully persisted JournalEntry with Id: {Id}, Title: {JournalTitle} at {Time}", updated.Id, updated.Title, DateTime.UtcNow);
                    return true;
                }

                _logger.LogWarning("No changes persisted for JournalEntry with Id: {Id}, Title: {JournalTitle} at {Time}", updated.Id, updated.Title, DateTime.UtcNow);
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception occurred while updating JournalEntry with Id: {Id}, Title: {JournalTitle} at {Time}", updated.Id, updated.Title, DateTime.UtcNow);
                throw;
            }
        }

        // soft-delete
        public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Attempting to delete JournalEntry with Id: {Id} at {Time}", id, DateTime.UtcNow);
            try
            {
                var entity = await _dbContext.JournalEntries.FindAsync(new object[] { id }, cancellationToken);
                if (entity is null)
                {
                    _logger.LogWarning("No JournalEntry found with Id: {Id} at {Time}. Nothing was deleted.", id, DateTime.UtcNow);
                    return false;
                }

                // _dbContext.Tasks.Remove(entity);             // this permenently deletes from DB
                // entity.RaiseDeletedEvent();

                entity.SoftDelete();

                var result = await _dbContext.SaveChangesAsync(cancellationToken);

                if (result > 0)
                {
                    _logger.LogInformation("Successfully deleted JournalEntry titled '{Title}' with Id: {Id} at {Time}", entity.Title, id, DateTime.UtcNow);
                    return true;
                }

                _logger.LogWarning("Delete operation for JournalEntry titled '{Title}' with Id: {Id} at {Time} did not affect any records.", entity.Title, id, DateTime.UtcNow);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception occurred while deleting JournalEntry with Id: {Id} at {Time}", id, DateTime.UtcNow);
                throw;
            }
        }

        // restore when soft-deleted
        public async Task<bool> RestoreAsync(Guid id, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Attempting to restore JournalEntry with Id: {Id} at {Time}", id, DateTime.UtcNow);
            try
            {
                var entity = await _dbContext.JournalEntries.FindAsync(new object[] { id }, cancellationToken);
                if (entity is null)
                {
                    _logger.LogWarning("No JournalEntry found with Id: {Id} at {Time}. Nothing was deleted.", id, DateTime.UtcNow);
                    return false;
                }


                entity.Restore();

                var result = await _dbContext.SaveChangesAsync(cancellationToken);

                if (result > 0)
                {
                    _logger.LogInformation("Successfully restored JournalEntry titled '{Title}' with Id: {Id} at {Time}", entity.Title, id, DateTime.UtcNow);
                    return true;
                }

                _logger.LogWarning("Restore operation for JournalEntry titled '{Title}' with Id: {Id} at {Time} did not affect any records.", entity.Title, id, DateTime.UtcNow);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception occurred while restoring JournalEntry with Id: {Id} at {Time}", id, DateTime.UtcNow);
                throw;
            }
        }

        // assumes that service layer already done these
        // journal with journalFeedback.JournalEntryId aleady exists --> should be Yes
        // journalFeedback.JournalEntryId does not have an attached jounnal feedback --> should be Yes
        // already service called AttachJournalFeedback(JournalFeedback journalFeedback) --> should be successful
        // if not this method is expected to crash --> service consumer agreement, consumer should call this method in the right context with rigt data

        // Task<bool> AttachJournalFeedback(JournalFeedback journalFeedback, CancellationToken cancellationToken);  // we dont need this as its a workflow method, this exists in journal service



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
