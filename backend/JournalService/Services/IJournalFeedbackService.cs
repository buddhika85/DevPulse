using JournalService.Application.Commands.JournalFeedback;
using JournalService.Application.Queries.JournalFeedback;
using SharedLib.DTOs.Journal;

namespace JournalService.Services
{
    public interface IJournalFeedbackService
    {
        Task<IReadOnlyList<JournalFeedbackDto>> GetJournalFeedbacksAsync(CancellationToken cancellationToken);
        Task<JournalFeedbackDto?> GetJournalFeedbackByIdAsync(GetJournalFeedbackByIdQuery query, CancellationToken cancellationToken);

        Task<IReadOnlyList<JournalFeedbackDto>> GetJournalFeedbacksByManagerIdAsync(GetJournalFeedbacksByManagerIdQuery query, CancellationToken cancellationToken);

        // business rule - journal entry can have exctly one feedback
        Task<bool> IsFeedbackGiven(IsFeedbackGivenQuery query, CancellationToken cancellationToken);

        // business rule - journal entry can have exctly one feedback
        Task<Guid?> AddJournalFeedbackAsync(AddJournalFeedbackCommand command, CancellationToken cancellationToken);
        Task<bool> JournalFeedbackSeenByUserAsync(JournalFeedbackSeenByUserCommand command, CancellationToken cancellationToken);
    }
}
