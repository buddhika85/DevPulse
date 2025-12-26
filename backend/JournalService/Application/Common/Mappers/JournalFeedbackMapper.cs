using JournalService.Domain.Entities;
using SharedLib.DTOs.Journal;

namespace JournalService.Application.Common.Mappers
{
    public static class JournalFeedbackMapper
    {
        public static JournalFeedbackDto ToDto(JournalFeedback entity)
        {
            return new JournalFeedbackDto(entity.Id, entity.JournalEntryId, entity.FeedbackManagerId, entity.CreatedAt, entity.Comment, entity.SeenByUser);
        }

        public static IEnumerable<JournalFeedbackDto> ToDtosList(IEnumerable<JournalFeedback> entities)
        {
            return entities.Select(ToDto);
        }
    }
}
