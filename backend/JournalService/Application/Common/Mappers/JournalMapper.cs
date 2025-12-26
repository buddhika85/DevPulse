using JournalService.Application.Commands.Journal;
using JournalService.Domain.Entities;
using SharedLib.DTOs.Journal;

namespace JournalService.Application.Common.Mappers
{
    public static class JournalMapper
    {
        public static void MapToEntity(JournalEntry entity, UpdateJournalEntryCommand command)
        {
            entity.Update(command.Title, command.Content);
        }

        public static JournalEntryDto ToDto(JournalEntry entity)
        {
            return new JournalEntryDto(entity.Id, entity.UserId, entity.CreatedAt, entity.Title, entity.Content, entity.IsDeleted, entity.JournalFeedbackId);
        }

        public static IEnumerable<JournalEntryDto> ToDtosList(IEnumerable<JournalEntry> entities)
        {
            return entities.Select(ToDto);
        }
    }
}
