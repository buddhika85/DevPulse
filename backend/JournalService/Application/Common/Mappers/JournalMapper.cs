using JournalService.Application.Commands.Journal;
using JournalService.Domain.Entities;
using SharedLib.DTOs.Journal;

namespace JournalService.Application.Common.Mappers
{
    public static class JournalMapper
    {
        public static void MapToEntity(JournalEntry entity, UpdateJournalEntryCommand command) 
            => entity.Update(command.Title, command.Content);

        public static JournalEntryDto ToDto(JournalEntry entity) 
            => new JournalEntryDto(entity.Id, entity.UserId, entity.CreatedAt, entity.Title, entity.Content, entity.IsDeleted, entity.JournalFeedbackId);

        public static IEnumerable<JournalEntryDto> ToDtosList(IEnumerable<JournalEntry> entities) 
            => entities.Select(ToDto);

        public static IEnumerable<JournalEntryWithFeedbackDto> ToDtosListWithFeedback(IEnumerable<JournalEntry> entities) 
            => entities.Select(ToDtoWithFeedback);

        private static JournalEntryWithFeedbackDto ToDtoWithFeedback(JournalEntry entity) 
            => new JournalEntryWithFeedbackDto(ToDto(entity), ToFeedbackDto(entity.JournalFeedback));

        private static JournalFeedbackDto? ToFeedbackDto(JournalFeedback? feedback) 
            => feedback != null ? 
            new JournalFeedbackDto(feedback.Id, feedback.JournalEntryId, feedback.FeedbackManagerId, feedback.CreatedAt, feedback.Comment, feedback.SeenByUser) 
            : null;
    }
}
