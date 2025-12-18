using MoodService.Application.Commands;
using MoodService.Domain.Entities;
using MoodService.Domain.ValueObjects;
using SharedLib.DTOs.Mood;

namespace MoodService.Application.Common.Mappers
{
    public static class MoodEntryMapper
    {
        public static void MapToEntity(MoodEntry entity, UpdateMoodEntryCommand command)
        {
            entity.Update(command.Day, MoodTime.From(command.MoodTime), MoodLevel.From(command.MoodLevel), command.Note);
        }

        public static MoodEntryDto ToDto(MoodEntry entity)
        {           
            return new MoodEntryDto(entity.Id, entity.Day, entity.MoodTime.ToString(), entity.MoodTime.TimeRange, 
                entity.MoodLevel.ToString(), entity.MoodLevel.Score, entity.Note, entity.CreatedAt, entity.UserId);
        }

        public static IEnumerable<MoodEntryDto> ToDtosList(IEnumerable<MoodEntry> entities)
        {
            return entities.Select(ToDto);
        }
    }
}
