using MoodService.Application.Commands;
using MoodService.Application.Common.Mappers;
using MoodService.Application.Queries;
using MoodService.Domain.Entities;
using MoodService.Domain.ValueObjects;
using MoodService.Repositories;
using SharedLib.DTOs.Mood;

namespace MoodService.Services
{
    public class MoodService : IMoodService
    {
        private readonly IMoodRepository _moodRepository;
        private readonly ILogger<MoodService> _logger;

        public MoodService(IMoodRepository moodRepository, ILogger<MoodService> logger)
        {
            _moodRepository = moodRepository;
            _logger = logger;
        }

        public async Task<IReadOnlyList<MoodEntryDto>> GetAllMoodEntriesAsync(CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Attempting to retrieve all mood-entries at {Time}", DateTime.UtcNow);

                var entities = await _moodRepository.GetAllAsync(cancellationToken);
                _logger.LogInformation("Retrieved {MoodEntriesCount} mood-entries at {Time}", entities.Count, DateTime.UtcNow);

                var dtos = MoodEntryMapper.ToDtosList(entities);
                _logger.LogInformation("Mapped {MoodEntriesCount} mood-entries at {Time}", dtos.Count(), DateTime.UtcNow);

                return [.. dtos];
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception occurred while retrieving all mood-entries at {Time}", DateTime.UtcNow);
                throw;
            }
        }

        public async Task<IReadOnlyList<MoodEntryDto>> GetMoodEntriesByUserIdAsync(GetMoodEntriesByUserIdQuery query, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Attempting to retrieve all mood-entries by user: {UserId} at {Time}", query.UserId, DateTime.UtcNow);

                var entities = await _moodRepository.GetMoodEntriesByUserIdAsync(query.UserId, cancellationToken);
                _logger.LogInformation("Retrieved {MoodEntriesCount} mood-entries at {Time}", entities.Count, DateTime.UtcNow);

                var dtos = MoodEntryMapper.ToDtosList(entities);
                _logger.LogInformation("Mapped {MoodEntriesCount} mood-entries for user: {UserId} at {Time}", dtos.Count(), query.UserId, DateTime.UtcNow);

                return [.. dtos];
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception occurred while retrieving all mood-entries by user: {UserId} at {Time}", query.UserId, DateTime.UtcNow);
                throw;
            }
        }

        public async Task<MoodEntryDto?> GetMoodEntryByIdAsync(GetMoodEntryByIdQuery query, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Attempting to retrieve a mood-entry by Id: {MoodEntryId} at {Time}", query.Id, DateTime.UtcNow);

                var entity = await _moodRepository.GetByIdAsync(query.Id, cancellationToken);
                if (entity is not null)
                {
                    _logger.LogInformation("Retrieved a mood-entry by Id: {MoodEntryId} at {Time}", query.Id, DateTime.UtcNow);

                    var dto = MoodEntryMapper.ToDto(entity);
                    _logger.LogInformation("Mapped a mood-entry with Id to DTO: {MoodEntryId} at {Time}", entity.Id, DateTime.UtcNow);

                    return dto;
                }

                _logger.LogInformation("Count not find a mood-entry by Id: {MoodEntryId} at {Time}", query.Id, DateTime.UtcNow);
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception occurred while attempting to retrieve a mood-entry by Id: {MoodEntryId} at {Time}", query.Id, DateTime.UtcNow);
                throw;
            }
        }



        public async Task<Guid?> AddMoodEntryAsync(AddMoodEntryCommand command, CancellationToken cancellationToken)
        {
            var now = DateTime.UtcNow;

            try
            {
                _logger.LogInformation(
                    "Adding a mood-entry for user: {UserId}, Day: {Day}, Mood Time: {MoodTime}, Mood Level: {MoodLevel} at {Time}",
                    command.UserId, command.Day, command.MoodTime, command.MoodLevel, now);

                var day = (command.Day ?? DateTime.Today).Date;

                var moodTime = string.IsNullOrWhiteSpace(command.MoodTime)
                    ? MoodTime.MorningSession
                    : MoodTime.From(command.MoodTime);

                var moodLevel = string.IsNullOrWhiteSpace(command.MoodLevel)
                    ? MoodLevel.Neutral
                    : MoodLevel.From(command.MoodLevel);

                if (await IsMoodEntryExists(command.UserId, day, moodTime, cancellationToken))
                {
                    _logger.LogError(
                        "A mood-entry already exists for user: {UserId}, Day: {Day}, Mood Time: {MoodTime} at {Time}. Cannot add again.",
                        command.UserId, day, moodTime, now);

                    return null;
                }

                var moodEntry = MoodEntry.Create(command.UserId, day, moodTime, moodLevel, command.Note);

                var result = await _moodRepository.AddAsync(moodEntry, cancellationToken);

                if (result is not null)
                {
                    _logger.LogInformation(
                        "Mood-entry created for user: {UserId}, Day: {Day}, Mood Time: {MoodTime}, Mood Level: {MoodLevel} at {Time}",
                        command.UserId, command.Day, command.MoodTime, command.MoodLevel, now);

                    return result.Id;
                }

                _logger.LogWarning(
                    "Mood-entry creation failed for user: {UserId}, Day: {Day}, Mood Time: {MoodTime}, Mood Level: {MoodLevel} at {Time}",
                    command.UserId, command.Day, command.MoodTime, command.MoodLevel, now);

                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Error occurred while creating a mood-entry for user: {UserId}, Day: {Day}, Mood Time: {MoodTime}, Mood Level: {MoodLevel} at {Time}",
                    command.UserId, command.Day, command.MoodTime, command.MoodLevel, now);

                throw;
            }
        }

        public async Task<bool> DeleteMoodEntryAsync(DeleteMoodEntryCommand command, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Attempting to delete a mood-entry with Id:{MoodEntryId} at {Time}", command.Id, DateTime.UtcNow);

                var moodEntry = await _moodRepository.GetByIdAsync(command.Id, cancellationToken);
                if (moodEntry is null)
                {
                    _logger.LogError("No mood-entry with Id: {MoodEntryId} at {Time}. Deletion aborted.", command.Id, DateTime.UtcNow);
                    return false;
                }

                
                var result = await _moodRepository.DeleteAsync(moodEntry.Id, cancellationToken);
                if (result)
                {
                    moodEntry.Delete();     // For raising domain events
                    _logger.LogInformation("Successfully deleted a mood-entry with Id: {MoodEntryId} of user {UserId}, at {Time}", moodEntry.Id, moodEntry.UserId, DateTime.UtcNow);
                    return true;
                }

                _logger.LogWarning("Deleting User with Id: {MoodEntryId} of user {UserId}, at {Time} did not affect any records.", moodEntry.Id, moodEntry.UserId, DateTime.UtcNow);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception occurred while deleting {MoodEntryId} at {Time}", command.Id, DateTime.UtcNow);
                throw;
            }
        }

        public async Task<bool> UpdateMoodEntryAsync(UpdateMoodEntryCommand command, CancellationToken cancellationToken)
        {
            var now = DateTime.UtcNow;

            _logger.LogInformation("Updating mood entry with Id {Id} at {Time}", command.Id, now);

            try
            {
                var entity = await _moodRepository.GetByIdAsync(command.Id, cancellationToken);
                if (entity is null)
                {
                    _logger.LogWarning("Mood entry not found for update. Id: {Id} at {Time}", command.Id, now);
                    return false;
                }

                var updatedMoodTime = MoodTime.From(command.MoodTime);

                if (await FindOtherMoodEntry(entity.Id, entity.UserId, command.Day, updatedMoodTime, cancellationToken))
                {
                    _logger.LogWarning(
                        "Update aborted. Another mood entry already exists for user {UserId} on {Day} during {MoodTime}. Excluded entry Id: {ExcludeId} at {Time}",
                        entity.UserId, command.Day.Date, updatedMoodTime, entity.Id, now);

                    return false;
                }

                var updatedMoodLevel = MoodLevel.From(command.MoodLevel);       // update entity and raise domain events for side-effects

                // Apply domain update - update entity and raise domain events for side-effects
                entity.Update(command.Day, updatedMoodTime, updatedMoodLevel, command.Note);

                var result = await _moodRepository.UpdateAsync(command.Id, entity, cancellationToken);

                if (result)
                {
                    _logger.LogInformation(
                        "Successfully updated mood entry with Id {Id} for user {UserId} at {Time}",
                        entity.Id, entity.UserId, now);

                    return true;
                }

                _logger.LogWarning(
                    "Update operation for mood entry with Id {Id} for user {UserId} at {Time} did not affect any records",
                    entity.Id, entity.UserId, now);

                return true; // If this is intentional, keep it
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Exception occurred while updating mood entry with Id {Id} at {Time}",
                    command.Id, now);

                throw;
            }
        }

        #region Checks_Before_Insert

        // A user can have exactly 1 MoodEntry for given day and a given session
        public async Task<bool> IsMoodEntryExists(IsMoodEntryExistsCommand command, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Checking if a mood-entry with for user: {UserId}, Day: {Day}, Mood Time: {MoodTime} at {Time}",
                   command.UserId, command.Day.Date, command.MoodTime, DateTime.UtcNow);
            try
            {
                var moodTime = MoodTime.From(command.MoodTime);
                return await IsMoodEntryExists(command.UserId, command.Day.Date, moodTime, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while checking if a mood-entry with for user: {UserId}, Day: {Day}, Mood Time: {MoodTime} at {Time}",
                   command.UserId, command.Day.Date, command.MoodTime, DateTime.UtcNow);
                throw;
            }
        }


        private async Task<bool> IsMoodEntryExists(Guid userId, DateTime day, MoodTime session, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Checking if a mood-entry with for user: {UserId}, Day: {Day}, Mood Time: {MoodTime} at {Time}",
                   userId, day, session, DateTime.UtcNow);
            try
            {
                return await _moodRepository.IsMoodEntryExists(userId, day, session, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while checking if a mood-entry with for user: {UserId}, Day: {Day}, Mood Time: {MoodTime} at {Time}",
                   userId, day, session, DateTime.UtcNow);
                throw;
            }
        }

        #endregion

        #region Checks_Before_Update

        // Are there any other mood entreies with same Guid userId, DateTime day, MoodTime session except for excludeId ?
        // If so we cannot perform this update
        public async Task<bool> FindOtherMoodEntry(FindOtherMoodEntryCommand command , CancellationToken cancellationToken)
        {
            var now = DateTime.UtcNow;

            _logger.LogInformation(
                "Checking for another mood entry for user {UserId} on {Day} during {MoodTime}, excluding entry {ExcludeMoodId} at {Time}",
                command.UserId, command.Day, command.MoodTime, command.ExcludeId, now);

            try
            {
                return await FindOtherMoodEntry(command.ExcludeId, command.UserId, command.Day, MoodTime.From(command.MoodTime), cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Error occurred while checking for another mood entry for user {UserId} on {Day} during {MoodTime} at {Time}",
                    command.UserId, command.Day, command.MoodTime, now);

                throw;
            }
        }

        private async Task<bool> FindOtherMoodEntry(
                                                Guid excludeId,
                                                Guid userId,
                                                DateTime day,
                                                MoodTime session,
                                                CancellationToken cancellationToken)
        {
            var now = DateTime.UtcNow;

            _logger.LogInformation(
                "Checking for another mood entry for user {UserId} on {Day} during {MoodTime}, excluding entry {ExcludeMoodId} at {Time}",
                userId, day, session, excludeId, now);

            try
            {
                var other = await _moodRepository.FindOtherMoodEntry(
                    excludeId, userId, day, session, cancellationToken);

                if (other != null)
                {
                    _logger.LogInformation(
                        "Found another mood entry for user {UserId}: EntryId {MoodEntryId}, Day {Day}, Mood Time {MoodTime}, excluded entry {ExcludeMoodId} at {Time}",
                        other.UserId, other.Id, other.Day.Date, other.MoodTime, excludeId, now);
                }

                return other != null;
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Error occurred while checking for another mood entry for user {UserId} on {Day} during {MoodTime} at {Time}",
                    userId, day, session, now);

                throw;
            }
        }
        #endregion
    }
}
