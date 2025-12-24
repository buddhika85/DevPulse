using JournalService.Domain.Entities;
using JournalService.Infrastructure.Common.Extensions;
using JournalService.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace JournalService.Repositories
{
    public class JournalFeedbackRepository : IJournalFeedbackRepository
    {
        private readonly JournalDbContext _dbContext;
        private readonly ILogger<JournalRepository> _logger;
        private readonly IMediator _mediator;

        public JournalFeedbackRepository(JournalDbContext dbContext, ILogger<JournalRepository> logger, IMediator mediator)
        {
            _dbContext = dbContext;
            _logger = logger;
            _mediator = mediator;
        }

        public async Task<IReadOnlyList<JournalFeedback>> GetAllAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Attempting to retrieve all JournalFeedbacks at {Time}", DateTime.UtcNow);
            try
            {
                var entities = await _dbContext.JournalFeedbacks
                    .AsNoTracking()
                    .OrderByDescending(t => t.CreatedAt)
                    .ToListAsync(cancellationToken);

                _logger.LogInformation("Successfully retrieved {Count} JournalFeedbacks at {Time}", entities.Count, DateTime.UtcNow);
                return entities;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception occurred while retrieving all JournalFeedbacks at {Time}", DateTime.UtcNow);
                throw;
            }
        }

        public async Task<JournalFeedback?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Attempting to retrieve a JournalFeedback with Id: {Id} at {Time}", id, DateTime.UtcNow);
            try
            {
                var entity = await _dbContext.JournalFeedbacks.FindAsync(new object[] { id }, cancellationToken);
                if (entity is null)
                {
                    _logger.LogWarning("No JournalFeedback found with Id: {Id} at {Time}", id, DateTime.UtcNow);
                    return null;
                }

                _logger.LogInformation("Successfully retrieved a JournalFeedback with Id: {Id} at {Time}", entity.Id, DateTime.UtcNow);
                return entity;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception occurred while retrieving JournalFeedback  with Id: {Id} at {Time}", id, DateTime.UtcNow);
                throw;
            }
        }

        public async Task<IReadOnlyList<JournalFeedback>> GetJournalFeedbacksByManagerIdAsync(Guid managerId, CancellationToken cancellationToken, bool includeJournal = false)
        {
            _logger.LogInformation("Attempting to retrieve all JournalFeedbacks for ManagerId={ManagerId}, with includeDeleted={includeDeleted} at {Time}",
               managerId, includeJournal, DateTime.UtcNow);
            try
            {
                var query = _dbContext.JournalFeedbacks
                    .AsNoTracking()
                    .Where(x => x.FeedbackManagerId == managerId);

                if (includeJournal)
                    query = query.Include(x => x.JournalEntry);

                var entities = await query.OrderByDescending(t => t.CreatedAt)
                    .ToListAsync(cancellationToken);

                _logger.LogInformation("Successfully retrieved {Count} JournalFeedbacks for ManagerId={ManagerId}, with includeDeleted={includeDeleted} at {Time}",
                    entities.Count, managerId, includeJournal, DateTime.UtcNow);
                return entities;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception occurred while retrieving all JournalFeedbacks for ManagerId={ManagerId}, with includeDeleted={includeDeleted} at {Time}",
                    managerId, includeJournal, DateTime.UtcNow);
                throw;
            }
        }


        // check before insert
        // business rule - journal entry can have exctly one feedback
        public async Task<bool> IsFeedbackGiven(Guid journalId, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Checking if a journal-feedback already exits for jounral-entry with for id: {journalId} at {Time}",
                   journalId, DateTime.UtcNow);

                return await _dbContext.JournalFeedbacks.AnyAsync(x => x.JournalEntryId == journalId,
                                            cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception occurred while checking if a journal-feedback already exits for jounral-entry with for id: {journalId} at {Time}",
                   journalId, DateTime.UtcNow);
                throw;
            }
        }

        // assumes jounral does not have a feedback - this should be checked in service layer before calling this method
        // this method is expected crash if jounral already has a feedback
        public async Task<JournalFeedback?> AddAsync(JournalFeedback entity, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Attempting to add new JournalFeedback for Journal Id: {JournalId} at {Time}", entity.JournalEntryId, DateTime.UtcNow);
            try
            {
                await _dbContext.JournalFeedbacks.AddAsync(entity, cancellationToken);
                var result = await SaveChangesAsync(cancellationToken);

                if (result > 0)
                {
                    _logger.LogInformation("Successfully added JournalFeedback with Journal Id: {JournalId} at {Time}", entity.JournalEntryId, DateTime.UtcNow);
                    return entity;
                }

                _logger.LogWarning("No JournalEntry added for new JournalFeedback with Journal Id: {JournalId}  at {Time}", entity.JournalEntryId, DateTime.UtcNow);
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception occurred while adding JournalFeedback with Journal Id: {JournalId} at {Time}", entity.JournalEntryId, DateTime.UtcNow);
                throw;
            }
        }


        /// <summary>
        /// To be implemented in the next version of this API
        /// </summary>
        /// <param name="id"></param>
        /// <param name="entity"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public Task<bool> UpdateAsync(Guid id, JournalFeedback entity, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// To be implemented in the next version of this API
        /// </summary>
        /// <param name="id"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
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
