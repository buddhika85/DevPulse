using JournalService.Application.Commands.Journal;
using JournalService.Application.Common.Mappers;
using JournalService.Application.Queries.Journal;
using JournalService.Domain.Entities;
using JournalService.Repositories;
using SharedLib.DTOs.Journal;

namespace JournalService.Services
{
    public class JournalService : IJournalService
    {
        private readonly IJournalRepository _journalRepository;
        private readonly ILogger<JournalService> _logger;

        public JournalService(IJournalRepository journalRepository, ILogger<JournalService> logger)
        {
            _journalRepository = journalRepository;
            _logger = logger;
        }

        public async Task<IReadOnlyList<JournalEntryDto>> GetAllJournalEntriesAsync(CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Attempting to retrieve all journal-entries at {Time}", DateTime.UtcNow);

                var entities = await _journalRepository.GetAllAsync(cancellationToken);
                _logger.LogInformation("Retrieved {JournalEntriesCount} journal-entries at {Time}", entities.Count, DateTime.UtcNow);

                var dtos = JournalMapper.ToDtosList(entities);
                _logger.LogInformation("Mapped {JournalEntriesCount} journal-entries at {Time}", dtos.Count(), DateTime.UtcNow);

                return [.. dtos];
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception occurred while retrieving all journal-entries at {Time}", DateTime.UtcNow);
                throw;
            }
        }

        public async Task<IReadOnlyList<JournalEntryDto>> GetJournalEntriesByUserIdAsync(GetJournalEntriesByUserIdQuery query, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Attempting to retrieve all journal-entries by user: {UserId} at {Time}", query.UserId, DateTime.UtcNow);

                var entities = await _journalRepository.GetJournalEntriesByUserIdAsync(query.UserId, cancellationToken);
                _logger.LogInformation("Retrieved {MoodEntriesCount} journal-entries at {Time}", entities.Count, DateTime.UtcNow);

                var dtos = JournalMapper.ToDtosList(entities);
                _logger.LogInformation("Mapped {JournalEntriesCount} journal-entries for user: {UserId} at {Time}", dtos.Count(), query.UserId, DateTime.UtcNow);

                return [.. dtos];
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception occurred while retrieving all journal-entries by user: {UserId} at {Time}", query.UserId, DateTime.UtcNow);
                throw;
            }
        }

        public async Task<JournalEntryDto> GetJournalEntryByIdAsync(GetJournalEntryByIdQuery query, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Attempting to retrieve a journal-entry by Id: {JournalEntryId} at {Time}", query.Id, DateTime.UtcNow);

                var entity = await _journalRepository.GetByIdAsync(query.Id, cancellationToken);
                if (entity is not null)
                {
                    _logger.LogInformation("Retrieved a journal-entry by Id: {JournalEntryId} at {Time}", query.Id, DateTime.UtcNow);

                    var dto = JournalMapper.ToDto(entity);
                    _logger.LogInformation("Mapped a journal-entry with Id to DTO: {JournalEntryId} at {Time}", entity.Id, DateTime.UtcNow);

                    return dto;
                }

                _logger.LogInformation("Count not find a journal-entry by Id: {JournalEntryId} at {Time}", query.Id, DateTime.UtcNow);
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception occurred while attempting to retrieve a journal-entry by Id: {JournalEntryId} at {Time}", query.Id, DateTime.UtcNow);
                throw;
            }
        }

        public async Task<bool> IsJournalEntryExistsByIdAsync(IsJournalEntryExistsByIdQuery query, CancellationToken cancellationToken)
        {
            var now = DateTime.UtcNow;
            _logger.LogInformation("Checking if a JournalEntry exists with Id: {Id} includeDeleted:{includeDeleted} at {Time}", query.Id, query.IncludedDeleted, now);
            try
            {
                var isExists = await _journalRepository.IsJournalEntryExistsByIdAsync(query.Id, cancellationToken, query.IncludedDeleted);

                if (isExists)
                    _logger.LogInformation("A JournalEntry found with Id: {Id} includeDeleted:{includeDeleted} at {Time}", query.Id, query.IncludedDeleted, now);
                else
                    _logger.LogInformation("No JournalEntry found with Id: {Id} includeDeleted:{includeDeleted} at {Time}", query.Id, query.IncludedDeleted, now);
                return isExists;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception occurred while checking if a JournalEntry exists Id: {Id} includeDeleted:{includeDeleted} at {Time}", 
                    query.Id, query.IncludedDeleted, now);
                throw;
            }
        }

        public async Task<Guid?> AddJournalEntryAsync(AddJournalEntryCommand command, CancellationToken cancellationToken)
        {
            var now = DateTime.UtcNow;

            try
            {
                _logger.LogInformation(
                    "Adding a journal-entry for user: {UserId}, Title: {Title}, Content: {Content} at {Time}",
                    command.UserId, command.Title, command.Content, now);

                var journalEntry = JournalEntry.Create(command.UserId, command.Title, command.Content);

                var result = await _journalRepository.AddAsync(journalEntry, cancellationToken);

                if (result is not null)
                {
                    _logger.LogInformation(
                        "Journal-entry with IdL:{JournalId} for user: {UserId}, Title: {Title}, Content: {Content} at {Time}",
                        result.Id, command.UserId, command.Title, command.Content, now);

                    return result.Id;
                }

                _logger.LogWarning(
                    "Journal-entry creation failed for user: {UserId}, Title: {Title}, Content: {Content} at {Time}",
                    command.UserId, command.Title, command.Content, now);

                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Error occurred while creating a journal-entry for user: {UserId}, Title: {Title}, Content: {Content} at {Time}",
                    command.UserId, command.Title, command.Content, now);

                throw;
            }
        }

        public async Task<bool> UpdateJournalEntryAsync(UpdateJournalEntryCommand command, CancellationToken cancellationToken)
        {
            var now = DateTime.UtcNow;

            _logger.LogInformation("Updating journal entry with Id {Id} at {Time}", command.JournalEntryId, now);

            try
            {
                var entity = await _journalRepository.GetByIdAsync(command.JournalEntryId, cancellationToken);
                if (entity is null)
                {
                    _logger.LogWarning("Journal entry not found for update. Id: {Id} at {Time}", command.JournalEntryId, now);
                    return false;
                }

                

                // Apply domain update - update entity and raise domain events for side-effects
                entity.Update(command.Title, command.Content);

                var result = await _journalRepository.UpdateAsync(command.JournalEntryId, entity, cancellationToken);

                if (result)
                {
                    _logger.LogInformation(
                        "Successfully updated journal entry with Id {Id} for user {UserId} at {Time}",
                        entity.Id, entity.UserId, now);

                    return true;
                }

                _logger.LogWarning(
                    "Update operation for journal entry with Id {Id} for user {UserId} at {Time} did not affect any records",
                    entity.Id, entity.UserId, now);

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Exception occurred while updating journal entry with Id {Id} at {Time}",
                    command.JournalEntryId, now);

                throw;
            }
        }

        public async Task<bool> DeleteAsync(DeleteJournalEntryCommand command, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Attempting to soft-delete a journal-entry with Id:{MoodEntryId} at {Time}", command.JournalEntryId, DateTime.UtcNow);

                var journalEntry = await _journalRepository.GetByIdAsync(command.JournalEntryId, cancellationToken);
                if (journalEntry is null)
                {
                    _logger.LogError("No journal-entry with Id: {JournalEntryId} at {Time}. Soft-Delete aborted.", command.JournalEntryId, DateTime.UtcNow);
                    return false;
                }


                var result = await _journalRepository.DeleteAsync(journalEntry.Id, cancellationToken);
                if (result)
                {
                    journalEntry.SoftDelete();     // For raising domain events
                    _logger.LogInformation("Successfully soft-deleted a journal-entry with Id: {JournalEntryId} of user {UserId}, at {Time}", journalEntry.Id, journalEntry.UserId, DateTime.UtcNow);
                    return true;
                }

                _logger.LogWarning("Soft-Deleting journal with Id: {JournalEntryId} of user {UserId}, at {Time} did not affect any records.", journalEntry.Id, journalEntry.UserId, DateTime.UtcNow);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception occurred while soft-deleting a journal with ID: {JournalEntryId} at {Time}", command.JournalEntryId, DateTime.UtcNow);
                throw;
            }
        }

        public async Task<bool> RestoreAsync(RestoreJournalEntryCommand command, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Attempting to restore a journal-entry with Id:{MoodEntryId} at {Time}", command.JournalEntryId, DateTime.UtcNow);

                var journalEntry = await _journalRepository.GetByIdAsync(command.JournalEntryId, cancellationToken);
                if (journalEntry is null)
                {
                    _logger.LogError("No journal-entry with Id: {JournalEntryId} at {Time}. Restore aborted.", command.JournalEntryId, DateTime.UtcNow);
                    return false;
                }


                var result = await _journalRepository.RestoreAsync(journalEntry.Id, cancellationToken);
                if (result)
                {
                    journalEntry.SoftDelete();     // For raising domain events
                    _logger.LogInformation("Successfully restored a journal-entry with Id: {JournalEntryId} of user {UserId}, at {Time}", journalEntry.Id, journalEntry.UserId, DateTime.UtcNow);
                    return true;
                }

                _logger.LogWarning("Restoring journal with Id: {JournalEntryId} of user {UserId}, at {Time} did not affect any records.", journalEntry.Id, journalEntry.UserId, DateTime.UtcNow);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception occurred while restoring a journal with ID: {JournalEntryId} at {Time}", command.JournalEntryId, DateTime.UtcNow);
                throw;
            }
        }
    }
}
