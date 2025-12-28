using JournalService.Application.Commands.JournalFeedback;
using JournalService.Application.Common.Mappers;
using JournalService.Application.Queries.JournalFeedback;
using JournalService.Domain.Entities;
using JournalService.Repositories;
using SharedLib.DTOs.Journal;

namespace JournalService.Services
{
    public class JournalFeedbackService : IJournalFeedbackService
    {
        private readonly IJournalFeedbackRepository _journalFeedbackRepository;
        private readonly IJournalRepository _journalRepository;
        private readonly ILogger<JournalService> _logger;

        public JournalFeedbackService(IJournalFeedbackRepository journalFeedbackRepository, IJournalRepository journalRepository, ILogger<JournalService> logger)
        {
            _journalFeedbackRepository = journalFeedbackRepository;
            _journalRepository = journalRepository;
            _logger = logger;
        }
        public async Task<IReadOnlyList<JournalFeedbackDto>> GetJournalFeedbacksAsync(CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Attempting to retrieve all journal-feedbacks at {Time}", DateTime.UtcNow);

                var entities = await _journalFeedbackRepository.GetAllAsync(cancellationToken);
                _logger.LogInformation("Retrieved {JournalFeedbacksCount} journal-feedbacks at {Time}", entities.Count, DateTime.UtcNow);

                var dtos = JournalFeedbackMapper.ToDtosList(entities);
                _logger.LogInformation("Mapped {JournalFeedbacksCount} journal-feedbacks at {Time}", dtos.Count(), DateTime.UtcNow);

                return [.. dtos];
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception occurred while retrieving all journal-feedbacks at {Time}", DateTime.UtcNow);
                throw;
            }
        }

        public async Task<IReadOnlyList<JournalFeedbackDto>> GetJournalFeedbacksByManagerIdAsync(GetJournalFeedbacksByManagerIdQuery query, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Attempting to retrieve all journal-feedbacks By ManagerId:{ManagerId} at {Time}", query.ManagerId, DateTime.UtcNow);

                var entities = await _journalFeedbackRepository.GetJournalFeedbacksByManagerIdAsync(query.ManagerId, cancellationToken);
                _logger.LogInformation("Retrieved {JournalFeedbacksCount} journal-feedbacks By ManagerId:{ManagerId} at {Time}", entities.Count, query.ManagerId, DateTime.UtcNow);

                var dtos = JournalFeedbackMapper.ToDtosList(entities);
                _logger.LogInformation("Mapped {JournalFeedbacksCount} journal-feedbacks By ManagerId:{ManagerId} at {Time}", dtos.Count(), query.ManagerId, DateTime.UtcNow);

                return [.. dtos];
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception occurred while retrieving all journal-feedbacks By ManagerId:{ManagerId} at {Time}", query.ManagerId, DateTime.UtcNow);
                throw;
            }
        }

        public async Task<JournalFeedbackDto?> GetJournalFeedbackByIdAsync(GetJournalFeedbackByIdQuery query, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Attempting to retrieve a journal-feedback by id:{JournalFeedbackId} at {Time}", query.Id, DateTime.UtcNow);

                var entity = await _journalFeedbackRepository.GetByIdAsync(query.Id, cancellationToken);
                if (entity is null)
                {
                    _logger.LogInformation("Did not find a journal-feedback by id:{JournalFeedbackId} at {Time}", query.Id, DateTime.UtcNow);
                    return null;
                }

                _logger.LogInformation("Retrieved journal-feedbacks by id:{JournalFeedbackId} at {Time}", query.Id, DateTime.UtcNow);
                var dto = JournalFeedbackMapper.ToDto(entity);
                _logger.LogInformation("Mapped journal-feedback by id:{JournalFeedbackId} at {Time}", query.Id, DateTime.UtcNow);

                return dto;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception occurred while retrieving journal-feedback by id:{JournalFeedbackId} at {Time}", query.Id, DateTime.UtcNow);
                throw;
            }
        }

        public async Task<bool> IsFeedbackGiven(IsFeedbackGivenQuery query, CancellationToken cancellationToken)
        {
            var now = DateTime.UtcNow;
            _logger.LogInformation("Attempting to check if a journal-feedback already exist for jounral Id:{JournalId} at {Time}", query.JournalId, now);

            try
            {
                var isFeedbackExists = await _journalFeedbackRepository.IsFeedbackGiven(query.JournalId, cancellationToken);
                if (isFeedbackExists)
                    _logger.LogInformation("Journal Feedback already exists for journal Id: {JournalId} at {Time}", query.JournalId, now);
                else
                    _logger.LogInformation("Journal Feedback does not exists for journal Id: {JournalId} at {Time}", query.JournalId, now);
                return isFeedbackExists;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception occurred while checking if journal-feedback exists for jounral id:{JournalId} at {Time}", query.JournalId, DateTime.UtcNow);
                throw;
            }
        }

        public async Task<Guid?> AddJournalFeedbackAsync(AddJournalFeedbackCommand command, CancellationToken cancellationToken)
        {
            var now = DateTime.UtcNow;
            _logger.LogInformation("Attempting to add a journal-feedback for jounral Id:{JournalId} by manager Id: {FeedbackManagerId} at {Time}", 
                command.JounralEntryId, command.FeedbackManagerId, now);

            try
            {
                var isJournalExists = await _journalRepository.IsJournalEntryExistsByIdAsync(command.JounralEntryId, cancellationToken);
                if (!isJournalExists)
                {
                    _logger.LogError("Adding journal-feedback aborted !! as Journal does not exist with journal Id:{JournalId} at {Time}", command.JounralEntryId, now);
                    return null;
                }

                var isFeedbackExists = await _journalFeedbackRepository.IsFeedbackGiven(command.JounralEntryId, cancellationToken);
                if (isFeedbackExists)
                {
                    _logger.LogError("Adding journal-feedback aborted !! as Journal feedback already exist for journal Id:{JournalId} at {Time}", command.JounralEntryId, now);
                    return null;
                }

                var entity = JournalFeedback.Create(command.JounralEntryId, command.FeedbackManagerId, command.Comment);
                entity = await _journalFeedbackRepository.AddAsync(entity, cancellationToken);
                if (entity is not null)
                {
                    _logger.LogInformation("Added Journal Feedback with Id:{JournalFeedbackId} for journal Id:{JournalId} is successful at {Time}", entity.Id, command.JounralEntryId, now);
                    return entity.Id;
                }

                _logger.LogError("An error occured while saving a journal feedback for journal Id:{JournalId} at {Time}", command.JounralEntryId, now);
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception occurred while adding a journal feedback for jounral id:{JournalId} at {Time}", command.JounralEntryId, now);
                throw;
            }
        }

        public async Task<bool> JournalFeedbackSeenByUserAsync(JournalFeedbackSeenByUserCommand command, CancellationToken cancellationToken)
        {
            var now = DateTime.UtcNow;
            _logger.LogInformation("Attempting to mark a journal-feedback seened by user for jounral-feedback Id:{JournalId} at {Time}",
                command.JounralFeedbackId, now);

            try
            {
                var entity = await _journalFeedbackRepository.GetByIdAsync(command.JounralFeedbackId, cancellationToken);
                if (entity == null)
                {
                    _logger.LogError("Marking as seended for journal-feedback aborted !! as Journal feedback with Id: {JounralFeedbackId} record does not exist at {Time}", 
                        command.JounralFeedbackId, now);
                    return false;
                }                

                entity.MarkAsSeen();
                var result = await _journalFeedbackRepository.MarkAsSeenByAsync(entity, cancellationToken);
                if (result)
                {
                    _logger.LogInformation("Journal Feedback with Id:{JounralFeedbackId} marked as seened by user at {Time}", command.JounralFeedbackId, now);
                    return true;
                }

                _logger.LogError("Error - Journal Feedback with Id:{JounralFeedbackId} could not be marked as seened by user at {Time}", command.JounralFeedbackId, now);
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception occurred while marking a journal feedback with Id:{JounralFeedbackId} as seened by user at {Time}", command.JounralFeedbackId, now);
                throw;
            }
        }
    }
}
